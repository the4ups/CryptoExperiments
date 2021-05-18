namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {

            [DllImport(
                Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20,
                CharSet = CharSet.Unicode,
                SetLastError = true)]
            internal static extern SafeCertStoreHandle CertOpenStore(
                IntPtr lpszStoreProvider,
                CertEncodingType dwMsgAndCertEncodingType,
                IntPtr hCryptProv,
                CertStoreFlags dwFlags,
                string pvPara);

            public static SafeCertStoreHandle CertOpenStore(
                CertStoreProvider lpszStoreProvider,
                CertEncodingType dwMsgAndCertEncodingType,
                IntPtr hCryptProv,
                CertStoreFlags dwFlags,
                string? pvPara)
            {
                return CertOpenStore((IntPtr)lpszStoreProvider, dwMsgAndCertEncodingType, hCryptProv, dwFlags, pvPara);
            }
        }
    }
}
