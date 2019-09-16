using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TexturePackerLoader;

namespace MonoTarget
{

    /// <summary>
    /// 
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager Graphics { get; set; }
        public ScreenManager screenManager;

        public GameConfig Config { get; set; } = new GameConfig();

//        public Vector2 centerPosition { get; set; }


#if ZUNE
        int BufferWidth = 272;
        int BufferHeight = 480;
#elif IPHONE
        int BufferWidth = 320;
        int BufferHeight = 480;
#else
        public int BufferWidth = 1280;
        public int BufferHeight = 768;
#endif

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = BufferWidth,
                PreferredBackBufferHeight = BufferHeight
            };
            Window.AllowAltF4 = true;
            Window.AllowUserResizing = false;
            Window.Title = "Mono Target";
            Window.ClientSizeChanged += ClientChangedWindowSize;
            //this.Window.IsBorderless = true;

            Content.RootDirectory = "Content";

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            screenManager.AddScreen(new IntroCompanyScreen(), null);
            string gameconfigfn = "gameconfig.json";
            if (!System.IO.File.Exists(gameconfigfn))
            {
                System.IO.File.WriteAllText(gameconfigfn, JsonConvert.SerializeObject(Config));
            }
            else
            {
                string cfgjson = System.IO.File.ReadAllText(gameconfigfn);
                Config = JsonConvert.DeserializeObject<GameConfig>(cfgjson);
            }

            Exiting += Game1_Exiting;
        }

        private void Game1_Exiting(object sender, EventArgs e)
        {
            MediaPlayer.Stop();
        }

        private void ClientChangedWindowSize(object sender, EventArgs e)
        {
            if (GraphicsDevice.Viewport.Width != Graphics.PreferredBackBufferWidth ||
                GraphicsDevice.Viewport.Height != Graphics.PreferredBackBufferHeight)
            {
                if (Window.ClientBounds.Width == 0)
                {
                    return;
                }

                Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                Graphics.ApplyChanges();

                //screenManager.UpdateResolution();

            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        //protected override void Initialize()
        //{
        //    //centerPosition = new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height / 2);

        //    base.Initialize();
        //}

        ///// <summary>
        ///// LoadContent will be called once per game and is the place to load
        ///// all of your content.
        ///// </summary>
        //protected override void LoadContent()
        //{
        //}

        ///// <summary>
        ///// UnloadContent will be called once per game and is the place to unload
        ///// game-specific content.
        ///// </summary>
        //protected override void UnloadContent()
        //{
        //    // TODO: Unload any non ContentManager content here

        //}

        ///// <summary>
        ///// Allows the game to run logic such as updating the world,
        ///// checking for collisions, gathering input, and playing audio.
        ///// </summary>
        ///// <param name="gameTime">Provides a snapshot of timing values.</param>
        //protected override void Update(GameTime gameTime)
        //{
        //    //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        //    //{
        //    //    Exit();
        //    //}

        //}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }
    }
}
