namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CertFreeCertificateContext(IntPtr pCertContext);
        }
    }
}
