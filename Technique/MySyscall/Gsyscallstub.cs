using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MySyscall
{
    class Gsyscallstub
    {
        public static IntPtr NtdllBaseADDR;
        public static byte[] SyscallTemplate(byte syscallstub)
        {
            byte[] template =
            {
            0x4c, 0x8b, 0xd1,               // mov r10,rcx
            0xb8, 0x00, 0x00, 0x00, 0x00,   // mov eax,4
            0x0F, 0x05,                     // syscall
            0xC3                            // ret
            };
            
            template[4] = syscallstub;
            return template;
        }

        public static byte GetSysCallStub(string FuncName)
        {
            
            IntPtr BaseAddress = NtdllBaseADDR;
            IntPtr SyscallAddress = Dinvoke.GetExpAddress(BaseAddress, FuncName);
            byte stub = Marshal.ReadByte(SyscallAddress, 4);
            return stub;
        }

        public static byte GetSysCallStubByHASH(string HASH) 
        {
            
            IntPtr BaseAddress = NtdllBaseADDR;
            IntPtr SyscallAddress = Dinvoke.GetExpAddressByHASH(BaseAddress, HASH);
            byte stub = Marshal.ReadByte(SyscallAddress, 4);
            return stub;

        }
        public static void Set_NtdllBaseADDR() 
        {
            IntPtr BaseAddress = Dinvoke.GetModuleAddress(@"ntdll.dll");
            NtdllBaseADDR = BaseAddress;
        }




    }    
}
