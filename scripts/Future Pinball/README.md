# Future Pinball

FP script with zip (7za) support. Tables should be a zip files with all dependencies without folder. The rom filename is mapped to the zip and fpt file names.

The script will : 

 1. Start the AutoHotKey exe FuturePinballHotkey.exe (adjust to your layout as needed, with the ahk script)
  * See Backglass : Map w to tab
  * Pause/High score : Map z to MButton (previously configure FP to use mouse and only map Middle button to high score because redirecting pause key is a mess..)
  * Quit : Map & (SC002) and " (SC004) to Escape.
 2. Decompress table into a temp folder
 3. Start Future Pinball

For HyperSpin, use the runzip.cmd script and the fpt extension.

## Hypersin settings

Don't forget to replace paths with yours.

```
[exe info]
path=D:\data\Future Pinball\
rompath=D:\data\Future Pinball\tables\
userompath=true
exe=runzip.cmd
romextension=fpt
parameters=
searchsubfolders=true
pcgame=false
winstate=HIDDEN
hyperlaunch=false
```

## Table database links
http://www.pinsimdb.org/pinball/index-10-future_pinball
