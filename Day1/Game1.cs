using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace MonoTarget
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics { get; set; }
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        private Vector2 centerPosition;
        Player player;
        Background bg;
        private FrameCounter _frameCounter = new FrameCounter();
        Color[] colorData;
        Color pixel;
        Song song;
        public MouseState lastMouseState { get; set; }
        public MouseState currentMouseState { get; set; }


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 768;
            this.Window.AllowAltF4 = true;
            this.Window.AllowUserResizing = true;
            //this.Window.IsBorderless = true;
            Window.Title = "Mono Target";
            Window.ClientSizeChanged += ClientChangedWindowSize;
            
            this.Exiting += Game1_Exiting;

            Content.RootDirectory = "Content";
        }

        private void Game1_Exiting(object sender, EventArgs e)
        {
            MediaPlayer.Stop();
        }

        private void ClientChangedWindowSize(object sender, EventArgs e)
        {
            if (GraphicsDevice.Viewport.Width != graphics.PreferredBackBufferWidth ||
                GraphicsDevice.Viewport.Height != graphics.PreferredBackBufferHeight)
            {
                if (Window.ClientBounds.Width == 0) return;
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.ApplyChanges();

                //screenManager.UpdateResolution();

            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            centerPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);

            player = new Player();
            bg = new Background();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            spriteFont = Content.Load<SpriteFont>("Arial");
            var bgTexture = Content.Load<Texture2D>("bg1");
            var playerTexture = Content.Load<Texture2D>("crosshair");
            song = Content.Load<Song>("song1");
            MediaPlayer.IsRepeating = true;
            var gunShot = Content.Load<SoundEffect>("gunshot1");
            player.gunShot = gunShot;

            bg.Initialize(bgTexture);
            player.Initialize(playerTexture);


            colorData = new Color[bgTexture.Width * bgTexture.Height];
            bgTexture.GetData<Color>(colorData);

            MediaPlayer.Play(song);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //TODO code that checks mouse is in boundary to be used globally
            //TODO code that checks if a mouse click and release has occurred to be used globally

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            currentMouseState = Mouse.GetState(this.Window);

            
            System.Diagnostics.Debug.WriteLine(currentMouseState.Position.X.ToString() + "," + currentMouseState.Position.Y.ToString());

            bg.Update(gameTime);
            
            // Check if the mouse position is inside the rectangle
            if (currentMouseState.Position.X >-1 && currentMouseState.Position.Y >-1 && currentMouseState.Position.X < graphics.PreferredBackBufferWidth && currentMouseState.Position.Y < graphics.PreferredBackBufferHeight)
                        //if (state.X > -1 && state.Y > -1 && state.X * bg.bgimage.Height + state.Y<colorData.Length)
            {
                pixel = colorData[currentMouseState.Position.Y * bg.bgimage.Width + currentMouseState.Position.X];
            }

            player.Update(gameTime, this);

            base.Update(gameTime);
            lastMouseState = currentMouseState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                bg.Draw(spriteBatch);
            player.Draw(spriteBatch);

            spriteBatch.DrawString(spriteFont, fps, new Vector2(1, 1), Color.Red);
            if (pixel != null)
            {
                spriteBatch.DrawString(spriteFont, $"R: {pixel.R} G: {pixel.G} B: {pixel.B}", new Vector2(500, 10), Color.Green);
            }


            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
