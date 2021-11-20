using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurniCracker
{
    public static class Logging
    {
        public static void WriteLine(object o, ConsoleColor c = ConsoleColor.Gray)
        {
            Console.ForegroundColor = c;
            Console.WriteLine(o);
        }
    }
}
