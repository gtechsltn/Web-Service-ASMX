namespace TSDocumentScanner
{
    /// <summary>
    /// public enum ClamScanResults
    /// https://github.com/tekmaven/nClam/blob/master/nClam/ClamScanResults.cs
    /// public class ClamScanResults
    /// https://github.com/tekmaven/nClam/blob/master/nClam/ClamScanResult.cs
    /// </summary>
    public enum ScanResult
    {
        Unknown,            // => Message: "Could not scan file"
        Clean,              // => Message: "No Virus found"
        Error,              // => Message: string.Format("VIRUS SCAN ERROR! {0}", ex.ToString())
        Ignore,             // => Message: "Disable virus scan"
        VirusDetected,      // => Message: "Virus found: " + scanResult.InfectedFiles.First().VirusName
    }
}