using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Character : Entity
    {
        private Vector2 followPosition;

        public Character(Entity.EntityType entityType, String name) : base(entityType, name)
        {
            followPosition = Vector2.Zero;
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
            followPosition.X = target.GetPosX() - GetPosX();
            followPosition.Y = (target.GetPosZ() - GetPosZ()) + 90;
            followPosition.Normalize();

            VelX(followPosition.X * 1.6f);
            VelZ(followPosition.Y * 1.6f);
        }

        public virtual void UpdateAI(GameTime gameTime, List<Player> players)
        {
            Entity target = GetNearestEntity(players.ToList<Entity>());

            if (target != null)
            {
                LookAtTarget(target);
                FollowTarget(target);
            }
        }
    }
}
