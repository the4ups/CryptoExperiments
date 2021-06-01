namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern SafeCertContextHandle CertDuplicateCertificateContext(IntPtr pCertContext);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, CharSet = CharSet.Unicode, SetLastError = true,
                EntryPoint = "CertDuplicateCertificateContext")]
            internal static extern IntPtr CertDuplicateCertificateContext2(IntPtr pCertContext);
        }
    }
}
