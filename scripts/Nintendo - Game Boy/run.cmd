START /B Hotkey.exe
START /B /W "bootstrap" "D:\HS\HyperSpin\Emulators\Nintendo - Game Boy\VisualBoyAdvance\VisualBoyAdvance.exe" "%~1"
taskkill /IM Hotkey.exe