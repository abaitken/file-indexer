using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileIndexer
{
    internal class DirectoryContentsIndexer : IIndexProcessor
    {
        public IEnumerable<IndexedFile> Index(string searchPath, out IEnumerable<string> additionalSearchPaths)
        {
            var files = (from item in Directory.EnumerateFiles(searchPath)
                         select new IndexedFile
                         {
                             Path = item
                         }).ToList();

            var compressedFiles = PickCompressedFiles(files);
            var subDirectories = Directory.EnumerateDirectories(searchPath);

            additionalSearchPaths = subDirectories.Concat(compressedFiles);
            return files;
        }

        private static IEnumerable<string> PickCompressedFiles(List<IndexedFile> files)
        {
            foreach (var item in files)
            {
                if (ZipFileContentsIndexer.IsCompressedFile(item.Path))
                    yield return item.Path;
            }
        }

        public bool CanProcess(string searchPath)
        {
            var extension = Path.GetExtension(searchPath);

            if (string.IsNullOrEmpty(extension))
                return true;

            return Directory.Exists(searchPath);
        }
    }
}