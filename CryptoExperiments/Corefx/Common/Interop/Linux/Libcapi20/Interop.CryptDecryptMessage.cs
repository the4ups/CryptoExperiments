namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Interop.Linux;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CryptDecryptMessage(
                ref CRYPT_DECRYPT_MESSAGE_PARA pDecryptPara,
                byte[] pbEncryptedBlob,
                int cbEncryptedBlob,
                [In, Out] byte[] pbDecrypted,
                ref int pcbDecrypted,
                IntPtr ppXchgCert);

            [StructLayout(LayoutKind.Sequential)]
            internal struct CRYPT_DECRYPT_MESSAGE_PARA
            {
                public Int32 cbSize;
                public Int32 dwMsgAndCertEncodingType;
                public Int32 cCertStore;
                public IntPtr rghCertStore;
                public Int32 dwFlags;
            }

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CryptMsgGetParam(
                SafeCryptMsgHandle hCryptMsg,
                CryptMsgParamType dwParamType,
                int dwIndex,
                out int pvData,
                [In, Out] ref int pcbData);

            // [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, CharSet = CharSet.Unicode, SetLastError = true)]
            // internal static extern bool CryptMsgGetParam(SafeCryptMsgHandle hCryptMsg, CryptMsgParamType dwParamType, int dwIndex, out CryptMsgType pvData, [In, Out] ref int pcbData);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool CryptMsgGetParam(SafeCryptMsgHandle hCryptMsg, CryptMsgParamType dwParamType, int dwIndex, [Out] byte[] pvData, [In, Out] ref int pcbData);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool CryptMsgGetParam(SafeCryptMsgHandle hCryptMsg, CryptMsgParamType dwParamType, int dwIndex, IntPtr pvData, [In, Out] ref int pcbData);
            
            internal enum CryptMsgParamType : int
            {
                CMSG_TYPE_PARAM = 1,
                CMSG_CONTENT_PARAM = 2,
                CMSG_BARE_CONTENT_PARAM = 3,
                CMSG_INNER_CONTENT_TYPE_PARAM = 4,
                CMSG_SIGNER_COUNT_PARAM = 5,
                CMSG_SIGNER_INFO_PARAM = 6,
                CMSG_SIGNER_CERT_INFO_PARAM = 7,
                CMSG_SIGNER_HASH_ALGORITHM_PARAM = 8,
                CMSG_SIGNER_AUTH_ATTR_PARAM = 9,
                CMSG_SIGNER_UNAUTH_ATTR_PARAM = 10,
                CMSG_CERT_COUNT_PARAM = 11,
                CMSG_CERT_PARAM = 12,
                CMSG_CRL_COUNT_PARAM = 13,
                CMSG_CRL_PARAM = 14,
                CMSG_ENVELOPE_ALGORITHM_PARAM = 15,
                CMSG_RECIPIENT_COUNT_PARAM = 17,
                CMSG_RECIPIENT_INDEX_PARAM = 18,
                CMSG_RECIPIENT_INFO_PARAM = 19,
                CMSG_HASH_ALGORITHM_PARAM = 20,
                CMSG_HASH_DATA_PARAM = 21,
                CMSG_COMPUTED_HASH_PARAM = 22,
                CMSG_ENCRYPT_PARAM = 26,
                CMSG_ENCRYPTED_DIGEST = 27,
                CMSG_ENCODED_SIGNER = 28,
                CMSG_ENCODED_MESSAGE = 29,
                CMSG_VERSION_PARAM = 30,
                CMSG_ATTR_CERT_COUNT_PARAM = 31,
                CMSG_ATTR_CERT_PARAM = 32,
                CMSG_CMS_RECIPIENT_COUNT_PARAM = 33,
                CMSG_CMS_RECIPIENT_INDEX_PARAM = 34,
                CMSG_CMS_RECIPIENT_ENCRYPTED_KEY_INDEX_PARAM = 35,
                CMSG_CMS_RECIPIENT_INFO_PARAM = 36,
                CMSG_UNPROTECTED_ATTR_PARAM = 37,
                CMSG_SIGNER_CERT_ID_PARAM = 38,
                CMSG_CMS_SIGNER_INFO_PARAM = 39,
            }

            public sealed class SafeCryptMsgHandle : SafeHandle
            {
                public SafeCryptMsgHandle() :
                    base(IntPtr.Zero, ownsHandle: true)
                {
                }

                public sealed override bool IsInvalid
                {
                    get { return handle == IntPtr.Zero; }
                }

                protected sealed override bool ReleaseHandle()
                {
                    bool success = Interop.Libcapi20.CryptMsgClose(handle);
                    SetHandle(IntPtr.Zero);
                    return success;
                }
            }

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CryptMsgClose(IntPtr hCryptMsg);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern SafeCryptMsgHandle CryptMsgOpenToDecode(
                MsgEncodingType dwMsgEncodingType,
                int dwFlags,
                int dwMsgType,
                IntPtr hCryptProv,
                IntPtr pRecipientInfo,
                IntPtr pStreamInfo);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CryptMsgUpdate(SafeCryptMsgHandle hCryptMsg, [In] byte[] pbData, int cbData, bool fFinal);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CryptMsgUpdate(SafeCryptMsgHandle hCryptMsg, IntPtr pbData, int cbData, bool fFinal);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            internal static extern bool CryptMsgUpdate(SafeCryptMsgHandle hCryptMsg, ref byte pbData, int cbData, bool fFinal);

            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, SetLastError = true)]
            public static extern SafeCertContextHandle CertCreateCertificateContext(
                CertEncodingType dwCertEncodingType,
                byte[] pbCertEncoded,
                int cbCertEncoded);

            // [DllImport(Libraries.Crypt32, CharSet = CharSet.Unicode, SetLastError = true)]
            // internal static extern unsafe SafeCertContextHandle CertCreateCertificateContext(MsgEncodingType dwCertEncodingType, void* pbCertEncoded, int cbCertEncoded);
        }
    }
}
