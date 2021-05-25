namespace CryptoExperiments
{
    using System.Runtime.InteropServices;

    internal static partial class Interop
    {
        internal partial class Libcapi20
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            internal struct PROV_ENUMALGS_EX
            {
                public uint aiAlgid; //ALG_ID
                public uint dwDefaultLen; //DWORD
                public uint dwMinLen; //DWORD
                public uint dwMaxLen; //DWORD
                public uint dwProtocols; //DWORD
                public uint dwNameLen; //DWORD
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
                public string szName; //WCHAR[20]
                public uint dwLongNameLen; //DWORD
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
                public string szLongName; //WCHAR[40]
            } //4 + 4 + 4 + 4 + 4 + 4 + 40 + 4 + 80 = 148
        }
    }
}
