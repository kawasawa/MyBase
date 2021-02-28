@echo off
cd /d %~dp0

echo [INFO] Set variables
set OUTPUT_DIR=%userprofile%\nuget_local
set ROOT_DIR=%~dp0
set ROOT_DIR=%ROOT_DIR:~0,-1%
for %%i in (%ROOT_DIR%) do set ROOT_DIR=%%~dpi
set ROOT_DIR=%ROOT_DIR:~0,-1%
for %%i in (%ROOT_DIR%) do set PACKAGE_NAME=%%~nxi
echo PACKAGE_NAME=%PACKAGE_NAME%, OUTPUT_DIR=%OUTPUT_DIR%

echo [INFO] Clean up old file
del %PACKAGE_NAME%.*.nupkg > NUL 2>&1

echo [INFO] Build the solution
dotnet build ..\%PACKAGE_NAME%.sln -c=Release
if %ERRORLEVEL% == 1 (
    echo [ERROR] Failed to build
    pause
    goto END
)

echo [INFO] Create package
nuget pack package.nuspec
if %ERRORLEVEL% == 1 (
    echo [ERROR] Failed to create package
    pause
    goto END
)

echo [INFO] Copy to output directory
mkdir %OUTPUT_DIR% > NUL 2>&1
copy *.nupkg %OUTPUT_DIR%

timeout 3

:END
