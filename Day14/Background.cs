using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MonoTarget
{
    public class Background : GameObject
    {
        private GameplayScreen _gameplayScreen;
        public RenderTarget2D renderTarget2D;

        public Background(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;

            MaterialAction = new Dictionary<Color, Action>
            {
                [Color.White] = () => ShotBreakGlass(),
                [Color.Purple] = () => ShotMissed(),
                [new Color(255, 216, 0, 255)] = () => ShotRicochet(),
                [new Color(72, 0, 255, 255)] = () => ShotBarberPole(),
                [new Color(255, 106, 0, 255)] = () => ShotBuilding(),
                [new Color(64, 64, 64, 255)] = () => ShotGround()
            };
        }

        private void ShotGround()
        {
            Debug.WriteLine("Action: ShotGround");
            var l = _gameplayScreen.PointsLabelManager.CreatePointsLabel("-1", _gameplayScreen.ScreenManager.input.CurrentMouseState.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Y);
            l.Negative();
            //_game.SoundManager.SoundLibrary["ricochet1"].Play(1.0f, 0.0f, 0.0f);
            _gameplayScreen.ScreenManager.SoundManager.PlaySound("ricochet1", 1.0f);
        }

        private void ShotBuilding()
        {
            Debug.WriteLine("Action: ShotBuilding");
            //_gameplayScreen.BulletManager.ActivateBullet(_gameplayScreen.CurrentMouseState.X, _gameplayScreen.CurrentMouseState.Y);
            Bullet b = _gameplayScreen.BulletManager.bullets[0];
            
            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget2D);
            _gameplayScreen.ScreenManager.SpriteBatch.Begin();
            _gameplayScreen.ScreenManager.SpriteBatch.Draw(b.Texture, new Vector2(_gameplayScreen.ScreenManager.input.CurrentMouseState.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Y)-b.Origin, Color.White);
            _gameplayScreen.ScreenManager.SpriteBatch.End();
            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(null);
        }

        private void ShotBarberPole()
        {
//            var l = _game.PointsLabelManager.CreatePointsLabel("+1", _game.CurrentMouseState.X, _game.CurrentMouseState.Y);

            Debug.WriteLine("Action: ShotBarberPole");
            //BarbersPole.Hit = true;
        }

        private void ShotRicochet()
        {
            Debug.WriteLine("Action: ShotRicochet");
            //            _gameplayScreen.BulletManager.ActivateBullet(_gameplayScreen.CurrentMouseState.X, _gameplayScreen.CurrentMouseState.Y);
            Bullet b = _gameplayScreen.BulletManager.bullets[0];

            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget2D);
            _gameplayScreen.ScreenManager.SpriteBatch.Begin();
            _gameplayScreen.ScreenManager.SpriteBatch.Draw(b.Texture, new Vector2(_gameplayScreen.ScreenManager.input.CurrentMouseState.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Y) - b.Origin, Color.White);
            _gameplayScreen.ScreenManager.SpriteBatch.End();
            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(null);

            _gameplayScreen.ricoManager.CreateRicochet(_gameplayScreen.ScreenManager.input.CurrentMouseState.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Y);
            var l = _gameplayScreen.PointsLabelManager.CreatePointsLabel("-1", _gameplayScreen.ScreenManager.input.CurrentMouseState.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Y);
            l.Negative();

            _gameplayScreen.ScreenManager.SoundManager.PlaySound("ricochet1", 1.0f);
        }

        private void ShotMissed()
        {
            Debug.WriteLine("Action: ShotMissed");
        }

        private void ShotBreakGlass()
        {
            Debug.WriteLine("Action: ShotBreakGlass");
            //            _gameplayScreen.BulletManager.ActivateBullet(_gameplayScreen.CurrentMouseState.X, _gameplayScreen.CurrentMouseState.Y);
            Bullet b = _gameplayScreen.BulletManager.bullets[0];

            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget2D);
            _gameplayScreen.ScreenManager.SpriteBatch.Begin();
            _gameplayScreen.ScreenManager.SpriteBatch.Draw(b.Texture, new Vector2(_gameplayScreen.ScreenManager.input.CurrentMouseState.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Y) - b.Origin, Color.White);
            _gameplayScreen.ScreenManager.SpriteBatch.End();
            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(null);

            var l = _gameplayScreen.PointsLabelManager.CreatePointsLabel("-1", _gameplayScreen.ScreenManager.input.CurrentMouseState.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Y);
            l.Negative();
            //play breaking glass sound
            //_game.SoundManager.SoundLibrary["breakglass1"].Play(1.0f, 0.0f, 0.0f);
            _gameplayScreen.ScreenManager.SoundManager.PlaySound("breakglass1", 1.0f);
        }

        public Texture2D BGMaterials { get; set; }
        public Color[] BGMaterialColorData { get; set; }
        public Dictionary<Color, Action> MaterialAction { get; set; }

        public void Initialize(string textureName, string bgMaterials = null)
        {
            base.Initialize(_gameplayScreen.ScreenManager.Game.Content, textureName);
            if (bgMaterials != null)
            {
                BGMaterials = _gameplayScreen.ScreenManager.Game.Content.Load<Texture2D>(bgMaterials);

                //make colour map data of bg materials image
                //? we don't really need the texture anymore
                BGMaterialColorData = new Color[BGMaterials.Width * BGMaterials.Height];
                BGMaterials.GetData<Color>(BGMaterialColorData);
            }

            renderTarget2D = new RenderTarget2D(_gameplayScreen.ScreenManager.GraphicsDevice, Texture.Width, Texture.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget2D);
            _gameplayScreen.ScreenManager.SpriteBatch.Begin();
            _gameplayScreen.ScreenManager.SpriteBatch.Draw(this.Texture, Vector2.Zero, Color.White);
            _gameplayScreen.ScreenManager.SpriteBatch.End();
            _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(null);
        }

        public override void Update(GameTime gameTime)
        {


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(renderTarget2D, Position, Color.White);
        }

        internal Color GetColorAtPos(int x, int y)
        {
            if (y < 0 || x < 0) return Color.Transparent;
            //formula for 2d coords on a 1d array = y * width + x;
            return BGMaterialColorData[y * BGMaterials.Width + x];
        }

        internal void CheckHitMaterial(Color pixel)
        {
            if (MaterialAction.ContainsKey(pixel))
            {
                //we have something defined for that color so call the action
                MaterialAction[pixel]();
            }
        }
    }
}
