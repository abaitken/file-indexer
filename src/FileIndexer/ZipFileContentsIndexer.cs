using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileIndexer
{
    internal class ZipFileContentsIndexer : IIndexProcessor
    {
        private readonly Regex _lineParse = new Regex(@"^(?<DATE>\d+-\d+-\d+)\s+(?<TIME>\d+:\d+:\d+)\s+(?<ATTR>.+?)\s+\d+\s+\d+\s+(?<FILE>.+?)\s*$", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly string[] _extensions = new[] { ".zip", ".gz", ".7z" };
        private readonly string _sevenZipPath;

        public ZipFileContentsIndexer(string sevenZipPath)
        {
            this._sevenZipPath = sevenZipPath;
        }

        public bool CanProcess(string searchPath)
        {
            return IsCompressedFile(searchPath);
        }

        public static bool IsCompressedFile(string file)
        {
            var extension = Path.GetExtension(file).ToLower();
            return _extensions.Contains(extension);
        }

        public IEnumerable<IndexedFile> Index(string searchPath, out IEnumerable<string> additionalSearchPaths)
        {
            additionalSearchPaths = Enumerable.Empty<string>();
            return from item in CompressedFileContents(searchPath)
                   select new IndexedFile
                   {
                       Path = Path.Combine(searchPath, item)
                   };
        }

        private IEnumerable<string> CompressedFileContents(string file)
        {
            var output = ExecuteSevenZipFileList(file);
            var files = _lineParse.Matches(output);

            foreach (Match item in files)
            {
                var attr = item.Groups["ATTR"].Value;
                if (attr.StartsWith("D"))
                    continue;

                yield return item.Groups["FILE"].Value;
            }
        }
        private string ExecuteSevenZipFileList(string file)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo(_sevenZipPath, $@"l -ba ""{file}""")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            return output;
        }
    }
}