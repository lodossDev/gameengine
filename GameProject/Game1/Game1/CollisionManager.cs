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
            Rectangle entityBox = entity.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();
            float ePosY = Math.Abs(entity.GetPosY());

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();
                    Rectangle heightBox = target.GetBoxes(CLNS.BoxType.HEIGHT_BOX)[0].GetRect();
                    float tPosY = Math.Abs(target.GetPosY());

                    if (entity.InBoundsZ(target, target.GetDepth())
                            && entityBox.Intersects(targetBox)
                            && Math.Abs(entity.GetPosY()) + 20 >= Math.Abs(target.GetPosY()) + target.GetHeight())
                    {
                        found.Add(target);
                    }
                }
            }

            return found;
        }

        public List<Entity> TouchTop(Entity entity)
        {
            List<Entity> found = new List<Entity>();
            Rectangle entityBox = entity.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();

            foreach (Entity target in entities)
            {
                if (entity != target)
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();

                    if (entity.InBoundsZ(target, target.GetDepth())
                           && entityBox.TouchTop(targetBox))
                    {
                        found.Add(target);
                    }
                }
            }

            return found;
        }

        public List<Entity> TouchBottom(Entity entity)
        {
            List<Entity> found = new List<Entity>();
            Rectangle entityBox = entity.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();

            foreach (Entity target in entities)
            {
                if (entity != target)
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();

                    if (entity.InBoundsZ(target, target.GetDepth())
                           && entityBox.TouchBottom(targetBox))
                    {
                        found.Add(target);
                    }
                }
            }

            return found;
        }

        public List<Entity> TouchRight(Entity entity)
        {
            List<Entity> found = new List<Entity>();
            Rectangle entityBox = entity.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();
            
            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();
                    
                    if (entity.InBoundsZ(target, target.GetDepth())
                           && entityBox.TouchRight(targetBox))
                    {
                        found.Add(target);
                    }
                }
            }

            return found;
        }

        public List<Entity> TouchLeft(Entity entity)
        {
            List<Entity> found = new List<Entity>();
            Rectangle entityBox = entity.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();

                    if (entity.InBoundsZ(target, target.GetDepth())
                           && entityBox.TouchLeft(targetBox))
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
            Rectangle entityBox = entity.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();

                    if (entity.InRangeZ(target, target.GetDepth())
                            && entityBox.InBoundsX(targetBox, 20)
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
                entity.SetGround(entity.GetGroundBase());

                if (!entity.IsToss())
                {
                    entity.SetAnimationState(Animation.State.FALL);
                    entity.Toss(5);
                }
            }

            /*if (belowEntities.Count == 0 && entity.InAir()
                    && entity.GetGround() != entity.GetGroundBase())
            {
                //entity.SetGround(entity.GetGroundBase());
            }*/
        }

        private void CheckLand(Entity entity)
        {
            Rectangle entityBox = entity.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();
            
            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();

                    if (entity.InBoundsZ(target, target.GetDepth())
                           && entityBox.Intersects(targetBox)
                           && entityBox.TouchBottom(targetBox)
                           && entity.InAir())
                    {
                        //Debug.WriteLine("E: " + entity.GetName() + " TOP: " + entityBox.Top);
                        //Debug.WriteLine("E: " + target.GetName() + " bottom: " + targetBox.Bottom);

                        //entity.SetGroundBase(target.GetPosY() + entityBox.Height + 5);
                        //entity.SetPosY(-(entityBox.Top - targetBox.Height) + h2);
                        //entity.SetPosY(entity.GetPosY() + x1.Y + 5);
                        //entity.VelY(5);
                        entity.GetTossInfo().velocity.Y = 5;
                    }
                }
            }

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();
                    
                    if (entity.InBoundsZ(target, target.GetDepth())
                            && entityBox.TouchTop(targetBox))
                    {
                        float posy = (Math.Abs(target.GetPosY()) + target.GetHeight() + 1);

                        if (entity.GetVelocity().Y > 2)
                        {
                            entity.SetGround(-posy);
                        }
                    }
                }
            }
        }

        private void CheckBounds(Entity entity)
        {
            List<CLNS.BoundingBox> bboxes = entity.GetBoxes(CLNS.BoxType.BOUNDS_BOX);
            bboxes.AddRange(entity.GetCurrentBoxes(CLNS.BoxType.BOUNDS_BOX));
            CLNS.BoundingBox entityBox = null;
            bool hasCollided = false;
            int ePosY = (int)Math.Abs(entity.GetPosY());

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    List<CLNS.BoundingBox> tboxes = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX);
                    tboxes.AddRange(target.GetCurrentBoxes(CLNS.BoxType.BOUNDS_BOX));
                    CLNS.BoundingBox targetBox = null;
                    int tPosY = (int)Math.Abs(target.GetPosY());
                   
                    if (entity.InRangeZ(target, target.GetDepth())
                            && ePosY + 10 < (tPosY + target.GetHeight() + 1)
                            && (int)Math.Abs(target.GetGround()) != (ePosY + entity.GetHeight() + 1)
                            && (int)Math.Abs(entity.GetGround()) < (tPosY + target.GetHeight() + 1))
                    {
                        int yOffset = (int)(Math.Abs((ePosY + entity.GetHeight() + 1) - Math.Abs(target.GetGround())));
                        Debug.WriteLine("yOffset: " + entity.GetName() + " : " + yOffset + " : " + entity.GetGround() + " : " + (entity.GetGround() + yOffset));

                        foreach (CLNS.BoundingBox bb1 in bboxes)
                        {
                            foreach (CLNS.BoundingBox bb2 in tboxes)
                            {
                                if (bb1.GetRect().Intersects(bb2.GetRect()))
                                { 
                                    Debug.WriteLine("yOffset: " + entity.GetName() + " : " + yOffset);
                                    Debug.WriteLine("FF: " + entity.GetName() + " : " + " : ePosy " + (ePosY + entity.GetHeight()) + " ---- " + target.GetName() + " : height: " + (tPosY + target.GetHeight()) + " : " + target.GetGround());
                                    entityBox = bb1;
                                    targetBox = bb2;
                                    hasCollided = true;
                                }
                            }
                        }

                        if (hasCollided && entityBox != null && targetBox != null)
                        {
                            //Problem with tposy not updating in time for comparison
                            if (entityBox.GetRect().Intersects(targetBox.GetRect()))
                            {
                                if (entity.InBoundsZ(target, target.GetDepth()))
                                {
                                    float depth = entityBox.GetRect().GetHorizontalIntersectionDepth(targetBox.GetRect());
                                    
                                    if (depth != 0)
                                    {
                                        if (entity.GetDirX() > 0 && entityBox.GetRect().TouchLeft(targetBox.GetRect()))
                                        {
                                            Debug.WriteLine("RIGHT: " + entity.GetName() + " : " + " : ePosy " + (ePosY + entity.GetHeight()) + " ---- " +  target.GetName() + " : height: "  + (tPosY + target.GetHeight()) + " : " + target.GetGround());
                                            entity.VelX(0f);
                                            entity.MoveX(depth + 2);
                                            entity.GetCollisionInfo().Right();
                                        }
                                        else if (entity.GetDirX() < 0 && entityBox.GetRect().TouchRight(targetBox.GetRect()))
                                        {
                                            Debug.WriteLine("LEFT: " + entity.GetName() + " : " + " : ePosy " + (ePosY + entity.GetHeight()) + " ---- " + target.GetName() + " : height: " + (tPosY + target.GetHeight()) + " : " + target.GetGround());
                                            entity.VelX(0f);
                                            entity.MoveX(depth - 2);
                                            entity.GetCollisionInfo().Left();
                                        }
                                    }
                                }

                                if (entity.GetDirZ() > 0 && entity.GetPosZ() <= target.GetPosZ() - target.GetDepth())
                                {
                                    entity.VelZ(0f);
                                    entity.GetCollisionInfo().Top();
                                }
                                else if (entity.GetDirZ() < 0 && entity.GetPosZ() >= target.GetPosZ() + target.GetDepth())
                                {
                                    entity.VelZ(0f);
                                    entity.GetCollisionInfo().Bottom();
                                }
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
                target.Toss(-10 * attackBox.GetHitStrength());
                //target.MoveY(-15 * attackBox.GetHitStrength());
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
                        Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();
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
                entity.GetCollisionInfo().Reset();

                CheckAttack(entity);
                
                CheckBounds(entity);

                
                CheckLand(entity);
                CheckFall(entity);
            }
        }
    }
}
