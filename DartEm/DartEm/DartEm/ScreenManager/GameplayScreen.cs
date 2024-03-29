#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Tasks;
using DartEm;
#endregion

namespace DartEm
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        SpriteFont gameFont2;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);
        Vector2 dartPosition;
        Vector2 dartFlick;
        Vector2 touchOrigin;

        string saveFileName = "HighScore";

        int maxDarts = 10;

        int score = 0;

        int highScore = 0;

        float dartYHolder;

        //List<Vector2> dartPositions = new List<Vector2>();

        //List<Texture2D> darts = new List<Texture2D>();

        List<Dart> darts = new List<Dart>();

        int activeDart = 0;

        Rectangle dartStartLocation;

        Texture2D picture, bullseye, door, door1;
        Texture2D dart, dart1, dart2, dart3, dart4, dart5, dart6;

        SoundEffect woosh;
        SoundEffect applause;

        Song bgmusic;

        static DataSaver<int> MyDataSaver = new DataSaver<int>();

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;

        bool flicked;
        bool touched;
        bool customPicture;
        bool music;
        bool sfx;
        bool zuneOverride;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(bool sfx, bool music, bool zune)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            loadHighScore();

            customPicture = false;
            flicked = false;
            touched = false;
            this.sfx = sfx;
            this.music = music;
            zuneOverride = zune;
        }

        public GameplayScreen(Texture2D photo, bool sfx, bool music, bool zune)
        {
            picture = photo;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            loadHighScore();

            customPicture = true;
            flicked = false;
            touched = false;
            this.sfx = sfx;
            this.music = music;
            zuneOverride = zune;
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");
                gameFont = content.Load<SpriteFont>("gamefont");
                gameFont2 = content.Load<SpriteFont>("gamefont2");

                if (!customPicture)
                {
                    picture = content.Load<Texture2D>("placeholder");
                }
                //dart = content.Load<Texture2D>("Dart");
                dart = content.Load<Texture2D>("Dart");
                dart1 = content.Load<Texture2D>("DartSprites/Dart1");
                dart2 = content.Load<Texture2D>("DartSprites/Dart2");
                dart3 = content.Load<Texture2D>("DartSprites/Dart3");
                dart4 = content.Load<Texture2D>("DartSprites/Dart4");
                dart5 = content.Load<Texture2D>("DartSprites/Dart5");
                dart6 = content.Load<Texture2D>("DartSprites/Dart6");
                door = content.Load<Texture2D>("door");
                door1 = content.Load<Texture2D>("door1");

                bullseye = content.Load<Texture2D>("bullseye");

                woosh = content.Load<SoundEffect>("Sounds/woosh");

                applause = content.Load<SoundEffect>("Sounds/applause");

                if (music)
                {
                    if (!MediaPlayer.GameHasControl && zuneOverride)
                    {
                        bgmusic = content.Load<Song>("Sounds/bgmusic");
                        MediaPlayer.Play(bgmusic);
                    }
                    else if (MediaPlayer.GameHasControl)
                    {
                        bgmusic = content.Load<Song>("Sounds/bgmusic");
                        MediaPlayer.Play(bgmusic);
                    }
                }

                dartPosition = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - (dart.Width / 2), (ScreenManager.GraphicsDevice.Viewport.Height) - (dart.Height));

                darts.Add(new Dart(dartPosition));

                dartStartLocation = new Rectangle((ScreenManager.GraphicsDevice.Viewport.Width / 2) - ((dart.Width / 2) * 5), (ScreenManager.GraphicsDevice.Viewport.Height) - (dart.Height), dart.Width * 5, dart.Height);

                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(1000);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
