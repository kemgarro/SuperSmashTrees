using Raylib_cs;
using System.Collections.Generic;

namespace SuperSmashTrees.Utils
{
    public static class TextureManager
    {
        private static Dictionary<string, Texture2D> textures = new();

        public static Texture2D Load(string path)
        {
            if (!textures.ContainsKey(path))
            {
                Texture2D tex = Raylib.LoadTexture(path);
                Raylib.SetTextureFilter(tex, TextureFilter.Point);
                textures[path] = tex;
            }

            return textures[path];
        }


        public static void UnloadAll()
        {
            foreach (var tex in textures.Values)
            {
                Raylib.UnloadTexture(tex);
            }

            textures.Clear();
        }


    }
}
