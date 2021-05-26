namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            internal enum ECC_CURVE_ALG_ID_ENUM : int
            {
                BCRYPT_NO_CURVE_GENERATION_ALG_ID = 0x0,
            }

            /// <summary>
            /// Version used for a buffer containing a scalar integer (not an IntPtr)
            /// </summary>
            [DllImport(Corefx.Common.Interop.Linux.Interop.Libraries.Libcapi20, CharSet = CharSet.Ansi)]
            public static extern IntPtr CryptFindOIDInfo(CryptOidInfoKeyType dwKeyType, ref int pvKey, OidGroup group);

            public static CRYPT_OID_INFO FindAlgIdOidInfo(ECC_CURVE_ALG_ID_ENUM algId)
            {
                int intAlgId = (int)algId;
                IntPtr fullOidInfo = CryptFindOIDInfo(
                    CryptOidInfoKeyType.CRYPT_OID_INFO_ALGID_KEY,
                    ref intAlgId,
                    OidGroup.HashAlgorithm);

                if (fullOidInfo != IntPtr.Zero)
                {
                    return Marshal.PtrToStructure<CRYPT_OID_INFO>(fullOidInfo);
                }

                // Otherwise the lookup failed.
                return new CRYPT_OID_INFO() { AlgId = -1 };
            }
        }
    }
}
