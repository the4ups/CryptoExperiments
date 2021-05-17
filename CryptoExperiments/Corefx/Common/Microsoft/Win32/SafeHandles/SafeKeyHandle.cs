// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles
{
    using global::Microsoft.Win32.SafeHandles;
    using global::System;
    using global::System.Diagnostics;
    using Interop = CryptoExperiments.Interop;

    /// <summary>
    ///     Safe handle representing a HCRYPTKEY
    /// </summary>
    /// <summary>
    ///     Since we need to delete the key handle before the provider is released we need to actually hold a
    ///     pointer to a CRYPT_KEY_CTX unmanaged structure whose destructor decrements a refCount. Only when
    ///     the provider refCount is 0 it is deleted. This way, we loose a race in the critical finalization
    ///     of the key handle and provider handle. This also applies to hash handles, which point to a
    ///     CRYPT_HASH_CTX. Those structures are defined in COMCryptography.h
    /// </summary>
    internal sealed class SafeKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private int _keySpec;
        private bool _fPublicOnly;
        private SafeProvHandle? _parent;

        public SafeKeyHandle() : base(true)
        {
            this.SetHandle(IntPtr.Zero);
            this._keySpec = 0;
            this._fPublicOnly = false;
        }

        internal int KeySpec
        {
            get
            {
                return this._keySpec;
            }
            set
            {
                this._keySpec = value;
            }
        }

        internal bool PublicOnly
        {
            get
            {
                return this._fPublicOnly;
            }
            set
            {
                this._fPublicOnly = value;
            }
        }

        internal void SetParent(SafeProvHandle parent)
        {
            if (this.IsInvalid || this.IsClosed)
            {
                return;
            }

            Debug.Assert(this._parent == null);
            Debug.Assert(!parent.IsClosed);
            Debug.Assert(!parent.IsInvalid);

            this._parent = parent;

            bool ignored = false;
            this._parent.DangerousAddRef(ref ignored);
        }

        internal static SafeKeyHandle InvalidHandle
        {
            get { return SafeHandleCache<SafeKeyHandle>.GetInvalidHandle(() => new SafeKeyHandle()); }
        }

        protected override void Dispose(bool disposing)
        {
            if (!SafeHandleCache<SafeKeyHandle>.IsCachedInvalidHandle(this))
            {
                base.Dispose(disposing);
            }
        }

        protected override bool ReleaseHandle()
        {
            bool successfullyFreed = Interop.Libcapi20.CryptDestroyKey(this.handle);
            Debug.Assert(successfullyFreed);

            SafeProvHandle? parent = this._parent;
            this._parent = null;
            parent?.DangerousRelease();

            return successfullyFreed;
        }
    }
}