@echo off
chcp 65001 >nul
title GitHub PR Monitor — muonline

:loop
cls
echo [%date% %time%] Checking for new PRs...
echo.

REM Fetch latest PR info
curl -s https://api.github.com/repos/exemic8973/muonline/pulls > pr_list.json 2>&1

REM Count open PRs
for /f "tokens=*" %%a in ('powershell -Command "$j = Get-Content pr_list.json | ConvertFrom-Json; Write-Host $j.Count"') do set PR_COUNT=%%a

if "%PR_COUNT%"=="" set PR_COUNT=0
echo Open PRs: %PR_COUNT%

if %PR_COUNT% GTR 0 (
    echo.
    echo ===== NEW PRs FOUND =====
    powershell -Command "$j = Get-Content pr_list.json | ConvertFrom-Json; foreach($p in $j) { Write-Host ('#' + $p.number + ' - ' + $p.title + ' [' + $p.state + '] by ' + $p.user.login) }"
    echo ========================
)

del pr_list.json 2>nul

timeout /t 120 /nobreak >nul
goto loop
