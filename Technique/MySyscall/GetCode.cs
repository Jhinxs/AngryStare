using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace MySyscall
{
class GetCode
{
public static bool UseRaw = false;
public static string ResName = "";
public static Stack<byte> CodeStack()
{
   Stack<byte> st = new Stack<byte>();
   st.Push(0);
return st;
}
}
}
