using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public static class Setup
    {
        public static bool isPause = false;
        public static GraphicsDevice graphicsDevice;
        public static SpriteBatch spriteBatch;
        public static ContentManager contentManager;

        public static float rotate = 0f;
        public static float scaleX, scaleY = 0f;
    }
}
