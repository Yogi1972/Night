# Publish release script for RPG Dungeon
# Usage: powershell -ExecutionPolicy Bypass -File .\scripts\publish_release.ps1

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

# Prepare zip of repo for release asset
$tmpDir = Join-Path $env:TEMP "rpg_release_$tag"
if (Test-Path $tmpDir) { Remove-Item $tmpDir -Recurse -Force }
New-Item -ItemType Directory -Path $tmpDir | Out-Null

Write-Host "Copying files to temp dir..." -ForegroundColor Cyan
Get-ChildItem -Path $PSScriptRoot -Force | Where-Object { $_.Name -ne '.git' -and $_.Name -ne 'bin' -and $_.Name -ne 'obj' -and $_.Name -ne '.vs' } | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination $tmpDir -Recurse -Force
}

$zipPath = Join-Path $env:TEMP "rpg_release_${tag}.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Write-Host "Creating zip package: $zipPath" -ForegroundColor Cyan
Compress-Archive -Path (Join-Path $tmpDir '*') -DestinationPath $zipPath -Force

# Use gh CLI to create release if available
if (Get-Command gh -ErrorAction SilentlyContinue) {
    Write-Host "Creating GitHub release $tag..." -ForegroundColor Cyan
    gh release create $tag $zipPath --title $tag --notes "Automated release $tag"
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

# Cleanup temp dir
Remove-Item $tmpDir -Recurse -Force -ErrorAction SilentlyContinue

exit 0
