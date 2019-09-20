#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoTarget;
using TexturePackerLoader;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {

        ContentManager content;
        SpriteFont gameFont;

        public Dictionary<int, List<GameObject>> VisualLayers;
        public List<Target> Targets;

        public HitLabelManager HitLabelManager { get; set; }
        public RicochetManager ricoManager { get; set; }
        public PointsLabelManager PointsLabelManager;
        public SceneManager SceneManager { get; set; }
        public BulletManager BulletManager { get; set; }
        
        public Player player { get; set; }
        private BloodOverlay bloodScreen;

        public HUD HUD { get; set; }

        Random random = new Random();

        float pauseAlpha;




        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            MediaPlayer.Stop();

            VisualLayers = new Dictionary<int, List<GameObject>>();
            for (int i = 0; i < 10; i++)
            {
                VisualLayers[i] = new List<GameObject>();
            }

        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            player = new Player(this);
            HUD = new HUD(this);
            SceneManager = new SceneManager(this);
            BulletManager = new BulletManager(this);
            ricoManager = new RicochetManager(this);
            bloodScreen = new BloodOverlay(this);


            ScreenManager.SoundManager.AddSound("reload-sound", "reload-sound");
            ScreenManager.SoundManager.AddSound("gunempty", "gunempty");
            ScreenManager.SoundManager.AddSound("gunshot1", "gunshot1");
            ScreenManager.SoundManager.AddSound("breakglass1", "breakglass1");
            ScreenManager.SoundManager.AddSound("ricochet1", "shotrico1");
            ScreenManager.SoundManager.AddMusic("song1", "song1");
            ScreenManager.SoundManager.AddSound("thunder1", "thunder1");
            ScreenManager.SoundManager.AddSound("crowcaw1", "crowcaw1");
            ScreenManager.SoundManager.AddSound("catapult", "catapult");
            ScreenManager.SoundManager.AddSound("noammo", "noammo");


            MediaPlayer.IsRepeating = true; //this is for music, not sound effects.
            //SoundManager.SetMasterVolume(0.1f);

            
            SceneManager.Initialize();
            Targets = new List<Target>();

            HUD.Initialize("Playbill");

            string crosshair = "crosshair6";  //default

            if (ScreenManager.PracticeMode)
            {
                SceneManager.currentSceneIndex = 0;
            }
            else
            {
                SceneManager.currentSceneIndex = 1;
                crosshair = $"crosshair{((Game1)ScreenManager.Game).ConfigMGR.gameConfig.Crosshair + 1}";
            }


            SceneManager.LoadContent();

            player.Initialize(content, crosshair, "gunshot1", "gunempty", "reload-sound");
            BulletManager.Initialize(50);
            PointsLabelManager = new PointsLabelManager(this);
            HitLabelManager = new HitLabelManager(this);
            PointsLabelManager.Initialize("Playbill");
            HitLabelManager.Initialize("hitlabels1");
            ricoManager.Initialize("Ricochet");
            bloodScreen.Initialize(content, "bloodscreen");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            //Thread.Sleep(1000);

            //TODO move this into the update which then checks if music is playing or not
            if (((Game1)ScreenManager.Game).ConfigMGR.gameConfig.MusicEnabled)
            {
                //TODO
                MediaPlayer.Play(ScreenManager.SoundManager.MusicLibrary["song1"]);
            }

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            SceneManager.Begin();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }





        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 60, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 60, 0);

            if (IsActive)
            {
                SceneManager.Update();

                //update in reverse order so top objects get interaction first
                for (int layer = VisualLayers.Count-1; layer >= 0; layer--)
                {
                    foreach (var obj in VisualLayers[layer])
                    {
                        obj.Update(gameTime);
                    }
                }

                HUD.Update(gameTime);

                player.Update(gameTime);

                bloodScreen.Update(gameTime);
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null) throw new ArgumentNullException("input");

            player.PlayerFired = false;
            player.PlayerClicked = false;

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];
            MouseState mouseState = input.CurrentMouseState;

            var LeftMouseButtonClicked = false;
            if (input.LastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
            {
                LeftMouseButtonClicked = true;
            }

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {

                //update the aimer location
                player.Position = new Vector2(mouseState.Position.X - player.Texture.Width / 2, mouseState.Position.Y - player.Texture.Height / 2);
                player.PlayerClicked = false;
                player.PlayerFired = false;

                if (ScreenManager.GraphicsDevice.Viewport.Bounds.Contains(mouseState.Position))
                {

                    if (LeftMouseButtonClicked)
                    {
                        player.PlayerClicked = true;

                        if (player.AmmoCount > 0)
                        {
                            player.PlayerFired = true;
                        }

                        if (player.PlayerFired)
                        {
                            player.GunReloading = false;
                            player.AmmoCount--;
                            player.Score--;
                            // Calculate the position the shot was fired based on center to create position audio effect.
                            float pan = 0.0f;
                            //float half = _game.Window.ClientBounds.Width / 2;
                            //float deltax = _game.CurrentMouseState.Position.X - half;

                            //if (deltax < 0)
                            //{
                            //    //mouse is on left half -1 to 0
                            //    pan = Math.Max(-1.0f, deltax / half);
                            //}
                            //else
                            //{
                            //    //mouse is on right half 0 to 1
                            //    pan = Math.Min(1.0f, deltax / half);
                            //}

                            ScreenManager.SoundManager.PlaySound(player._gunshotsound);
                        }
                        else
                        {
                            if (!player.GunReloading)
                            {
                                //GUN is empty so play a gun empty sound here
                                ScreenManager.SoundManager.PlaySound(player._gunemptysound);
                            }
                        }
                    }
                }

            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            //ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
            //                                   Color.CornflowerBlue, 0, 0);

            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteRender spriteRender = ScreenManager.SpriteRender;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);


            for (int layer = 0; layer < 10; layer++)
            {
                //MID LEVEL ARTIFACTS (targets, objects, etc.)
                //foreach (var gameObject in VisualLayers[VisualLayerID.Objects])
                foreach (var gameObject in VisualLayers[layer])
                {
                    if (gameObject.IsAnimated)
                    {
                        gameObject.Draw(spriteRender);
                    }
                    else
                    {
                        gameObject.Draw(spriteBatch);
                    }
                }
            }

            HUD.Draw(spriteBatch);

            ////PLAYER AND AIMER
            player.Draw(spriteBatch);

            bloodScreen.Draw(spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

    }
}
