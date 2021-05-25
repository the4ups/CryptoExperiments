namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using CryptoExperiments.Corefx.Common.Interop.Linux;

    public partial class LinuxCryptoApi
    {
        public byte[] Encrypt(X509Certificate2 cert, byte[] document)
        {
            // 0) Инициализация параметров
            var pParams = new Interop.Libcapi20.CRYPT_ENCRYPT_MESSAGE_PARA();
            pParams.dwMsgEncodingType = Interop.CertEncodingType.All;
            pParams.ContentEncryptionAlgorithm.pszObjId = GetEncodeAlgorithmOid(cert.PublicKey.Oid);
            pParams.cbSize = Marshal.SizeOf(pParams);

            return Array.Empty<byte>();
        }

        private IntPtr GetEncodeAlgorithmOid(Oid publicKeyOid)
        {
            var hProv = 80;
            var providerHandle = TryGetProvider(hProv);

            var enumFlag = 1;
            var cbFeex = Marshal.SizeOf(typeof(Interop.Libcapi20.PROV_ENUMALGS_EX));

            Span<byte> stackSpan = stackalloc byte[cbFeex];

            stackSpan.Clear();
            int size = stackSpan.Length;

            if (!Interop.Libcapi20.CryptGetProvParam(
                providerHandle,
                (Interop.Libcapi20.CryptProvParam)22,
                stackSpan,
                ref size,
                enumFlag))
            {
                throw Marshal.GetLastWin32Error().ToCryptographicException();
            }

            var peex = MemoryMarshal.Read<Interop.Libcapi20.PROV_ENUMALGS_EX>(stackSpan[..size]);

            return IntPtr.Zero;
        }
    }
}
