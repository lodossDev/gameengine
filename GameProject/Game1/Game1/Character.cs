﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Character : Entity
    {
        private Vector2 followPosition;
        private System.StateMachine aiStateMachine;

        public Character(Entity.EntityType entityType, String name) : base(entityType, name)
        {
            followPosition = Vector2.Zero;
            aiStateMachine = GetAiStateMachine();

            aiStateMachine.Add("FOLLOW_PATH", new AiState_Follow(this));
            aiStateMachine.Add("FOLLOW_XPATH", new AiState_FollowXPath(this));
            aiStateMachine.Change("FOLLOW_PATH");
        }

        public virtual Entity GetNearestEntity(List<Entity> entities)
        {
            Entity target = null;
            float maxDistance = 340f;

            if (entities != null && entities.Count > 0)
            {
                if (entities.Count == 1)
                {
                    target = entities.First();
                }
                else
                {
                    foreach (Entity entity in entities)
                    {
                        float distance = Vector2.Distance(GetConvertedPosition(), entity.GetConvertedPosition());

                        if (distance < maxDistance)
                        {
                            target = entity;
                            break;
                        }
                    }
                }
            }

            return target;
        }

        public virtual void LookAtTarget(Entity target)
        {
            if (GetPosX() - (GetCurrentSpriteWidth() / 4) > target.GetPosX())
            {
                SetIsLeft(true);
            }
            else if (GetPosX() + (GetCurrentSpriteWidth() / 4) < target.GetPosX())
            {
                SetIsLeft(false);
            }
        }

        public virtual void FollowTarget(Entity target)
        {
            float maxDistance = 240f;
            float distance = Vector2.Distance(GetConvertedPosition(), target.GetConvertedPosition());

            followPosition.X = target.GetPosX() - GetPosX();
            followPosition.Y = (target.GetPosZ() - GetPosZ()) + 91;
            followPosition.Normalize();

            if (distance < maxDistance)
            {
                followPosition.X = 0;
                followPosition.Y = 0;

                /*if (this.CollideZ(target, 130))
                {
                    SetAnimationState(Animation.State.ATTACK1);
                }
                else
                {
                    SetAnimationState(Animation.State.STANCE);
                }*/
                SetAnimationState(Animation.State.ATTACK1);
            }
            else
            {
                SetAnimationState(Animation.State.WALK_TOWARDS);
            }

            VelX(followPosition.X * 1.6f);
            VelZ(followPosition.Y * 2.1f);
        }

        public virtual void UpdateAI(GameTime gameTime, List<Player> players)
        {
            Entity target = GetNearestEntity(players.ToList<Entity>());
            SetCurrentTarget(target);

            if (target != null && !IsInAnimationAction(Animation.Action.ATTACKING))
            {
                LookAtTarget(target);
                aiStateMachine.Update(gameTime);
                //FollowTarget(target);
            }
        }
    }
}
