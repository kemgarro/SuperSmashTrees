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
        // ░░░ Campos ░░░
        private Texture2D background;
        private PlatformManager platformManager;
        private TokenManager tokenManager;

        private readonly int screenWidth;
        private readonly int screenHeight;
        private readonly float sidebarWidth;

        private Player player1;
        private Player player2;

        private readonly Timer gameTimer;
        private bool gameEnded = false;
        private bool isPaused = false;
        private bool shouldExit = false;

        private readonly CharacterOption player1Option;
        private readonly CharacterOption player2Option;

        // UI – botones
        private readonly Rectangle configButton;
        private readonly Rectangle resumeButton;
        private readonly Rectangle restartButton;
        private readonly Rectangle exitButton;

        // ░░░ Constructor ░░░
        public GameManager(int width, int height,
                           CharacterOption player1Option, CharacterOption player2Option)
        {
            screenWidth = width;
            screenHeight = height;
            sidebarWidth = screenWidth * 0.2f;
            gameTimer = new Timer(120f); // 2 min

            this.player1Option = player1Option;
            this.player2Option = player2Option;

            // Botonería
            configButton = new Rectangle(screenWidth - 60, 20, 40, 40);
            resumeButton = new Rectangle(screenWidth / 2 - 100, screenHeight / 2 - 60, 200, 40);
            restartButton = new Rectangle(screenWidth / 2 - 100, screenHeight / 2, 200, 40);
            exitButton = new Rectangle(screenWidth / 2 - 100, screenHeight / 2 + 60, 200, 40);

            // Recursos
            background = Raylib.LoadTexture("Assets/Sprites/Backgrounds/GameBackground.png");

            platformManager = new PlatformManager(new Texture2D[]
            {
                Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass.png"),
                Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass_small.png")
            }, width, height);

            tokenManager = new TokenManager();

            InitialisePlayers();
        }

        // ░░░ Bucle principal ░░░
        public void Run()
        {
            while (!Raylib.WindowShouldClose() && !shouldExit)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                float delta = Raylib.GetFrameTime();
                float gameAreaWidth = screenWidth * 0.8f;

                Vector2 mouse = Raylib.GetMousePosition();

                // Pausa desde engranaje
                if (!isPaused && Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                    Raylib.CheckCollisionPointRec(mouse, configButton))
                {
                    isPaused = true;
                }

                // Pausa desde ESC
                if (Raylib.IsKeyPressed(KeyboardKey.Escape) && !gameEnded)
                    isPaused = !isPaused;

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
                        if (Raylib.CheckCollisionPointRec(mouse, resumeButton))
                            isPaused = false;
                        else if (Raylib.CheckCollisionPointRec(mouse, restartButton))
                        {
                            isPaused = false;
                            ResetGame();
                        }
                        else if (Raylib.CheckCollisionPointRec(mouse, exitButton))
                        {
                            shouldExit = true;
                        }
                    }
                }

                DrawSidebar(gameAreaWidth);

                if (gameTimer.TimeOver && !gameEnded) gameEnded = true;

                if (gameEnded)
                {
                    DrawGameOverScreen();
                    if (Raylib.IsKeyPressed(KeyboardKey.R)) ResetGame();
                }

                Raylib.EndDrawing();
            }
        }

        // ░░░ Inicialización de jugadores ░░░
        private void InitialisePlayers()
        {
            var ground = platformManager.GetPlatforms().Get(0);
            float scale = 4f; int spriteW = 22; float margin = 50f; float y = ground.Rect.Y;
            Vector2 start1 = new Vector2(ground.Rect.X + margin + (spriteW * scale) / 2, y);
            Vector2 start2 = new Vector2(ground.Rect.X + ground.Rect.Width - margin - (spriteW * scale) / 2, y);

            var c1 = new PlayerControls(KeyboardKey.D, KeyboardKey.A, KeyboardKey.W, KeyboardKey.F);
            var c2 = new PlayerControls(KeyboardKey.Right, KeyboardKey.Left, KeyboardKey.Up, KeyboardKey.RightControl);

            string p1 = $"Assets/Sprites/Characters/{player1Option.Name}";
            string p2 = $"Assets/Sprites/Characters/{player2Option.Name}";

            player1 = new Player($"{p1}/IDLE", $"{p1}/RUN", $"{p1}/JUMP", $"{p1}/ATTACK1", // 📁 carpeta ATTACK1
                                 player1Option.IdleFrames, player1Option.RunFrames,
                                 player1Option.JumpFrames, player1Option.AttackFrames,
                                 start1, c1);
            

            player2 = new Player($"{p2}/IDLE", $"{p2}/RUN", $"{p2}/JUMP", $"{p2}/ATTACK1", // 📁 carpeta ATTACK1
                                 player2Option.IdleFrames, player2Option.RunFrames,
                                 player2Option.JumpFrames, player2Option.AttackFrames,
                                 start2, c2);

        }
        // ░░░ Dibujo de fondo ░░░
        private void DrawBackground()
        {
            float scale = MathF.Max(screenWidth / (float)background.Width,
                                      screenHeight / (float)background.Height);
            float dW = background.Width * scale;
            float dH = background.Height * scale;
            Vector2 pos = new Vector2((screenWidth - dW) / 2, (screenHeight - dH) / 2);

            Raylib.DrawTexturePro(background, new Rectangle(0, 0, background.Width, background.Height),
                                   new Rectangle(pos.X, pos.Y, dW, dH), Vector2.Zero, 0f, Color.White);
        }

        // ░░░ Actualizar y dibujar jugadores ░░░
        private void UpdateAndDrawPlayers(float delta, float gameAreaWidth)
        {
            player1.Update(delta, platformManager.GetPlatforms(), player2, gameAreaWidth);
            player2.Update(delta, platformManager.GetPlatforms(), player1, gameAreaWidth);

            player1.Draw();
            player2.Draw();
        }

        // ░░░ Panel lateral ░░░
        private void DrawSidebar(float gameAreaWidth)
        {
            Rectangle sidebar = new Rectangle(gameAreaWidth, 0, sidebarWidth, screenHeight);
            Raylib.DrawRectangleRec(sidebar, new Color(30, 30, 30, 180));
            Raylib.DrawLine((int)gameAreaWidth, 0, (int)gameAreaWidth, screenHeight, Color.White);

            Raylib.DrawText("Árboles y Retos", (int)(gameAreaWidth + 20), 20, 30, Color.Yellow);
            Raylib.DrawText($"Tiempo: {gameTimer.GetFormattedTime()}", (int)(screenWidth * 0.8f + 20), screenHeight - 40, 24, Color.White);

            float centerX = screenWidth * 0.8f + sidebarWidth / 2;
            DrawPlayerInfo(player1, "Jugador 1", centerX, 50, Color.Green);
            DrawPlayerInfo(player2, "Jugador 2", centerX, 400, Color.Blue);

            // engranaje
            Raylib.DrawRectangleRec(configButton, Color.Gray);
            Raylib.DrawText("⚙", (int)configButton.X + 10, (int)configButton.Y + 5, 24, Color.White);
        }

        private void DrawPlayerInfo(Player player, string label, float centerX, int yStart, Color color)
        {
            Raylib.DrawText(label, (int)(centerX - 40), yStart, 20, color);
            int treeY = yStart + 60;

            var challenge = player.ChallengeManager.GetActiveChallenge();
            if (challenge != null)
            {
                Raylib.DrawText($"Reto: {challenge.TargetTree} - {challenge.Goal} {challenge.TargetValue}", (int)(centerX - 60), yStart + 30, 16, Color.White);
                if (challenge.TargetTree == Challenge.TreeType.BST && player.TreeBST.Root != null)
                    TreeDrawer.DrawBST(player.TreeBST.Root, centerX, treeY, 60);
                else if (challenge.TargetTree == Challenge.TreeType.AVL && player.TreeAVL.Root != null)
                    TreeDrawer.DrawAVL(player.TreeAVL.Root, centerX, treeY, 60);
            }
        }

        // ░░░ Verificar retos ░░░
        private void CheckChallengeCompletion(Player player)
        {
            var challenge = player.ChallengeManager.GetActiveChallenge();
            if (challenge == null) return;

            if (!player.ChallengeManager.ValidateChallenge(challenge, player)) return;

            // Reset árbol y tokens
            if (challenge.TargetTree == Challenge.TreeType.BST) player.TreeBST = new BST();
            else player.TreeAVL = new AVLTree();
            player.CapturedTokens.Clear();
            player.IncrementChallenges();
            player.ChallengeManager.AdvanceChallenge();
        }

        // ░░░ Pantalla fin de juego ░░░
        private void DrawGameOverScreen()
        {
            string msg = "¡Tiempo finalizado!";
            Raylib.DrawText(msg, screenWidth / 2 - Raylib.MeasureText(msg, 40) / 2, screenHeight / 2 - 100, 40, Color.Yellow);

            int p1 = player1.CompletedChallenges;
            int p2 = player2.CompletedChallenges;
            string result = p1 > p2 ? "Jugador 1 gana" : p2 > p1 ? "Jugador 2 gana" : "¡Empate!";
            Raylib.DrawText(result, screenWidth / 2 - Raylib.MeasureText(result, 30) / 2, screenHeight / 2, 30, Color.White);
            Raylib.DrawText("Presiona R para reiniciar", screenWidth / 2 - 150, screenHeight / 2 + 50, 20, Color.LightGray);
        }

        // ░░░ Reinicio ░░░
        private void ResetGame()
        {
            gameEnded = false;
            gameTimer.Reset();

            platformManager = new PlatformManager(new Texture2D[]
            {
                Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass.png"),
                Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass_small.png")
            }, screenWidth, screenHeight);

            tokenManager = new TokenManager();
            InitialisePlayers();
        }

        // ░░░ Menú de pausa ░░░
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
