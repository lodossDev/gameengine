using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class CLNS
    {
        public static float VISIBILITY = 0.4f;
        public static int THICKNESS = 2;
        public enum BoxType { HIT_BOX, BODY_BOX, BOUNDS_BOX, OTHER }
        public enum DrawType { LINES, FILL }


        public class BoundingBox
        {
            protected Texture2D sprite;
            protected Rectangle rect;
            protected Vector2 offset;
            protected Color color;
            protected BoxType type;
            protected int frame;
            protected float zDepth;


            public BoundingBox(BoxType type, int w, int h, int x, int y, int frame = -1)
            {
                SetSpriteColor(type);
                rect = new Rectangle(0, 0, w, h);
                offset = new Vector2(x, y);
            }

            private void SetSpriteColor(BoxType type)
            {
                this.type = type;

                switch (this.type)
                {
                    case BoxType.BODY_BOX:
                        color = Color.Blue;
                        break;
                    case BoxType.HIT_BOX:
                        color = Color.Red;
                        break;
                    case BoxType.BOUNDS_BOX:
                        color = Color.Yellow;
                        break;
                    default:
                        color = Color.ForestGreen;
                        break;
                }

                sprite = new Texture2D(Setup.graphicsDevice, 1, 1);
                Color[] colorData = new Color[1];
                colorData[0] = color;
                sprite.SetData<Color>(colorData);
            }

            public void SetFrame(int frame)
            {
                this.frame = frame - 1;
            }

            public void SetOffSet(float x, float y)
            {
                offset.X = x;
                offset.Y = y;
            }

            public void SetZdepth(float depth)
            {
                zDepth = depth;
            }

            public void Update(GameTime gameTime, bool isLeft, Vector2 position)
            {
                if (isLeft)
                {
                    rect.X = (int)(position.X - (rect.Width + offset.X - 5));
                }
                else
                {
                    rect.X = (int)(position.X + offset.X);
                }

                rect.Y = (int)(position.Y + offset.Y);
            }

            public Texture2D GetSprite()
            {
                return sprite;
            }

            public Rectangle GetRect()
            {
                return rect;
            }

            public Vector2 GetOffset()
            {
                return offset;
            }

            public Color GetColor()
            {
                return color;
            }

            public BoxType GetBoxType()
            {
                return type;
            }

            public int GetFrame()
            {
                return frame;
            }

            public float GetZdepth()
            {
                return zDepth;
            }

            public virtual void DrawRectangle(DrawType drawType)
            {
                if (drawType == DrawType.LINES || drawType == DrawType.FILL)
                {
                    DrawStraightLine(new Vector2((int)rect.X, (int)rect.Y), new Vector2((int)rect.X + rect.Width, (int)rect.Y), sprite, color, THICKNESS); //top bar 
                    DrawStraightLine(new Vector2((int)rect.X, (int)rect.Y + rect.Height), new Vector2((int)rect.X + rect.Width + 1 * THICKNESS, (int)rect.Y + rect.Height), sprite, color, THICKNESS); //bottom bar 
                    DrawStraightLine(new Vector2((int)rect.X, (int)rect.Y), new Vector2((int)rect.X, (int)rect.Y + rect.Height), sprite, color, THICKNESS); //left bar 
                    DrawStraightLine(new Vector2((int)rect.X + rect.Width, (int)rect.Y), new Vector2((int)rect.X + rect.Width, (int)rect.Y + rect.Height), sprite, color, THICKNESS); //right bar 
                }
                
                if (drawType == DrawType.FILL)
                {
                    Setup.spriteBatch.Draw(sprite, new Vector2((float)rect.X, (float)rect.Y), rect, color * VISIBILITY, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
                }
            }

            //draws a line (rectangle of thickness) from A to B.  A and B have make either horiz or vert line. 
            public static void DrawStraightLine(Vector2 A, Vector2 B, Texture2D tex, Color color, int thickness)
            {
                Rectangle rect;

                if (A.X < B.X) // horiz line 
                {
                    rect = new Rectangle((int)A.X, (int)A.Y, (int)(B.X - A.X), thickness);
                }
                else //vert line 
                {
                    rect = new Rectangle((int)A.X, (int)A.Y, thickness, (int)(B.Y - A.Y));
                }

                Setup.spriteBatch.Draw(tex, rect, color);
            }
        }

        public class BoundsBox : BoundingBox
        {
            public BoundsBox(int w, int h, int x, int y, int depth) : base(BoxType.BOUNDS_BOX, w, h, x, y) {
                SetZdepth(depth);
            }

            public float GetHeight()
            {
                return rect.Height - GetZdepth();
            }
        }

        public class AttackBox : BoundingBox
        {
            public enum AttackPosition { STANDING, LOW, AIR, NONE };
            public enum BlockPosition { HI, LOW, AIR, NONE };
            public enum SparkRenderType { ALL, FRAME, ONCE }

            
            private float hitPauseTime;
            private float painTime;
            private int hitDamage;
            private int hitPoints;
            private float hitStrength;
            private int resetHit;
            private int comboStep;
            private int juggleCost;
            private AttackPosition attackPosition;
            private BlockPosition blockPosition;
            private SparkRenderType sparkRenderType;
            private Effect.EffectState sparkState;
            private Vector2 sparkOffset;


            public AttackBox(int w, int h, int x, int y, int resetHit = -1, float zDepth = 30,
                                        float hitPauseTime = 1 / 60, float painTime = 20 / 60, int hitDamage = 5,
                                        int hitPoints = 5, float hitStrength = 0.4f, int comboStep = 1,
                                        int juggleCost = 0, AttackPosition attackPosiiton = AttackPosition.NONE,
                                        BlockPosition blockPosition = BlockPosition.NONE,
                                        SparkRenderType sparkRenderFrame = SparkRenderType.ALL,
                                        Effect.EffectState sparkState = Effect.EffectState.NONE,
                                        float sparkX = 0, float sparkY = 0)
                                    : base(BoxType.HIT_BOX, w, h, x, y)
            {
                sparkOffset = Vector2.Zero;

                SetResetHit(resetHit);
                SetZdepth(zDepth);
                SetHitPauseTime(hitPauseTime);
                SetPainTime(painTime);
                SetHitDamage(hitDamage);
                SetHitPoints(hitPoints);
                SetHitStrength(hitStrength);
                SetComboStep(comboStep);
                SetJuggleCost(juggleCost);
                SetAttackPosition(attackPosition);
                SetSparkRenderType(sparkRenderFrame);
                SetSparkState(sparkState);
                SetSparkOffset(sparkX, sparkY);
            }

            public void SetHitPauseTime(float pauseTime)
            {
                hitPauseTime = pauseTime;
            }

            public void SetPainTime(float painTime)
            {
                this.painTime = painTime;
            }

            public void SetHitDamage(int damage)
            {
                hitDamage = damage;
            }

            public void SetHitPoints(int points)
            {
                hitPoints = points;
            }

            public void SetHitStrength(float strength)
            {
                hitStrength = strength;
            }

            public void SetResetHit(int hit)
            {
                resetHit = hit;
            }

            public void SetComboStep(int step)
            {
                comboStep = step;
            }

            public void SetJuggleCost(int cost)
            {
                juggleCost = cost;
            }

            public void SetAttackPosition(AttackPosition position)
            {
                attackPosition = position;
            }

            public void SetBlockPosition(BlockPosition position)
            {
                blockPosition = position;
            }

            public void SetSparkRenderType(SparkRenderType sparkRenderFrame)
            {
                this.sparkRenderType = sparkRenderFrame;
            }

            public void SetSparkState(Effect.EffectState sparkState)
            {
                this.sparkState = sparkState;
            }

            public void SetSparkOffset(float x1, float y1)
            {
                sparkOffset.X = x1;
                sparkOffset.Y = y1;
            }

            public float GetHitPauseTime()
            {
                return hitPauseTime;
            }

            public float GetPainTime()
            {
                return painTime;
            }

            public int GetHitDamage()
            {
                return hitDamage;
            }

            public int GetHitPoints()
            {
                return hitPoints;
            }

            public float GetHitStrength()
            {
                return hitStrength;
            }

            public int GetResetHit()
            {
                return resetHit;
            }

            public int GetComboStep()
            {
                return comboStep;
            }

            public int GetJuggleCost()
            {
                return juggleCost;
            }

            public AttackPosition GetAttackPosition()
            {
                return attackPosition;
            }

            public BlockPosition GetBlockPosition()
            {
                return blockPosition;
            }

            public SparkRenderType GetSparkRenderType()
            {
                return sparkRenderType;
            }

            public Effect.EffectState GetSparkState()
            {
                return sparkState;
            }

            public Vector2 GetSparkOffset()
            {
                return sparkOffset;
            }
        }
    }
}
