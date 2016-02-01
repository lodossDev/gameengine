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
        public static float posx = 0.0f;
        public static float ppx = 0.0f;
        public static float ssx = 0.0f;
        public static float ddx = 0.0f;
        public static string tName = "";

        public List<Entity> FindBelow(Entity entity)
        {
            List<Entity> found = new List<Entity>();
            Rectangle entityBox = entity.GetBoxes(BoundingBox.BoxType.BODY)[0].GetBox();

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(BoundingBox.BoxType.BODY)[0].GetBox();

                    if (EntityHelper.InRangeZ(entity, target, target.GetDepth())
                            && InBoundsX(entityBox, targetBox, 20)
                            && entityBox.Intersects(targetBox)
                            && Math.Abs(entity.GetPosY()) + 20 >= (Math.Abs(target.GetPosY()) + Math.Abs(target.GetHeight())))
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
            Rectangle entityBox = entity.GetBoxes(BoundingBox.BoxType.BODY)[0].GetBox();

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(BoundingBox.BoxType.BODY)[0].GetBox();

                    if (EntityHelper.InRangeZ(entity, target, target.GetDepth())
                            && InBoundsX(entityBox, targetBox, 20)
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

        //Move these in static helper class........................................
        public bool InBoundsX(Rectangle r1, Rectangle r2, int offset = 15)
        {
            return (r1.Right >= r2.Left + (r2.Width / offset)
                        && r1.Left <= r2.Right - (r2.Width / offset));
        }

        public bool TouchLeft(Rectangle r1, Rectangle r2, int offset = 4)
        {
            return (r1.Right <= r2.Right
                        && r1.Right >= r2.Left - 5
                        && r1.Top <= r2.Bottom - (r2.Width / offset)
                        && r1.Bottom >= r2.Top + (r2.Width / offset));
        }

        public bool TouchRight(Rectangle r1, Rectangle r2, int offset = 4)
        {
            return (r1.Left >= r2.Left
                        && r1.Left <= r2.Right + 5
                        && r1.Top <= r2.Bottom - (r2.Width / offset)
                        && r1.Bottom >= r2.Top + (r2.Width / offset));
        }

        public bool TouchTop(Rectangle r1, Rectangle r2, int offset = 5)
        {
            return r1.Bottom >= r2.Top - 1
                        && r1.Bottom <= r2.Top + (r2.Height / 2)
                        && r1.Right >= r2.Left + (r2.Width / offset)
                        && r1.Left <= r2.Right - (r2.Width / offset);
        }

        public bool TouchBottom(Rectangle r1, Rectangle r2, int offset = 15)
        {
            return r1.Top <= r2.Bottom + (r2.Height / 2)
                        && r1.Top >= r2.Bottom - (r2.Height / 2)
                        && r1.Right >= r2.Left + (r2.Width / offset)
                        && r1.Left <= r2.Right - (r2.Width / offset);
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
            Rectangle entityBox = entity.GetBoxes(BoundingBox.BoxType.BODY)[0].GetBox();
            
            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(BoundingBox.BoxType.BODY)[0].GetBox();

                    if (EntityHelper.InRangeZ(entity, target, target.GetDepth())
                           && entityBox.Intersects(targetBox)
                           && TouchBottom(entityBox, targetBox))
                    {
                        Debug.WriteLine("E: " + entity.GetName() + " TOP: " + entityBox.Top);
                        Debug.WriteLine("E: " + target.GetName() + " bottom: " + targetBox.Bottom);

                        //entity.SetGroundBase(target.GetPosY() + entityBox.Height + 5);
                        //entity.SetPosY(-(entityBox.Top - targetBox.Height) + h2);
                        entity.Toss(5);
                    }
                }
            }

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    Rectangle targetBox = target.GetBoxes(BoundingBox.BoxType.BODY)[0].GetBox();

                    if (EntityHelper.InRangeZ(entity, target, target.GetDepth())
                           && TouchTop(entityBox, targetBox))
                    {
                        if (entity.GetVelocity().Y > 1)
                        {
                            float posy = (Math.Abs(target.GetPosY()) + target.GetHeight()) + 1;
                            entity.SetGround(-posy);
                        }

                        if (target.IsToss())
                        {
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
            BoundingBox bb1 = entity.GetBoxes(BoundingBox.BoxType.BODY)[0];
            Rectangle entityBox = bb1.GetBox();
            Sprite pStance = entity.GetSprite(Animation.State.STANCE);
            int pWidth = entityBox.Width;
            entity.GetCollisionInfo().Reset();

            foreach (Entity target in entities)
            {
                if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                {
                    BoundingBox bb2 = target.GetBoxes(BoundingBox.BoxType.BODY)[0];
                    Rectangle targetBox = bb2.GetBox();
                    Sprite tStance = target.GetSprite(Animation.State.STANCE);
                    int tWidth = targetBox.Width;
                    int tHeight = targetBox.Height;

                    if (EntityHelper.InRangeZ(entity, target, target.GetDepth())
                            && entityBox.Intersects(targetBox))
                    {
                        Vector2 x1 = entityBox.GetIntersectionDepth(targetBox);
                        
                        if (!(Math.Abs(entity.GetPosY()) + 2 >= (Math.Abs(target.GetPosY()) + target.GetHeight()))
                                && !TouchTop(entityBox, targetBox))
                        {
                            if (!(entity.GetDirZ() > 0 && entity.GetPosZ() <= target.GetPosZ() - target.GetDepth())
                                    && !(entity.GetDirZ() < 0 && entity.GetPosZ() >= target.GetPosZ() + target.GetDepth()))
                            {
                                if ((entity.GetDirX() > 0) && TouchLeft(entityBox, targetBox))//left
                                {
                                    entity.VelX(0f);
                                    entity.GetCollisionInfo().Left();
                                    entity.SetPosX(entity.GetPosX() + x1.X + 2);
                                }
                                else if ((entity.GetDirX() < 0) && TouchRight(entityBox, targetBox))//right
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

        private static int hit_id = 1;

        private void CheckAttack(Entity entity)
        {
            ComboAttack.Chain attackChain = entity.GetDefaultAttackChain();
            Attributes.AttackInfo attackInfo = entity.GetAttackInfo();
            List<BoundingBox> attackBoxes = entity.GetCurrentSprite().GetCurrentBoxes(BoundingBox.BoxType.HIT_BOX);

            if (attackBoxes != null && attackBoxes.Count > 0)
            {
                foreach (Entity target in entities)
                {
                    if (entity != target && target.IsEntity(Entity.EntityType.OBSTACLE))
                    {
                        Rectangle targetBox = target.GetBoxes(BoundingBox.BoxType.BODY)[0].GetBox();
                        Attributes.AttackInfo targetAttackInfo = target.GetAttackInfo();

                        if (EntityHelper.InRangeZ(entity, target, target.GetDepth())
                                && targetAttackInfo.hitByAttackId != attackInfo.attackId
                                && entity.InAttackFrame())
                        {
                            foreach (BoundingBox attack in attackBoxes)
                            {
                                if (attack.GetBox().Intersects(targetBox))
                                {
                                    if (targetAttackInfo.hitByAttackState != attackInfo.lastAttackState)
                                    {
                                        hit_id++;
                                        attackChain.IncrementMoveIndex();
                                        attackInfo.lastAttackState = targetAttackInfo.hitByAttackState = entity.GetCurrentAnimationState();
                                    }

                                    target.Toss(-5f);
                                    targetAttackInfo.hitByAttackId = attackInfo.attackId = hit_id;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            posx = 0;
            ppx = 0.0f;
            ssx = 0.0f;
            ddx = 0.0f;

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
