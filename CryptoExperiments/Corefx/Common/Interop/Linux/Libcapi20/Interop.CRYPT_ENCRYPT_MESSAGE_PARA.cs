namespace CryptoExperiments
{
    using System;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            public struct CRYPT_ENCRYPT_MESSAGE_PARA
            {
                public int cbSize;

                public CertEncodingType dwMsgEncodingType;

                public int hCryptProv;

                public CRYPT_ALGORITHM_IDENTIFIER ContentEncryptionAlgorithm;

                public IntPtr pvEncryptionAuxInfo;

                public int dwFlags;

                public int dwInnerContentType;
            }
        }
    }
}
