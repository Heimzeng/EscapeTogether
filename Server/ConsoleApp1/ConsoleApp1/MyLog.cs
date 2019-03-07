using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class MyLog : TraceListener
    {
        public override void Write(String str)
        {
            Console.Write(str);
        }
        public override void WriteLine(String str)
        {
            Console.WriteLine(str);
        }
    }
}
