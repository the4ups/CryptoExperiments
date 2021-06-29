namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using CryptoExperiments.Corefx.Common.Interop.Linux;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles.System.Security.Cryptography;

    public partial class LinuxCryptoApi
    {
        public byte[] MakeSignature(X509Certificate2 certificate, byte[] content)
        {
            SafeProvHandle hProv = SafeProvHandle.InvalidHandle;
            var fCallerFreeProv = false;

            SafeHashHandle hHash = SafeHashHandle.InvalidHandle;

            try
            {
                var certContext = FindByThumbprint1(Convert.FromHexString("68da674f6c7c1eb57a2ec53becb0892a9247d632"));
                Interop.Libcapi20.CryptKeySpec dwKeySpec = 0;

                if (!Interop.Libcapi20.CryptAcquireCertificatePrivateKey(
                    certContext,
                    Interop.Libcapi20.CryptAcquireCertificatePrivateKeyFlags.CRYPT_ACQUIRE_COMPARE_KEY_FLAG,
                    IntPtr.Zero,
                    out hProv,
                    out dwKeySpec,
                    out fCallerFreeProv))
                {
                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                var hashAlgorithmId = GetHashAlgorithmId(certificate.PublicKey.Oid.Value);
                if (!Interop.Libcapi20.CryptCreateHash(
                    hProv,
                    hashAlgorithmId,
                    SafeKeyHandle.InvalidHandle,
                    Interop.Libcapi20.CryptCreateHashFlags.None,
                    out hHash))
                {
                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                int dwSigLen = 0;
                if (!Interop.Libcapi20.CryptSignHash(
                    hHash,
                    (Interop.Libcapi20.KeySpec)dwKeySpec,
                    null,
                    Interop.Libcapi20.CryptSignAndVerifyHashFlags.None,
                    null,
                    ref dwSigLen))
                {
                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                var signature = new byte[dwSigLen];
                if (!Interop.Libcapi20.CryptSignHash(
                    hHash,
                    (Interop.Libcapi20.KeySpec)dwKeySpec,
                    null,
                    Interop.Libcapi20.CryptSignAndVerifyHashFlags.None,
                    signature,
                    ref dwSigLen))
                {
                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                Array.Reverse(signature);
                return signature;
            }
            finally
            {
                // if (hHash != SafeHashHandle.InvalidHandle)
                // {
                //     Interop.Libcapi20.CryptDestroyHash(hHash);
                // }
                //
                // if (hProv != SafeProvHandle.InvalidHandle && fCallerFreeProv)
                // {
                //     Interop.Libcapi20.CryptReleaseContext(hProv, 0);
                // }
            }
        }

        private static int GetHashAlgorithmId(string publicKEyOid)
        {
            return 0x8021;
        }
    }
}
