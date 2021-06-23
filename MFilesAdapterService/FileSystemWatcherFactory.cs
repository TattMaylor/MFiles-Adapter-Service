namespace MFilesAdapterService
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Newtonsoft.Json;

    /// <summary>
    /// Responsible for creating and managing FileSystemWatchers
    /// </summary>
    public class FileSystemWatcherFactory
    {
        /// <summary>
        /// Tracks all active FileSystemWatchers
        /// </summary>
        private List<FileSystemWatcherObject> watcherList = new List<FileSystemWatcherObject>();

        /// <summary>
        /// The connection to an M-Files Vault
        /// </summary>
        private MFilesWrapper mfiles = new MFilesWrapper();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemWatcherFactory" /> class.
        /// </summary>
        /// <param name="mfiles">Handles the connection to a certain Vault within M-Files</param>
        public FileSystemWatcherFactory(MFilesWrapper mfiles)
        {
            this.mfiles = mfiles;
        }

        /// <summary>
        /// Read all JSON configurations and attempt to create a FileSystemWatcher for each
        /// </summary>
        /// <param name="configs">The configuration data for a FileSystemWatcher</param>
        public void InitializeWatchers(string[] configs)
        {
            Log($"Loaded {configs.Length} custom watcher configurations");

            foreach (var config in configs)
            {
                FileSystemWatcherObject watcherConfig = JsonConvert.DeserializeObject<FileSystemWatcherObject>(@File.ReadAllText(@config));

                if (IsCorrectFormat(watcherConfig))
                {
                    watcherConfig.FileName = Path.GetFileName(@config);
                    CreateWatcher(watcherConfig);
                }
                else
                {
                    Log($"Failed to create watcher for {watcherConfig.ProjectName} - review JSON file");
                }
            }
        }

        /// <summary>
        /// Create a new FileSystemWatcher
        /// </summary>
        /// <param name="watcherConfig">An object containing information from an XML configuration file</param>
        public void CreateWatcher(FileSystemWatcherObject watcherConfig)
        {
            watcherConfig.Watcher = new FileSystemWatcher(watcherConfig.FilePath)
            {
                NotifyFilter = NotifyFilters.FileName,
                Filter = watcherConfig.Filter,
                IncludeSubdirectories = watcherConfig.IncludeSubdirectories,
                EnableRaisingEvents = watcherConfig.EnableRaisingEvents
            };

            watcherConfig.Watcher.Created += OnCreated;
            watcherConfig.Watcher.Deleted += OnDeleted;
            watcherConfig.Watcher.Error   += OnError;

            watcherList.Add(watcherConfig);

            Log($"Created watcher for {watcherConfig.FileName}");
        }

        /// <summary>
        /// Creates a FileSystemWatcher to monitor the configuration folder
        /// </summary>
        public void CreateConfigWatcher()
        {
            var watcher = new FileSystemWatcher(@ConfigurationManager.AppSettings["ConfigFilePath"])
            {
                NotifyFilter = NotifyFilters.FileName,
                Filter = "*.json",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            watcher.Created += OnCreatedConfig;
            watcher.Deleted += OnDeletedConfig;
            watcher.Error   += OnErrorConfig;

            Log($"Created watcher for JSON configurations");
        }

        /// <summary>
        /// Dispose of all active FileSystemWatchers
        /// </summary>
        public void DisposeWatchers()
        {
            try
            {
                foreach (FileSystemWatcherObject watcher in watcherList.ToList())
                {
                    Log($"Disposed watcher for {watcher.FileName}");
                    watcher.Watcher.Dispose();
                    watcherList.Remove(watcher);
                }
            }
            catch (Exception ex)
            {
                Log($"EXCEPTION: {ex.Message}");
            }
        }

        /// <summary>
        /// When a document is added to a FileSystemWatcher's directory, system will communicate with the MFiles API to upload the document
        /// </summary>
        /// <param name="sender">The FileSystemWatcher object</param>
        /// <param name="e">The FileSystemWatcher's event arguments</param>
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Log($"Created: {e.FullPath}");

            try
            {
                mfiles.AddDocument(
                new MFilesObject
                {
                    FilePath = e.FullPath,
                    FileName = RemoveFileExtension(e.Name),
                    FileClass = GetMFilesClass((FileSystemWatcher)sender)
                });
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Triggered when a document is removed from a FileSystemWatcher's directory
        /// </summary>
        /// <param name="sender">The FileSystemWatcher object</param>
        /// <param name="e">The FileSystemWatcher's event arguments</param>
        private void OnDeleted(object sender, FileSystemEventArgs e) =>
            Log($"Deleted: {e.FullPath}");

        /// <summary>
        /// Triggered when an error involving a FileSystemWatcher occurs
        /// </summary>
        /// <param name="sender">The FileSystemWatcher object</param>
        /// <param name="e">The FileSystemWatcher's event arguments</param>
        private void OnError(object sender, ErrorEventArgs e) =>
            Log($"Error: {e.ToString()}");

        /// <summary>
        ///  When a configuration JSON is added to the configuration directory, system will attempt to create a new FileSystemWatcher
        /// </summary>
        /// <param name="sender">The FileSystemWatcher object</param>
        /// <param name="e">The FileSystemWatcher's event arguments</param>
        private void OnCreatedConfig(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000); // Allow the File class time to open and close the current file
            FileSystemWatcherObject watcherConfig = JsonConvert.DeserializeObject<FileSystemWatcherObject>(@File.ReadAllText(@e.FullPath));

            if (IsCorrectFormat(watcherConfig))
            {
                watcherConfig.FileName = Path.GetFileName(@e.FullPath);
                CreateWatcher(watcherConfig);
            }
            else
            {
                Log($"Failed to create watcher for {watcherConfig.ProjectName}");
            }
        }

        /// <summary>
        /// When a configuration JSON is removed from the configuration directory, system will dispose of the associated FileSystemWatcher
        /// </summary>
        /// <param name="sender">The FileSystemWatcher object</param>
        /// <param name="e">The FileSystemWatcher's event arguments</param>
        private void OnDeletedConfig(object sender, FileSystemEventArgs e)
        {
            foreach (FileSystemWatcherObject watcher in watcherList)
            {
                if (watcher.FileName.Equals(e.Name))
                {
                    Log($"Disposed watcher for {e.Name}");
                    watcher.Watcher.Dispose();
                    watcherList.Remove(watcher);
                    break;
                }
            }
        }

        /// <summary>
        /// Triggered when an error involving the configuration FileSystemWatcher occurs
        /// </summary>
        /// <param name="sender">The FileSystemWatcher object</param>
        /// <param name="e">The FileSystemWatcher's event arguments</param>
        private void OnErrorConfig(object sender, ErrorEventArgs e) =>
            Log($"Error: {e.ToString()}");

        /// <summary>
        /// Removes the file extension from a filename string
        /// </summary>
        /// <param name="filename">The filename including extension</param>
        /// <returns>A filename string without the file extension</returns>
        private string RemoveFileExtension(string filename) =>
            filename.Substring(0, filename.LastIndexOf('.'));

        /// <summary>
        /// Checks to see if the configuration JSON contains the correct data
        /// Data that is 
        /// </summary>
        /// <param name="watcherConfig">An object containing information from an XML configuration file</param>
        /// <returns>True if correct format, else false</returns>
        private bool IsCorrectFormat(FileSystemWatcherObject watcherConfig)
        {            
            foreach (PropertyInfo property in watcherConfig.GetType().GetProperties().Skip(2))
            {
                if (property.GetValue(watcherConfig) == null)
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Retrieves the M-Files class type that a document will be stored under
        /// </summary>
        /// <param name="sender">The FileSystemWatcher object</param>
        /// <returns>The integer value of an M-Files document class, else 0 (0 means a generic "Document" class in M-Files)</returns>
        private int GetMFilesClass(FileSystemWatcher sender)
        {
            foreach (FileSystemWatcherObject watcher in watcherList)
            {
                if (watcher.FilePath.Equals(sender.Path))
                {
                    return watcher.FileClass;
                }
            }

            return 0;
        }

        /// <summary>
        /// Logs service actions to a file
        /// </summary>
        /// <param name="message">The message to be logged</param>
        private void Log(string message) =>
            File.AppendAllText(@ConfigurationManager.AppSettings["LogFilePath"], $"[{DateTime.Now}][FACTORY] {message}\n");
    }
}
