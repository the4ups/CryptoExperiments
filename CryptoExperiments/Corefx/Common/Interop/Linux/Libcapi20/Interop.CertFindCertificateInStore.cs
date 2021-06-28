namespace CryptoExperiments
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    internal partial class Interop
    {
        internal partial class Libcapi20
        {
            /// <summary>
            ///     A less error-prone wrapper for CertEnumCertificatesInStore().
            ///     To begin the enumeration, set pCertContext to null. Each iteration replaces pCertContext with
            ///     the next certificate in the iteration. The final call sets pCertContext to an invalid SafeCertStoreHandle
            ///     and returns "false" to indicate the end of the store has been reached.
            /// </summary>
            public static unsafe bool CertFindCertificateInStore(
                SafeCertStoreHandle hCertStore,
                CertFindType dwFindType,
                void* pvFindPara,
                [NotNull] ref SafeCertContextHandle? pCertContext)
            {
                var pPrevCertContext = pCertContext == null ? null : pCertContext.Disconnect();
                pCertContext = CertFindCertificateInStore(
                    hCertStore,
                    CertEncodingType.All,
                    CertFindFlags.None,
                    dwFindType,
                    pvFindPara,
                    pPrevCertContext);
                return !pCertContext.IsInvalid;
            }

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            public static extern unsafe SafeCertContextHandle CertFindCertificateInStore(
                SafeCertStoreHandle hCertStore,
                CertEncodingType dwCertEncodingType,
                CertFindFlags dwFindFlags,
                CertFindType dwFindType,
                void* pvFindPara,
                CERT_CONTEXT* pPrevCertContext);

            public static unsafe bool CertEnumCertificatesInStore(
                SafeCertStoreHandle hCertStore,
                [NotNull] ref SafeCertContextHandle? pCertContext)
            {
                var pPrevCertContext = pCertContext == null ? null : pCertContext.Disconnect();
                pCertContext = CertEnumCertificatesInStore(hCertStore, pPrevCertContext);
                return !pCertContext.IsInvalid;
            }

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20)]
            public static extern unsafe SafeCertContextHandle CertEnumCertificatesInStore(SafeCertStoreHandle hCertStore, CERT_CONTEXT* pPrevCertContext);
        }
    }
}
