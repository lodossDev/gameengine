using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Player : Character
    {
        public Player(String name) : base(EntityType.PLAYER, name) {
        }
    }
}
