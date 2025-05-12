using Raylib_cs;
using System.Numerics;

namespace SuperSmashTrees.Entities
{
    public class Token
    {
        public int Value;
        public Vector2 Position;
        public float Speed = 200f;

        public Token(int value, Vector2 startPosition)
        {
            Value = value;
            Position = startPosition;
        }

        public void Update(float delta)
        {
            Position.Y += Speed * delta;
        }

        public void Draw()
        {
            Color tokenColor = GetColorForValue(Value);

            Raylib.DrawCircle((int)Position.X, (int)Position.Y, 20, tokenColor);

            string text = Value.ToString();
            int textWidth = Raylib.MeasureText(text, 20);
            Raylib.DrawText(text, (int)(Position.X - textWidth / 2), (int)(Position.Y - 10), 20, Color.Black);
        }

        private Color GetColorForValue(int value)
        {
            int group = (value - 1) / 10;

            return group switch
            {
                0 => new Color(200, 255, 200, 255),
                1 => new Color(150, 255, 150, 255),
                2 => new Color(100, 255, 100, 255),
                3 => new Color(50, 220, 50, 255),
                4 => new Color(0, 200, 0, 255),
                5 => new Color(0, 170, 0, 255),
                6 => new Color(0, 140, 0, 255),
                7 => new Color(0, 110, 0, 255),
                8 => new Color(0, 80, 0, 255),
                9 => new Color(0, 50, 0, 255),
                _ => Color.White,
            };
        }
    }
}
