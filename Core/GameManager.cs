using Raylib_cs;
using SuperSmashTrees.Entities;
using SuperSmashTrees.Structures;
using SuperSmashTrees.UI;
using System;
using System.Numerics;

namespace SuperSmashTrees.Core
{
    public class GameManager
    {
        private Texture2D background;
        private Texture2D[] platformTextures;
        private SuperSmashTrees.Structures.List<Platform> platforms;
        private int screenWidth;
        private int screenHeight;
        private Player player1;
        private Player player2;
        private SuperSmashTrees.Structures.List<Token> tokens = new SuperSmashTrees.Structures.List<Token>();
        private float tokenSpawnTimer = 0f;
        private float tokenSpawnInterval = 2f;
        private Random rng = new Random();
        private float sidebarWidth;

        public GameManager(int width, int height, CharacterOption player1Option, CharacterOption player2Option)
        {
            screenWidth = width;
            screenHeight = height;
            sidebarWidth = screenWidth * 0.2f;

            background = Raylib.LoadTexture("Assets/Sprites/Backgrounds/GameBackground.png");

            platformTextures = new Texture2D[]
            {
                Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass.png"),
                Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass_small.png")
            };

            platforms = new SuperSmashTrees.Structures.List<Platform>();
            GeneratePlatforms();

            Platform ground = platforms.Get(0);

            float playerScale = 4.0f;
            int spriteWidth = 22;

            float groundY = ground.Rect.Y;
            float margin = 50f;

            Vector2 startPos1 = new Vector2(ground.Rect.X + margin + (spriteWidth * playerScale) / 2, groundY);
            Vector2 startPos2 = new Vector2(ground.Rect.X + ground.Rect.Width - margin - (spriteWidth * playerScale) / 2, groundY);

            string basePath1 = $"Assets/Sprites/Characters/{player1Option.Name}";
            string basePath2 = $"Assets/Sprites/Characters/{player2Option.Name}";

            player1 = new Player($"{basePath1}/IDLE", $"{basePath1}/RUN", $"{basePath1}/JUMP",
                player1Option.IdleFrames, player1Option.RunFrames, player1Option.JumpFrames, startPos1);

            player2 = new Player($"{basePath2}/IDLE", $"{basePath2}/RUN", $"{basePath2}/JUMP",
                player2Option.IdleFrames, player2Option.RunFrames, player2Option.JumpFrames, startPos2);
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                float delta = Raylib.GetFrameTime();
                float gameAreaWidth = screenWidth * 0.8f;

                // Fondo
                float scaleX = screenWidth / (float)background.Width;
                float scaleY = screenHeight / (float)background.Height;
                float scale = MathF.Max(scaleX, scaleY);

                float drawWidth = background.Width * scale;
                float drawHeight = background.Height * scale;

                Vector2 position = new Vector2((screenWidth - drawWidth) / 2, (screenHeight - drawHeight) / 2);
                Raylib.DrawTexturePro(background, new Rectangle(0, 0, background.Width, background.Height),
                    new Rectangle(position.X, position.Y, drawWidth, drawHeight), Vector2.Zero, 0f, Color.White);

                // Dibujar plataformas
                for (int i = 0; i < platforms.Count; i++)
                {
                    platforms.Get(i).Draw();
                    Raylib.DrawRectangleLinesEx(platforms.Get(i).Rect, 1, Color.Red);
                }

                // Spawnear tokens
                tokenSpawnTimer += delta;
                if (tokenSpawnTimer >= tokenSpawnInterval)
                {
                    tokenSpawnTimer = 0f;
                    int value = rng.Next(1, 101); // 1 a 100
                    float x = rng.Next(50, (int)(gameAreaWidth) - 50);
                    tokens.Add(new Token(value, new Vector2(x, -20)));
                }

                // Actualizar y dibujar tokens
                for (int i = 0; i < tokens.Count; i++)
                {
                    Token token = tokens.Get(i);
                    token.Update(delta);
                    token.Draw();

                    // Capturar tokens por jugadores
                    if (Raylib.CheckCollisionCircleRec(token.Position, 20, player1.GetBounds()))
                    {
                        player1.CaptureToken(token.Value);
                        tokens = EliminarToken(tokens, i);
                        i--;
                        continue;
                    }
                    if (Raylib.CheckCollisionCircleRec(token.Position, 20, player2.GetBounds()))
                    {
                        player2.CaptureToken(token.Value);
                        tokens = EliminarToken(tokens, i);
                        i--;
                    }
                }
                // Coordenada base de sidebar para dibujar árboles
                float sidebarStartX = screenWidth * 0.8f + sidebarWidth / 2;

                // --- Jugador 1 ---

                // Dibuja el árbol BST de Jugador 1
                if (player1.TreeBST.Root != null)
                {
                    DrawBST(player1.TreeBST.Root, sidebarStartX, 500, 60); // y=500
                    Raylib.DrawText("BST Jugador 1", (int)(sidebarStartX - 40), 470, 20, Color.Green);
                }

                // Dibuja el árbol AVL de Jugador 1
                if (player1.TreeAVL.Root != null)
                {
                    DrawAVL(player1.TreeAVL.Root, sidebarStartX, 850, 60);
                    Raylib.DrawText("AVL Jugador 1", (int)(sidebarStartX - 40), 820, 20, Color.Green);
                }

                // --- Jugador 2 ---

                // Dibuja el árbol BST de Jugador 2
                if (player2.TreeBST.Root != null)
                {
                    DrawBST(player2.TreeBST.Root, sidebarStartX, 1200, 60); // y=1200
                    Raylib.DrawText("BST Jugador 2", (int)(sidebarStartX - 40), 1170, 20, Color.Blue);
                }

                // Dibuja el árbol AVL de Jugador 2
                if (player2.TreeAVL.Root != null)
                {
                    DrawAVL(player2.TreeAVL.Root, sidebarStartX, 1550, 60); // y=1550
                    Raylib.DrawText("AVL Jugador 2", (int)(sidebarStartX - 40), 1520, 20, Color.Blue);
                }



                // Actualizar jugadores
                player1.Update(delta, platforms, KeyboardKey.Right, KeyboardKey.Left, KeyboardKey.Space, player2, gameAreaWidth);
                player2.Update(delta, platforms, KeyboardKey.D, KeyboardKey.A, KeyboardKey.W, player1, gameAreaWidth);

                // Dibujar jugadores
                player1.Draw();
                player2.Draw();

                // Sidebar
                Rectangle sidebar = new Rectangle(gameAreaWidth, 0, sidebarWidth, screenHeight);
                Raylib.DrawRectangleRec(sidebar, new Color(30, 30, 30, 180)); // Fondo semitransparente
                Raylib.DrawLine((int)gameAreaWidth, 0, (int)gameAreaWidth, screenHeight, Color.White);

                Raylib.DrawText("Árbol", (int)(gameAreaWidth + 20), 20, 20, Color.White);

                // Tokens capturados Jugador 1
                Raylib.DrawText("Jugador 1:", (int)(gameAreaWidth + 20), 60, 20, Color.Green);
                for (int i = 0; i < player1.CapturedTokens.Count; i++)
                {
                    string tokenValue = player1.CapturedTokens.Get(i).ToString();
                    Raylib.DrawText(tokenValue, (int)(gameAreaWidth + 20), 90 + i * 20, 20, Color.White);
                }

                // Tokens capturados Jugador 2
                Raylib.DrawText("Jugador 2:", (int)(gameAreaWidth + 20), 300, 20, Color.Blue);
                for (int i = 0; i < player2.CapturedTokens.Count; i++)
                {
                    string tokenValue = player2.CapturedTokens.Get(i).ToString();
                    Raylib.DrawText(tokenValue, (int)(gameAreaWidth + 20), 330 + i * 20, 20, Color.White);
                }

                Raylib.EndDrawing();
            }
        }

