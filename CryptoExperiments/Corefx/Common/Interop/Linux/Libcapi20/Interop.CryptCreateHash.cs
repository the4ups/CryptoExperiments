namespace CryptoExperiments
{
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles.System.Security.Cryptography;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            internal enum CryptCreateHashFlags : int
            {
                None = 0
            }

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CryptCreateHash(
                SafeProvHandle hProv,
                int algId,
                SafeKeyHandle hKey,
                CryptCreateHashFlags swFlags,
                out SafeHashHandle phHash);
        }
    }
}
