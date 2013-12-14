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

        public MemoryReader(IntPtr process)
        {
            buffer = new ProcessMemoryBuffer(process);
        }

        public IntPtr Find(object o)
        {
            // grab our needle to find in our haystack.
            var key = ObjectToByteArray(o);
            // init our buffer. 2x the size of our key.  

            throw new NotImplementedException();
        }

        public static IEnumerable<Process> GetProcesses()
        {
            return Process.GetProcesses();
        }

        protected byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            var output = ms.ToArray();
            ms.Dispose();
            return output;
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
    }
}
