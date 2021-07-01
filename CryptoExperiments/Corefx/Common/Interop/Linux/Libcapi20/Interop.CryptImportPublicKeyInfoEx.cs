namespace CryptoExperiments
{
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern unsafe bool CryptImportPublicKeyInfoEx(
                SafeProvHandle hProvHandle,
                CertEncodingType dwCertEncodingType,
                CERT_PUBLIC_KEY_INFO* pInfo,
                int aiKeyALg,
                uint dwFlags,
                void* pvAuxInfo,
                out SafeKeyHandle phKey);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern unsafe bool CryptImportPublicKeyInfo(
                SafeProvHandle hProvHandle,
                CertEncodingType dwCertEncodingType,
                CERT_PUBLIC_KEY_INFO* pInfo,
                void* pvAuxInfo,
                out SafeKeyHandle phKey);
        }
    }
}
