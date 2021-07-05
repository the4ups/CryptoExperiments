namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(
                Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20,
                SetLastError = true,
                CharSet = CharSet.Ansi,
                EntryPoint = "CryptAcquireContextA")]
            internal static extern bool CryptAcquireContext(
                out SafeProvHandle phProv,
                string? szContainer,
                string? szProvider,
                int dwProvType,
                uint dwFlags);

            [Flags]
            internal enum CryptAcquireContextFlags : uint
            {
                None = 0x00000000,
                CRYPT_NEWKEYSET = 0x00000008, // CRYPT_NEWKEYSET
                CRYPT_DELETEKEYSET = 0x00000010, // CRYPT_DELETEKEYSET
                CRYPT_MACHINE_KEYSET = 0x00000020, // CRYPT_MACHINE_KEYSET
                CRYPT_SILENT = 0x00000040, // CRYPT_SILENT
                CRYPT_VERIFYCONTEXT = 0xF0000000, // CRYPT_VERIFYCONTEXT
            }
        }
    }
}
