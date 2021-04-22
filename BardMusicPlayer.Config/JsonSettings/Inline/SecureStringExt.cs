using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System.Security
{
    public static class SecureStringExt
    {
        public static SecureString EmptyString { get; } = new SecureString();
        static SecureStringExt()
        {
            EmptyString.MakeReadOnly();
        }

        public static SecureString ToSecureString(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return EmptyString;

            SecureString output = new SecureString();
            foreach (char c in str.ToCharArray(0, str.Length))
                output.AppendChar(c);
            output.MakeReadOnly();
            return output;
        }

        public static string ToRawString(this SecureString sstr)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
#if NETCORE
                valuePtr = SecureStringMarshal.SecureStringToGlobalAllocUnicode(sstr);
#else
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(sstr);
#endif
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}