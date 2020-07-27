REM Set-ExecutionPolicy Bypass -Scope LocalMachine
REM Set-ExecutionPolicy Bypass -Scope CurrentUser
SET SG_SCRIPT=run.ps1
powershell.exe -File "%~dp0%SG_SCRIPT%" "%~1"