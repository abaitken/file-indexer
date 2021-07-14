using System.Collections.Generic;
using System.Linq;

namespace FileIndexer
{
    class CommandLineParser
    {
        private readonly string[] _args;

        public CommandLineParser(string[] args)
        {
            _args = args;
        }

        public string this[string name] => Many(name).FirstOrDefault();


        public IEnumerable<string> Many(string name)
        {
            for (int i = 0; i < _args.Length; i++)
            {
                var item = _args[i];

                if (item.Equals("-" + name) || item.Equals("/" + name))
                {
                    i++;
                    if (i == _args.Length)
                        continue;

                    yield return _args[i];
                }


                if (item.StartsWith("-" + name) || item.StartsWith("/" + name))
                {
                    var seperator = item.Substring(name.Length + 1)[0];
                    if (seperator == ' ' || seperator == ':' || seperator == '=')
                        yield return item.Substring(name.Length + 2);
                }
            }
        }
    }
}
