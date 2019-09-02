using GameStateManagement;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTarget
{
    
    public class BulletManager
    {
        private GameplayScreen _gameplayScreen;
        public List<Bullet> bullets;
        Random random = new Random();

        public BulletManager(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
            bullets = new List<Bullet>();
        }

        public Bullet MakeNewBullet(Vector2 position)
        {
            foreach (var b in bullets)
            {
                //we found an available bullet object to reuse
                if (!b.IsActive)
                {
                    b.Position = new Vector2 { X = _gameplayScreen.CurrentMouseState.X - b.Texture.Width / 2, Y = _gameplayScreen.CurrentMouseState.Y - b.Texture.Height / 2 };
                    b.IsActive = true;
                    return b;
                }
            }

            return null;
        }

        internal void Initialize(int numberOfBullets)
        {
            for (int i = 0; i < numberOfBullets; i++)
            {
                Bullet bullet = new Bullet();
                bullet.Initialize(_gameplayScreen, "bullet");
                bullet.IsActive = false;
                bullets.Add(bullet);
                _gameplayScreen.VisualLayers[VisualLayerID.Objects].Add(bullet);
            }

        }

        internal Bullet ActivateBullet(int x, int y)
        {
            foreach (var b in bullets)
            {
                //we found an available bullet object to reuse
                if (!b.IsActive)
                {
                    b.Position = new Vector2 { X = x, Y = y };
                    b.Rotation = MathHelper.ToRadians(random.Next(359));

                    b.IsActive = true;
                    return b;
//                    break;
                }
            }
            return null;
        }
    }
}
