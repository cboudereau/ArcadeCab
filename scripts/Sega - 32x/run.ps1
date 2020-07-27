$hotkey = Start-Process -PassThru -NoNewWindow $PSScriptRoot\Hotkey.exe

Start-Process -WindowStyle Hidden -Wait -ArgumentList @("`"$($Args[0])`"", '-32x', '-auto', '-fullscreen') 'D:\HS\HyperSpin\Emulators\Sega\Fusion364\Fusion.exe'

Stop-Process $hotkey -Force