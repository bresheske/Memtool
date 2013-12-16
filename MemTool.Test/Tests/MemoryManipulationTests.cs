using MemTool.Core.MemoryServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTool.Test.Tests
{
    [TestFixture]
    public class MemoryManipulationTests
    {
        Process testproc;
        DefaultMemoryService service; 

        [TestFixtureSetUp]
        public void Init()
        {
            // Arrange a bit, set up a process.
            var p = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                //WindowStyle = ProcessWindowStyle.Hidden,
                FileName = @"..\..\..\MemTool.Test.Console\bin\debug\MemTool.Test.Console.exe"
            };
            testproc = Process.Start(p);
            service = new DefaultMemoryService(new DefaultMemoryFormatter());
        }

        [TestFixtureTearDown]
        public void Tear()
        {
            testproc.Kill();
        }

        [Test]
        public void TestOpenProcess()
        {
            // Act
            var handle = service.OpenProcess(testproc.Id);

            // Assert
            Assert.IsTrue((int)handle > 0);
        }

        [Test]
        public void TestOpenFalseProcess()
        {
            // Act
            var handle = service.OpenProcess(0000);

            // Assert
            Assert.IsTrue((int)handle == 0);
        }

        [Test]
        public void TestReadMemory()
        {
            // Arrange
            var handle = service.OpenProcess(testproc.Id);
            var addr = service.FindData(handle, Encoding.Unicode.GetBytes("private data"), Encoding.Unicode);

            // Act
            var bytes = service.ReadMemory(handle, addr.Last(), 24);

            // Assert
            Assert.IsNotEmpty(bytes);
            Assert.AreEqual("private data", Encoding.Unicode.GetString(bytes));
        }
    }
}
