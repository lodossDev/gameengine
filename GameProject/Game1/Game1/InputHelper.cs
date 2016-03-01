using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public static class InputHelper
    {
        public enum Key
        {
            NONE = 0,
            UP = 222,
            DOWN = 23,
            LEFT = 554,
            RIGHT = 225,
            UP_LEFT = UP | LEFT,
            UP_RIGHT = UP | RIGHT,
            DOWN_LEFT = DOWN | LEFT,
            DOWN_RIGHT = DOWN | RIGHT,

            A = 2221,
            B = 3331,
            C = 4441,
            X = 5555,
            Y = 6661,
            Z = 7771,

            START = 8881,
            PAUSE = 9991,
            ANY_DIRECTION = UP | DOWN | LEFT | RIGHT,
        }

        public const Buttons None = 0;
        public const Buttons Up = Buttons.DPadUp | Buttons.LeftThumbstickUp;
        public const Buttons Down = Buttons.DPadDown | Buttons.LeftThumbstickDown;
        public const Buttons Left = Buttons.DPadLeft | Buttons.LeftThumbstickLeft;
        public const Buttons Right = Buttons.DPadRight | Buttons.LeftThumbstickRight;
        public const Buttons UpLeft = Up | Left;
        public const Buttons UpRight = Up | Right;
        public const Buttons DownLeft = Down | Left;
        public const Buttons DownRight = Down | Right;
        public const Buttons Any = Up | Down | Left | Right;

        internal static readonly Dictionary<Buttons, Keys> NonDirectionButtons = new Dictionary<Buttons, Keys>
        {
            { Buttons.A, Keys.A },
            { Buttons.B, Keys.B },
            { Buttons.X, Keys.X },
            { Buttons.Y, Keys.Y },
            // Other available non-direction buttons:
            // Start, Back, LeftShoulder, LeftTrigger, LeftStick,
            // RightShoulder, RightTrigger, and RightStick.
        };

        internal static readonly Dictionary<Buttons, InputHelper.Key> NonDirectionButtonKeyMap = new Dictionary<Buttons, InputHelper.Key>
        {
            { Buttons.A, InputHelper.Key.A },
            { Buttons.B, InputHelper.Key.B },
            { Buttons.X, InputHelper.Key.X },
            { Buttons.Y, InputHelper.Key.Y },
        };

        internal static readonly Dictionary<Keys, InputHelper.Key> NonDirectionButtonKeysMap = new Dictionary<Keys, InputHelper.Key>
        {
            { Keys.A, InputHelper.Key.A },
            { Keys.B, InputHelper.Key.B },
            { Keys.X, InputHelper.Key.X },
            { Keys.Y, InputHelper.Key.Y },
        };

        public class Move
        {
            private Buttons input;
            private float time;

            public Move(Buttons input, float time = 100f)
            {
                this.input = input;
                this.time = time;
            }

            public Buttons GetInput()
            {
                return input;
            }

            public float GetTime()
            {
                return time;
            }
        }

        public static InputHelper.Key GetPressedDirections(GamePadState oldPadState, KeyboardState oldKeyboardState,
                                                GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.Key directions = InputHelper.Key.NONE;

            // Get vertical direction.
            if (oldPadState.IsButtonUp(Buttons.DPadUp) && newPadState.IsButtonDown(Buttons.DPadUp) 
                    || oldPadState.IsButtonUp(Buttons.LeftThumbstickUp) && newPadState.IsButtonDown(Buttons.LeftThumbstickUp) 
                    || oldKeyboardState.IsKeyUp(Keys.Up) && newKeyboardState.IsKeyDown(Keys.Up))
            {
                directions |= InputHelper.Key.UP;
            }
            else if (oldPadState.IsButtonUp(Buttons.DPadDown) && newPadState.IsButtonDown(Buttons.DPadDown)
                        || oldPadState.IsButtonUp(Buttons.LeftThumbstickDown) && newPadState.IsButtonDown(Buttons.LeftThumbstickDown)
                        || oldKeyboardState.IsKeyUp(Keys.Down) && newKeyboardState.IsKeyDown(Keys.Down))
            {
                directions |= InputHelper.Key.DOWN;
            }

            // Comebine with horizontal direction.
            if (oldPadState.IsButtonUp(Buttons.DPadLeft) && newPadState.IsButtonDown(Buttons.DPadLeft)
                    || oldPadState.IsButtonUp(Buttons.LeftThumbstickLeft) && newPadState.IsButtonDown(Buttons.LeftThumbstickLeft)
                    || oldKeyboardState.IsKeyUp(Keys.Left) && newKeyboardState.IsKeyDown(Keys.Left))
            {
                directions |= InputHelper.Key.LEFT;
            }
            else if (oldPadState.IsButtonUp(Buttons.DPadRight) && newPadState.IsButtonDown(Buttons.DPadRight)
                        || oldPadState.IsButtonUp(Buttons.LeftThumbstickRight) && newPadState.IsButtonDown(Buttons.LeftThumbstickRight)
                        || oldKeyboardState.IsKeyUp(Keys.Right) && newKeyboardState.IsKeyDown(Keys.Right))
            {
                directions |= InputHelper.Key.RIGHT;
            }

            return directions;
        }

        public static InputHelper.Key GetPressedButtons(GamePadState oldPadState, KeyboardState oldKeyboardState, 
                                                GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.Key buttons = InputHelper.Key.NONE;

            foreach (var buttonAndKey in NonDirectionButtons)
            {
                Buttons button = buttonAndKey.Key;
                Keys key = buttonAndKey.Value;

                // Check the game pad and keyboard for presses.
                if (oldPadState.IsButtonUp(button) && newPadState.IsButtonDown(button) 
                        || oldKeyboardState.IsKeyUp(key) && newKeyboardState.IsKeyDown(key))
                {
                    // Use a bitwise-or to accumulate button presses.
                    buttons |= NonDirectionButtonKeyMap[button] | NonDirectionButtonKeysMap[key];
                }
            }

            return buttons;
        }
    }
}
