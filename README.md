# MetroController

## Introduction

When i first tried out Windows 8.1 i was a little bit disappointed that it didn't support the Xbox360 controller for PC or any other XInput capable controller for navigating the new "Metro UI".
This little program tries to remedy that and provides an easy way to control your PC with a XInput capable controller.
While it is not intended to be used to input text, the most likely scenario would be a gaming rig or a in-house multimedia/streaming box.
Ps. Yes i am well aware that Steam Big Picture mode does a lot of things this app does, but it does not provide a convenient way to interface with the metro apps of windows.


## Controls

Double Left Mouse Click on the Trayicon opens the diagnostics panel, right clicking it closes the program.

Controller Layout:
*A Button:* simulates Enter press
*B Button:* Tapping switches to last used app, holding shows appchooser (simulates WIN+TAB press)
*X Button:* Shows context menu
*Y Button:* Switches between tiles and list of all installed apps (simulates CTRL+TAB press)
*Start Button:* Shows Charmbar
*Select Button:* currently not used
*D-pad:* simulate arrow keys
*Left Shoulder Button:* simulates Left Mouse Click 
*Right Shoulder Button:* simulates Right Mouse Click
*Left Trigger:* simulates Up Mouse Scroll
*Right Trigger:* simulates Down Mouse Scroll
*Left Thumbstick:* currently not used
*Right Thumbstick:* simulates Mouse Movement



## Building

First checkout the source by either using 

``` sh
git clone https://bitbucket.org/LukasReuter/metrocontroller.git
```

or

``` sh
git clone https://github.com/Luxxuor/metrocontroller.git
```

to get the source. Building requires .NET 4.5 or above and Microsoft Visual Studio [Express] 2013 or higher.
Open the solution file in Visual Studio [Express] 2013 and build the release configuration.
You will find the the output in "{pathtotheclonedfolder}/MetroController/bin/Release". You can find a build of the latest version under the Downloads section of the bitbucket procject page. 
Forks and Pull requests are welcome.


## Credits

This project integrates the source code of the following libraries:

*WindowsInput.dll* (0.2.0.0) - Windows Input Simulator by Micheal Noonan ([Project on Codeplex](https://inputsimulator.codeplex.com/)
																		  [Source on Github](https://github.com/michaelnoonan/inputsimulator))

*J2i.Net.XInputWrapper.dll* (1.0.0.0) - XInput Wrapper by Joel Ivory Johnson ([Source](http://www.j2i.net/blogEngine/post/2012/11/11/Using-XInput-to-access-an-Xbox-360-Controller-in-Managed-Code.aspx)
																			  [Twitter](https://twitter.com/j2inet))


## License

This project uses the MIT License. See the the `LICENSE` file for further information.
