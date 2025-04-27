using Raylib_cs;
using SuperSmashTrees.Entities;
using SuperSmashTrees.Structures;
using SuperSmashTrees.UI;
using SuperSmashTrees.Draw;
using System;
using System.Numerics;

namespace SuperSmashTrees.Core
{
    public class GameManager
    {
        private Texture2D background;
        private PlatformManager platformManager;
        private TokenManager tokenManager;
        private int screenWidth;
        private int screenHeight;
        private Player player1;
        private Player player2;
        private float sidebarWidth;

        public GameManager(int width, int height, CharacterOption player1Option, CharacterOption player2Option)
        {
            screenWidth = width;
            screenHeight = height;
            sidebarWidth = screenWidth * 0.2f;

            background = Raylib.LoadTexture("Assets/Sprites/Backgrounds/GameBackground.png");

            platformManager = new PlatformManager(
                new Texture2D[]
                {
                    Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass.png"),
                    Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass_small.png")
                }, width, height);

            tokenManager = new TokenManager();

            var ground = platformManager.GetPlatforms().Get(0);
            float playerScale = 4.0f;
            int spriteWidth = 22;
            float groundY = ground.Rect.Y;
            float margin = 50f;

            Vector2 startPos1 = new Vector2(ground.Rect.X + margin + (spriteWidth * playerScale) / 2, groundY);
            Vector2 startPos2 = new Vector2(ground.Rect.X + ground.Rect.Width - margin - (spriteWidth * playerScale) / 2, groundY);

            var controlsPlayer1 = new PlayerControls(KeyboardKey.D, KeyboardKey.A, KeyboardKey.W); // Jugador 1: WASD
            var controlsPlayer2 = new PlayerControls(KeyboardKey.Right, KeyboardKey.Left, KeyboardKey.Up); // Jugador 2: Flechas

            string basePath1 = $"Assets/Sprites/Characters/{player1Option.Name}";
            string basePath2 = $"Assets/Sprites/Characters/{player2Option.Name}";

            player1 = new Player($"{basePath1}/IDLE", $"{basePath1}/RUN", $"{basePath1}/JUMP",
                player1Option.IdleFrames, player1Option.RunFrames, player1Option.JumpFrames,
                startPos1, controlsPlayer1);

            player2 = new Player($"{basePath2}/IDLE", $"{basePath2}/RUN", $"{basePath2}/JUMP",
                player2Option.IdleFrames, player2Option.RunFrames, player2Option.JumpFrames,
                startPos2, controlsPlayer2);
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                float delta = Raylib.GetFrameTime();
                float gameAreaWidth = screenWidth * 0.8f;

                DrawBackground();
                platformManager.Draw();
                tokenManager.Update(delta, gameAreaWidth, player1, player2);
                CheckChallengeCompletion(player1);
                CheckChallengeCompletion(player2);
                UpdateAndDrawPlayers(delta, gameAreaWidth);
                DrawSidebar(gameAreaWidth);

                Raylib.EndDrawing();
            }
        }

        private void DrawBackground()
        {
            float scaleX = screenWidth / (float)background.Width;
            float scaleY = screenHeight / (float)background.Height;
            float scale = MathF.Max(scaleX, scaleY);

            float drawWidth = background.Width * scale;
            float drawHeight = background.Height * scale;

            Vector2 position = new Vector2((screenWidth - drawWidth) / 2, (screenHeight - drawHeight) / 2);
            Raylib.DrawTexturePro(background, new Rectangle(0, 0, background.Width, background.Height),
                new Rectangle(position.X, position.Y, drawWidth, drawHeight), Vector2.Zero, 0f, Color.White);
        }

        private void UpdateAndDrawPlayers(float delta, float gameAreaWidth)
        {
            player1.Update(delta, platformManager.GetPlatforms(), player2, gameAreaWidth);
            player2.Update(delta, platformManager.GetPlatforms(), player1, gameAreaWidth);

            player1.Draw();
            player2.Draw();
        }

        private void DrawSidebar(float gameAreaWidth)
        {
            Rectangle sidebar = new Rectangle(gameAreaWidth, 0, sidebarWidth, screenHeight);
            Raylib.DrawRectangleRec(sidebar, new Color(30, 30, 30, 180));
            Raylib.DrawLine((int)gameAreaWidth, 0, (int)gameAreaWidth, screenHeight, Color.White);

            Raylib.DrawText("Árboles y Retos", (int)(gameAreaWidth + 20), 20, 30, Color.Yellow);

            float sidebarStartX = screenWidth * 0.8f + sidebarWidth / 2;

            DrawPlayerInfo(player1, "Jugador 1", sidebarStartX, 50, Color.Green);
            DrawPlayerInfo(player2, "Jugador 2", sidebarStartX, 400, Color.Blue);
        }

        private void DrawPlayerInfo(Player player, string playerName, float sidebarStartX, int yStart, Color color)
        {
            Raylib.DrawText(playerName, (int)(sidebarStartX - 40), yStart, 20, color);

            int treeYStart = yStart + 60;

            // Mostrar reto activo
            var activeChallenge = player.ChallengeManager.GetActiveChallenge();
            if (activeChallenge != null)
            {
                Raylib.DrawText($"Reto: {activeChallenge.TargetTree} - {activeChallenge.Goal} {activeChallenge.TargetValue}",
                                (int)(sidebarStartX - 60), yStart + 30, 16, Color.White);
            }

            // Mostrar árbol
            if (activeChallenge != null)
            {
                if (activeChallenge.TargetTree == Challenge.TreeType.BST && player.TreeBST.Root != null)
                {
                    TreeDrawer.DrawBST(player.TreeBST.Root, sidebarStartX, treeYStart, 60);
                }
                else if (activeChallenge.TargetTree == Challenge.TreeType.AVL && player.TreeAVL.Root != null)
                {
                    TreeDrawer.DrawAVL(player.TreeAVL.Root, sidebarStartX, treeYStart, 60);
                }
            }
        }

        private void CheckChallengeCompletion(Player player)
        {
            var challenge = player.ChallengeManager.GetActiveChallenge();
            if (challenge == null)
                return;

            bool completed = player.ChallengeManager.ValidateChallenge(challenge, player);

            if (completed)
            {
                // ✅ Resetear árbol
                if (challenge.TargetTree == Challenge.TreeType.BST)
                    player.TreeBST = new BST();
                else if (challenge.TargetTree == Challenge.TreeType.AVL)
                    player.TreeAVL = new AVLTree();

                // ✅ Avanzar a siguiente reto
                player.ChallengeManager.AdvanceChallenge();
            }
        }
    }
}
