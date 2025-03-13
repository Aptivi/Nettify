@echo off

set ROOTDIR=%~dp0\..

echo Downloading ISP info...
powershell "Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process ; %ROOTDIR%/public/Nettify/assets/IspInfo/getispinfo.ps1"
