﻿namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles.System.Security.Cryptography;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [Flags]
            internal enum CryptSignAndVerifyHashFlags : int
            {
                None = 0x00000000,
                CRYPT_NOHASHOID = 0x00000001,
                CRYPT_TYPE2_FORMAT = 0x00000002,  // Not supported
                CRYPT_X931_FORMAT = 0x00000004,  // Not supported
            }

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CryptVerifySignatureW")]
            public static extern bool CryptVerifySignature(
                SafeHashHandle hHash,
                byte[] pbSignature,
                int dwSigLen,
                SafeKeyHandle hPubKey,
                string? szDescription,
                CryptSignAndVerifyHashFlags dwFlags);
        }
    }
}