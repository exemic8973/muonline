# Start the OpenMU game server
# Run this before launching the game client

$composeFile = Join-Path $PSScriptRoot "server" "docker-compose.yml"

# Check if containers are already running
$running = docker ps --filter "name=openmu-startup" --filter "status=running" --quiet 2>$null
if ($running) {
    Write-Host "✅ OpenMU server is already running." -ForegroundColor Green
    exit 0
}

Write-Host "🚀 Starting OpenMU server via Docker..." -ForegroundColor Cyan
docker compose -f $composeFile up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Server started!" -ForegroundColor Green
    Write-Host "   Connect Server: localhost:44405"
    Write-Host "   Admin Panel:    http://localhost/  (admin / muonline)"
} else {
    Write-Host "❌ Failed to start server." -ForegroundColor Red
    Write-Host "   Try: docker compose -f `"$composeFile`" up -d"
}
