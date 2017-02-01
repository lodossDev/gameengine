using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Game1;

/**
 * *
 * 
 *  GAME PAD BUTTON STATE AXIS
 * X= -1 - left , 1 - right
 * Y = -1 - down, 1 - up
 * 
 **/

namespace Game1 {

    public class InputControl {
        public enum InputDirection {NONE, UP, DOWN, LEFT, RIGHT, UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT}
        public bool UP, DOWN, LEFT, RIGHT, JUMP_PRESS, ATTACK_PRESS;

        private Entity player;
        private InputDirection inputDirection;
        private PlayerIndex playerIndex;

        public KeyboardState oldKeyboardState, currentKeyboardState;
        public GamePadState oldPadState, currentPadState;

        private InputBuffer pressedState;
        private InputBuffer releasedState;
        private InputBuffer heldState;
        public float walkPressTime = 0f;
        private float walkSpeed = 5f;
        private float runSpeed = 10f;
        private float veloctiy = 5f;


        public InputControl(Entity player, PlayerIndex index) {
            this.player = player;
            this.playerIndex = index;
            inputDirection = InputDirection.NONE;

            oldKeyboardState = new KeyboardState();
            oldPadState = new GamePadState();

            pressedState = new InputBuffer(InputHelper.ButtonState.Pressed);
            releasedState = new InputBuffer(InputHelper.ButtonState.Released);
            heldState = new InputBuffer(InputHelper.ButtonState.Held, 200);

            UP = false;
            DOWN = false;
            LEFT = false;
            RIGHT = false;
            JUMP_PRESS = false;
            ATTACK_PRESS = false;
        }

        private void Reset() {
            inputDirection = InputDirection.NONE;
            JUMP_PRESS = false;
            ATTACK_PRESS = false;

            if (UP && (!currentKeyboardState.IsKeyUp(Keys.Up) 
                    && (currentPadState.IsButtonUp(Buttons.DPadUp) 
                    && currentPadState.IsButtonUp(Buttons.LeftThumbstickUp)))) {

                player.ResetZ();
                walkPressTime = 0f;
                UP = false;
            }

            if (UP && (currentKeyboardState.IsKeyUp(Keys.Up) 
                    && !currentPadState.IsButtonDown(Buttons.DPadUp) 
                    && !currentPadState.IsButtonDown(Buttons.LeftThumbstickUp))) {

                player.ResetZ();
                walkPressTime = 0f;
                UP = false;
            }

            if (DOWN && (!currentKeyboardState.IsKeyDown(Keys.Down) 
                      && (currentPadState.IsButtonUp(Buttons.DPadDown) 
                      && currentPadState.IsButtonUp(Buttons.LeftThumbstickDown)))) {

                player.ResetZ();
                walkPressTime = 0f;
                DOWN = false;
            }

            if (DOWN && (currentKeyboardState.IsKeyUp(Keys.Down) 
                      && !currentPadState.IsButtonDown(Buttons.DPadDown) 
                      && !currentPadState.IsButtonDown(Buttons.LeftThumbstickDown))) {

                player.ResetZ();
                walkPressTime = 0f;
                DOWN = false;
            }

            if (RIGHT && (!currentKeyboardState.IsKeyDown(Keys.Right) 
                        && (currentPadState.IsButtonUp(Buttons.DPadRight) 
                        && currentPadState.IsButtonUp(Buttons.LeftThumbstickRight)))) {

                player.ResetX();
                walkPressTime = 0f;
                RIGHT = false;
            }
                player.ResetX();

            if (RIGHT && (currentKeyboardState.IsKeyUp(Keys.Right) 
                        && !currentPadState.IsButtonDown(Buttons.DPadRight) 
                        && !currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))) {

                player.ResetX();
                walkPressTime = 0f;
                RIGHT = false;
            }

            if (LEFT && (!currentKeyboardState.IsKeyDown(Keys.Left) 
                        && (currentPadState.IsButtonUp(Buttons.LeftThumbstickLeft) 
                        && currentPadState.IsButtonUp(Buttons.DPadLeft)))) {

                player.ResetX();
                walkPressTime = 0f;
                LEFT = false;
            }

            if (LEFT && currentKeyboardState.IsKeyUp(Keys.Left) 
                    && !currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft) 
                    && !currentPadState.IsButtonDown(Buttons.DPadLeft)) {

                player.ResetX();
                walkPressTime = 0f;
                LEFT = false;
            }

