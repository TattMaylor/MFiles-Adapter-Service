namespace MFilesAdapterService
{
    using System;
    using System.Configuration;
    using System.IO;
    using MFilesAPI;

    /// <summary>
    /// Handles communication with the MFiles API
    /// </summary>
    public class MFilesWrapper
    {
        private MFilesServerApplication serverApplication = new MFilesServerApplication();
        private Vault vault = new Vault();

        /// <summary>
        /// Add a document to a vault on M-Files
        /// </summary>
        /// <param name="mfilesConfig">Configuration data needed to create a document in M-Files</param>
        public void AddDocument(MFilesObject mfilesConfig)
        {
            if (this.vault == null)
            {
                Log($"Vault connection failed");
                throw new NotImplementedException("Vault connection failed");
            }

            var propertyValues = new PropertyValues();

            // M-Files Class (What project / department a file is associated with)
            var classPropertyValue = new PropertyValue()
            {
                PropertyDef = (int)MFBuiltInPropertyDef.MFBuiltInPropertyDefClass
            };
            classPropertyValue.Value.SetValue(
                MFDataType.MFDatatypeLookup,
                mfilesConfig.FileClass);
            propertyValues.Add(-1, classPropertyValue);

            // M-Files Filename
            var nameOrTitlePropertyValue = new PropertyValue()
            {
                PropertyDef = (int)MFBuiltInPropertyDef.MFBuiltInPropertyDefNameOrTitle
            };
            nameOrTitlePropertyValue.Value.SetValue(
                MFDataType.MFDatatypeText,
                mfilesConfig.FileName);
            propertyValues.Add(-1, nameOrTitlePropertyValue);

            var sourceFiles = new SourceObjectFiles();
            var myFile = new SourceObjectFile
            {
                SourceFilePath = mfilesConfig.FilePath,
                Title = mfilesConfig.FileName, // Ignored for single file documents
                Extension = "pdf"
            };
            sourceFiles.Add(-1, myFile);

            var objectTypeID = (int)MFBuiltInObjectType.MFBuiltInObjectTypeDocument;

            var isSingleFileDocument =
                    objectTypeID == (int)MFBuiltInObjectType.MFBuiltInObjectTypeDocument && sourceFiles.Count == 1;

            var objectVersion = this.vault.ObjectOperations.CreateNewObjectEx(
                    objectTypeID,
                    propertyValues,
                    sourceFiles,
                    SFD: isSingleFileDocument,
                    CheckIn: true);

            Log($"Added document {mfilesConfig.FilePath}");
        }

        /// <summary>
        /// Attempt to connect to the M-Files server
        /// </summary>
        public void Connect()
        {
            this.serverApplication = new MFilesServerApplication();
            try
            {
                this.serverApplication.Connect(
                AuthType: MFAuthType.MFAuthTypeSpecificWindowsUser,
                UserName: ConfigurationManager.AppSettings["ServiceUsername"],
                Password: ConfigurationManager.AppSettings["ServicePassword"],
                Domain: ConfigurationManager.AppSettings["UserDomain"],
                NetworkAddress: ConfigurationManager.AppSettings["MFNetworkAddress"]);
                Log($"Connected to M-Files server");

                var vaultGuid = ConfigurationManager.AppSettings["MFVaultGuid"];
                this.vault = serverApplication.LogInToVault(vaultGuid);
                Log($"Connected to M-Files vault");
            }
            catch (Exception ex)
            {
                Log($"Connection Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Disconnect from the M-Files server
        /// </summary>
        public void Disconnect()
        {
            this.serverApplication.Disconnect();
            Log($"Disconnected from M-Files server");
        }

        /// <summary>
        /// Logs service actions to a file
        /// </summary>
        /// <param name="message">The message to be logged</param>
        private static void Log(string message) =>
            File.AppendAllText(@ConfigurationManager.AppSettings["LogFilePath"], $"[{DateTime.Now}][MFILES] {message}\n");
    }
}