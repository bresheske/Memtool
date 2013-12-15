using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.MemoryServices
{
    public interface IProcessMemoryBuffer: IDisposable
    {
        IntPtr Position { get; }
        int Count { get; }
        bool EndOfStream { get; }
        void Seek(IntPtr position);
        byte[] Peek(int length);
        byte[] Read(int length);
    }
}
