using System.Collections.Generic;

namespace FileIndexer
{
    internal interface IIndexProcessor
    {
        IEnumerable<IndexedFile> Index(string searchPath, out IEnumerable<string> additionalSearchPaths);
        bool CanProcess(string searchPath);
    }
}