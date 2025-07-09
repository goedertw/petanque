$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

Write-Host ""
Write-Host "De update wordt gestart..."
Write-Host ""

git pull
$ExitCode = $LASTEXITCODE
Write-Host ""

if ($ExitCode -ne 0) {
    Write-Host "-------------------------------------"
    Write-Host "!! ERROR: De update is niet gelukt !!"
    Write-Host "-------------------------------------"
    Write-Host "Controleer je internetverbinding en probeer opnieuw"
    Write-Host ""
    Read-Host -Prompt "Druk op ENTER om dit scherm te sluiten..."
    exit $ExitCode
}

Write-Host ""
Write-Host "De update is geslaagd."
Write-Host "---------------------------------------------------------"
Write-Host "OPGELET: De eerst volgende keer starten kan langer duren."
Write-Host "---------------------------------------------------------"
Write-Host ""
Read-Host -Prompt "Druk op ENTER om dit scherm te sluiten..."

