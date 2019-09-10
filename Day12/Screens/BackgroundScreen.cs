#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoTarget;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D backgroundTexture;
        GameObject oxlogo;
        GameObject title;
        Rectangle titlepos = new Rectangle(-1000, -500, 25, 96);
        private bool shakeViewport = false;
        private double shakeStartAngle = 120;
        private double shakeRadius = 15.0;
        private TimeSpan shakeStart;
        Random random = new Random();

        #endregion


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            oxlogo = new GameObject();
            title = new GameObject();
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
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

            backgroundTexture = content.Load<Texture2D>("titlescreen");
            oxlogo.Initialize(content, "oxlogo");
            title.Initialize(content, "title");
            title.Origin = new Vector2(0, 0);
            ScreenManager.SoundManager.AddMusic("mainmenu", "mainmenu");
            ScreenManager.SoundManager.PlayMusic("mainmenu");
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


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
            Vector2 offset = new Vector2(0, 0);
            if (shakeViewport)
            {
                offset = new Vector2((float)(Math.Sin(shakeStartAngle) * shakeRadius), (float)(Math.Cos(shakeStartAngle) * shakeRadius));
                shakeRadius -= 0.25f;
                shakeStartAngle += (150 + random.Next(60));
                if (gameTime.TotalGameTime.TotalSeconds - shakeStart.TotalSeconds > 2 || shakeRadius <= 0)
                {
                    shakeViewport = false;
                }
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            title.Position.X = MathHelper.Lerp(titlepos.X, titlepos.Width, TransitionAlpha);
            title.Position.Y = MathHelper.Lerp(titlepos.Y, titlepos.Height, TransitionAlpha);
            oxlogo.Scale = MathHelper.Lerp(0.1f, 1.0f, TransitionAlpha);
            oxlogo.Position = new Vector2(fullscreen.Width - (oxlogo.Texture.Width/2) - 25, fullscreen.Height - (oxlogo.Texture.Height/2) - 25);

            //spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateTranslation(offset.X, offset.Y, 0));

            spriteBatch.Draw(backgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.Draw(title.Texture, title.Position, null, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), -0.125f, title.Origin, title.Scale, SpriteEffects.None, 0);
            spriteBatch.Draw(oxlogo.Texture, oxlogo.Position, null, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha), 0.0f, oxlogo.Origin, oxlogo.Scale, SpriteEffects.None, 0);

            spriteBatch.End();
        }


        #endregion
    }
}
