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

        public GameManager(int width, int height, CharacterOption player1Option, CharacterOption player2Option)
        {
            screenWidth = width;
            screenHeight = height;

            background = Raylib.LoadTexture("Assets/Sprites/Backgrounds/GameBackground.png");

            platformTextures = new Texture2D[]
            {
                Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass.png"),
                Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass_small.png")
            };

            platforms = new SuperSmashTrees.Structures.List<Platform>();
            GeneratePlatforms();
            
            Platform ground = platforms.Get(0); // La primera plataforma es el suelo

            float playerScale = 4.0f;
            int spriteWidth = 22;
            int spriteHeight = 34;

            float groundY = ground.Rect.Y;
            float margin = 50f; // margen del borde

            // Player 1: lado izquierdo
            Vector2 startPos1 = new Vector2(
                ground.Rect.X + margin + (spriteWidth * playerScale) / 2,
                groundY
            );

            // Player 2: lado derecho
            Vector2 startPos2 = new Vector2(
                ground.Rect.X + ground.Rect.Width - margin - (spriteWidth * playerScale) / 2,
                groundY
            );

            // Cargar personajes
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

                for (int i = 0; i < platforms.Count; i++)
                {
                    platforms.Get(i).Draw();
                    Raylib.DrawRectangleLinesEx(platforms.Get(i).Rect, 1, Color.Red); //---------------------------------------------------
                }
                float delta = Raylib.GetFrameTime();

                player1.Update(delta, platforms, KeyboardKey.Right, KeyboardKey.Left, KeyboardKey.Space);
                player2.Update(delta, platforms, KeyboardKey.D, KeyboardKey.A, KeyboardKey.W);

                player1.Draw();
                player2.Draw();

                Raylib.EndDrawing();
            }
        }

        private void GeneratePlatforms()
        {
            Random rng = new Random();
            platforms = new SuperSmashTrees.Structures.List<Platform>();

            float platformScale = 4f; // ESCALA visual clara y grande

            Texture2D platformTex = Raylib.LoadTexture("Assets/Sprites/Platforms/ground_grass.png");

            int tileWidth = (int)(platformTex.Width * platformScale);
            int tileHeight = (int)(platformTex.Height * platformScale);

            // 🟩 Suelo base
            int groundTiles = 8;
            float baseWidth = groundTiles * tileWidth;
            float baseHeight = tileHeight;

            float baseX = (screenWidth - baseWidth) / 2;
            float baseY = screenHeight - 100;

            platforms.Add(new Platform(baseX, baseY, baseWidth, baseHeight, platformTex, platformScale));

            // ☁️ Plataformas flotantes
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
                    float x = rng.Next(100, screenWidth - (int)width - 100);

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
    }
}
