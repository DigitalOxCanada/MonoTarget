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
        private Game1 _game;
        public List<Bullet> bullets;

        public BulletManager(Game1 game)
        {
            _game = game;
            bullets = new List<Bullet>();
        }

        public Bullet MakeNewBullet(Vector2 position)
        {
            foreach (var b in bullets)
            {
                //we found an available bullet object to reuse
                if (!b.IsActive)
                {
                    b.Position = new Vector2 { X = _game.CurrentMouseState.X - b.Texture.Width / 2, Y = _game.CurrentMouseState.Y - b.Texture.Height / 2 };
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
                bullet.Initialize(_game, "bullet");
                bullet.IsActive = false;
                bullets.Add(bullet);
                _game.VisualLayers[VisualLayerID.Objects].Add(bullet);
            }

        }

        internal Bullet ActivateBullet(int x, int y)
        {
            Random r = new Random();
            foreach (var b in bullets)
            {
                //we found an available bullet object to reuse
                if (!b.IsActive)
                {
                    b.Position = new Vector2 { X = x, Y = y };
                    b.Rotation = MathHelper.ToRadians(r.Next(359));

                    b.IsActive = true;
                    return b;
//                    break;
                }
            }
            return null;
        }
    }
}
