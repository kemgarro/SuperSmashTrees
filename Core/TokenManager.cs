using Raylib_cs;
using SuperSmashTrees.Entities;
using SuperSmashTrees.Structures;
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

        public void Update(float delta, float gameAreaWidth, Player player1, Player player2, Player player3)
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

                if (Raylib.CheckCollisionCircleRec(token.Position, 20, player1.GetBounds()))
                {
                    if (player1.ChallengeManager.GetActiveChallenge() != null)
                        player1.CaptureToken(token.Value);

                    tokens.RemoveAt(i);
                    i--;
                    continue;
                }

                if (Raylib.CheckCollisionCircleRec(token.Position, 20, player2.GetBounds()))
                {
                    if (player2.ChallengeManager.GetActiveChallenge() != null)
                        player2.CaptureToken(token.Value);

                    tokens.RemoveAt(i);
                    i--;
                    continue;
                }

                if (Raylib.CheckCollisionCircleRec(token.Position, 20, player3.GetBounds()))
                {
                    if (player3.ChallengeManager.GetActiveChallenge() != null)
                        player3.CaptureToken(token.Value);

                    tokens.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Draw()
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                tokens.Get(i).Draw();
            }
        }
    }
}
