@echo off
cd /d %~dp0

set OUTPUT_DIR=%userprofile%\nuget-local
set ROOT_DIR=%~dp0
set ROOT_DIR=%ROOT_DIR:~0,-1%
for %%i in (%ROOT_DIR%) do set ROOT_DIR=%%~dpi
set ROOT_DIR=%ROOT_DIR:~0,-1%
for %%i in (%ROOT_DIR%) do set PACKAGE_NAME=%%~nxi

echo [INFO] Clean up old file.
del %PACKAGE_NAME%.*.nupkg > NUL 2>&1
echo done.

echo [INFO] Build the solution.
dotnet build ..\%PACKAGE_NAME%.sln -c=Release
if %ERRORLEVEL% == 1 (
    echo [ERROR] Failed to build
    pause
    goto END
)
echo done.

echo [INFO] Create the package.
nuget pack package.nuspec
if %ERRORLEVEL% == 1 (
    echo [ERROR] Failed to create package
    pause
    goto END
)
echo done.

echo [INFO] Copy to '%OUTPUT_DIR%'.
mkdir %OUTPUT_DIR% > NUL 2>&1
copy *.nupkg %OUTPUT_DIR%
echo done.

timeout 5

:END
