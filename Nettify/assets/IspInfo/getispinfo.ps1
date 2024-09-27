# Some variables
Set-Variable DATABASEADDRESS "https://autoconfig.thunderbird.net/v1.1/"

Write-Output "Downloading the ISP list..."
Set-Variable ISPS ((Invoke-WebRequest -Uri $DATABASEADDRESS).links.href | Select-Object -Skip 5)

$ISPS | ForEach-Object -Process {
    Set-Variable XMLFILE "$PSScriptRoot\$_.xml"
    Write-Output "Saving ISP info $_ to $XMLFILE..."
    Invoke-WebRequest $DATABASEADDRESS$_ -OutFile $XMLFILE
    $a = Get-Content $XMLFILE
    $b = '<?xml version="1.0" encoding="UTF-8"?>'
    Set-Content $XMLFILE -value $b,$a
    if (Test-Path "$PSScriptRoot\isps.txt") { Remove-Item -Path "$PSScriptRoot\isps.txt" }
    Write-Output "$_" >> "$PSScriptRoot\isps.txt"
}
