using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core
{
    public static class Verbose
    {
        public static TextWriter OutputStream { private get; set; }

        public static void Write(string s, params object[] args)
        {
            if (OutputStream == null)
                return;
            OutputStream.Write(s, args);
        }

        public static void WriteLine(string s, params object[] args)
        {
            if (OutputStream == null)
                return;
            OutputStream.WriteLine(s, args);
        }
    }
}
