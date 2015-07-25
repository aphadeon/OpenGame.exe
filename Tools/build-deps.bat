@echo Checking dependencies...
@echo.
@if not exist %solutiondir%lib\bin\ mkdir %solutiondir%lib\bin\
@if not exist %solutiondir%lib\bin\IronRuby.dll (
	@echo Building IronRuby...
	@echo.
	%msbuildbinpath%\msbuild %solutiondir%\lib\IronLanguages\Solutions\Ruby.sln /p:Configuration=Release /verbosity:m /property:WarningLevel=0,TargetFrameworkVersion=4.0,DelaySign=false
	copy %solutiondir%lib\IronLanguages\bin\Release\IronRuby.dll %solutiondir%lib\bin\IronRuby.dll
	copy %solutiondir%lib\IronLanguages\bin\Release\IronRuby.Libraries.dll %solutiondir%lib\bin\IronRuby.Libraries.dll
	copy %solutiondir%lib\IronLanguages\bin\Release\Microsoft.Dynamic.dll %solutiondir%lib\bin\Microsoft.Dynamic.dll
	copy %solutiondir%lib\IronLanguages\bin\Release\Microsoft.Scripting.dll %solutiondir%lib\bin\Microsoft.Scripting.dll
	@echo.
)
@if not exist %solutiondir%\bin\ mkdir %solutiondir%\bin\
@if not exist %solutiondir%\bin\Debug mkdir %solutiondir%\bin\Debug
@if not exist %solutiondir%\bin\Release mkdir %solutiondir%\bin\Release
@if not exist %solutiondir%\bin\Debug\System mkdir %solutiondir%\bin\Debug\System
@if not exist %solutiondir%\bin\Release\System mkdir %solutiondir%\bin\Release\System
copy %solutiondir%lib\bin\IronRuby.dll %solutiondir%\bin\Release\System\IronRuby.dll
copy %solutiondir%lib\bin\IronRuby.dll %solutiondir%\bin\Debug\System\IronRuby.dll
copy %solutiondir%lib\bin\IronRuby.Libraries.dll %solutiondir%\bin\Release\System\IronRuby.Libraries.dll
copy %solutiondir%lib\bin\IronRuby.Libraries.dll %solutiondir%\bin\Debug\System\IronRuby.Libraries.dll
copy %solutiondir%lib\bin\Microsoft.Dynamic.dll %solutiondir%\bin\Release\System\Microsoft.Dynamic.dll
copy %solutiondir%lib\bin\Microsoft.Dynamic.dll %solutiondir%\bin\Debug\System\Microsoft.Dynamic.dll
copy %solutiondir%lib\bin\Microsoft.Scripting.dll %solutiondir%\bin\Release\System\Microsoft.Scripting.dll
copy %solutiondir%lib\bin\Microsoft.Scripting.dll %solutiondir%\bin\Debug\System\Microsoft.Scripting.dll
copy %solutiondir%packages\OpenTK.1.1.1589.5942\lib\NET40\OpenTK.dll %solutiondir%\bin\Debug\System\OpenTK.dll
copy %solutiondir%packages\OpenTK.1.1.1589.5942\lib\NET40\OpenTK.dll %solutiondir%\bin\Release\System\OpenTK.dll
@echo Dependency check finished.
@echo.