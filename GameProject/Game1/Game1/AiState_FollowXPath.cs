using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class AiState_FollowXPath : System.IState
    {
        private System.StateMachine stateMachine;
        private Entity entity;
        private float closeDistance;
        private float idleTime = 0;
        private float maxIdleTime = 2; 
        private Vector2 velocity;

        public AiState_FollowXPath(Entity entity)
        {
            this.entity = entity;
            stateMachine = this.entity.GetAiStateMachine();

            closeDistance = 540f;
            velocity = new Vector2(1.8f, 0f);
        }

        public void OnEnter()
        {

        }

        public void Update(GameTime gameTime)
        {
            Entity target = entity.GetCurrentTarget();
            float distance = Vector2.Distance(entity.GetConvertedPosition(), target.GetConvertedPosition());

            if (target != null)
            {
                if (!entity.IsInAnimationState(Animation.State.STANCE))
                {
                    if (entity.GetPosX() - (entity.GetCurrentSpriteWidth()) - 200 > target.GetPosX())
                    {
                        velocity.X = -1.0f;
                    }
                    else if (entity.GetPosX() + (entity.GetCurrentSpriteWidth() + 200) < target.GetPosX())
                    {
                        velocity.X = 2.0f;
                    }
                }

                if (entity.IsInAnimationState(Animation.State.STANCE))
                {
                    velocity.X = 0f;
                    idleTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (idleTime > maxIdleTime)
                    {
                        entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                        idleTime = 0f;
                    }

                    if ((entity.IsLeft() && velocity.X > 0 || !entity.IsLeft() && velocity.X < 0) && distance > 300)
                    {
                        entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                    }
                }

                if (!entity.IsInAnimationState(Animation.State.STANCE))
                {
                    if ((entity.IsLeft() && velocity.X > 0 || !entity.IsLeft() && velocity.X < 0) && distance > 300)
                    {
                        entity.SetAnimationState(Animation.State.STANCE);
                    }
                    else
                    {
                        entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                    }
                }
                
                entity.VelX(velocity.X);
                entity.VelZ(velocity.Y);
            }
        }

        public void OnExit()
        {

        }
    }
}
