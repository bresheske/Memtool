using MemTool.Console.Input;
using MemTool.Core;
using MemTool.Core.MemoryServices;
using Microsoft.Practices.Unity;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Console
{
    class Program
    {
        static void InitContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IMemoryService, DefaultMemoryService>();
            container.RegisterType<IMemoryFormatter, DefaultMemoryFormatter>();
            MemTool.Core.DependencyResolver.Container = container;
        }

        static void Main(string[] args)
        {
            // Set up dependency resolution first.
            InitContainer();

            // Parse out our action
            var action = BaseInput.ParseInput(args);
            
            // Perform action.
            action.PerformAction();
        }
    }
}
