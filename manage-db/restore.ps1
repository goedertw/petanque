param (
    [Parameter(Mandatory = $true)]
    [string]$SqlFile,
    [Parameter(Mandatory = $true)]
    [string]$CfgFile
)

$PathToMysql = 'C:\Program Files\MySQL\MySQL Server 8.4\bin\mysql.exe'
$Debug = $false

function Show-Usage {
    Write-Host ""
    Write-Host "Usage: .\restore.ps1 <sql-file> <mysql-cfg-file>"
    Write-Host ""
    exit 1
}

function CleanupAndAbort {
    if ($Debug) { Write-Host "exitcode=$exitcode" }
    if (Test-Path $TmpCfgFile) { Remove-Item $tmpCfgFile -Force }
    if (Test-Path $TmpErrFile) { Remove-Item $tmpErrFile -Force }
    Write-Host "... Aborting!"
    Write-Host ""
    exit 1
}

# Validate arguments
if (-not (Test-Path $SqlFile)) { Write-Host "ERROR: '$SqlFile' is not a file"; Show-Usage }
if ((Get-Item $SqlFile).Length -eq 0) { Write-Host "ERROR: '$SqlFile' is an empty file"; Show-Usage }

if (-not (Test-Path $CfgFile)) { Write-Host "ERROR: '$CfgFile' is not a file"; Show-Usage }
if ((Get-Item $CfgFile).Length -eq 0) { Write-Host "ERROR: '$CfgFile' is an empty file"; Show-Usage }

# Check the path to mysql.exe
if (-not (Test-Path -PathType Leaf $PathToMysql)) {
    Write-Host "ERROR: '$PathToMysql' is not a file"
    Write-Host 'Open this script and change the value of $PathToMysql'
    Write-Host ""
    exit 1
}

# Extract config values + set some variables
$DbHost = ((Select-String -Path $CfgFile -Pattern "^\s*host\s*=").Line -split "=")[1].Trim(' ',"`t","'",'"')
$DbName = ((Select-String -Path $CfgFile -Pattern "^\s*database\s*=").Line -split "=")[1].Trim(' ',"`t","'",'"')
$TimeStamp = Get-Date -Format "yyyy-MM-dd_HH'u'mm"
$CurrentPath = Get-Location
$TmpErrFile = "$CurrentPath\mysql.err.$TimeStamp"
$TmpCfgFile = "$CurrentPath\mysql.tmp.$TimeStamp"

# Create temporary config file without the 'database=' line
Get-Content $CfgFile | Where-Object { $_ -notmatch '^\s*database\s*=' } | Set-Content $TmpCfgFile

# Try to create the database
& $PathToMysql --defaults-extra-file=$TmpCfgFile -e "CREATE DATABASE $DbName" 2> $TmpErrFile
$exitcode = $LASTEXITCODE

if (Select-String -Path $TmpErrFile -Pattern "ERROR 1007.*database exists") {
    $ans = Read-Host "WARNING: The database '$DbName' already exists (on $DbHost)! Overwrite? [y/N]"
    if (-not $ans -or $ans.ToUpper() -eq 'N') { CleanupAndAbort }
    & $PathToMysql --defaults-extra-file=$TmpCfgFile -e "DROP DATABASE $DbName; CREATE DATABASE $dbName"
    if ($LASTEXITCODE -ne 0) { CleanupAndAbort }
}
elseif ($exitcode -ne 0 -or (Get-Content $TmpErrFile).Length -gt 0) {
    Get-Content $TmpErrFile
    CleanupAndAbort
}

# Restore the SQL file
Write-Host "Restoring '$SqlFile' to '$DbName' (on $DbHost) ..."
Get-Content $SqlFile | & $PathToMysql --defaults-extra-file=$TmpCfgFile $DbName
if ($LASTEXITCODE -ne 0) { CleanupAndAbort }

Write-Host "Done!"
Write-Host ""

