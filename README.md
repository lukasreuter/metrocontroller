# MetroController

## Introduction

When i first tried out Windows 8.1 i was a little bit disappointed that it didn't support the Xbox360 controller for PC to navigating the new "Metro UI".
This little program tries to remedy that and provides an easy way to control your PC with a XInput capable controller.
While it is not intended to be used to input text, the most likely scenario would be a gaming rig or a in-house multimedia/streaming box.
Ps. Yes i am well aware that Steam Big Picture mode does a lot of things this app does, but it does not provide a convenient way to interface with the metro apps of windows.


## Checking out the Source

``` sh
git clone https://luxxuor@bitbucket.org/luxxuor/metrocontroller.git
```

or

``` sh
git clone https://github.com/Luxxuor/metrocontroller.git
```

## Building

Requires .NET 4.5 or above.
Open the solution file in Visual Studio [Express] 2013 and build the release configuration.
You will find the the output in "{pathtotheclonedfolder}/MetroController/bin/Release".


## Credits

This project integrates the source code of the following libraries:

*WindowsInput.dll* (0.2.0.0) - Windows Input Simulator by Micheal Noonan ([Project on Codeplex](https://inputsimulator.codeplex.com/)
																		  [Source on Github](https://github.com/michaelnoonan/inputsimulator))

*J2i.Net.XInputWrapper.dll* (1.0.0.0) - XInput Wrapper by Joel Ivory Johnson ([Source](http://www.j2i.net/blogEngine/post/2012/11/11/Using-XInput-to-access-an-Xbox-360-Controller-in-Managed-Code.aspx)
																			  [Twitter](https://twitter.com/j2inet))


## Licenses

This project uses the MIT License. See the the `LICENSE` file for further information.


## To-do:
 - Need to find a better way to check if metro apps are running
 - Need to detect if full-screen apps are running and disable the polling
 - Make buttons/shortcuts customizable
 - make the layout prettier
 - Wireless: query battery info only every odd/x cycles of updating when polling is started, to save some performance (battery info aren't changing that often)
 - Add mouse controls
 - make left and right during RWIN+TAB snap the currently highlighted/selected window to the left/right
 - make dpad inputs repeated presses (e.g. when holding left on the keyboard your cursor moves until key-up, but on the controller it currently does not) (does not work because we only get the callback that the input changed once so there is no continuous input)
 - make the toast notification work
 - make it autostart with windows
