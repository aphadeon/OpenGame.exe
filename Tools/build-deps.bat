@echo Checking dependencies...
@echo.
@if not exist %solutiondir%\bin\ mkdir %solutiondir%\bin\
@if not exist %solutiondir%\bin\Debug mkdir %solutiondir%\bin\Debug
@if not exist %solutiondir%\bin\Release mkdir %solutiondir%\bin\Release
@if not exist %solutiondir%\bin\Debug\System mkdir %solutiondir%\bin\Debug\System
@if not exist %solutiondir%\bin\Release\System mkdir %solutiondir%\bin\Release\System
copy %solutiondir%bin\RGSS\RGSS.dll %solutiondir%bin\Debug\System\RGSS.dll
copy %solutiondir%bin\RGSS\RGSS.dll %solutiondir%bin\Release\System\RGSS.dll
copy %solutiondir%packages\IronRuby.1.1.3\Lib\IronRuby.dll %solutiondir%\bin\Release\System\IronRuby.dll
copy %solutiondir%packages\IronRuby.1.1.3\Lib\IronRuby.dll %solutiondir%\bin\Debug\System\IronRuby.dll
copy %solutiondir%packages\IronRuby.1.1.3\Lib\IronRuby.Libraries.dll %solutiondir%\bin\Release\System\IronRuby.Libraries.dll
copy %solutiondir%packages\IronRuby.1.1.3\Lib\IronRuby.Libraries.dll %solutiondir%\bin\Debug\System\IronRuby.Libraries.dll
copy %solutiondir%packages\IronRuby.1.1.3\Lib\Microsoft.Dynamic.dll %solutiondir%\bin\Release\System\Microsoft.Dynamic.dll
copy %solutiondir%packages\IronRuby.1.1.3\Lib\Microsoft.Dynamic.dll %solutiondir%\bin\Debug\System\Microsoft.Dynamic.dll
copy %solutiondir%packages\IronRuby.1.1.3\Lib\Microsoft.Scripting.dll %solutiondir%\bin\Release\System\Microsoft.Scripting.dll
copy %solutiondir%packages\IronRuby.1.1.3\Lib\Microsoft.Scripting.dll %solutiondir%\bin\Debug\System\Microsoft.Scripting.dll
copy %solutiondir%packages\OpenTK.1.1.1589.5942\lib\NET40\OpenTK.dll %solutiondir%\bin\Debug\System\OpenTK.dll
copy %solutiondir%packages\OpenTK.1.1.1589.5942\lib\NET40\OpenTK.dll %solutiondir%\bin\Release\System\OpenTK.dll
@echo Dependency check finished.
@echo.