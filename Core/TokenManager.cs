using Raylib_cs;
using SuperSmashTrees.Entities;
using SuperSmashTrees.Structures;
using System;
using System.Numerics;

namespace SuperSmashTrees.Core
{
    public class TokenManager
    {
        private SuperSmashTrees.Structures.List<Token> tokens;
        private float spawnTimer;
        private float spawnInterval;
        private Random rng;

        public TokenManager()
        {
            tokens = new SuperSmashTrees.Structures.List<Token>();
            spawnTimer = 0f;
            spawnInterval = 2f;
            rng = new Random();
        }

        public void Update(float delta, float gameAreaWidth, Player player1, Player player2)
        {
            spawnTimer += delta;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                int value = rng.Next(1, 101);
                float x = rng.Next(50, (int)(gameAreaWidth) - 50);
                tokens.Add(new Token(value, new Vector2(x, -20)));
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens.Get(i);
                token.Update(delta);
                token.Draw();

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
    }
}
