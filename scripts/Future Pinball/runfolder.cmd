REM cd /d "d:\data\Future Pinball" & "d:\data\Future Pinball\runfolder.cmd" "d:\data\Future Pinball\tables\rolling stones.zip"

START /B FuturePinballHotkey.exe
SET FPHS_TABLEPATH=%~d1%~p1
SET FPHS_TABLENAME=%~n1
SET FPHS_TABLEFULLPATH="%FPHS_TABLEPATH%%FPHS_TABLENAME%\%FPHS_TABLENAME%.fpt"

REM START /B /W "bootstrap" "D:\HS\HyperSpin\Emulators\Future Pinball\Future Pinball.exe" /open "%~1.fpt" /play /exit
taskkill /IM FuturePinballHotkey.exe