using Raylib_cs;

namespace SuperSmashTrees.UI
{
    public class CharacterOption
    {
        public string Name { get; }
        public Texture2D Icon { get; }
        public int IdleFrames { get; }
        public int RunFrames { get; }
        public int JumpFrames { get; }
        public int AttackFrames { get; }          // 🆕

        public CharacterOption(string name, Texture2D icon,
                               int idleFrames, int runFrames,
                               int jumpFrames, int attackFrames)
        {
            Name = name;
            Icon = icon;
            IdleFrames = idleFrames;
            RunFrames = runFrames;
            JumpFrames = jumpFrames;
            AttackFrames = attackFrames;               // 🆕
        }
    }
}
