using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AngryStare.UI
{
    class WriteProgram
    {
        public static void WritePebCS(string ProgramPath,string tech,string pebfile) 
        {
            /// 写入头部部分
            FileStream copyPEBHeader = new FileStream(ProgramPath + @"\PEB.cs", FileMode.Create);
            StreamWriter writePEBHeader = new StreamWriter(copyPEBHeader);
            writePEBHeader.WriteLine(@"using System;
using System.IO;
            using System.Text;
            using System.Diagnostics;
            using System.Runtime.InteropServices; ");
            writePEBHeader.WriteLine($"namespace {tech}");
            writePEBHeader.Close();
            copyPEBHeader.Close();
            ///读取模板peb并写入
            FileStream readPEBEnd = new FileStream(Directory.GetParent(Environment.CurrentDirectory).ToString() + "\\HatTrick\\PEB.txt", FileMode.Open,FileAccess.Read);
            // StreamWriter writePEBEnd = new StreamWriter(readPEBEnd);
            StreamReader readPEBEndStream = new StreamReader(readPEBEnd);

            FileStream writePEBEnd = new FileStream(ProgramPath + @"\PEB.cs", FileMode.Append);
            StreamWriter writePEBEndStream = new StreamWriter(writePEBEnd);

            string swapContent = string.Empty;
            try
            {
                while ((swapContent = readPEBEndStream.ReadLine()) != null)
                {
                    //writePEBEnd.WriteLine(swapContent);
                    writePEBEndStream.WriteLine(swapContent);
                }

            }
            catch (Exception ex) { Console.WriteLine(ex); }
            finally
            {
                writePEBEndStream.Close();
                writePEBEnd.Close();
                readPEBEndStream.Close();
                readPEBEnd.Close();
            }

           //File.Copy(Directory.GetParent(Environment.CurrentDirectory).ToString() + "\\HatTrick\\PEB.cs", ProgramPath + @"\PEB.cs", true);

    //        FileStream fs = new FileStream(ProgramPath + @"\Program.cs",FileMode.Create);
    //        StreamWriter sw = new StreamWriter(fs);
    //        sw.WriteLine(@"using System;
    //        using System.Collections.Generic;
    //        using System.Text;");

    //        sw.WriteLine($"namespace {tech}");
    //        sw.WriteLine(@"    {
    //    class Program
    //    {
    //        static void Main(string[] args)
    //        {");
    //        sw.WriteLine($"PEB peb = new PEB(@\"{pebfile}\");");

           
    //        sw.WriteLine(@"Execute.Exec();
    //        }
    //    }
    //}");
    //        sw.Close();
    //        fs.Close();
            
        }
    }
}
