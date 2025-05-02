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
        private Timer gameTimer;
        private bool gameEnded = false;
        private CharacterOption player1Option;
        private CharacterOption player2Option;
        private bool isPaused = false;
        private Rectangle configButton;
        private Rectangle resumeButton;
        private Rectangle restartButton;
        private Rectangle exitButton;
        private bool shouldExit = false;



        public GameManager(int width, int height, CharacterOption player1Option, CharacterOption player2Option)
        {
            screenWidth = width;
            screenHeight = height;
            configButton = new Rectangle(screenWidth - 60, 20, 40, 40);
            resumeButton = new Rectangle(screenWidth / 2 - 100, screenHeight / 2 - 60, 200, 40);
            restartButton = new Rectangle(screenWidth / 2 - 100, screenHeight / 2, 200, 40);
            exitButton = new Rectangle(screenWidth / 2 - 100, screenHeight / 2 + 60, 200, 40);
            sidebarWidth = screenWidth * 0.2f;
            gameTimer = new Timer(120f); // 2 minutos
            this.player1Option = player1Option;
            this.player2Option = player2Option;


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
            while (!Raylib.WindowShouldClose() && !shouldExit)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                float delta = Raylib.GetFrameTime();
                float gameAreaWidth = screenWidth * 0.8f;

                Vector2 mousePos = Raylib.GetMousePosition();

                if (!isPaused && Raylib.IsMouseButtonPressed(MouseButton.Left) && Raylib.CheckCollisionPointRec(mousePos, configButton))
                {
                    isPaused = true;
                }

                // 🔽 Detectar ESC para pausar/reanudar
                if (Raylib.IsKeyPressed(KeyboardKey.Escape) && !gameEnded)
                {
                    isPaused = !isPaused;
                }

                if (!gameEnded && !isPaused)
                {
                    gameTimer.Update(delta);
                    DrawBackground();
                    platformManager.Draw();
                    tokenManager.Update(delta, gameAreaWidth, player1, player2);
                    CheckChallengeCompletion(player1);
                    CheckChallengeCompletion(player2);
                    UpdateAndDrawPlayers(delta, gameAreaWidth);
                }
                else if (isPaused)
                {
                    DrawPauseMenu();

                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        if (Raylib.CheckCollisionPointRec(mousePos, resumeButton))
                            isPaused = false;
                        else if (Raylib.CheckCollisionPointRec(mousePos, restartButton))
                        {
                            isPaused = false;
                            ResetGame();
                        }
                        else if (Raylib.CheckCollisionPointRec(mousePos, exitButton))
                        {
                            shouldExit = true;
                        }

                    }
                }


                DrawSidebar(gameAreaWidth);

                if (gameTimer.TimeOver && !gameEnded)
                {
                    gameEnded = true;
                }

                if (gameEnded)
                {
                    DrawGameOverScreen();

                    if (Raylib.IsKeyPressed(KeyboardKey.R))
                    {
                        ResetGame();
                    }
                }

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

            Raylib.DrawText($"Tiempo: {gameTimer.GetFormattedTime()}",
                (int)(screenWidth * 0.8f + 20), screenHeight - 40, 24, Color.White);


            float sidebarStartX = screenWidth * 0.8f + sidebarWidth / 2;

            DrawPlayerInfo(player1, "Jugador 1", sidebarStartX, 50, Color.Green);
            DrawPlayerInfo(player2, "Jugador 2", sidebarStartX, 400, Color.Blue);

            // Botón de engranaje (config)
            Raylib.DrawRectangleRec(configButton, Color.Gray);
            Raylib.DrawText("⚙", (int)configButton.X + 10, (int)configButton.Y + 5, 24, Color.White);

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
            if (challenge == null) return;

            bool completed = player.ChallengeManager.ValidateChallenge(challenge, player);
            if (!completed) return;

            // ✅ Resetear árbol
            if (challenge.TargetTree == Challenge.TreeType.BST)
                player.TreeBST = new BST();
            else if (challenge.TargetTree == Challenge.TreeType.AVL)
                player.TreeAVL = new AVLTree();

            // ✅ Resetear lista de tokens (antes de capturar otro)
            player.CapturedTokens.Clear(); // Usa tu método personalizado

            player.IncrementChallenges();

            // ✅ Avanzar a siguiente reto
            player.ChallengeManager.AdvanceChallenge();
        }

        private void DrawGameOverScreen()
        {
            string msg = "¡Tiempo finalizado!";
            Raylib.DrawText(msg, screenWidth / 2 - Raylib.MeasureText(msg, 40) / 2, screenHeight / 2 - 100, 40, Color.Yellow);

            int p1Score = player1.CompletedChallenges;
            int p2Score = player2.CompletedChallenges;

            string resultado = p1Score > p2Score ? "Jugador 1 gana" :
                               p2Score > p1Score ? "Jugador 2 gana" :
                               "¡Empate!";

            Raylib.DrawText(resultado, screenWidth / 2 - Raylib.MeasureText(resultado, 30) / 2, screenHeight / 2, 30, Color.White);
            Raylib.DrawText("Presiona R para reiniciar", screenWidth / 2 - 150, screenHeight / 2 + 50, 20, Color.LightGray);

        }
        private void ResetGame()
        {
            gameEnded = false;
            gameTimer.Reset();

            platformManager = new PlatformManager(
                new Texture2D[]
                {
            Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass.png"),
            Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass_small.png")
                }, screenWidth, screenHeight);

            tokenManager = new TokenManager();

            var ground = platformManager.GetPlatforms().Get(0);
            float playerScale = 4.0f;
            int spriteWidth = 22;
            float groundY = ground.Rect.Y;
            float margin = 50f;

            Vector2 startPos1 = new Vector2(ground.Rect.X + margin + (spriteWidth * playerScale) / 2, groundY);
            Vector2 startPos2 = new Vector2(ground.Rect.X + ground.Rect.Width - margin - (spriteWidth * playerScale) / 2, groundY);

            player1 = new Player($"Assets/Sprites/Characters/{player1Option.Name}/IDLE",
                                 $"Assets/Sprites/Characters/{player1Option.Name}/RUN",
                                 $"Assets/Sprites/Characters/{player1Option.Name}/JUMP",
                                 player1Option.IdleFrames, player1Option.RunFrames, player1Option.JumpFrames,
                                 startPos1, new PlayerControls(KeyboardKey.D, KeyboardKey.A, KeyboardKey.W));

            player2 = new Player($"Assets/Sprites/Characters/{player2Option.Name}/IDLE",
                                 $"Assets/Sprites/Characters/{player2Option.Name}/RUN",
                                 $"Assets/Sprites/Characters/{player2Option.Name}/JUMP",
                                 player2Option.IdleFrames, player2Option.RunFrames, player2Option.JumpFrames,
                                 startPos2, new PlayerControls(KeyboardKey.Right, KeyboardKey.Left, KeyboardKey.Up));
        }


        private Player CreatePlayer(Player original, bool isPlayer1)
        {
            string name = original == null ? "Samurai" : original.ChallengeManager.GetActiveChallenge()?.TargetTree == Challenge.TreeType.BST ? "Samurai" : "Knight";
            string basePath = $"Assets/Sprites/Characters/{name}";

            int idle = original == null ? 10 : original.CompletedChallenges; // Solo para que compile. Cambia si quieres.
            int run = original == null ? 10 : original.CompletedChallenges;
            int jump = original == null ? 10 : original.CompletedChallenges;

            Vector2 startPos;
            if (isPlayer1)
            {
                var ground = platformManager.GetPlatforms().Get(0);
                startPos = new Vector2(ground.Rect.X + 100, ground.Rect.Y);
                return new Player($"{basePath}/IDLE", $"{basePath}/RUN", $"{basePath}/JUMP", idle, run, jump, startPos,
                    new PlayerControls(KeyboardKey.D, KeyboardKey.A, KeyboardKey.W));
            }
            else
            {
                var ground = platformManager.GetPlatforms().Get(0);
                startPos = new Vector2(ground.Rect.X + ground.Rect.Width - 100, ground.Rect.Y);
                return new Player($"{basePath}/IDLE", $"{basePath}/RUN", $"{basePath}/JUMP", idle, run, jump, startPos,
                    new PlayerControls(KeyboardKey.Right, KeyboardKey.Left, KeyboardKey.Up));
            }
        }
        private void DrawPauseMenu()
        {
            Raylib.DrawRectangle(screenWidth / 4, screenHeight / 4, screenWidth / 2, screenHeight / 2, new Color(0, 0, 0, 200));
            Raylib.DrawText("PAUSA", screenWidth / 2 - 50, screenHeight / 4 + 30, 30, Color.Yellow);

            DrawButton(resumeButton, "Reanudar");
            DrawButton(restartButton, "Reiniciar");
            DrawButton(exitButton, "Salir al menú");
        }
        private void DrawButton(Rectangle rect, string text)
        {
            Color bg = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect) ? Color.DarkGreen : Color.DarkGray;
            Raylib.DrawRectangleRec(rect, bg);
            Raylib.DrawText(text, (int)(rect.X + 20), (int)(rect.Y + 10), 24, Color.White);
        }

    }
}
