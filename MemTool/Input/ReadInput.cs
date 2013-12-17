using MemTool.Core.MemoryServices;
using Microsoft.Practices.Unity;
using System;
using System.Diagnostics;
using System.Text;

namespace MemTool.Console.Input
{
    public class ReadInput : BaseInput
    {
        public string ProcessId { get; set; }
        public string Address { get; set; }
        public string Length { get; set; }
        public Encoding Encoding { get; set; }

        private readonly IMemoryService memoryservice;
        private readonly IMemoryFormatter memoryformatter;
        public ReadInput()
        {
            memoryservice = Core.DependencyResolver.Container.Resolve<IMemoryService>();
            memoryformatter = Core.DependencyResolver.Container.Resolve<IMemoryFormatter>();
        }

        public override bool Validate()
        {
            var tempid = 0;
            var procidgood = int.TryParse(ProcessId, out tempid)
                && tempid > 0;
            var templen = 0;
            var lengood = int.TryParse(Length, out templen)
                && templen > 0;

            return procidgood && lengood;
        }

        public override void PerformAction()
        {
            var id = int.Parse(ProcessId);
            var proc = Process.GetProcessById(id);
            var handle = memoryservice.OpenProcess(id);
            var addr = new IntPtr(Convert.ToInt32(Address, 16));
            var len = int.Parse(Length);
            var readdata = memoryservice.ReadMemory(handle, addr, len);
            System.Console.WriteLine(memoryformatter.FormatMultiLineData(readdata, addr, Encoding));
        }
    }
}
