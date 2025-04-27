using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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

        private string state = "IDLE"; // o "RUN"
        private bool facingLeft = false;

        public SuperSmashTrees.Structures.List<int> CapturedTokens { get; private set; } = new SuperSmashTrees.Structures.List<int>();
        public SuperSmashTrees.Structures.BST TreeBST { get; private set; } = new SuperSmashTrees.Structures.BST();
        public SuperSmashTrees.Structures.AVLTree TreeAVL { get; private set; } = new SuperSmashTrees.Structures.AVLTree();


        public Player(string idlePath, string runPath, string jumpPath, int idleCount, int runCount, int jumpCount, Vector2 startPosition)
        {
            idleFrames = LoadFrames(idlePath, idleCount, "IDLE");
            runFrames = LoadFrames(runPath, runCount, "RUN");
            jumpFrames = LoadFrames(jumpPath, jumpCount, "JUMP");
            int jumpLoopStart = jumpFrames.Length - 3; // últimos 3
            Position = startPosition;
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

        public void Update(float delta, SuperSmashTrees.Structures.List<Platform> platforms,
                   KeyboardKey rightKey, KeyboardKey leftKey, KeyboardKey jumpKey,
                   Player? otherPlayer = null, float maxGameArea = 1920f)
        {
            bool moving = false;
            string prevState = state;

            if (Raylib.IsKeyDown(rightKey))
            {
                Position.X += Speed * delta;
                if (otherPlayer != null && Raylib.CheckCollisionRecs(GetBounds(), otherPlayer.GetBounds()))
                {
                    Position.X -= 5f; // Empuje pequeño hacia la izquierda
                    otherPlayer.Position.X += 5f; // Y al otro lo empujo un poquito a la derecha
                }
                else
                {
                    state = "RUN";
                    facingLeft = false;
                    moving = true;
                }
            }


            else if (Raylib.IsKeyDown(leftKey))
            {
                Position.X -= Speed * delta;
                if (otherPlayer != null && Raylib.CheckCollisionRecs(GetBounds(), otherPlayer.GetBounds()))
                {
                    Position.X += 5f; // Empuje pequeño hacia la derecha
                    otherPlayer.Position.X -= 5f; // Y al otro lo empujo un poquito a la izquierda
                }
                else
                {
                    state = "RUN";
                    facingLeft = true;
                    moving = true;
                }
            }



            if (Raylib.IsKeyPressed(jumpKey) && !isJumping)
            {
                velocityY = jumpForce;
                isJumping = true;
                state = "JUMP";
            }

            // Aplicar gravedad
            velocityY += gravity * delta;
            Position.Y += velocityY * delta;

            // Revisar colisiones con plataformas (solo por arriba)
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

            // Si cambió de estado, reiniciamos la animación
            if (state != prevState)
            {
                currentFrame = 0;
                animationTimer = 0f;
            }

            // Animación
            animationTimer += delta;
            if (animationTimer >= frameTime)
            {
                animationTimer = 0f;

                int totalFrames = GetCurrentFrames().Length;

                if (state == "JUMP")
                {
                    // 1. Si aún no llegamos al loop, avanzar normalmente
                    if (currentFrame < jumpFrames.Length - 3)
                    {
                        currentFrame++;
                    }
                    else
                    {
                        // 2. Ciclar entre los últimos 3 frames
                        currentFrame++;
                        if (currentFrame >= jumpFrames.Length)
                        {
                            currentFrame = jumpFrames.Length - 3;
                        }
                    }
                }
                else
                {
                    // IDLE y RUN se ciclan normalmente
                    currentFrame = (currentFrame + 1) % totalFrames;
                }
            }
            // Limitar movimiento: no pasarse del área de juego
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
                Rectangle source = new Rectangle(0, 0, -frame.Width, frame.Height); // ❗voltear en eje X
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
            TreeBST.Insert(tokenValue); // Insertar en BST normal
            TreeAVL.Insert(tokenValue); // Insertar en AVL balanceado
        }




    }
}
