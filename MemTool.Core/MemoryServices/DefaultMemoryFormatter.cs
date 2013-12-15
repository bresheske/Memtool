using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.MemoryServices
{
    public class DefaultMemoryFormatter : IMemoryFormatter
    {
        /// <summary>
        /// Formats the pointer into 0x00000000 format.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public string FormatAddress(IntPtr address)
        {
            var hex = string.Format("{0:X}", (int)address);
            while (hex.Length < 8)
                hex = "0" + hex;
            hex = "0x" + hex;
            return hex;
        }

        /// <summary>
        /// Formats data into 00 00 00 00 00 00 00 00 : Sample Text.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string FormatData(byte[] data)
        {
            var maxcols = 14;
            var sb = new StringBuilder();
            // Displaying X only per row.
            var length = Math.Min(data.Length, maxcols);
            for (int i = 0; i < length; i++)
            {
                sb.AppendFormat("{0:X2} ", data[i]);
            }
            // Add a pipe to seperate our characters.
            sb.Append("| ");
            // Add the rest of our text.
            var text = System.Text.Encoding.Unicode.GetString(data);
            if (text.Length > length)
                text = text.Substring(0, length);
            sb.Append(text);
            return sb.ToString();
        }
    }
}
