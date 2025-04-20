using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmashTrees.Entities
{
    public class Platform
    {
        public Rectangle Rect;
        private Texture2D texture;
        private float scale;

        public Platform(float x, float y, float width, float height, Texture2D texture, float scale = 4f)
        {
            Rect = new Rectangle(x, y, width, height);
            this.texture = texture;
            this.scale = scale;
        }

        public void Draw()
        {
            int tileWidth = texture.Width;
            int tileHeight = texture.Height;

            int tilesToDraw = (int)(Rect.Width / (tileWidth * scale));

            for (int i = 0; i < tilesToDraw; i++)
            {
                Rectangle source = new Rectangle(0, 0, tileWidth, tileHeight);
                Rectangle dest = new Rectangle(
                    Rect.X + i * tileWidth * scale,
                    Rect.Y,
                    tileWidth * scale,
                    tileHeight * scale
                );

                Raylib.DrawTexturePro(texture, source, dest, Vector2.Zero, 0, Color.White);
            }
        }
    }
}
