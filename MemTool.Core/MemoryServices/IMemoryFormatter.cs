using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.MemoryServices
{
    public interface IMemoryFormatter
    {
        string FormatAddress(IntPtr address);
        string FormatData(byte[] data);
    }
}
