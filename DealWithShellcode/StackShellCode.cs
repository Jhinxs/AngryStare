using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryStare.DealWithShellcode
{
    class StackShellCode
    {
        public static void StackGen(byte[] shellcodebyte,string path,string tech_namespace,string ResName)
        {
         
            byte[] shellcode = shellcodebyte;
            FileStream fs = new FileStream(path+@"\GetCode.cs", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(@"using System;");
            sw.WriteLine(@"using System.Collections.Generic;");
            sw.WriteLine(@"using System.Linq;");
            sw.WriteLine(@"using System.Text;");

            sw.WriteLine($"namespace {tech_namespace}");
            sw.WriteLine(@"{");
            sw.WriteLine(@"class GetCode");
            sw.WriteLine(@"{");

            if (AngryStare.MainWindow.UseRaw == true)
            {
                sw.WriteLine("public static bool UseRaw = true;");
                sw.WriteLine($"public static string ResName = \"{ResName}\";");
            }
            else 
            {
                sw.WriteLine("public static bool UseRaw = false;");
                sw.WriteLine($"public static string ResName = \"\";");
            }
             
            sw.WriteLine("public static Stack<byte> CodeStack()");
            sw.WriteLine("{");
            sw.WriteLine("   Stack<byte> st = new Stack<byte>();");
            Stack<byte> st = new Stack<byte>();
            for (int i = shellcode.Length - 1; i >= 0; i--)
            {

                for (int j = 0; j <= 255; j++)
                {

                    if (j == shellcode[i])
                    {

                        sw.WriteLine($"   st.Push({shellcode[i]});");
                        // sw.WriteLine(string.Format("   st.Push({0});",shellcode[i]));
                        break;
                    }

                }

            }
            sw.WriteLine("return st;");
            sw.WriteLine("}");
            sw.WriteLine("}");
            sw.WriteLine("}");
            sw.Close();
            fs.Close();

        }
    }
}
