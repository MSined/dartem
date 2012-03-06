#region File Description
//-----------------------------------------------------------------------------
// PhoneEndScreen.cs
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
namespace DartEm
{
    /// <summary>
    /// A basic pause screen for Windows Phone
    /// </summary>
    class PhoneConfirmScreen : PhoneMenuScreen
    {

        string saveFileName = "HighScore";
        static DataSaver<int> MyDataSaver = new DataSaver<int>();

        public PhoneConfirmScreen()
            : base("Are you sure?")
        {
            // Create the "Restart" and "Exit" buttons for the screen

            Button yesButton = new Button("Yes");
            yesButton.Tapped += yesButton_Tapped;
            MenuButtons.Add(yesButton);

            // Create Photo button
            Button noButton = new Button("No");
            noButton.Tapped += noButton_Tapped;
            //ScreenManager.AddScreen(new PhoneSettingsMainMenuScreen(), );
            MenuButtons.Add(noButton);


        }

        void yesButton_Tapped(object sender, EventArgs e)
        {
            MyDataSaver.SaveMyData(0, saveFileName);
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
            base.OnCancel();
        }

        void noButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
            base.OnCancel();
        }


        protected override void OnCancel()
        {
            //ExitScreen();
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
            base.OnCancel();

        }
    }
}
