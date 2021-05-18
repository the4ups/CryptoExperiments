namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal partial class Interop
    {
        internal partial class Libcapi20
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct CERT_INFO
            {
                internal int dwVersion;
                internal DATA_BLOB SerialNumber;
                internal CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;
                internal DATA_BLOB Issuer;
                internal System.Runtime.InteropServices.ComTypes.FILETIME NotBefore;
                internal System.Runtime.InteropServices.ComTypes.FILETIME NotAfter;
                internal DATA_BLOB Subject;
                internal CERT_PUBLIC_KEY_INFO SubjectPublicKeyInfo;
                internal CRYPT_BIT_BLOB IssuerUniqueId;
                internal CRYPT_BIT_BLOB SubjectUniqueId;
                internal int cExtension;
                internal IntPtr rgExtension;
            }
        }
    }
}
