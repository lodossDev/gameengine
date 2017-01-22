using Microsoft.Xna.Framework;
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
            public long hitByAttackId;
            public Animation.State lastAttackState;
            public int lastAttackFrame;
            public float hitPauseTime;


            public AttackInfo()
            {
                Reset();
            }

            public void Reset()
            {
                lastAttackFrame = -1;
                lastAttackState = Animation.State.NONE;
                hitPauseTime = 0;
            }
        }

        public class FrameInfo
        {
            public enum FrameState { NO_FRAME = -1 }

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

            public bool IsInFrame(int currentFrame)
            {
                return (currentFrame >= startFrame && currentFrame <= endFrame);
            }
        }

        public class TossInfo {
            public Vector3 velocity;
            public Vector3 maxVelocity;
            public bool isToss;
            public bool inTossFrame;
            public float gravity;
            public float height;
            public int maxHitGround;
            public int hitGoundCount;
            public int tossCount;
            public int maxTossCount;


            public TossInfo() {
                height = 0f;
                velocity = Vector3.Zero;
                maxVelocity = new Vector3(10f, 13f, 10f);
                gravity = 0.48f;
                inTossFrame = false;
                isToss = false;
                hitGoundCount = 0;
                tossCount = 0;
                maxHitGround = 3;
                maxTossCount = 1;
            }
        }

        public class ColourInfo
        {
            public float alpha;
            public float fadeFrequency;
            public float r, g, b;
            public float currentFadeTime;
            public float maxFadeTime;
            public bool isFlash;
            public bool expired;
            public float originalFreq;
            
            public ColourInfo()
            {
                r = 255;
                g = 255;
                b = 255;
                alpha = 255;
                fadeFrequency = 3f;
                originalFreq = 3f;
                currentFadeTime = 0f;
                maxFadeTime = 100f;
                isFlash = false;
                expired = false;
            }
            
            public Color GetColor()
            {
                return new Color((byte)r, (byte)g, (byte)b, (byte)MathHelper.Clamp(alpha, 0, 255));
            } 
        }
    }
}
