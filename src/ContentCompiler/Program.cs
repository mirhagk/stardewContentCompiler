using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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
        static Tuple<ContentManager,GameServiceContainer> SetupContentManager(string root)
        {
            var serviceContainer = new GameServiceContainer();
            var content = new ContentManager(serviceContainer, root);
            var graphicsDeviceManager = new GraphicsDeviceManager(new Game1());
            serviceContainer.AddService<IGraphicsDeviceService>(graphicsDeviceManager);
            graphicsDeviceManager.CreateDevice();
            serviceContainer.AddService(typeof(GraphicsDevice), graphicsDeviceManager.GraphicsDevice);
            return Tuple.Create(content, serviceContainer);
        }

        static void Decompile(string root)
        {
            var content = SetupContentManager(root);

            var decompiler = new Decompiler(content.Item1, content.Item2);
            decompiler.Decompile();
            content.Item1.Dispose();
        }
    }
    class Game1 : Game
    {
    }
}
