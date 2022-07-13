using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.IO;
using TSDocumentScanner;

namespace TSDocumentScannerTests
{
    [TestClass]
    public class ClamAvScannerTests : ScanVirusTestBase
    {
        [TestMethod]
        public void ScanBytes_WithNoInput_Return_NoVirusFound()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "TestFiles", "Sample.txt");
            var fileContent = File.ReadAllBytes(fileName);

            var result = Scanner.ScanBytes(fileContent);

            Assert.IsTrue(IsVirusScanEnable && result.Equals(ScanResult.Clean)
                || !IsVirusScanEnable && result.Equals(ScanResult.Ignore));
        }

        [TestMethod]
        public void ScanStream_WithNoInput_Return_NoVirusFound()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "TestFiles", "Sample.txt");

            using (FileStream fs = File.OpenRead(fileName))
            {
                var result = Scanner.ScanStream(fs);

                Assert.IsTrue(IsVirusScanEnable && result.Equals(ScanResult.Clean)
                || !IsVirusScanEnable && result.Equals(ScanResult.Ignore));
            }
        }

        [TestMethod]
        public void ScanFile_WithNoInput_Return_NoVirusFound()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "TestFiles", "Sample.txt");

            var result = Scanner.ScanFile(fileName);

            Assert.IsTrue(IsVirusScanEnable && result.Equals(ScanResult.Clean)
                || !IsVirusScanEnable && result.Equals(ScanResult.Ignore));
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