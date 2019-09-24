START /B FuturePinballHotkey.exe
START /B /W "bootstrap" "D:\HS\HyperSpin\Emulators\Future Pinball\Future Pinball.exe" /open "%~1" /play /exit
taskkill /IM FuturePinballHotkey.exe