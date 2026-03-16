param(
    [string]$ZipName = 'rpg_release_v3.5.4.zip'
)

$zip = Join-Path $env:TEMP $ZipName
if (-not (Test-Path $zip)) { Write-Error "Zip not found: $zip"; exit 1 }
$h = Get-FileHash -Path $zip -Algorithm SHA256
$out = $zip + '.sha256.txt'
"$($h.Hash)  $(Split-Path -Path $zip -Leaf)" | Out-File -FilePath $out -Encoding utf8
Write-Host "WROTE: $out"
