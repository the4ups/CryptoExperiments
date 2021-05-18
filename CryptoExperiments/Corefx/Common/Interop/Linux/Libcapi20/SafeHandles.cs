namespace CryptoExperiments
{
    using global::Microsoft.Win32.SafeHandles;
    using global::System;
    using global::System.Diagnostics;
    using global::System.Runtime.InteropServices;

    internal partial class Interop
    {
        /// <summary>
        /// Base class for safe handles representing NULL-based pointers.
        /// </summary>
        internal abstract class SafePointerHandle<T> : SafeHandle where T : SafeHandle, new()
        {
            protected SafePointerHandle()
                : base(IntPtr.Zero, true)
            {
            }

            public sealed override bool IsInvalid
            {
                get { return handle == IntPtr.Zero; }
            }

            public static T InvalidHandle
            {
                get { return SafeHandleCache<T>.GetInvalidHandle(() => new T()); }
            }

            protected override void Dispose(bool disposing)
            {
                if (!SafeHandleCache<T>.IsCachedInvalidHandle(this))
                {
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// SafeHandle for the CERT_CONTEXT structure defined by crypt32.
        /// </summary>
        internal class SafeCertContextHandle : SafePointerHandle<SafeCertContextHandle>
        {
            private SafeCertContextHandle? _parent;

            public SafeCertContextHandle()
            {
            }

            public SafeCertContextHandle(SafeCertContextHandle parent)
            {
                if (parent == null)
                    throw new ArgumentNullException(nameof(parent));

                Debug.Assert(!parent.IsInvalid);
                Debug.Assert(!parent.IsClosed);

                bool ignored = false;
                parent.DangerousAddRef(ref ignored);
                _parent = parent;

                SetHandle(_parent.handle);
            }

            internal new void SetHandle(IntPtr handle) => base.SetHandle(handle);

            protected override bool ReleaseHandle()
            {
                if (_parent != null)
                {
                    _parent.DangerousRelease();
                    _parent = null;
                }
                else
                {
                    Libcapi20.CertFreeCertificateContext(handle);
                }

                SetHandle(IntPtr.Zero);
                return true;
            }

            public unsafe Libcapi20.CERT_CONTEXT* CertContext
            {
                get { return (Libcapi20.CERT_CONTEXT*)handle; }
            }

            // Extract the raw CERT_CONTEXT* pointer and reset the SafeHandle to the invalid state so it no longer auto-destroys the CERT_CONTEXT.
            public unsafe Libcapi20.CERT_CONTEXT* Disconnect()
            {
                Libcapi20.CERT_CONTEXT* pCertContext = (Libcapi20.CERT_CONTEXT*)handle;
                SetHandle(IntPtr.Zero);
                return pCertContext;
            }

            public bool HasPersistedPrivateKey
            {
                get { return CertHasProperty(CertContextPropId.CERT_KEY_PROV_INFO_PROP_ID); }
            }

            public bool HasEphemeralPrivateKey
            {
                get { return CertHasProperty(CertContextPropId.CERT_KEY_CONTEXT_PROP_ID); }
            }

            public bool ContainsPrivateKey
            {
                get { return HasPersistedPrivateKey || HasEphemeralPrivateKey; }
            }

            public SafeCertContextHandle Duplicate()
            {
                return Libcapi20.CertDuplicateCertificateContext(handle);
            }

            private bool CertHasProperty(CertContextPropId propertyId)
            {
                int cb = 0;
                bool hasProperty = Libcapi20.CertGetCertificateContextProperty(
                    this,
                    propertyId,
                    null,
                    ref cb);

                return hasProperty;
            }
        }
    }
}
