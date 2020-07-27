$hotkey = Start-Process -PassThru -NoNewWindow $PSScriptRoot\Hotkey.exe

Start-Process -WindowStyle Hidden -Wait -ArgumentList @("`"$($Args[0])`"") 'D:\HS\HyperSpin\Emulators\Nintendo - N64\Project64 2.3\Project64.exe'

Stop-Process $hotkey -Force