@echo off

set ROOTDIR=%~dp0\..
set ROOTDIRFIND=%~dp0..

echo Processing ISP info...
mkdir "%ROOTDIR%\assets\ispdb"
del /s /q "%ROOTDIR%\assets\ispdb\*"
python3 -m venv "%ROOTDIR%\nvenv"
call "%ROOTDIR%\nvenv\Scripts\activate.bat"
pip install lxml
forfiles /s /m *.xml /p "%ROOTDIRFIND%\assets\autoconfig\ispdb" /C "cmd /c echo @path && python3 "%ROOTDIR%/assets/autoconfig/tools/convert.py" -a -d "%ROOTDIR%/assets/ispdb" @path"
forfiles /s /m * /p "%ROOTDIRFIND%\assets\ispdb" /C "cmd /c echo @path && move @path @path.xml"
call "%ROOTDIR%\nvenv\Scripts\deactivate.bat"
del /s /q "%ROOTDIR%\nvenv"
