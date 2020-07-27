# Nintendo - Nintendo 64

The Project 64 emulator is used and the config has to be patched to bind ESC properly

## Project64 Esc Mapping issue
Open settings > keyboard shortcuts 
Reset the page
Open Project64.sc3 file in the config folder (somepath\Project64\Config\Project64.sc3)
Find the lines beginning with 4173 and delete them; save the file
Open settings and you will be able to assign the esc key to anything you like.

Note that 27 is the ASCII keycode for esc

In the source code, there is nothing hard coded into the settings dialog to support game controller mapping.

## Sample Patched Project64 Config
A Patched project 64 config can be found under the folder Config

The run.cmd will
 * Start the Hotkey.exe which redirects &+" to Esc. You can change the mapping by updating the Hotkey.ahk scripts.
 * Start the Project 64 emulator under a powershell console to hide window. You have to change the path in the batch. 

## Hypersin Settings
```
[exe info]
path=D:\data\Nintendo - N64\
rompath=D:\data\Nintendo - N64\roms\
userompath=true
exe=run.cmd
romextension=z64
parameters=
searchsubfolders=true
pcgame=false
winstate=HIDDEN
hyperlaunch=false
```