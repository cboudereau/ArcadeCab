open System
open System.IO

module Option = 
    let nonEmpty x = if String.IsNullOrWhiteSpace x then None else Some x

module Stream = 
    let rec read buffer (stream:IO.Stream) = 
        seq { 
            let reads = stream.Read(buffer, 0, buffer.Length)
            yield reads, buffer
            if reads = buffer.Length then yield! read buffer stream
        }            

module Result =
    let map2 f x y = x |> Result.bind(fun x' -> y |> Result.map (fun y' -> f x' y'))

module String =
    let levenshtein word1 word2 =
        let preprocess = fun (str : string) -> str.ToLower().ToCharArray()
        let chars1, chars2 = preprocess word1, preprocess word2
        let m, n = chars1.Length, chars2.Length
        let table : int[,] = Array2D.zeroCreate (m + 1) (n + 1)
        for i in 0..m do
            for j in 0..n do
                match i, j with
                | i, 0 -> table.[i, j] <- i
                | 0, j -> table.[i, j] <- j
                | _, _ ->
                    let delete = table.[i-1, j] + 1
                    let insert = table.[i, j-1] + 1
                    //cost of substitution is 2
                    let substitute = 
                        if chars1.[i - 1] = chars2.[j - 1] 
                            then table.[i-1, j-1] //same character
                            else table.[i-1, j-1] + 2
                    table.[i, j] <- List.min [delete; insert; substitute]
        table.[m, n]

    module Operators = 
        let (=^) x y = (x:string).Equals(y, StringComparison.InvariantCultureIgnoreCase)

module Mame = 
    open FSharp.Data

    type ListXml = XmlProvider< """MAME-listxml-sample.xml""" >
    
    let tryGetMachine path = 
        let full = ListXml.Load(path:string)
        let data = 
            full.Machines
            |> Seq.collect(fun x -> x.Roms |> Seq.choose (fun r -> r.Crc.Value |> Option.nonEmpty) |> Seq.map (fun c -> c, x))
            |> dict
        fun crc ->
            match data.TryGetValue(crc) with
            | true, x -> Some x
            | _ -> None

    let gameNames (x:ListXml.Machine) = 
        let roms = x.Roms |> Array.choose (fun x -> x.Name.Value |> Option.nonEmpty) |> set
        match x.Cloneof.Value |> Option.nonEmpty with
        | Some clone -> roms |> Set.add clone
        | None -> roms 

module EmuMovies = 
    open FSharp.Data

    type private LoginResponse = XmlProvider< """
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

    type private SystemResponse = XmlProvider< """
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

    type private MediaResponse = XmlProvider< """
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

    type private SearchbulkResponse = XmlProvider< """
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

module EmuFile =
    let prune x = 
        let symbols = 
            [","
             "("
             ")"]

        let words = 
            ["europe"
             "usa"
             "us"
             "en"
             "fr"
             "de"
             "france"
             "rev"]

        let remove oldValue x = (x:string).Replace((oldValue :string), "") 
        let replace oldValue newValue x = (x:string).Replace((oldValue:string), newValue)

        let s = 
            words 
            |> List.fold (fun s w -> 
                symbols 
                |> List.fold(fun s2 symbol -> 
                    s2 
                    |> remove (symbol+w) 
                    |> remove (w+symbol) ) s
                ) ((x:string).ToLower() |> replace ", " ",")
        symbols |> List.fold (fun s x -> remove x s) s

module Crc32 =
  let defaultPolynomial = 0xedb88320u
  let defaultSeed       = 0xFFffFFffu
  let table             =
    let inline nextValue acc =
      if 0u <> (acc &&& 1u) then defaultPolynomial ^^^ (acc >>> 1) else acc >>> 1
    let rec iter k acc =
      if k = 0 then acc else iter (k-1) (nextValue acc)
    [| 0u .. 255u |] |> Array.map (iter 8)
  
  let compute bytes = 
    let inline f acc (x:byte) =
      table.[int32 ((acc ^^^ (uint32 x)) &&& 0xffu)] ^^^ (acc >>> 8)
    bytes 
    |> Seq.fold (fun acc (l, x) -> 
        if l = (x |> Array.length) then x |> Array.fold f acc
        else x |> Seq.take l |> Seq.fold f acc) defaultSeed 
    |> (^^^) defaultSeed