            if (JUMP_PRESS && (currentKeyboardState.IsKeyUp(Keys.Space) || currentPadState.IsButtonUp(Buttons.A))) {
                JUMP_PRESS = false;
            }

            if (ATTACK_PRESS && (currentKeyboardState.IsKeyUp(Keys.A) || currentPadState.IsButtonUp(Buttons.X))) {
                ATTACK_PRESS = false;
            }
        }

        public void UpdateDefaultControls(GameTime gameTime) {
            Reset();

            if (((currentKeyboardState.IsKeyDown(Keys.Space))
                    && (!oldKeyboardState.IsKeyDown(Keys.Space)))

                        || (currentPadState.IsButtonDown(Buttons.A)
                                && !oldPadState.IsButtonDown(Buttons.A))) {

                JUMP_PRESS = true;
            }

            if (((currentKeyboardState.IsKeyDown(Keys.A))
                    && (!oldKeyboardState.IsKeyDown(Keys.A)))
                        
                        || (currentPadState.IsButtonDown(Buttons.X)
                                && !oldPadState.IsButtonDown(Buttons.X))) {

                ATTACK_PRESS = true;
            }

            if ((currentKeyboardState.IsKeyDown(Keys.Z))
                    && (!oldKeyboardState.IsKeyDown(Keys.Z))) {

                player.GetCurrentSprite().IncrementFrame();
            }

            if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                veloctiy = walkSpeed;
            } else {
                veloctiy = runSpeed;
            }

            ProcessAttack();
            ProcessJump();
            
            if (player.IsNonActionState()) {

                if (!DOWN && (currentKeyboardState.IsKeyDown(Keys.Up) 
                                || currentPadState.IsButtonDown(Buttons.DPadUp) 
                                || currentPadState.IsButtonDown(Buttons.LeftThumbstickUp))) {

                    if (!RIGHT && (currentKeyboardState.IsKeyDown(Keys.Left) 
                                       || currentPadState.IsButtonDown(Buttons.DPadLeft) 
                                       || currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft))) {

                        inputDirection = InputDirection.UP_LEFT;
                        player.MoveX(veloctiy, -1);
                        player.SetIsLeft(true);
                        LEFT = true;

                    } else if (!LEFT && (currentKeyboardState.IsKeyDown(Keys.Right) 
                                            || currentPadState.IsButtonDown(Buttons.DPadRight) 
                                            || currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))) {

                        inputDirection = InputDirection.UP_RIGHT;
                        player.MoveX(veloctiy, 1);
                        player.SetIsLeft(false);
                        RIGHT = true;

                    } else {
                        inputDirection = InputDirection.UP;
                    }

                    if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                        player.SetAnimationState(Animation.State.WALK_TOWARDS);
                    } 

                    player.MoveZ(veloctiy, -1);
                    UP = true;

                } else if (!UP && (currentKeyboardState.IsKeyDown(Keys.Down) 
                                        || currentPadState.IsButtonDown(Buttons.DPadDown) 
                                        || currentPadState.IsButtonDown(Buttons.LeftThumbstickDown))) {

                    if (!RIGHT && (currentKeyboardState.IsKeyDown(Keys.Left) 
                                        || currentPadState.IsButtonDown(Buttons.DPadLeft) 
                                        || currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft))) {  

                        inputDirection = InputDirection.DOWN_LEFT;
                        player.MoveX(veloctiy, -1);
                        player.SetIsLeft(true);
                        LEFT = true;

                    } else if (!LEFT && (currentKeyboardState.IsKeyDown(Keys.Right) 
                                            || currentPadState.IsButtonDown(Buttons.DPadRight) 
                                            || currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))) {

                        inputDirection = InputDirection.DOWN_RIGHT;
                        player.MoveX(veloctiy, 1);
                        player.SetIsLeft(false);
                        RIGHT = true;

                    } else {
                        inputDirection = InputDirection.DOWN;
                    }

                    if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                        player.SetAnimationState(Animation.State.WALK_TOWARDS);
                    }

                    player.MoveZ(veloctiy, 1);
                    DOWN = true;
                }
            }

            if (player.IsNonActionState() && !IsDirectionalPress()) {

                if (!RIGHT && (currentKeyboardState.IsKeyDown(Keys.Left) 
                                  || currentPadState.IsButtonDown(Buttons.DPadLeft)
                                  || currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft))) {

                    inputDirection = InputDirection.LEFT;
                    walkPressTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    player.SetDirectionX(-1);

                    if (walkPressTime >= 120) {

                        if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                            player.SetAnimationState(Animation.State.WALK_TOWARDS);
                        }

                        player.MoveX(veloctiy, -1);
                    }

                    player.SetIsLeft(true);
                    LEFT = true;

                } else if (!LEFT && (currentKeyboardState.IsKeyDown(Keys.Right) 
                                         || currentPadState.IsButtonDown(Buttons.DPadRight)
                                         || currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))) {

                    inputDirection = InputDirection.RIGHT; 
                    walkPressTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    player.SetDirectionX(1);

                    if (walkPressTime >= 120) {

                        if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                            player.SetAnimationState(Animation.State.WALK_TOWARDS);
                        }
                       
                        player.MoveX(veloctiy, 1);
                    }

                    player.SetIsLeft(false);
                    RIGHT = true;
                }
            }
        }

        private void ProcessJump() {
            if (JUMP_PRESS) {
                if (LEFT) {
                    player.SetJump(-15f, -Math.Abs(veloctiy));

                } else if (RIGHT) {
                    player.SetJump(-15f, Math.Abs(veloctiy));

                } else {
                    player.SetJump(-15f);
                }
            }
        }

        private void ProcessAttack() {
            if (ATTACK_PRESS) {

                if (!player.IsToss()) {
                    player.ProcessAttackChainStep();

                } else {
                    if (!player.IsInAnimationAction(Animation.Action.ATTACKING)
                            && !player.IsInAnimationAction(Animation.Action.RECOVERY)
                            && player.InAir()) {

                        if ((double)player.GetTossInfo().velocity.X == 0.0) {
                            player.SetAnimationState(Animation.State.JUMP_ATTACK1);
                        }
                        else {
                            player.SetAnimationState(Animation.State.JUMP_TOWARD_ATTACK1);
                        }
                    }
                }
            }
        }

        public void ReadPressedInputBuffer(GameTime gameTime) {
            InputHelper.KeyPress pressedButtonState = InputHelper.KeyPress.NONE;
            InputHelper.KeyPress pressedDirectionState = InputHelper.KeyPress.NONE;

            pressedButtonState = InputHelper.GetPressedButtons(oldPadState, oldKeyboardState, currentPadState, currentKeyboardState);
            pressedDirectionState = InputHelper.GetPressedDirections(oldPadState, oldKeyboardState, currentPadState, currentKeyboardState);

            pressedState.ReadInputBuffer(gameTime, pressedButtonState, pressedDirectionState);
        }

        public void ReadReleasedInputBuffer(GameTime gameTime) {
            InputHelper.KeyPress releasedButtonState = InputHelper.KeyPress.NONE;
            InputHelper.KeyPress releasedDirectionState = InputHelper.KeyPress.NONE;

            releasedButtonState = InputHelper.GetReleasedButtons(oldPadState, oldKeyboardState, currentPadState, currentKeyboardState);
            releasedDirectionState = InputHelper.GetReleasedDirections(oldPadState, oldKeyboardState, currentPadState, currentKeyboardState);

            releasedState.ReadInputBuffer(gameTime, releasedButtonState, releasedDirectionState);
        }

        public void ReadHeldInputBuffer(GameTime gameTime) {
            InputHelper.KeyPress heldButtonState = InputHelper.KeyPress.NONE;
            InputHelper.KeyPress heldDirectionState = InputHelper.KeyPress.NONE;

            heldButtonState = InputHelper.GetHeldButtons(currentPadState, currentKeyboardState);
            heldDirectionState = InputHelper.GetHeldDirections(currentPadState, currentKeyboardState);

            heldState.ReadInputBuffer(gameTime, heldButtonState, heldDirectionState);
        }

        public void Update(GameTime gameTime) {
            currentKeyboardState = Keyboard.GetState();
            currentPadState = GamePad.GetState(playerIndex);
            

            UpdateDefaultControls(gameTime);
            
            ReadPressedInputBuffer(gameTime);
            ReadHeldInputBuffer(gameTime);
            ReadReleasedInputBuffer(gameTime);

            if (IsInputDirection(InputDirection.NONE)) {
                player.ResetX();
                player.ResetZ();
                player.ResetToIdle(gameTime);
            }

            oldKeyboardState = currentKeyboardState;
            oldPadState = currentPadState;
        }

        private void ResetBuffers() {
            pressedState.GetBuffer().Clear();
            releasedState.GetBuffer().Clear();
            heldState.GetBuffer().Clear();
        }

        private InputBuffer GetNextBuffer(InputHelper.KeyState currentKeyPress) {
            InputBuffer currentBuffer = null;

            if (currentKeyPress.GetState() == InputHelper.ButtonState.Pressed) {
                currentBuffer = pressedState;

            } else if (currentKeyPress.GetState() == InputHelper.ButtonState.Released) {
                currentBuffer = releasedState;

            } else if (currentKeyPress.GetState() == InputHelper.ButtonState.Held) {
                currentBuffer = heldState;
            }

            return currentBuffer;
        }

        private void checkHeld(InputHelper.CommandMove command, InputHelper.KeyState currentKeyState) {
            int held = 0;
            //Debug.WriteLine("HELD KEY: " + currentKeyState.GetState());
            currentKeyState = command.GetCurrentMove();

            if (command.IsMaxNegativeReached() == true) {
                command.Reset();
                held = 0;
            }

            if (releasedState.GetCurrentInputState() != InputHelper.KeyPress.NONE) {

                if (releasedState.GetCurrentInputState() == currentKeyState.GetKey()
                        || (releasedState.GetCurrentInputState(releasedState.GetCurrentStateStep() - 2) == currentKeyState.GetKey()
                                && releasedState.GetCurrentInputState() != currentKeyState.GetKey())) {

                    command.Reset();
                }
            }

            for (int i = 0; i < heldState.GetBuffer().Count - 1; i++) {

                bool reset = (releasedState.GetBuffer().Count >= i + 2
                                    && releasedState.GetBuffer()[i + 1] == currentKeyState.GetKey());

                if (reset) {
                    held = 0;
                    command.IncrementNegativeCount();
                    break;
                }

                if (heldState.GetBuffer()[i + 1] == currentKeyState.GetKey()) {
                    held++;
                    command.ResetNegativeEdge();

                } else {
                    held = 0;
                    command.Reset();
                    break;
                }
            }

            //Debug.WriteLine("HELD COUNT: " + held);
            //Debug.WriteLine("HELD TIME: " + currentKeyState.GetKeyHeldTime());

            if (held >= currentKeyState.GetKeyHeldTime()) {
                command.Next();

            } else {
                command.IncrementNegativeCount();
            }
        }

        public bool Matches(InputHelper.CommandMove command) {
            InputHelper.KeyState previousKeyState = command.GetPreviousMove();
            InputHelper.KeyState currentKeyState = command.GetCurrentMove();
            InputBuffer currentBuffer = GetNextBuffer(currentKeyState);

            if (currentKeyState.GetState() != InputHelper.ButtonState.Held) {
                if (command.IsMaxNegativeReached() == true) {
                    command.Reset();
                    return false;
                }
                
                if (currentBuffer.GetCurrentInputState() == currentKeyState.GetKey()) {
                    command.Next();

                    if (command.GetCurrentMoveStep() >= command.GetMoves().Count - 1) {
                        currentKeyState = command.GetMoves()[command.GetMoves().Count - 1];

                    } else {
                        currentKeyState = command.GetCurrentMove();
                    }

                    currentBuffer = GetNextBuffer(currentKeyState);
                    //Debug.WriteLine("NEXT BUFFER: " + currentKeyState.GetState());
                } else {
                    command.IncrementNegativeCount();
                }
            } else {
                //Debug.WriteLine("IN HELD");
                checkHeld(command, currentKeyState);
            }

            //Debug.WriteLine("CURRENTMOVE STEP: " + command.GetCurrentMoveStep());

            if (command.IsComplete()) {
                //Debug.WriteLine("IS COMPLETE");
                command.Reset();
                ResetBuffers();
                return true;
            }

            return false;
        }

        public InputDirection GetInputDirection() {
            return inputDirection;
        }

        public PlayerIndex GetPlayerIndex() {
            return playerIndex;
        }

        public bool IsInputDirection(InputDirection input) {
            return (inputDirection == input);
        }

        public bool IsDirectionalPress() {
            return (IsInputDirection(InputDirection.UP)
                        || IsInputDirection(InputDirection.DOWN)
                        || IsInputDirection(InputDirection.LEFT)
                        || IsInputDirection(InputDirection.RIGHT)
                        || IsInputDirection(InputDirection.DOWN_LEFT)
                        || IsInputDirection(InputDirection.DOWN_RIGHT)
                        || IsInputDirection(InputDirection.UP_LEFT)
                        || IsInputDirection(InputDirection.UP_RIGHT));
        }

        public InputBuffer GetPressedState() {
            return pressedState;
        }

        public InputBuffer GetReleasedState() {
            return releasedState;
        }

        public InputBuffer GetHeldState() {
            return heldState;
        }
    }
}
