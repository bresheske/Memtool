using MemTool.Core;
using MemTool.Core.MemoryServices;
using Microsoft.Practices.Unity;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var writestring = false;
            var writeint = false;
            var writebyte = false;

            var searchproc = "";
            var searchstring = "";
            var procid = "";
            var showhelp = false;
            var address = "";
            var length = "";
            var writedata = "";

            var opts = new OptionSet()
            {
                {"l", "List Processes", x => list = true},
                {"v", "Verbose On", x => {MemTool.Core.Verbose.OutputStream = System.Console.Out; verbose = true;}},
                {"sm", "Search Memory", x => searchmem = true},
                {"rm", "Read Memory", x => readmem = true},
                {"wm", "Write Memory", x => writemem = true},

                {"addr=", "Address to Read/Write", x => address = x},
                {"len=", "Length to Read", x => length = x},
                {"sp=", "Search for a Process with Name", x => searchproc = x},
                {"ss=", "Search for a String", x => searchstring = x},
                {"pid=", "Process ID", x =>  procid = x},

                {"string", "Write a String", x => writestring = true},
                {"int", "Write an Int", x => writeint = true},
                {"byte", "Write a Byte", x => writebyte = true},
                {"data=", "Data to Write", x => writedata = x},

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

            // Just list out processes.
            if (list)
            {
                var ps = string.IsNullOrWhiteSpace(searchproc)
                    ? Process.GetProcesses()
                    : Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(searchproc.ToLower()));
                foreach (var p in ps)
                    System.Console.WriteLine("{0}:\t{1}", p.Id, p.ProcessName);
                return;
            }

            // Searching Memory for data.
            if (searchmem)
            {
                var id = int.Parse(procid);
                var proc = Process.GetProcessById(id);
                var handle = service.OpenProcess(id);
                var results = service.FindData(handle, System.Text.Encoding.Unicode.GetBytes(searchstring));
                if (!verbose)
                    foreach (var r in results)
                    {
                        var data = service.ReadMemory(handle, r, searchstring.Length * 2);
                        System.Console.WriteLine("{0}:{1}", formatter.FormatAddress(r), formatter.FormatData(data));
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
                var data = service.ReadMemory(handle, addr, len);
                System.Console.WriteLine(formatter.FormatMultiLineData(data, addr));
            }

            // Writing Memory
            if (writemem)
            {
                var id = int.Parse(procid);
                var proc = Process.GetProcessById(id);
                var handle = service.OpenProcess(id);
                var addr = new IntPtr(Convert.ToInt32(address, 16));
                byte[] data;
                if (writeint)
                    data = IntToBytes(int.Parse(writedata));
                else if (writebyte)
                    data = System.Text.Encoding.ASCII.GetBytes(writedata);
                else
                    data = System.Text.Encoding.Unicode.GetBytes(writedata);

                if (!service.WriteMemory(handle, addr, data))
                    System.Console.WriteLine("Memory Write Failed.");
            }
        }

        private static byte[] IntToBytes(int integer)
        {
            byte[] intBytes = BitConverter.GetBytes(integer);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }
    }
}
