START /B Kill.exe "ePSXe.exe"
START /W "bootstrap" "D:\HS\HyperSpin\Emulators\Sony - PlayStation\epsxe\ePSXe.exe" -nogui -loadbin "%~1"
taskkill /IM Kill.exe