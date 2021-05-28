namespace CryptoExperiments
{
    using System;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            public struct CRYPT_ENCRYPT_MESSAGE_PARA
            {
                public int cbSize;

                public uint dwMsgEncodingType;

                public int hCryptProv;

                public CRYPT_ALGORITHM_IDENTIFIER ContentEncryptionAlgorithm;

                public IntPtr pvEncryptionAuxInfo;

                public int dwFlags;

                public int dwInnerContentType;
            }
        }
    }
}
