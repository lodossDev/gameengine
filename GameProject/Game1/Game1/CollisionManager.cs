﻿using System;
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
    public class CollisionManager : Manager {
        public static long current_hit_id = 0;
        public static long static_hit_id = 0;
        private SoundEffect hiteffect1;
        private SoundEffectInstance soundInstance, soundInstance2;
        private RenderManager renderManager;

        public CollisionManager(RenderManager renderManager) {
            hiteffect1 = Setup.contentManager.Load<SoundEffect>("Sounds//hit1");
            soundInstance = hiteffect1.CreateInstance();

            soundInstance2 = Setup.contentManager.Load<SoundEffect>("Sounds//test").CreateInstance();

            this.renderManager = renderManager;
        }

        public List<Entity> FindAbove(Entity entity){
            List<Entity> found = new List<Entity>();
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();
           
            int ePosY = (int)Math.Abs(entity.GetPosY());
            int eGround = (int)Math.Abs(entity.GetGround());
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - entityBox.GetZdepth()));

            foreach (Entity target in entities){
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE)) {
                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
           
                    int tPosY = (int)Math.Abs(target.GetPosY());
                    int tGround = (int)Math.Abs(target.GetGround());
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - targetBox.GetZdepth()));

                    if (entityBox.Intersects(targetBox) && eDepthBox.Intersects(tDepthBox) 
                            && tGround > eGround && tHeight > (eHeight / 2) && entity.InAir()) {

                        bool isWithInBoundsX1 = (entity.HorizontalCollisionLeft(target, 5) == true && entity.HorizontalCollisionRight(target, 5) == true);
                        bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, 5) == true && entity.VerticleCollisionBottom(target, 5) == true);

                        if (isWithInBoundsX1 && isWithInBoundsZ1) {
                            found.Add(target);
                        }
                    }
                }
            }

            return found;
        }

        public List<Entity> FindBelow(Entity entity){
            List<Entity> found = new List<Entity>();
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();
           
            int ePosY = (int)Math.Abs(entity.GetPosY());
            int eGround = (int)Math.Abs(entity.GetGround());
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - entityBox.GetZdepth()));

            foreach (Entity target in entities){
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE)) {
                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
           
                    int tPosY = (int)Math.Abs(target.GetPosY());
                    int tGround = (int)Math.Abs(target.GetGround());
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - targetBox.GetZdepth()));

                    if (entityBox.Intersects(targetBox) 
                            && eDepthBox.Intersects(tDepthBox) && ePosY >= tHeight - 10) {

                        bool isWithInBoundsX1 = (entity.HorizontalCollisionLeft(target, 5) == true && entity.HorizontalCollisionRight(target, 5) == true);
                        bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, 5) == true && entity.VerticleCollisionBottom(target, 5) == true);

                        if (isWithInBoundsX1 && isWithInBoundsZ1) {
                            found.Add(target);
                        }
                    }
                }
            }

            return found;
        }

        private void CheckFall(Entity entity) {
            List<Entity> belowEntities = FindBelow(entity);

            if ((belowEntities.Count == 0 && (int)entity.GetGround() != (int)entity.GetGroundBase() && entity.HasLanded()) 
                    || (belowEntities.Count == 0 && Math.Abs((int)entity.GetPosY()) != (int)entity.GetGround() && !entity.IsToss())) {

                entity.SetGround(entity.GetGroundBase());

                if (!entity.IsToss()) {
                    entity.SetAnimationState(Animation.State.FALL1);
                    entity.Toss(5);
                }
            }
        }

        private void CheckLand(Entity entity) {
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();
           
            int ePosY = (int)Math.Abs(entity.GetPosY());
            int eGround = (int)Math.Abs(entity.GetGround());
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - entityBox.GetZdepth()));

            foreach (Entity target in entities){
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE)) {
                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
           
                    int tPosY = (int)Math.Abs(target.GetPosY());
                    int tGround = (int)Math.Abs(target.GetGround());
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - targetBox.GetZdepth()));
 
                    if (entityBox.Intersects(targetBox) && eDepthBox.Intersects(tDepthBox) && ePosY >= tHeight - 10) {
                        bool isWithInBoundsX1 = (entity.HorizontalCollisionLeft(target, 5) == true && entity.HorizontalCollisionRight(target, 5) == true);
                        bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, 5) == true && entity.VerticleCollisionBottom(target, 5) == true);

                        if (isWithInBoundsX1 && isWithInBoundsZ1 && (double)entity.GetVelocity().Y > 1) {
                            entity.SetGround(-(tHeight + 5));
                        }
                    }
                }
            }
        }
        
        private void CheckBounds(Entity entity) {
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();
           
            int ePosY = (int)Math.Abs(entity.GetPosY());
            int eGround = (int)Math.Abs(entity.GetGround());
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - entityBox.GetZdepth()));

            List<Entity> aboveEntities = FindAbove(entity);
            
            foreach (Entity target in entities) {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE)) {
                    Entity aboveTarget = aboveEntities.Find(item => item == target);
                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
           
                    int tPosY = (int)Math.Abs(target.GetPosY());
                    int tGround = (int)Math.Abs(target.GetGround());
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - targetBox.GetZdepth()));

                    if (entityBox.Intersects(targetBox) && eDepthBox.Intersects(tDepthBox) 
                            && ePosY <= tHeight - 10 && eHeight >= tPosY && (aboveTarget != target)) {

                        bool isWithInBoundsX1 = ((entity.HorizontalCollisionLeft(target, 5) == true && entity.HorizontalCollisionRight(target, 5) == false
                                                    || entity.HorizontalCollisionLeft(target, 5) == false && entity.HorizontalCollisionRight(target, 5) == true));

                        bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, 5) == false && entity.VerticleCollisionBottom(target, 5) == true
                                                    || entity.VerticleCollisionTop(target, 5) == true && entity.VerticleCollisionBottom(target, 5) == false);

                        bool isWithInBoundsZ2 = (entity.VerticleCollisionTop(target, 5) == true && entity.VerticleCollisionBottom(target, 5) == true);

                        float depthX = entityBox.GetRect().GetHorizontalIntersectionDepth(targetBox.GetRect());
                        float depthZ = eDepthBox.GetRect().GetVerticalIntersectionDepth(tDepthBox.GetRect());

                        if (isWithInBoundsZ1 && !isWithInBoundsX1) { 
                            if (entity.GetAbsoluteDirZ() < 0 && entity.VerticleCollisionTop(target, 5)) {
                                entity.MoveZ(depthZ);
                                entity.GetCollisionInfo().Bottom();
                                entity.VelZ(0f);

                            } else if (entity.GetAbsoluteDirZ() > 0 && entity.VerticleCollisionBottom(target, 5)) {
                                entity.MoveZ(depthZ);
                                entity.GetCollisionInfo().Top();
                                entity.VelZ(0f);
                            }
                        }

                        if ((isWithInBoundsX1 || !isWithInBoundsX1 && !isWithInBoundsZ1) && (isWithInBoundsZ2)) {
                            if (entity.GetAbsoluteDirX() < 0 && entity.HorizontalCollisionRight(target, 5)) {
                                entity.MoveX(depthX);
                                entity.GetCollisionInfo().Left();
                                entity.VelX(0f); 

                            } else if (entity.GetAbsoluteDirX() > 0 && entity.HorizontalCollisionLeft(target, 5)) {
                                entity.MoveX(depthX);
                                entity.GetCollisionInfo().Right();
                                entity.VelX(0f); 
                            }
                        }
                    }
                }
            }

            if (aboveEntities.Count > 0 && entity.InAir()) {
                entity.GetTossInfo().velocity.Y = 5;
            }
        }
        
        private void OnAttack(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (entity != target) {
                ComboAttack.Chain attackChain = entity.GetDefaultAttackChain();
                //attackChain.IncrementMoveIndex(attackBox.GetComboStep());
                entity.GetAttackInfo().hitPauseTime = 3000f;
            }
        }

        public static int hitCount = 0;

        private void OnHit(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            if (target != entity) {
                hitCount++;
                hiteffect1.CreateInstance().Play();

                target.Toss(-10 * attackBox.GetHitStrength());
                //target.MoveY(-125 * attackBox.GetHitStrength());
            }
        }

        private float TargetBodyX(Entity target, Entity entity, CLNS.AttackBox attack) {
            int x1 = entity.GetBoundsBox().GetWidth();
            int x2 = target.GetBoundsBox().GetWidth();

            float v1 = ((target.GetPosX() / 2) + (entity.GetPosX() / 2));

            if (entity.GetPosX() >= target.GetPosX() + (x2 / 2)) {
                v1 = ((target.GetPosX() / 2) + (entity.GetPosX() / 2));
            } else if (entity.GetPosX() <= target.GetPosX() + (x2 / 2)) {
                v1 = ((target.GetPosX() / 2) + (entity.GetPosX() / 2));
            }

            if (entity.IsLeft()) {
                v1 -= attack.GetOffset().X;
            } else {
                v1 += attack.GetOffset().X;
            }

            return v1;
        }

        private float TargetBodyY(Entity target, Entity entity, CLNS.AttackBox attack) {
            float y1 = (target.GetPosY() / 2);
            return (y1 - (attack.GetRect().Height)) + (attack.GetOffset().Y / 2f) - 80;
        }

        private void CheckAttack(Entity entity) {
            List<CLNS.AttackBox> attackBoxes = entity.GetCurrentBoxes(CLNS.BoxType.HIT_BOX).Cast<CLNS.AttackBox>().ToList();
            List<CLNS.AttackBox> attackBoxesHitInFrame = new List<CLNS.AttackBox>();

            Attributes.AttackInfo entityAttackInfo = entity.GetAttackInfo();
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            if (attackBoxes != null && attackBoxes.Count > 0) {

                foreach (Entity target in entities) {
                    if (entity != target) {
                        //Get all body boxes for collision with attack boxes
                        List<CLNS.BoundingBox> targetBoxes = entity.GetCurrentBoxes(CLNS.BoxType.BODY_BOX);
                        targetBoxes.Add(target.GetBodyBox());
                        
                        Attributes.AttackInfo targetAttackInfo = target.GetAttackInfo();
                        CLNS.BoundsBox targetBox = target.GetBoundsBox();
                        CLNS.BoundingBox tDepthBox = target.GetDepthBox();
                        bool targetHit = false;
                        //int sparkTargetCount = 3;

                        if (Math.Abs(eDepthBox.GetRect().Bottom - tDepthBox.GetRect().Bottom) < tDepthBox.GetZdepth() + 10
                                && entity.IsInAnimationAction(Animation.Action.ATTACKING) && entity.InAttackFrame()) {

                            //Get all attackboxes for this one frame, you can only hit once in each attack frame.
                            foreach (CLNS.AttackBox attackBox in attackBoxes) {

                                foreach (CLNS.BoundingBox bodyBox in targetBoxes) {

                                    if (attackBox.Intersects(bodyBox)) {
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
                                               // current_hit_id++;
                                                static_hit_id++;
                                                OnAttack(entity, target, attackBox);
                                                entityAttackInfo.lastAttackState = entity.GetCurrentAnimationState();                                               
                                            }

                                            if (entityAttackInfo.lastAttackFrame != entity.GetCurrentSprite().GetCurrentFrame())
                                            {
                                                current_hit_id++;
                                                //OnAttack(entity, target, attackBox);
                                                entityAttackInfo.lastAttackFrame = entity.GetCurrentSprite().GetCurrentFrame();
                                            }
                                        }

                                        //Only 1 attack box will hit target.
                                        if (targetAttackInfo.hitByAttackId != current_hit_id)
                                        {
                                             List<Entity> sparks = new List<Entity>();

                                    

                                    if (attackBox.GetSparkRenderType() == CLNS.AttackBox.SparkRenderType.FRAME
                                            || attackBox.GetSparkRenderType() == CLNS.AttackBox.SparkRenderType.ONCE)
                                    {
                                        //sparkTargetCount = 1;
                                    }

                                    //if (targetAttackInfo.hitByAttackFrameCount < sparkTargetCount)
                                    //{
                                        float x1 = TargetBodyX(target, entity, attackBox);
                                        float y1 = TargetBodyY(target, entity, attackBox);

                                        Entity hitSpark1 = new Entity(Entity.EntityType.HIT_FLASH, "SPARK1");
                                        hitSpark1.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Leo/Spark1", Animation.Type.ONCE));
                                        hitSpark1.SetAnimationState(Animation.State.STANCE);
                                        hitSpark1.SetFrameDelay(Animation.State.STANCE, 2);
                                        //hitSpark1.SetFrameDelay(Animation.State.STANCE, 1, 5);
                                        hitSpark1.SetScale(1.2f, 1.2f);
                                        hitSpark1.SetPostion(x1 , y1, target.GetPosZ() + 8);
                                        hitSpark1.SetFade(225);

                                        sparks.Add(hitSpark1);
                                        
                                        
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
                                    //}

                                    //Debug.WriteLine("sparks: " + sparks.Count);
                                    foreach (Entity spark in sparks) {
                                         renderManager.AddEntity(spark);
                                    }

                                            targetAttackInfo.hitByAttackFrameCount = 0;
                                            OnHit(target, entity, attackBox);
                                        
                                            targetAttackInfo.hitByAttackId = current_hit_id;
                                        }
                                    }
                                }
                            }

                            attackBoxesHitInFrame = attackBoxesHitInFrame.Distinct().ToList();
                            Debug.WriteLine("AttackBoxes: " + attackBoxesHitInFrame.Count);
                            
                            if (targetHit && attackBoxesHitInFrame.Count > 0)
                            {
                                //Defaults to CLNS.AttackBox.SparkRenderType.ALL
                                
                                int i = 0;
                                foreach (CLNS.AttackBox attackBox in attackBoxesHitInFrame)
                                {
                                    
                                }
                            }

                            Debug.WriteLine("SparkCount: " + renderManager.entities.FindAll(item => item.IsEntity(Entity.EntityType.HIT_FLASH)).ToList().Count);
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime) {
            foreach (Entity entity in entities) {
                entity.GetCollisionInfo().Reset();

                CheckAttack(entity);
                CheckBounds(entity);
                CheckLand(entity);
                CheckFall(entity);
            }
        }
    }
}
