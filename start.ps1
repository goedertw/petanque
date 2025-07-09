$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

# First stop previous instances
Get-Process -Name "dotnet", "node" -ErrorAction SilentlyContinue | Sort-Object -Property Id -Descending | ForEach-Object {
    Write-Host "Het process '$($_.ProcessName)' (ID: $($_.Id)) van de vorige keer wordt gestopt"
    Stop-Process -Id $_.Id
}
Start-Sleep -Seconds 2
Get-Process -Name "dotnet", "node" -ErrorAction SilentlyContinue | Sort-Object -Property Id -Descending | ForEach-Object {
    Write-Host "Het process '$($_.ProcessName)' (ID: $($_.Id)) van de vorige keer wordt HARDHANDIG gestopt"
    Stop-Process -Id $_.Id -Force
    #Start-Sleep -Seconds 1
}

# Dump DB
Write-Host "De databank wordt gebackupt..."
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir
Set-Location manage-db
.\backup.ps1 .\mysql.cfg.local
Set-Location ..

# Start dotnet run in a new PowerShell window
Write-Host "De 'backend' (dotnet api) wordt gestart..."
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
$dotnetProc = Start-Process powershell -ArgumentList "-Command", "cd Petanque\Petanque.Api; dotnet run" -PassThru -WindowStyle Minimized
Start-Sleep -Seconds 2

# Start npm run dev in another new PowerShell window
Write-Host "De 'frontend' (node.js server) wordt gestart..."
$env:VITE_API_URL = "http://localhost:5251/api"
$npmProc = Start-Process powershell -ArgumentList "-Command", 'cd Petanque\petanquefrontend; npm run dev' -PassThru -WindowStyle Minimized
Start-Sleep -Seconds 2

Write-Host "Wachten op de 'frontend'..."
while ($true) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5173/" -UseBasicParsing -TimeoutSec 2
        if ($response.StatusCode -eq 200) {
            #Write-Host "De 'frontend' is klaar!"
            break
        }
    } catch {
	Write-Host "Nog even geduld..."
        Start-Sleep -Seconds 1
    }
}
Write-Host "Wachten op de 'backend'..."
while ($true) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5251/api/seizoenen" -UseBasicParsing -TimeoutSec 2
        if ($response.StatusCode -eq 200) {
            #Write-Host "De 'backend' is klaar!"
            break
        }
    } catch {
	Write-Host "Nog even geduld..."
        Start-Sleep -Seconds 1
    }
}

# Launch the frontend in default browser
Write-Host "De Petanque-App wordt gestart in je webbrowser..."
Start-Sleep -Seconds 2
Start-Process "http://localhost:5173/"

Write-Host ""
Start-Sleep -Seconds 10

