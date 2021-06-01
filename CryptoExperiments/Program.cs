namespace CryptoExperiments
{
    using System;
    using System.IO;
    using System.Runtime.ExceptionServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography.Pkcs;
    using System.Text;
    using System.Threading.Tasks;

    internal partial class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

                var t = new Task(Run);
                t.Start();
                t.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Run()
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            Console.WriteLine($"Is linux: {isLinux}");

            var api = new LinuxCryptoApi();

            var testData = Encoding.UTF8.GetBytes("Hello world!");

            Console.WriteLine("================= Hashing ======================");
            var hash = api.ComputeHash(string.Empty, testData);
            Console.WriteLine($"Hash: {Convert.ToHexString(hash)}");
            Console.WriteLine();

            Console.WriteLine("================= Certs store reading ======================");
            var c = api.FindByThumbprint(Convert.FromHexString("68da674f6c7c1eb57a2ec53becb0892a9247d632"));
            Console.WriteLine(
                $"Subject: {c?.SubjectName.Name}, Private key:{c?.HasPrivateKey}, " +
                $"Thumbprint: {c?.Thumbprint}, Serial: {c?.SerialNumber}");
            Console.WriteLine();

            Console.WriteLine("================= Encrypt ======================");
            Console.WriteLine("Skip while access violation.");
            // var encryptedTestData = api.Encrypt(c!, testData);
            // Console.WriteLine($"Encrypted test data: {Convert.ToHexString(encryptedTestData)}");
            Console.WriteLine();

            Console.WriteLine("================= Verify signature ======================");

            var signedContent = File.ReadAllBytes("/tmp/SampleData.txt");
            var signature = File.ReadAllBytes("/tmp/SampleDataSign.txt");

            var cms = new SignedCms(new ContentInfo(signedContent), true);
            cms.Decode(signature);

            //api.VerifySignature(c!, signedContent, signature, cms.SignerInfos[0].DigestAlgorithm.Value, false);
            Console.WriteLine();

            Console.WriteLine("================= Make signature ======================");

            var generatedSignature = api.MakeSignature(c!, signedContent);
            Console.WriteLine($"Generated signature: {Convert.ToHexString(generatedSignature)}");
        }

        // but it's important that this method is marked
        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // this will catch all unhandled exceptions, including CSEs
            Console.WriteLine(e.ExceptionObject as Exception);
        }
    }
}
