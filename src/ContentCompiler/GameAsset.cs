using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCompiler
{
    static class GameAsset
    {
        public static GameAsset<T> Create<T>(string filename, T content)
        {
            return new GameAsset<T> { Filename = filename, Content = content };
        }
    }
    class GameAsset<T>
    {
        public string Filename { get; set; }
        public T Content { get; set; }
    }
}
