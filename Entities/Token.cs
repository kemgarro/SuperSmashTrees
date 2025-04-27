using Raylib_cs;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            // Determinar el color según el número
            Color tokenColor = GetColorForValue(Value);

            // Dibuja el círculo con el color correspondiente
            Raylib.DrawCircle((int)Position.X, (int)Position.Y, 20, tokenColor);

            // Dibuja el número encima
            string text = Value.ToString();
            int textWidth = Raylib.MeasureText(text, 20);
            Raylib.DrawText(text, (int)(Position.X - textWidth / 2), (int)(Position.Y - 10), 20, Color.Black);
        }
        private Color GetColorForValue(int value)
        {
            // Primero agrupamos el valor en bloques de 10
            int group = (value - 1) / 10;

            // Ahora usamos switch sobre el grupo
            return group switch
            {
                0 => new Color(200, 255, 200, 255), // 1-10
                1 => new Color(150, 255, 150, 255), // 11-20
                2 => new Color(100, 255, 100, 255), // 21-30
                3 => new Color(50, 220, 50, 255),   // 31-40
                4 => new Color(0, 200, 0, 255),     // 41-50
                5 => new Color(0, 170, 0, 255),     // 51-60
                6 => new Color(0, 140, 0, 255),     // 61-70
                7 => new Color(0, 110, 0, 255),     // 71-80
                8 => new Color(0, 80, 0, 255),      // 81-90
                9 => new Color(0, 50, 0, 255),      // 91-100
                _ => Color.White,                   // Valor inválido
            };
        }












    }
}
