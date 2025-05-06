using Raylib_cs;

namespace SuperSmashTrees.Entities
{
    public class PlayerControls
    {
        private readonly KeyboardKey moveRightKey;
        private readonly KeyboardKey moveLeftKey;
        private readonly KeyboardKey jumpKey;
        private readonly KeyboardKey attackKey;

        public PlayerControls(KeyboardKey right, KeyboardKey left,
                              KeyboardKey jump, KeyboardKey attack)
        {
            moveRightKey = right;
            moveLeftKey = left;
            jumpKey = jump;
            attackKey = attack;
        }

        public bool MoveRight() => Raylib.IsKeyDown(moveRightKey);
        public bool MoveLeft() => Raylib.IsKeyDown(moveLeftKey);
        public bool JumpPressed() => Raylib.IsKeyPressed(jumpKey);
        public bool AttackPressed() => Raylib.IsKeyPressed(attackKey);
    }
}
