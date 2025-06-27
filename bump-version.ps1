# bump-version.ps1

$scriptPath = $PSScriptRoot
$assemblyInfo = Join-Path $scriptPath "Properties\AssemblyInfo.cs"

if (-Not (Test-Path $assemblyInfo)) {
    Write-Error "❌ AssemblyInfo.cs not found at path: $assemblyInfo"
    exit 1
}

Write-Host "🔍 Found AssemblyInfo.cs at: $assemblyInfo"

# Read original content
$content = Get-Content -Path $assemblyInfo

# New content with bumped versions
$updatedContent = @()
$versionFound = $false

foreach ($line in $content) {
    if ($line -match 'Assembly(File)?Version\("(\d+)\.(\d+)\.(\d+)\.(\d+)"\)') {
        $major = [int]$matches[2]
        $minor = [int]$matches[3]
        $build = [int]$matches[4]
        $rev   = [int]$matches[5] + 1

        $newVersion = "$major.$minor.$build.$rev"
        $updatedLine = $line -replace '(\d+\.\d+\.\d+\.\d+)', $newVersion

        Write-Host "✅ Bumping version to: $newVersion → $line"
        $updatedContent += $updatedLine
        $versionFound = $true
    }
    else {
        $updatedContent += $line
    }
}

if (-Not $versionFound) {
    Write-Warning "⚠️ No AssemblyVersion or AssemblyFileVersion attribute found!"
    exit 1
}

# Write updated content back to file
try {
    $updatedContent | Set-Content -Path $assemblyInfo -Encoding UTF8
    Write-Host "💾 AssemblyInfo.cs updated successfully."
}
catch {
    Write-Error ("❌ Failed to write to {0}: {1}" -f $assemblyInfo, $_)
}
