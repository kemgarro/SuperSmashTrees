using Raylib_cs;
using SuperSmashTrees.Utils;
using System.Numerics;

namespace SuperSmashTrees.UI
{
    public class Menu
    {
        private Texture2D background;
        private int screenWidth;
        private int screenHeight;

        private Rectangle playButton;
        private Rectangle exitButton;

        public bool StartGame { get; private set; } = false;
        public bool ShouldExit { get; private set; } = false;

        public Menu(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            background = TextureManager.Load("Assets/Sprites/Backgrounds/FondoMenu.png");

            float buttonWidth = 200;
            float buttonHeight = 60;
            float centerX = screenWidth / 2 - buttonWidth / 2;

            playButton = new Rectangle(centerX, screenHeight * 0.7f, buttonWidth, buttonHeight);
            exitButton = new Rectangle(centerX, screenHeight * 0.7f + 80, buttonWidth, buttonHeight);
        }

        public void Update()
        {
            Vector2 mousePos = Raylib.GetMousePosition();

            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                if (Raylib.CheckCollisionPointRec(mousePos, playButton))
                    StartGame = true;
                else if (Raylib.CheckCollisionPointRec(mousePos, exitButton))
                    ShouldExit = true; // NO cerrar la ventana aquí mismo
            }
        }

        public void Draw()
        {
            float scale = MathF.Max(
                screenWidth / (float)background.Width,
                screenHeight / (float)background.Height
            );

            float drawWidth = background.Width * scale;
            float drawHeight = background.Height * scale;

            Vector2 position = new(
                (screenWidth - drawWidth) / 2,
                (screenHeight - drawHeight) / 2
            );

            Raylib.DrawTexturePro(
                background,
                new Rectangle(0, 0, background.Width, background.Height),
                new Rectangle(position.X, position.Y, drawWidth, drawHeight),
                Vector2.Zero,
                0f,
                Color.White
            );

            DrawButton(playButton, "JUGAR");
            DrawButton(exitButton, "SALIR");
        }

        private void DrawButton(Rectangle rect, string text)
        {
            Color bg = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect) ? Color.DarkGreen : Color.DarkGray;
            Raylib.DrawRectangleRec(rect, bg);
            Raylib.DrawText(text, (int)(rect.X + 40), (int)(rect.Y + 15), 30, Color.RayWhite);
        }
    }
}
