using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmashTrees.UI
{
    public class CharacterSelect
    {
        private CharacterOption[] availableCharacters;
        private int selectedIndexPlayer1 = 0;
        private int selectedIndexPlayer2 = 1;

        private int screenWidth;
        private int screenHeight;

        public bool SelectionDone { get; private set; } = false;
        public CharacterOption Player1Character => availableCharacters[selectedIndexPlayer1];
        public CharacterOption Player2Character => availableCharacters[selectedIndexPlayer2];

        public CharacterSelect(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;

            availableCharacters = new CharacterOption[]
            {
                //         name     icon                                       idle run jump attack
                new CharacterOption("Samurai", Raylib.LoadTexture("Assets/Sprites/Icons/Samurai/IDLE1.png"), 10, 16,  9, 7),
                new CharacterOption("Knight",  Raylib.LoadTexture("Assets/Sprites/Icons/Knight/IDLE1.png"),   7,  8,  5, 6)
            };
        }
        private bool player1Confirmed = false;
        private bool player2Confirmed = false;

        public void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Enter)) player1Confirmed = true;
            if (Raylib.IsKeyPressed(KeyboardKey.Space)) player2Confirmed = true;

            if (player1Confirmed && player2Confirmed)
                SelectionDone = true;

            // Navegación (solo si no ha confirmado)
            if (!player1Confirmed)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Left)) selectedIndexPlayer1 = (selectedIndexPlayer1 - 1 + availableCharacters.Length) % availableCharacters.Length;
                if (Raylib.IsKeyPressed(KeyboardKey.Right)) selectedIndexPlayer1 = (selectedIndexPlayer1 + 1) % availableCharacters.Length;
            }

            if (!player2Confirmed)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.A)) selectedIndexPlayer2 = (selectedIndexPlayer2 - 1 + availableCharacters.Length) % availableCharacters.Length;
                if (Raylib.IsKeyPressed(KeyboardKey.D)) selectedIndexPlayer2 = (selectedIndexPlayer2 + 1) % availableCharacters.Length;
            }
        }

        public void Draw()
        {
            Raylib.DrawText("SELECCIÓN DE PERSONAJE", screenWidth / 2 - 200, 50, 30, Color.DarkGreen);

            DrawPlayerSelector("Jugador 1", selectedIndexPlayer1, screenWidth / 4);
            DrawPlayerSelector("Jugador 2", selectedIndexPlayer2, (screenWidth / 4) * 3);
        }

        private void DrawPlayerSelector(string title, int index, int centerX)
        {
            CharacterOption option = availableCharacters[index];

            float scale = 4.0f;

            int iconWidth = (int)(option.Icon.Width * scale);
            int iconHeight = (int)(option.Icon.Height * scale);

            Vector2 iconPosition = new Vector2(centerX - iconWidth / 2, 200);

            // Título "Jugador X"
            int titleWidth = Raylib.MeasureText(title, 20);
            Raylib.DrawText(title, centerX - titleWidth / 2, 150, 20, Color.RayWhite);

            // Icono escalado
            Raylib.DrawTextureEx(option.Icon, iconPosition, 0f, scale, Color.White);

            // Nombre del personaje
            int nameWidth = Raylib.MeasureText(option.Name, 20);
            Raylib.DrawText(option.Name, centerX - nameWidth / 2, (int)(iconPosition.Y + iconHeight + 10), 20, Color.LightGray);

            Rectangle outline = new Rectangle(iconPosition.X - 10, iconPosition.Y - 10, iconWidth + 20, iconHeight + 20);
            Raylib.DrawRectangleLinesEx(outline, 3, Color.Green);

            Raylib.DrawText("Presiona ENTER para confirmar", screenWidth / 2 - 150, screenHeight - 60, 20, Color.LightGray);

            Raylib.DrawText("¡Personaje seleccionado!", screenWidth / 2 - 150, screenHeight - 90, 20, Color.Yellow);
        }
    }
}
