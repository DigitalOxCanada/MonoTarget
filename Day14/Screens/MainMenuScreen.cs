#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoTarget;
using MonoTarget.Screens;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen(ScreenManager screenManager) : base("")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            
            if (!((Game1)screenManager.Game).ConfigMGR.gameConfig.PracticeCompleted)
            {
                playGameMenuEntry.IsDisabled = true;
            }

            base.selectedEntry = 1;
            MenuEntry practiceGameMenuEntry = new MenuEntry("Practice");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            practiceGameMenuEntry.Selected += PracticeGameMenuEntry_Selected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(practiceGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);

        }


        private void PracticeGameMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.SoundManager.StopAllSounds();
            ScreenManager.PracticeMode = true;
            ScreenManager.SoundManager.PlaySound("MenuMove");
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new GameplayScreen());
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if(((MenuEntry)sender).IsDisabled)
            {

            }
            else
            {
                ScreenManager.PracticeMode = false;
                ScreenManager.SoundManager.PlaySound("MenuMove");
                ScreenManager.AddScreen(new StartGameOptionsScreen(), e.PlayerIndex);
                //ScreenManager.SoundManager.StopAllSounds();
                //LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new JournalMenuScreen()); // GameplayScreen());
            }
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.SoundManager.PlaySound("MenuMove");
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.SoundManager.PlaySound("MenuMove");
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        public override void HandleInput(InputState input)
        {
            SpriteFont font = ScreenManager.Font;

            foreach (var m in MenuEntries)
            {
                Vector2 textSize = font.MeasureString(m.Text);

                Rectangle mRect = new Rectangle((int)m.Position.X, (int)m.Position.Y - (int)textSize.Y/2, (int)textSize.X, (int)textSize.Y);
                if(mRect.Contains(input.CurrentMouseState.Position))
                {
                    //mouse is in rect of a menu option
                    base.ChangeSelectionByMenuEntry(m);
                    break;
                }
            }

            base.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();
            spriteBatch.Draw(ScreenManager.cursorTexture, new Vector2(ScreenManager.input.CurrentMouseState.Position.X, ScreenManager.input.CurrentMouseState.Position.Y), Color.White);
            spriteBatch.End();
        }

    }
}
