using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Game1
{
    public class CollisionManager : Manager
    {
        public static int hit_id = 0;

        public List<Entity> FindBelow(Entity entity)
        {
            List<Entity> found = new List<Entity>();
            Rectangle entityBox = entity.GetBoxes(CLNS.BoxType.BODY_BOX)[0].GetRect();

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BODY_BOX)[0].GetRect();

                    if (entity.InRangeZ(target, target.GetDepth())
                            && entityBox.TouchTop(targetBox, 40)
                            && entityBox.Intersects(targetBox)
                            && Math.Abs(entity.GetPosY()) - 1 <= (Math.Abs(target.GetPosY()) + Math.Abs(target.GetHeight())))
                    {
                        found.Add(target);
                    }
                }
            }

            return found;
        }

        public List<Entity> FindAbove(Entity entity)
        {
            List<Entity> found = new List<Entity>();
            Rectangle entityBox = entity.GetBoxes(CLNS.BoxType.BODY_BOX)[0].GetRect();

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BODY_BOX)[0].GetRect();

                    if (entity.InRangeZ(target, target.GetDepth())
                            && entityBox.InBounds(targetBox, 20)
                            && entityBox.Intersects(targetBox)
                            && (Math.Abs(target.GetPosY()) + Math.Abs(target.GetHeight()) + 1) > Math.Abs(entity.GetPosY())
                            && !(Math.Abs(entity.GetPosY()) + 20 >= (Math.Abs(target.GetPosY()) + Math.Abs(target.GetHeight())))
                            && Math.Abs(entity.GetPosY()) < Math.Abs(target.GetPosY()))
                    {
                        found.Add(target);
                    }
                }
            }

            return found;
        }

        private void CheckFall(Entity entity)
        {
            List<Entity> belowEntities = FindBelow(entity);

            if (belowEntities.Count == 0 && entity.GetGround() != entity.GetGroundBase()
                    && entity.HasLanded() || belowEntities.Count == 0 
                    && Math.Abs(entity.GetPosY()) != entity.GetGround()
                    && !entity.IsToss())
            {
                entity.SetAnimationState(Animation.State.FALL);
                entity.SetGround(entity.GetGroundBase());
                entity.Toss(1);
            }

            if (belowEntities.Count == 0 && entity.InAir()
                    && entity.GetGround() != entity.GetGroundBase())
            {
                entity.SetGround(entity.GetGroundBase());
            }
        }

        private void CheckLand(Entity entity)
        {
            Rectangle entityBox = entity.GetBoxes(CLNS.BoxType.BODY_BOX)[0].GetRect();
            
            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BODY_BOX)[0].GetRect();
                    Vector2 x1 = entityBox.GetIntersectionDepth(targetBox);

                    if (entity.InRangeZ(target, target.GetDepth())
                           && !(entity.GetDirZ() > 0 && entity.GetPosZ() <= target.GetPosZ() - target.GetDepth())
                                   && !(entity.GetDirZ() < 0 && entity.GetPosZ() >= target.GetPosZ() + target.GetDepth())
                           && entityBox.Intersects(targetBox)
                           && entityBox.TouchBottom(targetBox))
                    {
                        //Debug.WriteLine("E: " + entity.GetName() + " TOP: " + entityBox.Top);
                        //Debug.WriteLine("E: " + target.GetName() + " bottom: " + targetBox.Bottom);

                        //entity.SetGroundBase(target.GetPosY() + entityBox.Height + 5);
                        //entity.SetPosY(-(entityBox.Top - targetBox.Height) + h2);
                        //entity.SetPosY(entity.GetPosY() + x1.Y + 5);
                        //entity.VelY(5);
                        entity.Toss(5);
                    }
                }
            }

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BODY_BOX)[0].GetRect();

                    if (entity.InRangeZ(target, target.GetDepth())
                           && !(entity.GetDirZ() > 0 && entity.GetPosZ() <= target.GetPosZ() - target.GetDepth())
                                    && !(entity.GetDirZ() < 0 && entity.GetPosZ() >= target.GetPosZ() + target.GetDepth())

                           && entityBox.TouchTop(targetBox))
                    {
                        if (entity.GetVelocity().Y > 1)
                        {
                            float posy = (Math.Abs(target.GetPosY()) + target.GetHeight()) + 1;
                            entity.SetGround(-posy);
                        }

                        if (target.IsToss())
                        {
                            //entity.Toss(-5f);
                            //entity.tossVelY = target.tossVelY;
                        }
                    }

                    /*if (EntityHelper.InRangeZ(entity, target, target.GetDepth())
                        && InBoundsX(entityBox, targetBox)
                        && entityBox.Intersects(targetBox)
                        && target.GetHeight() != 0
                        && !(entity.GetDirZ() > 0 && entity.GetPosZ() <= target.GetPosZ() - target.GetDepth())
                        && !(entity.GetDirZ() < 0 && entity.GetPosZ() >= target.GetPosZ() + target.GetDepth()))
                    {
                        //Get all possible obstacles above current target?
                        List<Entity> above = FindAbove(target);
                        //Get total height for entity needed to land.
                        int totalHeight = (int)Math.Abs(target.GetPosY()) + target.GetHeight() + above.Sum(e => e.GetHeight());
                        //totalHeight = (above.Count == 0 ? (int)Math.Abs(target.GetPosY()) + totalHeight : totalHeight);
                        Debug.WriteLine("TOTAL HEIGHT: " + target.GetName() + " : " + totalHeight);

                        if (Math.Abs(entity.GetPosY() + 20) > totalHeight && (entity.GetVelocity().Y > 1 || target.InAir()))
                        {
                            float posy = (Math.Abs(target.GetPosY()) + target.GetHeight()) + 1;
                            entity.SetGround(-posy);
                        }

                        if (Math.Abs(entity.GetPosY()) + 20 > Math.Abs(target.GetPosY()) + target.GetHeight()
                                 && (entity.GetVelocity().Y > 1 || target.InAir()))
                        {
                            float posy = (Math.Abs(target.GetPosY()) + target.GetHeight()) + 1;
                            entity.SetGround(-posy);
                        }
                    }*/
                }
            }
        }

        private void CheckBounds(Entity entity)
        {
            CLNS.BoundingBox bb1 = entity.GetBoxes(CLNS.BoxType.BODY_BOX)[0];
            Rectangle entityBox = bb1.GetRect();
            Sprite pStance = entity.GetSprite(Animation.State.STANCE);
            int pWidth = entityBox.Width;
            entity.GetCollisionInfo().Reset();

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    CLNS.BoundingBox bb2 = target.GetBoxes(CLNS.BoxType.BODY_BOX)[0];
                    Rectangle targetBox = bb2.GetRect();
                    Sprite tStance = target.GetSprite(Animation.State.STANCE);
                    int tWidth = targetBox.Width;
                    int tHeight = targetBox.Height;

                    if (entity.InRangeZ(target, target.GetDepth())
                            && entityBox.Intersects(targetBox))
                    {
                        Vector2 x1 = entityBox.GetIntersectionDepth(targetBox);

                        if ((Math.Abs(entity.GetPosY()) + 20 < (Math.Abs(target.GetPosY()) + target.GetHeight()))
                                && !entityBox.TouchBottom(targetBox))
                        {
                            if (!(entity.GetDirZ() > 0 && entity.GetPosZ() <= target.GetPosZ() - target.GetDepth())
                                    && !(entity.GetDirZ() < 0 && entity.GetPosZ() >= target.GetPosZ() + target.GetDepth()))
                            {
                                if ((entity.GetDirX() > 0) && entityBox.TouchLeft(targetBox))//left
                                {
                                    entity.VelX(0f);
                                    entity.GetCollisionInfo().Left();
                                    entity.SetPosX(entity.GetPosX() + x1.X + 2);
                                    
                                }
                                else if ((entity.GetDirX() < 0) && entityBox.TouchRight(targetBox))//right
                                {
                                    entity.VelX(0f);
                                    entity.GetCollisionInfo().Right();
                                    entity.SetPosX(entity.GetPosX() + x1.X - 2);
                                }
                            }

                            if (entity.GetDirZ() > 0 && entity.GetPosZ() <= target.GetPosZ() - target.GetDepth())
                            {
                                entity.VelZ(0f);
                                entity.GetCollisionInfo().Top();
                                entity.SetPosZ(target.GetPosZ() - target.GetDepth());
                            }
                            else if (entity.GetDirZ() < 0 && entity.GetPosZ() >= target.GetPosZ() + target.GetDepth())
                            {
                                entity.VelZ(0f);
                                entity.GetCollisionInfo().Bottom();
                                entity.SetPosZ(target.GetPosZ() + target.GetDepth());
                            }
                        }
                    }
                }
            }
        }
        
        private void OnAttack(Entity entity, Entity target, CLNS.AttackBox attackBox)
        {
            if (entity != target)
            {
                ComboAttack.Chain attackChain = entity.GetDefaultAttackChain();
                attackChain.IncrementMoveIndex(attackBox.GetComboStep());
            }
        }

        private void OnHit(Entity target, Entity entity, CLNS.AttackBox attackBox)
        {
            if (target != entity)
            {
                //target.Toss(-5 * attackBox.GetHitStrength());
                target.MoveY(-15 * attackBox.GetHitStrength());
            }
        }

        private void CheckAttack(Entity entity)
        {
            Attributes.AttackInfo entityAttackInfo = entity.GetAttackInfo();
            List<CLNS.AttackBox> attackBoxes = entity.GetCurrentBoxes(CLNS.BoxType.HIT_BOX).Cast<CLNS.AttackBox>().ToList();
            CLNS.AttackBox currentAttackBox = null;

            if (attackBoxes != null && attackBoxes.Count > 0)
            {
                foreach (Entity target in entities)
                {
                    if (entity != target)
                    {
                        Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BODY_BOX)[0].GetRect();
                        Attributes.AttackInfo targetAttackInfo = target.GetAttackInfo();
                        bool targetHit = false;

                        if (entity.InRangeZ(target, target.GetDepth())
                                && entity.IsInAnimationAction(Animation.Action.ATTACKING)
                                && entity.InAttackFrame())
                        {
                            //Get all attackboxes for this one frame, you can only hit once in each attack frame.
                            foreach (CLNS.AttackBox attack in attackBoxes)
                            {
                                if (attack.GetRect().Intersects(targetBox))
                                {
                                    currentAttackBox = attack;
                                    targetHit = true;
                                }
                            }

                            if (targetHit)
                            {
                                //This will hit the target in a different attack frame.
                                if (currentAttackBox.GetResetHit() == 1)
                                {
                                    if (entityAttackInfo.lastAttackFrame != entity.GetCurrentSprite().GetCurrentFrame())
                                    {
                                        hit_id++;
                                        OnAttack(entity, target, currentAttackBox);
                                        entityAttackInfo.lastAttackFrame = entity.GetCurrentSprite().GetCurrentFrame();
                                    }
                                }
                                else
                                {
                                    if (entityAttackInfo.lastAttackState != entity.GetCurrentAnimationState())
                                    {
                                        hit_id++;
                                        OnAttack(entity, target, currentAttackBox);
                                        entityAttackInfo.lastAttackFrame = entity.GetCurrentSprite().GetCurrentFrame();
                                        entityAttackInfo.lastAttackState = entity.GetCurrentAnimationState();
                                    }
                                }

                                //Only 1 attack box will hit target.
                                if (targetAttackInfo.hitByAttackId != hit_id)
                                {
                                    OnHit(target, entity, currentAttackBox);
                                    targetAttackInfo.hitByAttackId = hit_id;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Entity entity in entities)
            {
                CheckAttack(entity);
                CheckFall(entity);
                CheckLand(entity);
                CheckBounds(entity);
            }
        }
    }
}
