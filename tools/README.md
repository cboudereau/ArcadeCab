# Tools

This part contains tools to organize and manage your roms, misc/media files with database (mainly : HyperSpin database)

/!\ Becareful, backup your files before using this tool. There is no preview mode.

Each command is self documented.

## Resize-Image
Resize-Image is a powershell script usefull to convert 4:3 to 16:9 by a resize in percent of 75 percent width.

## HyperSyncRom
This tool help me to build a consistant HyperSpin database according to available roms I have.

### Syncing
Sync command will add available roms found in the supplied directory into the database. I use this one for missing emulator from HyperList like Future Pinball. For others, use the renaming command instead.

### Renaming
Rename command will rename the file according to the given crc found into the Hyperspin Database xml file. You can download this files depending to your Emulator at http://hyperlist.hyperspin-fe.com/. The default Hyperspin package does not contain any crc.
The purge parameter simply delete the rom if the file does not match any crc.

### Scanning
Scan command lists the missing files for a given directory. It is pretty easy to identify what files is missing (artwork, wheel, images and so on...).

### Matching
Match command will try to find associated files (given directory with a simple levenstein string distance). Usefull for MAME roms. There is also an interactive mode if you are not sure about the distance.