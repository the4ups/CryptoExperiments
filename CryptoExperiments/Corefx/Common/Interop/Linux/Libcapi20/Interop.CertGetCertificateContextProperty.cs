namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CertGetCertificateContextProperty(
                SafeCertContextHandle pCertContext,
                CertContextPropId dwPropId,
                [Out] byte[]? pvData,
                [In, Out] ref int pcbData);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CertGetCertificateContextProperty(
                SafeCertContextHandle pCertContext,
                CertContextPropId dwPropId,
                out IntPtr pvData,
                [In, Out] ref int pcbData);
        }
    }
}
