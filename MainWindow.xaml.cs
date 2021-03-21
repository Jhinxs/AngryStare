﻿using System;
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

namespace AngryStare
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string SelectTechPath;
        public string IconPath;
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            SetComboBox();
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
                DealWithShellcode.StackShellCode.StackGen(GetBuffer(), SelectTechPath + '\\' + Combo_Tech.Text, Combo_Tech.Text);
                Console.WriteLine("[+] Generating template Successful");
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            // MessageBox.Show(outputfile.Text);
            

            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo()
                    {

                        FileName = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe",
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
            Console.WriteLine($"[+] Complier Over : {outputfile.Text}");

        }
        public void SetCompilerBox()
        {

            CompileCommand.Text = $@"csc.exe /out:shellcode.exe /t:winexe *.cs /unsafe /nologo /platform:x64 /warn:0";

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
        public byte[] GetBuffer()
        {

            string[] BufferString = shellcodeText.Text.Split(',');
            List<byte> BufferList = new List<byte>();

            for (int i = 0; i < BufferString.Length; i++)
            {
                BufferString[i] = BufferString[i].Trim();
            }

            for (int i = 0; i < BufferString.Length; i++)
            {
                BufferList.Add(Convert.ToByte(BufferString[i], 16));

            }
            byte[] BuffByte = BufferList.ToArray();

            return BuffByte;
        }

        private void Combo_Tech_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearLog();
            try
            {

                string SelectValue = Combo_Tech.SelectedItem.ToString();
                Console.WriteLine($"[+] Use {SelectValue} Technique");
                string csfile = SelectTechPath;
                CompileCommand.Text = $@"csc.exe /out:shellcode.exe /t:winexe {csfile}{SelectValue}\*.cs /unsafe /nologo /platform:x64 /warn:0";
            }
            catch (Exception ex) { Console.WriteLine(ex); return; }

        }

        private void Button_dir_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
            if (Combo_Tech.SelectedItem == null) { Console.WriteLine("ERROR : Choose a Technique"); return; }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "exe file (*.exe)|*.exe";
            saveFileDialog.ShowDialog();
            outputfile.Text = saveFileDialog.FileName;
            try
            {
                string SelectValue = Combo_Tech.SelectedItem.ToString();
                string csfile = SelectTechPath;
                CompileCommand.Text = $@"csc.exe /out:{outputfile.Text} /t:winexe {csfile}{SelectValue}\*.cs /unsafe /nologo /platform:x64 /warn:0";
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
            string OriginFile = openFileDialog.FileName;
            if (OriginFile == "") { return; }
            WriteProgram.WritePebProgram(SelectTechPath + '\\' + Combo_Tech.Text, Combo_Tech.SelectedItem.ToString(), OriginFile);
            Console.WriteLine(@"[+] MasqueradePEB From： " + OriginFile);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string DelFilepath1 = Directory.GetParent(Environment.CurrentDirectory).ToString()+@"\Technique\MySyscall";
            foreach(var csfile in Directory.GetFiles(DelFilepath1))
            {
                if (csfile.EndsWith("AssemblyInfo.cs")||csfile.EndsWith("Program.cs")||csfile.EndsWith("PEB.cs")) 
                {
                    File.Delete(csfile);
                }
            }
            string DelFilepath2 = Directory.GetParent(Environment.CurrentDirectory).ToString() + @"\Technique\NETDelegate";
            foreach (var csfile in Directory.GetFiles(DelFilepath2))
            {
                if (csfile.EndsWith("AssemblyInfo.cs") || csfile.EndsWith("Program.cs")||csfile.EndsWith("PEB.cs"))
                {
                    File.Delete(csfile);
                }
            }
            

        }
    }
}