using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class BoundingBox
    {
        public static float VISIBILITY = 0.7f;
        public enum BoxType { HIT_BOX, BODY_BOX, COLLIDE_BOX }

        protected Texture2D sprite;
        protected Rectangle rect;
        protected Vector2 offset;
        protected Color color;
        protected BoxType type;
        protected bool draw;
        protected int frame;


        public BoundingBox(BoxType type, int w, int h, int x, int y, int frame = -1)
        {
            SetSpriteColor(type);
            rect = new Rectangle(0, 0, w, h);
            offset = new Vector2(x, y);
            draw = true;
        }

        private void SetSpriteColor(BoxType type)
        {
            this.type = type;

            switch (this.type)
            {
                case BoxType.BODY_BOX:
                    color = Color.Green;
                    break;
                case BoxType.HIT_BOX:
                    color = Color.Red;
                    break;
                default:
                    color = Color.Blue;
                    break;
            }

            sprite = new Texture2D(Setup.graphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = color;
            sprite.SetData<Color>(colorData);
        }

        public void SetRender(bool draw)
        {
            this.draw = draw;
        }

        public void SetFrame(int frame)
        {
            this.frame = frame - 1;
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

        public bool Render()
        {
            return draw;
        }

        public void Visible()
        {
            SetRender(true);
        }

        public void Hide()
        {
            SetRender(false);
        }

        public void DrawRectangle(bool solid)
        {
            if (!solid)
            {

                //Vector2 Position = new Vector2(rect.X, rect.Y);
                int thickness = 1;

                drawStraightLine(new Vector2((int)rect.X, (int)rect.Y), new Vector2((int)rect.X + rect.Width, (int)rect.Y), sprite, color, thickness); //top bar 
                drawStraightLine(new Vector2((int)rect.X, (int)rect.Y + rect.Height), new Vector2((int)rect.X + rect.Width + 1 * thickness, (int)rect.Y + rect.Height), sprite, color, thickness); //bottom bar 
                drawStraightLine(new Vector2((int)rect.X, (int)rect.Y), new Vector2((int)rect.X, (int)rect.Y + rect.Height), sprite, color, thickness); //left bar 
                drawStraightLine(new Vector2((int)rect.X + rect.Width, (int)rect.Y), new Vector2((int)rect.X + rect.Width, (int)rect.Y + rect.Height), sprite, color, thickness); //right bar 
            }
            else
            {
                Setup.spriteBatch.Draw(sprite, new Vector2((float)rect.X, (float)rect.Y), rect, color, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
            }

        }

        //draws a line (recttangle of thickness) from A to B.  A and B have make either horiz or vert line. 
        public static void drawStraightLine(Vector2 A, Vector2 B, Texture2D tex, Color color, int thickness)
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

    public class AttackBox : BoundingBox
    {
        private float zDepth;
        private float hitPauseTime;
        private float painTime;
        private float hitDamage;
        private int hitPoints;
        private int resetHit;


        public AttackBox(BoxType type, int w, int h, int x, int y, int resetHit = -1, float zDepth = 30,
                                    float hitPauseTime = 1 / 60, float painTime = 20 / 60, float hitDamage = 0.2f,
                                    int hitPoints = 5) : base(type, w, h, x, y)
        {
            SetResetHit(resetHit);
            SetZdepth(zDepth);
            SetHitPauseTime(hitPauseTime);
            SetPainTime(painTime);
            SetHitDamage(hitDamage);
            SetHitPoints(hitPoints);
        }

        public void SetZdepth(float depth)
        {
            zDepth = depth;
        }

        public void SetHitPauseTime(float pauseTime)
        {
            hitPauseTime = pauseTime;
        }

        public void SetPainTime(float painTime)
        {
            this.painTime = painTime;
        }

        public void SetHitDamage(float damage)
        {
            hitDamage = damage;
        }

        public void SetHitPoints(int points)
        {
            hitPoints = points;
        }

        public void SetResetHit(int hit)
        {
            resetHit = hit;
        }

        public float GetZdepth()
        {
            return zDepth;
        }

        public float GetHitPauseTime()
        {
            return hitPauseTime;
        }

        public float GetPainTime()
        {
            return painTime;
        }

        public float GetHitDamage()
        {
            return hitDamage;
        }

        public int GetHitPoints()
        {
            return hitPoints;
        }

        public int GetResetHit()
        {
            return resetHit;
        }
    }
}
