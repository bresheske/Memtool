using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MemTool.Console.Input
{
    public class ListInput : BaseInput
    {
        public string ProcessNameSearch { get; set; }

        public override bool Validate()
        {
            // input params are optional for this action.
            return true;
        }

        public override void PerformAction()
        {
            var ps = string.IsNullOrWhiteSpace(ProcessNameSearch)
                    ? Process.GetProcesses()
                    : Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(ProcessNameSearch.ToLower()));
            foreach (var p in ps)
                System.Console.WriteLine("{0}:\t{1}", p.Id, p.ProcessName);
        }
    }
}
