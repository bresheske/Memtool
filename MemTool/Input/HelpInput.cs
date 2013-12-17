using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemTool.Console.Input
{
    public class HelpInput : BaseInput
    {
        public override bool Validate()
        {
            return true;
        }

        public override void PerformAction()
        {
            BaseInput.PrintHelp();
        }
    }
}
