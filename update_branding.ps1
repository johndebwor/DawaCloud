# PowerShell script to update all DawaBes branding to DawaCloud
# This includes PageTitles, UI text, and CSS references

Write-Host "Starting UI branding update: DawaBes -> DawaCloud" -ForegroundColor Green
Write-Host "Working directory: $(Get-Location)" -ForegroundColor Yellow

# Navigate to project root
Set-Location -Path "E:\Projects\DawaFlow\DawaFlow\src\DawaCloud.Web"

# Step 1: Rename CSS file
Write-Host "`nStep 1: Renaming CSS file..." -ForegroundColor Cyan
$cssOldPath = "wwwroot\css\DawaBes.css"
$cssNewPath = "wwwroot\css\DawaCloud.css"

if (Test-Path $cssOldPath) {
    Move-Item -Path $cssOldPath -Destination $cssNewPath -Force
    Write-Host "  Renamed: DawaBes.css -> DawaCloud.css" -ForegroundColor Gray
} else {
    Write-Host "  CSS file already renamed or not found" -ForegroundColor Yellow
}

# Step 2: Update all text files (excluding bin/obj)
Write-Host "`nStep 2: Updating brand name in files..." -ForegroundColor Cyan
$files = Get-ChildItem -Path . -Include *.razor,*.cs,*.json,*.html -Recurse |
    Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\' }

Write-Host "Found $($files.Count) files to process" -ForegroundColor Cyan

$updatedCount = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue

    if ($content) {
        $newContent = $content -replace 'DawaBes', 'DawaCloud'

        if ($content -ne $newContent) {
            Set-Content -Path $file.FullName -Value $newContent -NoNewline
            Write-Host "  Updated: $($file.FullName.Substring($(Get-Location).Path.Length))" -ForegroundColor Gray
            $updatedCount++
        }
    }
}

Write-Host "`nUI branding update complete!" -ForegroundColor Green
Write-Host "Updated $updatedCount files" -ForegroundColor Cyan
