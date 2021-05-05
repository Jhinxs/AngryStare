using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace MySyscall
{
    class Dinvoke
    {   
        /// <summary>
        /// get func pointer by name
        /// </summary>
        /// <param name="baseaddress"></param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public static IntPtr GetExpAddress(IntPtr baseaddress, string funcName)
        {
            
            IntPtr FuncPtr = IntPtr.Zero;
            Int64 DosHeader = baseaddress.ToInt64();
            Int32 NTHeader = Marshal.ReadInt32((IntPtr)(DosHeader + 0x3c));
            Int64 OptionalHeader = DosHeader + NTHeader + 0x18;
            Int16 Magic = Marshal.ReadInt16((IntPtr)OptionalHeader);
            //  Console.WriteLine(Magic);
            Int64 IMAGE_DIRECTORY_ENTRY_EXPORT = 0;
            if (Magic == 0x10b)
            {
                IMAGE_DIRECTORY_ENTRY_EXPORT = OptionalHeader + 0x60;

            }
            else
            {
                IMAGE_DIRECTORY_ENTRY_EXPORT = OptionalHeader + 0x70;
            }
            Int32 EXPORT_RVA = Marshal.ReadInt32((IntPtr)IMAGE_DIRECTORY_ENTRY_EXPORT);
            Int32 IMAGE_DIRECTORY_ENTRY_EXPORT_BASE = Marshal.ReadInt32((IntPtr)(DosHeader + EXPORT_RVA + 0x10));
            Int32 NumberOfNames = Marshal.ReadInt32((IntPtr)(DosHeader + EXPORT_RVA + 0x18));
            Int32 AddressofFunctions = Marshal.ReadInt32((IntPtr)(DosHeader + EXPORT_RVA + 0x1c));
            Int32 AddressOfNames = Marshal.ReadInt32((IntPtr)(DosHeader + EXPORT_RVA + 0x20));
            Int32 AddressOfNameOrdinals = Marshal.ReadInt32((IntPtr)(DosHeader + EXPORT_RVA + 0x24));

            for (int i = 0; i < NumberOfNames; i++)
            {

                string ExpFuncName = Marshal.PtrToStringAnsi((IntPtr)(DosHeader + Marshal.ReadInt32((IntPtr)(DosHeader + AddressOfNames + i * 4))));
                //Console.WriteLine(ExpFuncName);
                if (ExpFuncName.Equals(funcName, StringComparison.OrdinalIgnoreCase))
                {

                    Int32 expFuncNameOrdinal = Marshal.ReadInt16((IntPtr)(DosHeader + AddressOfNameOrdinals + i * 2)) + IMAGE_DIRECTORY_ENTRY_EXPORT_BASE;
                    Int32 expFuncNameRVA = Marshal.ReadInt32((IntPtr)(DosHeader + AddressofFunctions + (4 * (expFuncNameOrdinal - IMAGE_DIRECTORY_ENTRY_EXPORT_BASE))));
                    FuncPtr = (IntPtr)((Int64)DosHeader + expFuncNameRVA);
                  //  Console.WriteLine(ExpFuncName + ":" + FuncPtr);
                    break;
                }
            }
            return FuncPtr;
        }
        /// <summary>
        /// get func pointer by apihash
        /// </summary>
        /// <param name="baseaddress"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static IntPtr GetExpAddressByHASH(IntPtr baseaddress, string hash)
        {

            IntPtr FuncPtr = IntPtr.Zero;
            Int64 DosHeader = baseaddress.ToInt64();
            Int32 NTHeader = Marshal.ReadInt32((IntPtr)(DosHeader + 0x3c));
            Int64 OptionalHeader = DosHeader + NTHeader + 0x18;
            Int16 Magic = Marshal.ReadInt16((IntPtr)OptionalHeader);
            //  Console.WriteLine(Magic);
            Int64 IMAGE_DIRECTORY_ENTRY_EXPORT = 0;
            if (Magic == 0x10b)
            {
                IMAGE_DIRECTORY_ENTRY_EXPORT = OptionalHeader + 0x60;

            }
            else
            {
                IMAGE_DIRECTORY_ENTRY_EXPORT = OptionalHeader + 0x70;
            }
            Int32 EXPORT_RVA = Marshal.ReadInt32((IntPtr)IMAGE_DIRECTORY_ENTRY_EXPORT);
            Int32 IMAGE_DIRECTORY_ENTRY_EXPORT_BASE = Marshal.ReadInt32((IntPtr)(DosHeader + EXPORT_RVA + 0x10));
            Int32 NumberOfNames = Marshal.ReadInt32((IntPtr)(DosHeader + EXPORT_RVA + 0x18));
            Int32 AddressofFunctions = Marshal.ReadInt32((IntPtr)(DosHeader + EXPORT_RVA + 0x1c));
            Int32 AddressOfNames = Marshal.ReadInt32((IntPtr)(DosHeader + EXPORT_RVA + 0x20));
            Int32 AddressOfNameOrdinals = Marshal.ReadInt32((IntPtr)(DosHeader + EXPORT_RVA + 0x24));

            for (int i = 0; i < NumberOfNames; i++)
            {

                string ExpFuncName = Marshal.PtrToStringAnsi((IntPtr)(DosHeader + Marshal.ReadInt32((IntPtr)(DosHeader + AddressOfNames + i * 4))));
                //Console.WriteLine(ExpFuncName);
                if (Dinvoke.GetHashFromAPI(ExpFuncName) == hash)
                {

                    Int32 expFuncNameOrdinal = Marshal.ReadInt16((IntPtr)(DosHeader + AddressOfNameOrdinals + i * 2)) + IMAGE_DIRECTORY_ENTRY_EXPORT_BASE;
                    Int32 expFuncNameRVA = Marshal.ReadInt32((IntPtr)(DosHeader + AddressofFunctions + (4 * (expFuncNameOrdinal - IMAGE_DIRECTORY_ENTRY_EXPORT_BASE))));
                    FuncPtr = (IntPtr)((Int64)DosHeader + expFuncNameRVA);
                  //  Console.WriteLine(ExpFuncName + ":" + FuncPtr);
                    break;
                }
            }
            return FuncPtr;
        }
        public static IntPtr GetModuleAddress(string DLLName)
        {
           
            ProcessModuleCollection modules = Process.GetCurrentProcess().Modules;
            foreach (ProcessModule pm in modules)
            {

                if (pm.ModuleName.ToLower() == DLLName.ToLower())
                {
                    
                    return pm.BaseAddress;
                }

            }
            
            return IntPtr.Zero;
        }
        public static string GetHashFromAPI(string api) 
        {

            return api.GetHashCode().ToString("x");

        }
    }
}
