using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace TSCrypto
{
    /// <summary>
    ///
    /// https://www.aspsnippets.com/Articles/Encrypt-AppSettings-Key-Tags-in-WebConfig-File-in-ASPNet-using-C-and-VBNet.aspx
    /// </summary>
    internal static class Program
    {
        private static readonly string EncryptionKey = "ThirdSight@Secret";

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
            string password = Decrypt(value);
            return password;
        }

        private static void DecryptAppSettingDemo()
        {
            string password = DecryptAppSetting("Password");
            Console.WriteLine(password);
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

        private static void EncryptAppSettingDemo()
        {
            EncryptAppSetting("Password");
            string path = Path.Combine(Environment.CurrentDirectory, "web.config");

            string notepad = $@"{Environment.ExpandEnvironmentVariables("%WINDIR%")}\System32\Notepad.exe";
            Process.Start(notepad, path);
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint GetWindowsDirectory(StringBuilder lpBuffer, uint uSize);

        private static void Main(string[] args)
        {
            string winDir = Environment.GetEnvironmentVariable("windir");
            Debug.Print(winDir);

            winDir = Environment.ExpandEnvironmentVariables("%WINDIR%");
            Debug.Print(winDir);

            winDir = WindowsDirectory();
            Debug.Print(winDir);

            string rootDir = Path.GetPathRoot(WindowsDirectory());
            Debug.Print(rootDir);

            EncryptAppSettingDemo();
            DecryptAppSettingDemo();

            Console.Write("Press any key to exit");
            Console.ReadKey();
        }

        private static string WindowsDirectory()
        {
            uint size = 0;
            size = GetWindowsDirectory(null, size);

            StringBuilder sb = new StringBuilder((int)size);
            GetWindowsDirectory(sb, size);

            return sb.ToString();
        }
    }
}