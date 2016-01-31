using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Animation
    {
        /**
        * 1 game-tick is 1/60 of a sec, so 60 ticks is 1 second
        */
        public const float TICK_RATE = 10; //fix this

        public enum State {
            NONE = -1, STANCE, WALK_TOWARDS, JUMP_START, JUMP, JUMP_TOWARDS, LAND, FALL, ATTACK1,
            ATTACK2, ATTACK3, ATTACK4, ATTACK5, ATTACK6, ATTACK7, ATTACK8, ATTACK9, ATTACK10,

            JUMP_ATTACK1, JUMP_ATTACK2, JUMP_ATTACK3, JUMP_ATTACK4, JUMP_ATTACK5, JUMP_ATTACK6,
            JUMP_ATTACK7, JUMP_ATTACK8, JUMP_ATTACK9, JUMP_ATTACK10,

            JUMP_TOWARD_ATTACK1, JUMP_TOWARD_ATTACK2, JUMP_TOWARD_ATTACK3, JUMP_TOWARD_ATTACK4,
            JUMP_TOWARD_ATTACK5, JUMP_TOWARD_ATTACK6, JUMP_TOWARD_ATTACK7, JUMP_TOWARD_ATTACK8,
            JUMP_TOWARD_ATTACK9, JUMP_TOWARD_ATTACK10,

            JUMP_RECOVER1, JUMP_RECOVER2
        }

        public enum Action {NONE, ATTACKING, JUMPING, FALLING, IDLE, WALKING, LANDING, RECOVERY}

        public enum Type {NONE, ONCE, REPEAT}

        public class Link
        {
            private Animation.State onState;
            private Animation.State toState;
            private int onFrameState;
            private bool onFrameComplete;

            public Link(Animation.State onState, Animation.State toState, int frameOnStart, bool onFrameComplete = true)
            {
                SetLink(onState, toState, frameOnStart, onFrameComplete);
            }

            public void SetLink(Animation.State onState, Animation.State toState, int frameOnStart, bool onFrameComplete = true)
            {
                this.onState = onState;
                this.toState = toState;
                this.onFrameState = (frameOnStart - 1);
                this.onFrameComplete = onFrameComplete;
            }

            public Animation.State GetOnState()
            {
                return onState;
            }

            public Animation.State GetToState()
            {
                return toState;
            }

            public int GetOnFrameStart()
            {
                return onFrameState;
            }

            public bool OnFrameComplete()
            {
                return onFrameComplete;
            }
        }
    }
}
