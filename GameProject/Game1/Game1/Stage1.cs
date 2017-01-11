using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Stage1 : Level{
        public Stage1() :base("Stage 1") {

        }

        public override void Load() {
            /*Entity layer2 = new Entity(Entity.EntityType.LEVEL, "LAYER 2");
            layer2.AddSprite(Animation.State.NONE, new Sprite(Setup.contentManager.Load<Texture2D>("Sprites/Levels/Stage1/ddpanel1")), true);

            layer2.SetPostion(1740, 0, -30);
            layer2.SetScale(3, 3);
            AddLayer(2, layer2);
            */

            Entity layer1 = new Entity(Entity.EntityType.LEVEL, "LAYER 1");
            layer1.AddSprite(Animation.State.NONE, new Sprite(Setup.contentManager.Load<Texture2D>("Sprites/Levels/Stage1/normal01")), true);

            layer1.SetPostion(4000, 0, -80);
            layer1.SetScale(3.1f, 3.2f);
            AddLayer(1, layer1);
        }
    }
}
