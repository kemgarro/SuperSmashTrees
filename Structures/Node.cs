using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmashTrees.Structures
{
    public class Node<T>
    {
        public T Value;
        public Node<T>? Next; // Allow Next to be nullable

        public Node(T value)
        {
            Value = value;
            Next = null; // This is now valid because Next is nullable
        }
    }
}
