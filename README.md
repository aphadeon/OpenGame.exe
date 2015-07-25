# ![logo](Tools/logo.png) OpenGame.exe
A custom Ruby-powered game engine that supports RPG Maker games.  It aims to emulate the original 
RPG Maker functionality as closely as possible, and to later be expanded to provide new functions 
to empower RPG Maker games.

**OpenGame.exe is currently in an Alpha phase - expect significant issues for the time being. I 
am working as quickly as I can to get things ready for production use, but it takes time.**


Usage
-----
OpenGame.exe acts as a drop-in replacement for RPG Maker's Game.exe. RPG Maker XP, VX, and Ace are 
supported.  Make sure you also add the dependency DLLs to the game's /System/ folder.


Features
--------
The engine will soon be capable of emulating vanilla RPG Maker XP, VX, and VX Ace games. In 
addition the engine provides additional features.  There are command line switches to enable
the console in older RPG Maker versions, as well as forcing a particular version of RGSS emulation.

*OpenGame.exe console* - will display the console window

*OpenGame.exe rgss1* - would force RGSS1 backend regardless of RPG Maker version.

*OpenGame.exe rgss2* - would force RGSS2 backend regardless of RPG Maker version.

*OpenGame.exe rgss3* - would force RGSS3 backend regardless of RPG Maker version.


The rendering backend is now OpenGL - this requires a (slightly) more modern machine, but will 
offer superior performance on compatible machines.  The specific target OpenGL version is yet to 
be decided.

More enhanced features will be added in time.


Limitations
-----------
There are two significant features of "vanilla" Game.exe behaviour that will not be emulated for
deliberate reasons.

The first is MP3 support.  This is due to the fact that there are licensing issues surrounding
the distribution and playback of MP3s, and it does not offer any significant benefit over the
OGG/Vorbis file format. Don't believe me? [See this link for MP3 licensing details.](http://mp3licensing.com/royalty/software.html)
[There *are* patents involved](http://mp3licensing.com/patents/index.html).

Secondly, encrypted RPG Maker archives will not be supported.  While the format is well known 
at this point, Enterbrain (the copyright holder for RPG Maker) has directly expressed that it is 
against their wishes for the encryption details to be investigated or made public. I will look 
into an alternative, external encryption method, as I do understand the significance of protecting
your game assets.


Building Process
----------------
We also use NuGet to manage dependencies- as long as you have package resolution enabled, 
this process should run automatically and install the required libraries.

A pre-build step exists, which invokes *Tools/build-deps.bat*, which copies the libraries to
their expected location in the Debug and Release binary output folders.

Basically, clone and build in Visual Studio should be all there is to it.  Mono/Xamarin Studio
(using xbuild) support is a high priority, but will not work correctly until further development.

After building, you may drop an RPG Maker project directly in the output folder for
simple testing, or copy the output files to your RPG Maker Project directory and run it from there.


Contributing
------------
Please see the [Contribution Guidelines](CONTRIBUTING.MD).


Credits and Thanks
------------------
Thanks to the [hbgames.org community](http://www.hbgames.org/) for helping me learn the RPG Maker 
series over the years.  

Thanks to vgvgf for demonstrating how to pack and unpack Table, Color, and Tone classes.  

Thanks to Xilef for the tip on using render-to-texture to emulate some of RPG Maker's features 
with OpenGL.

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