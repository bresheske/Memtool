using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.MemoryServices
{
    public class DefaultMemoryFormatter : IMemoryFormatter
    {
        private const int MAX_COLS_PER_ROW = 16;

        /// <summary>
        /// Formats the pointer into 00000000 format.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public string FormatAddress(IntPtr address)
        {
            var hex = string.Format("{0:X}", (int)address);
            while (hex.Length < 8)
                hex = "0" + hex;
            //hex = "0x" + hex;
            return hex;
        }

        /// <summary>
        /// Formats data into 00 00 00 00 00 00 00 00 : Sample Text.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string FormatData(byte[] data)
        {
            var sb = new StringBuilder();
            // Displaying X only per row.
            var length = Math.Min(data.Length, MAX_COLS_PER_ROW);
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

        /// <summary>
        /// Same as FormatData, but with multiline support.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="baseaddress"></param>
        /// <returns></returns>
        public string FormatMultiLineData(byte[] data, IntPtr baseaddress)
        {
            var numrows = Math.Ceiling((double)data.Length / (double)MAX_COLS_PER_ROW);
            var sb = new StringBuilder();
            for (int i = 0; i < numrows; i++)
            {
                var startindex = i * MAX_COLS_PER_ROW;
                var addr = IntPtr.Add(baseaddress, startindex);
                var subdata = data.Skip(startindex).Take(MAX_COLS_PER_ROW).ToArray();
                sb.AppendFormat("{0}:{1}", FormatAddress(addr), FormatData(subdata));
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
