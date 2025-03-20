@echo off

set ROOTDIR=%~dp0\..
set ROOTDIRFIND=%~dp0..

echo Processing ISP info...
mkdir assets\ispdb
del /s /q assets\ispdb\*
pip install lxml
forfiles /s /m *.xml /p %ROOTDIRFIND%\assets\autoconfig\ispdb /C "cmd /c echo @path && python3 %ROOTDIR%/assets/autoconfig/tools/convert.py -a -d %ROOTDIR%/assets/ispdb @path"
forfiles /s /m * /p %ROOTDIRFIND%\assets\ispdb /C "cmd /c echo @path && move @path @path.xml"
