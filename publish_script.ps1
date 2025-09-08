# ReportHub Publishing Script
# This script creates a production-ready deployment package

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$OutputPath = "./publish",
    [switch]$SelfContained = $true
)

Write-Host "=========================================" -ForegroundColor Green
Write-Host "ReportHub - Production Deployment Package" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""

# Ensure we're in the correct directory
if (-not (Test-Path "ReportHub.sln")) {
    Write-Error "Please run this script from the ReportHub solution root directory"
    exit 1
}

Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Runtime: $Runtime" -ForegroundColor Yellow
Write-Host "Output Path: $OutputPath" -ForegroundColor Yellow
Write-Host "Self-Contained: $SelfContained" -ForegroundColor Yellow
Write-Host ""

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Cyan
dotnet clean -c $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "Clean failed"
    exit 1
}

# Restore dependencies
Write-Host "Restoring dependencies..." -ForegroundColor Cyan
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed"
    exit 1
}

# Build the solution
Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

# Create output directory
if (Test-Path $OutputPath) {
    Write-Host "Removing existing output directory..." -ForegroundColor Cyan
    Remove-Item -Recurse -Force $OutputPath
}
New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null

# Publish the application
Write-Host "Publishing application..." -ForegroundColor Cyan
if ($SelfContained) {
    dotnet publish ReportHub/ReportHub.csproj -c $Configuration -r $Runtime --self-contained true -o "$OutputPath/$Runtime" --verbosity minimal
} else {
    dotnet publish ReportHub/ReportHub.csproj -c $Configuration -o "$OutputPath/framework-dependent" --verbosity minimal
}

if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed"
    exit 1
}

# Copy additional files
Write-Host "Copying additional files..." -ForegroundColor Cyan

# Copy documentation
if (Test-Path "docs") {
    $docsPath = if ($SelfContained) { "$OutputPath/$Runtime/docs" } else { "$OutputPath/framework-dependent/docs" }
    Copy-Item -Recurse "docs" $docsPath
}

# Copy sample files
if (Test-Path "tests/template_test_requests.json") {
    $testsPath = if ($SelfContained) { "$OutputPath/$Runtime/samples" } else { "$OutputPath/framework-dependent/samples" }
    New-Item -ItemType Directory -Force -Path $testsPath | Out-Null
    Copy-Item "tests/template_test_requests.json" "$testsPath/sample_requests.json"
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "DEPLOYMENT PACKAGE READY" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host "Output Path: $OutputPath" -ForegroundColor Yellow
Write-Host ""
Write-Host "To deploy:" -ForegroundColor Cyan
Write-Host "  1. Copy the contents of '$OutputPath' to your server" -ForegroundColor White
Write-Host "  2. Run: dotnet ReportHub.dll" -ForegroundColor White
Write-Host "  3. Access the API at http://localhost:5000" -ForegroundColor White
Write-Host ""
Write-Host "READY FOR DEPLOYMENT!" -ForegroundColor Green
Write-Host ""