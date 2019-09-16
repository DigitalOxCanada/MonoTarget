using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTarget
{
    public class BloodOverlay : GameObject
    {
        private GameplayScreen _gameplayScreen;

        public BloodOverlay(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
        }

        public override void Initialize(ContentManager content, string TextureName = null, float x = 0, float y = 0)
        {
            base.Initialize(content, TextureName, x, y);

            Position = Vector2.Zero;
            Origin = Vector2.Zero;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float factor = Math.Abs(1.0f - ((float)(_gameplayScreen.player.HealthCount)/_gameplayScreen.player.TotalHealth));
            spriteBatch.Draw(Texture, new Rectangle(0,0,_gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Width, _gameplayScreen.ScreenManager.GraphicsDevice.Viewport.Height), Color.White * factor);
        }
    }
}
