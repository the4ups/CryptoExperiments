namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern SafeCertStoreHandle CertOpenSystemStoreA(IntPtr hProv, string szSubsystemProtocol);
        }
    }
}
