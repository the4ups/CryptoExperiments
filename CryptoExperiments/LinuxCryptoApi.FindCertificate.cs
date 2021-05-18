namespace CryptoExperiments
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using CryptoExperiments.Corefx.Common.Interop.Linux;

    public partial class LinuxCryptoApi
    {
        // /// <summary>
        // /// Поиск сертификата (первого удовлетворяющего критериям поиска)
        // /// </summary>
        // /// <param name="filter"></param>
        // /// <param name="storeLocation"></param>
        // /// <param name="storeName"></param>
        // /// <param name="findType"></param>
        // /// <param name="verify"></param>
        // /// <returns></returns>
        // public X509Certificate2 FindCertificate(
        //     string filter,
        //     StoreLocation storeLocation = StoreLocation.CurrentUser,
        //     StoreName storeName = StoreName.My,
        //     X509FindType findType = X509FindType.FindByThumbprint,
        //     bool verify = false)
        // {
        //     // _pCert = null;
        //     // IntPtr hCert = IntPtr.Zero;
        //     // GCHandle hInternal = new GCHandle();
        //     // GCHandle hFull = new GCHandle();
        //     // IntPtr hSysStore = IntPtr.Zero;
        //     //
        //     // try
        //     // {
        //     // 0) Открываем хранилище
        //     var certStore = Interop.Libcapi20.CertOpenStore(
        //         Interop.CertStoreProvider.CERT_STORE_PROV_MEMORY,
        //         Interop.CertEncodingType.All,
        //         IntPtr.Zero,
        //         Interop.CertStoreFlags.CERT_STORE_ENUM_ARCHIVED_FLAG |
        //         Interop.CertStoreFlags.CERT_STORE_CREATE_NEW_FLAG,
        //         null);
        //
        //     if (certStore.IsInvalid)
        //     {
        //         throw Marshal.GetHRForLastWin32Error().ToCryptographicException();
        //     }
        //
        //     // 1) Формируем данные в пакете
        //     if (findType is X509FindType.FindByThumbprint or X509FindType.FindBySerialNumber)
        //     {
        //         byte[] arData = Convert.FromHexString(filter);
        //         Interop.CRYPTOAPI_BLOB blob = new Interop.CRYPTOAPI_BLOB(arData.Length, arData);
        //         Interop.CRYPTOAPI_BLOB cryptBlob;
        //         cryptBlob.cbData = arData.Length;
        //         hInternal = GCHandle.Alloc(arData, GCHandleType.Pinned);
        //         cryptBlob.pbData = hInternal.AddrOfPinnedObject();
        //         hFull = GCHandle.Alloc(cryptBlob, GCHandleType.Pinned);
        //     }
        //         else
        //         {
        //             byte[] arData;
        //             if (fIsLinux)
        //             {
        //                 arData = Encoding.UTF8.GetBytes(_pFindValue);
        //             }
        //             else
        //             {
        //                 arData = Encoding.Unicode.GetBytes(_pFindValue);
        //             }
        //
        //             hFull = GCHandle.Alloc(arData, GCHandleType.Pinned);
        //         }
        //
        //         // 2) Получаем
        //         IntPtr hPrev = IntPtr.Zero;
        //         do
        //         {
        //             hCert = UCryptoAPI.CertFindCertificateInStore(
        //                 hSysStore,
        //                 UCConsts.PKCS_7_OR_X509_ASN_ENCODING,
        //                 0,
        //                 UCConsts.AR_CRYPT_FIND_TYPE[(int)_pFindType, fIsLinux.ToByte()],
        //                 hFull.AddrOfPinnedObject(),
        //                 hPrev);
        //             // 2.1) Освобождаем предыдущий
        //             if (hPrev != IntPtr.Zero)
        //             {
        //                 UCryptoAPI.CertFreeCertificateContext(hPrev);
        //             }
        //
        //             // 2.2) Кончились в списке
        //             if (hCert == IntPtr.Zero)
        //             {
        //                 return UConsts.E_NO_CERTIFICATE;
        //             }
        //
        //             // 2.3) Нашли и валиден
        //             X509Certificate2 pCert = new ISDP_X509Cert(hCert);
        //             if (!_fVerify || pCert.ISDPVerify())
        //             {
        //                 hCert = IntPtr.Zero;
        //                 _pCert = pCert;
        //                 return UConsts.S_OK;
        //             }
        //
        //             hPrev = hCert;
        //             // Чтобы не очистило
        //             hCert = IntPtr.Zero;
        //         } while (hCert != IntPtr.Zero);
        //
        //         return UConsts.E_NO_CERTIFICATE;
        //     }
        //     catch (Exception E)
        //     {
        //         _sError = UCConsts.S_FIND_CERT_GEN_ERR.Frm(E.Message);
        //         return UConsts.E_GEN_EXCEPTION;
        //     }
        //     finally
        //     {
        //         // Очищаем ссылки и закрываем хранилище
        //         if (hInternal.IsAllocated)
        //         {
        //             hInternal.Free();
        //         }
        //
        //         if (hFull.IsAllocated)
        //         {
        //             hFull.Free();
        //         }
        //
        //         if (hCert != IntPtr.Zero)
        //         {
        //             UCryptoAPI.CertFreeCertificateContext(hCert);
        //         }
        //
        //         UCryptoAPI.CertCloseStore(hSysStore, 0);
        //     }
        //
        //     return null;
        // }

        public unsafe void FindByThumbprint(byte[] thumbPrint)
        {
            fixed (byte* pThumbPrint = thumbPrint)
            {
                Interop.CRYPTOAPI_BLOB blob = new Interop.CRYPTOAPI_BLOB(thumbPrint.Length, pThumbPrint);
                FindCore<object>(Interop.CertFindType.CERT_FIND_HASH, &blob);
            }
        }

        private unsafe void FindCore<TState>(TState state, Func<TState, Interop.SafeCertContextHandle, bool> filter)
        {
            FindCore(Interop.CertFindType.CERT_FIND_ANY, null, state, filter);
        }

        private unsafe void FindCore<TState>(
            Interop.CertFindType dwFindType,
            void* pvFindPara,
            TState state = default!,
            Func<TState, Interop.SafeCertContextHandle, bool>? filter = null)
        {
            Interop.SafeCertStoreHandle findResults = Interop.Libcapi20.CertOpenStore(
                Interop.CertStoreProvider.CERT_STORE_PROV_MEMORY,
                Interop.CertEncodingType.All,
                IntPtr.Zero,
                Interop.CertStoreFlags.CERT_STORE_ENUM_ARCHIVED_FLAG | Interop.CertStoreFlags.CERT_STORE_CREATE_NEW_FLAG,
                null);
            if (findResults.IsInvalid)
                throw Marshal.GetHRForLastWin32Error().ToCryptographicException();

            Interop.SafeCertContextHandle? pCertContext = null;
            while (Interop.Libcapi20.CertFindCertificateInStore(_storePal.SafeCertStoreHandle, dwFindType, pvFindPara, ref pCertContext))
            {
                if (filter != null && !filter(state, pCertContext))
                    continue;

                // if (_validOnly)
                // {
                //     if (!VerifyCertificateIgnoringErrors(pCertContext))
                //         continue;
                // }
                //
                // if (!Interop.crypt32.CertAddCertificateLinkToStore(findResults, pCertContext, CertStoreAddDisposition.CERT_STORE_ADD_ALWAYS, IntPtr.Zero))
                //     throw Marshal.GetLastWin32Error().ToCryptographicException();
            }
            //
            // using (StorePal resultsStore = new StorePal(findResults))
            // {
            //     resultsStore.CopyTo(_copyTo);
            // }
        }
    }
}
