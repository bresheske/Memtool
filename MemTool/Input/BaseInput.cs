using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Console.Input
{
    public abstract class BaseInput
    {
        private static bool list = false;
        private static bool searchmem = false;
        private static bool verbose = false;
        private static bool readmem = false;
        private static bool writemem = false;

        private static bool usestring = false;
        private static bool useint = false;
        private static bool usebyte = false;
        private static string usedata = "";

        private static string procid = "";
        private static bool showhelp = false;
        private static string address = "";
        private static string length = "";
        private static string enc = "";

        private static OptionSet options;

        public static BaseInput ParseInput(string[] args)
        {
            // Use NDesk to parse out args.
            options = new OptionSet()
            {
                {"l", "List Processes", x => list = true},
                {"v", "Verbose On", x => {MemTool.Core.Verbose.OutputStream = System.Console.Out; verbose = true;}},
                {"sm", "Search Memory", x => searchmem = true},
                {"rm", "Read Memory", x => readmem = true},
                {"wm", "Write Memory", x => writemem = true},

                {"addr=", "Address to Read/Write", x => address = x},
                {"len=", "Length to Read", x => length = x},
                {"pid=", "Process ID", x =>  procid = x},
                {"enc=", "Encoding. (uni, def)", x =>  enc = x},

                {"string", "Use a String for Write/Search", x => usestring = true},
                {"int", "Use an Int for Write/Search", x => useint = true},
                {"byte", "Use a Byte for Write/Search", x => usebyte = true},
                {"d=", "Data to Write/Search", x => usedata = x},

                {"h", "Show Help", x => showhelp = true}
            };
            options.Parse(args);
            
            // Build our input.
            var encoding = System.Text.Encoding.Default;
            if (enc == "uni")
                encoding = System.Text.Encoding.Unicode;
            byte[] data;
            if (useint)
                data = IntToBytes(int.Parse(usedata));
            else if (usebyte)
                data = ConvertHexStringToByteArray(usedata);
            else
                data = encoding.GetBytes(usedata);

            // Type of input.
            BaseInput input = null;
            if (list)
                input = new ListInput()
                {
                    ProcessNameSearch = usedata
                };
            else if (searchmem)
                input = new SearchInput()
                {
                    Data = data,
                    Encoding = encoding,
                    Verbose = verbose,
                    ProcessId = procid
                };
            else if (readmem)
                input = new ReadInput()
                {
                    Address = address,
                    Encoding = encoding,
                    Length = length,
                    ProcessId = procid
                };
            else if (writemem)
                input = new WriteInput()
                {
                    Address = address,
                    Data = data,
                    ProcessId = procid
                };

            if (input == null || !input.Validate())
                input = new HelpInput();

            return input;
        }

        public static void PrintHelp()
        {
            options.WriteOptionDescriptions(System.Console.Out);
        }

        public abstract bool Validate();
        public abstract void PerformAction();

        /// <summary>
        /// Small function I found somewhere in StackOverflow.
        /// </summary>
        /// <param name="integer"></param>
        /// <returns></returns>
        private static byte[] IntToBytes(int integer)
        {
            byte[] intBytes = BitConverter.GetBytes(integer);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/321370/convert-hex-string-to-byte-array
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }
    }

}
