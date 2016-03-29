using Microsoft.Xna.Framework.Content;
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
            var content = new ContentManager(serviceContainer, root);
            var graphicsDeviceManager = new Microsoft.Xna.Framework.GraphicsDeviceManager(new Game1());
            serviceContainer.AddService<IGraphicsDeviceService>(graphicsDeviceManager);
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

                var schedule = Schedule.Decompile(scheduleRaw, character);

                File.WriteAllText(Path.Combine(root, "characters\\schedules", character) + ".json", JsonConvert.SerializeObject(schedule, Formatting.Indented));
            }
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
    }
    class Game1 : Microsoft.Xna.Framework.Game
    {
    }
}
