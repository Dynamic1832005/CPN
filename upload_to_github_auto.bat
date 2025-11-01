@echo off
title Auto Upload Project to GitHub
echo ===============================
echo   Uploading Project to GitHub
echo ===============================
echo.

REM ==== Set your GitHub info here ====
set "REPO_URL=https://github.com/Dynamic1832005/IT-Career-Path-Navigator.git"
set "USER_NAME=Nyi Nyi Htwe"
set "USER_EMAIL=dynamic1832005@gmail.com"
set "PROJECT_PATH=D:\IT Career Path Navigator"

REM ==== Change to project folder ====
cd /d "%PROJECT_PATH%"

REM ==== Initialize Git if not already ====
if not exist ".git" (
    echo Initializing new git repository...
    git init
    git config user.name "%USER_NAME%"
    git config user.email "%USER_EMAIL%"
    git remote add origin "%REPO_URL%"
)

REM ==== Stage all changes ====
git add -A

REM ==== Check if there are changes to commit ====
for /f "delims=" %%i in ('git status --porcelain') do set "CHANGES=1"

if defined CHANGES (
    echo Changes detected, committing...
    for /f "usebackq delims=" %%I in (`powershell -NoProfile -Command "Get-Date -Format 'yyyy-MM-dd_HH-mm-ss'"`) do set "TS=%%I"
    git commit -m "Auto update %TS%"
) else (
    echo No changes to commit.
)

REM ==== Push to GitHub ====
echo.
echo Uploading to GitHub...
git branch -M main
git push -u origin main

echo.
echo ✅ Upload Complete!
pause
