namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using CryptoExperiments.Corefx.Common.Interop.Linux;
    using CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles;

    public partial class LinuxCryptoApi
    {
        public byte[] Encrypt(IX509Certificate certificate, byte[] document)
        {
            var certContext = new Interop.SafeCertContextHandle();
            certContext.SetHandle(certificate.CertificateHandle);

            // 0) Инициализация параметров
            var encryptionAlgorithm = this.GetEncodeAlgorithmOid(certificate.X509Certificate2.PublicKey.Oid);

            var pParams = new Interop.Libcapi20.CRYPT_ENCRYPT_MESSAGE_PARA();

            pParams.dwMsgEncodingType = (uint)Interop.CertEncodingType.All;
            pParams.ContentEncryptionAlgorithm.pszObjId = encryptionAlgorithm.pszOID;
            pParams.cbSize = Marshal.SizeOf(pParams);

            try
            {
                var iLen = 0;
                if (!Interop.Libcapi20.CryptEncryptMessage(
                    ref pParams,
                    1,
                    new[] { certContext.DangerousGetHandle() },
                    document,
                    document.Length,
                    null,
                    ref iLen))
                {
                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                var result = new byte[iLen];
                if (!Interop.Libcapi20.CryptEncryptMessage(
                    ref pParams,
                    1,
                    new[] { certContext.DangerousGetHandle() },
                    document,
                    document.Length,
                    result,
                    ref iLen))
                {
                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Interop.Libcapi20.CRYPT_OID_INFO GetEncodeAlgorithmOid(Oid publicKeyOid)
        {
            var hProv = 80;
            var providerHandle = TryGetProvider(hProv);

            var enumFlag = 1;
            var cbFeex = Marshal.SizeOf(typeof(Interop.Libcapi20.PROV_ENUMALGS_EX));

            Span<byte> provParamSpan = stackalloc byte[cbFeex];
            provParamSpan.Clear();

            var size = provParamSpan.Length;

            if (!Interop.Libcapi20.CryptGetProvParam(
                providerHandle,
                (Interop.Libcapi20.CryptProvParam)22,
                provParamSpan,
                ref size,
                enumFlag))
            {
                throw Marshal.GetLastWin32Error().ToCryptographicException();
            }

            var provEnumAlgsEx = MarshalAs<Interop.Libcapi20.PROV_ENUMALGS_EX>(provParamSpan[..size]);

            var oidInfo = CryptFindOIDInfo(provEnumAlgsEx.aiAlgid);

            return oidInfo;
        }

        private static T MarshalAs<T>(Span<byte> stackSpan) where T : struct
        {
            unsafe
            {
                fixed (byte* bytePtr = &MemoryMarshal.GetReference(stackSpan))
                {
                    return Marshal.PtrToStructure<T>((IntPtr)bytePtr);
                }
            }
        }

        private static Interop.Libcapi20.CRYPT_OID_INFO CryptFindOIDInfo(uint algId)
        {
            var algIdSpan = new Span<byte>(BitConverter.GetBytes(algId));

            unsafe
            {
                fixed (byte* bytePtr = &MemoryMarshal.GetReference(algIdSpan))
                {
                    var fullOidInfo = Interop.Libcapi20.CryptFindOIDInfo(
                        Interop.Libcapi20.CryptOidInfoKeyType.CRYPT_OID_INFO_ALGID_KEY,
                        (IntPtr)bytePtr,
                        OidGroup.EncryptionAlgorithm);

                    if (fullOidInfo == IntPtr.Zero)
                    {
                        throw Marshal.GetLastWin32Error().ToCryptographicException();
                    }

                    return Marshal.PtrToStructure<Interop.Libcapi20.CRYPT_OID_INFO>(fullOidInfo);
                }
            }
        }
    }
}
