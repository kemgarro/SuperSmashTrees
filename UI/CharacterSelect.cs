using Raylib_cs;
using SuperSmashTrees.Utils;
using System;
using System.Numerics;

namespace SuperSmashTrees.UI
{
    public class CharacterSelect
    {
        private readonly Texture2D background;
        private readonly CharacterOption[] availableCharacters;
        private int selectedIndexPlayer1 = 0;
        private int selectedIndexPlayer2 = 1;
        private int selectedIndexPlayer3 = 2;

        private readonly int screenWidth;
        private readonly int screenHeight;


        public bool SelectionDone { get; private set; } = false;
        public CharacterOption Player1Character => availableCharacters[selectedIndexPlayer1];
        public CharacterOption Player2Character => availableCharacters[selectedIndexPlayer2];
        public CharacterOption Player3Character => availableCharacters[selectedIndexPlayer3];

        public CharacterSelect(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;
            // Carga el fondo del selector (asegúrate en Visual Studio/IDE de marcar "Copy to Output Directory")
            background = TextureManager.Load("Assets/Sprites/Backgrounds/FondoSelect.png");

            availableCharacters = new CharacterOption[]
            {
                new CharacterOption("Samurai", TextureManager.Load("Assets/Sprites/Icons/Samurai/IDLE1.png"), 10,16,9,7),
                new CharacterOption("Knight",  TextureManager.Load("Assets/Sprites/Icons/Knight/IDLE1.png"), 7,8,5,6),
                new CharacterOption("Demon", TextureManager.Load("Assets/Sprites/Icons/Demon/IDLE1.png"), 6,8,9,6),
            };
        }

        private bool player1Confirmed = false;
        private bool player2Confirmed = false;
        private bool player3Confirmed = false;

        public void Update()
        {
            if (!player1Confirmed && Raylib.IsKeyPressed(KeyboardKey.Enter)) player1Confirmed = true;
            if (!player2Confirmed && Raylib.IsKeyPressed(KeyboardKey.Space)) player2Confirmed = true;
            if (!player3Confirmed && Raylib.IsKeyPressed(KeyboardKey.O)) player3Confirmed = true;
            if (player1Confirmed && player2Confirmed && player3Confirmed) SelectionDone = true;

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
            if (!player3Confirmed)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.J)) selectedIndexPlayer3 = (selectedIndexPlayer3 - 1 + availableCharacters.Length) % availableCharacters.Length;
                if (Raylib.IsKeyPressed(KeyboardKey.L)) selectedIndexPlayer3 = (selectedIndexPlayer3 + 1) % availableCharacters.Length;
            }
        }

        public void Draw()
        {
            // 1) Dibuja el fondo escalado "cover"
            float scaleX = screenWidth / (float)background.Width;
            float scaleY = screenHeight / (float)background.Height;
            float scale = MathF.Max(scaleX, scaleY);
            float drawW = background.Width * scale;
            float drawH = background.Height * scale;
            Vector2 pos = new Vector2((screenWidth - drawW) / 2, (screenHeight - drawH) / 2);

            Raylib.DrawTexturePro(
                background,
                new Rectangle(0, 0, background.Width, background.Height),
                new Rectangle(pos.X, pos.Y, drawW, drawH),
                Vector2.Zero,
                0f,
                Color.White
            );

            // 2) Texto y selectores

            DrawPlayerSelector("Jugador 1", selectedIndexPlayer1, (int)(screenWidth * 0.35f), 450);
            DrawPlayerSelector("Jugador 2", selectedIndexPlayer2, (int)(screenWidth * 0.65f), 450);
            DrawPlayerSelector("Jugador 3", selectedIndexPlayer3, (int)(screenWidth * 0.5f), 600);

        }

        private void DrawPlayerSelector(string label, int index, int centerX, int iconY)
        {
            var opt = availableCharacters[index];
            float scale = 4f;
            int w = (int)(opt.Icon.Width * scale);
            int h = (int)(opt.Icon.Height * scale);
            Vector2 p = new Vector2(centerX - w / 2, iconY);

            Raylib.DrawText(label, centerX - Raylib.MeasureText(label, 20) / 2, iconY - 40, 20, Color.RayWhite);
            Raylib.DrawTextureEx(opt.Icon, p, 0f, scale, Color.White);
            Raylib.DrawText(opt.Name, centerX - Raylib.MeasureText(opt.Name, 20) / 2, (int)(p.Y + h + 10), 20, Color.LightGray);
            Raylib.DrawRectangleLinesEx(new Rectangle(p.X - 10, p.Y - 10, w + 20, h + 20), 3, Color.Green);

            Raylib.DrawText("ENTER: confirma P1 / SPACE: confirma P2",
                           screenWidth / 2 - 180, screenHeight - 60, 20, Color.LightGray);

            bool confirmed = (label == "Jugador 1" ? player1Confirmed : player2Confirmed);
            if (confirmed)
            {
                Raylib.DrawText("¡Seleccionado!",
                               centerX - Raylib.MeasureText("¡Seleccionado!", 20) / 2,
                               screenHeight - 90, 20, Color.Yellow);
            }
        }
    }
}
