using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace Game1
{
    public class CollisionManager : Manager
    {
        public static int hit_id = 0;
        private SoundEffect hiteffect1;
        private SoundEffectInstance soundInstance, soundInstance2;
        private RenderManager renderManager;

        public CollisionManager(RenderManager renderManager)
        {
            hiteffect1 = Setup.contentManager.Load<SoundEffect>("Sounds//hit1");
            soundInstance = hiteffect1.CreateInstance();

            soundInstance2 = Setup.contentManager.Load<SoundEffect>("Sounds//test").CreateInstance();

            this.renderManager = renderManager;
        }

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
                            && Math.Abs(entity.GetPosY()) + 50 >= Math.Abs(target.GetPosY()) + target.GetHeight())
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

            if ((belowEntities.Count == 0 && entity.GetGround() != entity.GetGroundBase()
                    && entity.HasLanded()) || (belowEntities.Count == 0 
                                                    && Math.Abs(entity.GetPosY()) != entity.GetGround()
                                                    && !entity.IsToss()))
            {
                entity.SetGround(entity.GetGroundBase());

                if (!entity.IsToss())
                {
                    entity.SetAnimationState(Animation.State.FALL);
                    entity.Toss(5);
                }
            }
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
                           && !entityBox.TouchTop(targetBox)
                           && entity.InAir())
                    {
                        float depth = entityBox.GetVerticalIntersectionDepth(targetBox);

                        if (!target.IsToss())
                        {
                            entity.MoveY(depth + 5);
                        }

                        entity.GetTossInfo().velocity.Y = 5;
                    }
                }
            }

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();
                    float posy = (Math.Abs(target.GetPosY()) + target.GetHeight());

                    if (entity.InBoundsZ(target, target.GetDepth())
                            && entityBox.Intersects(targetBox)
                            && entityBox.InBoundsX(targetBox, 20))
                    {
                        List<Entity> above = FindAbove(target);
                        int totalHeight = (int)Math.Abs(target.GetPosY()) + target.GetHeight() + above.Sum(e => e.GetHeight());
                        totalHeight = (above.Count == 0 ? (int)Math.Abs(target.GetPosY()) + totalHeight : totalHeight);
    
                        if (entity.GetVelocity().Y > 2 && Math.Abs(entity.GetPosY() + 20) > totalHeight)
                        {
                            entity.SetGround(-posy);
                        }

                        if (entity.GetVelocity().Y > 2 && Math.Abs(entity.GetPosY()) + 20 > Math.Abs(target.GetPosY()) + target.GetHeight())
                        {
                            entity.SetGround(-posy);
                        }

                        if (target.IsToss() && Math.Abs(entity.GetPosY()) + 50 > Math.Abs(target.GetPosY()) + target.GetHeight())
                        {
                            entity.SetPosY(-posy);
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
            Rectangle entityBox = Rectangle.Empty;
            bool hasCollided = false;
            int ePosY = (int)Math.Abs(entity.GetPosY());
            int eGround = (int)Math.Abs(entity.GetGround());

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    List<CLNS.BoundingBox> tboxes = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX);
                    tboxes.AddRange(target.GetCurrentBoxes(CLNS.BoxType.BOUNDS_BOX));
                    Rectangle targetBox = Rectangle.Empty;
                    int tPosY = (int)Math.Abs(target.GetPosY());
                    int tGround = (int)Math.Abs(target.GetGround());
                    int offSetY = 10;

                    if (entity.InRangeZ(target, target.GetDepth()))
                    {
                        if (ePosY + 1 > (tPosY + target.GetHeight()))
                        {
                            offSetY = 50;
                        }
                    }

                    if (entity.InRangeZ(target, target.GetDepth())
                            && ePosY + offSetY < (tPosY + target.GetHeight())
                            && !(tPosY + offSetY > (ePosY + entity.GetHeight())))
                    {
                        foreach (CLNS.BoundingBox bb1 in bboxes)
                        {
                            foreach (CLNS.BoundingBox bb2 in tboxes)
                            {
                                if (bb1.GetRect().Intersects(bb2.GetRect()))
                                { 
                                    entityBox = bb1.GetRect();
                                    targetBox = bb2.GetRect();
                                    hasCollided = true;
                                }
                            }
                        }

                        if (hasCollided && !entityBox.IsEmpty && !targetBox.IsEmpty)
                        {
                            //Debug.WriteLine("TT tPosY: " + target.GetName() + ": " + tPosY);
                            //Debug.WriteLine("E: " + entity.GetName() + " : " + (ePosY + entity.GetHeight()));

                            //Problem with tposy not updating in time for comparison
                            if (entityBox.Intersects(targetBox))
                            {
                                if (entity.InBoundsZ(target, target.GetDepth()))
                                {
                                    float depth = entityBox.GetHorizontalIntersectionDepth(targetBox);
                                    
                                    if (depth != 0)
                                    {
                                        if (entity.GetDirX() > 0 && entityBox.TouchLeft(targetBox))
                                        {
                                            entity.VelX(0f);
                                            entity.MoveX(depth + 5);
                                            entity.GetCollisionInfo().Right();
                                        }
                                        else if (entity.GetDirX() < 0 && entityBox.TouchRight(targetBox))
                                        {
                                            entity.VelX(0f);
                                            entity.MoveX(depth - 5);
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

        public static int hitCount = 0;

        private void OnHit(Entity target, Entity entity, CLNS.AttackBox attackBox)
        {
            if (target != entity)
            {
                hitCount++;
                hiteffect1.CreateInstance().Play();

                target.Toss(-15 * attackBox.GetHitStrength());

                Entity hitSpark1 = new Entity(Entity.EntityType.HIT_FLASH, "SPARK1");
                hitSpark1.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Leo/Spark1", Animation.Type.ONCE));
                hitSpark1.SetAnimationState(Animation.State.STANCE);
                hitSpark1.SetFrameDelay(Animation.State.STANCE, 40);
                hitSpark1.SetScale(1.2f, 1.2f);
                hitSpark1.SetPostion(attackBox.GetRect().X, (attackBox.GetRect().Y / 2) - (attackBox.GetOffset().Y / 2), target.GetPosZ()+5);
                hitSpark1.SetFade(225);

                renderManager.AddEntity(hitSpark1);
                //target.MoveY(-125 * attackBox.GetHitStrength());
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
