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
        private bool renderBoxes;

        public RenderManager()
        {
            renderBoxes = false;
        }

        public void ShowBoxes()
        {
            renderBoxes = true;
        }

        public void HideBoxes()
        {
            renderBoxes = false;
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
                    List<CLNS.BoundingBox> currentBoxes = currentSprite.GetCurrentBoxes();

                    if (stance != null)
                    {
                        //Setup.spriteBatch.Draw(stance.GetCurrentTexture(), stance.GetPosition(), null, Color.White * 1f, 0f, entity.GetStanceOrigin(), entity.GetScale(), stance.GetEffects(), 0f);
                    }

                    Setup.spriteBatch.Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, Color.White * 1f, 0f, entity.GetOrigin(), entity.GetScale(), entity.GetEffects(), 0f);

                    if (renderBoxes)
                    {
                        if (currentBoxes != null)
                        {
                            foreach (CLNS.BoundingBox box in currentBoxes)
                            {
                                //if (box.Render())
                                //{
                                    //Setup.spriteBatch.Draw(box.GetSprite(), box.GetRect(), box.GetColor() * CLNS.BoundingBox.VISIBILITY);
                                    box.DrawRectangle(CLNS.DrawType.LINES);
                                //}
                            }
                        }

                        foreach (CLNS.BoundingBox box in entity.GetBoxes())
                        {
                            //if (box.Render())
                            //{
                                //Setup.spriteBatch.Draw(box.GetSprite(), box.GetRect(), box.GetColor() * CLNS.BoundingBox.VISIBILITY);
                                box.DrawRectangle(CLNS.DrawType.LINES);
                            //}
                        }
                    }

                    Setup.spriteBatch.Draw(entity.GetBaseSprite().GetCurrentTexture(), entity.GetBasePosition(), Color.White * 1f);
                }
            }
        }
    }
}
