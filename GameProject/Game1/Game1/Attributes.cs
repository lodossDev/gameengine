using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Attributes
    {
        public enum AttackState
        {
            NO_ATTACK_ID = -1
        }

        public class AttackInfo
        {
            public int attackId;
            public int hitByAttackId;
            public Animation.State lastAttackState;
            public Animation.State hitByAttackState;

            public AttackInfo()
            {
                attackId = (int)AttackState.NO_ATTACK_ID;
                hitByAttackId = 0;
                lastAttackState = Animation.State.NONE;
            }
        }
    }
}
