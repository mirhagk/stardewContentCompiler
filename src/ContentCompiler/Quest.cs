using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCompiler
{
    class Quest
    {
        public static Dictionary<int,Quest> Decompile(Dictionary<int,string> asset)
        {
            Dictionary<int, Quest> questMap = new Dictionary<int, Quest>();
            foreach(var pair in asset)
            {
                var pieces = pair.Value.Split('/');
                questMap.Add(pair.Key, new Quest()
                {
                    Type = pieces[0],
                    Name = pieces[1],
                    Description = pieces[2],
                    Objective = pieces[3],
                });
            }
            return questMap;
        }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Objective { get; set; }
    }
}
