using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmashTrees.Entities
{
    public interface IPlayerControls
    {
        bool MoveRight();
        bool MoveLeft();
        bool JumpPressed();
        bool AttackPressed();
    }
}
