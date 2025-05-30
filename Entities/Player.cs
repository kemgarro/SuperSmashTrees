using Raylib_cs;
using SuperSmashTrees.Core;
using SuperSmashTrees.Structures;
using System.Numerics;

namespace SuperSmashTrees.Entities
{
    public class Player
    {
        // Animaciones
        private readonly Texture2D[] idleFrames;
        private readonly Texture2D[] runFrames;
        private readonly Texture2D[] jumpFrames;
        private readonly Texture2D[] attackFrames;

        private float animationTimer;
        private int currentFrame;
        private readonly float frameTime = 0.07f;

        // Física
        private readonly float gravity = 900f;
        private readonly float jumpForce = -800f;
        private float velocityY = 0f;
        private bool isJumping = false;
        private bool isAttacking = false;

        // Propiedades públicas
        public Vector2 Position;
        public float Speed = 300f;
        public string state = "IDLE";
        private bool facingLeft = false;

        private readonly PlayerControls controls;

        public int CompletedChallenges { get; set; } = 0;
        public SuperSmashTrees.Structures.List<int> CapturedTokens { get; } = new SuperSmashTrees.Structures.List<int>();
        public BST TreeBST { get; set; } = new BST();
        public AVLTree TreeAVL { get; set; } = new AVLTree();
        public ChallengeManager ChallengeManager { get; set; } = new ChallengeManager();

        public int Score { get; private set; } = 0;

        public Player(string idlePath, string runPath, string jumpPath, string attackPath,
                      int idleCount, int runCount, int jumpCount, int attackCount,
                      Vector2 startPosition, PlayerControls controls)
        {
            idleFrames = LoadFrames(idlePath, idleCount, "IDLE");
            runFrames = LoadFrames(runPath, runCount, "RUN");
            jumpFrames = LoadFrames(jumpPath, jumpCount, "JUMP");
            attackFrames = LoadFrames(attackPath, attackCount, "ATTACK");

            Position = startPosition;
            this.controls = controls;

            ChallengeManager.AdvanceChallenge();
        }

        private Texture2D[] LoadFrames(string path, int count, string prefix)
        {
            var frames = new Texture2D[count];
            for (int i = 0; i < count; i++)
            {
                string framePath = $"{path}/{prefix}{i + 1}.png";
                Texture2D tex = Raylib.LoadTexture(framePath);
                Raylib.SetTextureFilter(tex, TextureFilter.Point);
                frames[i] = tex;
            }
            return frames;
        }

