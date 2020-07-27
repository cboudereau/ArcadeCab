<#
.SYNOPSIS
   Resize an image

   Add-Type -AssemblyName 'System.Drawing'

.DESCRIPTION
   Resize an image based on a new given height or width or a single dimension and a maintain ratio flag. 
   The execution of this CmdLet creates a new file named "OriginalName_resized" and maintains the original
   file extension
.PARAMETER Width
   The new width of the image. Can be given alone with the MaintainRatio flag
.PARAMETER Height
   The new height of the image. Can be given alone with the MaintainRatio flag
.PARAMETER ImagePath
   The path to the image being resized
.PARAMETER MaintainRatio
   Maintain the ratio of the image by setting either width or height. Setting both width and height and also this parameter
   results in an error
.PARAMETER Percentage
   Resize the image *to* the size given in this parameter. It's imperative to know that this does not resize by the percentage but to the percentage of
   the image.
.PARAMETER SmoothingMode
   Sets the smoothing mode. Default is HighQuality.
.PARAMETER InterpolationMode
   Sets the interpolation mode. Default is HighQualityBicubic.
.PARAMETER PixelOffsetMode
   Sets the pixel offset mode. Default is HighQuality.
.EXAMPLE
   Resize-Image -Height 45 -Width 45 -ImagePath "Path/to/image.jpg"
.EXAMPLE
   Resize-Image -Height 45 -MaintainRatio -ImagePath "Path/to/image.jpg"
.EXAMPLE
   #Resize to 50% of the given image
   Resize-Image -Percentage 50 -ImagePath "Path/to/image.jpg"
.NOTES
   Written By: 
   Christopher Walker
#>
Function Resize-Image() {
    [CmdLetBinding(
        SupportsShouldProcess=$true, 
        PositionalBinding=$false,
        ConfirmImpact="Medium",
        DefaultParameterSetName="Absolute"
    )]
    Param (
        [Parameter(Mandatory=$True, ValueFromPipelineByPropertyName)]
        [Alias('FullName')]
        [String]$ImagePath,
        [Parameter(Mandatory=$False, ParameterSetName="Absolute")][Int]$Height,
        [Parameter(Mandatory=$False, ParameterSetName="Absolute")][Int]$Width,
        [Parameter(Mandatory=$False)][Switch]$Percent,
        [Parameter(Mandatory=$False)][System.Drawing.Drawing2D.SmoothingMode]$SmoothingMode = "HighQuality",
        [Parameter(Mandatory=$False)][System.Drawing.Drawing2D.InterpolationMode]$InterpolationMode = "HighQualityBicubic",
        [Parameter(Mandatory=$False)][System.Drawing.Drawing2D.PixelOffsetMode]$PixelOffsetMode = "HighQuality",
        [Parameter(Mandatory=$False)][String]$NameModifier = "_resized"
    )
    Begin {
     }
    Process {
        $Path = (Resolve-Path $ImagePath).Path
        
        Write-Information "Processing $Path"

        $Dot = $Path.LastIndexOf(".")

        #Add name modifier (OriginalName_{$NameModifier}.jpg)
        $OutputPath = $Path.Substring(0,$Dot) + $NameModifier + $Path.Substring($Dot,$Path.Length - $Dot)
        $TempPath = $Path.Substring(0,$Dot) + "_temp" + $Path.Substring($Dot,$Path.Length - $Dot)
            
        #Move old image
        Move-Item -Path $Path -Destination $TempPath

        $OldImage = New-Object -TypeName System.Drawing.Bitmap -ArgumentList $TempPath
        # Grab these for use in calculations below. 
        $OldHeight = $OldImage.Height
        $OldWidth = $OldImage.Width
 
        if ($Percent) {
            [int]$NewHeight = $OldHeight * $Height / 100
            [int]$NewWidth = $OldWidth * $Width / 100
        }
        else {
            [int]$NewHeight = $Height
            [int]$NewWidth = $Width
        }

        $Bitmap = New-Object -TypeName System.Drawing.Bitmap -ArgumentList $NewWidth, $NewHeight
        $NewImage = [System.Drawing.Graphics]::FromImage($Bitmap)
             
        #Retrieving the best quality possible
        $NewImage.SmoothingMode = $SmoothingMode
        $NewImage.InterpolationMode = $InterpolationMode
        $NewImage.PixelOffsetMode = $PixelOffsetMode
        $NewImage.DrawImage($OldImage, $(New-Object -TypeName System.Drawing.Rectangle -ArgumentList 0, 0, $NewWidth, $NewHeight))

        If ($PSCmdlet.ShouldProcess("Resized image based on $Path", "save to $OutputPath")) {
            $OldImage.Dispose()
            Remove-Item -Path $TempPath
            $Bitmap.Save($OutputPath)
        }
            
        $Bitmap.Dispose()
        $NewImage.Dispose()

    }
}