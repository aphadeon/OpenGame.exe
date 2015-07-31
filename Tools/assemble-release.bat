if %OS% == Windows_NT goto WINDOWS
then
:
fi 2> /dev/null

NAME=$0
NAME=${NAME%.bat}
NAME=${NAME##*/}

mkdir -p ./bin/Release/System

cp ./bin/RGSS/RGSS.dll ./bin/Release/System/
cp ./lib/IronRuby.dll ./bin/Release/System/
cp ./lib/IronRuby.Libraries.dll ./bin/Release/System/
cp ./lib/Microsoft.Dynamic.dll ./bin/Release/System/
cp ./lib/Microsoft.Scripting.dll ./bin/Release/System/
cp ./packages/OpenTK.1.1.1589.5942/lib/NET40/OpenTK.dll ./bin/Release/System/
cp ./Tools/OpenGame.exe.config ./bin/Release/

exit

:WINDOWS
@echo off
set NAME=%~n0

if not exist .\bin\ mkdir .\bin\
if not exist .\bin\Release mkdir .\bin\Release
if not exist .\bin\Release\System mkdir .\bin\Release\System

copy .\bin\RGSS\RGSS.dll .\bin\Release\System\RGSS.dll
copy .\lib\IronRuby.dll .\bin\Release\System\IronRuby.dll
copy .\lib\IronRuby.Libraries.dll .\bin\Release\System\IronRuby.Libraries.dll
copy .\lib\Microsoft.Dynamic.dll .\bin\Release\System\Microsoft.Dynamic.dll
copy .\lib\Microsoft.Scripting.dll .\bin\Release\System\Microsoft.Scripting.dll
copy .\packages\OpenTK.1.1.1589.5942\lib\NET40\OpenTK.dll .\bin\Release\System\OpenTK.dll