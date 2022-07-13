using System;
using System.Diagnostics;
using System.IO;

namespace TSCrypto
{
    internal static class Program
    {
        private static string _AwsAccessKeyId;
        private static string _AwsSecretAccessKey;

        private static string _FilePath;

        public static string AwsAccessKeyId => nameof(AwsAccessKeyId);
        public static string AwsSecretAccessKey => nameof(AwsSecretAccessKey);

        private static void ExecuteCommand(string key, string value)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "web.config");

            _FilePath = filePath;
            _AwsAccessKeyId = key;
            _AwsSecretAccessKey = value;

            if (!IsValidArgs(filePath, key, value)) return;
            if (!IsValidFilePath(_FilePath)) return;

            Credentials.AwsAccessKeyId = _AwsAccessKeyId;
            Credentials.AwsSecretAccessKey = _AwsSecretAccessKey;

            PrintVariables();
        }

        private static void ExitProgram()
        {
            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }

        private static void HelpGuidline()
        {
            Console.WriteLine($"For example 1: {nameof(TSCrypto)} -k \"key\" -v \"value\"");
            Console.WriteLine($"For example 2: {nameof(TSCrypto)}.exe -k \"key\" -v \"value\"");
            Console.WriteLine($"For example 3: {nameof(TSCrypto)} -k key -v value");
            Console.WriteLine($"For example 4: {nameof(TSCrypto)}.exe -k key -v value");
        }

        private static bool IsValidArgs(string filePath, string key, string value)
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Invalid arguments: file_path");
                isValid = false;
            }
            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine("Invalid arguments: key");
                isValid = false;
            }
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine("Invalid arguments: value");
                isValid = false;
            }
            return isValid;
        }

        private static bool IsValidFilePath(string filePath)
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine($"Invalid arguments. File not found: '{filePath}'");
                isValid = false;
            }
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Invalid arguments. File not found: '{filePath}'");
                isValid = false;
            }
            return isValid;
        }

        private static void Main(string[] args)
        {
            int maxArgumentsLength = "-k key -v value".Split(new[] { ' ' }).Length;

            if (args.Length != maxArgumentsLength)
            {
                Console.WriteLine("Invalid arguments");
                HelpGuidline();
                ExitProgram();
                return;
            }

            var command = args[0];

            //TSCrypto -k "key" -v "value"
            //TSCrypto.exe -k "key" -v "value"
            switch (command)
            {
                case "-k" when args.Length == maxArgumentsLength:
                    ExecuteCommand(args[1], args[3]);
                    break;

                default:
                    Console.WriteLine("Invalid command.");
                    HelpGuidline();
                    break;
            }

            ExitProgram();
        }

        private static void PrintVariables()
        {
            Console.WriteLine($"FilePath: '{_FilePath}'");

            Console.WriteLine($"{nameof(AwsAccessKeyId)}: '{_AwsAccessKeyId}'");
            Console.WriteLine($"{nameof(AwsSecretAccessKey)}: '{_AwsSecretAccessKey}'");

            Console.WriteLine($"{nameof(AwsAccessKeyId)}: '{Credentials.GetValueWebConfig(nameof(AwsAccessKeyId))}'");
            Console.WriteLine($"{nameof(AwsSecretAccessKey)}: '{Credentials.GetValueWebConfig(nameof(AwsSecretAccessKey))}'");

            Process.Start(_FilePath);
        }
    }
}