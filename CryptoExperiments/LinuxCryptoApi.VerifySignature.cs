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
        public void VerifySignature(
            X509Certificate2 certificate,
            byte[] content,
            byte[] signature,
            string digestAlgorithmOid,
            bool isContentDigest)
        {
            var providerType = 80;
            var hProv = this.TryGetProvider(providerType);

            var hashAlgorithmId = 0x8021;

            using (var hHash = isContentDigest
                ? CreateHashHandle(hProv, hashAlgorithmId, content)
                : CreateAndCryptHash(hProv, hashAlgorithmId, content))
            {
                var signatureToVerify = (byte[])signature.Clone();
                Array.Reverse(signatureToVerify);

                // here context became invalid
                using (var certificateContext = CreateCertificateContextHandle(certificate))
                {
                    using (var hPublicKey = ImportPublicKeyInfo(hProv, certificateContext))
                    {
                        if (!Interop.Libcapi20.CryptVerifySignature(
                            hHash,
                            signatureToVerify,
                            signatureToVerify.Length,
                            hPublicKey,
                            null,
                            Interop.Libcapi20.CryptSignAndVerifyHashFlags.None))
                        {
                            throw Marshal.GetLastWin32Error().ToCryptographicException();
                        }
                    }
                }
            }
        }

        private static SafeKeyHandle ImportPublicKeyInfo(
            SafeProvHandle hProv,
            Interop.SafeCertContextHandle certContext)
        {
            unsafe
            {
                var mustRelease = false;
                certContext.DangerousAddRef(ref mustRelease);

                try
                {
                    if (!Interop.Libcapi20.CryptImportPublicKeyInfoEx(
                        hProv,
                        Interop.CertEncodingType.All,
                        &certContext.CertContext->pCertInfo->SubjectPublicKeyInfo,
                        0,
                        null,
                        out var hPublicKey))
                    {
                        throw Marshal.GetLastWin32Error().ToCryptographicException();
                    }

                    return hPublicKey;
                }
                finally
                {
                    if (mustRelease)
                    {
                        certContext.DangerousRelease();
                    }
                }
            }
        }

        private static Interop.SafeCertContextHandle CreateCertificateContextHandle(X509Certificate2 certificate)
        {
            var safeHandle = new Interop.SafeCertContextHandle();
            safeHandle.SetHandle(certificate.Handle);
            return safeHandle.Duplicate();
        }

        private static SafeHashHandle CreateHashHandle(SafeProvHandle hProv, int calgHash, byte[] hash)
        {
            if (!Interop.Libcapi20.CryptCreateHash(
                hProv,
                calgHash,
                SafeKeyHandle.InvalidHandle,
                Interop.Libcapi20.CryptCreateHashFlags.None,
                out var hHash))
            {
                var hr = Marshal.GetLastWin32Error();
                hHash.Dispose();

                throw hr.ToCryptographicException();
            }

            try
            {
                var cbHashSize = sizeof(int);
                if (!Interop.Libcapi20.CryptGetHashParam(
                    hHash,
                    Interop.Libcapi20.CryptHashProperty.HP_HASHSIZE,
                    out var dwHashSize,
                    ref cbHashSize,
                    0))
                {
                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                if (dwHashSize != hash.Length)
                {
                    throw unchecked((int)0x80090003).ToCryptographicException();
                }

                if (!Interop.Libcapi20.CryptSetHashParam(
                    hHash,
                    Interop.Libcapi20.CryptHashProperty.HP_HASHVAL,
                    hash,
                    0))
                {
                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                var hHashPermanent = hHash;
                hHash = null;
                return hHashPermanent;
            }
            finally
            {
                hHash?.Dispose();
            }

            return null;
        }

        private static SafeHashHandle CreateAndCryptHash(SafeProvHandle hProv, int calgHash, byte[] hash)
        {
            var hashProvider = CreateHashProvider(hProv, calgHash);

            if (!Interop.Libcapi20.CryptHashData(
                hashProvider.HashHandle,
                hash,
                hash.Length,
                0))
            {
                throw Marshal.GetLastWin32Error().ToCryptographicException();
            }

            return hashProvider.HashHandle;
        }
    }
}
