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
    public class MemoryBufferTests
    {
        private IProcessMemoryBuffer buffer;
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
            buffer = new ProcessMemoryBuffer(process.Handle);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            buffer.Dispose();
            process.Kill();
        }

        [Test]
        public void TestBasicRead()
        {
            // Act
            var data = buffer.Read(256);

            // Assert data is not empty, and EOF.
            Assert.IsNotEmpty(data);
            Assert.IsTrue(buffer.EndOfStream);
        }
    }
}
