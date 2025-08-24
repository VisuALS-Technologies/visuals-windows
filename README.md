# VisuALS for Windows

Our mission is to love our neighbors by restoring independence, dignity, and hope through affordable assistive technology solutions. Specifically, VisuALS is a software application that enables those with debilitating conditions to use their eyes to communicate. The software works with off-the-shelf computers and eye-tracking devices to allow the patient to speak (text-to-speech), browse the web, access social networks, and use other applications.

The main differentiators of VisuALS over commercially available eye-tracking based augmentative and alternative communications devices are:

- **Affordability:** This free software combined with commercially available off-the-shelf hardware is much less expensive than packaged solutions.
- **Ease-of-Use:** Most commercially available products have focused so much on adding features that the systems have become overly complex. VisuALS aims to have a simple and intuitive interface.

## Status

Currently the software is not setup to work with any eyetrackers, as we are currently getting funding to afford eye trackers for development. The application will run, but it only works with the mouse cursor.

## Installation

 - Download the software [here](https://github.com/VisuALSeyegaze/VisuALS_Windows/raw/master/installers/VisuALS_Windows_V2.0_Installer.exe)
 - Double click the package to begin the install process.
 - You may see a warning that the software may be unsafe. Click the "More Info" link and then click the "Run Anyway" button to install the software.
 - Follow the instructions.

## Tips & Tricks

### Recommended Hardware

 - VisuALS should run on any PC that can run Windows 10 or 11.
 - A tablet PC (like the Microsoft Surface Pro) may preferrable for mobile setups.
 - A mounting solution may also be a good idea for a mobile setup. We have found [The Joy Factory](https://thejoyfactory.com/product-line/?_bc_fsnf=1&product_type=Mounts) to provide quality mounts at a good value.

### Recommended PC Setups

 - Update Windows software to the latest available version
	 - Go to Settings (Windows icon at lower left, then select the gear symbol), then select “Update & Security”
	 - Check for Updates; if there are Updates (Pending install will show, then Pending restart).
	 - To completely install them, Restart the computer at that point (a button should pop up at the bottom of the Updates list that says “Restart now” - click that).
	 - You may need to repeat this process several times to get all the updates.
	 - This step can easily take several hours.
 - Change computer settings to make the system easier for use by eye gaze users
	 - Not all of these options may be available on your system. If you don't see the appropriate settings, you should be fine.
	 - Remove unneeded toolbar icons
		 - Right click on icons of any applications not likely to be used and select “Unpin from toolbar”
	 -  Set power settings to “Never” so that the system will stay active (otherwise someone will need to physically push a button to wake the system up)
		 - Go to Settings,
		 - Click on System,and then on “Power & sleep”
		 - Set all screen and sleep settings to “never”
	 - Disable touch keyboard (you will use the VisuALS keyboard instead)
		 - Go to Settings
		 - Click on Devices
		 - Click on Typing
		 - Toggle “Show the touch keyboard when not in tablet mode and there’s no keyboard attached” to off
	 - Disable handwriting panel (this can block the screen when the user is trying to communicate)
		 - Go to settings
		 - Click on Devices
		 - Click on Pen & Windows Ink
		 - Make sure that the dropdown “When I tap a text field with my pen..” is set to “Only in tablet mode.”
	 - Turn off the Windows error dialogue (this can also keep the system from operating without manual intervention)
		 - Search for edit group policy in the taskbar
		 - Navigate to computer configuration
		 - Click on the administrative templates folder
		 - Click on the windows components folder
		 - Click on the windows error reporting folder
		 - Double click on prevent display of the user display interface for critical errors
		 - Click the enabled radio button
		 - Click ok
	 - Disable automatic touch keyboard (you'll be using the VisuALS keyboard)
		 - Search services
		 - Find “Touch Keyboard and handwriting…” under services
		 - Double-click on that row in “Startup Type” column
		 - Change “Startup type” (middle of window) to Disabled and then click “stop”.
		 - Click “Apply”
		 - Finally, click ok and then close the services window (X at upper right corner of window).

## License

The VisuALS Windows code and the VisuALS branding is owned by Eagle Works, LLC.
The VisuALS Windows code is provided under the GPLv3 License (see the [LICENSE.txt](LICENSE.txt)).

## Authors

The original VisuALS software evolved out of student projects by Electrical and Computer Engineering students at Oklahoma Christian University. These original authors include:

* **Allison Chilton** 
* **Daniel Griffin**
* **Drew Harris**
* **Josh Bilello**
* **Aubrey Gonzalez**
* **Preston Kemp**
* **Tyler Sriver**

A company was then formed, VisuALS Technology Solutions, LLC, to productize and commercialize the software. The developers who furthered the software into the version that has been released here include:

* **McKenna Gameros**
* **Brendan McKinley**
* **Addison Schwamb**
* **Preston Seaman**
* **Russell Bian**
* **Nathaniel Markham**
* **Austin Merritt**
* **Andrew Ash**
* **Ian Robison**
* **Zachary Walden**

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project. **update link**

## Acknowledgments

* Special thanks to Ash Srinivas for originally identifying the need for VisuALS and for working with the original student team.
* Thanks also to Steve Maher, Russ McGuire, Austin McRay, Jevon Seaman, Kevin McGuire, Seth Reiter, and all the other non-developers who helped make VisuALS a reality.
* We also thank the original investors in VisuALS Technology Solutions, LLC who funded the development, and Oklahoma Christian University which continues to provide support for the VisuALS project.
* We dedicate VisuALS to the memory of Weyton, Carl, Steve, and the other early users of VisuALS who provided the inspiration and feedback for making this the best solution it can be.
