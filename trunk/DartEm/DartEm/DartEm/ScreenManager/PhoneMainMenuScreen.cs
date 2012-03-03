#region File Description
//-----------------------------------------------------------------------------
// PhoneMainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using DartEm;
using Microsoft.Xna.Framework;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace DartEm
{
    class PhoneMainMenuScreen : PhoneMenuScreen
    {

        PhotoChooserTask photoChooserTask = new PhotoChooserTask();

        Texture2D photo;

        bool usingCustomPicture = false;

        public PhoneMainMenuScreen()
            : base("Main Menu")
        {
            // Create a button to start the game
            Button playButton = new Button("Play");
            playButton.Tapped += playButton_Tapped;
            MenuButtons.Add(playButton);

            // Create Photo button
            Button photosButton = new Button("Use Custom Photo");
            photosButton.Tapped += photosButton_Tapped;
            //ScreenManager.AddScreen(new SettingsMainMenuScreen(), );
            MenuButtons.Add(photosButton);

            // Create Settings Menu
            Button settingsButton = new Button("Settings");
            settingsButton.Tapped += settingsButton_Tapped;
            //ScreenManager.AddScreen(new SettingsMainMenuScreen(), );
            MenuButtons.Add(settingsButton);

            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed); 

            //// Create two buttons to toggle sound effects and music. This sample just shows one way
            //// of making and using these buttons; it doesn't actually have sound effects or music
            //BooleanButton sfxButton = new BooleanButton("Sound Effects", true);
            //sfxButton.Tapped += sfxButton_Tapped;
            //MenuButtons.Add(sfxButton);

            //BooleanButton musicButton = new BooleanButton("Music", true);
            //musicButton.Tapped += musicButton_Tapped;
            //MenuButtons.Add(musicButton);
        }

        void playButton_Tapped(object sender, EventArgs e)
        {
            // When the "Play" button is tapped, we load the GameplayScreen
            if(!usingCustomPicture)
                LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameplayScreen());
            else
                LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameplayScreen(photo));

        }

        void photosButton_Tapped(object sender, EventArgs e)
        {
            // When the "Photos" button is tapped, we load the photoChooserTask
            photoChooserTask.Show();
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                usingCustomPicture = true;
                photo = Texture2D.FromStream(ScreenManager.GraphicsDevice, e.ChosenPhoto);
            }
        }



        void sfxButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;

            // In a real game, you'd want to store away the value of 
            // the button to turn off sounds here. :)
        }

        void musicButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;

            // In a real game, you'd want to store away the value of 
            // the button to turn off music here. :)
        }

        void settingsButton_Tapped(object sender, EventArgs e)
        {
            //BooleanButton button = sender as BooleanButton;

            // In a real game, you'd want to store away the value of 
            // the button to turn off music here. :)

            //LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new SettingsMainMenuScreen());
            ScreenManager.AddScreen(new SettingsMainMenuScreen(), null);
        }

        

        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
            
            base.OnCancel();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            if (usingCustomPicture)
                spriteBatch.Draw(photo, new Rectangle(90, 500, 300, 250), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
