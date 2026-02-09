# PowerShell script to update all DawaFlow.Web namespaces to DawaCloud.Web
# This script updates C#, Razor, and other source files

Write-Host "Starting namespace refactoring: DawaFlow.Web -> DawaCloud.Web" -ForegroundColor Green
Write-Host "Working directory: $(Get-Location)" -ForegroundColor Yellow

# Navigate to project root
Set-Location -Path "E:\Projects\DawaFlow\DawaFlow\src\DawaCloud.Web"

# Find all C# and Razor files, excluding bin/obj/Migrations initially
$files = Get-ChildItem -Path . -Include *.cs,*.razor,*.cshtml -Recurse |
    Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\' }

Write-Host "Found $($files.Count) files to process" -ForegroundColor Cyan

$updatedCount = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue

    if ($content) {
        $newContent = $content -replace 'DawaFlow\.Web', 'DawaCloud.Web'

        if ($content -ne $newContent) {
            Set-Content -Path $file.FullName -Value $newContent -NoNewline
            Write-Host "  Updated: $($file.FullName.Substring($(Get-Location).Path.Length))" -ForegroundColor Gray
            $updatedCount++
        }
    }
}

Write-Host "`nNamespace refactoring complete!" -ForegroundColor Green
Write-Host "Updated $updatedCount files" -ForegroundColor Cyan
