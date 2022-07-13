using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace TSCrypto
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
            set
            {
                EncryptAppSetting(nameof(AwsAccessKeyId));
            }
        }

        public static string AwsSecretAccessKey
        {
            get
            {
                return DecryptAppSetting(nameof(AwsSecretAccessKey));
            }
            set
            {
                EncryptAppSetting(nameof(AwsSecretAccessKey));
            }
        }

        public static string GetValueWebConfig(string key)
        {
            try
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "web.config");

                if (File.Exists(filePath))
                {
                    ExeConfigurationFileMap customWebConfig = new ExeConfigurationFileMap();

                    customWebConfig.ExeConfigFilename = Path.GetFileName(filePath);

                    Configuration customConfig = ConfigurationManager.OpenMappedExeConfiguration(customWebConfig, ConfigurationUserLevel.None);

                    AppSettingsSection appSettings = (customConfig.GetSection("appSettings") as AppSettingsSection);

                    return appSettings.Settings[key].Value;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return string.Empty;
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

        /// <summary>
        /// Usage:
        ///     string password = DecryptAppSetting("Password");
        ///     Debug.Print(password);
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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
            string password = Decrypt(value);
            return password;
        }

        private static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        /// <summary>
        /// Usage:
        ///     EncryptAppSetting("Password");
        /// </summary>
        /// <param name="key"></param>
        private static void EncryptAppSetting(string key)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "web.config");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList list = doc.DocumentElement.SelectNodes(string.Format("appSettings/add[@key='{0}']", key));

            if (list.Count == 1)
            {
                XmlNode node = list[0];
                string value = node.Attributes["value"].Value;
                node.Attributes["value"].Value = Encrypt(value);
                doc.Save(path);
            }
        }

        private static void SaveWebConfig(string key, string value)
        {
            try
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "web.config");

                if (File.Exists(filePath))
                {
                    ExeConfigurationFileMap customWebConfig = new ExeConfigurationFileMap();

                    customWebConfig.ExeConfigFilename = Path.GetFileName(filePath);

                    Configuration customConfig = ConfigurationManager.OpenMappedExeConfiguration(customWebConfig, ConfigurationUserLevel.None);

                    AppSettingsSection appSettings = (customConfig.GetSection("appSettings") as AppSettingsSection);

                    appSettings.Settings[key].Value = value;

                    customConfig.Save(ConfigurationSaveMode.Modified);

                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private static void UpsertSetting(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}