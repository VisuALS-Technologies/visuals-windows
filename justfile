set shell := ["cmd.exe", "/c"]

build:
	msbuild .\src\VisuALS_Windows\VisuALS_Windows.csproj -property:Configuration=Release

run: build
	.\src\VisuALS_Windows\bin\x86\Release\VisuALS.exe

build-installer: build clean-installer
	iscc .\src\VisuALS_Installer\VisuALS_WPF_APP.iss

clean-build:
	rmdir .\src\VisuALS_Windows\bin /s

clean-installer:
	rmdir .\src\VisuALS_Installer\Output /s

install: build-installer
	dir *Installer.exe /s/b | findstr .exe | cmd

setup-dev-env: install-vs add-vs-components add-msbuild-to-path install-innosetup

install-vs:
	winget install --id  Microsoft.VisualStudio.2022.Community

add-vs-components:
	"C:\Program Files (x86)\Microsoft Visual Studio\Installer\setup.exe" modify --installPath "C:\Program Files\Microsoft Visual Studio\2022\Community" --config ".\.vsconfig"

install-innosetup:
	winget install --id JRSoftware.InnoSetup -e -s winget

add-msbuild-to-path:
	setx /M PATH "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin"
