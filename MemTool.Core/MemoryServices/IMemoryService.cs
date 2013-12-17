using System;
using System.Collections.Generic;
using System.Text;

namespace MemTool.Core.MemoryServices
{
    public interface IMemoryService
    {
        byte[] ReadMemory(IntPtr handle, IntPtr address, int size);
        bool WriteMemory(IntPtr handle, IntPtr address, byte[] data);
        IntPtr OpenProcess(int id);
        IEnumerable<IntPtr> FindData(IntPtr handle, byte[] data, Encoding enc);
        uint GetError();
    }
}