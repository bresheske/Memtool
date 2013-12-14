using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.MemoryServices
{
    public interface IMemoryReader
    {
        byte[] Read(int length);
        void Seek(IntPtr position);
        void Skip(int length);
        void Reset();
        IntPtr Find(object o);
        int ReadInt32();
        bool ReadBool();
        char ReadChar();
        string ReadString();
    }
}