module HyperSpin = 

    open FSharp.Data
    type Database = XmlProvider<"HyperSpinDatabase.xml">

    let header (now:DateTime) =
        Database.Header(
            listname="", 
            lastlistupdate=(now.ToString("yyyy/MM/dd")), 
            listversion="1.0", 
            exporterversion="0.1")

    let relativeName dir (x:FileInfo) =
        let x' = x.FullName.Substring(0, x.FullName.Length - x.Extension.Length)
        let dirLength = 
            let l = (dir:string).Length
            if dir.EndsWith(@"\") then l else l + 1
        x'.Substring(dirLength, x'.Length - dirLength)

open String.Operators
let files directory extensions = (directory:DirectoryInfo).EnumerateFiles("*", SearchOption.AllDirectories) |> Seq.filter(fun x -> extensions |> Array.exists ((=^)x.Extension))

module Matching = 
    
    type Command = 
        { XmlPath : FileInfo
          Dirs : DirectoryInfo list
          Interactive: bool 
          Threshold:int }

    let cmd xmlPath dirs interactive threshold = { XmlPath=xmlPath; Dirs=dirs; Interactive=interactive; Threshold=threshold }
    
    let private match' x y = 
        let x' = EmuFile.prune x
        let y' = EmuFile.prune y
        let l = max x'.Length y'.Length |> decimal
        let distance = String.levenshtein x' y' |> decimal
        (l - distance) / l * 100m
    
    let patch (cmd:Command) = 
        let games = HyperSpin.Database.Load(cmd.XmlPath.FullName).Games |> Array.toList
        let files = 
            cmd.Dirs 
            |> Seq.map (fun d -> d.FullName, d.GetFiles())
            |> Map.ofSeq
        
        let found =
            games
            |> Seq.collect(fun g ->
                let name = g.Name
                let cloneOf = g.Cloneof
                cmd.Dirs
                |> Seq.choose(fun d ->
                    let notFoundByName = d.GetFiles(name+".*") |> Array.isEmpty
                    let notFoundByClone = 
                        cloneOf 
                        |> Option.map (fun c -> d.GetFiles(c+".*") |> Array.isEmpty)
                        |> Option.defaultValue false
                    
                    if notFoundByClone && notFoundByName then
                        let candidate =
                            files 
                            |> Map.find d.FullName
                            |> Seq.map(fun file -> 
                                let fileName = Path.GetFileNameWithoutExtension(file.FullName)
                                match' name fileName, file)
                            |> Seq.sortByDescending fst
                            |> Seq.tryHead
                        Some ((d,g), candidate)
                    else None ) )

        let (solved, problems) = 
            found
            |> Seq.choose(fun ((dir, game), solution) -> 
                match solution with
                | None ->
                    if cmd.Interactive then
                        printfn "/!\ no solution found for %s" game.Name
                        Some (0, 1)
                    else
                        printfn "N, -, %s, -, %s" game.Name dir.FullName
                        Some (0, 1)
                | Some (matching, file) ->

                    let dest = Path.Combine(file.Directory.FullName, game.Name+file.Extension)
                    if File.Exists dest then None
                    else 
                        let copy () = file.CopyTo(dest, false)

                        if cmd.Interactive then
                            printfn "\r\ncopy file [%.2f%%] ? [yN]\r\n%s -> \r\n%s (%s) " matching game.Name file.Name file.FullName
                            
                            let r = 
                                match Console.ReadKey(true).KeyChar with
                                | 'y' ->
                                    printfn "copying..."
                                    copy () |> ignore
                                    1, 1
                                | _ -> 
                                    //printfn "[%i] %s -> %s skipped" rank game.Name file.FullName
                                    0, 1

                            printfn "finding next orphan..."
                            r
                        else
                            if matching >= decimal cmd.Threshold then 
                                let dest = copy()
                                printfn "Y, %.2f%%, %s, %s, %s" matching file.Name dest.Name dir.FullName
                                1, 1
                            else
                                printfn "N, %.2f%%, %s, %s, %s" matching game.Name file.Name dir.FullName
                                0, 1
                        |> Some)
            |> Seq.fold (fun (sx, sy) (x, y) -> sx + x, sy + y) (0, 0)

        sprintf "Solved %i/%i orphan games" solved problems

module Renaming = 
    type Command = 
        { XmlPath : FileInfo
          RomDir : DirectoryInfo
          Extensions: string []
          MiscDirs : DirectoryInfo list
          Purge : bool }

    let cmd xmlPath romDir ext miscDirs purge = { XmlPath=xmlPath; Extensions=ext; RomDir=romDir; MiscDirs=miscDirs; Purge=purge }
    
    let rename (cmd:Command) = 
        let database = HyperSpin.Database.Load(cmd.XmlPath.FullName)
        let crcDatabase = 
            database.Games 
            |> Array.choose(fun x -> x.Crc |> Option.map (fun y -> y.ToUpperInvariant(), x)) 
            |> Array.groupBy fst
            |> Array.map(fun (k, l) -> k, l |> Array.map snd)
            |> Map.ofArray
        
        let roms = files cmd.RomDir cmd.Extensions
        
        let buffer = Array.zeroCreate 81920
        
        let crc x = 
            use stream = File.OpenRead(x)
            let bytes = Stream.read buffer stream
            let crc = Crc32.compute bytes
            Convert.ToString(int crc, 16).ToUpperInvariant()
        
        let header = "FilePath, NewFilePath, FileCrc, Found"
        let rename (dirs:DirectoryInfo list) filecrc (x:FileInfo) alias name = 
            let renameFile fileCrc newName (x:FileInfo) =
                let newFilePath = 
                    let dest = Path.Combine(x.DirectoryName, newName + x.Extension)
                    if File.Exists dest |> not then x.MoveTo dest
                    dest
                sprintf "%s, %s, %s, %b" x.FullName newFilePath fileCrc x.Exists

            let renameFiles (dir:DirectoryInfo) oldName alias newName = 
                let files = dir.GetFiles(oldName+".*")
                let aliasFiles = alias |> Array.collect(fun a -> dir.GetFiles(a+".*"))
                files
                |> Array.append aliasFiles
                |> Array.map(renameFile "" newName)
                |> Array.toList
            
            let oldName = Path.GetFileNameWithoutExtension(x.FullName)
            
            (renameFile filecrc name x) :: (dirs |> List.collect(fun d -> renameFiles d oldName alias name))
            |> String.concat Environment.NewLine

        let notFound filecrc filePath = 
            sprintf "%s, , %s, %b" filePath filecrc false
        
        let result =
            roms
            |> Seq.map(fun x -> 
                printfn "processing %s" x.FullName
                let filecrc = crc x.FullName
                crcDatabase |> Map.tryFind filecrc
                |> Option.map (fun l -> 
                    let games = l |> Array.sortBy(fun x -> x.Name.Length)
                    let g = Array.head games
                    let alias = games |> Array.tail |> Array.map (fun x -> x.Name) 
                    Some g, g.Name |> rename cmd.MiscDirs filecrc x alias)
                |> Option.defaultWith (fun () -> 
                    if cmd.Purge then x.Delete()
                    None, notFound filecrc x.FullName))
            |> Seq.toArray
        
        let values = result |> Array.map snd |> String.concat Environment.NewLine
        
        if cmd.Purge then
            let header = HyperSpin.Database.Header(database.Header.Listname, database.Header.Lastlistupdate, database.Header.Listversion, database.Header.Exporterversion)
            let db = HyperSpin.Database.Menu(header, result |> Array.choose fst)
            db.XElement.Save(cmd.XmlPath.FullName)

        sprintf "Rename command finished : %s%s%s%s" Environment.NewLine header Environment.NewLine values

module Syncing = 
    type SyncCommand = 
        { RomDir : DirectoryInfo
          XmlPath : FileInfo
          Extensions: string []
          Purge : bool }

    let cmd romDir xmlPath ext purge = { RomDir = romDir; XmlPath = xmlPath; Extensions = ext; Purge = purge }

    let syncRom now (cmd:SyncCommand) = 
        let roms = files cmd.RomDir cmd.Extensions

        let (header, games) = 
            if cmd.XmlPath.Exists then 
                let database = HyperSpin.Database.Load(cmd.XmlPath.FullName) 
                database.Header,
                database.Games
                |> Array.map (fun x -> x.Name, x) 
                |> Map.ofArray
            else (HyperSpin.header now), Map.empty
            
        let newGames = 
            roms
            |> Seq.choose(fun r -> 
                let name = HyperSpin.relativeName cmd.RomDir.FullName r
                let description = Path.GetFileName name
                match games |> Map.tryFind name with
                | Some g -> Some g
                | None -> 
                    if cmd.Purge then 
                        r.Delete()
                        None
                    else HyperSpin.Database.Game(name=name,description=description,manufacturer=description,year=now.Year,cloneof=None,genre="",rating="",crc=None) |> Some )
            |> Seq.toArray

        HyperSpin.Database.Menu(header, newGames).XElement.Save(cmd.XmlPath.FullName)
        
        sprintf "Xml Database saved to %s" cmd.XmlPath.FullName

module Scanning = 
    type ScanCommand = 
        { XmlPath : FileInfo
          Directory : DirectoryInfo }

    let cmd xmlPath directory = { XmlPath=xmlPath; Directory=directory }

    let scanDir (cmd:ScanCommand) = 
        let targetFiles = cmd.Directory.GetFiles("*", SearchOption.AllDirectories) |> Array.map (HyperSpin.relativeName cmd.Directory.FullName)
    
        let database = HyperSpin.Database.Load(cmd.XmlPath.FullName)
    
        let missings =
            database.Games
            |> Array.filter (fun game -> 
                targetFiles 
                |> Array.exists (fun t -> 
                    let cloneof = game.Cloneof |> Option.bind Option.nonEmpty 
                    let name = game.Name |> Option.nonEmpty
                    let targetName = Some t
                    cloneof = targetName || name = targetName)
                |> not
            )
    
        match missings with
        | [||] -> "No missing files!"
        | m ->  
            let headerName = sprintf "Name, CloneOf%s" Environment.NewLine
            let values = m |> Array.map (fun x -> sprintf "\"%s\",\"%s\"" x.Name (x.Cloneof |> Option.defaultValue "")) |> String.concat Environment.NewLine
            
            sprintf "Missing files found :%s%s%s" Environment.NewLine headerName values
        
let tryDir = 
    DirectoryInfo 
    >> fun x -> 
        if x.Exists then Ok x 
        else sprintf "%s directory does not exist" x.FullName |> Error 

let tryDirs x = 
    (x:string).Split([|","|], StringSplitOptions.RemoveEmptyEntries)
    |> Array.map tryDir
    |> Array.fold(fun l x ->
        match l, x with 
        | Error t, Error h -> Error (h :: t)
        | Ok t, Ok h -> Ok (h :: t)
        | Error e, _ -> Error e 
        | _, Error e -> Error [e]
        ) (Ok [])
    |> Result.mapError(String.concat Environment.NewLine)

let tryFile = 
    FileInfo
    >> fun x -> 
        if x.Exists then Ok x 
        else sprintf "%s file does not exist" x.FullName |> Error 

let tryExt x = 
    (x:string).Split([|","|], StringSplitOptions.RemoveEmptyEntries)
    |> fun y -> if y |> Array.isEmpty then Error "no extension supplied" else Ok y

let tryInt x = 
    match Int32.TryParse x with
    | true, i -> Ok i
    | _ -> x |> sprintf "expected an integer got %s" |> Error

type CommandError = 
    | Invalid of string
    | NotAvailable 

let (<!>) f x = Result.map f x
let (<*>) f x = f |> Result.bind (fun f' -> x |> Result.map (fun x' -> f' x'))
let (>>=) x f = Result.bind f x
let (>=>) f g = f >> Result.bind g

