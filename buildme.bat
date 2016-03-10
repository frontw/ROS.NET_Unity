@echo off

for /D %%A in (C:\Windows\Microsoft.NET\Framework*) do IF EXIST %%A\v4* for /f "tokens=*" %%B in ('dir /b /a:d %%A') do set TOOLSPATH=%%A\%%B
if NOT DEFINED TOOLSPATH goto :Fail
if NOT EXIST %TOOLSPATH%\msbuild.exe goto :Fail
%TOOLSPATH%\msbuild.exe UnityROS.NETHack.sln /t:rebuild
goto :eof
:Fail
echo "Could not locate msbuild! This build script should work with .NET build tools version >v4.0.*"
echo "If you have not installed visual studio 2008 or higher, or have not installed .NET 3.5 and/or 4.0, do so"
echo "Otherwise, open UnityROS.NETHack.sln in Visual Studio, and verify that it builds that way."