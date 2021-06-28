namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CryptAcquireCertificatePrivateKey(
                SafeCertContextHandle pCert,
                CryptAcquireCertificatePrivateKeyFlags dwFlags,
                IntPtr pvParameters,
                out SafeProvHandle phCryptProvOrNCryptKey,
                out CryptKeySpec pdwKeySpec,
                out bool pfCallerFreeProvOrNCryptKey);

            [Flags]
            internal enum CryptAcquireCertificatePrivateKeyFlags
            {
                CRYPT_ACQUIRE_CACHE_FLAG = 0x00000001,
                CRYPT_ACQUIRE_USE_PROV_INFO_FLAG = 0x00000002,
                CRYPT_ACQUIRE_COMPARE_KEY_FLAG = 0x00000004,
                CRYPT_ACQUIRE_NO_HEALING = 0x00000008,

                CRYPT_ACQUIRE_SILENT_FLAG = 0x00000040,
                CRYPT_ACQUIRE_WINDOW_HANDLE_FLAG = 0x00000080,

                CRYPT_ACQUIRE_NCRYPT_KEY_FLAGS_MASK = 0x00070000,
                CRYPT_ACQUIRE_ALLOW_NCRYPT_KEY_FLAG = 0x00010000,
                CRYPT_ACQUIRE_PREFER_NCRYPT_KEY_FLAG = 0x00020000,
                CRYPT_ACQUIRE_ONLY_NCRYPT_KEY_FLAG = 0x00040000,
            }

            internal enum CryptKeySpec
            {
                AT_KEYEXCHANGE = 1,
                AT_SIGNATURE = 2,

                CERT_NCRYPT_KEY_SPEC =
                    -1, // Special sentinel indicating that an api returned an NCrypt key rather than a CAPI key
            }
        }
    }
}
