using Microsoft.Xna.Framework.Content;
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

            var characters = Directory.EnumerateFiles(root + "\\characters\\schedules").Select(c=>Path.GetFileNameWithoutExtension(c));
            foreach (var character in characters)
            {
                var schedule = content.Load<Dictionary<string, string>>("characters\\schedules\\" + character);
                foreach(var keyPair in schedule)
                {
                    Console.WriteLine(keyPair.Key);
                    Console.WriteLine(keyPair.Value);
                }
            }

            //content.Load<Dictionary<string,string>>("characters\\schedules\\leah").Dump();
        }
    }
    class Game1 : Microsoft.Xna.Framework.Game
    {
    }
}
