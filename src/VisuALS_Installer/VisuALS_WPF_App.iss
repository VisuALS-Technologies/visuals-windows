[Setup]
AppName=VisuALS 3
AppVersion=3.0.0.0
DefaultDirName={autopf}\VisuALS-3
DefaultGroupName=VisuALS
UsePreviousAppDir=No

[Files]
Source: "..\VisuALS_Windows\bin\x86\Release\*"; DestDir: "{app}"; Flags: recursesubdirs
Source: "..\VisuALS_Windows\bin\x86\Release\Resources\*"; DestDir: "{app}\Resources"; Permissions: everyone-full; Flags: recursesubdirs
Source: "..\VisuALS_Windows\bin\x86\Release\Update Management\*"; DestDir: "{app}\Update Management"; Permissions: everyone-full; Flags: recursesubdirs

[Icons]
Name: "{group}\VisuALS"; Filename: "{app}\VisuALS.exe"; WorkingDir: "{app}"; IconFilename: "{app}\Resources\Images\visuals-icon.ico"
Name: "{commondesktop}\VisuALS"; Filename: "{app}\VisuALS.exe"; WorkingDir: "{app}"; IconFilename: "{app}\Resources\Images\visuals-icon.ico"
Name: "{commonprograms}\VisuALS"; Filename: "{app}\VisuALS.exe"; WorkingDir: "{app}"; IconFilename: "{app}\Resources\Images\visuals-icon.ico"

[UninstallRun]
RunOnceId: "SettingsOptions"; Filename: "{app}\VisuALS.exe"; Parameters: "uninstall_options"