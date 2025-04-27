using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmashTrees.Structures
{
    public class AVLNode
    {
        public int Value;
        public AVLNode? Left;
        public AVLNode? Right;
        public int Height;

        public AVLNode(int value)
        {
            Value = value;
            Left = null;
            Right = null;
            Height = 1; // Nuevo nodo empieza con altura 1
        }
    }
}
