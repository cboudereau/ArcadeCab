# Nintendo - Game Boy

Those scripts are compatible with Nintendo all Game Boy (Game Boy Advance, Game Boy Color, Game Boy)

The run.cmd will
 * start the Hotkey.exe which redirects &+" to Esc. You can change the mapping by updating the Hotkey.ahk scripts.
 * Start the VisualBoyAdvance emulator. You have to change the path in the batch. 

## Hypersin Settings

### Game Boy Classic
```
[video defaults]
path=D:\data\Nintendo - Game Boy\video\
[exe info]
rompath=D:\data\Nintendo - Game Boy\roms\
romextension=gb
path=D:\data\Nintendo - Game Boy\
exe=run.cmd
parameters=
winstate=HIDDEN
[navigation]
last_game=Boxxle (USA, Europe)
start_on_favorites=true
[wheel]
style=vertical
```

### Game Boy Color
```
[exe info]
path=D:\data\Nintendo - Game Boy\
exe=run.cmd
rompath=D:\data\Nintendo - Game Boy Color\roms\
parameters=
romextension=gbc
winstate=HIDDEN
[navigation]
last_game=102 Dalmatians - Puppies to the Rescue (USA, Europe)
start_on_favorites=true
[video defaults]
path=D:\data\Nintendo - Game Boy Color\video\
[wheel]
style=vertical
```

### Game Boy Advance
```
[exe info]
path=D:\data\Nintendo - Game Boy Advance\
rompath=D:\data\Nintendo - Game Boy Advance\roms\
userompath=true
exe=run.cmd
romextension=gba
parameters=
searchsubfolders=true
pcgame=false
winstate=HIDDEN
hyperlaunch=false
```