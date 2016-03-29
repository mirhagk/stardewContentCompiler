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
            using (var content = SetupContentManager(root))
            {
                var decompiler = new Decompiler();
                decompiler.Decompile(content);
            }
        }
    }
    class Game1 : Microsoft.Xna.Framework.Game
    {
    }
}
