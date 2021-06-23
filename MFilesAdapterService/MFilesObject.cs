namespace MFilesAdapterService
{
    /// <summary>
    /// Holds the necessary information to create a file in M-Files
    /// </summary>
    public class MFilesObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MFilesObject" /> class.
        /// </summary>
        public MFilesObject() { }

        /// <summary>
        /// Gets or sets the file path of the file to be uploaded to M-Files
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the name of the file to be uploaded to M-Files
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the M-Files class associated with the type of files being watched
        /// </summary>
        public int FileClass { get; set; }
    }
}
