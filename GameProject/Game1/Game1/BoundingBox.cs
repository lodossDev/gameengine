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
        public enum BoxType {HIT_BOX, BODY_BOX, BOUNDS, BODY, COLLISION}
        private Texture2D sprite;
        private Rectangle rect;
        private Vector2 offset;
        private Color color;
        private BoxType type;
        private bool draw;

        public BoundingBox(BoxType type, int w, int h, int x, int y)
        {
            SetSpriteColor(type);
            rect = new Rectangle(0, 0, w, h);
            offset = new Vector2(x, y);
            draw = true;
        }

        private void SetSpriteColor(BoxType type)
        {
            this.type = type;

            switch(this.type)
            {
                case BoxType.BODY:
                    color = Color.Green;
                    break;
                case BoxType.BODY_BOX:
                    color = Color.Blue;
                    break;
                case BoxType.HIT_BOX:
                    color = Color.Red;
                    break;
                default:
                    color = Color.Green;
                    break;  
            }

            sprite = new Texture2D(Setup.graphicsDevice, 1, 1);
            Color[] colourData = new Color[1];
            colourData[0] = color;
            sprite.SetData<Color>(colourData);
        }

        public void SetRender(bool draw)
        {
            this.draw = draw;
        }

        public void Update(GameTime gameTime, bool isLeft, Vector2 position)
        {
            if (isLeft)
            {
                rect.X = (int)(position.X - (rect.Width + offset.X) + 5);
            }
            else {
                rect.X = (int)(position.X + offset.X);
            }

            rect.Y = (int)(position.Y + offset.Y);
        }

        public Texture2D GetSprite()
        {
            return sprite;
        }

        public Rectangle GetBox()
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
    }
}
