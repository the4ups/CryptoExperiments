namespace CryptoExperiments
{
    using System;

    internal partial class Interop
    {
        internal partial class Libcapi20
        {
            [Flags]
            internal enum MsgEncodingType : int
            {
                PKCS_7_ASN_ENCODING = 0x10000,
                X509_ASN_ENCODING = 0x00001,

                All = PKCS_7_ASN_ENCODING | X509_ASN_ENCODING,
            }
        }
    }
}
