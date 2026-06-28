# Build and run the MuOnline client (DirectX)

$project = Join-Path $PSScriptRoot "MuWinDX" "MuWinDX.csproj"

# Check if server is running first
$serverRunning = docker ps --filter "name=openmu-startup" --filter "status=running" --quiet 2>$null
if (-not $serverRunning) {
    Write-Host "⚠️  OpenMU server is not running." -ForegroundColor Yellow
    Write-Host "   Run Start-Server.ps1 first, or the client won't connect."
    Write-Host ""
}

Write-Host "🔨 Building client..." -ForegroundColor Cyan
dotnet build $project -c Debug -p:MonoGameFramework=MonoGame.Framework.WindowsDX
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "🚀 Launching MuOnline..." -ForegroundColor Green
Write-Host "   (First launch will auto-download game assets ~1.9 GB from Webzen)"
Write-Host ""
dotnet run --project $project -c Debug -p:MonoGameFramework=MonoGame.Framework.WindowsDX
