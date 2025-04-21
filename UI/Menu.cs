using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

        public Menu(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            background = Raylib.LoadTexture("Assets/Sprites/Backgrounds/MenuBackground.png");

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
                {
                    StartGame = true;
                }
                else if (Raylib.CheckCollisionPointRec(mousePos, exitButton))
                {
                    Raylib.CloseWindow();
                }
            }
        }

        public void Draw()
        {
            // Escala proporcional para cubrir toda la pantalla (estilo "background cover")
            float scaleX = screenWidth / (float)background.Width;
            float scaleY = screenHeight / (float)background.Height;
            float scale = MathF.Max(scaleX, scaleY);

            float drawWidth = background.Width * scale;
            float drawHeight = background.Height * scale;

            Vector2 position = new Vector2(
                (screenWidth - drawWidth) / 2,
                (screenHeight - drawHeight) / 2
            );

            Rectangle sourceRec = new Rectangle(0, 0, background.Width, background.Height);
            Rectangle destRec = new Rectangle(position.X, position.Y, drawWidth, drawHeight);

            Raylib.DrawTexturePro(background, sourceRec, destRec, Vector2.Zero, 0f, Color.White);

            DrawButton(playButton, "JUGAR");
            DrawButton(exitButton, "SALIR");
        }

        private void DrawButton(Rectangle rect, string text)
        {
            Color bgColor = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect) ? Color.DarkGreen : Color.DarkGray;
            Raylib.DrawRectangleRec(rect, bgColor);
            Raylib.DrawText(text, (int)(rect.X + 40), (int)(rect.Y + 15), 30, Color.RayWhite);
        }
    }
}
