using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TSDocumentUtil;

namespace TSDocumentScanner
{
    /// <summary>
    /// Easily fit Antivirus for Amazon S3 into any workflow with 4 flexible scanning models. Plus, findings are published to AWS Security Hub.
    /// Scan a wide variety of file types up to 5 TB in size with industry-leading virus detection engines Sophos and ClamAV®.
    /// Have confidence that your data is secure - Antivirus for Amazon S3 installs within your AWS account, so data never leaves your environment or region.
    /// Scan objects outside of AWS in real time via a REST-based API
    /// </summary>
    /// <seealso cref="TSDocumentScanner.IScanViruses" />
    public class ClamAvScanner : IScanViruses
    {
        private static string _fileName;
        private static HttpClient client = new HttpClient();
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ClamAvScanner()
        {
        }

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
            try
            {
                log.Info("ScanBytes started.");
                if (!IsVirusScanEnable)
                {
                    return ScanResult.Ignore;
                }

                return ScanVirusAsync(bytes).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ScanResult.Error;
            }
        }

        public ScanResult ScanFile(string fullPath)
        {
            try
            {
                log.Info("ScanFile started.");
                if (!IsVirusScanEnable)
                {
                    return ScanResult.Ignore;
                }

                var bytes = File.ReadAllBytes(fullPath);
                return ScanVirusAsync(bytes).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ScanResult.Error;
            }
        }

        public ScanResult ScanStream(Stream stream)
        {
            try
            {
                log.Info("ScanStream started.");
                if (!IsVirusScanEnable)
                {
                    return ScanResult.Ignore;
                }

                var bytes = StreamExtensions.ReadToEnd(stream);
                return ScanVirusAsync(bytes).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ScanResult.Error;
            }
        }

        public void SetFileName(string fileName)
        {
            _fileName = fileName;
        }

