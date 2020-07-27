START /B Hotkey.exe
START /B /W "bootstrap" "D:\HS\HyperSpin\Emulators\Nintendo - Nes\fceux-2.2.3-win32\fceux.exe" "%~1"
taskkill /IM Hotkey.exe