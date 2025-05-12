namespace SuperSmashTrees.Structures
{
    public class BST
    {
        public BSTNode? Root;

        public void Insert(int value)
        {
            Root = InsertRecursive(Root, value);
        }

        private BSTNode InsertRecursive(BSTNode? node, int value)
        {
            if (node == null)
                return new BSTNode(value);

            if (value < node.Value)
                node.Left = InsertRecursive(node.Left, value);
            else if (value > node.Value)
                node.Right = InsertRecursive(node.Right, value);

            return node;
        }
    }
}
