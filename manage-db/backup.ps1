# PowerShell Script: backup.ps1

param (
    [Parameter(Mandatory = $true)]
    [string]$CfgFile
)

$PathToMysqlDump = 'C:\Program Files\MySQL\MySQL Server 8.4\bin\mysqldump.exe'
$DumpDir = ".\sqldumps"
$Debug = $false

function Show-Usage {
    Write-Host ""
    Write-Host "Usage: .\backup.ps1 <mysql-cfg-file>"
    Write-Host ""
    exit 1
}

# Check if config file exists
if (-not (Test-Path -PathType Leaf $CfgFile)) {
    Write-Host "ERROR: '$CfgFile' is not a file"
    Show-Usage
}

# Check the path to mysqldump.exe
if (-not (Test-Path -PathType Leaf $PathToMysqlDump)) {
    Write-Host "ERROR: '$PathToMysqlDump' is not a file"
    Write-Host 'Open this script and change the value of $PathToMysqlDump'
    Write-Host ""
    exit 1
}

# Create TimeStamp and extract host/database from config
$TimeStamp = Get-Date -Format "yyyy-MM-dd_HH'u'mm"
$DbHost = ((Select-String -Path $CfgFile -Pattern "^\s*host\s*=").Line -split "=")[1].Trim(' ',"`t","'",'"')
$DbName = ((Select-String -Path $CfgFile -Pattern "^\s*database\s*=").Line -split "=")[1].Trim(' ',"`t","'",'"')
$DumpFile = "$DumpDir\$TimeStamp-$DbHost-$DbName.sql"

# Create temporary config file without the 'database=' line
$CurrentPath = Get-Location
$TmpCfgFile = "$CurrentPath\mysql.tmp.$TimeStamp"
Get-Content $CfgFile | Where-Object { $_ -notmatch "^\s*database\s*=" } | Set-Content $TmpCfgFile

# Ensure dump directory exists
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir
if (-not (Test-Path $DumpDir)) {
    New-Item -ItemType Directory -Path $DumpDir
}

# Perform the dump
Write-Host "Dumping database to '$DumpFile' ..."
& $PathToMysqlDump --defaults-extra-file="$TmpCfgFile" --column-statistics=0 --no-tablespaces $DbName --result-file="$DumpFile"
$ExitCode = $LASTEXITCODE

if ($Debug) { Write-Host "ExitCode=$ExitCode" }

Remove-Item $TmpCfgFile -ErrorAction SilentlyContinue

if ($ExitCode -ne 0) {
    if (!$Debug) { Remove-Item $DumpFile -ErrorAction SilentlyContinue }
    Write-Host "---"
    Write-Host "ERROR: dump failed!"
    Write-Host ""
    exit $ExitCode
}

Write-Host "Done!"
Write-Host ""
