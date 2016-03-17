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
        private List<InputHelper.KeyPress> inputBuffer;
        public readonly float bufferTimeout = 26600f;
        public readonly float mergeInputTime = 120f;
        private float lastInputTime = 0f;
        private float timeSinceLast = 0f;
        public readonly int MAX_BUFFER = 120;
        private InputHelper.ButtonState stateType;

        public InputBuffer(InputHelper.ButtonState stateType)
        {
            inputBuffer = new List<InputHelper.KeyPress>(MAX_BUFFER);
            this.stateType = stateType;
        }

        public void ReadInputBuffer(GameTime gameTime, InputHelper.KeyPress currentButtonState, InputHelper.KeyPress currentDirectionState)
        {
            InputHelper.KeyPress pressedButton = InputHelper.KeyPress.NONE;

            int bufferStep = inputBuffer.Count - 1;
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
                    inputBuffer[bufferStep] = inputBuffer[bufferStep] | pressedButton;
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

        public List<InputHelper.KeyPress> GetBuffer()
        {
            return inputBuffer;
        }

        public float GetLastInputTime()
        {
            return lastInputTime;
        }

        public InputHelper.ButtonState GetStateType()
        {
            return stateType;
        }
    }
}
