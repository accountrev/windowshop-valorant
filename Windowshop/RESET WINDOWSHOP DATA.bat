@echo off

echo Killing all Windowshop.exe processes...
taskkill /F /IM Windowshop.exe

set "appdataPath=%appdata%"

if exist "%appdataPath%\Windowshop" (
    del /q "%appdataPath%\Windowshop\*"
    echo Windowshop has been reset.
) else (
    echo No settings found.
)

pause
