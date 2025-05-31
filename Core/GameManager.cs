using Raylib_cs;
using SuperSmashTrees.Entities;
using SuperSmashTrees.Structures;
using SuperSmashTrees.UI;
using SuperSmashTrees.Draw;
using SuperSmashTrees.Utils;
using System.Numerics;

namespace SuperSmashTrees.Core
{
    public class GameManager
    {
        private Texture2D background;
        private PlatformManager platformManager;
        private TokenManager tokenManager;

        private readonly int screenWidth, screenHeight;
        private readonly float sidebarWidth;

        private Player player1, player2, player3;


        private readonly Timer gameTimer;
        private bool gameEnded, isPaused, shouldExit;

        private readonly CharacterOption player1Option, player2Option, player3Option;

        private readonly Rectangle configButton, resumeButton, restartButton, exitButton;
        private readonly Rectangle gameOverRestartButton;
        private readonly Rectangle gameOverExitButton;

        private Texture2D gearIcon;


        public GameManager(int width, int height,
                           CharacterOption p1Opt, CharacterOption p2Opt, CharacterOption p3Opt)
        {
            screenWidth = width;
            screenHeight = height;
            sidebarWidth = width * 0.2f;
            gameTimer = new Timer(120f);

            player1Option = p1Opt;
            player2Option = p2Opt;
            player3Option = p3Opt;

            configButton = new Rectangle(width - 60, 20, 40, 40);
            resumeButton = new Rectangle(width / 2 - 100, height / 2 - 60, 200, 40);
            restartButton = new Rectangle(width / 2 - 100, height / 2, 200, 40);
            exitButton = new Rectangle(width / 2 - 100, height / 2 + 60, 200, 40);
            gameOverRestartButton = new Rectangle(screenWidth / 2 - 100, screenHeight / 2, 200, 40);
            gameOverExitButton = new Rectangle(screenWidth / 2 - 100, screenHeight / 2 + 60, 200, 40);

            gearIcon = TextureManager.Load("Assets/Sprites/Icons/gear.png");


            background = TextureManager.Load("Assets/Sprites/Backgrounds/GameBackground.png");

            ResetGame(); // Arranca siempre con todo limpio
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose() && !shouldExit)
            {
                float delta = Raylib.GetFrameTime();
                Vector2 mouse = Raylib.GetMousePosition();

                // Si terminó el tiempo, marcamos gameEnded
                if (gameTimer.TimeOver && !gameEnded)
                    gameEnded = true;

                // Permitir reiniciar con R en pantalla de fin
                if (gameEnded)
                {
                    DrawGameOverScreen();

                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        if (Raylib.CheckCollisionPointRec(mouse, gameOverRestartButton))
                            ResetGame();

                        else if (Raylib.CheckCollisionPointRec(mouse, gameOverExitButton))
                            shouldExit = true;
                    }
                }


                if (!gameEnded && !isPaused)
                {
                    // Abrir menú de pausa
                    if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                        Raylib.CheckCollisionPointRec(mouse, configButton))
                        isPaused = true;
                    if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                        isPaused = true;

                    // Actualizar lógica
                    gameTimer.Update(delta);
                    tokenManager.Update(delta, screenWidth * 0.8f, player1, player2, player3);

                    CheckChallengeCompletion(player1);
                    CheckChallengeCompletion(player2);
                    CheckChallengeCompletion(player3); // Si decides usar un tercer jugador

                    player1.Update(delta, platformManager.GetPlatforms(), new Player[] { player2, player3 }, screenWidth * 0.8f);
                    player2.Update(delta, platformManager.GetPlatforms(), new Player[] { player1, player3 }, screenWidth * 0.8f);
                    player3.Update(delta, platformManager.GetPlatforms(), new Player[] { player1, player2 }, screenWidth * 0.8f);
                }
                else if (isPaused && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    if (Raylib.CheckCollisionPointRec(mouse, resumeButton))
                        isPaused = false;
                    else if (Raylib.CheckCollisionPointRec(mouse, restartButton))
                    {
                        isPaused = false;
                        ResetGame();
                    }
                    else if (Raylib.CheckCollisionPointRec(mouse, exitButton))
                        shouldExit = true;
                }

