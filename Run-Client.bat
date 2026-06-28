@echo off
chcp 65001 >nul
title MuOnline Client

echo ============================================
echo   MuOnline - Private Server Launcher
echo ============================================
echo.

REM Check if server is running
docker ps --filter "name=openmu-startup" --filter "status=running" --quiet >nul 2>&1
if errorlevel 1 (
    echo [!] OpenMU server is NOT running.
    echo    Run Start-Server.ps1 first, or start Docker manually.
    echo.
)

echo [*] Building client...
dotnet build MuWinDX\MuWinDX.csproj -c Debug -p:MonoGameFramework=MonoGame.Framework.WindowsDX
if errorlevel 1 (
    echo [X] Build failed!
    pause
    exit /b 1
)

echo [*] Launching MuOnline...
echo    (First launch auto-downloads ~1.9 GB assets from Webzen)
echo.
dotnet run --project MuWinDX\MuWinDX.csproj -c Debug -p:MonoGameFramework=MonoGame.Framework.WindowsDX
pause
