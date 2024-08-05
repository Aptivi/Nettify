@echo off

REM This script builds and packs the artifacts. Use when you have VS installed.
set releaseconfig=%1
if "%releaseconfig%" == "" set releaseconfig=Release

:ispinfo
echo Downloading ISP info...
powershell "Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process ; ../Nettify/assets/IspInfo/getispinfo.ps1"
if %errorlevel% == 0 goto :download
echo There was an error trying to download ISP info (%errorlevel%).
goto :finished

:download
echo Downloading packages...
"%ProgramFiles%\dotnet\dotnet.exe" msbuild "..\Nettify.sln" -t:restore -p:Configuration=%releaseconfig%
if %errorlevel% == 0 goto :build
echo There was an error trying to download packages (%errorlevel%).
goto :finished

:build
echo Building Nitrocid KS...
"%ProgramFiles%\dotnet\dotnet.exe" msbuild "..\Nettify.sln" -p:Configuration=%releaseconfig%
if %errorlevel% == 0 goto :success
echo There was an error trying to build (%errorlevel%).
goto :finished

:success
echo Build successful.
:finished
