namespace CryptoExperiments
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using CryptoExperiments.Corefx.Common.Interop.Linux;

    public partial class LinuxCryptoApi
    {
        public unsafe IList<IX509Certificate> FindBySubjectName(
            string subjectName,
            bool validOnly = false,
            StoreLocation storeLocation = StoreLocation.CurrentUser,
            StoreName storeName = StoreName.My)
        {
            fixed (char* pSubjectName = subjectName)
            {
                return this.FindCore<object>(
                    Interop.CertFindType.CERT_FIND_SUBJECT_STR,
                    pSubjectName,
                    validOnly,
                    storeLocation,
                    storeName);
            }
        }

        public unsafe IList<IX509Certificate> FindByIssuerName(
            string issuerName,
            bool validOnly = false,
            StoreLocation storeLocation = StoreLocation.CurrentUser,
            StoreName storeName = StoreName.My)
        {
            fixed (char* pIssuerName = issuerName)
            {
                return this.FindCore<object>(
                    Interop.CertFindType.CERT_FIND_ISSUER_STR,
                    pIssuerName,
                    validOnly,
                    storeLocation,
                    storeName);
            }
        }

        public unsafe IList<IX509Certificate> FindBySerialNumber(
            BigInteger hexValue,
            BigInteger decimalValue,
            bool validOnly = false,
            StoreLocation storeLocation = StoreLocation.CurrentUser,
            StoreName storeName = StoreName.My)
        {
            return this.FindCore(
                (hexValue, decimalValue),
                static(state, pCertContext) =>
                {
                    byte[] actual = ToByteArray(pCertContext.CertContext->pCertInfo->SerialNumber);
                    GC.KeepAlive(pCertContext);

                    // Convert to BigInteger as the comparison must not fail due to spurious leading zeros
                    var actualAsBigInteger = PositiveBigIntegerFromByteArray(actual);

                    var (hexValue, decimalValue) = state;
                    return hexValue.Equals(actualAsBigInteger) || decimalValue.Equals(actualAsBigInteger);
                },
                validOnly,
                storeLocation,
                storeName);
        }

        public unsafe IList<IX509Certificate> FindByThumbprint(
            byte[] thumbPrint,
            bool validOnly = false,
            StoreLocation storeLocation = StoreLocation.CurrentUser,
            StoreName storeName = StoreName.My)
        {
            fixed (byte* pThumbPrint = thumbPrint)
            {
                var blob = new Interop.CRYPTOAPI_BLOB(thumbPrint.Length, pThumbPrint);
                return this.FindCore<object>(
                    Interop.CertFindType.CERT_FIND_HASH,
                    &blob,
                    validOnly,
                    storeLocation,
                    storeName);
            }
        }

        private unsafe IList<IX509Certificate> FindCore<TState>(
            TState state,
            Func<TState, Interop.SafeCertContextHandle, bool> filter,
            bool validOnly,
            StoreLocation storeLocation,
            StoreName storeName)
        {
            return this.FindCore(
                Interop.CertFindType.CERT_FIND_ANY,
                null,
                validOnly,
                storeLocation,
                storeName,
                state,
                filter);
        }

        private unsafe IList<IX509Certificate> FindCore<TState>(
            Interop.CertFindType dwFindType,
            void* pvFindPara,
            bool validOnly,
            StoreLocation storeLocation,
            StoreName storeName,
            TState state = default!,
            Func<TState, Interop.SafeCertContextHandle, bool>? filter = null)
        {
            List<IX509Certificate> result = new();

            var certStore = storeLocation switch
            {
                StoreLocation.CurrentUser => Interop.CertStoreFlags.CERT_SYSTEM_STORE_CURRENT_USER,
                StoreLocation.LocalMachine => Interop.CertStoreFlags.CERT_SYSTEM_STORE_LOCAL_MACHINE,
                _ => throw new ArgumentOutOfRangeException(nameof(storeLocation), storeLocation, null),
            };

            Interop.SafeCertContextHandle? safeCertContextHandle = null;
            using (var store = CertOpenStore(storeName, certStore))
            {
                while (Interop.Libcapi20.CertFindCertificateInStore(store, dwFindType, pvFindPara, ref safeCertContextHandle))
                {
                    if (filter != null && !filter(state, safeCertContextHandle))
                    {
                        continue;
                    }

                    var pCertContext = *safeCertContextHandle.CertContext;
                    byte[] pbBlob = new byte[pCertContext.cbCertEncoded];
                    Marshal.Copy((IntPtr)pCertContext.pbCertEncoded, pbBlob, 0, pbBlob.Length);

                    var certificate = new X509Certificate2(
                        pbBlob,
                        new System.Security.SecureString(),
                        X509KeyStorageFlags.PersistKeySet);

                    if (validOnly)
                    {
                        if (!certificate.Verify())
                        {
                            continue;
                        }
                    }

                    var x509Certificate = new X509CertificateWrapper(safeCertContextHandle.Duplicate(), certificate);
                    result.Add(x509Certificate);
                }
            }

            return result;
        }

        private static Interop.SafeCertStoreHandle CertOpenStore(StoreName storeName, Interop.CertStoreFlags certStore)
        {
            var storeHandle = Interop.Libcapi20.CertOpenStore(
                Interop.CertStoreProvider.CERT_STORE_PROV_SYSTEM_A,
                Interop.CertEncodingType.All,
                IntPtr.Zero,
                certStore,
                storeName.ToString("G"));

            if (storeHandle.IsInvalid)
            {
                throw Marshal.GetLastWin32Error().ToCryptographicException();
            }

            return storeHandle;
        }

        private static BigInteger PositiveBigIntegerFromByteArray(byte[] bytes)
        {
            // To prevent the big integer from misinterpreted as a negative number,
            // add a "leading 0" to the byte array if it would considered negative.
            //
            // Since BigInteger(bytes[]) requires a little-endian byte array,
            // the "leading 0" actually goes at the end of the array.

            // An empty array is 0 (non-negative), so no over-allocation is required.
            //
            // If the last indexed value doesn't have the sign bit set (0x00-0x7F) then
            // the number would be positive anyways, so no over-allocation is required.
            if (bytes.Length == 0 || bytes[^1] < 0x80)
            {
                return new BigInteger(bytes);
            }

            // Since the sign bit is set, put a new 0x00 on the end to move that bit from
            // the sign bit to a data bit.
            byte[] newBytes = new byte[bytes.Length + 1];
            Buffer.BlockCopy(bytes, 0, newBytes, 0, bytes.Length);
            return new BigInteger(newBytes);
        }

        private static byte[] ToByteArray(Interop.Libcapi20.DATA_BLOB blob)
        {
            if (blob.cbData == 0)
            {
                return Array.Empty<byte>();
            }

            var length = (int)blob.cbData;
            byte[] data = new byte[length];
            Marshal.Copy(blob.pbData, data, 0, length);
            return data;
        }

        public interface IX509Certificate
        {
            IntPtr CertificateHandle { get; }

            bool ContainsPrivateKey { get; }

            X509Certificate2 X509Certificate2 { get; }
        }

        public class X509CertificateWrapper : IDisposable, IX509Certificate
        {
            private Interop.SafeCertContextHandle _handle;

            public X509Certificate2 X509Certificate2 { get; }

            internal X509CertificateWrapper(Interop.SafeCertContextHandle handle, X509Certificate2 x509Certificate2)
            {
                this._handle = handle ?? throw new ArgumentNullException(nameof(handle));
                this.X509Certificate2 = x509Certificate2 ?? throw new ArgumentNullException(nameof(x509Certificate2));
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _handle.Dispose();
                    X509Certificate2.Dispose();
                }
            }

            public IntPtr CertificateHandle => this._handle.DangerousGetHandle();

            public bool ContainsPrivateKey => this._handle.ContainsPrivateKey;
        }
    }
}
