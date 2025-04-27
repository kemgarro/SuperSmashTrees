using SuperSmashTrees.Entities;
using SuperSmashTrees.Structures;

namespace SuperSmashTrees.Core
{
    public class ChallengeManager
    {
        private SuperSmashTrees.Structures.List<Challenge> challenges;
        private int currentChallengeIndex;

        public ChallengeManager()
        {
            challenges = new SuperSmashTrees.Structures.List<Challenge> ();
            currentChallengeIndex = 0;
            LoadChallenges();
        }

        public Challenge? GetActiveChallenge()
        {
            if (currentChallengeIndex < challenges.Count)
                return challenges.Get(currentChallengeIndex);
            else
                return null;
        }

        public void AdvanceChallenge()
        {
            currentChallengeIndex++;
        }

        private void LoadChallenges()
        {
            challenges.Add(new Challenge(Challenge.TreeType.BST, Challenge.GoalType.MaxHeight, 5));
            challenges.Add(new Challenge(Challenge.TreeType.BST, Challenge.GoalType.NodeCount, 10));
            challenges.Add(new Challenge(Challenge.TreeType.AVL, Challenge.GoalType.MaxHeight, 4));
            challenges.Add(new Challenge(Challenge.TreeType.AVL, Challenge.GoalType.NodeCount, 15));
        }

        public bool ValidateChallenge(Challenge challenge, Player player)
        {
            int result = 0;

            if (challenge.TargetTree == Challenge.TreeType.BST)
            {
                result = GetTreeStat(player.TreeBST.Root, challenge.Goal);
            }
            else if (challenge.TargetTree == Challenge.TreeType.AVL)
            {
                result = GetTreeStat(player.TreeAVL.Root, challenge.Goal);
            }

            if (challenge.Goal == Challenge.GoalType.MaxHeight)
                return result <= challenge.TargetValue;
            else if (challenge.Goal == Challenge.GoalType.MinHeight)
                return result >= challenge.TargetValue;
            else if (challenge.Goal == Challenge.GoalType.NodeCount)
                return result == challenge.TargetValue;

            return false;
        }

        private int GetTreeStat(object? node, Challenge.GoalType goal)
        {
            if (node == null)
                return 0;

            if (node is BSTNode bstNode)
            {
                if (goal == Challenge.GoalType.MaxHeight || goal == Challenge.GoalType.MinHeight)
                    return 1 + Math.Max(GetTreeStat(bstNode.Left, goal), GetTreeStat(bstNode.Right, goal));
                else if (goal == Challenge.GoalType.NodeCount)
                    return 1 + GetTreeStat(bstNode.Left, goal) + GetTreeStat(bstNode.Right, goal);
            }
            else if (node is AVLNode avlNode)
            {
                if (goal == Challenge.GoalType.MaxHeight || goal == Challenge.GoalType.MinHeight)
                    return 1 + Math.Max(GetTreeStat(avlNode.Left, goal), GetTreeStat(avlNode.Right, goal));
                else if (goal == Challenge.GoalType.NodeCount)
                    return 1 + GetTreeStat(avlNode.Left, goal) + GetTreeStat(avlNode.Right, goal);
            }

            return 0;
        }
    }
}
