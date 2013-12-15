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
            var sw = new Stopwatch();

            sw.Start();
            StandardSearch();
            sw.Stop();
            System.Console.WriteLine();
            System.Console.WriteLine("{0:0.00} seconds to execute.", sw.ElapsedMilliseconds / 1000d);

            sw.Restart();
            BetterSearch();
            sw.Stop();
            System.Console.WriteLine();
            System.Console.WriteLine("{0:0.00} seconds to execute.", sw.ElapsedMilliseconds / 1000d);
            sw.Restart();
        }

        static void BetterSearch()
        {
            // Startup a test process.
            var info = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = @"..\..\..\MemTool.Test.Console\bin\Debug\MemTool.Test.Console.exe"
            };
            var process = Process.Start(info);

            var handle = OpenProcess(0x0010, true, process.Id);
            var str = System.Text.Encoding.Unicode.GetBytes("private data");
            System.Console.WriteLine("Beginning Better Search... Searching for '{0}'", System.Text.Encoding.Unicode.GetString(str));
            System.Console.WriteLine();

            var addr = new IntPtr(0x00000000);
            var end = new IntPtr(0x0F000000);
            var queue = new Queue<byte>();
            var buffsize = 512;

            Fill(queue, handle, addr, buffsize, end);
            while (queue.Count > 0)
            {
                if (queue.Count < buffsize / 2)
                    Fill(queue, handle, addr, buffsize, end);

                // we have data, let's dequeue and loop through.
                var numcorrect = 0;
                var correctaddress = IntPtr.Zero;
                var b = queue.Dequeue();
                if (b == str[numcorrect])
                {
                    if (numcorrect == 0)
                        correctaddress = addr;
                    numcorrect++;
                }
                else
                {
                    numcorrect = 0;
                }

                if (numcorrect == str.Length)
                {
                    // Found!

                }
            }
        }

        static void Fill(Queue<byte> data, IntPtr handle, IntPtr address, int buffsize, IntPtr endaddress)
        {
            if ((int)address >= (int)endaddress)
                return;
            IntPtr numread;
            var buff = new byte[buffsize];
            ReadProcessMemory(handle, address, buff, buff.Length, out numread);
            for (int i = 0; i < (int)numread; i++)
            {
                data.Enqueue(buff[i]);
            }
        }

        static void StandardSearch()
        {
            // Startup a test process.
            var info = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = @"..\..\..\MemTool.Test.Console\bin\Debug\MemTool.Test.Console.exe"
            };
            var process = Process.Start(info);

            var handle = OpenProcess(0x0010, true, process.Id);
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
            process.Kill();
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
