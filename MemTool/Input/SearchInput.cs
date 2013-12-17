using MemTool.Core.MemoryServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Diagnostics;

namespace MemTool.Console.Input
{
    public class SearchInput : BaseInput
    {
        public string ProcessId { get; set; }
        public bool Verbose { get; set; }
        public Encoding Encoding { get; set; }
        public byte[] Data { get; set; }

        private readonly IMemoryService memoryservice;
        private readonly IMemoryFormatter memoryformatter;
        public SearchInput()
        {
            memoryservice = Core.DependencyResolver.Container.Resolve<IMemoryService>();
            memoryformatter = Core.DependencyResolver.Container.Resolve<IMemoryFormatter>();
        }

        public override bool Validate()
        {
            var procid = 0;
            return int.TryParse(ProcessId, out procid)
                && procid > 0
                && Encoding != null
                && Data != null;
        }

        public override void PerformAction()
        {
            var procid = int.Parse(ProcessId);
            var proc = Process.GetProcessById(procid);
            var handle = memoryservice.OpenProcess(procid);
            var results = memoryservice.FindData(handle, Data, Encoding);
            if (!Verbose)
                foreach (var r in results)
                {
                    var readdata = memoryservice.ReadMemory(handle, r, Data.Length * 2);
                    System.Console.WriteLine("{0}:{1}", memoryformatter.FormatAddress(r), memoryformatter.FormatData(readdata, Encoding));
                }
        }
    }
}
