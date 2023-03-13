# About
The M-Files Adapter Service (MFAS) was designed as a lightweight solution to effortlessly transfer documents into M-Files via M-Files' COM API.

The service is capable of watching multiple directories simultaneously through the use of FileSystemWatchers.

When a document that meets a specified criteria enters a watched folder, MFAS will automatically transfer it into M-Files.
<br>

# How To Use

### Watching a Directory
MFAS utilizes a specified configuration directory where JSON files are loaded to create FileSystemWatchers. The configuration directory is set in the App.config file. JSON templates can be added and removed while MFAS is running, the service will automatically dispose of any removed watcher data or add new watcher data when a template leaves or enters the folder.

### Watcher Template (JSON)
    {
		"projectname": "Test Project",
		"filepath": "C:\\the\\directory\\to\\watch",
		"filter": "*.pdf",
		"includesubdirectories": "true",
		"enableraisingevents": "true",
		"fileclass": 0
    }

- `projectname`: The name of the project or system associated with the files
- `filepath`: The directory to be monitored for changes
- `filter`: The file extensions to monitor
- `includesubdirectories`: Whether or not to check directories within the filepath
- `enableraisingevents`: Whether or not to raise FileSystemWatcher events
- `fileclass`: The type of document being uploaded (check M-Files for this integer)

<br>

# Testing and Development

### Setting Up Local Environment
> Note: The created files are ignored by source control to prevent users from accidentally pushing their test configurations

1. Clone the repo 
2. Create an `App.config` file in `mfiles-adapter-service/MFilesAdapterService`
3. Create a `config` folder in `mfiles-adapter-service/MFilesAdapterService` (optional location)
4. Create a `debug` folder in `mfiles-adapter-service/MFilesAdapterService` (optional location)
5. Populate the key-value pairs in `App.config`
```
[App.config]

	<add key="ConfigFilePath" value="" />
	<add key="LogFilePath" value="" />
	<add key="MFNetworkAddress" value="" />
	<add key="MFVaultGuid" value="" />
	<add key="MFVaultGuidDebug" value="" />
	<add key="ServiceUsername" value="" />
	<add key="ServicePassword" value="" />
	<add key="UserDomain" value="" />
```
6. Build the project

### Installing the Service
> Note: Click the footnotes to view example commands

You can install the service by calling `installutil.exe` on `MFilesAdapterService.exe`. [^1]

You can uninstall the service by including `/u` before the executible file. [^2]

For quick reinstallations during debugging, I recommend creating a batch script to run the uninstall and install commands back to back.

You can verify that the service has been installed / uninstalled by opening the <a href="https://www.thewindowsclub.com/open-windows-services" title="Hobbit lifestyles">Services Manager</a>.

### Debugging the Service
> Note: Click the footnotes to view specific instructions

1. Build the project using Debug configuration
2. Reinstall the service onto your machine
3. The process should automatically start [^3]
4. Attach to the process [^4]

Attaching to the process will allow you to use breakpoints in Visual Studio while the service runs.

<br>

# Footnotes
[^1]: `C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe C:\Path\To\Project\bin\Debug\MFilesAdapterService.exe`
[^2]: `C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe /u C:\Path\To\Project\bin\Debug\MFilesAdapterService.exe`
[^3]: If the process doesn't start automatically, you can manually start, stop, and restart it in the Service Manager
[^4]: In Visual Studio, go to `Debug` > `Attach to Process` > Select the `MFilesAdapterService` process
