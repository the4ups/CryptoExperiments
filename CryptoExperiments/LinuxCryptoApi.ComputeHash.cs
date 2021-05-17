namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Interop.Linux;

    public partial class LinuxCryptoApi
    {
        public byte[] ComputeHash(string digestAlgorithmOid, byte[] data)
        {
            var providerId = 80;
            var hProv = this.TryGetProvider(providerId);

            var hashAlgorithmId = (int)0x8021;

            using (var hashProvider = CreateHashProvider(hProv, hashAlgorithmId))
            {
                if (!Interop.Libcapi20.CryptHashData(
                    hashProvider.HashHandle,
                    data,
                    data.Length,
                    0))
                {
                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                var hashSize = hashProvider.HashSizeInBytes;
                var hash = new byte[hashProvider.HashSizeInBytes];

                if (!Interop.Libcapi20.CryptGetHashParam(
                    hashProvider.HashHandle,
                    Interop.Libcapi20.CryptHashProperty.HP_HASHVAL,
                    hash,
                    ref hashSize,
                    0))
                {
                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                return hash;
            }
        }
    }
}
