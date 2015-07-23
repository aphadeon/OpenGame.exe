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
@echo Dependency check finished.
@echo.