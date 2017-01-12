using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1 {

    public class PhoneBooth1 : Obstacle {

        public PhoneBooth1() : base("PhoneBooth1") {
            AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Phone/Stance"), true);
            AddSprite(Animation.State.DIE1, new Sprite("Sprites/Misc/Phone/Die1"));

            AddBoundsBox(125, 380, -63, -10, 60);
            SetScale(2.2f, 2.6f);
            SetPostion(200, 0, 300);
        }
    }
}
