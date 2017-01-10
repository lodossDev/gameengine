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
            SetFrameDelay(Animation.State.ATTACK1, 4);
            SetSpriteOffSet(Animation.State.ATTACK1, -10, -27);

            AddSprite(Animation.State.ATTACK2, new Sprite("Sprites/Actors/Taskmaster/Attack2", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK2, 4);
            SetSpriteOffSet(Animation.State.ATTACK2, 75, -116);

            AddSprite(Animation.State.ATTACK3, new Sprite("Sprites/Actors/Taskmaster/Attack3", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK3, 4);
            SetSpriteOffSet(Animation.State.ATTACK3, 8, -121);

            AddSprite(Animation.State.ATTACK4, new Sprite("Sprites/Actors/Taskmaster/Attack4", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK4, 4);
            SetSpriteOffSet(Animation.State.ATTACK4, 25, -88);

            SetAnimationState(Animation.State.STANCE);

            AddBox(Animation.State.STANCE, 1, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.STANCE, 2, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.STANCE, 3, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.STANCE, 4, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.STANCE, 5, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.STANCE, 6, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.STANCE, 7, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));

            AddBox(Animation.State.STANCE, 8, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.STANCE, 9, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.STANCE, 10, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));

            AddBox(Animation.State.WALK_BACKWARDS, 1, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_BACKWARDS, 2, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_BACKWARDS, 3, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_BACKWARDS, 4, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_BACKWARDS, 5, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_BACKWARDS, 6, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_BACKWARDS, 7, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));

            AddBox(Animation.State.WALK_TOWARDS, 1, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_TOWARDS, 2, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_TOWARDS, 3, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_TOWARDS, 4, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_TOWARDS, 5, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_TOWARDS, 6, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));
            AddBox(Animation.State.WALK_TOWARDS, 7, new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 600, 200, 30, 20));

            AddBoundsBox(120, 283, -60, 0, 50);

            SetScale(2.2f, 2.8f);
            SetPostion(650, 0, 340);
            SetBaseOffset(-90, -25f);
        }
    }
}
