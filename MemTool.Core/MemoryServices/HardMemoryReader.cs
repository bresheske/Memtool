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
        private IntPtr openhandle;

        [Flags]
        enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        public HardMemoryReader(Process p)
        {
            memory = new List<byte>();
            process = p;
            Init();
        }

        private void Init()
        {
            openhandle = OpenProcess((int)ProcessAccessFlags.All, true, process.Id);
            var addr = process.MainModule.BaseAddress;
            var buff = new byte[256];
            IntPtr numread;

            while (ReadProcessMemory(openhandle, addr, buff, buff.Length, out numread))
            {
                var data = buff;
                if ((int)numread < buff.Length)
                    data = buff.Take((int)numread).ToArray();
                memory.AddRange(data);
                addr = IntPtr.Add(addr, (int)numread);
            }
            
            File.WriteAllText("test-uni.txt", System.Text.Encoding.Unicode.GetString(memory.ToArray()));
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
