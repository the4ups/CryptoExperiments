namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal partial class Interop
    {
        internal partial class Libcapi20
        {
            internal enum CryptProvParam : int
            {
                PP_CLIENT_HWND = 1,
                PP_IMPTYPE = 3,
                PP_NAME = 4,
                PP_CONTAINER = 6,
                PP_PROVTYPE = 16,
                PP_KEYSET_TYPE = 27,
                PP_KEYEXCHANGE_PIN = 32,
                PP_SIGNATURE_PIN = 33,
                PP_UNIQUE_CONTAINER = 36
            }

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            public static extern bool CryptGetProvParam(
                SafeHandle safeProvHandle,
                CryptProvParam dwParam,
                IntPtr pbData,
                ref int dwDataLen,
                int dwFlags);

            public static bool CryptGetProvParam(
                SafeHandle safeProvHandle,
                CryptProvParam dwParam,
                Span<byte> pbData,
                ref int dwDataLen,
                int dwFlags)
            {
                if (pbData.IsEmpty)
                {
                    return CryptGetProvParam(safeProvHandle, dwParam, IntPtr.Zero, ref dwDataLen, dwFlags);
                }

                if (dwDataLen > pbData.Length)
                {
                    throw new IndexOutOfRangeException();
                }

                unsafe
                {
                    fixed (byte* bytePtr = &MemoryMarshal.GetReference(pbData))
                    {
                        return CryptGetProvParam(safeProvHandle, dwParam, (IntPtr)bytePtr, ref dwDataLen, dwFlags);
                    }
                }
            }
        }
    }
}
