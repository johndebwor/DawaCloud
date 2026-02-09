# PowerShell script to update documentation files
# Replaces both DawaBes and DawaFlow with DawaCloud

Write-Host "Starting documentation update" -ForegroundColor Green
Write-Host "Working directory: $(Get-Location)" -ForegroundColor Yellow

# Navigate to project root
Set-Location -Path "E:\Projects\DawaFlow\DawaFlow"

# Find all documentation files
$docFiles = Get-ChildItem -Path . -Include *.md,*.txt -Recurse |
    Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\|\\node_modules\\|\\.claude\\' }

Write-Host "Found $($docFiles.Count) documentation files to process" -ForegroundColor Cyan

$updatedCount = 0

foreach ($file in $docFiles) {
    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue

    if ($content) {
        # Replace both DawaBes and DawaFlow with DawaCloud
        $newContent = $content -replace 'DawaBes', 'DawaCloud'
        $newContent = $newContent -replace 'DawaFlow', 'DawaCloud'
        # Also update lowercase versions
        $newContent = $newContent -replace 'dawabes', 'dawacloud'
        $newContent = $newContent -replace 'dawaflow', 'dawacloud'

        if ($content -ne $newContent) {
            Set-Content -Path $file.FullName -Value $newContent -NoNewline -Encoding UTF8
            Write-Host "  Updated: $($file.FullName.Substring($(Get-Location).Path.Length))" -ForegroundColor Gray
            $updatedCount++
        }
    }
}

Write-Host "`nDocumentation update complete!" -ForegroundColor Green
Write-Host "Updated $updatedCount files" -ForegroundColor Cyan
