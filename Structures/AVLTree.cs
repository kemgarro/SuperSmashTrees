using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmashTrees.Structures
{
    public class AVLTree
    {
        public AVLNode? Root;

        public void Insert(int value)
        {
            Root = InsertRecursive(Root, value);
        }

        private AVLNode InsertRecursive(AVLNode? node, int value)
        {
            if (node == null)
                return new AVLNode(value);

            if (value < node.Value)
                node.Left = InsertRecursive(node.Left, value);
            else if (value > node.Value)
                node.Right = InsertRecursive(node.Right, value);
            else
                return node; // No se permiten valores duplicados

            // Actualizar altura
            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

            // Obtener balance
            int balance = GetBalance(node);

            // Rotaciones
            if (balance > 1 && value < node.Left!.Value)
                return RightRotate(node);

            if (balance < -1 && value > node.Right!.Value)
                return LeftRotate(node);

            if (balance > 1 && value > node.Left!.Value)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            if (balance < -1 && value < node.Right!.Value)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        private int GetHeight(AVLNode? node)
        {
            if (node == null)
                return 0;
            return node.Height;
        }

        private int GetBalance(AVLNode? node)
        {
            if (node == null)
                return 0;
            return GetHeight(node.Left) - GetHeight(node.Right);
        }

        private AVLNode RightRotate(AVLNode y)
        {
            AVLNode x = y.Left!;
            AVLNode T2 = x.Right!;

            // Rotación
            x.Right = y;
            y.Left = T2;

            // Actualizar alturas
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            // Nueva raíz
            return x;
        }

        private AVLNode LeftRotate(AVLNode x)
        {
            AVLNode y = x.Right!;
            AVLNode T2 = y.Left!;

            // Rotación
            y.Left = x;
            x.Right = T2;

            // Actualizar alturas
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            // Nueva raíz
            return y;
        }
    }
}
