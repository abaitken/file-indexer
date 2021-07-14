using System;

namespace FileIndexer
{
    class ConsoleFloodProtection
    {
        private DateTime lastUpdate;

        public void WriteLine(string text)
        {
            if (DateTime.Now - lastUpdate < TimeSpan.FromMilliseconds(200))
                return;

            lastUpdate = DateTime.Now;
            Console.WriteLine(text);
        }
    }
}
