namespace MFilesAdapterService
{
    using System.ServiceProcess;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[]
            {
                new AdapterService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}