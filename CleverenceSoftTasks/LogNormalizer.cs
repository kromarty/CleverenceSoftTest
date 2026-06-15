using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CleverenceSoftTasks
{
    public static class LogNormalizer
    {
        public static string Normalize(LogEntry entry)
        {
            string dateStr = entry.Date.ToString("dd-MM-yyyy");
            string timeStr = entry.Time.ToString(@"hh\:mm\:ss\.fff");
            return $"{dateStr}\t{timeStr}\t{entry.Level}\t{entry.Method}\t{entry.Message}";
        }
    }

    public class LogEntry
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Level { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
    }
    public interface ILogParser
    {
        bool TryParse(string line, out LogEntry entry);
    }

    public class Format1Parser : ILogParser
    {
        private static readonly Regex Regex = new Regex(
            @"^(?<date>\d{2}\.\d{2}\.\d{4})\s+(?<time>\d{2}:\d{2}:\d{2}\.\d{3})\s+(?<level>INFORMATION|WARNING|ERROR|DEBUG)\s+(?<message>.+)$"
        );

        public bool TryParse(string line, out LogEntry entry)
        {
            entry = null;
            var match = Regex.Match(line);
            if (!match.Success) return false;

            if (!DateTime.TryParseExact(match.Groups["date"].Value, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                return false;
            if (!TimeSpan.TryParse(match.Groups["time"].Value, out TimeSpan time))
                return false;

            entry = new LogEntry
            {
                Date = date,
                Time = time,
                Level = NormalizeLevel(match.Groups["level"].Value),
                Method = "DEFAULT",
                Message = match.Groups["message"].Value.Trim()
            };
            return true;
        }

        private string NormalizeLevel(string level) => level switch
        {
            "INFORMATION" => "INFO",
            "WARNING" => "WARN",
            _ => level
        };
    }

    public class Format2Parser : ILogParser
    {
        private static readonly Regex Regex = new Regex(
            @"^(?<date>\d{4}-\d{2}-\d{2})\s+(?<time>\d{2}:\d{2}:\d{2}\.\d+)\|(?<level>INFO|WARN|ERROR|DEBUG)\|(?<id>\d+)\|(?<method>.+?)\|\s*(?<message>.+)$"
        );

        public bool TryParse(string line, out LogEntry entry)
        {
            entry = null;
            var match = Regex.Match(line);
            if (!match.Success) return false;

            if (!DateTime.TryParseExact(match.Groups["date"].Value, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                return false;
            if (!TimeSpan.TryParse(match.Groups["time"].Value, out TimeSpan time))
                return false;

            entry = new LogEntry
            {
                Date = date,
                Time = time,
                Level = match.Groups["level"].Value,
                Method = match.Groups["method"].Value.Trim(),
                Message = match.Groups["message"].Value.Trim()
            };
            return true;
        }
    }

}
