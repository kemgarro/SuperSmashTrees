using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmashTrees.Core
{
    public class PlayerControls
    {
        public KeyboardKey Left;
        public KeyboardKey Right;
        public KeyboardKey Jump;

        public PlayerControls(KeyboardKey left, KeyboardKey right, KeyboardKey jump)
        {
            Left = left;
            Right = right;
            Jump = jump;
        }
    }
}