let (<|>) f g = 
    fun x -> 
        match f x with
        | Ok x' -> Ok x'
        | Error (Invalid e) -> Error (Invalid e)
        | Error NotAvailable -> g x 

let outConsole = function 
    | Ok x -> 
        sprintf "Command finished : %s" x
        |> System.Console.WriteLine
        0
    | Error NotAvailable -> 
        "Commands are : /rename, /sync,  /match, /scan"
        |> System.Console.Error.WriteLine
        -1
    | Error (Invalid e) ->
        sprintf "Command failed : %s" e
        |> System.Console.Error.WriteLine
        -2

let matchCommandArgs = Array.toList >> function
    | "/match" :: options ->
      match options with
      | "/xmlPath" :: xmlPath
        :: "/dirs" :: dirs
        :: t ->
          let cmd = 
              Matching.cmd
              <!> (tryFile xmlPath |> Result.mapError Invalid)
              <*> (tryDirs dirs |> Result.mapError Invalid)

          match t with
          | "/nointeractive" :: option -> 
              match option with
              | "/threshold" :: threshold :: _ ->
                  cmd 
                  <*> (Ok false)
                  <*> (tryInt threshold |> Result.mapError Invalid)
              | _ -> cmd <*> (Ok false) <*> (Ok 100)
          | _ -> cmd <*> (Ok true) <*> (Ok 100)
      | _ -> Error (Invalid "invalid match command arguments, expected /match /xmlPath <XmlPath> /dirs \"<Dir1>, <Dir2>\" (/nointeractive) (/threshold <[0-100]>)")
    | _ -> Error NotAvailable

