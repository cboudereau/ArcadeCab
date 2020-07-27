START /B Hotkey.exe
START /B /W "bootstrap" "D:\HS\HyperSpin\Emulators\Nintendo - SuperNintendo\ZSNES_V1.51_win\zsnesw.exe" "%~1"
taskkill /IM Hotkey.exe