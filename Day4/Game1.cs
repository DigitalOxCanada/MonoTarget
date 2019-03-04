using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TexturePackerLoader;

namespace MonoTarget
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //X TODO make animation & spritesheet of barber pole
        //X TODO new bgmaterial color for hit testing for barber pole
        //X TODO speed up barber pole when hit, slowing back to normal speed over 3 seconds.

        //TODO spritesheet anim for guy target
        //TODO hit testing for head, body, gun
        //TODO spritesheet death animation

        //TODO bullet hit marks on everything  
        //choice: track bullets as sprites and draw them all
        //  or - directly draw the bullet sprite into the texture2d of what it hit. (preferred)

        //TODO bullet ricochet animation

        public BarberPoleObject barberPole = new BarberPoleObject();

        SpriteRender spriteRender;

        public GameConfig Config { get; set; } = new GameConfig();
        public GraphicsDeviceManager Graphics { get; set; }
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        private Vector2 centerPosition;
        public Player player { get; set; }
        public Background BackGrnd { get; set; }

        public Dictionary<string, SoundEffect> SoundLibrary = new Dictionary<string, SoundEffect>();
        public Dictionary<string, Song> MusicLibrary = new Dictionary<string, Song>();

        public Target Target1 { get; set; }
        private FrameCounter _frameCounter = new FrameCounter();
        Color pixel;

        public MouseState LastMouseState { get; set; }
        public MouseState CurrentMouseState { get; set; }
        public bool LeftMouseButtonClicked;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 768
            };
            Window.AllowAltF4 = true;
            Window.AllowUserResizing = false;
            Window.Title = "Mono Target";
            Window.ClientSizeChanged += ClientChangedWindowSize;
            //this.Window.IsBorderless = true;

            Exiting += Game1_Exiting;

            Content.RootDirectory = "Content";
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
        protected override void Initialize()
        {
            centerPosition = new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height / 2);

            player = new Player();
            BackGrnd = new Background();
            Target1 = new Target();

            var spriteSheetLoader = new SpriteSheetLoader(Content, GraphicsDevice);
            barberPole.Initialize(spriteSheetLoader, "barberpole.png");


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
            spriteRender = new SpriteRender(spriteBatch);

            spriteFont = Content.Load<SpriteFont>("Arial");

            var bgTexture = Content.Load<Texture2D>("bg1");
            var playerTexture = Content.Load<Texture2D>("crosshair");
            var targettx = Content.Load<Texture2D>("target1");
            var bgMaterials = Content.Load<Texture2D>("bg-materials");

            MediaPlayer.IsRepeating = true; //this is for music, not sound effects.

            SoundLibrary.Add("gunshot1", Content.Load<SoundEffect>("gunshot1"));
            SoundLibrary.Add("breakglass1", Content.Load<SoundEffect>("breakglass1"));
            SoundLibrary.Add("ricochet1", Content.Load<SoundEffect>("shotrico1"));
            MusicLibrary.Add("song1", Content.Load<Song>("song1"));

            player.GunSFX = SoundLibrary["gunshot1"];

            BackGrnd.Initialize(bgTexture, bgMaterials, this);
            player.Initialize(playerTexture, this);
            Target1.Initialize(targettx, this);

            Target1.CurrentPosition = centerPosition;   //just default to center of screen for testing

            if (Config.MusicEnabled)
            {
                MediaPlayer.Play(MusicLibrary["song1"]);
            }
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            CurrentMouseState = Mouse.GetState(this.Window);
            LeftMouseButtonClicked = false;
            if (LastMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                LeftMouseButtonClicked = true;
            }

            //            Debug.WriteLine(currentMouseState.Position.X.ToString() + "," + currentMouseState.Position.Y.ToString());

            BackGrnd.Update(gameTime);

            //characterAnimationManager.Update(gameTime);
            barberPole.Update(gameTime);

            player.Update(gameTime);
            if (player.PlayerClicked)
            {
                pixel = BackGrnd.GetColorAtPos(CurrentMouseState.X, CurrentMouseState.Y);
                Debug.WriteLine($"Player clicked on color: {pixel}");

                BackGrnd.CheckHitMaterial(pixel);
            }

            Target1.Update(gameTime);

            base.Update(gameTime);
            LastMouseState = CurrentMouseState;
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

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            //BACKGROUND
            BackGrnd.Draw(spriteBatch);

            //MID LEVEL ARTIFACTS (targets, objects, etc.)

            Target1.Draw(spriteBatch);

            barberPole.Draw(spriteRender);

            //PLAYER AND AIMER
            player.Draw(spriteBatch);

            //ON SCREEN GUI
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
