using Raylib_cs;

namespace SuperSmashTrees.Entities
{
    public class PlayerControls
    {
        private KeyboardKey moveRightKey;
        private KeyboardKey moveLeftKey;
        private KeyboardKey jumpKey;

        public PlayerControls(KeyboardKey right, KeyboardKey left, KeyboardKey jump)
        {
            moveRightKey = right;
            moveLeftKey = left;
            jumpKey = jump;
        }

        public bool MoveRight()
        {
            return Raylib.IsKeyDown(moveRightKey);
        }

        public bool MoveLeft()
        {
            return Raylib.IsKeyDown(moveLeftKey);
        }

        public bool JumpPressed()
        {
            return Raylib.IsKeyPressed(jumpKey);
        }
    }
}
