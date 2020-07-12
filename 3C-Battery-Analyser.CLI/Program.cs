using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

using _3C_Battery_Analyser.Core;

namespace _3C_Battery_Analyser.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
#if DEBUG
            Analyse(@"D:\Shared\battery-monitor-data\bmw_history_202007.txt");
            Console.ReadLine();
            return 0;
#else
            var rootCommand = new RootCommand
            {
                new Option<string>(
                    "--file",
                    getDefaultValue: () => "bmw_history.txt",
                    description: "History File"),
            };

            rootCommand.Description = "3C Battery history analyser";

            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<string>(Analyse);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
#endif
        }

        private static void Analyse(string file)
        {
            file = Path.GetFullPath(file);
            Console.WriteLine($"File: {file}");

            var data = File.ReadLines(file).Select(BatteryHistory.Parse);

            foreach (var item in data)
            {
                Console.WriteLine(item);
            }
        }
    }
}
