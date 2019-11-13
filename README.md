**Discord Launcher/Tool**

As you may know, to reduce **_CPU usage_** discord processes automaticaly sets to a lower priority state. On a low end computer, this cause **interruptions** in the audio when talking to other people when the **CPU experiences a high load**. To mitigate this issue, the discord tool/launcher that I made **forces discord processes** to a **designated level of _priority_**. It can also work as a **launcher**, as there's the option to automatically start discord on launching the app.


**This launcher/tool is an one click solution to an _annoying problem_.**


**Full change log**

**v1.0.0.0**

-Initial build.

**v1.0.1.1-First feature update**

-Performance update.

-Added start minimized.

-Added auto launch discord app.

-Changed auto state set to ON, on app launch.

**v1.0.1.2**

-Added change log on version number/tray notification click.

-Minor bug fix(Handling when discord.exe is missing).

-Overall stability fixes.

-Added debug state for developing.

**v1.0.1.3** **First GitHub version.**

-Change log button GUI flow fix.

-Made change log window not resizable.

-Stability issues fixed.

**v1.0.1.4**

-Added updater service for the app.

-Changed app name to Discord Launcher.

**v1.0.2.0-Second feature update**

-Fixed a bug ocurring when closing the app from the icon tray.

-Massive changes to the app update system.

-Future updates will work only from version 1.0.2.0 and newer.

-Remove past updates from the change log.(for all logs visit the GitHub page) 

-Changed the looks of the change log window.

-App checks for updates at launch by default and only display a message if
it finds one available.

-Added check for updates button in the main menu.

-One click experience at startup.

**v1.0.2.1**

-Further application updater stability improvements.

-If Discord is already running when the app is starting, now the app displays a

message to confirm running state.

-Only one instance of the application is now allowed.

-Fixed a typo in the application update dialog.

**v1.0.2.2**

-Softcoded Discord Application path. Now Discord Launcher searches in 
the Registry for Discord's application path.

-Implemented settings feature. (BETA)

-The application checks if the .settings file is present in the folder at startup.

-The application settings persit between application updates if the application's
.exe directory is not changed. (Will be fixed when the application installer is 
implemented)

**v1.0.2.3**

-Further improvements to Discord's executable detection system.

-If the application encounters any issues, it prompts the user to 
manually select Discord's executable.

-Added Discord executable detection wizard in application settings.

-General stability fixes for the application setting system.

-Fixed a bug occurring when exiting the application with close discord process at 
application exit setting set to on.
