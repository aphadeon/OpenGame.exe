@echo Checking dependencies...
@echo.
@if not exist %solutiondir%lib\bin\ mkdir %solutiondir%lib\bin\
@if not exist %solutiondir%lib\bin\Debug mkdir %solutiondir%lib\bin\Debug
@if not exist %solutiondir%lib\bin\Release mkdir %solutiondir%lib\bin\Release
@if not exist %solutiondir%lib\bin\Release\IronRuby.dll (
	@echo Building IronRuby...
	@echo.
	%msbuildbinpath%\msbuild %solutiondir%\lib\IronLanguages\Solutions\Ruby.sln /p:Configuration=Release /verbosity:m /property:WarningLevel=0,TargetFrameworkVersion=4.0,DelaySign=false
	copy %solutiondir%lib\IronLanguages\bin\Release\IronRuby.dll %solutiondir%lib\bin\Release\IronRuby.dll
	copy %solutiondir%lib\IronLanguages\bin\Release\IronRuby.Libraries.dll %solutiondir%lib\bin\Release\IronRuby.Libraries.dll
	copy %solutiondir%lib\IronLanguages\bin\Release\Microsoft.Dynamic.dll %solutiondir%lib\bin\Release\Microsoft.Dynamic.dll
	copy %solutiondir%lib\IronLanguages\bin\Release\Microsoft.Scripting.dll %solutiondir%lib\bin\Release\Microsoft.Scripting.dll
	@echo.
)
@if not exist %solutiondir%lib\bin\Release\OpenTK.dll (
	@echo Building OpenTK Release
	@echo.
	%msbuildbinpath%\msbuild %solutiondir%\lib\OpenTK\OpenTK.sln /p:Configuration=Release /verbosity:m /property:TargetFrameworkVersion=4.0,DelaySign=false
	copy %solutiondir%lib\OpenTK\Binaries\OpenTK\Release\OpenTK.dll %solutiondir%lib\bin\Release\OpenTK.dll
	@echo.
)
@if not exist %solutiondir%lib\bin\Debug\OpenTK.dll (
	@echo Building OpenTK Debug
	@echo.
	%msbuildbinpath%\msbuild %solutiondir%\lib\OpenTK\OpenTK.sln /p:Configuration=Debug /verbosity:m /property:TargetFrameworkVersion=4.0,DelaySign=false
	copy %solutiondir%lib\OpenTK\Binaries\OpenTK\Debug\OpenTK.dll %solutiondir%lib\bin\Debug\OpenTK.dll
	@echo.
)
@if not exist %solutiondir%\bin\ mkdir %solutiondir%\bin\
@if not exist %solutiondir%\bin\Debug mkdir %solutiondir%\bin\Debug
@if not exist %solutiondir%\bin\Debug\System mkdir %solutiondir%\bin\Debug\System
@if not exist %solutiondir%\bin\Release\System mkdir %solutiondir%\bin\Release\System
copy %solutiondir%lib\bin\Release\IronRuby.dll %solutiondir%\bin\Release\System\IronRuby.dll
copy %solutiondir%lib\bin\Release\IronRuby.dll %solutiondir%\bin\Debug\System\IronRuby.dll
copy %solutiondir%lib\bin\Release\IronRuby.Libraries.dll %solutiondir%\bin\Release\System\IronRuby.Libraries.dll
copy %solutiondir%lib\bin\Release\IronRuby.Libraries.dll %solutiondir%\bin\Debug\System\IronRuby.Libraries.dll
copy %solutiondir%lib\bin\Release\Microsoft.Dynamic.dll %solutiondir%\bin\Release\System\Microsoft.Dynamic.dll
copy %solutiondir%lib\bin\Release\Microsoft.Dynamic.dll %solutiondir%\bin\Debug\System\Microsoft.Dynamic.dll
copy %solutiondir%lib\bin\Release\Microsoft.Scripting.dll %solutiondir%\bin\Release\System\Microsoft.Scripting.dll
copy %solutiondir%lib\bin\Release\Microsoft.Scripting.dll %solutiondir%\bin\Debug\System\Microsoft.Scripting.dll
copy %solutiondir%lib\bin\Release\OpenTK.dll %solutiondir%\bin\Release\System\OpenTK.dll
copy %solutiondir%lib\bin\Debug\OpenTK.dll %solutiondir%\bin\Debug\System\OpenTK.dll
@echo Dependency check finished.
@echo.