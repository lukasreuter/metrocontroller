# MetroController

When i first tried out Windows 8.1 i was a little bit dissapointed that it didn't support the Xbox360 controller for PC to navigating the new "Metro UI".
This little programm tries to remedy that and provides an easy way to control your PC with a XInput capable controller.
While it is not intended to be used to input text, the most likely scenario would be an gameing rig or an multimedia inhouse box.
Ps. Yes i am well aware that Steam Big Picture mode does alot of things this app does, but it does not provide an way to interface with the metro apps of windows.


## Checking out the Source

git clone https://luxxuor@bitbucket.org/luxxuor/metrocontroller.git

or

git clone https://github.com/Luxxuor/metrocontroller.git


## Building

Requires .NET 4.5 or above.
Open the solution file in Visual Studio [Express] 2013 and build (press F7) the release configuration.
You will find the the output in "{pathtotheclonedfolder}/MetroController/bin/Release".


## Credits

This project integrates the source code of the following libraries:

WindowsInput.dll (0.2.0.0) - Windows Input Simulator by Micheal Noonan (https://inputsimulator.codeplex.com/
																		https://github.com/michaelnoonan/inputsimulator)

J2i.Net.XInputWrapper.dll (1.0.0.0) - XInput Wrapper by Joel Ivory Johnson (http://www.j2i.net/blogEngine/post/2012/11/11/Using-XInput-to-access-an-Xbox-360-Controller-in-Managed-Code.aspx
																			https://twitter.com/j2inet)


## Licenses

This project uses the *** License





## Todo:
 - Need to find a better way to check if metro apps are running
 - Need to detect if fullscreen apps are running and disable the polling
 - Make buttons/shortcuts customizable
 - make the layout prettier
 - Wireless: query batteryinfo only every odd/x cycles of updating when polling is started, to save some performance (battery info arent changing that often)
 - Wired: dont query batterinfo on wired controllers as their info is not going to change
 - Add mouse controls
 - write a new Tostring method for XInputState
 - write more comments/add summaries for private methods/fields
 - use XInputCapabilties for the correct result of IsButtonPresent
 - make left and right during RWIN+TAB snap the currently highlighted/seleted window to the left/right
 - make dpad inputs repeated presses (e.g. when holding left on the keyboard your cursor moves until keyup, but on the controler it currently does not)
 - make the toast notification work
