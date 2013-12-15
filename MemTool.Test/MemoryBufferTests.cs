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
            buffer = new ProcessMemoryBuffer(process);
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
            var data = buffer.Read(1);

            // Assert data is not empty.
            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);
            Assert.IsFalse(buffer.EndOfStream);

            // Act
            data = buffer.Read(4095);

            // Assert not end of stream.
            Assert.IsFalse(buffer.EndOfStream);

            // Act
            data = buffer.Read(1);

            // Assert end of stream.
            Assert.IsTrue(buffer.EndOfStream);
        }

        [Test]
        public void TestOneByOneRead()
        {
            // Act & Assert
            for (int i = 0; i < 4096; i++)
            {
                var data = buffer.Read(1);
                Assert.IsTrue(buffer.EndOfStream == false);
            }

            // Act
            var d = buffer.Read(1);
            
            // Assert EOS
            Assert.IsTrue(buffer.EndOfStream);
        }
    }
}