using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Enemy : Character
    {
        public Enemy(String name) : base(EntityType.ENEMY, name) {
        }
    }
}
