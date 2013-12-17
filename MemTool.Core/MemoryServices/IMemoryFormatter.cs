using System;
using System.Text;

namespace MemTool.Core.MemoryServices
{
    public interface IMemoryFormatter
    {
        string FormatAddress(IntPtr address);
        string FormatData(byte[] data, Encoding enc);
        string FormatMultiLineData(byte[] data, IntPtr baseaddress, Encoding enc);
    }
}