        private static (string accessToken, string refreshToken) GetAccessToken(
            string baseUri,
            string loginEndpoint,
            string accessKey,
            string secretKey)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                Console.WriteLine($"BaseUri: {baseUri}");
                Console.WriteLine($"LoginEndpoint: {loginEndpoint}");
                var tokensClient = new TokensClient(client);
                var tokenRequest = new TokenRequest();
                tokenRequest.Email = accessKey;
                tokenRequest.Password = secretKey;
                var tenant = "root";
                var tokenResponse = tokensClient.GetTokenAsync(tenant, tokenRequest)
                                                .GetAwaiter()
                                                .GetResult();
                var accessToken = tokenResponse.Token;
                var refreshToken = tokenResponse.RefreshToken;
                Console.WriteLine($"AccessToken: {accessToken}");
                Console.WriteLine($"RefreshToken: {refreshToken}");
                return (accessToken, refreshToken);
            }
        }

        private static ScanResult ObjectScanning(
            string baseUri,
            string objectScanningEndpoint,
            string accessToken,
            string refreshToken,
            byte[] bytes)
        {
            try
            {
                string uploadFilePath = $@"{Environment.CurrentDirectory}\TestFiles\Sample.pdf";
                string fileContentType = "application/pdf";
                //MIME types ~ (Multipurpose Internet Mail Extensions) types
                //https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types
                //https://stackoverflow.com/questions/4212861/what-is-a-correct-mime-type-for-docx-pptx-etc
                //-----------------------------------------------------------
                //PDF           : "application/pdf"
                //Zip           : "application/zip"
                //Doc           : "application/msword"
                //Docx          : "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                //Xls           : "application/vnd.ms-excel"
                //Xlsx          : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                //Ppt           : "application/vnd.ms-powerpoint"
                //Pptx          : "application/vnd.openxmlformats-officedocument.presentationml.presentation"
                //
                //Application
                //----------------------------------------------
                //File          : "application/octet-stream"
                //
                //For example:
                //  headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //  headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                //  headers.ContentDisposition.FileName = "ebook.mobi";
                Console.WriteLine($"BaseUri : {baseUri}");
                Console.WriteLine($"ObjectScanningEndpoint : {objectScanningEndpoint}");
                string fileName = Path.GetFileName(uploadFilePath);

                using (var client = new HttpClient())
                {
                    var values = new[]
                    {
                        new KeyValuePair<string, string>("CustomerId", "123"),
                        new KeyValuePair<string, string>("CustomerName", "Third Sight Ltd."),
                    };
                    using (var content = new MultipartFormDataContent())
                    {
                        foreach (var val in values)
                        {
                            content.Add(new StringContent(val.Value), val.Key);
                        }
                        var fileContent = new StreamContent(new FileStream(uploadFilePath, FileMode.Open));
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(fileContentType);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "File",
                            FileName = fileName,
                        };
                        content.Add(fileContent);
                        client.BaseAddress = new Uri(baseUri);
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                        var result = client.PostAsync(objectScanningEndpoint, content).Result;
                        Console.WriteLine($"Response : {result.StatusCode}");
                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            var jsonContent = result.Content.ReadAsStringAsync().Result;
                            var objectScanningResult = JsonConvert.DeserializeObject<ObjectScanningResult>(jsonContent);
                            if (objectScanningResult.Result == nameof(ScanResult.Clean))
                            {
                                return ScanResult.Clean;
                            }
                            if (objectScanningResult.Result == nameof(ScanResult.VirusDetected))
                            {
                                return ScanResult.VirusDetected;
                            }
                            else
                            {
                                return ScanResult.Unknown;
                            }
                        }
                        else if (result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            return ScanResult.Error;
                        }
                        else
                        {
                            return ScanResult.Unknown;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Debug.WriteLine(ex);
                return ScanResult.Error;
            }
        }

        private static async Task<ScanResult> ScanVirusAsync(byte[] bytes)
        {
            var config = new AmazonS3Config
            {
                AuthenticationRegion = "Region",
                ServiceURL = "http://localhost:9000",
                ForcePathStyle = true,
            };

            var baseUri = ConfigurationManager.AppSettings["AwsBaseUri"];
            var loginEndpoint = ConfigurationManager.AppSettings["AwsLoginEndpoint"];
            var objectScanningEndpoint = ConfigurationManager.AppSettings["AwsVirusScanEndpoint"];

            //var accessKey = Credentials.AwsAccessKeyId;
            //var secretKey = Credentials.AwsSecretAccessKey;

            var accessKey = "admin@root.com";
            var secretKey = "123Pa$$word!";

            var (accessToken, refreshToken) = GetAccessToken(baseUri, loginEndpoint, accessKey, secretKey);

            return ObjectScanning(baseUri, objectScanningEndpoint, accessToken, refreshToken, bytes);

            //var amazonS3Client = new AmazonS3Client(accessKey, secretKey, config);

            //amazonS3Client.ExceptionEvent += OnAmazonS3Exception;

            //var response = await amazonS3Client.ScanAsync(stream);
        }

        //private void OnAmazonS3Exception(object sender, ExceptionEventArgs e)
        //{
        //}

        private class AmazonS3Client
        {
            private string accessKey;
            private AmazonS3Config config;
            private string secretKey;

            public AmazonS3Client(string accessKey, string secretKey, AmazonS3Config config)
            {
                this.accessKey = accessKey;
                this.secretKey = secretKey;
                this.config = config;
            }
        }

        private class AmazonS3Config
        {
            public string AuthenticationRegion { get; set; }
            public bool ForcePathStyle { get; set; }
            public string ServiceURL { get; set; }
        }

        private class Detectedinfection
        {
            public string File { get; set; }
            public string Infection { get; set; }
        }

        private class ObjectScanningResult
        {
            public DateTime DateScanned { get; set; }
            public Detectedinfection[] DetectedInfections { get; set; }
            public object ErrorMessage { get; set; }
            public string Result { get; set; }
        }
    }
}