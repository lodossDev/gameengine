﻿using Microsoft.Xna.Framework;
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
        public readonly float bufferTimeout = 500f;
        public readonly float mergeInputTime = 120f;
        private float lastInputTime = 0f;
        public float timeSinceLast = 0f;
        public readonly int MAX_BUFFER = 120;

        public InputBuffer()
        {
            inputBuffer = new List<InputHelper.KeyPress>(MAX_BUFFER);
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

        public bool Matches(InputHelper.CommandMove command)
        {
            if (inputBuffer.Count < 0)
            {
                //command.Reset();
                return false;
            }

            InputHelper.KeyPress currentTargetKeyPress = command.GetMoves()[0].GetKeyPress();

            for (int i = 0; i < inputBuffer.Count; i++)
            {
                Debug.WriteLine("INPUT BUFFER A: " + inputBuffer[i]);
                Debug.WriteLine("TARGET PRESS: " + currentTargetKeyPress);

                if (inputBuffer[i] == currentTargetKeyPress)
                {
                    if (i < command.GetMoves().Count)
                    {
                        currentTargetKeyPress = command.GetMoves()[i + 1].GetKeyPress();
                        command.Increment();
                    }
                }
            }

            /*for (int i = 1; i <= command.GetMoves().Count; ++i)
            {
                int commandStep = command.GetMoves().Count - i;
                int bufferStep = inputBuffer.Count - i;

                if (inputBuffer[bufferStep] != command.GetMoves()[commandStep].GetKeyPress())
                {
                    return false;
                } 
            }

            inputBuffer.Clear();
            return true;*/
            return false;
        }
    }
}
