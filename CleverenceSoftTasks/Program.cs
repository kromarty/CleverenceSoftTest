using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverenceSoftTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: LogNormalizer <inputFile> [outputFile]");
                return;
            }

            string inputPath = args[0];
            string outputPath = args.Length > 1 ? args[1] : "output.txt";
            string problemsPath = "problems.txt";

            var parsers = new List<ILogParser> { new Format1Parser(), new Format2Parser() };

            using (var reader = new StreamReader(inputPath, Encoding.UTF8))
            using (var goodWriter = new StreamWriter(outputPath, false, Encoding.UTF8))
            using (var badWriter = new StreamWriter(problemsPath, false, Encoding.UTF8))
            {
                string line;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    bool parsed = false;
                    foreach (var parser in parsers)
                    {
                        if (parser.TryParse(line, out LogEntry entry))
                        {
                            string normalized = LogNormalizer.Normalize(entry);
                            goodWriter.WriteLine(normalized);
                            parsed = true;
                            break;
                        }
                    }
                    if (!parsed)
                    {
                        badWriter.WriteLine(line);
                        Console.WriteLine($"Warning: line {lineNumber} is invalid, written to {problemsPath}");
                    }
                }
            }

            Console.WriteLine("Processing completed.");
        }
    }
}
