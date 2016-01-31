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

        public enum CollisionState
        {
            NO_COLLISION = -1,
            RIGHT_SIDE = 0,
            LEFT_SIDE = 1,
            TOP = 2,
            BOTTOM = 3
        }

        public class CollisionInfo
        {
            public CollisionState collide_x;
            public CollisionState collide_y;
            public CollisionState collide_z;

            public CollisionInfo() 
            {
                Reset();
            }

            public void Reset()
            {
                collide_x = CollisionState.NO_COLLISION;
                collide_y = CollisionState.NO_COLLISION;
                collide_z = CollisionState.NO_COLLISION;
            }

            public void Right()
            {
                collide_x = CollisionState.RIGHT_SIDE;
            }

            public void Left()
            {
                collide_x = CollisionState.LEFT_SIDE;
            }

            public void Top()
            {
                collide_z = CollisionState.TOP;
            }

            public void Bottom()
            {
                collide_z = CollisionState.BOTTOM;
            }

            public void Above()
            {
                collide_y = CollisionState.TOP;
            }

            public void Below()
            {
                collide_y = CollisionState.BOTTOM;
            }
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

        public class FrameInfo
        {
            public enum FrameState{NO_FRAME = -1}
            private int startFrame;
            private int endFrame;

            public FrameInfo(int startFrame, int endFrame)
            {
                this.startFrame = startFrame;
                this.endFrame = endFrame;
            }

            public FrameInfo(int startFrame)
            {
                this.startFrame = startFrame;
                this.endFrame = (int)FrameState.NO_FRAME;
            }

            public int GetStartFrame()
            {
                return startFrame;
            }

            public int GetEndFrame()
            {
                return endFrame;
            }

            public void SetStartFrame(int sx)
            {
                startFrame = sx;
            }

            public void SetEndFrame(int ex)
            {
                endFrame = ex;
            }
        }
    }
}
