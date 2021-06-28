namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles.System.Security.Cryptography;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            internal enum CryptHashProperty : int
            {
                HP_ALGID = 0x0001,  // Hash algorithm
                HP_HASHVAL = 0x0002,  // Hash value
                HP_HASHSIZE = 0x0004,  // Hash value size
                HP_HMAC_INFO = 0x0005,  // information for creating an HMAC
                HP_TLS1PRF_LABEL = 0x0006,  // label for TLS1 PRF
                HP_TLS1PRF_SEED = 0x0007,  // seed for TLS1 PRF
            }

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            public static extern bool CryptGetHashParam(
                SafeHashHandle hHash,
                CryptHashProperty dwParam,
                out int pbData,
                [In, Out] ref int pdwDataLen,
                int dwFlags);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            public static extern bool CryptGetHashParam(
                SafeHashHandle hHash,
                CryptHashProperty dwParam,
                IntPtr pbData,
                [In, Out] ref int pdwDataLen,
                int dwFlags);

            public static bool CryptGetHashParam(
                SafeHashHandle safeHashHandle,
                CryptHashProperty dwParam,
                Span<byte> pbData,
                [In, Out] ref int pwdDataLen,
                int dwFlags)
            {
                if (pbData.IsEmpty)
                {
                    return CryptGetHashParam(safeHashHandle, dwParam, IntPtr.Zero, ref pwdDataLen, dwFlags);
                }

                if (pwdDataLen > pbData.Length)
                {
                    throw new IndexOutOfRangeException();
                }

                unsafe
                {
                    fixed (byte* bytePtr = &MemoryMarshal.GetReference(pbData))
                    {
                        return CryptGetHashParam(safeHashHandle, dwParam, (IntPtr)bytePtr, ref pwdDataLen, 0);
                    }
                }
            }

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            public static extern bool CryptSetHashParam(SafeHashHandle hHash, CryptHashProperty dwParam, byte[] buffer, int dwFlags);
        }
    }
}