let syncCommandArgs = Array.toList >> function
    | "/sync" :: options ->
        match options with
        | "/romDir" :: romDir 
          :: "/xmlPath" :: xmlPath
          :: "/ext" ::     ext
          :: t -> 
            let cmd = 
                Syncing.cmd 
                <!> (tryDir romDir |> Result.mapError Invalid)
                <*> (FileInfo xmlPath |> Ok)
                <*> (tryExt ext |> Result.mapError Invalid)
            match t with
            | "/purge" :: _ -> cmd <*> Ok true
            | _ -> cmd <*> Ok false
        | _ -> Error (Invalid "invalid sync command arguments, expected /sync /romDir <RomDir> /xmlPath <XmlPath> /ext <.ext1, .ext2>")
    | _ -> Error NotAvailable

let scanCommandArgs = Array.toList >> function
    | "/scan" :: options ->
        match options with
        | "/xmlPath" :: xmlPath
          :: "/dir" ::  directory :: _ ->
            Scanning.cmd
            <!> (tryFile xmlPath |> Result.mapError Invalid)
            <*> (tryDir directory |> Result.mapError Invalid)
        | _ -> Error (Invalid "invalid scan command arguments, expected /scan /xmlPath <XmlPath> /dir <TargetDir>")
    | _ -> Error NotAvailable


