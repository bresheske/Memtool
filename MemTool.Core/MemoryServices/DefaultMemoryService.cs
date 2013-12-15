using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.MemoryServices
{
    public class DefaultMemoryService : IMemoryService
    {
        private readonly IMemoryFormatter formatter;

        public DefaultMemoryService(IMemoryFormatter formatter)
        {
            this.formatter = formatter;
        }

        public byte[] ReadMemory(IntPtr handle, IntPtr address, int size)
        {
            var data = new byte[size];
            IntPtr numread;
            ReadProcessMemory(handle, address, data, data.Length, out numread);
            return data;
        }

        public bool WriteMemory(IntPtr handle, IntPtr address, byte[] data)
        {
            UIntPtr numwritten;
            return WriteProcessMemory(handle, address, data, (uint)data.Length, out numwritten);
        }

        public IntPtr OpenProcess(int id)
        {
            return OpenProcess(0x001F0FFF, true, id);
        }

        public IEnumerable<IntPtr> FindData(IntPtr handle, byte[] data, Encoding enc)
        {
            var output = new List<IntPtr>();
            var addr = new IntPtr(0x00000000);
            var end = new IntPtr(0x7F000000);
            var queue = new Queue<byte>();
            var buffsize = 2048;

            addr = Fill(queue, handle, addr, buffsize, end);
            var numcorrect = 0;
            var correctaddress = IntPtr.Zero;
            while (queue.Count > 0)
            {
                if (queue.Count < buffsize / 2)
                {
                    addr = Fill(queue, handle, addr, buffsize, end);
                }

                var b = queue.Dequeue();
                if (b == data[numcorrect])
                {
                    if (numcorrect == 0)
                        correctaddress = IntPtr.Subtract(addr, queue.Count + 1);
                    numcorrect++;
                }
                else
                {
                    numcorrect = 0;
                }

                if (numcorrect == data.Length)
                {
                    // Found!
                    var tempdata = ReadMemory(handle, correctaddress, data.Length * 2);

                    Verbose.WriteLine("{0}:{1}", formatter.FormatAddress(correctaddress), formatter.FormatData(tempdata, enc));
                    numcorrect = 0;
                    output.Add(correctaddress);
                }
            }
            return output;
        }

        private IntPtr Fill(Queue<byte> data, IntPtr handle, IntPtr address, int buffsize, IntPtr endaddress)
        {
            if ((int)address >= (int)endaddress)
                return address;
            var numread = IntPtr.Zero;

            var buff = new byte[buffsize];
            var curaddress = address;
            while ((int)numread == 0 && (int)curaddress < (int)endaddress)
            {
                var percent = (double)curaddress / (double)endaddress;
                //Verbose.Write("\r{0:P} : {1:X}", percent, (int)curaddress);

                ReadProcessMemory(handle, curaddress, buff, buff.Length, out numread);
                for (int i = 0; i < (int)numread; i++)
                {
                    data.Enqueue(buff[i]);
                }
                curaddress = (int)numread > 0
                    ? IntPtr.Add(curaddress, (int)numread)
                    : IntPtr.Add(curaddress, buffsize);
            }

            return curaddress;
        }

        public uint GetError()
        {
            return GetLastError();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(
            IntPtr hProcess, 
            IntPtr lpBaseAddress, 
            byte[] lpBuffer, 
            uint nSize, 
            out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(
            int dwDesiredAccess, 
            bool bInheritHandle, 
            int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

    }
}
