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
        public void Decompile(ContentManager content)
        {
            DecompileSchedules(content);
            DecompilePortraits(content);
        }
        void DecompileSchedules(ContentManager content)
        {
            foreach (var asset in GetGameAssetsIn<Dictionary<string, string>>(content, "characters\\schedules"))
            {
                var schedule = Schedule.Decompile(asset);

                File.WriteAllText(Path.Combine(content.RootDirectory, "characters\\schedules", asset.Filename) + ".json", JsonConvert.SerializeObject(schedule, Formatting.Indented));
            }
        }
        void DecompilePortraits(ContentManager content) => DecompileTextureFolder(content, "portraits");
        
        void DecompileTextureFolder(ContentManager content, string relativePath)
        {
            foreach (var asset in GetGameAssetsIn<Texture2D>(content, relativePath))
                using (var stream = File.Create(Path.Combine(content.RootDirectory, relativePath, asset.Filename) + ".png"))
                    asset.Content.SaveAsPng(stream, asset.Content.Width, asset.Content.Height);
        }
        IEnumerable<GameAsset<T>> GetGameAssetsIn<T>(ContentManager content, string relativePath)
        {
            var items = Directory.EnumerateFiles(Path.Combine(content.RootDirectory, relativePath)).Where(c => Path.GetExtension(c) == ".xnb").Select(c => Path.GetFileNameWithoutExtension(c));
            foreach (var item in items)
            {
                yield return GameAsset.Create(item, content.Load<T>(Path.Combine(relativePath, item)));
            }
        }
    }
}
