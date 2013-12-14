using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.MemoryServices
{
    public class ProcessMemoryBuffer : IProcessMemoryBuffer, IDisposable
    {
        private const int BUFFER_SIZE = 256;

        private readonly IntPtr process;
        private IntPtr position;
        private Queue<byte> buffer;

        public ProcessMemoryBuffer(IntPtr process)
        {
            this.process = process;
            buffer = new Queue<byte>(BUFFER_SIZE);
        }

        public void Seek(IntPtr position)
        {
            throw new NotImplementedException();
        }

        public byte[] Peek(int length)
        {
            throw new NotImplementedException();
        }

        public byte[] Read(int length)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            buffer.Clear();
            buffer = null;
        }

        public bool EndOfStream()
        {
            return buffer == null || buffer.Count == 0;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            IntPtr lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead
        );
    }
}
