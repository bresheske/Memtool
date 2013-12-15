using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MemTool.Core.Extensions;
using System.IO;

namespace MemTool.Core.MemoryServices
{
    public class HardMemoryReader : IMemoryReader
    {
        List<byte> memory;
        private Process process;

        public HardMemoryReader(Process p)
        {
            memory = new List<byte>();
            process = p;
            Init();
        }

        private void Init()
        {
            var buff = new byte[256];
            var prochandle = OpenProcess(0x0010, false, process.Id);
            var firstaddress = new IntPtr(0);
            var lastaddress = new IntPtr((int)process.MainModule.BaseAddress + process.MainModule.ModuleMemorySize);
            var addr = firstaddress;
            IntPtr numread;

            while ((int)addr < (int)lastaddress)
            {
                ReadProcessMemory(prochandle, addr, buff, buff.Length, out numread);
                memory.AddRange(buff);
                addr = IntPtr.Add(addr, (int)numread);
            }

            File.WriteAllLines("test.txt", memory.Select(x => ((int)x).ToString()));
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        public byte[] Read(int length)
        {
            throw new NotImplementedException();
        }

        public void Seek(IntPtr position)
        {
            throw new NotImplementedException();
        }

        public void Skip(int length)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public IntPtr Find(byte[] needle)
        {
            var index = memory.FindSub(needle);
            return index > 0
                ? process.MainModule.BaseAddress + index
                : new IntPtr(0);
        }

        public int ReadInt32()
        {
            throw new NotImplementedException();
        }

        public bool ReadBool()
        {
            throw new NotImplementedException();
        }

        public char ReadChar()
        {
            throw new NotImplementedException();
        }

        public string ReadString()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
