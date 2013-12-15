using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.MemoryServices
{
    public class MemoryReader : IMemoryReader
    {
        private IProcessMemoryBuffer buffer;

        public MemoryReader(Process p)
        {
            buffer = new ProcessMemoryBuffer(p);
        }

        public IntPtr Find(byte[] needle)
        {
            // one at a time, checking the array.
            var found = false;
            var correctindex = 0;
            var pointer = new IntPtr();
            var b = buffer.Read(1)[0];
            var c = 0;
            while (!buffer.EndOfStream || found)
            {
                if (needle[correctindex] == b)
                    correctindex++;
                else
                    correctindex = 0;
                if (correctindex == needle.Length - 1)
                {
                    found = true;
                    pointer = buffer.Position - buffer.Count;
                }
                var data = buffer.Read(1);
                if (data != null)
                    b = data[0];
                c++;
            }
            return pointer;
        }

        public static IEnumerable<Process> GetProcesses()
        {
            return Process.GetProcesses();
        }

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
            buffer.Dispose();
        }
    }
}
