namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal partial class Interop
    {
        internal partial class Libcapi20
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct DATA_BLOB
            {
                internal uint cbData;
                internal IntPtr pbData;

                internal DATA_BLOB(IntPtr handle, uint size)
                {
                    this.cbData = size;
                    this.pbData = handle;
                }
            }
        }
    }
}
