using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.MemoryServices
{
    public class ProcessMemoryBuffer : IProcessMemoryBuffer
    {
        private const int BUFFER_SIZE = 256;

        private readonly IntPtr process;
        private IntPtr position;
        private Queue<byte> buffer;
        private bool reading;

        public bool EndOfStream 
        {
            get
            {
                return buffer == null || buffer.Count == 0;
            }
        }

        public ProcessMemoryBuffer(IntPtr process)
        {
            reading = true;
            this.process = process;
            buffer = new Queue<byte>(BUFFER_SIZE);
            position = Process.GetProcesses().First(x => x.Handle == process)
                .MainModule
                .BaseAddress;
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
            if (reading && length > buffer.Count)
                Fill();
            if (buffer.Count == 0)
                return null;
            var size = Math.Min(length, buffer.Count);
            var data = new byte[size];
            for (int i = 0; i < length; i++)
            {
                data[i] = buffer.Dequeue();
                if (buffer.Count == 0)
                    break;
            }
            return data;
        }

        /// <summary>
        /// Fills the buffer, from process memory, when we need to.
        /// </summary>
        private void Fill()
        {
            if (!reading)
                return;

            var data = new byte[BUFFER_SIZE];
            IntPtr numread;
            var success = ReadProcessMemory(process, position, data, BUFFER_SIZE, out numread);
            // if not succeed, or numread is less than buffer_size, then we have reached the end of our memory.
            if (!success | numread.ToInt32() < BUFFER_SIZE)
                reading = false;
            // Fill up what we read, if didn't read anything, quit.
            if (!success | numread.ToInt32() == 0)
                return;
            for (int i = 0; i < numread.ToInt32(); i++)
                buffer.Enqueue(data[i]);
            position += numread.ToInt32();
        }

        public void Dispose()
        {
            buffer.Clear();
            buffer = null;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead
        );
    }
}
