using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmashTrees.UI
{
    public class CharacterOption
    {
        public string Name { get; }
        public Texture2D Icon { get; }
        public int IdleFrames { get; }
        public int RunFrames { get; }

        public int JumpFrames { get; }
        public CharacterOption(string name, Texture2D icon, int idleFrames, int runFrames, int jumpFrames)
        {
            Name = name;
            Icon = icon;
            IdleFrames = idleFrames;
            RunFrames = runFrames;
            JumpFrames = jumpFrames;
        }
    }
}
