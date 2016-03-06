using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Game1
{
    public class InputControl
    {
        public enum InputDirection {NONE, UP, DOWN, LEFT, RIGHT, UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT}
        public bool UP, DOWN, LEFT, RIGHT, JUMP_PRESS, ATTACK_PRESS;

        private Entity player;
        private InputDirection inputDirection;
        private PlayerIndex playerIndex;

        private KeyboardState oldKeyboardState, currentKeyboardState;
        private GamePadState oldPadState, currentPadState;
        public InputBuffer pressedBuffer;
        private float walkPressTime = 0f;


        public InputControl(Entity player, PlayerIndex index)
        {
            this.player = player;
            this.playerIndex = index;
            inputDirection = InputDirection.NONE;
            currentKeyboardState = new KeyboardState();
            currentPadState = new GamePadState();
            pressedBuffer = new InputBuffer();
            Reset();
        }

        public void Reset()
        {
            UP = false;
            DOWN = false;
            LEFT = false;
            RIGHT = false;
            JUMP_PRESS = false;
            ATTACK_PRESS = false;
        }

        public void UpdateDefaultControls(GameTime gameTime)
        {
            inputDirection = InputDirection.NONE;
            JUMP_PRESS = false;
            ATTACK_PRESS = false;

            if (UP && currentKeyboardState.IsKeyUp(Keys.Up))
            {
                player.VelZ(0f);
                walkPressTime = 0f;
                UP = false;
            }

            if (DOWN && currentKeyboardState.IsKeyUp(Keys.Down))
            {
                player.VelZ(0f);
                walkPressTime = 0f;
                DOWN = false;
            }

            if (RIGHT && currentKeyboardState.IsKeyUp(Keys.Right))
            {
                player.VelX(0f);
                walkPressTime = 0f;
                RIGHT = false;
            }

            if (LEFT && currentKeyboardState.IsKeyUp(Keys.Left))
            {
                player.VelX(0f);
                walkPressTime = 0f;
                LEFT = false;
            }

            if (JUMP_PRESS && currentKeyboardState.IsKeyUp(Keys.Space))
            {
                JUMP_PRESS = false;
            }

            if (ATTACK_PRESS && currentKeyboardState.IsKeyUp(Keys.A))
            {
                ATTACK_PRESS = false;
            }

            if (!player.IsToss() && !player.IsInAnimationAction(Animation.Action.ATTACKING))
            {
                if (!DOWN && currentKeyboardState.IsKeyDown(Keys.Up))
                {
                    if (!RIGHT && currentKeyboardState.IsKeyDown(Keys.Left))
                    {
                        inputDirection = InputDirection.UP_LEFT;
                        player.VelX(-5);
                        player.SetIsLeft(true);
                        LEFT = true;
                    }
                    else if (!LEFT && currentKeyboardState.IsKeyDown(Keys.Right))
                    {
                        inputDirection = InputDirection.UP_RIGHT;
                        player.VelX(5);
                        player.SetIsLeft(false);
                        RIGHT = true;
                    }
                    else
                    {
                        inputDirection = InputDirection.UP;
                    }

                    player.SetAnimationState(Animation.State.WALK_TOWARDS);
                    player.VelZ(-5);
                    UP = true;
                }
                else if (!UP && currentKeyboardState.IsKeyDown(Keys.Down))
                {
                    if (!RIGHT && currentKeyboardState.IsKeyDown(Keys.Left))
                    {
                        inputDirection = InputDirection.DOWN_LEFT;
                        player.VelX(-5);
                        player.SetIsLeft(true);
                        LEFT = true;
                    }
                    else if (!LEFT && currentKeyboardState.IsKeyDown(Keys.Right))
                    {
                        inputDirection = InputDirection.DOWN_RIGHT;
                        player.VelX(5);
                        player.SetIsLeft(false);
                        RIGHT = true;
                    }
                    else
                    {
                        inputDirection = InputDirection.DOWN;
                    }

                    player.SetAnimationState(Animation.State.WALK_TOWARDS);
                    player.VelZ(5);
                    DOWN = true;
                }
            }

            if (!IsDirectionalPress() && !player.IsToss()
                    && !player.IsInAnimationAction(Animation.Action.ATTACKING))
            {
                if (!RIGHT && currentKeyboardState.IsKeyDown(Keys.Left))
                {
                    inputDirection = InputDirection.LEFT;
                    walkPressTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (walkPressTime >= 120)
                    {
                        player.SetAnimationState(Animation.State.WALK_TOWARDS);
                        player.VelX(-5);
                        walkPressTime = 0f;
                    }

                    player.SetIsLeft(true);
                    LEFT = true;
                }
                else if (!LEFT && currentKeyboardState.IsKeyDown(Keys.Right))
                {
                    inputDirection = InputDirection.RIGHT;

                    walkPressTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (walkPressTime >= 120)
                    {
                        player.SetAnimationState(Animation.State.WALK_TOWARDS);
                        player.VelX(5);
                        walkPressTime = 0f;
                    }

                    player.SetIsLeft(false);
                    RIGHT = true;
                }
            }

            if ((currentKeyboardState.IsKeyDown(Keys.Space)) && (!oldKeyboardState.IsKeyDown(Keys.Space)))
            {
                JUMP_PRESS = true;
            }

            if ((currentKeyboardState.IsKeyDown(Keys.A)) && (!oldKeyboardState.IsKeyDown(Keys.A)))
            {
                ATTACK_PRESS = true;
            }

            if ((currentKeyboardState.IsKeyDown(Keys.Z)) && (!oldKeyboardState.IsKeyDown(Keys.Z)))
            {
                player.GetCurrentSprite().IncrementFrame();
            }

            if (!player.IsToss() && !player.IsInAnimationAction(Animation.Action.ATTACKING))
            {
                if (JUMP_PRESS)
                {
                    if (LEFT)
                    {
                        player.SetJump(-25f, -5f);
                    }
                    else if (RIGHT)
                    {
                        player.SetJump(-25f, 5f);
                    }
                    else
                    {
                        player.SetJump();
                    }
                }
            }

            if (ATTACK_PRESS)
            {
                if (!player.IsToss())
                {
                    player.AttackChainStep();
                }
                else
                {
                    if (!player.IsInAnimationAction(Animation.Action.ATTACKING)
                            && !player.IsInAnimationAction(Animation.Action.RECOVERY))
                    {
                        if (player.GetTossInfo().velocity.X == 0.0)
                        {
                            if (!player.InAir())
                            {
                                player.SetAnimationState(Animation.State.JUMP_START);
                                player.SetJumpLink(Animation.State.JUMP_ATTACK1);
                            }
                            else
                            {
                                player.SetAnimationState(Animation.State.JUMP_ATTACK1);
                            }

                            player.SetAnimationLink(Animation.State.JUMP_ATTACK1, Animation.State.JUMP_RECOVER1, player.GetSprite(Animation.State.JUMP_ATTACK1).GetFrames());
                        }
                        else
                        {
                            if (!player.InAir())
                            {
                                player.SetAnimationState(Animation.State.JUMP_START);
                                player.SetJumpLink(Animation.State.JUMP_TOWARD_ATTACK1);
                            }
                            else
                            {
                                player.SetAnimationState(Animation.State.JUMP_TOWARD_ATTACK1);
                            }

                            player.SetAnimationLink(Animation.State.JUMP_TOWARD_ATTACK1, Animation.State.JUMP_RECOVER1, player.GetSprite(Animation.State.JUMP_TOWARD_ATTACK1).GetFrames());
                        }
                    }
                }
            }
        }

        public void ReadInputBuffer(GameTime gameTime)
        {
            InputHelper.KeyPress pressedButtonState = InputHelper.KeyPress.NONE;
            InputHelper.KeyPress pressedDirectionState = InputHelper.KeyPress.NONE;

            pressedButtonState = InputHelper.GetPressedButtons(oldPadState, oldKeyboardState, currentPadState, currentKeyboardState);
            pressedDirectionState = InputHelper.GetPressedDirections(oldPadState, oldKeyboardState, currentPadState, currentKeyboardState);

            pressedBuffer.ReadInputBuffer(gameTime, pressedButtonState, pressedDirectionState);
        }

        public void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState(playerIndex);
            currentPadState = GamePad.GetState(playerIndex);

            UpdateDefaultControls(gameTime);
            ReadInputBuffer(gameTime);

            if (IsInputDirection(InputDirection.NONE))
            {
                player.VelX(0f);
                player.VelZ(0f);

                player.ResetToIdle(gameTime);
            }

            oldKeyboardState = currentKeyboardState;
            oldPadState = currentPadState;
        }

        public InputDirection GetInputDirection()
        {
            return inputDirection;
        }

        public PlayerIndex GetPlayerIndex()
        {
            return playerIndex;
        }

        public bool IsInputDirection(InputDirection input)
        {
            return (inputDirection == input);
        }

        public bool IsDirectionalPress()
        {
            return (IsInputDirection(InputDirection.UP)
                        || IsInputDirection(InputDirection.DOWN)
                        || IsInputDirection(InputDirection.LEFT)
                        || IsInputDirection(InputDirection.RIGHT)
                        || IsInputDirection(InputDirection.DOWN_LEFT)
                        || IsInputDirection(InputDirection.DOWN_RIGHT)
                        || IsInputDirection(InputDirection.UP_LEFT)
                        || IsInputDirection(InputDirection.UP_RIGHT));
        }
    }
}
