using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Entity leo, leo2, drum, drum2, drum3, drum4;
        RenderManager renderManager;
        CollisionManager collisionManager;
        CLNS.BoundingBox box1;
        SpriteFont font1;
        InputControl control;
        Camera camera;
        float ticks = 0f;
        Stage1 level1;
        LifeBar bar;
        float barHealth = 100f;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 700;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Setup.graphicsDevice = GraphicsDevice;
            Setup.contentManager = Content;
            Setup.spriteBatch = spriteBatch;

            camera = new Camera();
            camera.Pos = new Vector2(800f, 0.0f);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            font1 = Content.Load<SpriteFont>("Fonts/MyFont");

            leo = new Entity(Entity.EntityType.PLAYER, "Leo1");
            leo2 = new Entity(Entity.EntityType.ENEMY, "Leo2");

            leo.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Leo/Stance"));
            leo.SetFrameDelay(Animation.State.STANCE, 13);


            leo.AddSprite(Animation.State.WALK_TOWARDS, new Sprite("Sprites/Actors/Leo/Walk", Animation.Type.REPEAT));

            leo.AddSprite(Animation.State.JUMP_START, new Sprite("Sprites/Actors/Leo/JumpStart", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.JUMP_START, 20, 13);
            leo.SetFrameDelay(Animation.State.JUMP_START, 13);

            leo.AddSprite(Animation.State.LAND, new Sprite("Sprites/Actors/Leo/Land", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.LAND, 45, -24);
            leo.SetFrameDelay(Animation.State.LAND, 13);

            leo.AddSprite(Animation.State.JUMP, new Sprite("Sprites/Actors/Leo/Jump", Animation.Type.REPEAT, 13));
            leo.SetFrameDelay(Animation.State.JUMP, 15);

            leo.AddSprite(Animation.State.JUMP_TOWARDS, new Sprite("Sprites/Actors/Leo/JumpTowards", Animation.Type.REPEAT, 13));
            leo.SetFrameDelay(Animation.State.JUMP_TOWARDS, 10);

            leo.SetSpriteOffSet(Animation.State.JUMP, 30, -120);
            leo.SetSpriteOffSet(Animation.State.JUMP_TOWARDS, 30, -120);

            leo.AddAnimationLink(new Animation.Link(Animation.State.JUMP_START, Animation.State.JUMP, leo.GetSprite(Animation.State.JUMP_START).GetFrames()));
            leo.SetTossFrame(Animation.State.JUMP, 1);
            leo.SetTossFrame(Animation.State.JUMP_TOWARDS, 1);

            leo.AddSprite(Animation.State.FALL, new Sprite("Sprites/Actors/Leo/Falling", Animation.Type.REPEAT, 5));
            leo.SetSpriteOffSet(Animation.State.FALL, 30, -120);

            leo.SetAnimationState(Animation.State.STANCE);
            leo.AddBox(new CLNS.BoundingBox(CLNS.BoxType.BOUNDS_BOX, 125, 210, -30, 80));
            leo.AddBox(new CLNS.BoundingBox(CLNS.BoxType.HEIGHT_BOX, 105, 180, -20, 100));
            leo.SetScale(1.6f, 2.2f);
            leo.SetPostion(400, 0, 400);
            leo.SetSpriteOffSet(Animation.State.WALK_TOWARDS, 40, -15);
            leo.SetResetFrame(Animation.State.WALK_TOWARDS, 3);
            leo.SetMoveFrame(Animation.State.WALK_TOWARDS, 3);
            leo.SetHeight(180);

            leo.SetFrameDelay(Animation.State.WALK_TOWARDS, 10);
            leo.SetFrameDelay(Animation.State.WALK_TOWARDS, 1, 12);
            leo.SetFrameDelay(Animation.State.WALK_TOWARDS, 2, 12);
            leo.SetFrameDelay(Animation.State.WALK_TOWARDS, 3, 25);

            leo.AddSprite(Animation.State.ATTACK1, new Sprite("Sprites/Actors/Leo/Attack1", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.ATTACK1, 65, -75);
            leo.SetFrameDelay(Animation.State.ATTACK1, 18);
            leo.SetFrameDelay(Animation.State.ATTACK1, 1, 17);
            leo.SetFrameDelay(Animation.State.ATTACK1, 2, 17);

            leo.AddBox(Animation.State.ATTACK1, 6, new CLNS.AttackBox(100, 80, 132, 45));
            //leo.GetAttackBox(Animation.State.ATTACK1, 6).SetComboStep(0);

            leo.AddBox(Animation.State.ATTACK1, 6, new CLNS.AttackBox(100, 80, 59, 99, 1));
            //leo.GetAttackBox(Animation.State.ATTACK1, 6).SetComboStep(1);

            leo.AddBox(Animation.State.ATTACK1, 7, new CLNS.AttackBox(150, 50, -10, 210, 1));
            leo.GetAttackBox(Animation.State.ATTACK1, 7).SetComboStep(0);


            leo.AddSprite(Animation.State.ATTACK2, new Sprite("Sprites/Actors/Leo/Attack2", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.ATTACK2, 50, -75);
            leo.SetFrameDelay(Animation.State.ATTACK2, 20);
            leo.AddBox(Animation.State.ATTACK2, 6, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK2, 7, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK2, 8, new CLNS.AttackBox(220, 230, 20, 30));

            leo.AddSprite(Animation.State.ATTACK3, new Sprite("Sprites/Actors/Leo/Attack3", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.ATTACK3, 90, -25);
            leo.SetFrameDelay(Animation.State.ATTACK3, 20);
            leo.AddBox(Animation.State.ATTACK3, 6, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK3, 7, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK3, 8, new CLNS.AttackBox(220, 230, 20, 30));

            leo.AddSprite(Animation.State.ATTACK4, new Sprite("Sprites/Actors/Leo/Attack4", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.ATTACK4, 50, 10);
            leo.SetFrameDelay(Animation.State.ATTACK4, 15);
            leo.AddBox(Animation.State.ATTACK4, 3, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK4, 4, new CLNS.AttackBox(220, 230, 20, 30));

            leo.AddSprite(Animation.State.ATTACK5, new Sprite("Sprites/Actors/Leo/Attack5", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.ATTACK5, 75, -52);
            leo.SetFrameDelay(Animation.State.ATTACK5, 18);
            leo.AddBox(Animation.State.ATTACK5, 5, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK5, 6, new CLNS.AttackBox(220, 230, 20, 30));

            leo.AddSprite(Animation.State.ATTACK6, new Sprite("Sprites/Actors/Leo/Attack6", Animation.Type.ONCE), true);
            leo.SetSpriteOffSet(Animation.State.ATTACK6, 45, -90);
            leo.SetFrameDelay(Animation.State.ATTACK6, 1);
            leo.AddBox(Animation.State.ATTACK6, 5, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK6, 6, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK6, 7, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK6, 8, new CLNS.AttackBox(220, 230, 20, 30));

            leo.AddSprite(Animation.State.JUMP_ATTACK1, new Sprite("Sprites/Actors/Leo/JumpAttack1", Animation.Type.ONCE), true);
            leo.SetSpriteOffSet(Animation.State.JUMP_ATTACK1, 80, -90);
            leo.SetFrameDelay(Animation.State.JUMP_ATTACK1, 19);
            leo.SetFrameDelay(Animation.State.JUMP_ATTACK1, 1, 15);
            leo.SetFrameDelay(Animation.State.JUMP_ATTACK1, 2, 15);

            leo.AddSprite(Animation.State.JUMP_TOWARD_ATTACK1, new Sprite("Sprites/Actors/Leo/JumpAttack2", Animation.Type.ONCE), true);
            leo.SetSpriteOffSet(Animation.State.JUMP_TOWARD_ATTACK1, 80, -90);
            leo.SetFrameDelay(Animation.State.JUMP_TOWARD_ATTACK1, 19);
            leo.SetFrameDelay(Animation.State.JUMP_TOWARD_ATTACK1, 1, 15);
            leo.SetFrameDelay(Animation.State.JUMP_TOWARD_ATTACK1, 2, 15);

            leo.SetTossFrame(Animation.State.JUMP_ATTACK1, 1);
            leo.SetTossFrame(Animation.State.JUMP_TOWARD_ATTACK1, 1);

            leo.AddSprite(Animation.State.JUMP_RECOVER1, new Sprite("Sprites/Actors/Leo/JumpRecover1", Animation.Type.REPEAT, 3));
            leo.SetSpriteOffSet(Animation.State.JUMP_RECOVER1, 35, -110);
            leo.SetFrameDelay(Animation.State.JUMP_RECOVER1, 19);
            leo.SetFrameDelay(Animation.State.JUMP_RECOVER1, 1, 10);
            leo.SetFrameDelay(Animation.State.JUMP_RECOVER1, 2, 10);

            leo.AddAnimationLink(new Animation.Link(Animation.State.JUMP_ATTACK1, Animation.State.JUMP_RECOVER1, 8));
            leo.AddAnimationLink(new Animation.Link(Animation.State.JUMP_TOWARD_ATTACK1, Animation.State.JUMP_RECOVER1, 9));

            leo.SetDefaultAttackChain(new ComboAttack.Chain(new List<ComboAttack.Move>{
                new ComboAttack.Move(Animation.State.ATTACK1, 222000, 9),
                new ComboAttack.Move(Animation.State.ATTACK1, 222000, 9),
                new ComboAttack.Move(Animation.State.ATTACK1, 222000, 9),
                new ComboAttack.Move(Animation.State.ATTACK4, 222000, 6),
                new ComboAttack.Move(Animation.State.ATTACK4, 222000, 6),
                new ComboAttack.Move(Animation.State.ATTACK4, 222000, 6),
                new ComboAttack.Move(Animation.State.ATTACK2, 222000, 10),
                new ComboAttack.Move(Animation.State.ATTACK3, 222000, 9),
                new ComboAttack.Move(Animation.State.ATTACK3, 222000, 9),
                new ComboAttack.Move(Animation.State.ATTACK3, 222000, 9),
                new ComboAttack.Move(Animation.State.ATTACK5, 222000, 8),
                new ComboAttack.Move(Animation.State.ATTACK6, 222000, 10)
            }));

            /*leo.SetFrameDelay(Animation.State.ATTACK1, 1);
            leo.SetFrameDelay(Animation.State.ATTACK2, 1);
            leo.SetFrameDelay(Animation.State.ATTACK3, 1);
            leo.SetFrameDelay(Animation.State.ATTACK4, 1);
            leo.SetFrameDelay(Animation.State.ATTACK5, 1);
            leo.SetFrameDelay(Animation.State.ATTACK6, 1);*/

            leo2.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Leo/Stance"));
            leo2.AddSprite(Animation.State.WALK_TOWARDS, new Sprite("Sprites/Actors/Leo/Walk", Animation.Type.ONCE));

            leo2.SetAnimationState(Animation.State.STANCE);
            //leo2.SetFrameDelay(Animation.State.STANCE, 10);
            leo2.AddBox(new CLNS.BoundingBox(CLNS.BoxType.BOUNDS_BOX, 120, 160, -30, 60));
            

            drum = new Entity(Entity.EntityType.OBSTACLE, "DRUM1");
            drum.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Drum"));
            drum.SetAnimationState(Animation.State.STANCE);
            drum.AddBox(new CLNS.BoundingBox(CLNS.BoxType.BOUNDS_BOX, 125, 210, -30, 80));
            drum.AddBox(new CLNS.BoundingBox(CLNS.BoxType.HEIGHT_BOX, 125, 170, -30, 125));
            drum.SetScale(2.2f, 2.6f);
            drum.SetPostion(700, 0, 200);
            drum.SetSpriteOffSet(Animation.State.STANCE, 32, 90);
            drum.SetDepthOffset(-5);
            drum.SetDepth(20);
            drum.SetHeight(170);

            drum2 = new Entity(Entity.EntityType.OBSTACLE, "DRUM2");
            drum2.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Drum"));
            drum2.SetAnimationState(Animation.State.STANCE);
            drum2.AddBox(new CLNS.BoundingBox(CLNS.BoxType.BOUNDS_BOX, 125, 210, -30, 80));
            drum2.AddBox(new CLNS.BoundingBox(CLNS.BoxType.HEIGHT_BOX, 125, 170, -30, 125));
            drum2.SetScale(2.2f, 2.6f);
            drum2.SetPostion(500, 0, 200);
            drum2.SetSpriteOffSet(Animation.State.STANCE, 32, 90);
            drum2.SetDepthOffset(-5);
            drum2.SetDepth(20);
            drum2.SetHeight(170);

            drum3 = new Entity(Entity.EntityType.OBSTACLE, "DRUM3");
            drum3.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Drum"));
            drum3.SetAnimationState(Animation.State.STANCE);
            drum3.AddBox(new CLNS.BoundingBox(CLNS.BoxType.BOUNDS_BOX, 125, 210, -30, 80));
            drum3.AddBox(new CLNS.BoundingBox(CLNS.BoxType.HEIGHT_BOX, 125, 170, -30, 125));
            drum3.SetScale(2.2f, 2.6f);
            drum3.SetPostion(290, -180, 200);
            drum3.SetSpriteOffSet(Animation.State.STANCE, 32, 90);
            drum3.SetDepthOffset(-5);
            drum3.SetDepth(20);
            drum3.SetHeight(170);

            drum4 = new Entity(Entity.EntityType.OBSTACLE, "DRUM4");
            drum4.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Drum"));
            drum4.SetAnimationState(Animation.State.STANCE);
            drum4.AddBox(new CLNS.BoundingBox(CLNS.BoxType.BOUNDS_BOX, 125, 210, -30, 80));
            drum4.AddBox(new CLNS.BoundingBox(CLNS.BoxType.HEIGHT_BOX, 125, 170, -30, 125));
            drum4.SetScale(2.2f, 2.6f);
            drum4.SetPostion(800, -320, 200);
            drum4.SetGroundBase(-320);
            drum4.SetSpriteOffSet(Animation.State.STANCE, 32, 90);
            drum4.SetDepthOffset(-5);
            drum4.SetDepth(20);
            drum4.SetHeight(170);

            leo2.SetScale(1.6f, 2.2f);
            leo2.SetPostion(650, 0, 100);

            float x1 = -340+(339/2);
            float x2 = -340;

            //leo2.SetGroundBase(x1);
            leo2.SetHeight(180);
            leo.SetHeight(180);

            //level1 = new Stage1();
            //bar = new LifeBar(0, 0);

            renderManager = new RenderManager();
            renderManager.AddEntity(leo);
            renderManager.AddEntity(leo2);
            renderManager.AddEntity(drum);
            renderManager.AddEntity(drum2);
            renderManager.AddEntity(drum3);
            renderManager.AddEntity(drum4);
            //renderManager.AddLevel(level1);

            collisionManager = new CollisionManager();
            collisionManager.AddEntity(leo);
            //collisionManager.AddEntity(leo2);
            collisionManager.AddEntity(drum);
            collisionManager.AddEntity(drum2);
            collisionManager.AddEntity(drum3);
            collisionManager.AddEntity(drum4);

            control = new InputControl(leo, PlayerIndex.One);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //TimeSpan ts = new TimeSpan(60);
            //ticks = (float)ts.TotalMilliseconds;
            //collisionManager.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                Setup.isPause = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                Setup.isPause = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                renderManager.HideBoxes();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                renderManager.ShowBoxes();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                //Setup.rotate += 2.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Setup.scaleY += 2.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //barHealth -= (20.05f * (float)gameTime.ElapsedGameTime.TotalSeconds);
                //leo.SetColor(255, 0, 0);
                leo.Flash(2);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                //Setup.rotate -= 2.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Setup.scaleY += 5.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Setup.scaleX -= 5.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //barHealth += (20.05f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            //bar.Percent((int)barHealth);

            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.K))
                {
                    drum3.Toss(-15, -5);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.L))
                {
                    drum3.Toss(-15, 5);
                }
                else
                {
                    drum3.Toss(-15);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                drum3.VelX(-5);
                //drum3.SetIsLeft(true);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                drum3.VelX(5);
                //drum3.SetIsLeft(false);
            }
            else
            {
                drum3.VelX(0);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
               // level1.ScrollX(-5/2f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                //level1.ScrollX(5/2f);
            }

            if (!Setup.isPause)
            {
                control.Update(gameTime);
                collisionManager.Update(gameTime);

                //Toss needs to be updated before collision
                leo.Update(gameTime);
                leo2.Update(gameTime);
                drum.Update(gameTime);
                drum2.Update(gameTime);
                drum3.Update(gameTime);
                drum4.Update(gameTime);

                /*level1.ScrollX(-leo.GetVelocity().X);
                drum.MoveX(-leo.GetVelocity().X);
                drum2.MoveX(-leo.GetVelocity().X);
                drum3.MoveX(-leo.GetVelocity().X);
                drum4.MoveX(-leo.GetVelocity().X);*/
            }

            // TODO: Add your update logic here
            //bar.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.NonPremultiplied,
                        SamplerState.PointClamp,
                        null,
                        null,
                        null,
                        null);


            renderManager.Draw(gameTime);

            /*List<Entity> above = collisionManager.FindAbove(drum2);
            int i = 1;
            foreach(Entity entity in above)
            {
                i++;
                spriteBatch.DrawString(font1, "BELOW ENTITY: " + entity.GetName(), new Vector2(20, 110+(i*20)), Color.Black);
            }*/

            int i = 1;
            foreach (InputHelper.KeyPress button in control.pressedBuffer.inputBuffer)
            {
                i++;
                spriteBatch.DrawString(font1, "BUTTON: " + button, new Vector2(20, 110 + (i * 20)), Color.Black);
            }

            InputHelper.KeyPress bb = InputHelper.KeyPress.B | InputHelper.KeyPress.X;
            spriteBatch.DrawString(font1, "current frame: " + (bb == (InputHelper.KeyPress.X | InputHelper.KeyPress.B)), new Vector2(20, 40), Color.Black);

            /*Rectangle targetBox1 = drum3.GetBoxes(CLNS.BoxType.HEIGHT_BOX)[0].GetRect();
            Rectangle targetBox2 = drum2.GetBoxes(CLNS.BoxType.HEIGHT_BOX)[0].GetRect();

            spriteBatch.DrawString(font1, "current frame: " + leo.GetCurrentFrame(), new Vector2(20, 40), Color.Black);
            spriteBatch.DrawString(font1, "leo trans: " + leo.colorInfo.fadeFrequency, new Vector2(20, 60), Color.Black);
            spriteBatch.DrawString(font1, "leo flash: " + leo.colorInfo.isFlash, new Vector2(20, 80), Color.Black);
            spriteBatch.DrawString(font1, "leo flashFinish: " + leo.colorInfo.expired, new Vector2(20, 100), Color.Black);
            //spriteBatch.DrawString(font1, "drum1 hitbyattackid: " + drum.GetAttackInfo().hitByAttackId, new Vector2(20, 110), Color.Black);
            //spriteBatch.DrawString(font1, "drum3 hitbyattackid: " + drum3.GetAttackInfo().hitByAttackId, new Vector2(20, 140), Color.Black);
            //spriteBatch.DrawString(font1, "current attack id: " + CollisionManager.hit_id, new Vector2(20, 170), Color.Black);

            //spriteBatch.DrawString(font1, "moveIndex: " + leo.GetDefaultAttackChain().GetCurrentMoveIndex(), new Vector2(20, 60), Color.Black);

            /*int i = 1;
            foreach (Keys key in Keyboard.GetState().GetPressedKeys())
            {
                spriteBatch.DrawString(font1, "PRESS: " + key, new Vector2(20, 15*i), Color.Black);
                i++;
            }*/
            //spriteBatch.DrawString(font1, "LEO2 X: " + leo2.GetPosX(), new Vector2(20, 70), Color.Black);
            //spriteBatch.DrawString(font1, "LEO STANCE WIDTH: " + leo.GetSprite(Animation.State.STANCE).GetCurrentTexture().Width, new Vector2(20, 90), Color.Black);
            //spriteBatch.DrawString(font1, "LEO WALKING WIDTH: " + leo.GetSprite(Animation.State.JUMP_START).GetCurrentTexture().Width, new Vector2(20, 110), Color.Black);

            //bar.Render();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
