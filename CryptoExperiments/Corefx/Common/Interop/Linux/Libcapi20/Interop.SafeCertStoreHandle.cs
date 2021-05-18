namespace CryptoExperiments
{
    using global::System;
    using global::System.Runtime.InteropServices;

    internal static partial class Interop
    {
        /// <summary>
        /// A safe handle for certificate stores.
        /// </summary>
        public class SafeCertStoreHandle : SafeHandle
        {
            /// <summary>
            /// A handle that may be used in place of <see cref="IntPtr.Zero"/>.
            /// </summary>
            public static readonly SafeCertStoreHandle Null = new SafeCertStoreHandle();

            /// <summary>
            /// Initializes a new instance of the <see cref="SafeCertStoreHandle"/> class.
            /// </summary>
            public SafeCertStoreHandle()
                : base(IntPtr.Zero, true)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SafeCertStoreHandle"/> class.
            /// </summary>
            /// <param name="preexistingHandle">An object that represents the pre-existing handle to use.</param>
            /// <param name="ownsHandle">
            ///     <see langword="true" /> to have the native handle released when this safe handle is disposed or finalized;
            ///     <see langword="false" /> otherwise.
            /// </param>
            public SafeCertStoreHandle(IntPtr preexistingHandle, bool ownsHandle = true)
                : base(IntPtr.Zero, ownsHandle)
            {
                this.SetHandle(preexistingHandle);
            }

            /// <inheritdoc />
            public override bool IsInvalid => this.handle == IntPtr.Zero;

            /// <inheritdoc />
            protected override bool ReleaseHandle()
            {
                return Libcapi20.CertCloseStore(this.handle);
            }
        }
    }
}
