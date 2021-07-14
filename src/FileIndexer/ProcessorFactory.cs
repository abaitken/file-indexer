using System.Collections.Generic;

namespace FileIndexer
{
    class ProcessorFactory
    {
        private readonly List<IIndexProcessor> _processors;

        public ProcessorFactory()
        {
            _processors = new List<IIndexProcessor> 
            {
                new DirectoryContentsIndexer()
            };
        }

        public void AddCompressedFileIndexing(string sevenZipPath)
        {
            _processors.Add(new ZipFileContentsIndexer(sevenZipPath));
        }

        public IIndexProcessor Select(string searchPath)
        {
            foreach (var item in _processors)
            {
                if (item.CanProcess(searchPath))
                    return item;
            }

            return null;
        }

    }
}
