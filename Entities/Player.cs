using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Raylib_cs;
using System.Numerics;

namespace Entities
{
    public class Player
    {
        public Vector2 Position;
        public Color Color;
        private float speed = 4f;

        private KeyboardKey up, down, left, right;

        public Player(Vector2 startPosition, Color color, KeyboardKey up, KeyboardKey down, KeyboardKey left, KeyboardKey right)
        {
            Position = startPosition;
            Color = color;
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }

        public void Update()
        {
            if (Raylib.IsKeyDown(up)) Position.Y -= speed;
            if (Raylib.IsKeyDown(down)) Position.Y += speed;
            if (Raylib.IsKeyDown(left)) Position.X -= speed;
            if (Raylib.IsKeyDown(right)) Position.X += speed;
        }

        public void Draw()
        {
            Raylib.DrawRectangle((int)Position.X, (int)Position.Y, 40, 40, Color);
        }
    }
}
