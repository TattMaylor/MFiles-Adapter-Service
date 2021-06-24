# Setting Up Local Environment
> Note: Use the installation on Portal as a reference for things like `App.config` contents

1. Clone the repo 
2. Create an `App.config` file in `mfiles-adapter-service/MFilesAdapterService`
3. Create a `config` folder in `mfiles-adapter-service/MFilesAdapterService` (optional location)
4. Create a `debug` folder in `mfiles-adapter-service/MFilesAdapterService` (optional location)
5. Populate the key-value pairs in `App.config`

# Installing the Service
> Note: Click the footnotes to view example commands

You can install the service by calling `installutil.exe` on `MFilesAdapterService.exe`. [^1]

You can uninstall the service by including `/u` before the executible file. [^2]

For quick reinstallations during debugging, I recommend creating a batch script to run the uninstall and install commands back to back.

You can verify that the service has been installed / uninstalled by opening the <ins>[Services Manager]("https://www.thewindowsclub.com/open-windows-services")</ins>.

# Debugging the Service
> Note: Click the footnotes to view specific instructions

1. Build the project using Debug configuration
2. Reinstall the service onto your machine
3. The process should automatically start [^3]
4. Attach to the process [^4]

Attaching to the process will allow you to use breakpoints in Visual Studio while the service runs.

<br>

# Footnotes
[^1]: `C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe C:\Users\mttaylor\Documents\Projects\MFilesAdapterService\MFilesAdapterService\bin\Debug\MFilesAdapterService.exe`
[^2]: `C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe /u C:\Users\mttaylor\Documents\Projects\MFilesAdapterService\MFilesAdapterService\bin\Debug\MFilesAdapterService.exe`
[^3]: If the process doesn't start automatically, you can manually start, stop, and restart it in the Service Manager
[^4]: In Visual Studio, go to `Debug` > `Attach to Process` > Select the `MFilesAdapterService` process
