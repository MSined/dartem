#region File Description
//-----------------------------------------------------------------------------
// PhoneEndScreen.cs
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Phone.Tasks;

namespace DartEm
{
    /// <summary>
    /// A basic pause screen for Windows Phone
    /// </summary>
    class PhoneEndScreen : PhoneMenuScreen
    {

        PhotoChooserTask photoChooserTask = new PhotoChooserTask();
        bool usingCustomPicture = false;
        Texture2D photo;

        bool chooserChosen = false;
        bool music;
        bool sfx;
        bool zuneOverride;

        public PhoneEndScreen(bool sfx, bool music, bool zune)
            : base("End Game")
        {
            // Create the "Restart" and "Exit" buttons for the screen

            Button resumeButton = new Button("New Game");
            resumeButton.Tapped += restartButton_Tapped;
            MenuButtons.Add(resumeButton);

            // Create Photo button
            Button photosButton = new Button("Use Custom Photo");
            photosButton.Tapped += photosButton_Tapped;
            //ScreenManager.AddScreen(new PhoneSettingsMainMenuScreen(), );
            MenuButtons.Add(photosButton);

            Button exitButton = new Button("Exit");
            exitButton.Tapped += exitButton_Tapped;
            MenuButtons.Add(exitButton);

            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            this.sfx = sfx;
            this.music = music;
            zuneOverride = zune;
        }

        public PhoneEndScreen(Texture2D input, bool sfx, bool music, bool zune)
            : base("End Game")
        {
            // Create the "Restart" and "Exit" buttons for the screen

            Button resumeButton = new Button("New Game");
            resumeButton.Tapped += restartButton_Tapped;
            MenuButtons.Add(resumeButton);

            // Create Photo button
            Button photosButton = new Button("Use Custom Photo");
            photosButton.Tapped += photosButton_Tapped;
            //ScreenManager.AddScreen(new PhoneSettingsMainMenuScreen(), );
            MenuButtons.Add(photosButton);

            Button exitButton = new Button("Exit");
            exitButton.Tapped += exitButton_Tapped;
            MenuButtons.Add(exitButton);

            photo = input;
            usingCustomPicture = true;

            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            this.sfx = sfx;
            this.music = music;
            zuneOverride = zune;
        }

        /// <summary>
        /// The "Resume" button handler just calls the OnCancel method so that 
        /// pressing the "Resume" button is the same as pressing the hardware back button.
        /// </summary>
        void restartButton_Tapped(object sender, EventArgs e)
        {
            //OnCancel();
            if (!usingCustomPicture)
                LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameplayScreen(sfx, music, zuneOverride));
            else
                LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameplayScreen(photo, sfx, music, zuneOverride));
        }

        /// <summary>
        /// The "Exit" button handler uses the LoadingScreen to take the user out to the main menu.
        /// </summary>
        void exitButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
        }

        void photosButton_Tapped(object sender, EventArgs e)
        {
            // When the "Photos" button is tapped, we load the photoChooserTask
            if (!chooserChosen)
                photoChooserTask.Show();
            chooserChosen = true;
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                chooserChosen = false;
                usingCustomPicture = true;
                photo = Texture2D.FromStream(ScreenManager.GraphicsDevice, e.ChosenPhoto);
            }
        }


        protected override void OnCancel()
        {
            //ExitScreen();
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
            base.OnCancel();

        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            if (!photo.IsDisposed)
            {
                spriteBatch.Begin();

                if (usingCustomPicture)
                {
                    spriteBatch.Draw(photo, new Rectangle(90, 500, 300, 250), Color.White);
                }

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
