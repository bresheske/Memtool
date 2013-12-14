using MemTool.Core.MemoryServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Test
{
    [TestFixture]
    public class MemoryReadingTests
    {
        private IMemoryReader memoryreader;
        private Process process;

        [TestFixtureSetUp]
        public void Init()
        {
            // Startup a test process.
            var info = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = @"..\..\..\MemTool.Test.Console\bin\Debug\MemTool.Test.Console.exe"
            };
            process = Process.Start(info);
            memoryreader = new MemoryReader(process.Handle);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            process.Kill();
        }

        [Test]
        public void TestFind()
        {
            // Act
            var position = memoryreader.Find("public data");

            // Assert the position is in memory.
            Assert.IsNotNull(position);
        }
    }
}
