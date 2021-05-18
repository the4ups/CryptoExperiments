namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal partial class Interop
    {
        internal partial class Libcapi20
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct CRYPT_BIT_BLOB
            {
                internal int cbData;
                internal IntPtr pbData;
                internal int cUnusedBits;

                internal byte[] ToByteArray()
                {
                    int numBytes = this.cbData;
                    byte[] data = new byte[numBytes];
                    Marshal.Copy(this.pbData, data, 0, numBytes);
                    return data;
                }
            }
        }
    }
}
