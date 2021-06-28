namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(
                Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20,
                CharSet = CharSet.Ansi,
                SetLastError = true)]
            public static extern bool CertAddCertificateLinkToStore(
                SafeCertStoreHandle hCertStore,
                SafeCertContextHandle pCertContext,
                CertStoreAddDisposition dwAddDisposition,
                IntPtr ppStoreContext);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, CharSet = CharSet.Ansi, SetLastError = true)]
            internal static extern bool CertAddCertificateLinkToStore(
                SafeCertStoreHandle hCertStore,
                SafeCertContextHandle pCertContext,
                uint dwAddDisposition,
                [In] [Out] SafeCertContextHandle ppStoreContext);
        }
    }
}
