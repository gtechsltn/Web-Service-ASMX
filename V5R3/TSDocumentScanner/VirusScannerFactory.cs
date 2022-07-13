namespace TSDocumentScanner
{
    /// <summary>
    /// Virus scanner factory, should be extended to support other Anti virus applications
    /// </summary>
    public class VirusScannerFactory
    {
        public static IScanViruses GetVirusScanner()
        {
            // Currently we only have one Anti virus implementation,
            // but later we want to include other such as AVG, McAfee
            return new ClamAvScanner();
            // --------------------------------------------------
            // Extend this using AvgScanner for example
            //return new AvgScanner();
        }
    }
}