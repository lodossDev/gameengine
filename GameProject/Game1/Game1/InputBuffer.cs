using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Game1
{
    public class InputBuffer
    {
        public List<InputHelper.KeyPress> inputBuffer;
        public readonly float bufferTimeout = 600f;
        public readonly float mergeInputTime = 200f;
        private float lastInputTime = 0f;
        private float timeSinceLast = 0f;
        public readonly int MAX_BUFFER = 60;

        public InputBuffer()
        {
            inputBuffer = new List<InputHelper.KeyPress>(MAX_BUFFER);
        }

        public void ReadInputBuffer(GameTime gameTime, InputHelper.KeyPress currentButtonState, InputHelper.KeyPress currentDirectionState)
        {
            InputHelper.KeyPress pressedButton = InputHelper.KeyPress.NONE;

            float time = (float)gameTime.TotalGameTime.TotalMilliseconds;
            timeSinceLast = time - lastInputTime;

            if (timeSinceLast > bufferTimeout)
            {
                inputBuffer.Clear();
            }

            pressedButton |= currentButtonState;
            pressedButton |= currentDirectionState;

            bool mergeInput = (inputBuffer.Count > 0 && timeSinceLast < mergeInputTime);

            if (pressedButton != InputHelper.KeyPress.NONE)
            {
                if (mergeInput)
                {
                    inputBuffer[inputBuffer.Count - 1] = inputBuffer[inputBuffer.Count - 1] | pressedButton;
                    Debug.WriteLine("CURRENT MERGING BTN: " + inputBuffer[inputBuffer.Count - 1]);
                }
                else
                {
                    if (inputBuffer.Count == inputBuffer.Capacity)
                    {
                        inputBuffer.RemoveAt(0);
                    }

                    inputBuffer.Add(pressedButton);
                    lastInputTime = time;
                }
            }
        }

        public bool Matches(InputHelper.CommandMove command)
        {
            if (timeSinceLast > command.GetMaxTime())
            {
                return false;
            }

            for (int i = 1; i <= command.GetMoves().Count; ++i)
            {
                if (inputBuffer[inputBuffer.Count - i] == command.GetMoves()[command.GetMoves().Count - i].GetKeyPress())
                {
                    command.Increment();
                }
            }

            if (command.IsComplete())
            {
                inputBuffer.Clear();
                command.Reset();
                return true;
            }

            return false;
        }
    }
}
