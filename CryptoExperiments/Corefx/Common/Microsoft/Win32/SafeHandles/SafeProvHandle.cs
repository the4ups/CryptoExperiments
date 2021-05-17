namespace CryptoExperiments.Corefx.Common.Microsoft.Win32.SafeHandles
{
    using global::Microsoft.Win32.SafeHandles;
    using global::System;
    using global::System.Diagnostics;
    using Interop = CryptoExperiments.Interop;

    /// <summary>
    /// Safehandle representing HCRYPTPROV
    /// </summary>
    internal sealed class SafeProvHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private string _containerName;
        private string _providerName;
        private int _type;
        private uint _flags;
        private bool _fPersistKeyInCsp;

        // begin: gost
        /// <summary>
        /// Ñîçäàíèå áåçîïàñíîãî handle ïî IntPtr ñ âîçìîæíîñòüþ
        /// óâåëè÷åíèÿ êîëè÷åñòâà ññûëîê.
        /// </summary>
        /// <param name="handle">õåíäë â âèäå <c>IntPtr</c>.</param>
        /// <param name="addref"><see langword="true"/> äëÿ AddRef,
        /// <see langword="false"/> äëÿ âëàäåíèÿ.</param>
        ///
        /// <unmanagedperm action="LinkDemand" />
        internal SafeProvHandle(IntPtr handle, bool addref)
            : base(true)
        {
            // if (!addref)
            // {
            //     SetHandle(handle);
            //     return;
            // }
            //
            // bool ret = Interop.Advapi32.CryptContextAddRef(handle, null, 0);
            // int hr = Marshal.GetLastWin32Error();
            // if (ret)
            //     SetHandle(handle);
            // if (!ret)
            //     throw new CryptographicException(hr);
            //
            // // Âûñòàâëÿåì â true, òàê êàê íå õîòèì, ÷òîá êëþ÷ óáèëè ïðè îñâîáîæäåíèè õýãäëà â Dispose
            // // òàê êàê íà õýíä ìîãóò ïðèñóñòâîâàòü äðóãèå âíåøíèå ññûëêè
            // _fPersistKeyInCsp = true;
        }
        // end: gost

        private SafeProvHandle() : base(true)
        {
            this.SetHandle(IntPtr.Zero);
            this._containerName = null;
            this._providerName = null;
            this._type = 0;
            this._flags = 0;
            this._fPersistKeyInCsp = true;
        }

        internal string ContainerName
        {
            get
            {
                return this._containerName;
            }
            set
            {
                this._containerName = value;
            }
        }

        internal string ProviderName
        {
            get
            {
                return this._providerName;
            }
            set
            {
                this._providerName = value;
            }
        }

        internal int Types
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }

        internal uint Flags
        {
            get
            {
                return this._flags;
            }
            set
            {
                this._flags = value;
            }
        }

        internal bool PersistKeyInCsp
        {
            get
            {
                return this._fPersistKeyInCsp;
            }
            set
            {
                this._fPersistKeyInCsp = value;
            }
        }

        internal static SafeProvHandle InvalidHandle
        {
             get { return SafeHandleCache<SafeProvHandle>.GetInvalidHandle(() => new SafeProvHandle()); }
        }

        protected override void Dispose(bool disposing)
        {
            if (!SafeHandleCache<SafeProvHandle>.IsCachedInvalidHandle(this))
            {
                base.Dispose(disposing);
            }
        }

        protected override bool ReleaseHandle()
        {
            // Make sure not to delete a key that we want to keep in the key container or an ephemeral key
            if (!this._fPersistKeyInCsp && 0 == (this._flags & (uint)Interop.Libcapi20.CryptAcquireContextFlags.CRYPT_VERIFYCONTEXT))
            {
                // Delete the key container.

                uint flags = (this._flags & (uint)Interop.Libcapi20.CryptAcquireContextFlags.CRYPT_MACHINE_KEYSET) | (uint)Interop.Libcapi20.CryptAcquireContextFlags.CRYPT_DELETEKEYSET;
                SafeProvHandle hIgnoredProv;
                bool ignoredSuccess = Interop.Libcapi20.CryptAcquireContext(out hIgnoredProv, this._containerName, this._providerName, this._type, flags);
                hIgnoredProv.Dispose();
                // Ignoring success result code as CryptAcquireContext is being called to delete a key container rather than acquire a context.
                // If it fails, we can't do anything about it anyway as we're in a dispose method.
            }

            bool successfullyFreed = Interop.Libcapi20.CryptReleaseContext(this.handle, 0);
            Debug.Assert(successfullyFreed);

            this.SetHandle(IntPtr.Zero);
            return successfullyFreed;
        }
    }
}
