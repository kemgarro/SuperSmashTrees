using Raylib_cs;
using SuperSmashTrees.Core;
using SuperSmashTrees.Structures;
using System.Numerics;

namespace SuperSmashTrees.Entities
{
    public class Player
    {
        // ░░░ Animaciones ░░░
        private readonly Texture2D[] idleFrames;
        private readonly Texture2D[] runFrames;
        private readonly Texture2D[] jumpFrames;
        private readonly Texture2D[] attackFrames; // 🆕

        private float animationTimer;
        private int currentFrame;
        private readonly float frameTime = 0.07f;

        // ░░░ Física ░░░
        private readonly float gravity = 900f;
        private readonly float jumpForce = -800f;
        private float velocityY = 0f;
        private bool isJumping = false;
        private bool isAttacking = false; // 🆕

        // ░░░ Propiedades públicas ░░░
        public Vector2 Position;
        public float Speed = 300f;
        public string state = "IDLE";
        private bool facingLeft = false;

        private readonly PlayerControls controls;

        public int CompletedChallenges { get; private set; } = 0;
        public SuperSmashTrees.Structures.List<int> CapturedTokens { get; } = new SuperSmashTrees.Structures.List<int>();
        public BST TreeBST { get; set; } = new BST();
        public AVLTree TreeAVL { get; set; } = new AVLTree();
        public ChallengeManager ChallengeManager { get; } = new ChallengeManager();

        // ░░░ Carga de sprites ░░░
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

        // ░░░ Lógica principal ░░░
        public void Update(float delta, SuperSmashTrees.Structures.List<Platform> platforms,
                           Player? otherPlayer = null, float maxGameArea = 1920f)
        {
            bool moving = false;
            string prevState = state;

            // ◄ Ataque ►
            if (controls.AttackPressed() && !isAttacking)
            {
                state = "ATTACK";
                isAttacking = true;
                currentFrame = 0;
                animationTimer = 0f;
            }

            // ◄ Movimiento (si no ataca) ►
            if (!isAttacking)
            {
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
            }

            // Gravedad
            velocityY += gravity * delta;
            Position.Y += velocityY * delta;

            // Colisiones con plataformas
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

                        if (!moving && !isAttacking)
                            state = "IDLE";
                    }
                }
            }

            if (!onPlatform && velocityY > 0 && !isAttacking)
                state = "JUMP";

            // ░░░ Animaciones ░░░
            if (state != prevState)
            {
                currentFrame = 0;
                animationTimer = 0f;
            }

            animationTimer += delta;
            if (animationTimer >= frameTime)
            {
                animationTimer = 0f;
                int total = GetCurrentFrames().Length;

                if (state == "ATTACK")
                {
                    if (currentFrame < total - 1)
                        currentFrame++;
                    else // Fin del ataque
                    {
                        isAttacking = false;
                        state = isJumping ? "JUMP" : moving ? "RUN" : "IDLE";
                        currentFrame = 0;
                    }
                }
                else if (state == "JUMP")
                {
                    if (currentFrame < jumpFrames.Length - 3)
                        currentFrame++;
                    else
                    {
                        currentFrame++;
                        if (currentFrame >= jumpFrames.Length)
                            currentFrame = jumpFrames.Length - 3;
                    }
                }
                else // RUN o IDLE
                {
                    currentFrame = (currentFrame + 1) % total;
                }
            }

            // Límites del escenario
            if (Position.X < 0) Position.X = 0;
            if (Position.X > maxGameArea) Position.X = maxGameArea;

            // ░░░ Colisión con la espada (área de ataque) ░░░
            if (isAttacking && otherPlayer != null)
            {
                // Solo empuja después de que la animación haya terminado
                if (currentFrame == attackFrames.Length - 1)
                {
                    // Define el área de colisión de la espada a ambos lados, y la hace más grande
                    Rectangle swordCollisionArea = GetSwordCollisionArea();

                    // Si hay colisión con el otro jugador, empujamos al otro jugador
                    if (Raylib.CheckCollisionRecs(swordCollisionArea, otherPlayer.GetBounds()))
                    {
                        float pushForce = 150f; // Controla la fuerza del empuje
                        if (facingLeft)
                        {
                            otherPlayer.Position.X -= pushForce;  // Empuja hacia la izquierda
                        }
                        else
                        {
                            otherPlayer.Position.X += pushForce;  // Empuja hacia la derecha
                        }
                    }
                }
            }
        }

        // ░░░ Generar área de colisión para la espada (más grande) ░░░
        private Rectangle GetSwordCollisionArea()
        {
            // Aumentamos el tamaño del hitbox (más grande)
            float width = 100f; // Aumentamos el ancho del área de la espada
            float height = 40f; // Aumentamos la altura del área de la espada

            // Si el jugador está mirando a la izquierda, la espada estará a la izquierda de él
            float xOffset = facingLeft ? Position.X - width : Position.X + 22; // Ajusta la posición de la espada
            float yOffset = Position.Y - 30; // Ajusta la posición de la espada en el eje Y

            return new Rectangle(xOffset, yOffset, width, height);
        }

        // ░░░ Render ░░░
        public void Draw()
        {
            var frames = GetCurrentFrames();
            Texture2D frame = frames[currentFrame];

            float scale = 4f;
            Vector2 drawPos = new Vector2(
                Position.X - (frame.Width * scale) / 2,
                Position.Y - (frame.Height * scale)
            );

            if (facingLeft)
            {
                Rectangle src = new Rectangle(0, 0, -frame.Width, frame.Height);
                Rectangle dest = new Rectangle(drawPos.X, drawPos.Y, frame.Width * scale, frame.Height * scale);
                Raylib.DrawTexturePro(frame, src, dest, Vector2.Zero, 0f, Color.White);
            }
            else
            {
                Raylib.DrawTextureEx(frame, drawPos, 0f, scale, Color.White);
            }
        }

        // ░░░ Utilidades ░░░
        private Texture2D[] GetCurrentFrames() => state switch
        {
            "RUN" => runFrames,
            "JUMP" => jumpFrames,
            "ATTACK" => attackFrames,
            _ => idleFrames,
        };

        public Rectangle GetBounds() => new Rectangle(Position.X - 22, Position.Y - 34, 44, 34);

        // ░░░ Reto y tokens ░░░
        public void CaptureToken(int tokenValue)
        {
            CapturedTokens.Add(tokenValue);
            var challenge = ChallengeManager.GetActiveChallenge();
            if (challenge == null) return;
            if (challenge.TargetTree == Challenge.TreeType.BST) TreeBST.Insert(tokenValue);
            if (challenge.TargetTree == Challenge.TreeType.AVL) TreeAVL.Insert(tokenValue);
        }

        public void IncrementChallenges() => CompletedChallenges++;
    }
}
