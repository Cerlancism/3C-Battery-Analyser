using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace _3C_Battery_Analyser
{
    class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<string>(
                    "--file",
                    getDefaultValue: () => "history.txt",
                    description: "History File"),
            };

            rootCommand.Description = "3C Battery history analyser";

            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<string>(Analyse);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }

        private static void Analyse(string file)
        {
            file = Path.GetFullPath(file);
            Console.WriteLine($"File: {file}");
        }
    }
}
