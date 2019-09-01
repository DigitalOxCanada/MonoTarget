using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoTarget
{
    public class Player : GameObject
    {
        public bool PlayerClicked { get; set; }
        public bool PlayerFired { get; set; }
        public bool GunReloading { get; set; }
        public int AmmoCount { get; set; }
        public int HealthCount { get; set; }
        public int Score { get; set; }

        public Player()
        {
            PlayerClicked = false;
            PlayerFired = false;
            GunReloading = false;
            Score = 1000;
        }

        public string _gunshotsound;
        private string _gunreloadsound;
        private string _gunemptysound;

        public void Initialize(Game1 game, string textureName, string gunshot = null, string gunempty = null, string gunreload = null)
        {
            base.Initialize(game, textureName);
            AmmoCount = 6;
            HealthCount = 3;

            _gunshotsound = gunshot;
            _gunemptysound = gunempty;
            _gunreloadsound = gunreload;
        }

        public override void Update(GameTime gameTime)
        {
            PlayerFired = false;
            PlayerClicked = false;
            Position.X = _game.CurrentMouseState.Position.X - Texture.Width/2;
            Position.Y = _game.CurrentMouseState.Position.Y - Texture.Height/2;
            if (_game.CurrentMouseState.Position.X > -1 
                && _game.CurrentMouseState.Position.Y > -1 
                && _game.CurrentMouseState.Position.X < _game.Graphics.PreferredBackBufferWidth 
                && _game.CurrentMouseState.Position.Y < _game.Graphics.PreferredBackBufferHeight)
            {
                if(_game.LeftMouseButtonClicked)
                {
                    PlayerClicked = true;

                    //first lets check our ammo
                    if (AmmoCount > 0)
                    {
                        PlayerFired = true;
                        AmmoCount--;
                        Score--;
                        // Calculate the position the shot was fired based on center to create position audio effect.
                        float pan = 0.0f;
                        float half = _game.Window.ClientBounds.Width / 2;
                        float deltax = _game.CurrentMouseState.Position.X - half;

                        if (deltax < 0)
                        {
                            //mouse is on left half -1 to 0
                            pan = Math.Max(-1.0f, deltax / half);
                        }
                        else
                        {
                            //mouse is on right half 0 to 1
                            pan = Math.Min(1.0f, deltax / half);
                        }

                        this._game.SoundManager.PlaySound(_gunshotsound);
                    }
                    else
                    {
                        if (!GunReloading)
                        {
                            //GUN is empty so play a gun empty sound here
                            this._game.SoundManager.PlaySound(_gunemptysound);
                        }
                    }
                }
            }
        }

        internal void ReloadGun()
        {
            GunReloading = true;
            Score -= 5;
            AmmoCount = 6;
            _game.SoundManager.PlaySound(_gunreloadsound);
            GunReloading = false;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (PlayerClicked)
            {
                spriteBatch.Draw(Texture, Position, Color.Red);
            }
            else
            {
                spriteBatch.Draw(Texture, Position, Color);
            }

        }
    }
}
