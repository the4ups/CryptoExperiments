namespace CryptoExperiments
{
    using System;
    using System.IO;
    using System.Runtime.ExceptionServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography.Pkcs;
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

            var testData = File.ReadAllBytes("/tmp/SampleData.txt");

            Console.WriteLine("================= Hashing ======================");
            var hash = api.ComputeHash(string.Empty, testData);
            Console.WriteLine($"Hash: {Convert.ToHexString(hash)}");
            Console.WriteLine();

            Console.WriteLine("================= Certs store reading ======================");
            var certificates = api.FindByThumbprint(Convert.FromHexString("5AED7061564832AAFB6E00E759C4651263004EAD"));

            var c = certificates.Count > 0 ? certificates[0] : null;

            Console.WriteLine(
                $"Subject: {c?.SubjectName.Name}, Private key:{c?.HasPrivateKey}, " +
                $"Thumbprint: {c?.Thumbprint}, Serial: {c?.SerialNumber}");
            Console.WriteLine();

            Console.WriteLine("================= Encrypt ======================");
            var encryptedTestData = api.Encrypt(c!, testData);
            Console.WriteLine($"Encrypted test data: {Convert.ToHexString(encryptedTestData)}");
            Console.WriteLine();

            Console.WriteLine("================= Make signature ======================");

            var generatedSignature = api.MakeSignature(c!, testData);
            Console.WriteLine($"Generated signature: {Convert.ToHexString(generatedSignature)}");
            Console.WriteLine();

            Console.WriteLine("================= Verify signature generated with MakeSignature ======================");

            api.VerifySignature(
                c!,
                testData,
                generatedSignature,
                "",
                false);
            Console.WriteLine("Verification success!");
            Console.WriteLine();

            Console.WriteLine("================= Verify signature generated with CSP ======================");

            var signature = File.ReadAllBytes("/tmp/SampleDataSign.txt");

            var cmsAsBase64 = Convert.ToBase64String(signature);
            Console.WriteLine($"CMS as base64: {cmsAsBase64}");

            var cms = new SignedCms(new ContentInfo(testData), true);
            cms.Decode(signature);
            //cms.CheckSignature(true);

            api.VerifySignature(
                cms.SignerInfos[0].Certificate,
                testData,
                cms.SignerInfos[0].GetSignature(),
                cms.SignerInfos[0].DigestAlgorithm.Value,
                false);
            Console.WriteLine("Verification success!");
            Console.WriteLine();
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
