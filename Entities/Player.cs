using Raylib_cs;
using SuperSmashTrees.Core;
using SuperSmashTrees.Structures;
using System.Numerics;

namespace SuperSmashTrees.Entities
{
    public class Player
    {
        private Texture2D[] idleFrames;
        private Texture2D[] runFrames;
        private Texture2D[] jumpFrames;

        private float animationTimer;
        private int currentFrame;
        private float frameTime = 0.07f;

        private float gravity = 900f;
        private float velocityY = 0f;
        private bool isJumping = false;

        private float jumpForce = -800f;

        public Vector2 Position;
        public float Speed = 300f;

        public string state = "IDLE";
        private bool facingLeft = false;

        private PlayerControls controls;

        public int CompletedChallenges { get; private set; } = 0;
       

        public SuperSmashTrees.Structures.List<int> CapturedTokens { get; private set; } = new SuperSmashTrees.Structures.List<int>();
        public BST TreeBST { get; set; } = new BST();
        public AVLTree TreeAVL { get; set; } = new AVLTree();
        public ChallengeManager ChallengeManager { get; private set; } = new ChallengeManager();

        public Player(string idlePath, string runPath, string jumpPath, int idleCount, int runCount, int jumpCount, Vector2 startPosition, PlayerControls controls)
        {
            idleFrames = LoadFrames(idlePath, idleCount, "IDLE");
            runFrames = LoadFrames(runPath, runCount, "RUN");
            jumpFrames = LoadFrames(jumpPath, jumpCount, "JUMP");
            Position = startPosition;
            this.controls = controls;

            ChallengeManager.AdvanceChallenge();

        }

        private Texture2D[] LoadFrames(string path, int count, string prefix)
        {
            Texture2D[] frames = new Texture2D[count];
            for (int i = 0; i < count; i++)
            {
                string framePath = $"{path}/{prefix}{i + 1}.png";
                Texture2D tex = Raylib.LoadTexture(framePath);
                Raylib.SetTextureFilter(tex, TextureFilter.Point);
                frames[i] = tex;
            }
            return frames;
        }

        public void Update(float delta, SuperSmashTrees.Structures.List<SuperSmashTrees.Entities.Platform> platforms, Player? otherPlayer = null, float maxGameArea = 1920f)
        {
            bool moving = false;
            string prevState = state;

            if (controls.MoveRight())
            {
                Position.X += Speed * delta;
                if (otherPlayer != null && Raylib.CheckCollisionRecs(GetBounds(), otherPlayer.GetBounds()))
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
            else if (controls.MoveLeft())
            {
                Position.X -= Speed * delta;
                if (otherPlayer != null && Raylib.CheckCollisionRecs(GetBounds(), otherPlayer.GetBounds()))
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

            if (controls.JumpPressed() && !isJumping)
            {
                velocityY = jumpForce;
                isJumping = true;
                state = "JUMP";
            }

            // Gravedad
            velocityY += gravity * delta;
            Position.Y += velocityY * delta;

            // Colisiones
            Rectangle playerRect = new Rectangle(Position.X - 22, Position.Y - 34, 44, 34);
            bool onPlatform = false;

            for (int i = 0; i < platforms.Count; i++)
            {
                Rectangle plat = platforms.Get(i).Rect;
                bool falling = velocityY >= 0;

                if (falling && Raylib.CheckCollisionRecs(playerRect, plat))
                {
                    float playerBottom = Position.Y;
                    float platformTop = plat.Y;

                    if (playerBottom <= platformTop + 10)
                    {
                        Position.Y = platformTop;
                        velocityY = 0f;
                        isJumping = false;
                        onPlatform = true;

                        if (!moving)
                            state = "IDLE";
                    }
                }
            }

            if (!onPlatform && velocityY > 0)
            {
                state = "JUMP";
            }

            // Animaciones
            if (state != prevState)
            {
                currentFrame = 0;
                animationTimer = 0f;
            }

            animationTimer += delta;
            if (animationTimer >= frameTime)
            {
                animationTimer = 0f;

                int totalFrames = GetCurrentFrames().Length;

                if (state == "JUMP")
                {
                    if (currentFrame < jumpFrames.Length - 3)
                    {
                        currentFrame++;
                    }
                    else
                    {
                        currentFrame++;
                        if (currentFrame >= jumpFrames.Length)
                        {
                            currentFrame = jumpFrames.Length - 3;
                        }
                    }
                }
                else
                {
                    currentFrame = (currentFrame + 1) % totalFrames;
                }
            }

            // No salir del área
            if (Position.X < 0)
                Position.X = 0;
            if (Position.X > maxGameArea)
                Position.X = maxGameArea;
        }

        public void Draw()
        {
            Texture2D[] frames = GetCurrentFrames();
            Texture2D frame = frames[currentFrame];

            float scale = 4.0f;

            Vector2 drawPos = new Vector2(
                Position.X - (frame.Width * scale) / 2,
                Position.Y - (frame.Height * scale)
            );

            if (facingLeft)
            {
                Rectangle source = new Rectangle(0, 0, -frame.Width, frame.Height);
                Rectangle dest = new Rectangle(drawPos.X, drawPos.Y, frame.Width * scale, frame.Height * scale);
                Raylib.DrawTexturePro(frame, source, dest, Vector2.Zero, 0f, Color.White);
            }
            else
            
            {
                Raylib.DrawTextureEx(frame, drawPos, 0f, scale, Color.White);
            }
        }

        private Texture2D[] GetCurrentFrames()
        {
            return state switch
            {
                "RUN" => runFrames,
                "JUMP" => jumpFrames,
                _ => idleFrames,
            };
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(Position.X - 22, Position.Y - 34, 44, 34);
        }

        public void CaptureToken(int tokenValue)
        {
            CapturedTokens.Add(tokenValue);

            var challenge = ChallengeManager.GetActiveChallenge();
            if (challenge == null) return;

            if (challenge.TargetTree == Challenge.TreeType.BST)
                TreeBST.Insert(tokenValue);
            else if (challenge.TargetTree == Challenge.TreeType.AVL)
                TreeAVL.Insert(tokenValue);
        }

        public void IncrementChallenges()
        {
            CompletedChallenges++;
        }

    }
}
