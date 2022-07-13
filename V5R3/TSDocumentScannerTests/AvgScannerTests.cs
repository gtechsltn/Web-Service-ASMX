using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using TSDocumentScanner;

namespace TSDocumentScannerTests
{
    [TestClass]
    public class AvgScannerTests : ScanVirusTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException), nameof(NotImplementedException))]
        public void ScanBytes_WithNoInput_Throw_NotImplementedException()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "TestFiles", "Sample.txt");
            var fileContent = File.ReadAllBytes(fileName);

            var result = Scanner.ScanBytes(fileContent);

            Debug.WriteLine(result);

            Assert.AreEqual(result, ScanResult.Unknown);
        }

        public bool IsVirusScanEnable
        {
            get
            {
                return ConfigurationManager.AppSettings["VirusScanEnable"].Equals("1", System.StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}