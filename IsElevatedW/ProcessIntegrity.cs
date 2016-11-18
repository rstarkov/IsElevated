using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IsElevated
{
    static class ProcessIntegrity
    {
        public static Level GetCurrentProcessIntegrity()
        {
            // https://msdn.microsoft.com/en-us/library/bb625966.aspx
            var hProcess = Process.GetCurrentProcess().Handle;
            var hToken = IntPtr.Zero;
            var pTIL = IntPtr.Zero;
            try
            {
                if (!OpenProcessToken(hProcess, 0x0008 /* TOKEN_QUERY */, out hToken))
                    throw new Win32Exception();
                uint dwLengthNeeded;
                GetTokenInformation(hToken, 25 /* TokenIntegrityLevel */, IntPtr.Zero, 0, out dwLengthNeeded);
                pTIL = Marshal.AllocHGlobal((int) dwLengthNeeded);
                if (!GetTokenInformation(hToken, 25 /* TokenIntegrityLevel */, pTIL, dwLengthNeeded, out dwLengthNeeded))
                    throw new Win32Exception();
                var TIL = Marshal.PtrToStructure<TOKEN_MANDATORY_LABEL>(pTIL);
                var pCount = GetSidSubAuthorityCount(TIL.Label.Sid);
                var count = Marshal.ReadInt16(pCount);
                var pAuth = GetSidSubAuthority(TIL.Label.Sid, (uint) (count - 1));
                var auth = Marshal.ReadInt32(pAuth);
                if (auth >= SECURITY_MANDATORY_SYSTEM_RID)
                    return Level.System;
                else if (auth >= SECURITY_MANDATORY_HIGH_RID)
                    return Level.High;
                else if (auth >= SECURITY_MANDATORY_MEDIUM_RID)
                    return Level.Medium;
                else if (auth >= SECURITY_MANDATORY_LOW_RID)
                    return Level.Low;
                else
                    throw new InvalidOperationException($"Unexpected security level: {auth}");
            }
            finally
            {
                if (hToken != IntPtr.Zero)
                    CloseHandle(hToken);
                if (pTIL != IntPtr.Zero)
                    Marshal.FreeHGlobal(pTIL);
            }
        }

        public enum Level
        {
            System, High, Medium, Low
        }

        const int SECURITY_MANDATORY_LOW_RID = 0x00001000;
        const int SECURITY_MANDATORY_MEDIUM_RID = 0x00002000;
        const int SECURITY_MANDATORY_HIGH_RID = 0x00003000;
        const int SECURITY_MANDATORY_SYSTEM_RID = 0x00004000;

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetTokenInformation(
            IntPtr TokenHandle,
            int TokenInformationClass,
            IntPtr TokenInformation,
            uint TokenInformationLength,
            out uint ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle,
            UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern IntPtr GetSidSubAuthority(IntPtr sid, UInt32 subAuthorityIndex);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern IntPtr GetSidSubAuthorityCount(IntPtr psid);

        [StructLayout(LayoutKind.Sequential)]
        struct TOKEN_MANDATORY_LABEL
        {
            public SID_AND_ATTRIBUTES Label;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public uint Attributes;
        }
    }
}
