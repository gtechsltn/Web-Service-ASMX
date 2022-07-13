using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using TSDocumentUtil;

namespace TSDocumentScanner.ConsoleApp
{
    internal static class Program
    {
        private static string _accessKey;
        private static string _accessToken;
        private static string _baseUri;
        private static string _loginEndpoint;
        private static string _refreshToken;
        private static string _secretKey;
        private static string _tenant;
        private static string _virusScanEndpoint;
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                var tokensClient = new TokensClient(client);
                var tokenRequest = new TokenRequest();
                tokenRequest.Email = _accessKey;
                tokenRequest.Password = _secretKey;

                var tokenResponse = tokensClient.GetTokenAsync(_tenant, tokenRequest)
                                                .GetAwaiter()
                                                .GetResult();
                _accessToken = tokenResponse.Token;
                _refreshToken = tokenResponse.RefreshToken;
                Console.WriteLine($"Access_Token: {_accessToken}");
                Console.WriteLine($"Refresh_Token: {_refreshToken}");
            }
        }

        private static void GetVariables()
        {
            _baseUri = ConfigurationManager.AppSettings["AwsBaseUri"];
            _loginEndpoint = ConfigurationManager.AppSettings["AwsLoginEndpoint"];
            _tenant = "root";
            _virusScanEndpoint = ConfigurationManager.AppSettings["AwsVirusScanEndpoint"];

            _accessKey = Credentials.AwsAccessKeyId;
            _secretKey = Credentials.AwsSecretAccessKey;

            Console.WriteLine($"Base_Uri: {_baseUri}");
            Console.WriteLine($"AWS_Access_Key_Id: {_accessKey}");
            Console.WriteLine($"AWS_Secret_Access_Key: {_secretKey}");
        }

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            log.Info($"{nameof(Main)} started.");

            GetVariables();

            GetAccessToken();

            ScanVirus();

            log.Info($"{nameof(Main)} finished.");

            Console.Write("Press any key to exit");
            Console.ReadKey();
        }

        private static void ScanVirus()
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
                string apiBaseAddress = _baseUri;

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
                        client.BaseAddress = new Uri(apiBaseAddress);
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
                        var result = client.PostAsync(_virusScanEndpoint, content).Result;
                        Console.WriteLine($"Response : {result.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Debug.WriteLine(ex);
            }
        }
    }
}