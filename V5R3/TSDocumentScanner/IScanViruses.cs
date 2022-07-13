using System.IO;

namespace TSDocumentScanner
{
    /// <summary>
    /// Interface to implement virus scanning
    /// </summary>
    public interface IScanViruses
    {
        /// <summary>
        /// Scans a file for a virus
        /// </summary>
        /// <param name="fullPath">The full path to the file</param>
        ScanResult ScanFile(string fullPath);

        /// <summary>
        /// Scans some bytes for a virus
        /// </summary>
        /// <param name="bytes">The bytes to scan</param>
        ScanResult ScanBytes(byte[] bytes);

        /// <summary>
        /// Scans a stream for a virus
        /// </summary>
        /// <param name="stream">The stream to scan</param>
        ScanResult ScanStream(Stream stream);

        /// <summary>
        /// Gets a value from appSettings:VirusScanEnable (1/0).
        /// </summary>
        /// <value>
        ///   <c>true</c> if appSettings:VirusScanEnable = 1 is enable; otherwise, <c>false</c>.
        /// </value>
        bool IsVirusScanEnable { get; }
    }
}