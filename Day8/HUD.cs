using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTarget
{
    public class HUD
    {
        private Game1 _game;
        //HUD items
        //1- health
        //2- ammo
        //3- reload
        //4- menu items
        Texture2D AmmoTexture;
        Texture2D HealthTexture;
        Texture2D ReloadButtonTexture;
        SpriteFont spriteFont;
        string FPS;

        private FrameCounter _frameCounter = new FrameCounter();


        public HUD()
        {

        }

        public void Initialize(Game1 game, string font)
        {
            _game = game;
            AmmoTexture = _game.Content.Load<Texture2D>("ammo");
            HealthTexture = _game.Content.Load<Texture2D>("heart");
            ReloadButtonTexture = _game.Content.Load<Texture2D>("reload-button");
            spriteFont = _game.Content.Load<SpriteFont>(font);

        }

        public void Update(GameTime gameTime)
        {
            //if the player is out of ammo...
            if (_game.player.AmmoCount < 1)
            {
                //did the player click on the reload button
                if (_game.player.PlayerClicked && _game.CurrentMouseState.Position.X >= 500 && _game.CurrentMouseState.Position.Y >= 680)
                {
                    _game.player.GunReloading = true;
                }
            }
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            FPS = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            //DRAW HEALTH
            int x = 800;
            int y = 680;
            for (int i = 0; i < _game.player.HealthCount; i++)
            {
                spriteBatch.Draw(HealthTexture, new Vector2(x, y), Color.White);
                x += 64;
            }

            if (_game.player.AmmoCount > 0)
            {
                //DRAW AMMO
                int x2 = 500;
                int y2 = 680;
                for (int i = 0; i < _game.player.AmmoCount; i++)
                {
                    spriteBatch.Draw(AmmoTexture, new Vector2(x2, y2), Color.White);
                    x2 += 32;
                }
            }
            else
            {
                //draw reload button
                spriteBatch.Draw(ReloadButtonTexture, new Vector2(500, 680), Color.White);
            }

            spriteBatch.DrawString(spriteFont, FPS, new Vector2(1, 1), Color.Red);
            spriteBatch.DrawString(spriteFont, $"Score: {_game.player.Score}", new Vector2(600, 10), Color.Green);
        }


    }
}
