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
        public readonly float bufferTimeout = 500f;
        public readonly float mergeInputTime = 120f;
        private float lastInputTime = 0f;
        private float timeSinceLast = 0f;
        public readonly int MAX_BUFFER = 120;
        private InputHelper.ButtonState stateType;
        private List<InputHelper.KeyPress> inputState;
        private int currentStateStep;

        public InputBuffer(InputHelper.ButtonState stateType, float bufferTimeout = 500f)
        {
            inputBuffer = new List<InputHelper.KeyPress>(MAX_BUFFER);
            inputState = new List<InputHelper.KeyPress>(MAX_BUFFER);
            currentStateStep = 0;

            this.bufferTimeout = bufferTimeout;
            this.stateType = stateType;

            InitInputState();
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
                    AddInputState(inputBuffer[bufferStep]);
                }
                else
                {
                    if (inputBuffer.Count == inputBuffer.Capacity)
                    {
                        inputBuffer.RemoveAt(0);
                    }

                    inputBuffer.Add(pressedButton);
                    AddInputState(pressedButton);
                    lastInputTime = time;
                }
            }

            currentStateStep++;

            if (currentStateStep >= MAX_BUFFER - 1)
            {
                ClearInputState();
                currentStateStep = 0;
            }
        }

        private void AddInputState(InputHelper.KeyPress keyPress)
        {
            inputState[currentStateStep] = keyPress;
        }

        private void InitInputState()
        {
            for (int i = 0; i < MAX_BUFFER - 1; i++)
            {
                inputState.Add(InputHelper.KeyPress.NONE);
            }
        }

        private void ClearInputState()
        {
            for (int i = 0; i < inputState.Count - 1; i++)
            {
                inputState[i] = InputHelper.KeyPress.NONE;
            }
        }

        public InputHelper.KeyPress GetCurrentInputState()
        {
            int step = currentStateStep - 1;
            if (step < 0) step = 0;

            return inputState[step];
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
