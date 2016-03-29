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
                var quest = new Quest()
                {
                    Type = pieces[0],
                    Name = pieces[1],
                    Description = pieces[2],
                    Objective = pieces[3],
                };
                var typeSpecificInfo = pieces[4].Split(' ');
                switch (quest.Type)
                {
                    case "Crafting":
                        quest.ItemIDToCraft = int.Parse(typeSpecificInfo[0]);
                        quest.ItemIsBigItem = typeSpecificInfo[1] == "true";
                        break;
                    default:
                        break;
                }
                questMap.Add(pair.Key, quest);
            }
            return questMap;
        }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Objective { get; set; }
        public int ItemIDToCraft { get; set; }
        public bool ItemIsBigItem { get; set; }
    }
}
