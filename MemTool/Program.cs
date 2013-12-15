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

            var searchproc = "";
            var searchstring = "";
            var searchprocid = "";
            var showhelp = false;

            var opts = new OptionSet()
            {
                {"l", "List Processes", x => list = true},
                {"v", "Verbose On", x => {MemTool.Core.Verbose.OutputStream = System.Console.Out; verbose = true;}},
                {"sp=", "Search for a Process with Name", x => {searchproc = x; list = true;}},
                {"sm", "Search Memory", x => searchmem = true},
                {"ss=", "Search for a String", x => {searchmem = true; searchstring = x;}},
                {"pid=", "Process ID to Search Through", x => {searchmem = true; searchprocid = x;}},
                {"h", "Show Help", x => showhelp = true}
            };
            opts.Parse(args);

            // Validate
            if (searchmem && 
                (string.IsNullOrWhiteSpace(searchstring) || string.IsNullOrWhiteSpace(searchprocid) ))
                showhelp = true;

            if (!list && !searchmem)
                showhelp = true;

            // Help if needed.
            if (showhelp)
            {
                opts.WriteOptionDescriptions(System.Console.Out);
                return;
            }

            // Execute commands.
            if (list)
            {
                var ps = string.IsNullOrWhiteSpace(searchproc)
                    ? Process.GetProcesses()
                    : Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(searchproc.ToLower()));
                foreach (var p in ps)
                    System.Console.WriteLine("{0}:\t{1}", p.Id, p.ProcessName);
                return;
            }

            if (searchmem)
            {
                var service = DependencyResolver.Container.Resolve<IMemoryService>();
                var formatter = DependencyResolver.Container.Resolve<IMemoryFormatter>();
                var id = int.Parse(searchprocid);
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
        }
    }
}
