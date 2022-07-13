using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace TSDocumentScanner
{
    public class Credentials
    {
        private static readonly string EncryptionKey = "ThirdSight@Secret";

        public static string AwsAccessKeyId
        {
            get
            {
                return DecryptAppSetting(nameof(AwsAccessKeyId));
            }
        }

        public static string AwsSecretAccessKey
        {
            get
            {
                return DecryptAppSetting(nameof(AwsSecretAccessKey));
            }
        }

        private static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        private static string DecryptAppSetting(string key)
        {
            string value = string.Empty;
            string path = Path.Combine(Environment.CurrentDirectory, "web.config");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList list = doc.DocumentElement.SelectNodes(string.Format("appSettings/add[@key='{0}']", key));

            if (list.Count == 1)
            {
                XmlNode node = list[0];
                value = node.Attributes["value"].Value;
            }
            return Decrypt(value);
        }
    }
}