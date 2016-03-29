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
            DecompileDialogue();
            DecompileTV();
            DecompileFestivals();
            DecompileEvents();
            DecompileQuests();
            DecompilePortraits();
            DecompileMonsters();
            DecompileFarmer();
            DecompileAnimals();
            DecompileBuildings();
            DecompileLooseSprites();
            DecompileLighting();
            DecompileMines();
            DecompileMinigames();
            DecompileTerrainFeatures();
            DecompileTileSheets();
            DecompileMisc();
        }
        void DecompileSchedules()
        {
            foreach (var asset in GetGameAssetsIn<Dictionary<string, string>>("characters\\schedules"))
                OutputToFile("characters\\schedules", asset.Filename, Schedule.Decompile(asset));
        }
        void DecompileQuests() =>
            OutputToFile("Data", "Quests", Content.Load<Dictionary<int, string>>("Data\\Quests"));

        void DecompileDialogue() => DecompileToJson("characters\\dialogue");
        void DecompileTV() => DecompileToJson("data\\tv");
        void DecompileFestivals() => DecompileToJson("data\\Festivals");
        void DecompileEvents() => DecompileToJson("data\\Events");
        void DecompilePortraits() => DecompileTextureFolder("portraits");
        void DecompileMonsters() => DecompileTextureFolder("characters\\monsters");
        void DecompileFarmer() => DecompileTextureFolder("characters\\farmer");
        void DecompileAnimals() => DecompileTextureFolder("animals");
        void DecompileBuildings() => DecompileTextureFolder("buildings");
        void DecompileLooseSprites() => DecompileTextureFolder("loosesprites");
        void DecompileLighting() => DecompileTextureFolder("loosesprites\\lighting");
        void DecompileMines() => DecompileTextureFolder("mines");
        void DecompileMinigames() => DecompileTextureFolder("minigames");
        void DecompileTerrainFeatures() => DecompileTextureFolder("TerrainFeatures");
        void DecompileTileSheets() => DecompileTextureFolder("TileSheets");
        void DecompileMisc() => DecompileTextureFolder("", "BloomCombine", "BloomExtract", "BrightWhite", "GaussianBlur");

        void DecompileToJson(string relativePath)
        {
            foreach (var asset in GetGameAssetsIn<Dictionary<string, string>>(relativePath))
            {
                File.WriteAllText(Path.Combine(Content.RootDirectory, relativePath, asset.Filename) + ".json", JsonConvert.SerializeObject(asset.Content, Formatting.Indented));
            }
        }
        void OutputToFile<T>(string relativePath, string filename, T data)
        {
            File.WriteAllText(Path.Combine(Content.RootDirectory, relativePath, filename) + ".json", JsonConvert.SerializeObject(data, Formatting.Indented));
        }
        void DecompileTextureFolder(string relativePath, params string[] except)
        {
            var graphics = ServiceContainer.GetService<GraphicsDevice>();
            using (var spriteBatch = new SpriteBatch(graphics))
                foreach (var asset in GetGameAssetsIn<Texture2D>(relativePath, except))
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
        IEnumerable<GameAsset<T>> GetGameAssetsIn<T>(string relativePath, string[] except = null)
        {
            except = except ?? new string[0];
            var items = Directory.EnumerateFiles(Path.Combine(Content.RootDirectory, relativePath)).Where(c => Path.GetExtension(c) == ".xnb").Select(c => Path.GetFileNameWithoutExtension(c)).Except(except);
            foreach (var item in items)
            {
                GameAsset<T> asset = null;
                try
                {
                    asset =  GameAsset.Create(item, Content.Load<T>(Path.Combine(relativePath, item)));
                }
                catch
                {
                    Console.Error.WriteLine($"Could not read {item} as {typeof(T).Name}");
                }
                if (asset != null)
                    yield return asset;
            }
        }
    }
}
