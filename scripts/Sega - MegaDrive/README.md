# Sega - Megadrive / Genesis

The run.cmd will
 * Start the run.ps1 script to
  * Start the Hotkey.exe which redirects &+" to Esc. You can change the mapping by updating the Hotkey.ahk scripts.
  * Start the Fusion364 emulator with the -gen platform parameter in a hidden window. You have to change the path in the batch. 

## Hypersin Settings
```
[exe info]
path=D:\data\Sega - MegaDrive\
rompath=D:\data\Sega - MegaDrive\roms\
userompath=true
exe=run.cmd
romextension=gen
parameters=
searchsubfolders=true
pcgame=false
winstate=HIDDEN
hyperlaunch=false
```