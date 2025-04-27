using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SuperSmashTrees.Structures
{
    public class BSTNode
    {
        public int Value;
        public BSTNode? Left;
        public BSTNode? Right;

        public BSTNode(int value)
        {
            Value = value;
            Left = null;
            Right = null;
        }
    }
}
