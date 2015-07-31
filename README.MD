# ![logo](Tools/logo.png) OpenGame.exe
A custom Ruby-powered game engine that supports RPG Maker games.  It aims to emulate the original 
RPG Maker's *Game.exe* functionality as closely as possible, and to be expanded to provide new 
functions to empower RPG Maker games.

**OpenGame.exe is currently in an Alpha phase - expect significant issues for the time being. I 
am working as quickly as I can to get things ready for production use, but it takes time.**


Usage
-----
OpenGame.exe acts as a drop-in replacement for RPG Maker's Game.exe. RPG Maker XP, VX, and Ace are 
supported.  Make sure you also add the dependency DLLs to the game's /System/ folder.

There may be some a few compatibility issues.  We have done quite a bit to keep these as rare as
possible.  But some things work differently, deliberately.  For example, the backend uses OpenGL.
This means that bitmaps are not stored in the same place or format as the original, so DLL-powered
scripts that modified them are going to have problems.  That said, the tradeoff is that this
enables us to add new extensions and methods that will make such scripts easier and less hack-ish
to implement.


Features
--------
The engine will soon be capable of emulating vanilla RPG Maker XP, VX, and VX Ace games. In 
addition the engine provides additional features.

There are a few command-line switches to enable new functionality.

*OpenGame.exe rgss1* - would force RGSS1 backend regardless of RPG Maker version.

*OpenGame.exe rgss2* - would force RGSS2 backend regardless of RPG Maker version.

*OpenGame.exe rgss3* - would force RGSS3 backend regardless of RPG Maker version.

*OpenGame.exe -rtp /path/to/rtp/* - would override the default RTP search location

*OpenGame.exe -addrtp /path/to/extra/rtp/* - would add an additional RTP search location

*OpenGame.exe -game /path/to/game/folder/* - would treat the specified path as the game directory


Play-testing features, such as the walk-through-walls and debug menus, which were activated by
command-line switches, are now disabled in Release builds, but operational via commandline (like 
before) in Debug builds.  The console window will always show for Debug builds, but not for Release.

The rendering backend is now OpenGL - this requires a (slightly) more modern machine, but will 
offer superior performance on compatible machines.  The specific target OpenGL version is yet to 
be decided.

More enhanced features will be added in time.


Cross-Platform
--------------
OpenGame.exe officially supports Windows, Mac, and Linux.  Other platforms may be supported later.
There are some things to know regarding running OpenGame.exe on non-Windows platforms.

First, know that because of the lack of a Windows registry, RTPs are not supported by default.
If you want to add RTP support, you will have to manually place the RTP files on your hard drive,
and launch the game with the -rtp command line switch in order to tell the engine where to find
them.

Example: *OpenGame.exe -rtp /path/to/rtp/*

The console window is not available by default for non-Windows operating systems due to technical
limitations. (You can manually set the project configuration to be a console application for these
targets, but this has to be done at compile time.)

Finally, for obvious reasons, Win32API will not be available for your scripts on non-Windows
operating systems.  A plugin system is planned for the future that will enable scripts to use
libraries without being neccessarily platform-specific.

**Important:** If you are on a Unix operating system, please set the executable bit for the files
*Tools/assemble-debug.bat* and *Tools/assemble-release.bat* as otherwise you will not get a 
complete product on building.


Limitations
-----------
There are two significant features of "vanilla" Game.exe behaviour that will not be emulated for
deliberate reasons.

The first is MP3 support.  This is due to the fact that there are licensing issues surrounding
the distribution and playback of MP3s, and it does not offer any significant benefit over the
OGG/Vorbis file format. Don't believe me? 
[See this link for MP3 licensing details.](http://mp3licensing.com/royalty/software.html)
[There *are* patents involved](http://mp3licensing.com/patents/index.html).
*Update: A workaround for the license issue is being investigated. No promises yet.*

Secondly, encrypted RPG Maker archives will not be supported.  While the format is well known 
at this point, Enterbrain (the copyright holder for RPG Maker) has directly expressed that it is 
against their wishes for the encryption details to be investigated or made public. I will look 
into an alternative, external encryption method, as I do understand the significance of protecting
your game assets.


Building from source
--------------------
We also use NuGet to manage dependencies- as long as you have package resolution enabled, 
this process should run automatically and install the required libraries.

A pre-build step exists, which invokes *Tools/assemble-[debug|release]*, which copies the libraries
to their expected location in the Debug and Release binary output folders.

Basically, clone and build in Visual Studio should be all there is to it.

After building, you may drop an RPG Maker project directly in the output folder for
simple testing, or copy the output files to your RPG Maker Project directory and run it from there.
Or make use of the -game commandline switch to specify the game location.


Contributing
------------
Please see the [Contribution Guidelines](CONTRIBUTING.MD). :eyes:


Credits and Thanks
------------------
Thanks to the [hbgames.org community](http://www.hbgames.org/) for helping me learn the RPG Maker 
series over the years.  

Thanks to Xilef for invaluable input that has accelerated this project significantly.

Thanks to vgvgf for demonstrating how to pack and unpack Table, Color, and Tone classes.  

Thanks to tmm1 for the soft Fiber implementation, and for granting me permission to use it in this
project.


License
-------
OpenGame.exe is licensed under a 
[Creative Commons Attribution 3.0 Unported License](http://creativecommons.org/licenses/by/3.0/).


Dependency License Information:
-------------------------------

IronRuby is licensed under [Apache License v2](http://www.apache.org/licenses/LICENSE-2.0).  
OpenTK is licensed under [The Open Toolkit library license](http://www.opentk.com/project/license).