        private void GeneratePlatforms()
        {
            Random rng = new Random();
            platforms = new SuperSmashTrees.Structures.List<Platform>();

            float platformScale = 4f;
            Texture2D platformTex = Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass.png");

            int tileWidth = (int)(platformTex.Width * platformScale);
            int tileHeight = (int)(platformTex.Height * platformScale);

            float gameAreaWidth = screenWidth * 0.8f;

            int groundTiles = 8;
            float baseWidth = groundTiles * tileWidth;
            float baseHeight = tileHeight;
            float baseX = (gameAreaWidth - baseWidth) / 2;
            float baseY = screenHeight - 100;

            platforms.Add(new Platform(baseX, baseY, baseWidth, baseHeight, platformTex, platformScale));

            int levels = 3;
            float levelHeightGap = 300;

            for (int level = 1; level <= levels; level++)
            {
                int numPlatforms = rng.Next(1, 4);
                float y = baseY - level * levelHeightGap;

                for (int i = 0; i < numPlatforms; i++)
                {
                    int tiles = rng.Next(2, 4);
                    float width = tiles * tileWidth;
                    float height = tileHeight;
                    float x = rng.Next(100, (int)(gameAreaWidth - width - 100));

                    Rectangle newRect = new Rectangle(x, y, width, height);

                    bool overlaps = false;
                    for (int j = 0; j < platforms.Count; j++)
                    {
                        if (Raylib.CheckCollisionRecs(platforms.Get(j).Rect, newRect))
                        {
                            overlaps = true;
                            break;
                        }
                    }

                    if (!overlaps)
                    {
                        Texture2D randomTex = platformTextures[rng.Next(platformTextures.Length)];
                        platforms.Add(new Platform(x, y, width, height, randomTex, platformScale));
                    }
                }
            }
        }

