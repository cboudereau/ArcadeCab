# ArcadeCab
arcadecab script for frontend / emulator integration

## Why
I started with one front end which gives me a first experience but after years I saw other front ends with great features and less problems. So I migrated my front end to an other one and discovered some integration problems.

For every problem I found a standalone solution to make a front-end/launcher free scripts to reduce front end migration effort.

## Philosophy

AracadeCab helps you to build the software part of an arcade machine from the frontend to emulators integration.
I use only a transversal way to integrate frontend to emulators. Basically each Frontend launch an exe with the rom name/path as parameter.
I use batch script + AutoHotkey exe to map keyboard. I am not a big fan of AutoHotkey but it is the only one which handles keyboard keys properly (ie pause and break keys, multiple keys and so on).

Remember that each emulator has their own way to map / configure keys and sometimes keys are hard coded.

I tried an attemps with a powershell script but it does not work : KeyLog.ps1

## OS

### Windows
After noticing a performance downgrade with windows 10 updates I started a comparision between Windows 7/ Windows 8 and Windows 10 version.

The best FPS with my harware was the Windows 10 1511 after windows 7 then windows 8. Higher version of Windows 10 slowdown the performance and you could have some FPS performance instability and penalty.

## Architecture

The integration is a chain of executable scripts from the frontend to the emulator.

Frontend > Integration Script > Emulator

### Frontends
Hyperspin, Launchbox, Maximus or whatever. I used Maximus due to the transversal features but after somes issues (game not saved, ...) I tried a custom integration with Hyperspin which is perfect.

### tools
Under the tools folder you can find useful script/tools for HyperSpin Databases and theme integration

### Integration Scripts : The script called by the frontend
You can find each script for specific emulator into script folder
The integration script is used to : 

 - Correctly map keys to quit a game and avoid a kill of the process (like maximus does). The kill can corrupt your data or simply doesn't  save your state.
 - Launch the emulator with the correct parameters / paths and window style
 - Map additionals keys like Future Pinball.
 
### Emulators
For the sake of the simplicity, sometimes it is duplicated but easy to update without too much dependencies and impacts. Scripts are available for :
 - Mame
 - Future Pinball
 - Nintendo - Game & Watch
 - Nintendo - Game Boy
 - Nintendo - Game Boy Advance
 - Nintendo - Game Boy Color
 - Nintendo - N64
 - Nintendo - Nes
 - Nintendo - SuperNintendo
 - Sega - 32x
 - Sega - GameGear
 - Sega - MasterSystem
 - Sega - MegaCd
 - Sega - MegaDrive
 - Sony - PlayStation

### Demo
- Front End : https://youtu.be/YXh1UzTFK6g
- Light Gun : https://youtu.be/BDYVlEeiiqk
- Pinball : https://youtu.be/V_hiqR8MrH4