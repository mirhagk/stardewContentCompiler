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
                        quest.ItemID = int.Parse(typeSpecificInfo[0]);
                        quest.ItemIsBigItem = typeSpecificInfo[1] == "true";
                        break;
                    case "Location":
                        quest.TargetLocation = typeSpecificInfo[0];
                        break;
                    case "Building":
                        quest.CompletionText = typeSpecificInfo[0];
                        break;
                    case "Basic":
                        break;
                    case "Social":
                        break;
                    case "ItemDelivery":
                        quest.ItemDeliveryTarget = typeSpecificInfo[0];
                        quest.ItemID = int.Parse(typeSpecificInfo[1]);
                        quest.TargetMessage =  pieces[9];
                        if (typeSpecificInfo.Count() > 2)
                            quest.NumberOfItems = int.Parse(typeSpecificInfo[2]);
                        break;

                    case "Monster":
                        quest.MonsterNameToKill = typeSpecificInfo[0].Replace('_', ' ');
                        quest.MonsterNumberToKill = int.Parse(typeSpecificInfo[1]);
                        if (typeSpecificInfo.Count() > 2)
                            quest.TargetLocation = typeSpecificInfo[2];
                        else
                            quest.TargetLocation = "null";
                        break;
                    case "ItemHarvest":
                        quest.ItemID = int.Parse(typeSpecificInfo[0]);
                        quest.NumberOfItems = typeSpecificInfo.Length > 1 ? int.Parse(typeSpecificInfo[1]) : 1;
                        break;
                    case "LostItem":
                        quest.NPCName = typeSpecificInfo[0];
                        quest.TargetLocation = typeSpecificInfo[2];
                        quest.ItemID = int.Parse(typeSpecificInfo[1]);
                        quest.TileX = int.Parse(typeSpecificInfo[3]);
                        quest.TileY = int.Parse(typeSpecificInfo[4]);
                        break;
                    default:
                        Console.Error.WriteLine($"Could not understand quest type {quest.Type} of quest {quest.Name} with ID {pair.Key}");
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
        public bool? ItemIsBigItem { get; set; }
        public string TargetLocation { get; set; }
        public string CompletionText { get; set; }
        public string ItemDeliveryTarget { get; set; }
        public int? ItemID { get; set; }
        public string TargetMessage { get; set; }
        public int? NumberOfItems { get; set; }
        public string MonsterNameToKill { get; set; }
        public int? MonsterNumberToKill { get; set; }
        public string NPCName { get; set; }
        public int? TileX { get; set; }
        public int? TileY { get; set; }
    }
}
