using Raylib_cs;
using System.Numerics;
using SuperSmashTrees.Structures;

namespace SuperSmashTrees.Draw
{
    public static class TreeDrawer
    {
        public static void DrawBST(BSTNode? node, float x, float y, float horizontalSpacing)
        {
            if (node == null) return;

            DrawNode(node.Value, x, y, Color.Green);

            if (node.Left != null)
            {
                float newX = x - horizontalSpacing;
                float newY = y + 60;
                Raylib.DrawLine((int)x, (int)y, (int)newX, (int)newY, Color.White);
                DrawBST(node.Left, newX, newY, horizontalSpacing * 0.6f);
            }

            if (node.Right != null)
            {
                float newX = x + horizontalSpacing;
                float newY = y + 60;
                Raylib.DrawLine((int)x, (int)y, (int)newX, (int)newY, Color.White);
                DrawBST(node.Right, newX, newY, horizontalSpacing * 0.6f);
            }
        }

        public static void DrawAVL(AVLNode? node, float x, float y, float horizontalSpacing)
        {
            if (node == null) return;

            DrawNode(node.Value, x, y, Color.Blue);

            if (node.Left != null)
            {
                float newX = x - horizontalSpacing;
                float newY = y + 60;
                Raylib.DrawLine((int)x, (int)y, (int)newX, (int)newY, Color.White);
                DrawAVL(node.Left, newX, newY, horizontalSpacing * 0.6f);
            }

            if (node.Right != null)
            {
                float newX = x + horizontalSpacing;
                float newY = y + 60;
                Raylib.DrawLine((int)x, (int)y, (int)newX, (int)newY, Color.White);
                DrawAVL(node.Right, newX, newY, horizontalSpacing * 0.6f);
            }
        }

        private static void DrawNode(int value, float x, float y, Color color)
        {
            Raylib.DrawCircle((int)x, (int)y, 15, color);
            string text = value.ToString();
            int textWidth = Raylib.MeasureText(text, 20);
            Raylib.DrawText(text, (int)(x - textWidth / 2), (int)(y - 10), 20, Color.Black);
        }
    }
}
