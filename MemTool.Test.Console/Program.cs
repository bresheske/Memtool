using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Test.Console
{
    /// <summary>
    /// All this program does is contain some memory to be manipulated.
    /// </summary>
    class Program
    {
        public static string PublicString = "public data";
        private static string PrivateString = "private data";

        static void Main(string[] args)
        {
            // Hold execution to hold memory.
            var something = System.Console.In.ReadLine();
        }
    }
}
