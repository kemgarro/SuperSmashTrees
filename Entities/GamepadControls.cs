using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace SuperSmashTrees.Entities
{
    public class GamepadControls : IPlayerControls
    {
        private readonly int gamepadId;

        public GamepadControls(int gamepadId)
        {
            this.gamepadId = gamepadId;
        }

        public bool MoveRight()
        {
            return Raylib.IsGamepadAvailable(gamepadId) &&
                   Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX) > 0.3f;
        }

        public bool MoveLeft()
        {
            return Raylib.IsGamepadAvailable(gamepadId) &&
                   Raylib.GetGamepadAxisMovement(gamepadId, GamepadAxis.LeftX) < -0.3f;
        }

        public bool JumpPressed()
        {
            return Raylib.IsGamepadAvailable(gamepadId) &&
                   Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.RightFaceDown); // Botón X en mando PS5
        }

        public bool AttackPressed()
        {
            return Raylib.IsGamepadAvailable(gamepadId) &&
                   Raylib.IsGamepadButtonDown(gamepadId, GamepadButton.RightFaceLeft);
        }
    }
}

