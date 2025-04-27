using Raylib_cs;
using System.Numerics;
using SuperSmashTrees.Structures;

namespace SuperSmashTrees.Draw
{
    public static class TreeDrawer
    {
        public static void DrawBST(BSTNode? node, float x, float y, float horizontalSpacing)
        {
            if (node == null)
                return;

            Raylib.DrawCircle((int)x, (int)y, 15, Color.Green);
            string valueText = node.Value.ToString();
            int textWidth = Raylib.MeasureText(valueText, 20);
            Raylib.DrawText(valueText, (int)(x - textWidth / 2), (int)(y - 10), 20, Color.Black);

            if (node.Left != null)
            {
                Raylib.DrawLine((int)x, (int)y, (int)(x - horizontalSpacing), (int)(y + 60), Color.White);
                DrawBST(node.Left, x - horizontalSpacing, y + 60, horizontalSpacing * 0.6f);
            }
            if (node.Right != null)
            {
                Raylib.DrawLine((int)x, (int)y, (int)(x + horizontalSpacing), (int)(y + 60), Color.White);
                DrawBST(node.Right, x + horizontalSpacing, y + 60, horizontalSpacing * 0.6f);
            }
        }

        public static void DrawAVL(AVLNode? node, float x, float y, float horizontalSpacing)
        {
            if (node == null)
                return;

            Raylib.DrawCircle((int)x, (int)y, 15, Color.Blue);
            string valueText = node.Value.ToString();
            int textWidth = Raylib.MeasureText(valueText, 20);
            Raylib.DrawText(valueText, (int)(x - textWidth / 2), (int)(y - 10), 20, Color.Black);

            if (node.Left != null)
            {
                Raylib.DrawLine((int)x, (int)y, (int)(x - horizontalSpacing), (int)(y + 60), Color.White);
                DrawAVL(node.Left, x - horizontalSpacing, y + 60, horizontalSpacing * 0.6f);
            }
            if (node.Right != null)
            {
                Raylib.DrawLine((int)x, (int)y, (int)(x + horizontalSpacing), (int)(y + 60), Color.White);
                DrawAVL(node.Right, x + horizontalSpacing, y + 60, horizontalSpacing * 0.6f);
            }
        }
    }
}
