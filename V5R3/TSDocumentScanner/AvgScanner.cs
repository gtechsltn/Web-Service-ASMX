using System.Configuration;
using System.IO;

namespace TSDocumentScanner
{
    public class AvgScanner : IScanViruses
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool IsVirusScanEnable
        {
            get
            {
                var enableScanVirus = ConfigurationManager.AppSettings["VirusScanEnable"];
                if (enableScanVirus != null)
                {
                    return enableScanVirus.Equals("1", System.StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    log.Error("Cannot get the AppSettings > VirusScanEnable from App.config/Web.config");
                    return false;
                }
            }
        }

        public ScanResult ScanBytes(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }

        public ScanResult ScanFile(string fullPath)
        {
            throw new System.NotImplementedException();
        }

        public ScanResult ScanStream(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}