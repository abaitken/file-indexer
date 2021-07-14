using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileIndexer
{
    class App
    {
        public void Run(string[] args)
        {
            if (args.Length == 0)
            {
                DisplayHelp();
                return;
            }
            var commandLineParser = new CommandLineParser(args);

            var paths = commandLineParser.Many("i").ToList();
            if (paths.Count == 0)
            {
                Console.WriteLine("No paths provided");
                return;
            }

            if (!ValidatePaths(paths))
                return;

            var dbFile = commandLineParser["db"];
            if (string.IsNullOrEmpty(dbFile))
            {
                Console.WriteLine("No database file was specified");
                return;
            }

            var sevenZipPath = commandLineParser["7z"];
            if (!string.IsNullOrEmpty(sevenZipPath) && !File.Exists(sevenZipPath))
            {
                Console.WriteLine($"Given 7-zip path '{sevenZipPath}' not found");
                return;
            }

            var db = new DataContext($"Data Source={dbFile}");
            db.InitialiseAndUpgrade();

            var processorFactory = new ProcessorFactory();
            if (!string.IsNullOrEmpty(sevenZipPath))
                processorFactory.AddCompressedFileIndexing(sevenZipPath);

            var searchPaths = new Queue<string>();
            searchPaths.Enqueue(paths);
            var processed = 0;

            var console = new ConsoleFloodProtection();
            var start = DateTime.Now;
            while (searchPaths.Count != 0)
            {
                console.WriteLine($"{searchPaths.Count} search paths remaining. {processed} processed. {DateTime.Now - start}");
                var searchPath = searchPaths.Dequeue();

                var matchingPaths = from item in db.IndexedFiles
                                    where item.Path.Equals(searchPath)
                                    select item;

                var processor = processorFactory.Select(searchPath);
                if (processor.GetType() == typeof(ZipFileContentsIndexer))
                    continue;

                var files = processor.Index(searchPath, out var additionalSearchPaths);

                if (!matchingPaths.Any())
                {
                    db.IndexedFiles.AddRange(files);
                    db.SaveChanges();
                }

                searchPaths.Enqueue(additionalSearchPaths);
                processed++;
            }
        }

        private bool ValidatePaths(List<string> paths)
        {
            var result = true;
            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"Directpry '{path}' not found");
                    result = false;
                }
            }

            return result;
        }

        private void DisplayHelp()
        {
            Console.WriteLine(@"
-db <filename>        SQLLite database
-i <directory>        (Multiple) Locations to index
-7z <7z.exe path>     Index inside compressed files (zip, 7z etc)
");
        }
    }
}