        public void Update(float delta, SuperSmashTrees.Structures.List<Platform> platforms,
                           Player[] otherPlayers, float maxGameArea = 1920f)
        {
            bool moving = false;
            string prevState = state;

            // Ataque
            if (controls.AttackPressed() && !isAttacking)
            {
                state = "ATTACK";
                isAttacking = true;
                currentFrame = 0;
                animationTimer = 0f;
            }

            // Movimiento (solo si no ataca)
            if (!isAttacking)
            {
                if (controls.MoveRight())
                {
                    Position.X += Speed * delta;

                    // Chequear colisiones y empujar con todos los otros jugadores
                    foreach (var otherPlayer in otherPlayers)
                    {
                        if (Raylib.CheckCollisionRecs(GetBounds(), otherPlayer.GetBounds()))
                        {
                            Position.X -= 5f;
                            otherPlayer.Position.X += 5f;
                        }
                        else
                        {
                            state = "RUN";
                            facingLeft = false;
                            moving = true;
                        }
                    }
                }
                else if (controls.MoveLeft())
                {
                    Position.X -= Speed * delta;

                    foreach (var otherPlayer in otherPlayers)
                    {
                        if (Raylib.CheckCollisionRecs(GetBounds(), otherPlayer.GetBounds()))
                        {
                            Position.X += 5f;
                            otherPlayer.Position.X -= 5f;
                        }
                        else
                        {
                            state = "RUN";
                            facingLeft = true;
                            moving = true;
                        }
                    }
                }

                if (controls.JumpPressed() && !isJumping)
                {
                    velocityY = jumpForce;
                    isJumping = true;
                    state = "JUMP";
                }
            }

            // Gravedad y colisiones con plataformas
            velocityY += gravity * delta;
            float totalFall = velocityY * delta;
            int steps = 10;
            float stepSize = totalFall / steps;
            bool onPlatform = false;

            for (int s = 0; s < steps; s++)
            {
                Position.Y += stepSize;
                Rectangle playerRect = GetBounds();

                for (int i = 0; i < platforms.Count; i++)
                {
                    var plat = platforms.Get(i).Rect;
                    bool falling = velocityY >= 0;
                    if (Raylib.CheckCollisionRecs(playerRect, plat))
                    {
                        float playerBottom = Position.Y;
                        float platformTop = plat.Y;
                        if (falling && playerBottom <= platformTop + 10)
                        {
                            Position.Y = platformTop;
                            velocityY = 0f;
                            isJumping = false;
                            onPlatform = true;
                            if (!moving && !isAttacking) state = "IDLE";
                            break;
                        }
                    }
                }
                if (onPlatform) break;
            }

            if (onPlatform && !moving && !isAttacking) state = "IDLE";
            if (!onPlatform && velocityY > 0 && !isAttacking) state = "JUMP";

            // Animación
            if (state != prevState)
            {
                currentFrame = 0;
                animationTimer = 0f;
            }
            animationTimer += delta;
            if (animationTimer >= frameTime)
            {
                animationTimer = 0f;
                var frames = GetCurrentFrames();
                int total = frames.Length;

                if (state == "ATTACK")
                {
                    if (currentFrame < total - 1) currentFrame++;
                    else
                    {
                        isAttacking = false;
                        state = isJumping ? "JUMP" : moving ? "RUN" : "IDLE";
                        currentFrame = 0;
                    }
                }
                else if (state == "JUMP")
                {
                    if (currentFrame < jumpFrames.Length - 3) currentFrame++;
                    else
                    {
                        currentFrame++;
                        if (currentFrame >= jumpFrames.Length)
                            currentFrame = jumpFrames.Length - 3;
                    }
                }
                else
                {
                    currentFrame = (currentFrame + 1) % total;
                }
            }

            // Límites de pantalla
            if (Position.X < 0) Position.X = 0;
            if (Position.X > maxGameArea) Position.X = maxGameArea;

            // Colisión de espada (ataque)
            if (isAttacking && currentFrame == attackFrames.Length - 1)
            {
                var swordArea = GetSwordCollisionArea();
                foreach (var otherPlayer in otherPlayers)
                {
                    if (Raylib.CheckCollisionRecs(swordArea, otherPlayer.GetBounds()))
                    {
                        float push = 150f;
                        if (facingLeft) otherPlayer.Position.X -= push;
                        else otherPlayer.Position.X += push;
                        AddScore(2);
                    }
                }
            }

            // Caída del escenario
            if (Position.Y > 1080)
            {
                Position = new Vector2(100, 100);
                velocityY = 0;
                isJumping = false;
                foreach (var otherPlayer in otherPlayers)
                {
                    otherPlayer.AddScore(5);
                }
            }
        }

        private Texture2D[] GetCurrentFrames() => state switch
        {
            "RUN" => runFrames,
            "JUMP" => jumpFrames,
            "ATTACK" => attackFrames,
            _ => idleFrames,
        };

        public void Draw()
        {
            var frames = GetCurrentFrames();
            var frame = frames[currentFrame];
            float scale = 4f;
            var pos = new Vector2(
                Position.X - (frame.Width * scale) / 2,
                Position.Y - (frame.Height * scale)
            );

            if (facingLeft)
            {
                var src = new Rectangle(0, 0, -frame.Width, frame.Height);
                var dest = new Rectangle(pos.X, pos.Y, frame.Width * scale, frame.Height * scale);
                Raylib.DrawTexturePro(frame, src, dest, Vector2.Zero, 0f, Color.White);
            }
            else
            {
                Raylib.DrawTextureEx(frame, pos, 0f, scale, Color.White);
            }
        }

        public Rectangle GetBounds() =>
            new Rectangle(Position.X - 22, Position.Y - 34, 44, 34);

        public void CaptureToken(int value)
        {
            CapturedTokens.Add(value);
            var active = ChallengeManager.GetActiveChallenge();
            if (active != null)
            {
                if (active.TargetTree == Challenge.TreeType.BST) TreeBST.Insert(value);
                else TreeAVL.Insert(value);
            }
        }

        public void AddScore(int amount) => Score += amount;
        public void ResetScore() => Score = 0;

        // Limpia TODO el estado del jugador
        public void ClearProgress()
        {
            ResetScore();
            CompletedChallenges = 0;
            CapturedTokens.Clear();
            TreeBST = new BST();
            TreeAVL = new AVLTree();
            ChallengeManager = new ChallengeManager();
        }

        private Rectangle GetSwordCollisionArea()
        {
            float width = 100f, height = 40f;
            float xOffset = facingLeft
                ? Position.X - width
                : Position.X + 22;
            float yOffset = Position.Y - 30;
            return new Rectangle(xOffset, yOffset, width, height);
        }
    }
}
