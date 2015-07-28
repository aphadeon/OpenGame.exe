@if not exist .\bin\ mkdir .\bin\
@if not exist .\bin\Debug mkdir .\bin\Debug
@if not exist .\bin\Release mkdir .\bin\Release
@if not exist .\bin\Debug\System mkdir .\bin\Debug\System
@if not exist .\bin\Release\System mkdir .\bin\Release\System
copy .\bin\RGSS\RGSS.dll .\bin\Debug\System\RGSS.dll
copy .\bin\RGSS\RGSS.dll .\bin\Release\System\RGSS.dll
copy .\lib\IronRuby.dll .\bin\Release\System\IronRuby.dll
copy .\lib\IronRuby.dll .\bin\Debug\System\IronRuby.dll
copy .\lib\IronRuby.Libraries.dll .\bin\Release\System\IronRuby.Libraries.dll
copy .\lib\IronRuby.Libraries.dll .\bin\Debug\System\IronRuby.Libraries.dll
copy .\lib\Microsoft.Dynamic.dll .\bin\Release\System\Microsoft.Dynamic.dll
copy .\lib\Microsoft.Dynamic.dll .\bin\Debug\System\Microsoft.Dynamic.dll
copy .\lib\Microsoft.Scripting.dll .\bin\Release\System\Microsoft.Scripting.dll
copy .\lib\Microsoft.Scripting.dll .\bin\Debug\System\Microsoft.Scripting.dll
copy .\packages\OpenTK.1.1.1589.5942\lib\NET40\OpenTK.dll .\bin\Debug\System\OpenTK.dll
copy .\packages\OpenTK.1.1.1589.5942\lib\NET40\OpenTK.dll .\bin\Release\System\OpenTK.dll