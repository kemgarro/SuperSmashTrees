using Raylib_cs;
using SuperSmashTrees.Entities;
using SuperSmashTrees.Structures; // 👈 Importante: SOLO tu propia lista
using System;
using System.Numerics;

namespace SuperSmashTrees.Core
{
    public class PlatformManager
    {
        private Texture2D[] platformTextures;
        private SuperSmashTrees.Structures.List<Platform> platforms;
        private int screenWidth;
        private int screenHeight;

        public PlatformManager(Texture2D[] textures, int width, int height)
        {
            platformTextures = textures;
            screenWidth = width;
            screenHeight = height;
            platforms = new SuperSmashTrees.Structures.List<Platform>();
            GeneratePlatforms();
        }

        public SuperSmashTrees.Structures.List<Platform> GetPlatforms() => platforms;

        public void Draw()
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                platforms.Get(i).Draw();
                Raylib.DrawRectangleLinesEx(platforms.Get(i).Rect, 1, Color.Red);
            }
        }

        private void GeneratePlatforms()
        {
            Random rng = new Random();
            float platformScale = 4f;
            Texture2D platformTex = platformTextures[0];

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
    }
}
