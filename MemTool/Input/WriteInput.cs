using MemTool.Core.MemoryServices;
using Microsoft.Practices.Unity;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MemTool.Console.Input
{
    public class WriteInput : BaseInput
    {
        public string ProcessId { get; set; }
        public byte[] Data { get; set; }
        public string Address { get; set; }

        public bool WriteSuccess { get; private set; }

        private readonly IMemoryService memoryservice;
        public WriteInput()
        {
            memoryservice = Core.DependencyResolver.Container.Resolve<IMemoryService>();
            WriteSuccess = true;
        }

        public override bool Validate()
        {
            var tempid = 0;
            var procidgood = int.TryParse(ProcessId, out tempid)
                && tempid > 0;

            return procidgood;
        }

        public override void PerformAction()
        {
            var id = int.Parse(ProcessId);
            var proc = Process.GetProcessById(id);
            var handle = memoryservice.OpenProcess(id);
            var addr = new IntPtr(Convert.ToInt32(Address, 16));

            if (!memoryservice.WriteMemory(handle, addr, Data))
            {
                WriteSuccess = false;
                var error = Marshal.GetLastWin32Error();
                System.Console.WriteLine("Memory Write Failed.");
                System.Console.WriteLine("Error: {0}:{1}", error, new Win32Exception(error).Message);
            }
        }
    }
}
