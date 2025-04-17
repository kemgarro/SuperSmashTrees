using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using System.Numerics;

namespace Entities
{
    public class Platform
    {
        public Rectangle Rect;
        private Texture2D texture;

        // Cargar las texturas una sola vez
        private static List<Texture2D> textures = new();

        public static void LoadTextures()
        {
            if (textures.Count == 0)
            {
                textures.Add(Raylib.LoadTexture("C:\\Users\\garro\\OneDrive - Estudiantes ITCR\\Datos 1\\SuperSmashTrees\\SuperSmashTrees\\Assets\\Sprites\\Platforms\\ground_wood.png"));
                textures.Add(Raylib.LoadTexture("C:\\Users\\garro\\OneDrive - Estudiantes ITCR\\Datos 1\\SuperSmashTrees\\SuperSmashTrees\\Assets\\Sprites\\Platforms\\ground_wood_small.png"));
                textures.Add(Raylib.LoadTexture("C:\\Users\\garro\\OneDrive - Estudiantes ITCR\\Datos 1\\SuperSmashTrees\\SuperSmashTrees\\Assets\\Sprites\\Platforms\\ground_grass.png"));
                textures.Add(Raylib.LoadTexture("C:\\Users\\garro\\OneDrive - Estudiantes ITCR\\Datos 1\\SuperSmashTrees\\SuperSmashTrees\\Assets\\Sprites\\Platforms\\ground_grass_small.png"));
            }
        }

        public Platform(float x, float y, float width, float height)
        {
            Rect = new Rectangle(x, y, width, height);

            // Elegir textura aleatoria
            Random rng = new Random(Guid.NewGuid().GetHashCode()); // más aleatorio
            int index = rng.Next(textures.Count);
            texture = textures[index];
        }

        public void Draw()
        {
            Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle dest = Rect;
            Vector2 origin = new Vector2(0, 0);
            Raylib.DrawTexturePro(texture, source, dest, origin, 0, Color.White);
        }

        public static void UnloadTextures()
        {
            foreach (var tex in textures)
                Raylib.UnloadTexture(tex);
        }
    }
}
