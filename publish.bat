@echo off
if "%~1"=="" goto usage
dotnet publish Altium.Generator\Altium.Generator.csproj -c Release -r %1 --self-contained=true -p:PublishSingleFile=True -p:PublishTrimmed=True -o "bin\%1"
dotnet publish Altium.Sorter\Altium.Sorter.csproj -c Release -r %1 --self-contained=true -p:PublishSingleFile=True -p:PublishTrimmed=True -o "bin\%1"
xcopy /I /Y Altium.Generator\dictionary\*.txt "bin\%1\dictionary"
goto exit

:usage
echo %0 RID
echo For RID list refer to https://docs.microsoft.com/en-us/dotnet/core/rid-catalog

:exit