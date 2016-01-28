using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public static class TextureContent
    {
        public static List<Texture2D> LoadTextures(string contentFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(Setup.contentManager.RootDirectory + "/" + contentFolder);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException();
            }

            List<Texture2D> result = new List<Texture2D>();

            FileInfo[] files = dir.GetFiles("*.*");

            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                result.Add(Setup.contentManager.Load<Texture2D>(contentFolder + "/" + key));
            }

            return result;
        }
    }
}
