Write-Host "Alle onderdelen worden gestopt..."

# Kill dotnet and npm processes started by the dev scripts
Get-Process -Name "dotnet", "node" -ErrorAction SilentlyContinue | Sort-Object -Property Id -Descending | ForEach-Object {
    Write-Host "Het process '$($_.ProcessName)' (ID: $($_.Id)) wordt gestopt"
    Stop-Process -Id $_.Id
    #Start-Sleep -Seconds 1
}
Start-Sleep -Seconds 2
Get-Process -Name "dotnet", "node" -ErrorAction SilentlyContinue | Sort-Object -Property Id -Descending | ForEach-Object {
    Write-Host "Het process '$($_.ProcessName)' (ID: $($_.Id)) wordt HARDHANDIG gestopt"
    Stop-Process -Id $_.Id -Force
    #Start-Sleep -Seconds 1
}
Write-Host "De databank wordt gebackupt..."
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir
Set-Location manage-db
.\backup.ps1 .\mysql.cfg.local
Set-Location ..

Write-Host "Klaar!"
Start-Sleep -Seconds 3

