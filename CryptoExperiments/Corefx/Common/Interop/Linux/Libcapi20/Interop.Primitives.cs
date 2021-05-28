namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;

    internal partial class Interop
    {
        internal enum CertEncodingType : int
        {
            PKCS_7_ASN_ENCODING = 0x00010000,
            X509_ASN_ENCODING   = 0x00000001,

            All = PKCS_7_ASN_ENCODING | X509_ASN_ENCODING,
        }

        internal enum CertStoreProvider : int
        {
            CERT_STORE_PROV_MEMORY = 2,
            CERT_STORE_PROV_SYSTEM_A = 9,
            CERT_STORE_PROV_SYSTEM_W = 10,
        }

        [Flags]
        internal enum CertStoreFlags : int
        {
            CERT_STORE_NO_CRYPT_RELEASE_FLAG                = 0x00000001,
            CERT_STORE_SET_LOCALIZED_NAME_FLAG              = 0x00000002,
            CERT_STORE_DEFER_CLOSE_UNTIL_LAST_FREE_FLAG     = 0x00000004,
            CERT_STORE_DELETE_FLAG                          = 0x00000010,
            CERT_STORE_UNSAFE_PHYSICAL_FLAG                 = 0x00000020,
            CERT_STORE_SHARE_STORE_FLAG                     = 0x00000040,
            CERT_STORE_SHARE_CONTEXT_FLAG                   = 0x00000080,
            CERT_STORE_MANIFOLD_FLAG                        = 0x00000100,
            CERT_STORE_ENUM_ARCHIVED_FLAG                   = 0x00000200,
            CERT_STORE_UPDATE_KEYID_FLAG                    = 0x00000400,
            CERT_STORE_BACKUP_RESTORE_FLAG                  = 0x00000800,
            CERT_STORE_READONLY_FLAG                        = 0x00008000,
            CERT_STORE_OPEN_EXISTING_FLAG                   = 0x00004000,
            CERT_STORE_CREATE_NEW_FLAG                      = 0x00002000,
            CERT_STORE_MAXIMUM_ALLOWED_FLAG                 = 0x00001000,

            CERT_SYSTEM_STORE_CURRENT_USER                  = 0x00010000,
            CERT_SYSTEM_STORE_LOCAL_MACHINE                 = 0x00020000,

            None                                            = 0x00000000,
        }

        [Flags]
        internal enum CertFindFlags : int
        {
            None = 0x00000000,
        }

        internal enum CertFindType : int
        {
            CERT_FIND_SUBJECT_CERT = 0x000b0000,
            CERT_FIND_HASH         = 0x00010000,
            CERT_FIND_SUBJECT_STR  = 0x00080007,
            CERT_FIND_ISSUER_STR   = 0x00080004,
            CERT_FIND_EXISTING     = 0x000d0000,
            CERT_FIND_ANY          = 0x00000000,
        }

        internal enum CertContextPropId : int
        {
            CERT_KEY_PROV_INFO_PROP_ID   = 2,
            CERT_SHA1_HASH_PROP_ID       = 3,
            CERT_KEY_CONTEXT_PROP_ID     = 5,
            CERT_FRIENDLY_NAME_PROP_ID   = 11,
            CERT_ARCHIVED_PROP_ID        = 19,
            CERT_KEY_IDENTIFIER_PROP_ID  = 20,
            CERT_PUBKEY_ALG_PARA_PROP_ID = 22,
            CERT_NCRYPT_KEY_HANDLE_PROP_ID = 78,
            CERT_CLR_DELETE_KEY_PROP_ID = 125,
        }

        // CRYPTOAPI_BLOB has many typedef aliases in the C++ world (CERT_BLOB, DATA_BLOB, etc.) We'll just stick to one name here.
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CRYPTOAPI_BLOB
        {
            public CRYPTOAPI_BLOB(int cbData, byte* pbData)
            {
                this.cbData = cbData;
                this.pbData = pbData;
            }

            public int cbData;
            public byte* pbData;

            public byte[] ToByteArray()
            {
                if (cbData == 0)
                {
                    return Array.Empty<byte>();
                }

                byte[] array = new byte[cbData];
                Marshal.Copy((IntPtr)pbData, array, 0, cbData);
                return array;
            }
        }

    }
}
