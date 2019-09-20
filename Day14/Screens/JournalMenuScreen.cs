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
    class JournalMenuScreen : MenuScreen
    {
        ContentManager content;
        Texture2D backgroundTexture;

        public DateTime ExpireTime { get; set; }
        Microsoft.Xna.Framework.Audio.SoundEffectInstance speech;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public JournalMenuScreen() : base("")
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.5);

            // Create our menu entries.
            MenuEntry skipMenuEntry = new MenuEntry("skip");

            base.selectedEntry = 0;

            // Hook up menu event handlers.
            skipMenuEntry.Selected += continueMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(skipMenuEntry);


#if DEBUG
            ExpireTime = DateTime.Now.AddSeconds(1.0);
#else
            ExpireTime = DateTime.Now.AddSeconds(5.0);
#endif

        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("journal1bg");
            ScreenManager.SoundManager.AddSound("journal1Speech", "journal1Speech");
            speech = ScreenManager.SoundManager.PlaySound("journal1Speech");
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void continueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.SoundManager.StopAllSounds();
            LoadingScreen.Load(ScreenManager, true, 0, new GameplayScreen());
        }

        //public override void HandleInput(InputState input)
        //{
        //    //CurrentMouseState = Mouse.GetState(ScreenManager.Game.Window);
        //    //LeftMouseButtonClicked = false;

        //    //if (!IsExiting && DateTime.Now > ExpireTime)
        //    //{
        //    //    if (LastMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed)
        //    //    {
        //    //        LeftMouseButtonClicked = true;
        //    //        ExitScreen();
        //    //    }
        //    //}

        //}

        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!IsExiting && DateTime.Now > ExpireTime)
            {
                if (speech.State != Microsoft.Xna.Framework.Audio.SoundState.Playing)
                {
                    ExpireTime = DateTime.MaxValue;
                    ExitScreen();
                }
            }

            if (IsExiting)
            {
                speech.Volume = TransitionAlpha;
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.End();

            if (DateTime.Now > ExpireTime)
            {
                base.Draw(gameTime);
            }

            spriteBatch.Begin();
            spriteBatch.Draw(ScreenManager.cursorTexture, new Vector2(ScreenManager.input.CurrentMouseState.Position.X, ScreenManager.input.CurrentMouseState.Position.Y), Color.White);
            spriteBatch.End();

        }

    }
}



