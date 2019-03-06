using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        //X TODO make animation & spritesheet of barber pole
        //X TODO new bgmaterial color for hit testing for barber pole
        //X TODO speed up barber pole when hit, slowing back to normal speed over 3 seconds.
        //x TODO create layer system
        //x TODO create sound manager
        //x TODO bullet hit marks on everything  
        //x choice: track bullets as sprites and draw them all
        //  or - directly draw the bullet sprite into the texture2d of what it hit. (preferred)


        //TODO spritesheet anim for guy target
        //TODO hit testing for head, body, gun
        //TODO spritesheet death animation

        //TODO bullet ricochet animation

        //TODO onscreen gui (score, ammo, health, etc.)
        //TODO onscreen gui (sound/music toggle)

        //TODO make content manager
        //TODO make screen manager for menu system


        public Dictionary<int, List<GameObject>> VisualLayers;

        public List<Ammo> Ammo;

        public SoundManager SoundManager { get; set; }
        public BulletManager BulletManager { get; set; }
        public GraphicsDeviceManager Graphics { get; set; }

        SpriteRender spriteRender;
        SpriteBatch spriteBatch;
        SpriteSheetLoader spriteSheetLoader;

        public GameConfig Config { get; set; } = new GameConfig();


        private Vector2 centerPosition;

        public Player player { get; set; }
        public Background BackGrnd { get; set; }
        public Target Target1 { get; set; }
        public BarberPole BarbersPole { get; set; }
        public HUD HUD { get; set; }

        //TODO remove this temporary
        Color pixel;

        public MouseState LastMouseState { get; set; }
        public MouseState CurrentMouseState { get; set; }
        public bool LeftMouseButtonClicked;

        public Game1()
        {
            VisualLayers = new Dictionary<int, List<GameObject>>();
            for(int i=0; i<10; i++)
            {
                VisualLayers[i] = new List<GameObject>();
            }
            
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

            Content.RootDirectory = "Content";

            SoundManager = new SoundManager(this);
            BulletManager = new BulletManager(this);

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
        protected override void Initialize()
        {
            centerPosition = new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height / 2);

            
            player = new Player();
            BackGrnd = new Background();
            Target1 = new Target();
            BarbersPole = new BarberPole();
            HUD = new HUD();

            spriteSheetLoader = new SpriteSheetLoader(Content, GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteRender = new SpriteRender(spriteBatch);


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            MediaPlayer.IsRepeating = true; //this is for music, not sound effects.

            HUD.Initialize(this, "Arial");

            BulletManager.Initialize(50);
            SoundManager.SetMasterVolume(0.1f);

            SoundManager.AddSound("reload-sound", "reload-sound");
            SoundManager.AddSound("gunempty", "gunempty");
            SoundManager.AddSound("gunshot1", "gunshot1");
            SoundManager.AddSound("breakglass1", "breakglass1");
            SoundManager.AddSound("ricochet1", "shotrico1");
            SoundManager.AddMusic("song1", "song1");

            BackGrnd.Initialize(this, "bg1", "bg-materials");
            VisualLayers[VisualLayerID.BG].Add(BackGrnd);


            Target1.Initialize(this, "target1", centerPosition.X, centerPosition.Y);
            BarbersPole.Initialize(this, spriteSheetLoader, "barberpole.png");
            VisualLayers[VisualLayerID.Objects].Add(BarbersPole);
            VisualLayers[VisualLayerID.Objects].Add(Target1);

            player.Initialize(this, "crosshair", "gunshot1", "gunempty", "reload-sound");

            //TODO move this into the update which then checks if music is playing or not
            if (Config.MusicEnabled)
            {
                MediaPlayer.Play(SoundManager.MusicLibrary["song1"]);
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
            for (int layer = 0; layer < 10; layer++)
            {
                foreach(var obj in VisualLayers[layer])
                {
                    obj.Update(gameTime);
                }
                //foreach (var bgobject in VisualLayers[VisualLayerID.BG])
                //{
                //    ((Background)bgobject).Update(gameTime);
                //}
                //foreach (var gmObject in VisualLayers[VisualLayerID.Objects])
                //{
                //    gmObject.Update(gameTime);
                //}
            }

            player.Update(gameTime);
            if (player.PlayerFired)
            {
                pixel = BackGrnd.GetColorAtPos(CurrentMouseState.X, CurrentMouseState.Y);
                Debug.WriteLine($"Player clicked on color: {pixel}");

                BackGrnd.CheckHitMaterial(pixel);
            }

            HUD.Update(gameTime);

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

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            for (int layer = 0; layer < 10; layer++)
            {
                //BACKGROUND
                //foreach (var bgObject in VisualLayers[VisualLayerID.BG]) {
                //    bgObject.Draw(spriteBatch);
                //}

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

            //PLAYER AND AIMER
            player.Draw(spriteBatch);

            //ON SCREEN GUI
            //if (pixel != null)
            //{
            //    spriteBatch.DrawString(spriteFont, $"R: {pixel.R} G: {pixel.G} B: {pixel.B}", new Vector2(500, 10), Color.Green);
            //}


            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
