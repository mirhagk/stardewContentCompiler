using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Decompile();
        }
        static ContentManager SetupContentManager(string root)
        {

            var serviceContainer = new Microsoft.Xna.Framework.GameServiceContainer();
            var content = new Microsoft.Xna.Framework.Content.ContentManager(serviceContainer, root);
            var graphicsDevice = new Microsoft.Xna.Framework.GraphicsDeviceManager(new Game1());
            serviceContainer.AddService<Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService>(graphicsDevice);
            return content;
        }
        static void Decompile()
        {
            var root = @"F:\steam\steamapps\common\Stardew Valley\Content";
            var content = SetupContentManager(root);

            var characters = Directory.EnumerateFiles(root + "\\characters\\schedules").Where(c=>Path.GetExtension(c)==".xnb").Select(c=>Path.GetFileNameWithoutExtension(c));
            foreach (var character in characters)
            {
                var scheduleRaw = content.Load<Dictionary<string, string>>("characters\\schedules\\" + character);
                var json = JsonConvert.SerializeObject(scheduleRaw, Formatting.Indented);
                File.WriteAllText(Path.Combine(root, "characters\\schedules", character) + ".json", json);
                var schedule = new Schedule() { Character = character };
                foreach(var keyPair in scheduleRaw)
                {
                    schedule.ScheduledItems.Add(new Schedule.ScheduleItem()
                    {
                        Key = keyPair.Key
                    });
                    if (keyPair.Value.StartsWith("GOTO"))
                    {
                        schedule.ScheduledItems.Last().GotoKey = keyPair.Value.Substring(5);
                        continue;
                    }
                    else if (keyPair.Value.StartsWith("NOT"))
                    {

                    }
                    Console.WriteLine(keyPair.Key);
                    Console.WriteLine(keyPair.Value);
                }
                File.WriteAllText(Path.Combine(root, "characters\\schedules", character) + ".src.json", JsonConvert.SerializeObject(schedule, Formatting.Indented));
            }

            //content.Load<Dictionary<string,string>>("characters\\schedules\\leah").Dump();
        }
    }
    class Game1 : Microsoft.Xna.Framework.Game
    {
    }
    class Schedule
    {
        public class ScheduleItem
        {
            public class TargetLocationTime
            {
                public string Time { get; set; }
                public string Location { get; set; }
                public int X { get; set; }
                public int Y { get; set; }
                public int Direction { get; set; }

            }
            public class NotCondition
            {

            }
            public string Key { get; set; }
            public List<TargetLocationTime> TargetLocations { get; set; } = new List<TargetLocationTime>();
            public string GotoKey { get; set; }
            public NotCondition Condition { get; set; }
        }
        public string Character { get; set; }
        public List<ScheduleItem> ScheduledItems { get; set; } = new List<ScheduleItem>();
    }
}
