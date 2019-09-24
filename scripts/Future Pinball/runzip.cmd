REM cd /d "d:\data\Future Pinball" & "d:\data\Future Pinball\runzip.cmd" "d:\data\Future Pinball\tables\rolling stones.fpt"
SETLOCAL
SET FPHS_TMP=tmp\
SET FPHS_TMPFOLDER=%~dp0%FPHS_TMP%
SET FPHS_TABLEPATH=%~d1%~p1
SET FPHS_TABLENAME=%~n1
SET FPHS_TABLEZIPPATH="%FPHS_TABLEPATH%%FPHS_TABLENAME%.zip"
SET FPHS_TMPPATH="%FPHS_TMPFOLDER%"
SET FPHS_TABLETMPPATH="%FPHS_TMPFOLDER%%FPHS_TABLENAME%.fpt"

IF EXIST %FPHS_TMPPATH% ( RMDIR /S /Q %FPHS_TMPPATH% )
7z1900-extra\7za.exe x %FPHS_TABLEZIPPATH% -aoa -o%FPHS_TMPPATH%

START /B FuturePinballHotkey.exe
START /MIN /W "bootstrap" "D:\HS\HyperSpin\Emulators\Future Pinball\Future Pinball.exe" /open %FPHS_TABLETMPPATH% /play /exit
taskkill /IM FuturePinballHotkey.exe
RMDIR /S /Q %FPHS_TMPPATH%