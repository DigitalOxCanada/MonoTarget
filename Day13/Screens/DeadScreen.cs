using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTarget.Screens
{
    class DeadScreen : MenuScreen
    {

        ContentManager content;
        Texture2D backgroundTexture;
        Random random = new Random();



        public DeadScreen() : base("")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            MenuEntry continueMenuEntry = new MenuEntry("continue");

            //if (!((Game1)screenManager.Game).Config.PracticeCompleted)
            //{
            //    playGameMenuEntry.IsDisabled = true;
            //}

            base.selectedEntry = 0;

            // Hook up menu event handlers.
            continueMenuEntry.Selected += continueMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(continueMenuEntry);
        }

        void continueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.SoundManager.StopAllSounds();
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen(), new MainMenuScreen(ScreenManager));
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("dead");
            ScreenManager.SoundManager.AddMusic("deadmusic", "deadmusic");
            ScreenManager.SoundManager.PlayMusic("deadmusic");
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Vector2 offset = new Vector2(0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