                // Dibujar todo
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                DrawBackground();
                platformManager.Draw();
                tokenManager.Draw();

                if (!gameEnded && !isPaused)
                {
                    player1.Draw();
                    player2.Draw();
                    player3.Draw(); // Si decides usar un tercer jugador
                }

                DrawSidebar(screenWidth * 0.8f);

                if (isPaused) DrawPauseMenu();
                if (gameEnded) DrawGameOverScreen();

                Raylib.EndDrawing();
            }

            TextureManager.UnloadAll();
        }

        private void ResetGame()
        {
            gameEnded = false;
            isPaused = false;
            shouldExit = false;
            gameTimer.Reset();

            platformManager = new PlatformManager(new[]
            {
                TextureManager.Load("Assets/Sprites/Platforms/ground_grass.png"),
                TextureManager.Load("Assets/Sprites/Platforms/ground_grass_small.png")
            }, screenWidth, screenHeight);

            tokenManager = new TokenManager();

            player1 = ResetPlayer(player1Option, true, 1);
            player2 = ResetPlayer(player2Option, false, 2);
            player3 = ResetPlayer(player3Option, false, 3); // Si decides usar un tercer jugador

            // Limpia TODO el estado de los jugadores
            player1.ClearProgress();
            player2.ClearProgress();
        }

        private Player ResetPlayer(CharacterOption opt, bool first, int playerNumber)
        {
            var ground = platformManager.GetPlatforms().Get(0);
            float scale = 4f;
            int spriteW = 22;
            float margin = 50f, y = ground.Rect.Y;

            Vector2 start;

            if (playerNumber == 1)
            {
                start = new Vector2(ground.Rect.X + margin + (spriteW * scale) / 2, y);
            }
            else if (playerNumber == 2)
            {
                start = new Vector2(ground.Rect.X + ground.Rect.Width - margin - (spriteW * scale) / 2, y);
            }
            else // jugador 3, por ejemplo en el centro
            {
                start = new Vector2(ground.Rect.X + ground.Rect.Width / 2, y);
            }

            IPlayerControls controls;

            if (playerNumber == 1)
            {
                controls = new PlayerControls(KeyboardKey.D, KeyboardKey.A, KeyboardKey.W, KeyboardKey.F);
            }
            else if (playerNumber == 2)
            {
                controls = new PlayerControls(KeyboardKey.Right, KeyboardKey.Left, KeyboardKey.Up, KeyboardKey.RightControl);
            }
            else // jugador 3 con nuevas teclas (puedes ajustar)
            {
                controls = new GamepadControls(0);
            }

            string basePath = $"Assets/Sprites/Characters/{opt.Name}";
            return new Player(
                $"{basePath}/IDLE", $"{basePath}/RUN",
                $"{basePath}/JUMP", $"{basePath}/ATTACK1",
                opt.IdleFrames, opt.RunFrames,
                opt.JumpFrames, opt.AttackFrames,
                start, controls
            );
        }

        private void CheckChallengeCompletion(Player p)
        {
            var ch = p.ChallengeManager.GetActiveChallenge();
            if (ch == null) return;

            // Solo validar tras capturar ≥1 token
            if (p.CapturedTokens.Count == 0) return;

            if (!p.ChallengeManager.ValidateChallenge(ch, p)) return;

            // Cumplido: reinicia árbol y tokens
            if (ch.TargetTree == Challenge.TreeType.BST) p.TreeBST = new BST();
            else p.TreeAVL = new AVLTree();

            p.CapturedTokens.Clear();
            p.CompletedChallenges++;
            p.ChallengeManager.AdvanceChallenge();
            p.AddScore(10);
        }

        private void DrawBackground()
        {
            float sx = screenWidth / (float)background.Width;
            float sy = screenHeight / (float)background.Height;
            float scale = MathF.Max(sx, sy);
            float w = background.Width * scale, h = background.Height * scale;
            Vector2 pos = new Vector2((screenWidth - w) / 2, (screenHeight - h) / 2);

            Raylib.DrawTexturePro(
                background,
                new Rectangle(0, 0, background.Width, background.Height),
                new Rectangle(pos.X, pos.Y, w, h),
                Vector2.Zero, 0f, Color.White
            );
        }

        private void DrawSidebar(float x0)
        {
            var sidebar = new Rectangle(x0, 0, sidebarWidth, screenHeight);
            Raylib.DrawRectangleRec(sidebar, new Color(30, 30, 30, 180));
            Raylib.DrawLine((int)x0, 0, (int)x0, screenHeight, Color.White);

            Raylib.DrawText("Árboles y Retos", (int)(x0 + 20), 20, 30, Color.Yellow);

            string t = $"Tiempo: {gameTimer.GetFormattedTime()}";
            int tx = (int)(x0 + 20), ty = screenHeight - 40;
            Raylib.DrawText(t, tx, ty, 24, Color.White);
            Raylib.DrawText($"P1: {player1.Score}  P2: {player2.Score} P3: {player3.Score}",
                            tx + Raylib.MeasureText(t, 24), ty, 24, Color.LightGray);

            float cx = x0 + sidebarWidth / 2;
            DrawPlayerInfo(player1, "Jugador 1", cx, 50, Color.Green);
            DrawPlayerInfo(player2, "Jugador 2", cx, 400, Color.Blue);
            DrawPlayerInfo(player3, "Jugador 3", cx, 750, Color.Red); // Si decides usar un tercer jugador

            // Ícono encima, centrado dentro del botón
            Vector2 iconPos = new Vector2(
                configButton.X + (configButton.Width - gearIcon.Width * 1.5f) / 2,
                configButton.Y + (configButton.Height - gearIcon.Height * 1.5f) / 2
            );

            Raylib.DrawTextureEx(gearIcon, iconPos, 0f, 1.5f, Color.White);


        }

        private void DrawPlayerInfo(Player p, string label, float cx, int y, Color col)
        {
            Raylib.DrawText(label, (int)(cx - 40), y, 20, col);
            var ch = p.ChallengeManager.GetActiveChallenge();
            if (ch != null)
            {
                Raylib.DrawText(
                    $"Reto: {ch.TargetTree} - {ch.Goal} {ch.TargetValue}",
                    (int)(cx - 60), y + 30, 16, Color.White
                );
                if (ch.TargetTree == Challenge.TreeType.BST && p.TreeBST.Root != null)
                    TreeDrawer.DrawBST(p.TreeBST.Root, cx, y + 60, 60);
                else if (ch.TargetTree == Challenge.TreeType.AVL && p.TreeAVL.Root != null)
                    TreeDrawer.DrawAVL(p.TreeAVL.Root, cx, y + 60, 60);
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

        private void DrawGameOverScreen()
        {
            // Fondo oscuro semitransparente
            Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 200));

            // Título
            string msg = "¡Tiempo finalizado!";
            Raylib.DrawText(msg,
                screenWidth / 2 - Raylib.MeasureText(msg, 40) / 2,
                screenHeight / 2 - 100,
                40, Color.Yellow
            );

            // Ganador
            int s1 = player1.Score, s2 = player2.Score , s3 = player3.Score;
            string res = "";

            if (s1 > s2 && s1 > s3)
                res = "¡Jugador 1 gana!";
            else if (s2 > s1 && s2 > s3)
                res = "¡Jugador 2 gana!";
            else if (s3 > s1 && s3 > s2)
                res = "¡Jugador 3 gana!";
            else
                res = "¡Empate!";

            Raylib.DrawText(
                res,
                screenWidth / 2 - Raylib.MeasureText(res, 30) / 2,
                screenHeight / 2 - 40,
                30,
                Color.White
            );

            Raylib.DrawText(res,
                screenWidth / 2 - Raylib.MeasureText(res, 30) / 2,
                screenHeight / 2 - 40,
                30, Color.White
            );

            // Botones
            DrawButton(gameOverRestartButton, "Reiniciar");
            DrawButton(gameOverExitButton, "Salir al menú");
        }


        private void DrawButton(Rectangle r, string txt)
        {
            Color bg = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), r)
                       ? Color.DarkGreen : Color.DarkGray;
            Raylib.DrawRectangleRec(r, bg);
            Raylib.DrawText(txt, (int)(r.X + 20), (int)(r.Y + 10), 24, Color.White);
        }
    }
}
