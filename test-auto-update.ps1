# Test Auto-Update Detection
Write-Host 'Testing Auto-Update Detection...' -ForegroundColor Cyan
Write-Host ''

$currentMajor = 3
$currentMinor = 3
$currentPatch = 0
$currentVersion = "$currentMajor.$currentMinor.$currentPatch"

Write-Host "Current Version: $currentVersion" -ForegroundColor Yellow
Write-Host ''

try {
    $apiUrl = 'https://api.github.com/repos/Yogi1972/Rpg_Dungeon/releases/latest'
    Write-Host "Checking: $apiUrl" -ForegroundColor Gray
    
    $headers = @{
        'User-Agent' = 'Rpg-Dungeon-Crawler/3.0'
        'Accept' = 'application/vnd.github+json'
        'X-GitHub-Api-Version' = '2022-11-28'
    }
    
    $release = Invoke-RestMethod -Uri $apiUrl -Headers $headers
    
    Write-Host 'Latest Release Found!' -ForegroundColor Green
    Write-Host "  Tag: $($release.tag_name)" -ForegroundColor White
    Write-Host "  Name: $($release.name)" -ForegroundColor White
    Write-Host ''
    
    $tagName = $release.tag_name -replace '^[vV]', ''
    $parts = $tagName -split '-'
    $versionPart = $parts[0]
    
    $versionNumbers = $versionPart -split '\.'
    $remoteMajor = [int]$versionNumbers[0]
    $remoteMinor = if ($versionNumbers.Length -gt 1) { [int]$versionNumbers[1] } else { 0 }
    $remotePatch = if ($versionNumbers.Length -gt 2) { [int]$versionNumbers[2] } else { 0 }
    
    $remoteVersion = "$remoteMajor.$remoteMinor.$remotePatch"
    
    Write-Host "Version Comparison:" -ForegroundColor Cyan
    Write-Host "  Current: $currentVersion" -ForegroundColor Yellow
    Write-Host "  Latest:  $remoteVersion" -ForegroundColor Green
    Write-Host ''
    
    $isNewer = $false
    if ($remoteMajor -gt $currentMajor) {
        $isNewer = $true
    } elseif ($remoteMajor -eq $currentMajor -and $remoteMinor -gt $currentMinor) {
        $isNewer = $true
    } elseif ($remoteMajor -eq $currentMajor -and $remoteMinor -eq $currentMinor -and $remotePatch -gt $currentPatch) {
        $isNewer = $true
    }
    
    if ($isNewer) {
        Write-Host 'NEW UPDATE AVAILABLE!' -ForegroundColor Green
        Write-Host "Latest: v$remoteVersion" -ForegroundColor Green
        Write-Host "Current: v$currentVersion" -ForegroundColor Yellow
    } elseif ($remoteMajor -eq $currentMajor -and $remoteMinor -eq $currentMinor -and $remotePatch -eq $currentPatch) {
        Write-Host 'You are running the latest version!' -ForegroundColor Green
    } else {
        Write-Host 'Your version is NEWER (development)' -ForegroundColor Yellow
    }
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ''
Write-Host 'Test complete!' -ForegroundColor Gray
