﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace Game1
{
    public class Entity : IComparable<Entity>
    {
        private static int id = 0;
        public enum EntityType {PLAYER, ENEMY, OBSTACLE, PLATFORM, ITEM, WEAPON, LEVEL, LIFE_BAR, OTHER, HIT_FLASH}

        private Dictionary<Animation.State, Sprite> spriteMap;
        private Sprite currentSprite;
        private Attributes.ColourInfo colorInfo; 
        private Animation.State currentAnimationState;
        private Animation.State lastAnimationState;

        private Dictionary<Animation.State, SoundEffect> animationSounds;
        private Dictionary<Animation.State, int> moveFrames;
        private Dictionary<Animation.State, int> tossFrames;
        private List<CLNS.BoundingBox> boxes;
        private List<Animation.Link> animationLinks;
        private ComboAttack.Chain defaultAttackChain;
        private List<InputHelper.CommandMove> commandMoves;

        private string name;
        private EntityType type;
        private Vector3 position;
        private Vector3 velocity;
        private Vector3 absoluteDir;
        private Vector3 direction;
        private Vector2 convertedPosition;
        private Vector2 origin;
        private Vector2 scale;
        private Vector2 nScale;
        private Vector2 stanceOrigin;

        private int width;
        private int height;
        private float depth;
        private float depthOffSet;

        private float ground;
        private float groundBase;

        private Sprite baseSprite;
        private Vector2 baseCenter;
        private Vector2 baseOffset;
        private Vector2 basePosition;

        private Attributes.CollisionInfo collisionInfo;
        private Attributes.AttackInfo attackInfo;
        private Attributes.TossInfo tossInfo;

        private System.StateMachine aiStateMachine;
        private Entity currentTarget;

        private int entityId;
        private int health;
        private bool alive;


        public Entity(EntityType type, string name)
        {
            this.type = type;
            this.name = name;

            spriteMap = new Dictionary<Animation.State, Sprite>();
            moveFrames = new Dictionary<Animation.State, int>();
            tossFrames = new Dictionary<Animation.State, int>();
            animationLinks = new List<Animation.Link>();
            animationSounds = new Dictionary<Animation.State, SoundEffect>();

            boxes = new List<CLNS.BoundingBox>();
            scale = nScale = new Vector2(1f, 1f);
            stanceOrigin = Vector2.Zero;

            currentAnimationState = Animation.State.NONE;

            colorInfo = new Attributes.ColourInfo();
            
            position = Vector3.Zero;
            convertedPosition = Vector2.Zero;

            direction = Vector3.Zero;
            direction.X = 1;
            absoluteDir = direction;

            origin = Vector2.Zero;
            velocity = Vector3.Zero;

            width = 0;
            height = 0;
            depth = 30f;
            depthOffSet = 0f;

            ground = groundBase = 0;
            baseSprite = new Sprite("Sprites/Misc/Marker");
            baseCenter = new Vector2(0f, 0f);
            baseOffset = new Vector2(0f, 0f);
            basePosition = Vector2.Zero;

            collisionInfo = new Attributes.CollisionInfo();
            attackInfo = new Attributes.AttackInfo();
            tossInfo = new Attributes.TossInfo();

            aiStateMachine = new System.StateMachine();
            commandMoves = new List<InputHelper.CommandMove>();

            id++;
            entityId = id;
            alive = true;
            health = 100;
        }

        public void AddSprite(Animation.State state, Sprite sprite)
        {
            spriteMap.Add(state, sprite);
        }

        public void AddSprite(Animation.State state, Sprite sprite, bool setAsDefaultState)
        {
            AddSprite(state, sprite);

            if (setAsDefaultState)
            {
                SetAnimationState(state);
            }
        }

        public void AddSprite(Animation.State state, String location, bool setAsDefaultState)
        {
            AddSprite(state, new Sprite(location), setAsDefaultState);
        }

        public void AddAnimationLink(Animation.Link link)
        {
            animationLinks.Add(link);
        }

        public void SetAnimationLink(Animation.State onState, Animation.State toState, int frameOnStart, bool onFrameComplete = true)
        {
            Animation.Link link = animationLinks.Find(item => item.GetOnState() == onState);
            link.SetLink(onState, toState, frameOnStart, onFrameComplete);
        }

        public void SetJumpLink(Animation.State toState)
        {
            Sprite jumpStart = GetSprite(Animation.State.JUMP_START);

            if (jumpStart != null)
            {
                int frames = jumpStart.GetFrames();
                SetAnimationLink(Animation.State.JUMP_START, toState, frames);
            }
        }

        public void SetAnimationState(Animation.State state)
        {
            if (!IsInAnimationState(state))
            {
                attackInfo.lastAttackFrame = -1;
                attackInfo.lastAttackState = Animation.State.NONE;
                Sprite newSprite = GetSprite(state);

                if (newSprite != null)
                {
                    lastAnimationState = currentAnimationState;
                    currentAnimationState = state;
                    currentSprite = newSprite;
                }

                foreach (Sprite sprite in spriteMap.Values)
                {
                    sprite.ResetAnimation();
                }
            }
        }

        public void AddBox(CLNS.BoundingBox box)
        {
            boxes.Add(box);
        }

        public void AddBox(Animation.State state, int frame, CLNS.BoundingBox box)
        {
            GetSprite(state).AddBox(frame, box);

            if (box.GetBoxType() == CLNS.BoxType.BOUNDS_BOX)
            {
                CLNS.BoundsBox bbox = (CLNS.BoundsBox)box;
                double y1 = ((int)(bbox.GetHeight() - (int)bbox.GetRect().Height) / (int)bbox.GetRect().Height) + 1;
                double y2 = ((int)bbox.GetOffset().Y * y1) / 2;

                AddBox(state, frame, new CLNS.BoundingBox(CLNS.BoxType.HEIGHT_BOX, bbox.GetRect().Width, (int)bbox.GetHeight(), (int)bbox.GetOffset().X, (int)bbox.GetOffset().Y + (int)y2));
            }
        }

        public void AddBoundsBox(CLNS.BoundsBox box)
        {
            AddBox(box);

            double y1 = (box.GetRect().Height - box.GetHeight());
            //AddBox(new CLNS.BoundingBox(CLNS.BoxType.HEIGHT_BOX, box.GetRect().Width, (int)box.GetHeight(), (int)box.GetOffset().X, (int)(box.GetOffset().Y + y1)));
        }

        public void AddAnimationSound(Animation.State state, String location)
        {
            animationSounds.Add(state, Setup.contentManager.Load<SoundEffect>(location));
        }

        public SoundEffect GetAnimationSound(Animation.State state)
        {
            return animationSounds[state];
        }

        public void AddCommandMove(InputHelper.CommandMove commandMove)
        {
            commandMoves.Add(commandMove);
        }

        public List<InputHelper.CommandMove> GetCommandMoves()
        {
            return commandMoves;
        }

        public CLNS.BoundingBox GetLastBoxFrame(Animation.State state, int frame)
        {
            return GetSprite(state).GetBoxes(frame).Last();
        }

        public CLNS.AttackBox GetAttackBox(Animation.State state, int frame)
        {
            return (CLNS.AttackBox)GetSprite(state).GetBoxes(frame).Last();
        }

        public CLNS.AttackBox GetAttackBox(Animation.State state, int frame, int index)
        {
            return (CLNS.AttackBox)GetSprite(state).GetBoxes(frame)[index];
        }

        public void SetOffset(Animation.State state, int frame, float x, float y)
        {
            GetSprite(state).SetFrameOffset(frame, x, y);
        }

        public void SetOffset(Animation.State state, float x, float y)
        {   
            GetSprite(state).SetFrameOffset(x, y);    
        }

        public void SetSpriteOffSet(Animation.State state, float x, float y)
        {
            GetSprite(state).SetSpriteOffset(x, y);
        }

        public void SetFrameDelay(Animation.State state, int frame, int ticks)
        {
            GetSprite(state).SetFrameTime(frame, ticks);
        }

        public void SetFrameDelay(Animation.State state, int ticks)
        {
            GetSprite(state).SetFrameTime(ticks);
        }

        public void SetResetFrame(Animation.State state, int frame)
        {
            GetSprite(state).SetResetFrame(frame);
        }

        public void SetMoveFrame(Animation.State state, int frame)
        {
            moveFrames.Add(state, frame - 1);
        }

        public void SetTossFrame(Animation.State state, int frame)
        {
            tossFrames.Add(state, frame - 1);
        }

        public void SetDefaultAttackChain(ComboAttack.Chain attackChain)
        {
            defaultAttackChain = attackChain;
        }

        public void SetPostion(float x, float y, float z)
        {
            position.X = x;
            position.Y = y;
            position.Z = z;
        }

        public void SetPosX(float x)
        {
            position.X = x;
        }

        public void SetPosY(float y)
        {
            position.Y = y;
        }

        public void SetPosZ(float z)
        {
            position.Z = z;
        }

        public void SetCurrentTarget(Entity target)
        {
            currentTarget = target;
        }

        public void MoveX(float velX)
        {
            if (velX != 0.0)
            {
                absoluteDir.X = (velX < 0 ? -1 : 1);
            }

            if (IsInMoveFrame() && collisionInfo.collide_x == Attributes.CollisionState.NO_COLLISION)
            {
                position.X += velX;
            }
        }

        public void MoveY(float velY)
        {
            if (velY != 0.0)
            {
                absoluteDir.Y = (velY < 0 ? -1 : 1);
            }

            if (collisionInfo.collide_y == Attributes.CollisionState.NO_COLLISION)
            {
                position.Y += velY;
            }
        }

        public void MoveZ(float velZ)
        {
            if (velZ != 0.0)
            {
                absoluteDir.Z = (velZ < 0 ? -1 : 1);
            }

            if (IsInMoveFrame() && collisionInfo.collide_z == Attributes.CollisionState.NO_COLLISION)
            {
                position.Z += velZ;
            }
        }

        public void VelX(float velX)
        {
            velocity.X = velX;

            if (velX != 0.0)
            {
                direction.X = (velX < 0 ? -1 : 1);
            }
        }

        public void VelY(float velY)
        {
            velocity.Y = velY;

            if (velY != 0.0)
            {
                direction.Y = (velY < 0 ? -1 : 1);
            }
        }

        public void VelZ(float velZ)
        {
            velocity.Z = velZ;

            if (velZ != 0.0)
            {
                direction.Z = (velZ < 0 ? -1 : 1);
            }
        }

        public void SetScale(float x=1f, float y=1f)
        {
            scale.X = x;
            scale.Y = y;
        }

        public void SetScaleX(float x)
        {
            scale.X = x;
        }

        public void SetScaleY(float y)
        {
            scale.Y = y;
        }

        public void SetHealth(int health)
        {
            this.health = health;
        }

        public void SetAlive(bool alive)
        {
            this.alive = alive;
        }

        public void SetIsLeft(bool isLeft)
        {
            foreach (Sprite sprite in spriteMap.Values)
            {
                sprite.SetIsLeft(isLeft);
            }
        }

        public void SetWidth(int width)
        {
            this.width = width;
        }

        public void SetHeight(int height)
        {
            this.height = height;
        }

        public void SetDimension(int width, int height, int depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public void SetDimension(int width, int height)
        {
            SetDimension(width, height, (int)this.depth);
        }

        public void SetDepth(float depth)
        {
            this.depth = depth;
        }

        public void SetDepthOffset(float z)
        {
            depthOffSet = z;
        }

        public void SetGround(float ground)
        {
            this.ground = ground;
        }

        public void SetGroundBase(float groundBase)
        {
            this.groundBase = groundBase;
        }
        
        public void SetBaseOffset(float x, float y)
        {
            baseOffset.X = x;
            baseOffset.Y = y;
        }

        public string GetName()
        {
            return name;
        }

        public int GetHealth()
        {
            return health;
        }

        public bool Alive()
        {
            return alive;
        }

        public bool IsLeft()
        {
            return currentSprite.IsLeft();
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public float GetPosX()
        {
            return position.X;
        }

        public float GetPosY()
        {
            return position.Y;
        }

        public float GetPosZ()
        {
            return position.Z;
        }

        public float GetDepthOffset()
        {
            return depthOffSet;
        }

        public Vector2 GetScale()
        {
            return scale;
        }

        public int GetDirX()
        {
            return (int)direction.X;
        }

        public int GetDirY()
        {
            return (int)direction.Y;
        }

        public int GetDirZ()
        {
            return (int)direction.Z;
        }

        public int GetAbsoluteDirX()
        {
            return (int)absoluteDir.X;
        }

        public int GetAbsoluteDirY()
        {
            return (int)absoluteDir.Y;
        }

        public int GetAbsoluteDirZ()
        {
            return (int)absoluteDir.Z;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return Math.Abs(height);
        }

        public float GetDepth()
        {
            return depth;
        }

        public float GetGround()
        {
            return ground;
        }

        public float GetGroundBase()
        {
            return groundBase;
        }

        public EntityType GetEntityType()
        {
            return type;
        }

        public bool IsEntity(EntityType type)
        {
            return (this.type == type);
        }

        public Vector2 GetConvertedPosition()
        {
            convertedPosition.X = position.X;
            convertedPosition.Y = position.Y + position.Z;
            return convertedPosition;
        }

        public virtual Vector2 GetOrigin()
        {
            Sprite sprite = GetCurrentSprite();
            origin.X = (sprite.GetCurrentTexture().Width / 2);
            origin.Y = 0;

            return origin;
        }

        public Vector2 GetStanceOrigin()
        {
            Sprite sprite = GetSprite(Animation.State.STANCE);
            stanceOrigin.X = sprite.GetCurrentTexture().Width / 2;
            stanceOrigin.Y = 0;

            return stanceOrigin;
        }

        public Vector3 GetVelocity()
        {
            return velocity;
        }

        public int GetSpriteCount()
        {
            return spriteMap.Count;
        }

        public Sprite GetSprite(Animation.State state)
        {
            if (spriteMap.ContainsKey(state))
            {
                return spriteMap[state];
            }
            else
            {
                return null;
            }
        }

        public int GetSpriteFrames(Animation.State state)
        {
            return GetSprite(state).GetFrames();
        }

        public Sprite GetCurrentSprite()
        {
            return currentSprite;
        }

        public int GetCurrentFrame()
        {
            return GetCurrentSprite().GetCurrentFrame();
        }

        public Entity GetCurrentTarget()
        {
            return currentTarget;
        }

        public float GetSpriteWidth(Animation.State state)
        {
            return GetSprite(state).GetCurrentTexture().Width * GetScale().X;
        }

        public float GetSpriteHeight(Animation.State state)
        {
            return GetSprite(state).GetCurrentTexture().Height * GetScale().Y;
        }

        public float GetCurrentSpriteWidth()
        {
            return GetCurrentSprite().GetCurrentTexture().Width * GetScale().X;
        }

        public float GetCurrentSpriteHeight()
        {
            return GetCurrentSprite().GetCurrentTexture().Height * GetScale().Y;
        }

        public int GetCurrentSpriteFrame()
        {
            return GetCurrentSprite().GetCurrentFrame();
        }

        public Sprite GetBaseSprite()
        {
            return baseSprite;
        }

        public Color GetSpriteColor()
        {
            return colorInfo.GetColor();
        }

        public Vector2 GetBasePosition()
        {
            Sprite stance = GetSprite(Animation.State.STANCE);
            baseCenter.X = (baseOffset.X * scale.X) + ((stance.GetCurrentTexture().Width * scale.X) / 2);
            baseCenter.Y = (baseOffset.Y * scale.Y) + ((stance.GetCurrentTexture().Height * scale.Y));

            if (IsLeft())
            {
                basePosition.X = GetConvertedPosition().X - baseCenter.X - 3;
            }
            else
            {
                basePosition.X = GetConvertedPosition().X + baseCenter.X + 8;
            }

            basePosition.Y = GetConvertedPosition().Y + (stance.GetSpriteOffSet().Y * scale.Y) + (stance.GetCurrentFrameOffSet().Y * scale.Y) + baseCenter.Y;
            return basePosition;
        }

        public List<CLNS.BoundingBox> GetAllFrameBoxes()
        {
            List<CLNS.BoundingBox> currentBoxes = new List<CLNS.BoundingBox>();

            foreach (Sprite sprite in spriteMap.Values)
            {
                currentBoxes.AddRange(sprite.GetAllBoxes());
            }

            return currentBoxes;
        }

        public List<CLNS.BoundingBox> GetBoxes()
        {
            return boxes;
        }

        public List<CLNS.BoundingBox> GetBoxes(CLNS.BoxType type)
        {
            return boxes.FindAll(box => box.GetBoxType() == type);
        }

        public List<CLNS.BoundingBox> GetCurrentBoxes()
        {
            return GetCurrentSprite().GetCurrentBoxes();
        }

        public List<CLNS.BoundingBox> GetCurrentBoxes(CLNS.BoxType type)
        {
            return GetCurrentSprite().GetCurrentBoxes(type);
        }

        public SpriteEffects GetEffects()
        {
            return currentSprite.GetEffects();
        }

        public System.StateMachine GetAiStateMachine()
        {
            return aiStateMachine;
        }

        public bool IsInAnimationState(Animation.State state)
        {
            return (currentSprite != null 
                        && spriteMap.ContainsKey(state) 
                        && currentSprite == GetSprite(state)
                        && this.currentAnimationState == state);
        }

        public Animation.State GetCurrentAnimationState()
        {
            return currentAnimationState;
        }

        public Animation.State GetLastAnimationState()
        {
            return lastAnimationState;
        }

        public bool IsInAnimationAction(Animation.Action animationAction)
        {
            Animation.Action currentAction = GetCurrentAnimationAction();
            return (currentAction == animationAction);
        }

        public Animation.Action GetCurrentAnimationAction()
        {
            Animation.Action currentAction = Animation.Action.NONE;
            Animation.State currentState = GetCurrentAnimationState();

            if (currentState.ToString().Contains("ATTACK"))
            {
                return Animation.Action.ATTACKING;
            }
            else
            {
                if (currentState.ToString().Contains("RECOVER"))
                {
                    return Animation.Action.RECOVERY;
                }
                else if (currentState.ToString().Contains("JUMP"))
                {
                    return Animation.Action.JUMPING;
                }
                else if (currentState.ToString().Contains("FALL"))
                {
                    return Animation.Action.FALLING;
                }
            }

            switch (currentState)
            {
                case Animation.State.NONE:
                    currentAction = Animation.Action.NONE;
                    break;
                case Animation.State.STANCE:
                    currentAction = Animation.Action.IDLE;
                    break;
                case Animation.State.WALK_TOWARDS:
                case Animation.State.WALK_BACKWARDS:
                    currentAction = Animation.Action.WALKING;
                    break;
                case Animation.State.LAND:
                    currentAction = Animation.Action.LANDING;
                    break;
                default:
                    currentAction = Animation.Action.NONE;
                    break;
            }

            return currentAction;
        }

        public bool IsInMoveFrame()
        {
            return ((moveFrames.ContainsKey(GetCurrentAnimationState())
                        && IsInAnimationState(GetCurrentAnimationState())
                        && currentSprite.GetCurrentFrame() >= moveFrames[GetCurrentAnimationState()]) 
                    || !moveFrames.ContainsKey(GetCurrentAnimationState()));
        }
        
        public int GetMoveFrame()
        {
            return (moveFrames.ContainsKey(GetCurrentAnimationState()) ? moveFrames[GetCurrentAnimationState()] : 0);
        }

        public bool IsInTossFrame()
        {
            return ((tossFrames.ContainsKey(GetCurrentAnimationState())
                        && IsInAnimationState(GetCurrentAnimationState())
                        && currentSprite.GetCurrentFrame() >= tossFrames[GetCurrentAnimationState()])
                    || tossFrames.Count == 0);
        }

        public int GetTossFrame()
        {
            return (tossFrames.ContainsKey(GetCurrentAnimationState()) ? tossFrames[GetCurrentAnimationState()] : 0);
        }

        public bool IsFrameComplete(Animation.State state, int frame)
        {
            Sprite sprite = GetSprite(state);
            return sprite.IsFrameComplete(frame);
        }

        public int GetEntityId()
        {
            return entityId;
        }

        public bool IsToss()
        {
            return tossInfo.isToss;
        }

        public bool HasLanded()
        {
            return GetPosY() >= GetGround();
        }

        public bool IsOnGround()
        {
            return GetPosY() == GetGroundBase();
        }

        public bool InAir()
        {
            return GetPosY() < GetGround();
        }

        public Attributes.TossInfo GetTossInfo()
        {
            return tossInfo;
        }

        public ComboAttack.Chain GetDefaultAttackChain()
        {
            return defaultAttackChain;
        }

        public Animation.State GetCurrentAttackChainState()
        {
            return defaultAttackChain.GetCurrentAttackState();
        }

        public Animation.State GetPreviousAttackChainState()
        {
            return defaultAttackChain.GetPreviousAttackState();
        }

        public bool InCurrentAttackCancelState()
        {
            List<ComboAttack.Move> attackStates = defaultAttackChain.GetMoves().FindAll(item => item.GetState().Equals(GetCurrentAnimationState()));

            return IsInAnimationAction(Animation.Action.ATTACKING)
                        && attackStates.Count > 0
                        && IsInAnimationState(attackStates[0].GetState())
                        && GetCurrentSprite().GetCurrentFrame() >= attackStates[0].GetCancelFrame()
                        && IsFrameComplete(attackStates[0].GetState(), attackStates[0].GetCancelFrame() + 1);
        }

        public void AttackChainStep()
        {
            if (!IsInAnimationAction(Animation.Action.ATTACKING) 
                    || InCurrentAttackCancelState())
            {
                SetAnimationState(GetCurrentAttackChainState());
            }

            if (InCurrentAttackCancelState())
            {
                GetAttackInfo().Reset();
                GetCurrentSprite().ResetAnimation();
            }
        }

        public bool InAttackFrame()
        {
            List<CLNS.BoundingBox> attackBoxes = GetCurrentSprite().GetCurrentBoxes(CLNS.BoxType.HIT_BOX);
            return IsInAnimationAction(Animation.Action.ATTACKING) && attackBoxes != null && attackBoxes.Count > 0;
        }

        public Attributes.CollisionInfo GetCollisionInfo()
        {
            return collisionInfo;
        }

        public Attributes.AttackInfo GetAttackInfo()
        {
            return attackInfo;
        }

        public bool IsJumpingOrInAir()
        {
            return (IsToss() || IsInAnimationAction(Animation.Action.JUMPING));
        }

        public void SetJump(float height = -25f, float velX = 0f) 
        {
            Toss(height, velX);
            Sprite jumpStart = GetSprite(Animation.State.JUMP_START);

            if (jumpStart != null)
            {
                SetAnimationState(Animation.State.JUMP_START);
            }
            else
            {
                SetAnimationState(Animation.State.JUMP);
            }

            if (velX < 0.0 || velX > 0.0)
            {
                SetJumpLink(Animation.State.JUMP_TOWARDS);
            }
            else
            {
                SetJumpLink(Animation.State.JUMP);
            }
        }

        public void Toss(float height = -20, float velX = 0f)
        {
            tossInfo.height = height;
            tossInfo.velocity.Y = (height / 2);
            tossInfo.velocity.X = velX;
            tossInfo.inTossFrame = false;
            tossInfo.isToss = true;
        }

        public void ResetToss()
        {
            velocity.Y = 0f;
            velocity.X = 0f;
            tossInfo.velocity.X = 0f;
            tossInfo.velocity.Y = 0f;
            tossInfo.inTossFrame = false;
            tossInfo.isToss = false;
        }

        public void UpdateToss(GameTime gameTime)
        {
            bool alwaysToss = IsInAnimationAction(Animation.Action.FALLING);

            if (tossInfo.isToss || alwaysToss)
            {
                if (IsInTossFrame() || alwaysToss)
                {
                    if (!tossInfo.inTossFrame)
                    {
                        MoveY(tossInfo.height);
                        tossInfo.inTossFrame = true;
                    }
                }

                if (tossInfo.inTossFrame)
                {
                    tossInfo.velocity.Y += tossInfo.gravity;
                    
                    if (tossInfo.velocity.Y >= tossInfo.maxVelocity.Y)
                    {
                        tossInfo.velocity.Y = tossInfo.maxVelocity.Y;
                    }

                    VelX(tossInfo.velocity.X);
                    VelY(tossInfo.velocity.Y);
                }

                if (GetPosY() > GetGround())
                {
                    SetPosY(GetGround());
                    SetAnimationState(Animation.State.LAND);
                    ResetToss();
                }
            }
        }

        public void UpdateAnimationLinks(GameTime gameTime)
        {
            List<Animation.Link> links = animationLinks.FindAll(item => item.GetOnState() == GetCurrentAnimationState());

            foreach (Animation.Link link in links)
            {
                if (link.OnFrameComplete() 
                        && currentSprite.GetCurrentFrame() >= link.GetOnFrameStart())
                {
                    if (IsFrameComplete(link.GetOnState(), link.GetOnFrameStart() + 1))
                    {
                        SetAnimationState(link.GetToState());
                    }
                }
                else
                {
                    if (link.GetOnFrameStart() == currentSprite.GetCurrentFrame())
                    {
                        SetAnimationState(link.GetToState());
                    }
                }
            }
        }

        public bool IsNonActionState()
        {
            return (!IsToss() && !IsInAnimationAction(Animation.Action.ATTACKING));
        }

        public bool InResetState()
        {
            return (!InAir()
                        &&  (IsInAnimationAction(Animation.Action.WALKING)
                                || IsInAnimationAction(Animation.Action.JUMPING)
                                        && GetCurrentSprite().IsAnimationComplete()
                                || IsInAnimationAction(Animation.Action.LANDING)
                                        && GetCurrentSprite().IsAnimationComplete())
                                || IsInAnimationAction(Animation.Action.ATTACKING)
                                        && GetCurrentSprite().IsAnimationComplete());
        }

        public virtual void ResetToIdle(GameTime gameTime)
        {
            if (InResetState())
            {
                int frame = (IsEntity(EntityType.PLAYER) ? GetCurrentSprite().GetCurrentFrame() : GetCurrentSprite().GetFrames());

                bool isFrameComplete = (IsEntity(EntityType.PLAYER) ? IsFrameComplete(GetCurrentAnimationState(), frame) 
                                            : IsFrameComplete(GetCurrentAnimationState(), frame) && !IsInAnimationAction(Animation.Action.WALKING));

                if (isFrameComplete)
                {
                    SetAnimationState(Animation.State.STANCE);
                }
            }
        }

        public void UpdateAnimation(GameTime gameTime)
        {
            currentSprite.UpdateAnimation(gameTime);
            UpdateAnimationLinks(gameTime);
        }

        public void UpdateDefaultAttackChain(GameTime gameTime)
        {
            if (defaultAttackChain != null)
            {
                defaultAttackChain.UpdateCombo(gameTime);
            }
        }

        public void SetFade(int alpha)
        {
            colorInfo.alpha = (float)alpha;
        }

        public void SetColor(int r, int g, int b)
        {
            colorInfo.r = r;
            colorInfo.g = g;
            colorInfo.b = b;
        }

        public void Flash(float time = 5, float speed = 80f)
        {
            colorInfo.isFlash = true;
            colorInfo.expired = false;
            colorInfo.alpha = 255;
            colorInfo.fadeFrequency = speed;
            colorInfo.originalFreq = speed;
            colorInfo.currentFadeTime = 0f;
            colorInfo.maxFadeTime = time;
        }

        public void UpdateFade(GameTime gameTime)
        {
            if (colorInfo.isFlash)
            {
                if (!colorInfo.expired)
                {
                    colorInfo.currentFadeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (colorInfo.alpha >= 255 || colorInfo.alpha <= 0)
                    {
                        colorInfo.fadeFrequency *= -1;
                    }

                    colorInfo.alpha += colorInfo.fadeFrequency;
                }

                if (colorInfo.currentFadeTime > colorInfo.maxFadeTime)
                {
                    colorInfo.expired = true;
                }
                
                if (colorInfo.expired)
                {
                    colorInfo.currentFadeTime = 0f;

                    if (colorInfo.alpha != 255)
                    {
                        float freq = colorInfo.originalFreq * 1f;

                        colorInfo.fadeFrequency = 1 * Math.Abs(freq);
                        colorInfo.alpha += colorInfo.fadeFrequency;
                    }

                    if (colorInfo.alpha >= 255)
                    {
                        colorInfo.alpha = 255;
                        colorInfo.isFlash = false;
                        colorInfo.expired = false;
                    }
                }
            }
        }

        public bool IsPauseHit(GameTime gameTime)
        {
            bool isPauseHit = false;

            if (attackInfo.hitPauseTime > 0)
            {
                attackInfo.hitPauseTime -= (5 * (float)gameTime.ElapsedGameTime.Milliseconds);
                isPauseHit = true;
            }

            if (attackInfo.hitPauseTime < 0)
            {
                attackInfo.hitPauseTime = 0;
                isPauseHit = false;
            }

            return isPauseHit;
        }

        public void Update(GameTime gameTime)
        {
            bool isPauseHit = IsPauseHit(gameTime);
            Vector2 drawScale = scale;
            
            if (IsEntity(EntityType.LIFE_BAR))
            {
                drawScale = nScale;
            }

            foreach (Sprite sprite in spriteMap.Values)
            {
                sprite.Update(gameTime, position, drawScale);
            }

            UpdateFade(gameTime);

            //Update animation.
            if (!isPauseHit)
            {
                UpdateAnimation(gameTime);
            }

            UpdateDefaultAttackChain(gameTime);

            //Update physics.
            UpdateToss(gameTime);

            //Update bounding boxes.
            foreach (CLNS.BoundingBox box in GetAllFrameBoxes())
            {
                box.Update(gameTime, IsLeft(), GetConvertedPosition());
            }
            
            foreach (CLNS.BoundingBox box in boxes)
            {
                box.Update(gameTime, IsLeft(), GetConvertedPosition());
            }

            //Update movement.
            MoveX(velocity.X);
            MoveY(velocity.Y);
            MoveZ(velocity.Z);
        }

        public int CompareTo(Entity other)
        {
            if (other == null)
            {
                return 0;
            }

            int h1 = GetSprite(Animation.State.STANCE).GetCurrentTexture().Height;
            int h2 = other.GetSprite(Animation.State.STANCE).GetCurrentTexture().Height;
            int offset = (h1 - h2) / 2;

            float z1 = h1 + (GetPosZ() / 2);
            float z2 = h2 + (other.GetPosZ() / 2) - offset;

            if (z1.Equals(z2))
            {
                return GetEntityId().CompareTo(other.GetEntityId());
            }
            else
            {
                return z1.CompareTo(z2);
            }
        }
    }
}
