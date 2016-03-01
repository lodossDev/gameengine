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

        public static Buttons GetDirectionInput(GamePadState gamePad, KeyboardState keyboard)
        {
            Buttons direction = None;

            // Get vertical direction.
            if (gamePad.IsButtonDown(Buttons.DPadUp) ||
                    gamePad.IsButtonDown(Buttons.LeftThumbstickUp) ||
                        keyboard.IsKeyDown(Keys.Up))
            {
                direction |= Up;
            }
            else if (gamePad.IsButtonDown(Buttons.DPadDown) ||
                        gamePad.IsButtonDown(Buttons.LeftThumbstickDown) ||
                            keyboard.IsKeyDown(Keys.Down))
            {
                direction |= Down;
            }

            // Comebine with horizontal direction.
            if (gamePad.IsButtonDown(Buttons.DPadLeft) ||
                    gamePad.IsButtonDown(Buttons.LeftThumbstickLeft) ||
                        keyboard.IsKeyDown(Keys.Left))
            {
                direction |= Left;
            }
            else if (gamePad.IsButtonDown(Buttons.DPadRight) ||
                        gamePad.IsButtonDown(Buttons.LeftThumbstickRight) ||
                            keyboard.IsKeyDown(Keys.Right))
            {
                direction |= Right;
            }

            return direction;
        }

        public static Buttons GetButtonsPressed(GamePadState oldPadState, KeyboardState oldKeyboardState, 
                                                GamePadState newPadState, KeyboardState newKeyboardState)
        {
            Buttons buttons = None;

            foreach (var buttonAndKey in NonDirectionButtons)
            {
                Buttons button = buttonAndKey.Key;
                Keys key = buttonAndKey.Value;

                // Check the game pad and keyboard for presses.
                if (oldPadState.IsButtonUp(button) && newPadState.IsButtonDown(button) 
                        || oldKeyboardState.IsKeyUp(key) && newKeyboardState.IsKeyDown(key))
                {
                    // Use a bitwise-or to accumulate button presses.
                    buttons |= button;
                }
            }

            return buttons;
        }
    }
}