        private SuperSmashTrees.Structures.List<Token> EliminarToken(SuperSmashTrees.Structures.List<Token> lista, int index)
        {
            var nueva = new SuperSmashTrees.Structures.List<Token>();
            for (int i = 0; i < lista.Count; i++)
            {
                if (i != index)
                    nueva.Add(lista.Get(i));
            }
            return nueva;
        }
        private void DrawBST(BSTNode? node, float x, float y, float horizontalSpacing)
        {
            if (node == null)
                return;

            // Dibujar el valor
            Raylib.DrawCircle((int)x, (int)y, 15, Color.Green);
            string valueText = node.Value.ToString();
            int textWidth = Raylib.MeasureText(valueText, 20);
            Raylib.DrawText(valueText, (int)(x - textWidth / 2), (int)(y - 10), 20, Color.Black);

            // Dibujar conexiones a hijos
            if (node.Left != null)
            {
                Raylib.DrawLine((int)x, (int)y, (int)(x - horizontalSpacing), (int)(y + 60), Color.White);
                DrawBST(node.Left, x - horizontalSpacing, y + 60, horizontalSpacing * 0.6f);
            }
            if (node.Right != null)
            {
                Raylib.DrawLine((int)x, (int)y, (int)(x + horizontalSpacing), (int)(y + 60), Color.White);
                DrawBST(node.Right, x + horizontalSpacing, y + 60, horizontalSpacing * 0.6f);
            }
        }

        private void DrawAVL(AVLNode? node, float x, float y, float horizontalSpacing)
        {
            if (node == null)
                return;

            // Dibuja el nodo
            Raylib.DrawCircle((int)x, (int)y, 15, Color.Blue);
            string valueText = node.Value.ToString();
            int textWidth = Raylib.MeasureText(valueText, 20);
            Raylib.DrawText(valueText, (int)(x - textWidth / 2), (int)(y - 10), 20, Color.Black);

            // Dibuja las conexiones
            if (node.Left != null)
            {
                Raylib.DrawLine((int)x, (int)y, (int)(x - horizontalSpacing), (int)(y + 60), Color.White);
                DrawAVL(node.Left, x - horizontalSpacing, y + 60, horizontalSpacing * 0.6f);
            }
            if (node.Right != null)
            {
                Raylib.DrawLine((int)x, (int)y, (int)(x + horizontalSpacing), (int)(y + 60), Color.White);
                DrawAVL(node.Right, x + horizontalSpacing, y + 60, horizontalSpacing * 0.6f);
            }
        }


    }
}
