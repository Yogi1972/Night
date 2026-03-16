# Publish release script for RPG Dungeon
# Usage: powershell -ExecutionPolicy Bypass -File .\scripts\publish_release.ps1

param(
    [string]$RuntimeId = '',
    [switch]$Standalone
)

try {
    $root = Resolve-Path -Path ".." -Relative | ForEach-Object { Join-Path (Get-Location) $_ }
} catch {
    $root = Get-Location
}
Set-Location $PSScriptRoot

# Extract version from Systems/VersionControl.cs
$vcPath = Join-Path $PSScriptRoot "..\Systems\VersionControl.cs"
if (-not (Test-Path $vcPath)) { $vcPath = Join-Path $PSScriptRoot "Systems\VersionControl.cs" }
if (-not (Test-Path $vcPath)) { Write-Error "VersionControl.cs not found"; exit 1 }
$vc = Get-Content $vcPath -Raw
$major = ([regex]::Match($vc, 'MajorVersion\s*=\s*(\d+)').Groups[1].Value)
$minor = ([regex]::Match($vc, 'MinorVersion\s*=\s*(\d+)').Groups[1].Value)
$patch = ([regex]::Match($vc, 'PatchVersion\s*=\s*(\d+)').Groups[1].Value)
if (-not $major) { Write-Error "Could not parse major version"; exit 1 }
$tag = "v$major.$minor.$patch"
Write-Host "Preparing release $tag" -ForegroundColor Cyan

# Ensure git is available
if (-not (Get-Command git -ErrorAction SilentlyContinue)) { Write-Error "git CLI not found on PATH"; exit 1 }

# Ensure working tree clean
$status = git status --porcelain
if ($status) {
    Write-Host "Working tree has unstaged changes. Staging all changes..." -ForegroundColor Yellow
}

git add -A
$commitMsg = "Release $tag"
try {
    git commit -m "$commitMsg" | Out-Null
} catch {
    Write-Host "No new changes to commit or commit failed (continuing)" -ForegroundColor Yellow
}

# Create tag
if (git rev-parse "$tag" 2>$null) {
    Write-Host "Tag $tag already exists locally" -ForegroundColor Yellow
} else {
    git tag -a $tag -m "Release $tag"
}

# Push
Write-Host "Pushing commits and tags to origin..." -ForegroundColor Cyan
git push origin --follow-tags

# Build and publish compiled output
$projectFile = Get-ChildItem -Path $PSScriptRoot -Filter '*.csproj' -Recurse | Select-Object -First 1
if (-not $projectFile) { Write-Error "Project file not found (.csproj)"; exit 1 }

$publishDir = Join-Path $env:TEMP "rpg_publish_$tag"
if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }
New-Item -ItemType Directory -Path $publishDir | Out-Null

Write-Host "Running dotnet publish for Release configuration..." -ForegroundColor Cyan

# Determine runtime id when standalone requested
if ($Standalone) {
    if (-not $RuntimeId) { $RuntimeId = 'win-x64' }
    Write-Host "Publishing standalone self-contained build for runtime: $RuntimeId" -ForegroundColor Cyan
    dotnet publish $projectFile.FullName -c Release -r $RuntimeId --self-contained true -o $publishDir
} else {
    dotnet publish $projectFile.FullName -c Release -o $publishDir
}

if ($LASTEXITCODE -ne 0) { Write-Error "dotnet publish failed (exit $LASTEXITCODE)"; exit 1 }

$zipPath = Join-Path $env:TEMP "rpg_release_${tag}.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Write-Host "Creating zip package from published output: $zipPath" -ForegroundColor Cyan
Compress-Archive -Path (Join-Path $publishDir '*') -DestinationPath $zipPath -Force

# Generate SHA256 checksum file for the zip and the main exe (if present)
$shaPath = "$zipPath.sha256.txt"
try {
    Write-Host "Generating SHA256 checksum for zip..." -ForegroundColor Cyan
    $sha = Get-FileHash -Path $zipPath -Algorithm SHA256
    $shaText = "$($sha.Hash)  $(Split-Path -Path $zipPath -Leaf)"
    Set-Content -Path $shaPath -Value $shaText -Encoding UTF8
    Write-Host "Checksum written to: $shaPath" -ForegroundColor Green
} catch {
    Write-Host "Failed to generate zip checksum: $($_.Exception.Message)" -ForegroundColor Yellow
}

# If an executable exists in the published output, generate a separate checksum for it
$exePath = Get-ChildItem -Path $publishDir -Filter '*.exe' -Recurse | Select-Object -First 1
$exeShaPath = $null
if ($exePath) {
    try {
        $exeShaPath = "$($exePath.FullName).sha256.txt"
        Write-Host "Generating SHA256 for executable: $($exePath.Name)" -ForegroundColor Cyan
        $esha = Get-FileHash -Path $exePath.FullName -Algorithm SHA256
        $eshaText = "$($esha.Hash)  $($exePath.Name)"
        Set-Content -Path $exeShaPath -Value $eshaText -Encoding UTF8
        Write-Host "Executable checksum written to: $exeShaPath" -ForegroundColor Green
    } catch {
        Write-Host "Failed to generate exe checksum: $($_.Exception.Message)" -ForegroundColor Yellow
        $exeShaPath = $null
    }
}

# Use gh CLI to create release if available
if (Get-Command gh -ErrorAction SilentlyContinue) {
    Write-Host "Creating GitHub release $tag..." -ForegroundColor Cyan
    # Upload zip, zip checksum, and exe+checksum (if present)
    $assets = @($zipPath)
    if (Test-Path $shaPath) { $assets += $shaPath }
    if ($exePath) { $assets += $exePath.FullName }
    if ($exeShaPath -and (Test-Path $exeShaPath)) { $assets += $exeShaPath }

    gh release create $tag @assets --title $tag --notes "Automated release $tag"
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Release $tag created and asset uploaded." -ForegroundColor Green
    } else {
        Write-Error "gh release create failed (exit $LASTEXITCODE)"
    }
} else {
    # Fallback: open releases page and advise manual upload
    $repoUrl = "https://github.com/Yogi1972/Night/releases/new?tag=$tag"
    Write-Host "gh CLI not installed. Please create a release manually at:" -ForegroundColor Yellow
    Write-Host $repoUrl -ForegroundColor Cyan
    Start-Process $repoUrl
}

Write-Host "Done." -ForegroundColor Green

# Cleanup temp publish directory
if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force -ErrorAction SilentlyContinue }

exit 0
