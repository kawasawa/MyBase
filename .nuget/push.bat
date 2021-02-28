@echo off
cd /d %~dp0

echo [INFO] Set variables
set ROOT_DIR=%~dp0
set ROOT_DIR=%ROOT_DIR:~0,-1%
for %%i in (%ROOT_DIR%) do set ROOT_DIR=%%~dpi
set ROOT_DIR=%ROOT_DIR:~0,-1%
for %%i in (%ROOT_DIR%) do set PACKAGE_NAME=%%~nxi
echo PACKAGE_NAME=%PACKAGE_NAME%

echo [INFO] Push the package
nuget push -Source https://www.nuget.org/api/v2/package %PACKAGE_NAME%*.nupkg
if %ERRORLEVEL% == 1 (
    echo [ERROR] Failed to push
    pause
    goto END
)

echo [INFO] Clean up temporary file
del %PACKAGE_NAME%.*.nupkg

timeout 3

:END