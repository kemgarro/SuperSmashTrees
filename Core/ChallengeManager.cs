using SuperSmashTrees.Entities;
using SuperSmashTrees.Structures;

namespace SuperSmashTrees.Core
{
    public class ChallengeManager
    {
        private SuperSmashTrees.Structures.List<Challenge> challenges;
        private Challenge? currentChallenge;
        private Random rng = new Random();

        public ChallengeManager()
        {
            challenges = new SuperSmashTrees.Structures.List<Challenge>();
            LoadChallenges();
            AdvanceChallenge();
        }

        public Challenge? GetActiveChallenge() => currentChallenge;

        public void AdvanceChallenge()
        {
            if (challenges.Count == 0) return;

            int index = rng.Next(challenges.Count);
            currentChallenge = challenges.Get(index);
            challenges = RemoveAt(challenges, index);
        }

        private void LoadChallenges()
        {
            challenges.Add(new Challenge(Challenge.TreeType.BST, Challenge.GoalType.MaxHeight, 5));
            challenges.Add(new Challenge(Challenge.TreeType.BST, Challenge.GoalType.MaxHeight, 6));
            challenges.Add(new Challenge(Challenge.TreeType.BST, Challenge.GoalType.NodeCount, 8));
            challenges.Add(new Challenge(Challenge.TreeType.BST, Challenge.GoalType.NodeCount, 10));
            challenges.Add(new Challenge(Challenge.TreeType.BST, Challenge.GoalType.NodeCount, 15));
            challenges.Add(new Challenge(Challenge.TreeType.BST, Challenge.GoalType.MinHeight, 4));

            challenges.Add(new Challenge(Challenge.TreeType.AVL, Challenge.GoalType.MaxHeight, 4));
            challenges.Add(new Challenge(Challenge.TreeType.AVL, Challenge.GoalType.MaxHeight, 5));
            challenges.Add(new Challenge(Challenge.TreeType.AVL, Challenge.GoalType.NodeCount, 10));
            challenges.Add(new Challenge(Challenge.TreeType.AVL, Challenge.GoalType.NodeCount, 15));
            challenges.Add(new Challenge(Challenge.TreeType.AVL, Challenge.GoalType.MinHeight, 4));
            challenges.Add(new Challenge(Challenge.TreeType.AVL, Challenge.GoalType.MinHeight, 5));
        }

        public bool ValidateChallenge(Challenge challenge, Player player)
        {
            int result = 0;

            if (challenge.TargetTree == Challenge.TreeType.BST)
                result = GetTreeStat(player.TreeBST.Root, challenge.Goal);
            else if (challenge.TargetTree == Challenge.TreeType.AVL)
                result = GetTreeStat(player.TreeAVL.Root, challenge.Goal);

            return challenge.Goal switch
            {
                Challenge.GoalType.MaxHeight => result <= challenge.TargetValue,
                Challenge.GoalType.MinHeight => result >= challenge.TargetValue,
                Challenge.GoalType.NodeCount => result == challenge.TargetValue,
                _ => false,
            };
        }

        private int GetTreeStat(object? node, Challenge.GoalType goal)
        {
            if (node == null) return 0;

            if (node is BSTNode bst)
            {
                return goal == Challenge.GoalType.NodeCount
                    ? 1 + GetTreeStat(bst.Left, goal) + GetTreeStat(bst.Right, goal)
                    : 1 + Math.Max(GetTreeStat(bst.Left, goal), GetTreeStat(bst.Right, goal));
            }

            if (node is AVLNode avl)
            {
                return goal == Challenge.GoalType.NodeCount
                    ? 1 + GetTreeStat(avl.Left, goal) + GetTreeStat(avl.Right, goal)
                    : 1 + Math.Max(GetTreeStat(avl.Left, goal), GetTreeStat(avl.Right, goal));
            }

            return 0;
        }

        private SuperSmashTrees.Structures.List<Challenge> RemoveAt(SuperSmashTrees.Structures.List<Challenge> list, int index)
        {
            var nueva = new SuperSmashTrees.Structures.List<Challenge>();
            for (int i = 0; i < list.Count; i++)
            {
                if (i != index)
                    nueva.Add(list.Get(i));
            }
            return nueva;
        }
    }
}
