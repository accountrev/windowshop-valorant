@echo off

echo Killing all Windowshop.exe processes...
taskkill /F /IM Windowshop.exe

set "appdataPath=%AppData%"

set "startupPath=%UserProfile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup"

if exist "%appdataPath%\Windowshop\no_startup" (
    del "%appdataPath%\Windowshop\no_startup"
    echo Deleted no_startup file from Windowshop AppData Roaming folder.
)

if exist "%startupPath%\Windowshop.lnk" (
    del "%startupPath%\Windowshop.lnk"
    echo Deleted Windowshop.lnk shortcut from Startup folder.
)

echo Autostart settings have been reset.

pause
