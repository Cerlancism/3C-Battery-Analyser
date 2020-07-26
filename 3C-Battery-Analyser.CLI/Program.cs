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
            var rootCommand = new RootCommand
            {
                new Option<string>(
                    "--path",
                    getDefaultValue: () => Directory.GetCurrentDirectory(),
                    description: "Path of all the bmw_history files"
                ),
                new Option<Mode>(
                    "--mode",
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

            foreach (var item in files)
            {
                Console.WriteLine(item);
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
