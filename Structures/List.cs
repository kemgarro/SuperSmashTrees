using System;
using System.Linq;
using System.Text;

namespace SuperSmashTrees.Structures
{
    public class List<T>
    {
        private Node<T>? head;
        private Node<T>? tail;
        private int count;

        public List()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public void Add(T value)
        {
            Node<T> newNode = new Node<T>(value);
            if (head == null)
                head = tail = newNode;
            else
            {
                tail!.Next = newNode; // Use null-forgiving operator (!) since tail is checked for null.
                tail = newNode;
            }
            count++;
        }
        public void Clear()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            Node<T>? current = head;
            for (int i = 0; i < index; i++)
                current = current!.Next; // Use null-forgiving operator (!) since current is checked for null.

            return current!.Value; // Use null-forgiving operator (!) since current is guaranteed to be non-null.
        }

        public int Count => count;
    }
}
