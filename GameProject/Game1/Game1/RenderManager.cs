using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class RenderManager : Manager
    {
        public RenderManager()
        {

        }

        public void ShowBoxes()
        {
            foreach (Entity entity in entities)
            {
                entity.ShowBoxes();
            }
        }

        public void HideBoxes()
        {
            foreach (Entity entity in entities)
            {
                entity.HideBoxes();
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            entities.Sort();

            foreach (Entity entity in entities)
            {
                if (entity.Alive())
                {
                    Sprite currentSprite = entity.GetCurrentSprite();
                    Sprite stance = entity.GetSprite(Animation.State.STANCE);
                    List<BoundingBox> currentBoxes = currentSprite.GetCurrentBoxes();

                    if (stance != null)
                    {
                        //Setup.spriteBatch.Draw(stance.GetCurrentTexture(), stance.GetPosition(), null, Color.White * 1f, 0f, entity.GetStanceOrigin(), entity.GetScale(), stance.GetEffects(), 0f);
                    }

                    Setup.spriteBatch.Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, Color.White * 1f, 0f, entity.GetOrigin(), entity.GetScale(), entity.GetEffects(), 0f);
                    
                    if (currentBoxes != null)
                    {
                        foreach (BoundingBox box in currentBoxes)
                        {
                            if (box.Render())
                            {
                                //Setup.spriteBatch.Draw(box.GetSprite(), box.GetRect(), box.GetColor() * BoundingBox.VISIBILITY);
                                box.DrawRectangle(false);
                            }
                        }
                    }

                    foreach(BoundingBox box in entity.GetBoxes())
                    {
                        if (box.Render())
                        {
                            //Setup.spriteBatch.Draw(box.GetSprite(), box.GetRect(), box.GetColor() * BoundingBox.VISIBILITY);
                            box.DrawRectangle(false);
                        }
                    }

                    Setup.spriteBatch.Draw(entity.GetBaseSprite().GetCurrentTexture(), entity.GetBasePosition(), Color.White * 1f);
                }
            }
        }
    }
}
