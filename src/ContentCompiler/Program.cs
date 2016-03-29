﻿using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ContentCompiler
{
    class Program
    {
        class Arguments
        {
            public bool Decompile { get; set; }
            [PowerCommandParser.Position(0)]
            public string ContentRoot { get; set; } = @"C:\Program Files(x86)\steam\steamapps\common\Stardew Valley\Content";
        }
        static void Main(string[] rawArgs)
        {
            var args = PowerCommandParser.Parser.ParseArguments<Arguments>(rawArgs);
            if (args.Decompile)
            {
                Decompile(args.ContentRoot);
                DecompilePortraits(args.ContentRoot);
            }
        }
        static ContentManager SetupContentManager(string root)
        {

            var serviceContainer = new Microsoft.Xna.Framework.GameServiceContainer();
            var content = new Microsoft.Xna.Framework.Content.ContentManager(serviceContainer, root);
            var graphicsDeviceManager = new Microsoft.Xna.Framework.GraphicsDeviceManager(new Game1());
            serviceContainer.AddService<Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService>(graphicsDeviceManager);
            graphicsDeviceManager.CreateDevice();
            serviceContainer.AddService(typeof(GraphicsDevice), graphicsDeviceManager.GraphicsDevice);
            return content;
        }

        static void Decompile(string root)
        {
            var content = SetupContentManager(root);

            var characters = Directory.EnumerateFiles(root + "\\characters\\schedules").Where(c => Path.GetExtension(c) == ".xnb").Select(c => Path.GetFileNameWithoutExtension(c));
            foreach (var character in characters)
            {
                var scheduleRaw = content.Load<Dictionary<string, string>>("characters\\schedules\\" + character);
                var schedule = new Schedule() { Character = character };
                foreach(var keyPair in scheduleRaw)
                {
                    if (keyPair.Key.EndsWith("_Replacement"))
                        continue;
                    var value = keyPair.Value;
                    schedule.ScheduledItems.Add(new Schedule.ScheduleItem()
                    {
                        Key = keyPair.Key
                    });
                    if (value.StartsWith("NOT"))
                    {
                        var pieces = value.Split('/')[0].Split(' ');
                        schedule.ScheduledItems.Last().Condition = new Schedule.ScheduleItem.NotCondition()
                        {
                            Type = pieces[1],
                            Name = pieces[2],
                            Level = int.Parse(pieces[3]),
                        };
                        value = string.Join("/", value.Split('/').Skip(1));
                    }
                    if (value.StartsWith("GOTO"))
                    {
                        schedule.ScheduledItems.Last().GotoKey = keyPair.Value.Substring(5);
                        continue;
                    }
                    var items = value.Split('/');
                    foreach(var item in items)
                    {
                        var pieces = item.Split(' ');
                        schedule.ScheduledItems.Last().TargetLocations.Add(new Schedule.ScheduleItem.TargetLocationTime()
                        {
                            Time = int.Parse(pieces[0]),
                            Location = pieces[1],
                            X = int.Parse(pieces[2]),
                            Y = int.Parse(pieces[3]),
                            Direction = GetInt(pieces[4])??2,
                        });
                    }
                    Console.WriteLine(keyPair.Key);
                    Console.WriteLine(keyPair.Value);
                }
                File.WriteAllText(Path.Combine(root, "characters\\schedules", character) + ".json", JsonConvert.SerializeObject(schedule, Formatting.Indented));
            }

            //content.Load<Dictionary<string,string>>("characters\\schedules\\leah").Dump();
        }

        static void DecompilePortraits(string root)
        {
            using (var content = SetupContentManager(root))
            {
                var portraits = Directory.EnumerateFiles(root + "\\Portraits").Where(c => Path.GetExtension(c) == ".xnb").Select(c => Path.GetFileNameWithoutExtension(c));
                foreach (var portrait in portraits)
                {
                    var texture = content.Load<Texture2D>("Portraits\\" + portrait);

                    using (var stream = File.Create(root + "\\Portraits\\" + portrait + ".png"))
                    {
                        texture.SaveAsPng(stream, texture.Width, texture.Height);
                    }
                }
            }
        }

        static int? GetInt(string value)
        {
            int result;
            if (int.TryParse(value, out result))
                return result;
            return null;
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
                public int Time { get; set; }
                public string Location { get; set; }
                public int X { get; set; }
                public int Y { get; set; }
                public int Direction { get; set; }

            }
            public class NotCondition
            {
                public string Type { get; set; }
                public string Name { get; set; }
                public int Level { get; set; }
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
