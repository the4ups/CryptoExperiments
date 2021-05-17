namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal partial class Program
    {
        private static void Main(string[] args)
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            Console.WriteLine($"Is linux: {isLinux}");

            var api = new LinuxCryptoApi();
            var hash = api.ComputeHash(string.Empty, Encoding.UTF8.GetBytes("Hello world!"));

            Console.WriteLine($"Hash: {Convert.ToBase64String(hash)}");
        }
    }
}
