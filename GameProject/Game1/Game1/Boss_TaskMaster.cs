﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Boss_TaskMaster : Boss
    {
        public Boss_TaskMaster() : base("Task Master") {
            AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Taskmaster/Stance"));
            SetFrameDelay(Animation.State.STANCE, 7);

            AddSprite(Animation.State.WALK_TOWARDS, new Sprite("Sprites/Actors/Taskmaster/Walk Towards", Animation.Type.REPEAT));
            SetFrameDelay(Animation.State.WALK_TOWARDS, 7);
            SetSpriteOffSet(Animation.State.WALK_TOWARDS, 5, -20);

            AddSprite(Animation.State.WALK_BACKWARDS, new Sprite("Sprites/Actors/Taskmaster/Walk Backwards", Animation.Type.REPEAT));
            SetFrameDelay(Animation.State.WALK_BACKWARDS, 7);
            SetSpriteOffSet(Animation.State.WALK_BACKWARDS, 5, -20);

            AddSprite(Animation.State.ATTACK1, new Sprite("Sprites/Actors/Taskmaster/Attack1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK1, 5);
            SetSpriteOffSet(Animation.State.ATTACK1, -5, -28);

            AddSprite(Animation.State.ATTACK2, new Sprite("Sprites/Actors/Taskmaster/Attack2", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK2, 5);
            SetSpriteOffSet(Animation.State.ATTACK2, 75, -116);

            AddSprite(Animation.State.ATTACK3, new Sprite("Sprites/Actors/Taskmaster/Attack3", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK3, 5);
            SetSpriteOffSet(Animation.State.ATTACK3, 8, -121);

            AddSprite(Animation.State.ATTACK4, new Sprite("Sprites/Actors/Taskmaster/Attack4", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK4, 5);
            SetSpriteOffSet(Animation.State.ATTACK4, 25, -88);

            SetAnimationState(Animation.State.STANCE);
            AddBox(new CLNS.BoundingBox(CLNS.BoxType.BOUNDS_BOX, 120, 283, -60, -20));

            SetScale(1.9f, 2.7f);
            SetPostion(650, 0, 340);
            SetBaseOffset(-90, -25f);
        }
    }
}
