namespace CryptoExperiments
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.Pkcs;

    internal class Program
    {
        public static void Main(string[] args)
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
                $"Subject: {c?.X509Certificate2.SubjectName.Name}, Private key:{c?.ContainsPrivateKey}, " +
                $"Thumbprint: {c?.X509Certificate2.Thumbprint}, Serial: {c?.X509Certificate2.SerialNumber}");
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
                c!.X509Certificate2,
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
    }
}
