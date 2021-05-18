namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal partial class Interop
    {
        internal partial class Libcapi20
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct CRYPT_ALGORITHM_IDENTIFIER
            {
                internal IntPtr pszObjId;
                internal DATA_BLOB Parameters;
            }
        }
    }
}
