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
    public class CollisionManager : Manager {
        public static int hitCount = 0;
        public static long current_hit_id = 0;
        private SoundEffect hiteffect1;
        private SoundEffectInstance soundInstance, soundInstance2;
        private RenderManager renderManager;

        public CollisionManager(RenderManager renderManager) {
            hiteffect1 = Setup.contentManager.Load<SoundEffect>("Sounds//hit1");
            soundInstance = hiteffect1.CreateInstance();

            soundInstance2 = Setup.contentManager.Load<SoundEffect>("Sounds//test").CreateInstance();

            this.renderManager = renderManager;
        }

        public List<Entity> FindAbove(Entity entity) {
            List<Entity> found = new List<Entity>();
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();
           
            int ePosY = (int)Math.Abs(Math.Round((double)entity.GetPosY()));
            int ePosZ = (int)Math.Abs(Math.Round((double)entity.GetPosZ()));
            int eDepth = (int)Math.Abs(Math.Round((double)entityBox.GetZdepth()));
            int eGround = (int)Math.Abs(Math.Round((double)entity.GetGround()));
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - eDepth));

            int vx = Math.Abs(entity.GetDirVelX());
            int vz = Math.Abs(entity.GetDirVelZ());

            foreach (Entity target in entities) {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE)) {
                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    if (entityBox.Intersects(targetBox) && eDepthBox.Intersects(tDepthBox) 
                            && ePosY < tHeight / 2) {

                        bool isWithInBoundsX1 = (entity.HorizontalCollisionLeft(target, vx) == true && entity.HorizontalCollisionRight(target, vx) == true);
                        bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, vz) == true && entity.VerticleCollisionBottom(target, vz) == true);

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
           
            int ePosY = (int)Math.Abs(Math.Round((double)entity.GetPosY()));
            int ePosZ = (int)Math.Abs(Math.Round((double)entity.GetPosZ()));
            int eDepth = (int)Math.Abs(Math.Round((double)entityBox.GetZdepth()));
            int eGround = (int)Math.Abs(Math.Round((double)entity.GetGround()));
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - eDepth));

            int vx = Math.Abs(entity.GetDirVelX());
            int vz = Math.Abs(entity.GetDirVelZ());

            foreach (Entity target in entities) {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE)) {
                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    bool isWithInBoundsX1 = (entity.HorizontalCollisionLeft(target, vx) == true && entity.HorizontalCollisionRight(target, vx) == true);
                    bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, vz) == true && entity.VerticleCollisionBottom(target, vz) == true);

                    if (entityBox.Intersects(targetBox) 
                            && eDepthBox.Intersects(tDepthBox) && ePosY >= tHeight - 10
                            && isWithInBoundsX1 && isWithInBoundsZ1) {

                        found.Add(target);
                    }

                    if (entity.GetRayBottom() != null 
                            && entity.GetRayBottom().Intersects(targetBox) 
                            && isWithInBoundsX1 && isWithInBoundsZ1 && ePosY >= tHeight - 20)  {
                        
                        found.Add(target);
                    }
                }
            }

            return found.Distinct().ToList();
        }

        private void CheckFall(Entity entity) {
            List<Entity> belowEntities = FindBelow(entity);

            if ((belowEntities.Count == 0 
                    && (int)entity.GetGround() != (int)entity.GetGroundBase()
                    && entity.HasLanded())

                    || (belowEntities.Count == 0 
                            && (int)entity.GetGround() != (int)entity.GetGroundBase()
                            && entity.GetCollisionInfo().onTop)) {

                entity.SetGround(entity.GetGroundBase());
                entity.GetCollisionInfo().onTop = false;

                if (!entity.IsToss()) {
                    entity.SetAnimationState(Animation.State.FALL1);
                    entity.Toss(5);
                }
            }
        }

        private void CheckLand(Entity entity) {
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();
           
            int ePosY = (int)Math.Abs(Math.Round((double)entity.GetPosY()));
            int ePosZ = (int)Math.Abs(Math.Round((double)entity.GetPosZ()));
            int eDepth = (int)Math.Abs(Math.Round((double)entityBox.GetZdepth()));
            int eGround = (int)Math.Abs(Math.Round((double)entity.GetGround()));
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - eDepth));

            int vx = Math.Abs(entity.GetDirVelX());
            int vz = Math.Abs(entity.GetDirVelZ());
          
            foreach (Entity target in entities) {

                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE)) {

                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    if (entityBox.Intersects(targetBox) && eDepthBox.Intersects(tDepthBox)) {

                        bool isWithInBoundsX1 = (entity.HorizontalCollisionLeft(target, vx) == true && entity.HorizontalCollisionRight(target, vx) == true);
                        bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, vz) == true && entity.VerticleCollisionBottom(target, vz) == true);

                        if (entity.GetRayBottom() != null 
                                && entity.GetRayBottom().Intersects(targetBox) 
                                && isWithInBoundsX1 && isWithInBoundsZ1 
                                && entity.GetCollisionInfo().onTop)  { 
                        
                            if (target.IsMovingY()) { 
                                entity.MoveY(target.GetDirVelY());
                                entity.SetGround(-(tHeight + 5) - target.GetDirVelY());
                            } else {
                                entity.SetGround(-(tHeight + 5));
                            }
                        }

                        if (isWithInBoundsX1 && isWithInBoundsZ1 
                                && (double)entity.GetVelocity().Y > 0 && ePosY >= tHeight - 10) {

                            entity.GetCollisionInfo().onTop = true;
                            entity.MoveY(target.GetDirVelY());
                            entity.SetGround(-(tHeight + 5) - target.GetDirVelY());
                        }
                    }
                }
            }
        }
        
        private void CheckBounds(Entity entity) {
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();
           
            int ePosY = (int)Math.Abs(Math.Round((double)entity.GetPosY()));
            int ePosZ = (int)Math.Abs(Math.Round((double)entity.GetPosZ()));
            int eDepth = (int)Math.Abs(Math.Round((double)entityBox.GetZdepth()));
            int eGround = (int)Math.Abs(Math.Round((double)entity.GetGround()));
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - eDepth));

            int vx = Math.Abs(entity.GetDirVelX());
            int vz = Math.Abs(entity.GetDirVelZ());

            List<Entity> aboveEntities = FindAbove(entity);
            List<Entity> belowEntities = FindBelow(entity);
            
            foreach (Entity target in entities) {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE)) {
                    Entity aboveTarget = aboveEntities.Find(item => item == target);
                    Entity belowTarget = belowEntities.Find(item => item == target);

                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    if (entityBox.Intersects(targetBox) && eDepthBox.Intersects(tDepthBox) 
                            && ePosY <= tHeight - 10 && eHeight >= tPosY 
                            && (/*aboveTarget != target &&*/ belowTarget != target)) { 

                        bool isWithInBoundsX1 = ((entity.HorizontalCollisionLeft(target, vx) == true && entity.HorizontalCollisionRight(target, vx) == false
                                                    || entity.HorizontalCollisionLeft(target, vx) == false && entity.HorizontalCollisionRight(target, vx) == true));

                        bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, vz) == false && entity.VerticleCollisionBottom(target, vz) == true
                                                    || entity.VerticleCollisionTop(target, vz) == true && entity.VerticleCollisionBottom(target, vz) == false);

                        bool isWithInBoundsZ2 = (entity.VerticleCollisionTop(target, vz) == true && entity.VerticleCollisionBottom(target, vz) == true);

                        float depthX = entityBox.GetRect().GetHorizontalIntersectionDepth(targetBox.GetRect());
                        float depthZ = eDepthBox.GetRect().GetVerticalIntersectionDepth(tDepthBox.GetRect());

                        if (isWithInBoundsZ1 && !isWithInBoundsX1) { 
                            if (entity.GetDirVelZ() < 0 && entity.VerticleCollisionTop(target, 5)) {
                                entity.MoveZ(depthZ);
                                entity.GetCollisionInfo().Bottom();

                            } else if (entity.GetDirVelZ() > 0 && entity.VerticleCollisionBottom(target, 5)) {
                                entity.MoveZ(depthZ);
                                entity.GetCollisionInfo().Top();
                            }
                        }
                        
                        if ((isWithInBoundsX1 || !isWithInBoundsX1 && !isWithInBoundsZ1) && isWithInBoundsZ2) {
                            if (entity.GetDirVelX() < 0 && entity.HorizontalCollisionRight(target, 5)) {
                                entity.MoveX(depthX);
                                entity.MoveX(0, 0);
                                entity.GetCollisionInfo().Left();

                            } else if (entity.GetDirVelX() > 0 && entity.HorizontalCollisionLeft(target, 5)) {
                                entity.MoveX(depthX);
                                entity.MoveX(0, 0);
                                entity.GetCollisionInfo().Right();
                            }
                        }
                    }
                }
            }

            if (aboveEntities.Count > 0 && entity.InAir()) {
                entity.VelY(0f);
                entity.GetTossInfo().velocity.Y = entity.GetTossInfo().maxVelocity.Y;
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
            return (int)-attack.GetRect().Height;
        }

        private void OnAttack(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (entity != target) {
                ComboAttack.Chain attackChain = entity.GetDefaultAttackChain();

                if (attackChain != null) { 
                    attackChain.IncrementMoveIndex(attackBox.GetComboStep());
                }

                //entity.GetAttackInfo().hitPauseTime = 3000f;
            }
        }

        private void OnHit(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            if (target != entity) {
                hitCount++;
                hiteffect1.CreateInstance().Play();

                target.Toss(-1.2f, 0, 200000000);
                //target.MoveY(-125 * attackBox.GetHitStrength());
            }
        }

        private void OnHit(Entity target, Entity entity) {
            if (target != entity) {
                hitCount++;
                hiteffect1.CreateInstance().Play();
            }
        }

        private void CheckAttack(Entity entity) {
            List<CLNS.AttackBox> attackBoxes = entity.GetCurrentBoxes(CLNS.BoxType.HIT_BOX).Cast<CLNS.AttackBox>().ToList();
            List<CLNS.AttackBox> attackBoxesHitInFrame = new List<CLNS.AttackBox>();

            Attributes.AttackInfo entityAttackInfo = entity.GetAttackInfo();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            if (attackBoxes != null && attackBoxes.Count > 0) {

                foreach (Entity target in entities) {

                    if (entity != target) {
                        //Get all body boxes for collision with attack boxes
                        List<CLNS.BoundingBox> targetBoxes = target.GetCurrentBoxes(CLNS.BoxType.BODY_BOX);
                        targetBoxes.Add(target.GetBodyBox());
                        
                        Attributes.AttackInfo targetAttackInfo = target.GetAttackInfo();
                        CLNS.BoundingBox tDepthBox = target.GetDepthBox();
                        CLNS.BoundingBox tBodyBox = null;
                        int currentAttackHits = 0;
                        bool targetHit = false;

                        if (Math.Abs(eDepthBox.GetRect().Bottom - tDepthBox.GetRect().Bottom) < tDepthBox.GetZdepth() + 10
                                && entity.IsInAnimationAction(Animation.Action.ATTACKING) 
                                && attackBoxes.Count > 0 && targetBoxes.Count > 0) {

                            //Get all attackboxes for this one frame, you can only hit once in each attack frame.
                            foreach (CLNS.AttackBox attackBox in attackBoxes) {

                                foreach (CLNS.BoundingBox bodyBox in targetBoxes) {

                                    if (attackBox.Intersects(bodyBox)) {
                                        attackBoxesHitInFrame.Add(attackBox);
                                        tBodyBox = bodyBox;

                                        if (attackBox.GetSettingType() == CLNS.AttackBox.SettingType.ONCE) { 
                                            if (entityAttackInfo.lastAttackState != entity.GetCurrentAnimationState()) {
                                                current_hit_id++;

                                                OnAttack(entity, target, attackBox);
                                                entity.OnAttack(target, attackBox);

                                                entityAttackInfo.lastAttackState = entity.GetCurrentAnimationState();                                               
                                            }
                                        } else { 
                                            if (entityAttackInfo.lastAttackState != entity.GetCurrentAnimationState()) {
                                                OnAttack(entity, target, attackBox);
                                                entity.OnAttack(target, attackBox);

                                                entityAttackInfo.lastAttackState = entity.GetCurrentAnimationState();                                               
                                            }

                                            if (entityAttackInfo.lastAttackFrame != entity.GetCurrentSprite().GetCurrentFrame()) {
                                                current_hit_id++;
                                                entityAttackInfo.lastAttackFrame = entity.GetCurrentSprite().GetCurrentFrame();
                                            }
                                        }    
                                    }
                                }
                            }

                            attackBoxesHitInFrame = attackBoxesHitInFrame.Distinct().ToList();
                            //Debug.WriteLine("AttackBoxes: " + attackBoxesHitInFrame.Count);
                            
                            if (tBodyBox != null && attackBoxesHitInFrame.Count > 0 
                                    && targetAttackInfo.hitByAttackId != current_hit_id) {

                                foreach (CLNS.AttackBox attackBox in attackBoxesHitInFrame) {
                                    
                                    if (attackBox.Intersects(tBodyBox)) {
                                        //Debug.WriteLine("currentAttackHits: " + currentAttackHits);

                                        if (currentAttackHits > 0 && (attackBox.GetSettingType() == CLNS.AttackBox.SettingType.FRAME
                                                || attackBox.GetSettingType() == CLNS.AttackBox.SettingType.ONCE)) {

                                            break;
                                        }

                                        if (!targetHit) {
                                            OnHit(target, entity, attackBox);
                                            target.OnHit(entity, attackBox);
                                            targetHit = true;
                                        }

                                        float x1 = TargetBodyX(target, entity, attackBox);
                                        float y1 = TargetBodyY(target, entity, attackBox);

                                        Entity hitSpark1 = new Entity(Entity.EntityType.HIT_FLASH, "SPARK1");
                                        hitSpark1.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Ryo/Hitflash1", Animation.Type.ONCE));
                                        hitSpark1.SetAnimationState(Animation.State.STANCE);
                                        hitSpark1.SetFrameDelay(Animation.State.STANCE, 2);
                                        //hitSpark1.SetFrameDelay(Animation.State.STANCE, 1, 5);
                                        hitSpark1.SetScale(1.8f, 1.5f);
                                        hitSpark1.AddBoundsBox(160, 340, -60, 15, 50);

                                        Debug.WriteLine("Y HITSPAK: " + y1);
                                        hitSpark1.SetPostion(x1 , y1, (entity.GetPosZ() + target.GetBoundsBox().GetZdepth()) + 5);
                                        //hitSpark1.SetFade(225);

                                        renderManager.AddEntity(hitSpark1);
                                        currentAttackHits++;
                                    }
                                }

                                targetAttackInfo.hitByAttackId = current_hit_id;
                            }

                            //Debug.WriteLine("SparkCount: " + renderManager.entities.FindAll(item => item.IsEntity(Entity.EntityType.HIT_FLASH)).ToList().Count);
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
