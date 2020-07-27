$folder = 'D:\data\Sega - 32x\video'

Get-ChildItem $folder -Filter "*.avi" | Rename-Item -NewName { $_.Name -replace ".avi",".mp4" }

