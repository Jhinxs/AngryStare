using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace NETDelegate
{
class GetCode
{
public static bool UseRaw = false;
public static Stack<byte> CodeStack()
{
   Stack<byte> st = new Stack<byte>();
   st.Push(0);
return st;
}
}
}
