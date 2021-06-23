namespace MFilesAdapterService
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.ServiceProcess;

    public partial class AdapterService : ServiceBase
    {
        private readonly MFilesWrapper mfiles;
        private FileSystemWatcherFactory factory; 

        public AdapterService()
        {
            InitializeComponent();
            this.mfiles = new MFilesWrapper();
            this.factory = new FileSystemWatcherFactory(this.mfiles);
        }

        protected override void OnStart(string[] args)
        {
            Log("Service has started");
            this.mfiles.Connect();
            this.factory.CreateConfigWatcher();
            this.factory.InitializeWatchers(
                Directory.GetFiles(
                    @ConfigurationManager.AppSettings["ConfigFilePath"]));
        }

        protected override void OnStop()
        {
            this.mfiles.Disconnect();
            this.factory.DisposeWatchers();
            Log("Service has stopped");
        }

        private static void Log(string message) =>
            File.AppendAllText(@ConfigurationManager.AppSettings["LogFilePath"], $"[{DateTime.Now}][SERVICE] {message}\n");
    }
}
