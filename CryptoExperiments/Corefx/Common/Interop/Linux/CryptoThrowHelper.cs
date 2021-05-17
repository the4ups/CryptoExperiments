namespace CryptoExperiments.Corefx.Common.Interop.Linux
{
    using System.Security.Cryptography;

    public static class CryptoThrowHelper
    {
        public static CryptographicException ToCryptographicException(this int hr)
        {
            return new LinuxCryptographicException(hr);
        }

        private sealed class LinuxCryptographicException : CryptographicException
        {
            public LinuxCryptographicException(int hr)
            {
                this.HResult = hr;
            }
        }
    }
}
