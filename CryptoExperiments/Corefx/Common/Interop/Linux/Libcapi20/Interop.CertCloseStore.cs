namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            /// <summary>
            /// Flags for the <see cref="CertCloseStore(IntPtr, CertCloseStoreFlags)"/> method.
            /// </summary>
            [Flags]
            internal enum CertCloseStoreFlags
            {
                /// <summary>
                /// The default is to close the store with memory remaining allocated for contexts that have not been freed. In this case, no check is made to determine whether memory for contexts remains allocated.
                /// </summary>
                None = 0x0,

                /// <summary>
                /// Forces the freeing of memory for all contexts associated with the store. This flag can be safely used only when the store is opened in a function and neither the store handle nor any of its contexts are passed to any called functions. For details, see Remarks.
                /// </summary>
                CERT_CLOSE_STORE_FORCE_FLAG = 0x1,

                /// <summary>
                /// Checks for nonfreed certificate, CRL, and CTL contexts. A returned error code indicates that one or more store elements is still in use. This flag should only be used as a diagnostic tool in the development of applications.
                /// </summary>
                CERT_CLOSE_STORE_CHECK_FLAG = 0x2,
            }

            [DllImport(
                Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20,
                CharSet = CharSet.Unicode,
                SetLastError = true)]
            internal static extern bool CertCloseStore(IntPtr hCertStore, CertCloseStoreFlags dwFlags = CertCloseStoreFlags.None);
        }
    }
}
