using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;

using _3C_Battery_Analyser.Core;

namespace _3C_Battery_Analyser.CLI
{
    enum Mode
    {
        Plain,
        CSV
    }

    class Program
    {
        static int Main(string[] args)
        {
#if DEBUG
            args = new[] { "--path", @"D:\Shared\battery-monitor-data", "--mode", "plain" };
#endif
            var rootCommand = new RootCommand
            {
                new Option<string>(
                    new [] { "-p", "--path" },
                    getDefaultValue: () => Directory.GetCurrentDirectory(),
                    description: "Path of all the bmw_history files"
                ),
                new Option<Mode>(
                    new [] { "-m", "--mode" },
                    getDefaultValue: () => Mode.Plain,
                    description: "Type of output"
                ),
            };

            rootCommand.Description = "3C Battery history analyser";

            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<string, Mode>(Analyse);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }

        private static void Analyse(string path, Mode mode)
        {
            var files = Directory.GetFiles(Path.GetFullPath(path), "*.txt");

            static (bool success, string file, BatteryHistory firstHistory) selector(string x)
            {
                if (BatteryHistory.TryParse(File.ReadLines(x).FirstOrDefault(), out BatteryHistory result))
                {
                    return (success: true, file: x, firstHistory: result);
                }
                return (success: false, default, default);
            }

            var targets = files.Select(selector)
                .Where(x => x.success)
                .OrderByDescending(x => x.firstHistory.Date)
                .Select(x => x.file);

            foreach (var item in targets)
            {
                AnalyseFile(item, mode);
            }
        }

        private static void AnalyseFile(string file, Mode mode)
        {
            file = Path.GetFullPath(file);

            var allHistory = File.ReadLines(file).Select(BatteryHistory.Parse);
            var cycles = ChargeCycle.EnumerateChargeCycles(allHistory);

            if (mode == Mode.Plain)
            {
                Console.WriteLine($"{file}");

                foreach (var item in cycles)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                foreach (var item in cycles)
                {
                    Console.WriteLine(item.ToCSVString());
                }
            }
        }
    }
}
