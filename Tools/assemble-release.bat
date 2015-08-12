if %OS% == Windows_NT goto WINDOWS
then
:
fi 2> /dev/null

NAME=$0
NAME=${NAME%.bat}
NAME=${NAME##*/}

mkdir -p ./bin/Release/System

cp ./lib/IronRuby.dll ./bin/Release/System/
cp ./lib/IronRuby.Libraries.dll ./bin/Release/System/
cp ./lib/Microsoft.Dynamic.dll ./bin/Release/System/
cp ./lib/Microsoft.Scripting.dll ./bin/Release/System/
cp ./lib/OpenTK.dll ./bin/Release/System/
cp ./Tools/OpenGame.exe.config ./bin/Release/

exit

:WINDOWS
@echo off
set NAME=%~n0

if not exist .\bin\ mkdir .\bin\
if not exist .\bin\Release mkdir .\bin\Release
if not exist .\bin\Release\System mkdir .\bin\Release\System

copy .\lib\csogg.dll .\bin\Release\System\csogg.dll
copy .\lib\csvorbis.dll .\bin\Release\System\csvorbis.dll
copy .\lib\IronRuby.dll .\bin\Release\System\IronRuby.dll
copy .\lib\IronRuby.Libraries.dll .\bin\Release\System\IronRuby.Libraries.dll
copy .\lib\Microsoft.Dynamic.dll .\bin\Release\System\Microsoft.Dynamic.dll
copy .\lib\Microsoft.Scripting.dll .\bin\Release\System\Microsoft.Scripting.dll
copy .\lib\OpenTK.dll .\bin\Release\System\OpenTK.dll
