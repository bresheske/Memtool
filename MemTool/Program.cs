using MemTool.Core;
using MemTool.Core.MemoryServices;
using Microsoft.Practices.Unity;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Console
{
    class Program
    {
        static void InitContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IMemoryService, DefaultMemoryService>();
            container.RegisterType<IMemoryFormatter, DefaultMemoryFormatter>();
            MemTool.Core.DependencyResolver.Container = container;
        }

        static void Main(string[] args)
        {
            // Set up dependency resolution first.
            InitContainer();

            var list = false;
            var searchmem = false;
            var verbose = false;
            var readmem = false;
            var writemem = false;

            var usestring = false;
            var useint = false;
            var usebyte = false;
            var usedata = "";

            var procid = "";
            var showhelp = false;
            var address = "";
            var length = "";
            var enc = "";

            var opts = new OptionSet()
            {
                {"l", "List Processes", x => list = true},
                {"v", "Verbose On", x => {MemTool.Core.Verbose.OutputStream = System.Console.Out; verbose = true;}},
                {"sm", "Search Memory", x => searchmem = true},
                {"rm", "Read Memory", x => readmem = true},
                {"wm", "Write Memory", x => writemem = true},

                {"addr=", "Address to Read/Write", x => address = x},
                {"len=", "Length to Read", x => length = x},
                {"pid=", "Process ID", x =>  procid = x},
                {"enc=", "Encoding. (uni, def)", x =>  enc = x},

                {"string", "Use a String for Write/Search", x => usestring = true},
                {"int", "Use an Int for Write/Search", x => useint = true},
                {"byte", "Use a Byte for Write/Search", x => usebyte = true},
                {"d=", "Data to Write/Search", x => usedata = x},

                {"h", "Show Help", x => showhelp = true}
            };
            opts.Parse(args);

            if (!list && !searchmem && !readmem && !writemem)
                showhelp = true;

            // Help if needed.
            if (showhelp)
            {
                opts.WriteOptionDescriptions(System.Console.Out);
                return;
            }

            // Execute commands.
            var service = DependencyResolver.Container.Resolve<IMemoryService>();
            var formatter = DependencyResolver.Container.Resolve<IMemoryFormatter>();
            var encoding = System.Text.Encoding.Unicode;
            if (enc == "def")
                encoding = System.Text.Encoding.Default;
            byte[] data;
            if (useint)
                data = IntToBytes(int.Parse(usedata));
            else if (usebyte)
                data = ConvertHexStringToByteArray(usedata);
            else
                data = encoding.GetBytes(usedata);

            // Just list out processes.
            if (list)
            {
                var ps = string.IsNullOrWhiteSpace(encoding.GetString(data))
                    ? Process.GetProcesses()
                    : Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(encoding.GetString(data).ToLower()));
                foreach (var p in ps)
                    System.Console.WriteLine("{0}:\t{1}", p.Id, p.ProcessName);
            }

            // Searching Memory for data.
            if (searchmem)
            {
                var id = int.Parse(procid);
                var proc = Process.GetProcessById(id);
                var handle = service.OpenProcess(id);
                var results = service.FindData(handle, data, encoding);
                if (!verbose)
                    foreach (var r in results)
                    {
                        var readdata = service.ReadMemory(handle, r, data.Length * 2);
                        System.Console.WriteLine("{0}:{1}", formatter.FormatAddress(r), formatter.FormatData(readdata, encoding));
                    }
            }

            // Reading Memory
            if (readmem)
            {
                var id = int.Parse(procid);
                var proc = Process.GetProcessById(id);
                var handle = service.OpenProcess(id);
                var addr = new IntPtr(Convert.ToInt32(address, 16));
                var len = int.Parse(length);
                var readdata = service.ReadMemory(handle, addr, len);
                System.Console.WriteLine(formatter.FormatMultiLineData(readdata, addr, encoding));
            }

            // Writing Memory
            if (writemem)
            {
                var id = int.Parse(procid);
                var proc = Process.GetProcessById(id);
                var handle = service.OpenProcess(id);
                var addr = new IntPtr(Convert.ToInt32(address, 16));

                if (!service.WriteMemory(handle, addr, data))
                {
                    var error = Marshal.GetLastWin32Error();
                    System.Console.WriteLine("Memory Write Failed.");
                    System.Console.WriteLine("Error: {0}:{1}", error, new Win32Exception(error).Message);
                }
            }
        }

        /// <summary>
        /// Small function I found somewhere in StackOverflow.
        /// </summary>
        /// <param name="integer"></param>
        /// <returns></returns>
        private static byte[] IntToBytes(int integer)
        {
            byte[] intBytes = BitConverter.GetBytes(integer);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/321370/convert-hex-string-to-byte-array
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }
    }
}
