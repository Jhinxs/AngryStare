using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace MySyscall
{
    class NativeAPI
    {

        public static WinNativeshit.Native.NTSTATUS MYNAVM(
           IntPtr ProcessHandle,
           ref IntPtr BaseAddress,
           IntPtr ZeroBits,
           ref UIntPtr RegionSize,
           uint AllocationType,
           uint Protect)
        {
            //e77e4cf7 NtAllocateVirtualMemory
            byte bMYNAVM = Gsyscallstub.GetSysCallStubByHASH (@"e77e4cf7");
            //byte bMYNAVM = Gsyscallstub.GetSysCallStubByHASH(@"NtAllocateVirtualMemory");
            byte[] bMYNAVMs = Gsyscallstub.SyscallTemplate(bMYNAVM);

            unsafe
            {
                
                fixed (byte* ptr = bMYNAVMs)
                {
                    
                    IntPtr memoryAddress = (IntPtr)ptr;
                    uint lpflOldProtect;
                    uint lpflOldProtect2;
                    if (!MYVirtualProtect(memoryAddress, (UIntPtr)bMYNAVMs.Length, (uint)WinNativeshit.Native.AllocationProtect.PAGE_READWRITE, out lpflOldProtect))
                    {
                        Environment.Exit(0);
                    }
                   
                    Delegates.MYNAVM assembledFunction = (Delegates.MYNAVM)Marshal.GetDelegateForFunctionPointer(memoryAddress, typeof(Delegates.MYNAVM));

                    if (!MYVirtualProtect(memoryAddress, (UIntPtr)bMYNAVMs.Length, (uint)WinNativeshit.Native.AllocationProtect.PAGE_EXECUTE, out lpflOldProtect2))
                    {
                        Environment.Exit(0);
                    }
                    return (WinNativeshit.Native.NTSTATUS)assembledFunction(
                        ProcessHandle,
                        ref BaseAddress,
                        ZeroBits,
                        ref RegionSize,
                        AllocationType,
                        Protect);
                }
            }
        }
        public static WinNativeshit.Native.NTSTATUS MYMCT(
            out IntPtr hThread,
            WinNativeshit.Native.ACCESS_MASK DesiredAccess,
            IntPtr ObjectAttributes,
            IntPtr ProcessHandle,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            bool CreateSuspended,
            uint StackZeroBits,
            uint SizeOfStackCommit,
            uint SizeOfStackReserve,
            IntPtr lpBytesBuffer
            )
        {
            //NtCreateThreadEx 9699b669
            byte bMYMCT = Gsyscallstub.GetSysCallStubByHASH(@"9699b669");
            //byte bMYMCT = Gsyscallstub.GetSysCallStubByHASH(@"NtCreateThreadEx");
            byte[] bMYMCTs = Gsyscallstub.SyscallTemplate(bMYMCT);
                    
            unsafe
            {
              
                fixed (byte* ptr = bMYMCTs)
                {

                    IntPtr memoryAddress = (IntPtr)ptr;
                    uint lpflOldProtect;
                    uint lpflOldProtect2;
                    if (!MYVirtualProtect(memoryAddress, (UIntPtr)bMYMCTs.Length, (uint)WinNativeshit.Native.AllocationProtect.PAGE_READWRITE, out lpflOldProtect))
                    {
                        Environment.Exit(0);
                    }

                    Delegates.MYMCT assembledFunction = (Delegates.MYMCT)Marshal.GetDelegateForFunctionPointer(memoryAddress, typeof(Delegates.MYMCT));
                    if (!MYVirtualProtect(memoryAddress, (UIntPtr)bMYMCTs.Length, (uint)WinNativeshit.Native.AllocationProtect.PAGE_EXECUTE, out lpflOldProtect2))
                    {
                        Environment.Exit(0);
                    }


                    return (WinNativeshit.Native.NTSTATUS)assembledFunction(
                        out hThread,
                        DesiredAccess,
                        ObjectAttributes,
                        ProcessHandle,
                        lpStartAddress,
                        lpParameter,
                        CreateSuspended,
                        StackZeroBits,
                        SizeOfStackCommit,
                        SizeOfStackReserve,
                        lpBytesBuffer
                        );
                }
            }
        }
        public static WinNativeshit.Native.NTSTATUS MYWFSO(IntPtr Object, bool Alertable, uint Timeout)
        {
            //NtWaitForSingleObject f26894a5
            byte bMYWFSO = Gsyscallstub.GetSysCallStubByHASH(@"f26894a5");
            //byte bMYWFSO = Gsyscallstub.GetSysCallStubByHASH(@"NtWaitForSingleObject");
            byte[] bMYWFSOs = Gsyscallstub.SyscallTemplate(bMYWFSO);

            unsafe
            {
                
                fixed (byte* ptr = bMYWFSOs)
                {
                    
                    IntPtr memoryAddress = (IntPtr)ptr;
                    uint lpflOldProtect;
                    uint lpflOldProtect2;
                    if (!MYVirtualProtect( memoryAddress, (UIntPtr)bMYWFSOs.Length, (uint)WinNativeshit.Native.AllocationProtect.PAGE_READWRITE, out lpflOldProtect))
                    {
                        Environment.Exit(0);
                    }

                    Delegates.MYWFSO assembledFunction = (Delegates.MYWFSO)Marshal.GetDelegateForFunctionPointer(memoryAddress, typeof(Delegates.MYWFSO));
                    if (!MYVirtualProtect(memoryAddress, (UIntPtr)bMYWFSOs.Length, (uint)WinNativeshit.Native.AllocationProtect.PAGE_EXECUTE, out lpflOldProtect2))
                    {
                        Environment.Exit(0);
                    }

                    return (WinNativeshit.Native.NTSTATUS)assembledFunction(Object, Alertable, Timeout);
                }
            }
        }
        public static bool MYVirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect) 
        {
            IntPtr vppointer = Dinvoke.GetExpAddressByHASH(Dinvoke.GetModuleAddress(@"kernel32.dll"), @"ad9f9cd0");
            NativeAPI.Delegates.MYVirtualProtect MYVirtualProtect = (NativeAPI.Delegates.MYVirtualProtect)Marshal.GetDelegateForFunctionPointer(vppointer, typeof(NativeAPI.Delegates.MYVirtualProtect));
            return MYVirtualProtect(lpAddress, dwSize, flNewProtect, out lpflOldProtect);
        }
        public struct Delegates
        {
            
            //[UnmanagedFunctionPointer(CallingConvention.StdCall)]
            //public delegate WinNativeshit.Native.NTSTATUS NtCreateFile(
            //      out Microsoft.Win32.SafeHandles.SafeFileHandle FileHandle,
            //      WinNativeshit.Native.FileAccess DesiredAcces,
            //      ref WinNativeshit.Native.OBJECT_ATTRIBUTES ObjectAttributes,
            //      ref WinNativeshit.Native.IO_STATUS_BLOCK IoStatusBlock,
            //      ref long AllocationSize,
            //      WinNativeshit.Native.FileAttributes fileAttributes,
            //      WinNativeshit.Native.FileShare share,
            //      WinNativeshit.Native.CreationDisposition createDisposition,
            //      WinNativeshit.Native.CreateOption createOptions,
            //      IntPtr EaBuffer,
            //      uint EaLength
            //);
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate WinNativeshit.Native.NTSTATUS MYNAVM(
                IntPtr ProcessHandle,
                ref IntPtr BaseAddress,
                IntPtr ZeroBits,
                ref UIntPtr RegionSize,
                ulong AllocationType,
                ulong Protect);
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate WinNativeshit.Native.NTSTATUS MYMCT(
                out IntPtr hThread,
               WinNativeshit.Native.ACCESS_MASK DesiredAccess,
                IntPtr ObjectAttributes,
                IntPtr ProcessHandle,
                IntPtr lpStartAddress,
                IntPtr lpParameter,
                bool CreateSuspended,
                uint StackZeroBits,
                uint SizeOfStackCommit,
                uint SizeOfStackReserve,
                IntPtr lpBytesBuffer
                );
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate WinNativeshit.Native.NTSTATUS MYWFSO(IntPtr Object, bool Alertable, uint Timeout);
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate bool MYVirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        }
     
       
    }
}
