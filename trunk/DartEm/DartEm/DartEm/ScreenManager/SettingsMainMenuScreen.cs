using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DartEm;
using Microsoft.Xna.Framework;

namespace DartEm
{
    class SettingsMainMenuScreen : PhoneMenuScreen
    {
        public SettingsMainMenuScreen()
            : base("Settings Menu")
        {
            // Create two buttons to toggle sound effects and music. This sample just shows one way
            // of making and using these buttons; it doesn't actually have sound effects or music
            BooleanButton sfxButton = new BooleanButton("Sound Effects", true);
            sfxButton.Tapped += sfxButton_Tapped;
            MenuButtons.Add(sfxButton);

            BooleanButton musicButton = new BooleanButton("Music", true);
            musicButton.Tapped += musicButton_Tapped;
            MenuButtons.Add(musicButton);

            // Back button
            Button backButton = new Button("Back");
            backButton.Tapped += backButton_Tapped;
            MenuButtons.Add(backButton);
        }

        void backButton_Tapped(object sender, EventArgs e)
        {
            this.ExitScreen();
            //LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new PhoneMainMenuScreen());
            ScreenManager.AddScreen(new PhoneMainMenuScreen(), null);
        }

        void musicButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;

            // In a real game, you'd want to store away the value of 
            // the button to turn off music here. :)
        }

        void sfxButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;

            // In a real game, you'd want to store away the value of 
            // the button to turn off sounds here. :)
        }

        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
            base.OnCancel();
        }
    }
}
