namespace SuperSmashTrees.Structures
{
    public class Challenge
    {
        public enum TreeType { BST, AVL }
        public enum GoalType { MaxHeight, MinHeight, NodeCount }

        public TreeType TargetTree;
        public GoalType Goal;
        public int TargetValue;

        public Challenge(TreeType tree, GoalType goal, int target)
        {
            TargetTree = tree;
            Goal = goal;
            TargetValue = target;
        }
    }
}
