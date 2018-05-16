using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using NLog;

namespace Common
{
    internal class Worker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly WorkerArgs _wargs;
        private int?[] _results;
        private string[] _urls;
        private int _done;
        private readonly object _lock = new object();

        public Worker(WorkerArgs wargs)
        {
            _wargs = wargs ?? throw new ArgumentNullException(nameof(wargs));
        }

        public async Task<int> RunAsync()
        {
            // read
            var urls = new List<string>();
            try
            {
                await Task.Run(() => ReadUrls(urls));
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, $"Cannot read file {_wargs.InputFileName}");
                return ExitCodes.FileReadError;
            }

            // prepare
            _urls = urls.ToArray();
            _results = new int?[_urls.Length];

            // process
            _done = 0;
            var opts = new ParallelOptions { MaxDegreeOfParallelism = _wargs.MaxThreads };
            Parallel.For(0, _urls.Length, opts, ProcessUrl);

            // save
            try
            {
                await Task.Run(() => WriteResults());
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, $"Cannot write file {_wargs.OutputFileName}");
                return ExitCodes.FileWriteError;
            }

            return ExitCodes.Ok;
        }

        private static bool Filter(string s)
        {
            return s.StartsWith("HTTP://", StringComparison.InvariantCultureIgnoreCase)
                   || s.StartsWith("HTTPS://", StringComparison.InvariantCultureIgnoreCase);
        }

        private void ProcessUrl(int i)
        {
            int? result = null;

            try
            {
                var parser = new HtmlParser();
                using (var client = new WebClient())
                {
                    var stream = client.OpenRead(_urls[i]);
                    var document = parser.Parse(stream);
                    result = document.Links.Select(l => l.GetAttribute("href")).Where(Filter).Count();
                }
            }
            catch (WebException ex)
            {
                Logger.Error($"Cannot get {_urls[i]}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, _urls[i]);
            }

            _results[i] = result;
            ShowProgress(i);
        }

        private void ShowProgress(int i)
        {
            lock (_lock)
            {
                _done++;
                var result = FormatResultString(i);

                // show progress
                Console.Write($"{_done}/{_urls.Length}");
                if (_wargs.Verbose)
                    Console.Write($": {FormatResultString(i)}");
                Console.WriteLine();

                // add result to file
                try
                {
                    File.AppendAllLines(_wargs.OutputFileName, new[] { result });
                }
                catch (Exception ex)
                {
                    Logger.Fatal(ex);
                    Environment.Exit(ExitCodes.FileWriteError);
                }
            }
        }

        private void ReadUrls(ICollection<string> urls)
        {
            using (var reader = File.OpenText(_wargs.InputFileName))
            {
                string line;
                while (null != (line = reader.ReadLine()))
                    urls.Add(line);
            }
        }

        private void WriteResults()
        {
            using (var writer = new StreamWriter(_wargs.OutputFileName))
                for (var i = 0; i < _results.Length; i++)
                    writer.WriteLine(FormatResultString(i));
        }

        private string FormatResultString(int i)
        {
            var result = _results[i].HasValue ? _results[i].Value.ToString() : "-";
            return $"{result}: {_urls[i]}";
        }
    }
}