﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class StateSystem
    {
        public interface IState
        {
            void OnEnter();
            void Update(GameTime gameTime);
            void OnExit();
        }

        public class StateMachine
        {
            Dictionary<string, IState> states;
            private IState currentState;

            public StateMachine()
            {
                states = new Dictionary<string, IState>();
            }

            public void Add(string id, IState state)
            {
                states.Add(id, state);
            }

            public void Remove(string id)
            {
                states.Remove(id);
            }

            public void Clear()
            {
                states.Clear();
            }

            public void Change(string id)
            {
                currentState.OnExit();

                IState next = states[id];
                next.OnEnter();

                currentState = next;
            }

            public void Update(GameTime gameTime)
            {
                currentState.Update(gameTime);
            }
        }
    }
}
