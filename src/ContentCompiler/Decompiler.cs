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
            foreach (var asset in GetGameAssetsIn<Dictionary<string, string>>(content, "characters\\schedules"))
            {
                var schedule = Schedule.Decompile(asset);

                File.WriteAllText(Path.Combine(content.RootDirectory, "characters\\schedules", asset.Filename) + ".json", JsonConvert.SerializeObject(schedule, Formatting.Indented));
            }
            DecompilePortraits(content);
        }
        static IEnumerable<GameAsset<T>> GetGameAssetsIn<T>(ContentManager content, string relativePath)
        {
            var items = Directory.EnumerateFiles(Path.Combine(content.RootDirectory, relativePath)).Where(c => Path.GetExtension(c) == ".xnb").Select(c => Path.GetFileNameWithoutExtension(c));
            foreach (var item in items)
            {
                yield return GameAsset.Create(item, content.Load<T>(Path.Combine(relativePath, item)));
            }
        }

        static void DecompilePortraits(ContentManager content)
        {
            foreach (var asset in GetGameAssetsIn<Texture2D>(content, "portraits"))
                using (var stream = File.Create(content.RootDirectory + "\\Portraits\\" + asset.Filename + ".png"))
                    asset.Content.SaveAsPng(stream, asset.Content.Width, asset.Content.Height);
        }
    }
}
