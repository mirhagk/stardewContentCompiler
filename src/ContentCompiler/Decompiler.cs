﻿using Microsoft.Xna.Framework;
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
		string OutputPath { get; }

        public Decompiler(ContentManager content, GameServiceContainer serviceContainer, string outputPath)
        {
            Content = content;
            ServiceContainer = serviceContainer;
			OutputPath = outputPath;

			if (Directory.Exists(outputPath) == false)
			{
				Directory.CreateDirectory(outputPath);
			}
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
			foreach (var asset in GetGameAssetsIn<Dictionary<string, string>>(Path.Combine("Characters", "Schedules")))
				OutputToFile(Path.Combine("Characters", "Schedules"), asset.Filename, Schedule.Decompile(asset));
        }
        void DecompileQuests()
        {
			var quests = Quest.Decompile(Content.Load<Dictionary<int, string>>(Path.Combine("Data", "Quests")));
            OutputToFile("Data", "Quests", quests);
        }

		void DecompileDialogue() => DecompileToJson(Path.Combine("characters", "dialogue"));
		void DecompileTV() => DecompileToJson(Path.Combine("data", "tv"));
		void DecompileFestivals() => DecompileToJson(Path.Combine("data", "Festivals"));
		void DecompileEvents() => DecompileToJson(Path.Combine("data", "Events"));
        void DecompilePortraits() => DecompileTextureFolder("portraits");
		void DecompileMonsters() => DecompileTextureFolder(Path.Combine("characters", "monsters"));
		void DecompileFarmer() => DecompileTextureFolder(Path.Combine("characters", "farmer"));
        void DecompileAnimals() => DecompileTextureFolder("animals");
        void DecompileBuildings() => DecompileTextureFolder("buildings");
        void DecompileLooseSprites() => DecompileTextureFolder("loosesprites");
		void DecompileLighting() => DecompileTextureFolder(Path.Combine("loosesprites", "lighting"));
        void DecompileMines() => DecompileTextureFolder("mines");
        void DecompileMinigames() => DecompileTextureFolder("minigames");
        void DecompileTerrainFeatures() => DecompileTextureFolder("TerrainFeatures");
        void DecompileTileSheets() => DecompileTextureFolder("TileSheets");
        void DecompileMisc() => DecompileTextureFolder("", "BloomCombine", "BloomExtract", "BrightWhite", "GaussianBlur");

        void DecompileToJson(string relativePath)
        {
            foreach (var asset in GetGameAssetsIn<Dictionary<string, string>>(relativePath))
                OutputToFile(relativePath, asset.Filename, asset.Content);
        }
        void OutputToFile<T>(string relativePath, string filename, T data)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            };
			string tempPath = Path.Combine(OutputPath, relativePath);
			if (Directory.Exists(tempPath) == false)
			{
				Directory.CreateDirectory(tempPath);
			}
			using (var writer = new StreamWriter(Path.Combine(tempPath, filename) + ".json"))
                serializer.Serialize(writer, data);
        }
        void DecompileTextureFolder(string relativePath, params string[] except)
        {
            var graphics = ServiceContainer.GetService<GraphicsDevice>();
            using (var spriteBatch = new SpriteBatch(graphics))
                foreach (var asset in GetGameAssetsIn<Texture2D>(relativePath, except))
					using (var target = new RenderTarget2D(graphics, asset.Content.Width, asset.Content.Height))
                    {
                        graphics.SetRenderTarget(target);
						spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                        spriteBatch.Draw(asset.Content, new Rectangle(0, 0, asset.Content.Width, asset.Content.Height), Color.White);
                        spriteBatch.End();
                        graphics.SetRenderTarget(null);

						string tempPath = Path.Combine(OutputPath, relativePath);
						if (Directory.Exists(tempPath) == false)
						{
							Directory.CreateDirectory(tempPath);
						}
						using (var stream = File.Create(Path.Combine(tempPath, asset.Filename) + ".png"))
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
