namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal partial class Interop
    {
        internal partial class Libcapi20
        {
            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct CERT_CONTEXT
            {
                internal MsgEncodingType dwCertEncodingType;
                internal byte* pbCertEncoded;
                internal int cbCertEncoded;
                internal CERT_INFO* pCertInfo;
                internal IntPtr hCertStore;
            }
        }
    }
}
