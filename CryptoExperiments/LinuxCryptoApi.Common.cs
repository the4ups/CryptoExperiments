namespace CryptoExperiments
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Interop.Linux;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles.System.Security.Cryptography;

    public partial class LinuxCryptoApi
    {
        private readonly ConcurrentDictionary<int, SafeProvHandle> providers = new ();

        private SafeProvHandle TryGetProvider(int providerId) => this.providers.GetOrAdd(
            providerId,
            _ => new Lazy<SafeProvHandle>(() => CreateProvider(providerId)).Value);

        private static SafeProvHandle CreateProvider(int providerId)
        {
            if (!Interop.Libcapi20.CryptAcquireContext(
                out var hProv,
                null,
                null,
                providerId,
                (uint)Interop.Libcapi20.CryptAcquireContextFlags.CRYPT_VERIFYCONTEXT))
            {
                throw Marshal.GetLastWin32Error().ToCryptographicException();
            }

            return hProv;
        }

        private static HashHandleWrapper CreateHashProvider(SafeProvHandle providerHandle, int calgHash)
        {
            if (!Interop.Libcapi20.CryptCreateHash(
                providerHandle,
                calgHash,
                SafeKeyHandle.InvalidHandle,
                Interop.Libcapi20.CryptCreateHashFlags.None,
                out var hHash))
            {
                throw Marshal.GetLastWin32Error().ToCryptographicException();
            }

            hHash.SetParent(providerHandle);

            var cbHashSize = sizeof(int);
            if (!Interop.Libcapi20.CryptGetHashParam(
                hHash,
                Interop.Libcapi20.CryptHashProperty.HP_HASHSIZE,
                out var dwHashSize,
                ref cbHashSize,
                0))
            {
                hHash.Dispose();
                throw Marshal.GetLastWin32Error().ToCryptographicException();
            }

            if (dwHashSize < 0)
            {
                hHash.Dispose();
                throw new PlatformNotSupportedException();
            }

            return new HashHandleWrapper(dwHashSize, hHash);
        }

        internal class HashHandleWrapper : IDisposable
        {
            public HashHandleWrapper(int hashSizeInBytes, SafeHashHandle hashHandle)
            {
                this.HashSizeInBytes = hashSizeInBytes;
                this.HashHandle = hashHandle;
            }

            public int HashSizeInBytes { get; }

            public SafeHashHandle HashHandle { get; }

            public void Dispose()
            {
                this.HashHandle.Dispose();
            }
        }
    }
}
