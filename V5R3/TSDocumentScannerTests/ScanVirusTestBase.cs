using Microsoft.VisualStudio.TestTools.UnitTesting;
using TSDocumentScanner;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace TSDocumentScannerTests
{
    public abstract class ScanVirusTestBase
    {
        private IScanViruses scanner;

        public IScanViruses Scanner
        {
            get
            {
                return scanner;
            }
        }

        [AssemblyInitialize]
        public void Configure()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        [TestInitialize()]
        public void Startup()
        {
            scanner = VirusScannerFactory.GetVirusScanner();
        }
    }
}