#endif
        }


        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (MediaPlayer.State == MediaState.Paused && !PhonePauseScreen.isPaused)
            {
                if (MediaPlayer.GameHasControl)
                    MediaPlayer.Resume();
            }

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                //// Apply some random jitter to make the enemy move around.
                //const float randomization = 10;

                //enemyPosition.X += (float)(random.NextDouble() - 0.5) * randomization;
                //enemyPosition.Y += (float)(random.NextDouble() - 0.5) * randomization;

                // Apply a stabilizing force to stop the enemy moving off the screen.
                //Vector2 targetPosition = new Vector2(
                //    ScreenManager.GraphicsDevice.Viewport.Width / 2 - gameFont.MeasureString("Insert Gameplay Here").X / 2,
                //    200);

                //enemyPosition = Vector2.Lerp(enemyPosition, targetPosition, 0.05f);

                //// TODO: this game isn't very fun! You could probably improve
                //// it by inserting something more interesting in this space :-)


                // This is the code to handle the forward motion of the darts.

                if (flicked)
                {
                    //dartPosition += dartFlick;
                    dartFlick = new Vector2(dartFlick.X, MathHelper.Clamp(dartFlick.Y, -35, 0));
                    //dartFlick = new Vector2(dartFlick.X, dartFlick.Y);
                    //System.Diagnostics.Debug.WriteLine(dartFlick.Y);

                    //To fix, store dartFlick.Y in temp variable then scale according to % of Y vector in terms of temp (IE: dartFlick.Y / temp)

                    if (dartFlick.Y / dartYHolder > 0.55f) //(dartFlick.Y < -6)
                    {
                        darts[activeDart].setSpriteStage(1);
                        //dart = dart1;
                    }
                    else if (dartFlick.Y / dartYHolder > 0.45f) //(dartFlick.Y < -5)
                    {
                        darts[activeDart].setSpriteStage(2);
                        //dart = dart2;
                    }
                    else if (dartFlick.Y / dartYHolder > 0.4f) //(dartFlick.Y < -4)
                    {
                        darts[activeDart].setSpriteStage(3);
                        //dart = dart3;
                    }
                    else if (dartFlick.Y / dartYHolder > 0.35f) //(dartFlick.Y < -3)
                    {
                        darts[activeDart].setSpriteStage(4);
                        //dart = dart4;
                    }
                    else if (dartFlick.Y / dartYHolder > 0.3f) //(dartFlick.Y < -2)
                    {
                        darts[activeDart].setSpriteStage(5);
                        //dart = dart5;
                    }
                    else if (dartFlick.Y / dartYHolder > 0.25f) //(dartFlick.Y < -1)
                    {
                        darts[activeDart].setSpriteStage(6);
                        //dart = dart6;
                    }

                    darts[activeDart].setPosition((darts[activeDart].getPosition() + dartFlick));
                    //dartFlick += new Vector2(0, Math.Abs(dartFlick.Y * 0.075f));
                    dartFlick += new Vector2(0, 1f);
                    //System.Diagnostics.Debug.WriteLine(dartFlick);
                    if (dartFlick.Y >= -1.0)
                    {
                        flicked = false;
                        touched = false;
                        calculateScore();
                        resetDart();
                        dart = dart1;
                    }
                }

                checkHighScore();
            }
        }

        public void saveScore()
        {
            int temp = MyDataSaver.LoadMyData(saveFileName);

            //System.Diagnostics.Debug.WriteLine(temp);
            if (temp != null)
            {
                if (temp < score)
                {
                    MyDataSaver.SaveMyData(score, saveFileName);
                    applause.Play();
                }
            }
        }

        public void loadHighScore()
        {
            int temp = MyDataSaver.LoadMyData(saveFileName);
            if (temp != null)
            {
                highScore = temp;
            }
        }


        public void checkHighScore()
        {
            if (highScore < score)
            {
                highScore = score;
            }
        }

        public void calculateScore()
        {
            int targetRadius1 = 48;
            int targetRadius2 = 96;
            int targetRadius3 = 144;
            int targetRadius4 = 192;
            int targetRadius5 = 240;

            Vector2 center = new Vector2(240f, 240f);

            if ((Math.Pow(((darts[activeDart].getPosition().X + (dart6.Width / 2)) - center.X), 2)) + (Math.Pow(((darts[activeDart].getPosition().Y + (dart6.Height / 2)) - center.Y), 2)) < Math.Pow(targetRadius1, 2))
            {
                score++;
            }
            if ((Math.Pow(((darts[activeDart].getPosition().X + (dart6.Width / 2)) - center.X), 2)) + (Math.Pow(((darts[activeDart].getPosition().Y + (dart6.Height / 2)) - center.Y), 2)) < Math.Pow(targetRadius2, 2))
            {
                score++;
            }
            if ((Math.Pow(((darts[activeDart].getPosition().X + (dart6.Width / 2)) - center.X), 2)) + (Math.Pow(((darts[activeDart].getPosition().Y + (dart6.Height / 2)) - center.Y), 2)) < Math.Pow(targetRadius3, 2))
            {
                score++;
            }
            if ((Math.Pow(((darts[activeDart].getPosition().X + (dart6.Width / 2)) - center.X), 2)) + (Math.Pow(((darts[activeDart].getPosition().Y + (dart6.Height / 2)) - center.Y), 2)) < Math.Pow(targetRadius4, 2))
            {
                score++;
            }
            if ((Math.Pow(((darts[activeDart].getPosition().X + (dart6.Width / 2)) - center.X), 2)) + (Math.Pow(((darts[activeDart].getPosition().Y + (dart6.Height / 2)) - center.Y), 2)) < Math.Pow(targetRadius5, 2))
            {
                score++;
            }
        }

        public void resetDart()
        {
            maxDarts--;
            if (maxDarts == 0)
            {
                //LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new PhoneEndScreen());
                ScreenManager.AddScreen(new PhoneEndScreen(picture, sfx, music, zuneOverride), ControllingPlayer);
                saveScore();
                if (MediaPlayer.GameHasControl)
                    MediaPlayer.Stop();
            }
            darts.Add(new Dart(dartPosition));
            activeDart++;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            foreach (GestureSample gs in input.Gestures)
            {
                switch (gs.GestureType)
                {
                    case GestureType.FreeDrag:
                        if (dartStartLocation.Contains((int)gs.Position.X, (int)gs.Position.Y))
                        {
                            touched = true;
                        }
                        break;
                    case GestureType.Flick:
                        if (gs.Delta.Y < -900 && touched && !flicked)
                        {
                            flicked = true;
                            //dartFlick = new Vector2(gs.Delta.X / new Vector2(gs.Delta.X, gs.Delta.Y).Length(), gs.Delta.Y / new Vector2(gs.Delta.X, gs.Delta.Y).Length()) * 30;
                            dartFlick = gs.Delta / 150;
                            dartYHolder = dartFlick.Y;
                            if (sfx)
                                woosh.Play();
                            //System.Diagnostics.Debug.WriteLine(dartFlick);
                        }
                        //System.Diagnostics.Debug.WriteLine("Flick");
                        break;

                }
            }

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                    ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
                    if (MediaPlayer.GameHasControl)
                        MediaPlayer.Pause();
#else
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
#endif
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (input.TouchState.Count > 0)
                {
                    Vector2 touchPosition = input.TouchState[0].Position;
                    Vector2 direction = touchPosition - playerPosition;
                    direction.Normalize();
                    movement += direction;
                }

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 8f;


            }



        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            if (picture.IsDisposed)
            {
                picture = content.Load<Texture2D>("placeholder");
            }
            else
            {
                spriteBatch.Begin();

                //spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);

                //spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
                //                       enemyPosition, Color.DarkRed);

                //spriteBatch.Draw(picture, new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - (picture.Width/2), 0), Color.White);

                spriteBatch.Draw(door, new Rectangle(0, 0, 480, 800), Color.White);

                spriteBatch.Draw(picture, new Rectangle(0, (240 - (int)((480f / picture.Width * picture.Height) / 2)), 480, (int)(480f / picture.Width * picture.Height)), Color.White);
                spriteBatch.Draw(door1, new Rectangle(0, 0, 480, 800), Color.White);
                spriteBatch.Draw(bullseye, new Vector2(0, 0), Color.White);

                spriteBatch.DrawString(gameFont, "Darts: " + maxDarts, new Vector2(300, 750), Color.Gray);
                spriteBatch.DrawString(gameFont, "Darts: " + maxDarts, new Vector2(301, 749), Color.Black);

                spriteBatch.DrawString(gameFont2, "High Score: " + highScore, new Vector2(10, 721), Color.Gray);
                spriteBatch.DrawString(gameFont2, "High Score: " + highScore, new Vector2(11, 720), Color.Black);

                spriteBatch.DrawString(gameFont, "Score: " + score, new Vector2(10, 750), Color.Gray);
                spriteBatch.DrawString(gameFont, "Score: " + score, new Vector2(11, 749), Color.Black);

                //spriteBatch.Draw(dart, dartPosition, Color.White);

                foreach (Dart t in darts)
                {
                    switch (t.getSpriteStage())
                    {
                        case 1:
                            dart = dart1;
                            break;
                        case 2:
                            dart = dart2;
                            break;
                        case 3:
                            dart = dart3;
                            break;
                        case 4:
                            dart = dart4;
                            break;
                        case 5:
                            dart = dart5;
                            break;
                        case 6:
                            dart = dart6;
                            break;

                    }
                    spriteBatch.Draw(dart, t.getPosition(), Color.White);
                }

                spriteBatch.End();

            }

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}
