START /B FuturePinballHotkey.exe
START /B /W "bootstrap" "D:\HS\HyperSpin\Emulators\Future Pinball\BAM\FPLoader.exe" /open "%~1.fpt" /play /exit
taskkill /IM FuturePinballHotkey.exe