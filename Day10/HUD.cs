using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTarget
{
    public class HUD : GameComponent
    {
        //private Game1 _game;
        //HUD items
        //1- health
        //2- ammo
        //3- reload
        //4- menu items
        Texture2D AmmoTexture;
        Texture2D HealthTexture;
        Texture2D ReloadButtonTexture;
        SpriteFont spriteFont;
        string FPS = "?";

        private FrameCounter _frameCounter = new FrameCounter();
        private GameplayScreen _gameplayScreen;

        public Vector2 ReloadbtnPosition { get; private set; }

        public HUD(GameplayScreen gameplayScreen)
            :base(gameplayScreen.ScreenManager.Game)
        {
            _gameplayScreen = gameplayScreen;
        }


        public void Initialize(string font)
        {
            ContentManager content = Game.Content;

            //_game = game;
            AmmoTexture = content.Load<Texture2D>("ammo");
            HealthTexture = content.Load<Texture2D>("heart");
            ReloadButtonTexture = content.Load<Texture2D>("reload-button");
            spriteFont = content.Load<SpriteFont>(font);
            ReloadbtnPosition = new Vector2(500, 680);
        }

        public override void Update(GameTime gameTime)
        {
            //if the player is out of ammo...
            if (_gameplayScreen.player.AmmoCount < 1 && _gameplayScreen.player.PlayerClicked)
            {
                if(new Rectangle(new Point((int)ReloadbtnPosition.X, (int)ReloadbtnPosition.Y), new Point(ReloadButtonTexture.Width, ReloadButtonTexture.Height)).Contains(_gameplayScreen.CurrentMouseState.Position))
                {
                    _gameplayScreen.player.ReloadGun();
                }
            }

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            FPS = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            ////DRAW HEALTH
            int x = 800;
            int y = 680;
            for (int i = 0; i < _gameplayScreen.player.HealthCount; i++)
            {
                spriteBatch.Draw(HealthTexture, new Vector2(x, y), Color.White);
                x += 64;
            }

            if (_gameplayScreen.player.AmmoCount > 0)
            {
                //DRAW AMMO
                int x2 = 500;
                int y2 = 680;
                for (int i = 0; i < _gameplayScreen.player.AmmoCount; i++)
                {
                    spriteBatch.Draw(AmmoTexture, new Vector2(x2, y2), Color.White);
                    x2 += 32;
                }
            }
            else
            {
                //draw reload button
                spriteBatch.Draw(ReloadButtonTexture, ReloadbtnPosition, Color.White);
            }

            spriteBatch.DrawString(spriteFont, FPS, new Vector2(1, 1), Color.Red);
            spriteBatch.DrawString(spriteFont, $"Score: {_gameplayScreen.player.Score}", new Vector2(600, 10), Color.Green);
        }


    }
}