let renameCommandArgs = Array.toList >> function
    | "/rename" :: options ->
        match options with
        | "/romDir" :: romDir
          :: "/xmlPath" :: xmlPath
          :: "/ext" :: ext
          :: "/miscDirs" :: miscDirs :: t -> 
                let cmd = 
                    Renaming.cmd 
                    <!> (tryFile xmlPath |> Result.mapError Invalid)
                    <*> (tryDir romDir |> Result.mapError Invalid)
                    <*> (tryExt ext |> Result.mapError Invalid)
                    <*> (tryDirs miscDirs |> Result.mapError Invalid)
                match t with
                | "/purge" :: _ -> cmd <*> Ok true
                | _ -> cmd <*> Ok false
        | _ -> Error (Invalid "invalid rename command arguments, expected /rename /romDir <RomDir> /xmlPath <XmlPath> /ext <.ext1, .ext2> /miscDirs \"<Dir1>, <Dir2>\" (/purge)")
    | _ -> Error NotAvailable

[<EntryPoint>]
let main argsv = 
    let syncRom args = Syncing.syncRom DateTime.Now <!> syncCommandArgs args
    let scanDir args = Scanning.scanDir <!> scanCommandArgs args
    let rename args = Renaming.rename <!> renameCommandArgs args
    let match' args = Matching.patch <!> matchCommandArgs args
    let command = 
        syncRom 
        <|> scanDir 
        <|> rename 
        <|> match'
    
    argsv
    |> command
    |> outConsole 