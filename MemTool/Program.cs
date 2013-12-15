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
        static void Main(string[] args)
        {
            var opts = new OptionSet()
            {

            };
            opts.Parse(args);


        }

        static void BetterSearch(IntPtr handle)
        {
            var str = System.Text.Encoding.Unicode.GetBytes("private data");
            System.Console.WriteLine("Beginning Better Search... Searching for '{0}'", System.Text.Encoding.Unicode.GetString(str));
            System.Console.WriteLine();

            var addr = new IntPtr(0x00000000);
            var end = new IntPtr(0x0F000000);
            var queue = new Queue<byte>();
            var buffsize = 512;

            addr = Fill(queue, handle, addr, buffsize, end);
            var numcorrect = 0;
            var correctaddress = IntPtr.Zero;
            var count = 0;
            while (queue.Count > 0)
            {
                if (queue.Count < buffsize / 2)
                {
                    addr = Fill(queue, handle, addr, buffsize, end);
                    count = 0;
                }

                // we have data, let's dequeue and loop through.
                
                var b = queue.Dequeue();
                count++;
                if (b == str[numcorrect])
                {
                    if (numcorrect == 0)
                        correctaddress = IntPtr.Add(addr, count * 2);
                    numcorrect++;
                }
                else
                {
                    numcorrect = 0;
                }

                if (numcorrect == str.Length)
                {
                    // Found!
                    numcorrect = 0;
                    System.Console.WriteLine();
                    System.Console.WriteLine("Found string at Address: {0:X}", (int)correctaddress);
                    System.Console.WriteLine();
                }
            }
        }

        static IntPtr Fill(Queue<byte> data, IntPtr handle, IntPtr address, int buffsize, IntPtr endaddress)
        {
            if ((int)address >= (int)endaddress)
                return address;

            var numread = IntPtr.Zero;
            
            var buff = new byte[buffsize];
            var curaddress = address;
            while ((int)numread == 0 && (int) curaddress < (int)endaddress)
            {
                var percent = (double)curaddress / (double)endaddress;
                System.Console.Write("\r{0:P} :: {1:X}", percent, (int)curaddress);
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

        static void StandardSearch(IntPtr handle)
        {
            var str = System.Text.Encoding.Unicode.GetBytes("private data");
            System.Console.WriteLine("Beginning Standard Search... Searching for '{0}'", System.Text.Encoding.Unicode.GetString(str));
            System.Console.WriteLine();

            var addr = new IntPtr(0x00000000);
            var end = new IntPtr(0x0F000000);
            var buff = new byte[512];
            while ((uint)addr < (uint)end)
            {
                IntPtr numread;

                var percent = (double)addr / (double)end;
                System.Console.Write("\r{0:P} :: {1:X}", percent, (int)addr);

                var success = ReadProcessMemory(handle, addr, buff, buff.Length, out numread);
                if (!success || (int)numread == 0)
                {
                    addr = IntPtr.Add(addr, buff.Length);
                    continue;
                }

                // do search
                var uni = System.Text.Encoding.Unicode.GetString(buff);
                var index = uni.IndexOf(System.Text.Encoding.Unicode.GetString(str));
                if (index > -1)
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine("Found string at Address: {0:X}", (int)IntPtr.Add(addr, index * 2));
                    System.Console.WriteLine(uni.Substring(index));
                    break;
                }

                addr = IntPtr.Add(addr, (int)numread);
            }
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
    }
}
