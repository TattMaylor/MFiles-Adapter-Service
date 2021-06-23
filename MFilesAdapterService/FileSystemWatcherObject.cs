namespace MFilesAdapterService
{
    using System.IO;

    /// <summary>
    /// Created using data from an XML configuration file
    /// </summary>
    public class FileSystemWatcherObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemWatcherObject" /> class.
        /// </summary>
        public FileSystemWatcherObject() { }

        /// <summary>
        /// Gets or sets the FileSystemWatcher
        /// </summary>
        public FileSystemWatcher Watcher { get; set; }

        /// <summary>
        /// Gets or sets the file name of the FileSystemWatcher's XML file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the project associated with this FileSystemWatcher
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the file path of the directory to be watched
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the type of file to be watched (ex. PDF)
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the FileSystemWatcher should watch subdirectories in the FilePath
        /// </summary>
        public bool IncludeSubdirectories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the FileSystemWatcher should raise events
        /// </summary>
        public bool EnableRaisingEvents { get; set; }

        /// <summary>
        /// Gets or sets the M-Files class associated with the type of files being watched
        /// </summary>
        public int FileClass { get; set; }
    }
}
