namespace CryptoExperiments
{
    using System.Runtime.InteropServices;
    using CryptoExperiments.Corefx.Common.Interop.Linux;

    internal static class MyClass
    {
        internal static byte[] GetMsgParamAsByteArray(this Interop.Libcapi20.SafeCryptMsgHandle hCryptMsg, Interop.Libcapi20.CryptMsgParamType paramType, int index = 0)
        {
            int cbData = 0;
            if (!Interop.Libcapi20.CryptMsgGetParam(hCryptMsg, paramType, index, null, ref cbData))
                throw Marshal.GetLastWin32Error().ToCryptographicException();

            byte[] pvData = new byte[cbData];
            if (!Interop.Libcapi20.CryptMsgGetParam(hCryptMsg, paramType, index, pvData, ref cbData))
                throw Marshal.GetLastWin32Error().ToCryptographicException();

            return pvData;//.Resize(cbData);
        }
    }
}
