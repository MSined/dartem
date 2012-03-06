using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DartEm;
using Microsoft.Xna.Framework;

namespace DartEm
{
    class PhoneSettingsMainMenuScreen : PhoneMenuScreen
    {
        string sfxBoolFilename = "SfxBool";
        string musicBoolFilename = "MusicBool";
        static DataSaver<int> MyDataSaver1 = new DataSaver<int>(); // IMPORTANT: 0 = Not initiated, 1 = true, 2 = false;
        static DataSaver<int> MyDataSaver2 = new DataSaver<int>(); // IMPORTANT: 0 = Not initiated, 1 = true, 2 = false;

        public PhoneSettingsMainMenuScreen()
            : base("Settings Menu")
        {
            // Create two buttons to toggle sound effects and music. This sample just shows one way
            // of making and using these buttons; it doesn't actually have sound effects or music

            int temp1 = MyDataSaver1.LoadMyData(sfxBoolFilename);
            if (temp1 == 0 || temp1 == 1)
            {
                BooleanButton sfxButton = new BooleanButton("Sound Effects", true);
                sfxButton.Tapped += sfxButton_Tapped;
                MenuButtons.Add(sfxButton);
            }
            else
            {
                BooleanButton sfxButton = new BooleanButton("Sound Effects", false);
                sfxButton.Tapped += sfxButton_Tapped;
                MenuButtons.Add(sfxButton);
            }

            int temp2 = MyDataSaver2.LoadMyData(musicBoolFilename);
            if (temp2 == 0 || temp2 == 1)
            {
                BooleanButton musicButton = new BooleanButton("Music", true);
                musicButton.Tapped += musicButton_Tapped;
                MenuButtons.Add(musicButton);            
            }
            else
            {
                BooleanButton musicButton = new BooleanButton("Music", false);
                musicButton.Tapped += musicButton_Tapped;
                MenuButtons.Add(musicButton);
            }

            Button resetHighScore = new Button("Reset High Score");
            resetHighScore.Tapped += resetHighScore_Tapped;
            MenuButtons.Add(resetHighScore);

            // Back button
            Button backButton = new Button("Back");
            backButton.Tapped += backButton_Tapped;
            MenuButtons.Add(backButton);
        }

        void backButton_Tapped(object sender, EventArgs e)
        {
            this.ExitScreen();
            ScreenManager.AddScreen(new PhoneMainMenuScreen(), null);
        }

        void musicButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;
            if (button.getBool())
                MyDataSaver2.SaveMyData(1, musicBoolFilename);
            else
                MyDataSaver2.SaveMyData(2, musicBoolFilename);
        }

        void sfxButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;
            if (button.getBool())
                MyDataSaver1.SaveMyData(1, sfxBoolFilename);   
            else
                MyDataSaver1.SaveMyData(2, sfxBoolFilename);   
        }

        void resetHighScore_Tapped(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new PhoneConfirmScreen(), ControllingPlayer);
        }



        protected override void OnCancel()
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
            base.OnCancel();
        }
    }
}
