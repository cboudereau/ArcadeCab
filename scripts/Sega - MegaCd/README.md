# Sega - CD/Mega Cd

The run.cmd will
 * Start the run.ps1 script to
  * Start the Hotkey.exe which redirects &+" to Esc. You can change the mapping by updating the Hotkey.ahk scripts.
  * Start the Fusion364 emulator with the -mcd platform parameter in a hidden window. You have to change the path in the batch. 

## Hypersin Settings
```
[exe info]
path=D:\data\Sega - MegaCd\
rompath=D:\data\Sega - MegaCd\roms\
userompath=true
exe=run.cmd
romextension=iso
parameters=
searchsubfolders=true
pcgame=false
winstate=HIDDEN
hyperlaunch=false
```