﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1 {
    public class Animation {
        /**
        * 1 game-tick is 1/60 of a sec, so 60 ticks is 1 second
        */
        public const float TICK_RATE = (1.0f / 60.0f);
        public const float DEFAULT_TICKS = TICK_RATE * 3.0f;

        public enum State {
            NONE = -1, STANCE, WALK_TOWARDS, WALK_BACKWARDS, RUN,

            JUMP_START, JUMP, JUMP_TOWARDS,

            LAND1, LAND2, LAND3, LAND4, LAND5,

            ATTACK1, ATTACK2, ATTACK3, ATTACK4, ATTACK5, ATTACK6,
            ATTACK7, ATTACK8, ATTACK9, ATTACK10,

            SPECIAL1, SPECIAL2, SPECIAL3, SPECIAL4, SPECIAL6,
            SPECIAL8, SPECIAL9, SPECIAL10,

            JUMP_ATTACK1, JUMP_ATTACK2, JUMP_ATTACK3, JUMP_ATTACK4, JUMP_ATTACK5, JUMP_ATTACK6,
            JUMP_ATTACK7, JUMP_ATTACK8, JUMP_ATTACK9, JUMP_ATTACK10,

            JUMP_TOWARD_ATTACK1, JUMP_TOWARD_ATTACK2, JUMP_TOWARD_ATTACK3, JUMP_TOWARD_ATTACK4,
            JUMP_TOWARD_ATTACK5, JUMP_TOWARD_ATTACK6, JUMP_TOWARD_ATTACK7, JUMP_TOWARD_ATTACK8,
            JUMP_TOWARD_ATTACK9, JUMP_TOWARD_ATTACK10,

            JUMP_RECOVER1, JUMP_RECOVER2, JUMP_RECOVER3, JUMP_RECOVER4, JUMP_RECOVER5,

            FALL1, FALL2, FALL3, FALL4, FALL5,
             
            PAIN1, PAIN2, PAIN3, PAIN4, PAIN5, PAIN6, PAIN7, PAIN8, PAIN9, PAIN10,

            RECOVER1, RECOVER2, RECOVER3, RECOVER4, RECOVER5, RECOVER6, RECOVER7, RECOVER8,
            RECOVER9, RECOVER10,

            FALLING1, FALLING2, FALLING3, FALLING4, FAILLING5,

            KNOCK_DOWN1, KNOCK_DOWN2, KNOCK_DOWN3, KNOCK_DOWN4, KNOCK_DOWN5,

            DIE1, DIE2, DIE3, DIE4, DIE5, DIE6,

            GRAB_HOLD1, GRAB_HOLD2, GRAB_HOLD3, GRAB_HOLD4, GRAB_HOLD5,

            GRAB_ATTACK1, GRAB_ATTACK2, GRAB_ATTACK3, GRAB_ATTACK4, GRAB_ATTACK5,

            THROW1, THROW2, THROW3, THROW4, THROW5,

            DIZZY1, DIZZY2, DIZZY3, DIZZY4, DIZZY5,

            TAUNT1, TAUNT2, TAUNT3, TAUNT4, TAUNT5
        }

        public enum Action {NONE, ATTACKING, JUMPING, FALLING, IDLE, WALKING, LANDING, RECOVERY, RUNNING}

        public enum Type {NONE, ONCE, REPEAT}

        public class Link {
            private Animation.State onState;
            private Animation.State toState;
            private int onFrameState;
            private bool onFrameComplete;

            public Link(Animation.State onState, Animation.State toState, int frameOnStart, bool onFrameComplete = true) {
                SetLink(onState, toState, frameOnStart, onFrameComplete);
            }

            public void SetLink(Animation.State onState, Animation.State toState, int frameOnStart, bool onFrameComplete = true) {
                this.onState = onState;
                this.toState = toState;
                this.onFrameState = (frameOnStart - 1);
                this.onFrameComplete = onFrameComplete;
            }

            public Animation.State GetOnState() {
                return onState;
            }

            public Animation.State GetToState() {
                return toState;
            }

            public int GetOnFrameStart() {
                return onFrameState;
            }

            public bool OnFrameComplete() {
                return onFrameComplete;
            }
        }
    }
}
