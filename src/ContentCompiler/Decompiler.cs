using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCompiler
{
    class Decompiler
    {
        ContentManager Content { get; }
        GameServiceContainer ServiceContainer { get; }
        public Decompiler(ContentManager content, GameServiceContainer serviceContainer)
        {
            Content = content;
            ServiceContainer = serviceContainer;
        }
        public void Decompile()
        {
            DecompileSchedules();
            DecompilePortraits();
            DecompileMonsters();
        }
        void DecompileSchedules()
        {
            foreach (var asset in GetGameAssetsIn<Dictionary<string, string>>("characters\\schedules"))
            {
                var schedule = Schedule.Decompile(asset);

                File.WriteAllText(Path.Combine(Content.RootDirectory, "characters\\schedules", asset.Filename) + ".json", JsonConvert.SerializeObject(schedule, Formatting.Indented));
            }
        }
        void DecompilePortraits() => DecompileTextureFolder("portraits");
        void DecompileMonsters() => DecompileTextureFolder("characters\\monsters");

        void DecompileTextureFolder(string relativePath)
        {
            GameServiceContainer serviceContainer = new GameServiceContainer();

            var graphics = serviceContainer.GetService<GraphicsDevice>();
            using (var spriteBatch = new SpriteBatch(graphics))
                foreach (var asset in GetGameAssetsIn<Texture2D>(relativePath))
                    using (var target = new RenderTarget2D(graphics, asset.Content.Width, asset.Content.Height))
                    {
                        graphics.SetRenderTarget(target);
                        spriteBatch.Begin();
                        spriteBatch.Draw(asset.Content, new Rectangle(0, 0, asset.Content.Width, asset.Content.Height), Color.White);
                        spriteBatch.End();
                        graphics.SetRenderTarget(null);

                        using (var stream = File.Create(Path.Combine(Content.RootDirectory, relativePath, asset.Filename) + ".png"))
                            target.SaveAsPng(stream, asset.Content.Width, asset.Content.Height);
                    }
        }
        IEnumerable<GameAsset<T>> GetGameAssetsIn<T>(string relativePath)
        {
            var items = Directory.EnumerateFiles(Path.Combine(Content.RootDirectory, relativePath)).Where(c => Path.GetExtension(c) == ".xnb").Select(c => Path.GetFileNameWithoutExtension(c));
            foreach (var item in items)
            {
                yield return GameAsset.Create(item, Content.Load<T>(Path.Combine(relativePath, item)));
            }
        }
    }
}
