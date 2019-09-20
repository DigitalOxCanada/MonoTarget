using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TexturePackerLoader;

namespace MonoTarget
{
    public class AmmBox : GameObject
    {
        private GameplayScreen _gameplayScreen;
        public Color[] BGMaterialColorData { get; set; }
        private DateTime EndOfLife;
        public RenderTarget2D renderTarget2D;

        private const int TARGETHIT_SCORE = 25;

        public AmmBox(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
        }

        public void Initialize(string tex, float X = 0.0f, float Y = 0.0f)
        {
            base.Initialize(_gameplayScreen.ScreenManager.Game.Content, tex, X, Y);

            //convert the texture into a color map for pixel accuracy hit testing
            BGMaterialColorData = new Color[Texture.Width * Texture.Height];
            Texture.GetData<Color>(BGMaterialColorData);
            renderTarget2D = new RenderTarget2D(_gameplayScreen.ScreenManager.GraphicsDevice, Texture.Width, Texture.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget2D);
            _gameplayScreen.ScreenManager.SpriteBatch.Begin();
            _gameplayScreen.ScreenManager.SpriteBatch.Draw(this.Texture, Vector2.Zero, Color.White);
            _gameplayScreen.ScreenManager.SpriteBatch.End();
            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(null);


            EndOfLife = DateTime.Now.AddSeconds(2.5); //life until target expires

        }




        public override void Update(GameTime gameTime)
        {
            Rectangle r;

            r = new Rectangle((int)Position.X, (int)Position.Y, (int)Texture.Width, (int)Texture.Height);

            //if target expired...
            if (DateTime.Now > EndOfLife)
            {
                IsActive = false;
            }
            else
            {

                //if the mouse is within the image
                if (_gameplayScreen.player.PlayerFired)   //shouldn't check click, should check fired
                {
                    Debug.WriteLine($"Fired at {_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.X}x{_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.Y}");
                    if (r.Contains(_gameplayScreen.ScreenManager.input.CurrentMouseState.Position))
                    {
                        Debug.WriteLine($"Target Rect at {r.X}x{r.Y}:{r.Width}x{r.Height}");
                        if (GetColorAtPos(_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.X - r.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Position.Y - r.Y) != Color.Transparent)
                        {
                            Debug.WriteLine($"Target hit {_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.X}x{_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.Y}");

                            _gameplayScreen.player.TotalAmmo += 10;
                            _gameplayScreen.player.Score += TARGETHIT_SCORE;
                            //todo sound for collecting ammo
                            IsActive = false;

                            Bullet b = _gameplayScreen.BulletManager.bullets[0];
                            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget2D);
                            _gameplayScreen.ScreenManager.SpriteBatch.Begin();
                            _gameplayScreen.ScreenManager.SpriteBatch.Draw(b.Texture, new Vector2(_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.X - r.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Position.Y - r.Y) - b.Origin, Color.White);
                            _gameplayScreen.ScreenManager.SpriteBatch.End();
                            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(null);

                            HitLabel l = _gameplayScreen.HitLabelManager.CreateHitLabel(HitLabelFrames.HitLabel_Gotcha, r.X + (r.Width / 2), r.Y);
                            _gameplayScreen.PointsLabelManager.CreatePointsLabel($"+{TARGETHIT_SCORE}", r.X + (r.Width / 2), r.Y - 25);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Lookup the color value on the 1d array of color data based on x,y position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal Color GetColorAtPos(int x, int y)
        {
            if (y < 0 || x < 0) return Color.TransparentBlack;
            if (BGMaterialColorData == null) return Color.TransparentBlack;
            //formula for 2d coords on a 1d array = y * width + x;
            return BGMaterialColorData[y * Texture.Width + x];
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(renderTarget2D, Position, Color.White);
            }
        }
    }
}

