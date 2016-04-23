﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class InputManager : Manager
    {
        private Dictionary<Entity, InputControl> controllMapEntity;

        public InputManager()
        {
            controllMapEntity = new Dictionary<Entity, InputControl>();
        }

        public void AddControl(Entity entity, PlayerIndex playerIndex)
        {
            InputControl inputControl = new InputControl(entity, playerIndex);
            controllMapEntity.Add(entity, inputControl);

            this.AddEntity(entity);
        }

        public void Update(GameTime gameTime)
        {
            foreach(Entity entity in entities)
            {
                List<InputHelper.CommandMove> commandMoves = entity.GetCommandMoves();
                commandMoves.Sort();

                InputControl inputControl = controllMapEntity[entity];
                inputControl.Update(gameTime);

                foreach (InputHelper.CommandMove command in commandMoves)
                {
                    if (inputControl.Matches(command))
                    {
                        entity.SetAnimationState(command.GetAnimationState());
                        break;
                    }
                }
            }
        }
    }
}
