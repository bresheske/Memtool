using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Core.Extensions
{
    public static class FindSubEnumerable
    {
        public static int FindSub(this IEnumerable<byte> data, IEnumerable<byte> needle)
        {
            var foundindex = -1;
            for (int i = 0; i < data.Count(); i++)
            {
                var correct = 0;
                var startindex = i;
                var current = i;
                while ( current < data.Count() && correct < needle.Count() && data.ElementAt(current) == needle.ElementAt(correct))
                {
                    correct++;
                    current++;
                }
                if (correct == needle.Count())
                    foundindex = startindex;
            }

            return foundindex;
        }
    }
}
