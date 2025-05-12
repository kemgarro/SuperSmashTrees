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
            var newNode = new Node<T>(value);
            if (head == null)
                head = tail = newNode;
            else
            {
                tail!.Next = newNode;
                tail = newNode;
            }
            count++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            if (index == 0)
            {
                head = head!.Next;
                if (head == null) tail = null;
            }
            else
            {
                var current = head;
                for (int i = 0; i < index - 1; i++)
                    current = current!.Next;

                current!.Next = current.Next?.Next;
                if (current.Next == null)
                    tail = current;
            }

            count--;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            var current = head;
            for (int i = 0; i < index; i++)
                current = current!.Next;

            return current!.Value;
        }

        public void Clear()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public int Count => count;
    }
}
