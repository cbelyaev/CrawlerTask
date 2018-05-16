using System;
using McMaster.Extensions.CommandLineUtils;

namespace Common
{
    public class Crawler
    {
        public static int Run(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption("-h|--help");
            var optionInput = app.Option<string>("-i|--input <file>", "Use <file> as an input file", CommandOptionType.SingleValue);
            var optionOutput = app.Option<string>("-o|--output <file>", "Write result to <file> file", CommandOptionType.SingleValue);
            var optionThreads = app.Option<int>("-j|--jobs <jobs>", "Use no more than <jobs> threads for execution", CommandOptionType.SingleValue);
            var optionVerbose = app.VerboseOption();

            app.OnExecute(async () =>
            {
                var wargs = new WorkerArgs();

                if (!optionInput.HasValue())
                {
                    Console.WriteLine("No input file");
                    app.ShowHelp();
                    return ExitCodes.InvalidArgument;
                }

                wargs.InputFileName = optionInput.ParsedValue;

                if (!optionOutput.HasValue())
                {
                    Console.WriteLine("No output file");
                    app.ShowHelp();
                    return ExitCodes.InvalidArgument;
                }

                wargs.OutputFileName = optionOutput.Value();

                wargs.MaxThreads = Environment.ProcessorCount;
                if (optionThreads.HasValue())
                {
                    var value = optionThreads.ParsedValue;
                    if (value < 0 || value > 1000)
                    {
                        Console.WriteLine("MaxThreads must be in range 0..1000");
                        app.ShowHelp();
                        return ExitCodes.InvalidArgument;
                    }

                    if (value > 0)
                        wargs.MaxThreads = value;
                }

                wargs.Verbose = optionVerbose.HasValue();

                return await new Worker(wargs).RunAsync();
            });

            return app.Execute(args);
        }
    }
}
