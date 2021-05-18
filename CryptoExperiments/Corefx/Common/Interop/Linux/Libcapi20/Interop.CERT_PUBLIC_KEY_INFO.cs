namespace CryptoExperiments
{
    using System.Runtime.InteropServices;

    internal partial class Interop
    {
        internal partial class Libcapi20
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct CERT_PUBLIC_KEY_INFO
            {
                internal CRYPT_ALGORITHM_IDENTIFIER Algorithm;
                internal CRYPT_BIT_BLOB PublicKey;
            }
        }
    }
}
