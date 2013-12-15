using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.MemoryServices
{
    public interface IMemoryService
    {
        byte[] ReadMemory(IntPtr handle, IntPtr address, int size);
        void WriteMemory(IntPtr handle, IntPtr address, byte[] data);
        IntPtr OpenProcess(int id);
        IEnumerable<IntPtr> FindData(IntPtr handle, byte[] data);
    }
}