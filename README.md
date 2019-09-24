# ArcadeCab
arcadecab script for frontend / emulator integration

## Why
I started with one front end that give me a first experience but after years I saw other front end with great features and less problem. So I migrated my front end to an other one and discovered some integration problems.

For every problem I found a standalone solution to make a front-end/launcher free solution that give...

## Philosophy

AracadeCab helps you to build the software part of an arcade machine from the frontend to emulators integration.
I use only a transversal way to integrate frontend to emulators. Basically each Frontend launch an exe with the rom name/path as parameter.
I use batch script + AutoHotkey exe to map keyboard. I am not a big fan of AutoHotkey but it is the only one that handles keyboard keys fine (ie pause and break keys, multiple keys and so on).

Remember that each emulator has their own way to map / configure keys and sometimes keys are hard coded.

I tried an attemps with a powershell script but it does not work : KeyLog.ps1

## Architecture

The integration is a chain of executable script from the frontend to the emulator.

Frontend > Integration Script > Emulator

### Frontends
Hyperspin, Launchbox, Maximus or whatever. I had used Maximus due to the transversal features but after somes issues (game not saved, ...) I try a custom integration with Hyperspin.

### Integration Script : The script called by the frontend
You can find each script for specific emulator into script folder
The integration script is used to : 

 - Correctly map keys to quit a game and avoid a kill of the process (like maximus does). The kill can corrupt your data or simply not save your state.
 - Launch the emulator with the correct parameters / paths and window style
 - Map additionals keys like Future Pinball.
 
### Emulators
I Wrote integration script for 
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
