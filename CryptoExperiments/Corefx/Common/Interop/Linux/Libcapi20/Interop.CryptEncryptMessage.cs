namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            public static extern bool CryptEncryptMessage(
                ref CRYPT_ENCRYPT_MESSAGE_PARA pEncryptPara,
                uint cRecipientCert,
                [In] [MarshalAs(UnmanagedType.LPArray)]
                IntPtr[] rgpRecipientCert,
                byte[] pbToBeEncrypted,
                int cbToBeEncrypted,
                [In] [Out] byte[] pbEncryptedBlob,
                ref int pcbEncryptedBlob);
        }
    }
}
