@ECHO off

DEL /q build
dotnet clean source\DefaultSerialization\DefaultSerialization.csproj -c Release
dotnet clean source\DefaultSerialization.Test\DefaultSerialization.Test.csproj -c Release

dotnet test source\DefaultSerialization.Test\DefaultSerialization.Test.csproj -c Release -r build -l trx

IF %ERRORLEVEL% GTR 0 GOTO :end

dotnet pack source\DefaultSerialization\DefaultSerialization.csproj -c Release -o build /p:LOCAL_VERSION=true

:end