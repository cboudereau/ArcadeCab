#r "../packages/FSharp.Data.3.1.1/lib/net45/FSharp.Data.dll"

#r "System.Xml.Linq"

open FSharp.Data

module Result = 
    let toOption = function Ok x -> Some x | Error _ -> None

type LoginResponse = XmlProvider< """
    <samples>
        <Results> 
            <Result Success="True" Session="B6B98D98F6D86E2598AE8483" TimeTaken="1.75 seconds" UserType="2" />
        </Results>
        <Results>
        	<Result Success="False" Session="XXX" Error="True" MSG="Invalid username or password." TimeTaken="1.531 seconds" />
        </Results>
    </samples>""", SampleIsList=true >

type [<Struct>] SessionId = SessionId of string

let login user password = 
    let form = 
        [ "user", user
          "api", password
          "product", "2CDEF11B2D4BDA415532C494B88F5E5FE78C" ]
    
    Http.RequestString("https://api.gamesdbase.com/login.aspx", httpMethod="POST", body=HttpRequestBody.FormValues form)
    |> LoginResponse.Parse
    |> fun x -> 
        if x.Result.Success then x.Result.Session |> SessionId |> Ok
        else 
            x.Result.Msg 
            |> Option.defaultValue "no error message supplied by EmuMovies"
            |> Error

type SystemResponse = XmlProvider< """
    <Systems>
        <System Name = "AAE" Maker = "Various" Lookup = "AAE" Media = "Advert,Cabinet,CP,Marquee,PCB,Snap,Title"  MediaUpdated = "x20161209,20161209,20161209,20161209,20161209,20180225,20171214" GameExName = "AAE" />
        <System Name = "Aamber Pegasus" Maker = "Aamber" Lookup = "Aamber_Pegasus" Media = "Snap,System_Logo,Title"  MediaUpdated = "x20171227,20161209,20171227" GameExName = "Aamber Pegasus" />
        <System Name = "Acorn Archimedes" Maker = "Acorn" Lookup = "Acorn_Archimedes" Media = "Snap,System_Logo,Title"  MediaUpdated = "x20161209,20161209,20161209" GameExName = "Acorn Archimedes" />
        <System Name = "Acorn Atom" Maker = "Acorn" Lookup = "Acorn_Atom" Media = "Box,Box_3D,Cart_3D,Logos,Snap,System_Logo,Title"  MediaUpdated = "x20161209,20161209,20170614,20170614,20161209,20161209,20161209" GameExName = "Acorn Atom" />
        <System Name = "Acorn BBC Micro" Maker = "Acorn" Lookup = "Acorn_BBC_Micro" Media = "Logos,Snap,System_Logo"  MediaUpdated = "x20180523,20161209,20161209" GameExName = "Acorn BBC Micro" />
        <System Name = "Acorn Electron" Maker = "Acorn" Lookup = "Acorn_Electron" Media = "Advert,Box,Logos,Snap,System_Logo,Title"  MediaUpdated = "x20170606,20170606,20180523,20161209,20161209,20161209" GameExName = "Acorn Electron" />
    </Systems>
""" >

type [<Struct>] MediaName = MediaName of string 

open System

type [<Struct>] SystemName = SystemName of string
type System = 
    { Name : SystemName
      Label : string
      Maker : string
      Lookup : string
      MediaNames : MediaName Set
      Updates : DateTime Set }

let unwrap x = 
    let rpos = (x:string).IndexOf("<Results>")
    if rpos > 0 then x.Substring(0, rpos)
    else x

let split x = (x:string).Split([|','|], System.StringSplitOptions.RemoveEmptyEntries)


let getSystems (SessionId sid) = 
    Http.RequestString("https://api.gamesdbase.com/getsystems.aspx", query = [ "sessionid", sid ])
    |> unwrap
    |> SystemResponse.Parse
    |> fun x -> 
        let date x = DateTime.ParseExact(x, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
        x.Systems 
        |> Array.map(fun y -> 
            { Name = SystemName y.Name
              Label = y.GameExName
              Maker = y.Maker
              Lookup = y.Lookup 
              MediaNames = y.Media |> split |> Array.map MediaName |> set
              Updates = y.MediaUpdated |> split |> Array.map date |> set })

type MediaResponse = XmlProvider< """
<Medias>
	<Media Name = "Advert" Extensions = "jpg,png" />
	<Media Name = "Artwork" Extensions = "zip" />
	<Media Name = "Artwork_Preview" Extensions = "jpg,png" />
</Medias>
""" >

type Media = 
    { Name : MediaName
      Extensions : string Set }

let getMedias (SessionId sid) = 
    Http.RequestString("https://api.gamesdbase.com/getmedias.aspx", query = [ "sessionid", sid ])
    |> unwrap
    |> MediaResponse.Parse
    |> fun x ->
        x.Medias
        |> Array.map (fun y -> 
            { Name = MediaName y.Name
              Extensions = y.Extensions |> split |> set } )

type SearchbulkResponse = XmlProvider< """
<Results>
    <Result Found="True" URL="http://api.gamesdbase.com/c834c35fc66135c832c82f2bc86137c8616032c363c8616036ffddc9fb57c85f755e71c55b6bc85c6cc86d.png" search="pacman" CRC="2fad5688" Size="1445976"/>
	<Result Found="True" URL="http://api.gamesdbase.com/6234c35fc66135c832c82f2bc86137c8616032c363c8616036ffddc9fb57c85f755e71c55b6bc75c6a68c869.png" search="puckman" CRC="20096c1a" Size="1380321"/>
</Results>
""" >

type SearchResult = 
    { Url : string
      Name : string 
      Crc : string
      Size : int }

let searchbulk (SessionId sid) (SystemName systemName) (MediaName mediaName) fileNames = 
    let query = 
        [ "sessionid", sid
          "system", systemName
          "media", mediaName ] 
    
    let form = 
        [ "FileNames", (String.concat "\r\n" fileNames) ] 
    
    Http.RequestString("https://api.gamesdbase.com/searchbulk.aspx", query = query, httpMethod="POST", body = HttpRequestBody.FormValues form)
    |> SearchbulkResponse.Parse
    |> fun x -> 
        x.Results
        |> Array.map (fun y ->
            { Url = y.Url
              Name = y.Search
              Crc = y.Crc
              Size = y.Size } )

login "" "" |> Result.map (fun x -> searchbulk x (SystemName "MAME") (MediaName "HQ") ["pacman";"puckman";"Pac-Man"] )
login "" "" |> Result.map getMedias
login "" "" |> Result.map getSystems

