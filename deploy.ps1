# Deploy Unity WebGL build to GitHub Pages (gh-pages branch)
# Usage: .\deploy.ps1 [build-folder]
#   build-folder: path to your WebGL build output (default: build)

param(
    [string]$BuildDir = "build"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $BuildDir)) {
    Write-Host "Error: Build folder '$BuildDir' not found." -ForegroundColor Red
    Write-Host ""
    Write-Host "Build your game first in Unity:"
    Write-Host "  File -> Build Settings -> WebGL -> Build"
    Write-Host "  Save to: build"
    Write-Host ""
    Write-Host "Then run: .\deploy.ps1"
    exit 1
}

if (-not (Test-Path "$BuildDir\index.html")) {
    Write-Host "Error: No index.html found in '$BuildDir'." -ForegroundColor Red
    Write-Host "Make sure this is a Unity WebGL build output folder."
    exit 1
}

$Remote = git remote get-url origin
$TempDir = Join-Path ([System.IO.Path]::GetTempPath()) ("gh-pages-" + [System.Guid]::NewGuid().ToString("N").Substring(0, 8))

Write-Host "Deploying '$BuildDir' to gh-pages branch..." -ForegroundColor Cyan

# Copy build to temp directory
New-Item -ItemType Directory -Path $TempDir | Out-Null
Copy-Item -Path "$BuildDir\*" -Destination $TempDir -Recurse

# Set up a fresh git repo in the temp dir and push to gh-pages
Push-Location $TempDir
try {
    git init
    git checkout -b gh-pages
    git add -A
    git commit -m "Deploy WebGL build to GitHub Pages"
    git remote add origin $Remote
    git push origin gh-pages --force

    Write-Host ""
    Write-Host "Deployed!" -ForegroundColor Green
    Write-Host "Enable GitHub Pages if you haven't:"
    Write-Host "  Repo -> Settings -> Pages -> Source: 'Deploy from a branch' -> Branch: 'gh-pages' / '/ (root)'"
    Write-Host ""
}
finally {
    Pop-Location
    Remove-Item -Recurse -Force $TempDir
}
