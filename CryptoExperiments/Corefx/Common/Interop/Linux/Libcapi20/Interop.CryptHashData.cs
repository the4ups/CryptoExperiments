namespace CryptoExperiments
{
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles.System.Security.Cryptography;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(
                Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20,
                CharSet = CharSet.Unicode,
                SetLastError = true)]
            public static extern bool CryptHashData(SafeHashHandle hHash, byte[] pbData, int dwDataLen, int dwFlags);
        }
    }
}
