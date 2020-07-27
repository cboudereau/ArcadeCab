# Sega - Master System

The run.cmd will
 * Start the run.ps1 script to
  * Start the Hotkey.exe which redirects &+" to Esc. You can change the mapping by updating the Hotkey.ahk scripts.
  * Start the Fusion364 emulator with the -sms platform parameter in a hidden window. You have to change the path in the batch. 

## Hypersin Settings
```
[exe info]
path=D:\data\Sega - MasterSystem\
rompath=D:\data\Sega - MasterSystem\roms\
userompath=true
exe=run.cmd
romextension=sms
parameters=
searchsubfolders=true
pcgame=false
winstate=HIDDEN
hyperlaunch=false
```