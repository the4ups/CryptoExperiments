namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal partial class Program
    {
        public static void Main(string[] args)
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
            Console.WriteLine($"Subject: {c?.SubjectName.Name}, Private key:{c?.HasPrivateKey}, " +
                              $"Thumbrint: {c?.Thumbprint}, Serial: {c?.SerialNumber}");

            Console.WriteLine("================= Encrypt ======================");
            var encryptedTesData = api.Encrypt(c!, testData);
            Console.WriteLine($"Hash: {Convert.ToHexString(encryptedTesData)}");
        }
    }
}
