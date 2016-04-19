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
        [Flags]
        public enum KeyPress
        {
            NONE = 0,
            UP = 1,
            DOWN = 2,
            LEFT = 4,
            RIGHT = 8,
            UP_LEFT = UP | LEFT,
            UP_RIGHT = UP | RIGHT,
            DOWN_LEFT = DOWN | LEFT,
            DOWN_RIGHT = DOWN | RIGHT,

            A = 16,
            B = 32,
            C = 64,
            X = 128,
            Y = 256,
            Z = 512,

            START = 2048,
            PAUSE = 4096,
            ANY_DIRECTION = UP | DOWN | LEFT | RIGHT,
        }

        [Flags]
        public enum ButtonState
        {
            Pressed = 1,
            Released = 2,
            Held = 4
        }

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

        internal static readonly Dictionary<Buttons, InputHelper.KeyPress> NonDirectionButtonKeyMap = new Dictionary<Buttons, InputHelper.KeyPress>
        {
            { Buttons.A, InputHelper.KeyPress.A },
            { Buttons.B, InputHelper.KeyPress.B },
            { Buttons.X, InputHelper.KeyPress.X },
            { Buttons.Y, InputHelper.KeyPress.Y },
        };

        internal static readonly Dictionary<Keys, InputHelper.KeyPress> NonDirectionButtonKeysMap = new Dictionary<Keys, InputHelper.KeyPress>
        {
            { Keys.A, InputHelper.KeyPress.A },
            { Keys.B, InputHelper.KeyPress.B },
            { Keys.X, InputHelper.KeyPress.X },
            { Keys.Y, InputHelper.KeyPress.Y },
        };

        internal static readonly int NEGATIVE_EDGE_PRESS = 30;

        public class KeyState
        {
            private InputHelper.KeyPress key;
            private InputHelper.ButtonState state;
            private int negativeEdge = NEGATIVE_EDGE_PRESS;
            private float keyHeldTime = 0f;

            public KeyState(InputHelper.KeyPress key, InputHelper.ButtonState state, float keyHeldTime = 5, int negativeEdge = 30) {
                this.key = key;
                this.state = state;
                this.negativeEdge = negativeEdge;
                this.keyHeldTime = keyHeldTime;
            }

            public KeyState(InputHelper.KeyPress key, InputHelper.ButtonState state, int negativeEdge = 30)
            {
                this.key = key;
                this.state = state;
                this.negativeEdge = negativeEdge;
                this.keyHeldTime = 0f;
            }

            public KeyState(InputHelper.KeyPress key, InputHelper.ButtonState state)
            {
                this.key = key;
                this.state = state;
                this.keyHeldTime = 0f;
            }

            public InputHelper.KeyPress GetKey()
            {
                return key;
            } 

            public InputHelper.ButtonState GetState()
            {
                return state;
            }

            public int GetNegativeEdge()
            {
                return negativeEdge;
            }

            public float GetKeyHeldTime()
            {
                return keyHeldTime;
            }
        }

        public class CommandMove
        {
            private List<InputHelper.KeyState> moves;
            private string name;
            private double priority;
            public int currentMoveStep = 0;
            public float currentMoveTime = 0f;
            private float maxMoveTime = 500f;
            private int currentNegativeEdge = 0;
            private Animation.State animationState;


            public CommandMove(string name, Animation.State animationState, List<InputHelper.KeyState> moves, float maxMoveTime = 10000f, double priority = 1)
            {
                this.name = name;
                this.animationState = animationState;
                this.moves = moves;
                this.maxMoveTime = maxMoveTime;
                this.priority = priority;
            }

            public string GetName()
            {
                return name;
            }

            public List<InputHelper.KeyState> GetMoves()
            {
                return moves;
            }

            public double GetPriority()
            {
                return priority;
            }

            public Animation.State GetAnimationState()
            {
                return animationState;
            }

            public InputHelper.KeyState GetCurrentMove()
            {
                return moves[currentMoveStep];
            }

            public int GetCurrentMoveStep()
            {
                return currentMoveStep;
            }

            public int GetCurrentNegativeEdgeExpire()
            {
                return GetCurrentMove().GetNegativeEdge();
            }

            public int GetNegativeCount()
            {
                return currentNegativeEdge;
            }

            public bool IsMaxNegativeReached()
            {
                return (GetNegativeCount() >= GetCurrentNegativeEdgeExpire());
            }

            public void ResetNegativeEdge()
            {
                currentNegativeEdge = 0;
            }

            public void IncrementNegativeCount()
            {
                currentNegativeEdge++;
            }

            public void Next()
            {
                currentNegativeEdge = 0;
                currentMoveStep++;
            }

            public void Reset()
            {
                currentNegativeEdge = 0;
                currentMoveTime = 0f;
                currentMoveStep = 0;
            }

            public bool IsComplete()
            {
                return (currentMoveStep > moves.Count - 1);
            }

            public void Update(GameTime gameTime)
            {
                if (currentMoveStep > 0)
                {
                    if (currentMoveStep > moves.Count - 1)
                    {
                        currentMoveStep = moves.Count - 1;
                    }

                    currentMoveTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (currentMoveTime >= maxMoveTime)
                    {
                        Reset();
                    }
                }
            }
        }

        public static InputHelper.KeyPress GetPressedDirections(GamePadState oldPadState, KeyboardState oldKeyboardState,
                                                                GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress directions = InputHelper.KeyPress.NONE;

            // Get vertical direction.
            if (oldPadState.IsButtonUp(Buttons.DPadUp) && newPadState.IsButtonDown(Buttons.DPadUp) 
                    || oldPadState.IsButtonUp(Buttons.LeftThumbstickUp) && newPadState.IsButtonDown(Buttons.LeftThumbstickUp) 
                    || oldKeyboardState.IsKeyUp(Keys.Up) && newKeyboardState.IsKeyDown(Keys.Up))
            {
                directions |= InputHelper.KeyPress.UP;
            }
            else if (oldPadState.IsButtonUp(Buttons.DPadDown) && newPadState.IsButtonDown(Buttons.DPadDown)
                        || oldPadState.IsButtonUp(Buttons.LeftThumbstickDown) && newPadState.IsButtonDown(Buttons.LeftThumbstickDown)
                        || oldKeyboardState.IsKeyUp(Keys.Down) && newKeyboardState.IsKeyDown(Keys.Down))
            {
                directions |= InputHelper.KeyPress.DOWN;
            }

            // Comebine with horizontal direction.
            if (oldPadState.IsButtonUp(Buttons.DPadLeft) && newPadState.IsButtonDown(Buttons.DPadLeft)
                    || oldPadState.IsButtonUp(Buttons.LeftThumbstickLeft) && newPadState.IsButtonDown(Buttons.LeftThumbstickLeft)
                    || oldKeyboardState.IsKeyUp(Keys.Left) && newKeyboardState.IsKeyDown(Keys.Left))
            {
                directions |= InputHelper.KeyPress.LEFT;
            }
            else if (oldPadState.IsButtonUp(Buttons.DPadRight) && newPadState.IsButtonDown(Buttons.DPadRight)
                        || oldPadState.IsButtonUp(Buttons.LeftThumbstickRight) && newPadState.IsButtonDown(Buttons.LeftThumbstickRight)
                        || oldKeyboardState.IsKeyUp(Keys.Right) && newKeyboardState.IsKeyDown(Keys.Right))
            {
                directions |= InputHelper.KeyPress.RIGHT;
            }

            return directions;
        }

        public static InputHelper.KeyPress GetReleasedDirections(GamePadState oldPadState, KeyboardState oldKeyboardState,
                                                                 GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress directions = InputHelper.KeyPress.NONE;

            // Get vertical direction.
            if (oldPadState.IsButtonDown(Buttons.DPadUp) && newPadState.IsButtonUp(Buttons.DPadUp)
                    || oldPadState.IsButtonDown(Buttons.LeftThumbstickUp) && newPadState.IsButtonUp(Buttons.LeftThumbstickUp)
                    || oldKeyboardState.IsKeyDown(Keys.Up) && newKeyboardState.IsKeyUp(Keys.Up))
            {
                directions |= InputHelper.KeyPress.UP;
            }
            else if (oldPadState.IsButtonDown(Buttons.DPadDown) && newPadState.IsButtonUp(Buttons.DPadDown)
                        || oldPadState.IsButtonDown(Buttons.LeftThumbstickDown) && newPadState.IsButtonUp(Buttons.LeftThumbstickDown)
                        || oldKeyboardState.IsKeyDown(Keys.Down) && newKeyboardState.IsKeyUp(Keys.Down))
            {
                directions |= InputHelper.KeyPress.DOWN;
            }

            // Comebine with horizontal direction.
            if (oldPadState.IsButtonDown(Buttons.DPadLeft) && newPadState.IsButtonUp(Buttons.DPadLeft)
                    || oldPadState.IsButtonDown(Buttons.LeftThumbstickLeft) && newPadState.IsButtonUp(Buttons.LeftThumbstickLeft)
                    || oldKeyboardState.IsKeyDown(Keys.Left) && newKeyboardState.IsKeyUp(Keys.Left))
            {
                directions |= InputHelper.KeyPress.LEFT;
            }
            else if (oldPadState.IsButtonDown(Buttons.DPadRight) && newPadState.IsButtonUp(Buttons.DPadRight)
                        || oldPadState.IsButtonDown(Buttons.LeftThumbstickRight) && newPadState.IsButtonUp(Buttons.LeftThumbstickRight)
                        || oldKeyboardState.IsKeyDown(Keys.Right) && newKeyboardState.IsKeyUp(Keys.Right))
            {
                directions |= InputHelper.KeyPress.RIGHT;
            }

            return directions;
        }

        public static InputHelper.KeyPress GetHeldDirections(GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress directions = InputHelper.KeyPress.NONE;

            // Get vertical direction.
            if (newPadState.IsButtonDown(Buttons.DPadUp)
                    || newPadState.IsButtonDown(Buttons.LeftThumbstickUp)
                    || newKeyboardState.IsKeyDown(Keys.Up))
            {
                directions |= InputHelper.KeyPress.UP;
            }
            else if (newPadState.IsButtonDown(Buttons.DPadDown)
                        || newPadState.IsButtonDown(Buttons.LeftThumbstickDown)
                        || newKeyboardState.IsKeyDown(Keys.Down))
            {
                directions |= InputHelper.KeyPress.DOWN;
            }

            // Comebine with horizontal direction.
            if (newPadState.IsButtonDown(Buttons.DPadLeft)
                    || newPadState.IsButtonDown(Buttons.LeftThumbstickLeft)
                    || newKeyboardState.IsKeyDown(Keys.Left))
            {
                directions |= InputHelper.KeyPress.LEFT;
            }
            else if (newPadState.IsButtonDown(Buttons.DPadRight)
                        || newPadState.IsButtonDown(Buttons.LeftThumbstickRight)
                        || newKeyboardState.IsKeyDown(Keys.Right))
            {
                directions |= InputHelper.KeyPress.RIGHT;
            }

            return directions;
        }

        public static InputHelper.KeyPress GetPressedButtons(GamePadState oldPadState, KeyboardState oldKeyboardState, 
                                                             GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress buttons = InputHelper.KeyPress.NONE;

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

        public static InputHelper.KeyPress GetReleasedButtons(GamePadState oldPadState, KeyboardState oldKeyboardState,
                                                              GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress buttons = InputHelper.KeyPress.NONE;

            foreach (var buttonAndKey in NonDirectionButtons)
            {
                Buttons button = buttonAndKey.Key;
                Keys key = buttonAndKey.Value;

                // Check the game pad and keyboard for presses.
                if (oldPadState.IsButtonDown(button) && newPadState.IsButtonUp(button)
                        || oldKeyboardState.IsKeyDown(key) && newKeyboardState.IsKeyUp(key))
                {
                    // Use a bitwise-or to accumulate button presses.
                    buttons |= NonDirectionButtonKeyMap[button] | NonDirectionButtonKeysMap[key];
                }
            }

            return buttons;
        }

        public static InputHelper.KeyPress GetHeldButtons(GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress buttons = InputHelper.KeyPress.NONE;

            foreach (var buttonAndKey in NonDirectionButtons)
            {
                Buttons button = buttonAndKey.Key;
                Keys key = buttonAndKey.Value;

                // Check the game pad and keyboard for presses.
                if (newPadState.IsButtonDown(button) || newKeyboardState.IsKeyDown(key))
                {
                    // Use a bitwise-or to accumulate button presses.
                    buttons |= NonDirectionButtonKeyMap[button] | NonDirectionButtonKeysMap[key];
                }
            }

            return buttons;
        }
    }
}
