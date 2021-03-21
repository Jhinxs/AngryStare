using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AngryStare.MetaData
{
    class CopyMateData
    {
        public static void ModifyMetadata(string path) 
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "exe file (*.exe)|*.exe";
            openFileDialog.ShowDialog();
            string OriginFile = openFileDialog.FileName;
            if (OriginFile == "") { return; }
            
            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(OriginFile);
            Console.WriteLine($"[+] Clone MetaData From: {OriginFile}");
            Console.WriteLine("      FileDescription: " + myFileVersionInfo.FileDescription + '\n'+
                "      FileVersion:  "+myFileVersionInfo.FileVersion +'\n'+
                "      ProductName:  "+myFileVersionInfo.ProductName+'\n'+
                "      Version number:  " + myFileVersionInfo.ProductVersion + '\n' +
                "      Copyright:  "+myFileVersionInfo.LegalCopyright + '\n'+
                "      CompanyName:  " +myFileVersionInfo.CompanyName);
            
            FileStream fs = new FileStream(path + @"\AssemblyInfo.cs", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(@"using System.Reflection;
                           using System.Runtime.CompilerServices;
                           using System.Runtime.InteropServices;");
            sw.WriteLine($"[assembly: AssemblyTitle(\"{myFileVersionInfo.FileDescription}\")]");
            sw.WriteLine($"[assembly: AssemblyDescription(\"{myFileVersionInfo.FileDescription}\")]");
            sw.WriteLine($"[assembly: AssemblyConfiguration(\"\")]");
            sw.WriteLine($"[assembly: AssemblyCompany(\"{myFileVersionInfo.CompanyName}\")]");
            sw.WriteLine($"[assembly: AssemblyProduct(\"{myFileVersionInfo.ProductName}\")]");
            sw.WriteLine($"[assembly: AssemblyCopyright(\"{myFileVersionInfo.LegalCopyright}\")]");
            sw.WriteLine($"[assembly: AssemblyTrademark(\"{myFileVersionInfo.CompanyName}\")]");
            sw.WriteLine($"[assembly: AssemblyCulture(\"\")]");
            sw.WriteLine($"[assembly: ComVisible(false)]");
            sw.WriteLine($"[assembly: Guid(\"{Guid.NewGuid()}\")]");
            sw.WriteLine($"[assembly: AssemblyVersion(\"{myFileVersionInfo.ProductVersion}\")]");
            sw.WriteLine($"[assembly: AssemblyFileVersion(\"{myFileVersionInfo.FileVersion}\")]");
            sw.Close();
            fs.Close();
        }
    }
}
