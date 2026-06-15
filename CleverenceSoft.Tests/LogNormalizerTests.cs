using CleverenceSoftTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverenceSoft.Tests
{
    internal class LogNormalizerTests
    {
        [Test]
        public void Format1Parser_ValidLine_ReturnsEntry()
        {
            string line = "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'";
            var parser = new Format1Parser();
            bool result = parser.TryParse(line, out LogEntry entry);
            Assert.IsTrue(result);
            Assert.AreEqual(new DateTime(2025, 3, 10), entry.Date);
            Assert.AreEqual("INFO", entry.Level);
            Assert.AreEqual("DEFAULT", entry.Method);
            Assert.AreEqual("Версия программы: '3.4.0.48729'", entry.Message);
        }

        [Test]
        public void Format2Parser_ValidLine_ReturnsEntry()
        {
            string line = "2025-03-10 15:14:51.5882|INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";
            var parser = new Format2Parser();
            bool result = parser.TryParse(line, out LogEntry entry);
            Assert.IsTrue(result);
            Assert.AreEqual(new DateTime(2025, 3, 10), entry.Date);
            Assert.AreEqual("INFO", entry.Level);
            Assert.AreEqual("MobileComputer.GetDeviceId", entry.Method);
            Assert.AreEqual("Код устройства: '@MINDEO-M40-D-410244015546'", entry.Message);
        }

        [Test]
        public void LogNormalizer_ProducesCorrectOutput()
        {
            var entry = new LogEntry
            {
                Date = new DateTime(2025, 3, 10),
                Time = TimeSpan.Parse("15:14:49.523"),
                Level = "INFO",
                Method = "DEFAULT",
                Message = "Test message"
            };
            string normalized = LogNormalizer.Normalize(entry);
            Assert.AreEqual("10-03-2025\t15:14:49.523\tINFO\tDEFAULT\tTest message", normalized);
        }
    }
}
