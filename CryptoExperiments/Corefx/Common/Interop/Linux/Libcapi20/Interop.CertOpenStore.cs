namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, CharSet = CharSet.Ansi, SetLastError = true)]
            internal static extern SafeCertStoreHandle CertOpenStore(
                CertStoreProvider lpszStoreProvider,
                CertEncodingType dwMsgAndCertEncodingType,
                IntPtr hCryptProv,
                CertStoreFlags dwFlags,
                [In, MarshalAs(UnmanagedType.LPStr)] string pvPara);
        }
    }
}
