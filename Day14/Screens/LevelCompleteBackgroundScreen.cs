using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MonoTarget.Screens
{
    class LevelCompleteBackgroundScreen : MenuScreen
    {

        ContentManager content;
        Texture2D backgroundTexture;
        Random random = new Random();
        


        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelCompleteBackgroundScreen() : base("")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.25);
            TransitionOffTime = TimeSpan.FromSeconds(2);

            MenuEntry continueMenuEntry = new MenuEntry("continue");

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


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("levelcompletescreen");
            ScreenManager.SoundManager.AddMusic("levelcompletemusic", "levelcompletemusic");
            MediaPlayer.IsRepeating = false;
            ScreenManager.SoundManager.PlayMusic("levelcompletemusic");

            //if we just completed level and we are doing practice mode then update our config so we can now play the main levels
            if (ScreenManager.PracticeMode)
            {
                ((Game1)ScreenManager.Game).ConfigMGR.gameConfig.PracticeCompleted = true;
                ((Game1)ScreenManager.Game).ConfigMGR.Save();
            }
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }



        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteFont font = ScreenManager.Font;
            Vector2 offset = new Vector2(0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateTranslation(offset.X, offset.Y, 0));

            Vector2 textPosition = new Vector2(viewport.Width / 2, viewport.Height / 2  );

            spriteBatch.DrawString(font, $"******" , textPosition, Color.White);

            spriteBatch.Draw(backgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
