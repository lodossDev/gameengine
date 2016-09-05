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
        public static long current_hit_id = 0;
        public static long static_hit_id = 0;
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
                    Rectangle heightBox = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX)[0].GetRect();
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
                    entity.SetAnimationState(Animation.State.FALL1);
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
            List<CLNS.BoundsBox> bboxes = entity.GetBoxes(CLNS.BoxType.BOUNDS_BOX).Cast<CLNS.BoundsBox>().ToList();
            bboxes.AddRange(entity.GetCurrentBoxes(CLNS.BoxType.BOUNDS_BOX).Cast<CLNS.BoundsBox>().ToList());
            CLNS.BoundsBox entityBox = null;
           
            int ePosY = (int)Math.Abs(entity.GetPosY());
            int eGround = (int)Math.Abs(entity.GetGround());
            bool onTop = false;
            
            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    List<CLNS.BoundsBox> tboxes = target.GetBoxes(CLNS.BoxType.BOUNDS_BOX).Cast<CLNS.BoundsBox>().ToList();
                    tboxes.AddRange(target.GetCurrentBoxes(CLNS.BoxType.BOUNDS_BOX).Cast<CLNS.BoundsBox>().ToList());
                    CLNS.BoundsBox targetBox = null;
                    bool hasCollided = false;

                    int tPosY = (int)Math.Abs(target.GetPosY());
                    int tGround = (int)Math.Abs(target.GetGround());
                    
                    foreach (CLNS.BoundsBox bb1 in bboxes)
                    {
                        foreach (CLNS.BoundsBox bb2 in tboxes)
                        {
                            if (entity.InBoundsZ(target, bb2.GetZdepth()) 
                                    && bb1.GetRect().Intersects(bb2.GetRect()))
                            {
                                entityBox = bb1;
                                targetBox = bb2;
                                hasCollided = true;
                                break;
                            }
                        }
                    }

                    if (hasCollided && entityBox != null && targetBox != null)
                    {
                        //Debug.WriteLine("TT tPosY: " + target.GetName() + ": " + tPosY);
                        //Debug.WriteLine("E: " + entity.GetName() + " : " + (ePosY + entity.GetHeight()));

                        //Problem with tposy not updating in time for comparison
                        if (entityBox.GetRect().Intersects(targetBox.GetRect()))
                        {
                            int eHeight = (int)(entityBox.GetHeight() + eGround);
                            int tHeight = (int)(targetBox.GetHeight() + tGround);
                            Debug.WriteLine("tHeight: " + tHeight);

                            if (entityBox.GetRect().InBoundsX(targetBox.GetRect(), 60)
                                   && entity.InBoundsZ(target, targetBox.GetZdepth() - 5)
                                   && entityBox.GetRect().TouchTop(targetBox.GetRect(), 10))
                            {
                                onTop = true;
                            }

                            if (entityBox.GetRect().InBoundsX(targetBox.GetRect(), 60) 
                                    && entity.InBoundsZ(target, targetBox.GetZdepth() - 5) 
                                    && entityBox.GetRect().TouchTop(targetBox.GetRect(), 10)
                                    && entity.GetVelocity().Y > 1)
                            {
                                entity.SetGround(-(tHeight + 1));
                            }

                            bool over = false;

                            if (targetBox.GetRect().TouchTop(entityBox.GetRect(), 10)
                                    && tGround >= eGround)
                            {
                                over = true;
                            }

                            if (ePosY < tHeight - 10)
                            {
                                if (entity.InBoundsZ(target, targetBox.GetZdepth() - 5) && over == false)
                                {
                                    float depth = entityBox.GetRect().GetHorizontalIntersectionDepth(targetBox.GetRect());

                                    if (depth != 0)
                                    {
                                        if (!entity.IsLeft() && entityBox.GetRect().TouchLeft(targetBox.GetRect()))
                                        {
                                            entity.MoveX(depth + 5);
                                            entity.GetCollisionInfo().Right();
                                            entity.VelX(0f);
                                        }
                                        else if (entity.IsLeft() && entityBox.GetRect().TouchRight(targetBox.GetRect()))
                                        {
                                            entity.MoveX(depth - 5);
                                            entity.GetCollisionInfo().Left();
                                            entity.VelX(0f);
                                        }
                                    }
                                }

                                if (entity.GetDirZ() > 0 && entity.GetPosZ() <= target.GetPosZ())
                                {
                                    entity.VelZ(0f);
                                    entity.GetCollisionInfo().Top();
                                }
                                else if (entity.GetDirZ() < 0 && entity.GetPosZ() >= target.GetPosZ())
                                {
                                    entity.VelZ(0f);
                                    entity.GetCollisionInfo().Bottom();
                                }
                            }
                        }
                    }
                }
            }

            if ((onTop == false && entity.GetGround() != entity.GetGroundBase() && entity.HasLanded()) 
                    || (onTop == false && Math.Abs(entity.GetPosY()) != entity.GetGround()
                            && !entity.IsToss()))
            {
                if (!entity.IsToss())
                {
                    entity.SetGround(entity.GetGroundBase());
                    entity.SetAnimationState(Animation.State.FALL1);
                    entity.Toss(5);
                }
            }
        }
        
        private void OnAttack(Entity entity, Entity target, CLNS.AttackBox attackBox)
        {
            if (entity != target)
            {
                ComboAttack.Chain attackChain = entity.GetDefaultAttackChain();
                attackChain.IncrementMoveIndex(attackBox.GetComboStep());
                entity.GetAttackInfo().hitPauseTime = 3000f;
            }
        }

        public static int hitCount = 0;

        private void OnHit(Entity target, Entity entity, CLNS.AttackBox attackBox)
        {
            if (target != entity)
            {
                hitCount++;
                hiteffect1.CreateInstance().Play();

                target.Toss(-25 * attackBox.GetHitStrength());
                //target.MoveY(-125 * attackBox.GetHitStrength());
            }
        }

        private float TargetBodyX(Entity target, Entity entity, CLNS.AttackBox attack)
        {
            float x1 = (target.GetPosX() + (target.GetWidth() / 4));

            if (entity.GetPosX() >= target.GetPosX() + (target.GetWidth() / 4))
            {
                x1 = (target.GetPosX() + (target.GetWidth() / 4));
            }
            else if (entity.GetPosX() <= target.GetPosX() + (target.GetWidth() / 4))
            {
                x1 = (target.GetPosX() - (target.GetWidth() / 4));
            }

            return x1 + (attack.GetOffset().X / 2);
        }

        private float TargetBodyY(Entity target, Entity entity, CLNS.AttackBox attack)
        {
            float y1 = entity.GetPosY();
            return (y1) - (attack.GetRect().Height / 1.5f) + (attack.GetOffset().Y);
        }

        private void CheckAttack(Entity entity)
        {
            Attributes.AttackInfo entityAttackInfo = entity.GetAttackInfo();
            List<CLNS.AttackBox> attackBoxes = entity.GetCurrentBoxes(CLNS.BoxType.HIT_BOX).Cast<CLNS.AttackBox>().ToList();

            if (attackBoxes != null && attackBoxes.Count > 0)
            {
                foreach (Entity target in entities)
                {
                    if (entity != target)
                    {
                        //Get all body boxes for collision with attack boxes
                        List<CLNS.BoundingBox> targetBoxes = entity.GetCurrentBoxes(CLNS.BoxType.BODY_BOX);
                        targetBoxes.AddRange(target.GetBoxes(CLNS.BoxType.BODY_BOX));
                        List<CLNS.AttackBox> attackBoxesHitInFrame = new List<CLNS.AttackBox>();
                        Attributes.AttackInfo targetAttackInfo = target.GetAttackInfo();
                        bool targetHit = false;

                        if (entity.IsInAnimationAction(Animation.Action.ATTACKING)
                                && entity.InAttackFrame())
                        {
                            //Get all attackboxes for this one frame, you can only hit once in each attack frame.
                            foreach (CLNS.AttackBox attackBox in attackBoxes)
                            {
                                foreach (CLNS.BoundingBox bodyBox in targetBoxes)
                                {
                                    if (target.InRangeZ(entity, attackBox.GetZdepth())
                                            && attackBox.GetRect().Intersects(bodyBox.GetRect()))
                                    {
                                        attackBoxesHitInFrame.Add(attackBox);
                                        targetHit = true;

                                        //This will hit the target in a different attack frame.
                                        if (attackBox.GetResetHit() == 1)
                                        {
                                            if (entityAttackInfo.lastAttackFrame != entity.GetCurrentSprite().GetCurrentFrame())
                                            {
                                                current_hit_id++;
                                                OnAttack(entity, target, attackBox);
                                                entityAttackInfo.lastAttackFrame = entity.GetCurrentSprite().GetCurrentFrame();
                                            }

                                            if (entityAttackInfo.lastAttackState != entity.GetCurrentAnimationState())
                                            {
                                                static_hit_id++;
                                                entityAttackInfo.lastAttackState = entity.GetCurrentAnimationState();
                                            }
                                        }
                                        else
                                        {
                                            if (entityAttackInfo.lastAttackState != entity.GetCurrentAnimationState())
                                            {
                                                current_hit_id++;
                                                static_hit_id++;
                                                OnAttack(entity, target, attackBox);
                                                entityAttackInfo.lastAttackState = entity.GetCurrentAnimationState();
                                            }
                                        }

                                        //Only 1 attack box will hit target.
                                        if (targetAttackInfo.hitByAttackId != current_hit_id)
                                        {
                                            OnHit(target, entity, attackBox);
                                            targetAttackInfo.hitByAttackFrameCount = 0;
                                            targetAttackInfo.hitByAttackId = current_hit_id;
                                        }
                                    }
                                }
                            }
                            
                            if (targetHit && attackBoxesHitInFrame.Count > 0)
                            {
                                //Defaults to CLNS.AttackBox.SparkRenderType.ALL
                                int sparkTargetCount = attackBoxesHitInFrame.Count;

                                foreach (CLNS.AttackBox attackBox in attackBoxesHitInFrame)
                                {
                                    if (attackBox.GetSparkRenderType() == CLNS.AttackBox.SparkRenderType.FRAME
                                            || attackBox.GetSparkRenderType() == CLNS.AttackBox.SparkRenderType.ONCE)
                                    {
                                        sparkTargetCount = 1;
                                    }

                                    if (attackBox.GetSparkRenderType() != CLNS.AttackBox.SparkRenderType.ONCE
                                            && targetAttackInfo.hitByAttackFrameCount < sparkTargetCount 
                                                    || attackBox.GetSparkRenderType() == CLNS.AttackBox.SparkRenderType.ONCE 
                                                            && targetAttackInfo.hitByStaticAttackId != static_hit_id)
                                    {
                                        float x1 = TargetBodyX(target, entity, attackBox);
                                        float y1 = TargetBodyY(target, entity, attackBox);

                                        Entity hitSpark1 = new Entity(Entity.EntityType.HIT_FLASH, "SPARK1");
                                        hitSpark1.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Leo/Spark1", Animation.Type.ONCE));
                                        hitSpark1.SetAnimationState(Animation.State.STANCE);
                                        hitSpark1.SetFrameDelay(Animation.State.STANCE, 2);
                                        //hitSpark1.SetFrameDelay(Animation.State.STANCE, 1, 5);
                                        hitSpark1.SetScale(1.2f, 1.2f);
                                        hitSpark1.SetPostion(x1, y1, target.GetPosZ() + 1);
                                        hitSpark1.SetFade(225);

                                        renderManager.AddEntity(hitSpark1);
                                        targetAttackInfo.hitByAttackFrameCount++;

                                        if (attackBox.GetSparkRenderType() == CLNS.AttackBox.SparkRenderType.ONCE)
                                        {
                                            targetAttackInfo.hitByStaticAttackId = static_hit_id;
                                        }

                                        if (attackBox.GetSparkRenderType() == CLNS.AttackBox.SparkRenderType.FRAME
                                                || attackBox.GetSparkRenderType() == CLNS.AttackBox.SparkRenderType.ONCE)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                            Debug.WriteLine("AttackBoxes: " + attackBoxesHitInFrame.Count);
                            Debug.WriteLine("SparkCount: " + renderManager.entities.FindAll(item => item.IsEntity(Entity.EntityType.HIT_FLASH)).ToList().Count);
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
                //CheckLand(entity);
                CheckFall(entity);
            }
        }
    }
}
