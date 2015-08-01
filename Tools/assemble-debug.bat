if %OS% == Windows_NT goto WINDOWS
then
:
fi 2> /dev/null

NAME=$0
NAME=${NAME%.bat}
NAME=${NAME##*/}

mkdir -p ./bin/Debug/System

cp ./lib/IronRuby.dll ./bin/Debug/System/
cp ./lib/IronRuby.Libraries.dll ./bin/Debug/System/
cp ./lib/Microsoft.Dynamic.dll ./bin/Debug/System/
cp ./lib/Microsoft.Scripting.dll ./bin/Debug/System/
cp ./packages/OpenTK.1.1.1589.5942/lib/NET40/OpenTK.dll ./bin/Debug/System/
cp ./Tools/OpenGame.exe.config ./bin/Debug/

exit

:WINDOWS
@echo off
set NAME=%~n0

if not exist .\bin\ mkdir .\bin\
if not exist .\bin\Debug mkdir .\bin\Debug
if not exist .\bin\Debug\System mkdir .\bin\Debug\System

copy .\lib\csogg.dll .\bin\Debug\System\csogg.dll
copy .\lib\csvorbis.dll .\bin\Debug\System\csvorbis.dll
copy .\lib\IronRuby.dll .\bin\Debug\System\IronRuby.dll
copy .\lib\IronRuby.Libraries.dll .\bin\Debug\System\IronRuby.Libraries.dll
copy .\lib\Microsoft.Dynamic.dll .\bin\Debug\System\Microsoft.Dynamic.dll
copy .\lib\Microsoft.Scripting.dll .\bin\Debug\System\Microsoft.Scripting.dll
copy .\packages\OpenTK.1.1.1589.5942\lib\NET40\OpenTK.dll .\bin\Debug\System\OpenTK.dll