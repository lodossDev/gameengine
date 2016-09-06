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
        private Vector2 velocity;

        public AiState_FollowXPath(Entity entity)
        {
            this.entity = entity;
            stateMachine = this.entity.GetAiStateMachine();

            closeDistance = 140f;
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
                if (distance < closeDistance)
                {
                    //velocity.X = 0f;
                    velocity.X = 3.6f;
                    entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                }
                else
                {
                    /*if (entity.IsLeft() != target.IsLeft())
                    {
                        velocity.X = 1.6f;
                        entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                    }
                    else
                    {
                        velocity.X = -1.6f;
                        entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                    }*/
                    velocity.X = 0f;
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
