using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoTarget
{
    public class Player : GameObject
    {
        private GameplayScreen _gameplayScreen;

        public bool PlayerClicked { get; set; }
        public bool PlayerFired { get; set; }
        public bool GunReloading { get; set; }
        public int TotalAmmo { get; set; }
        public int AmmoCount { get; set; }
        public int HealthCount { get; set; }
        public int TotalHealth { get; set; }
        public int Score { get; set; }


        public Player(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
            PlayerClicked = false;
            PlayerFired = false;
            GunReloading = false;
        }

        public string _gunshotsound;
        private string _gunreloadsound;
        private string _gunemptysound;

        public void Initialize(ContentManager content, string textureName, string gunshot = null, string gunempty = null, string gunreload = null)
        {
            base.Initialize(content, textureName);
            TotalAmmo = 10;
            AmmoCount = 6;
            TotalHealth = 10;
            HealthCount = TotalHealth;
            Score = 0;

            _gunshotsound = gunshot;
            _gunemptysound = gunempty;
            _gunreloadsound = gunreload;

        }

        public override void Update(GameTime gameTime)
        {
            if(PlayerClicked)
            {
                if (PlayerFired)
                {
                    GunReloading = false;
                    AmmoCount--;
                    Score--;
                    // Calculate the position the shot was fired based on center to create position audio effect.
                    float pan = 0.0f;
                    //float half = _game.Window.ClientBounds.Width / 2;
                    //float deltax = _game.CurrentMouseState.Position.X - half;

                    //if (deltax < 0)
                    //{
                    //    //mouse is on left half -1 to 0
                    //    pan = Math.Max(-1.0f, deltax / half);
                    //}
                    //else
                    //{
                    //    //mouse is on right half 0 to 1
                    //    pan = Math.Min(1.0f, deltax / half);
                    //}

                    _gameplayScreen.ScreenManager.SoundManager.PlaySound(_gunshotsound);
                }
                else
                {
                    if (!GunReloading)
                    {
                        //GUN is empty so play a gun empty sound here
                        _gameplayScreen.ScreenManager.SoundManager.PlaySound(_gunemptysound);
                    }
                }

            }
        }

        internal void ReloadGun()
        {
            if (TotalAmmo > 0)
            {
                GunReloading = true;
                Score -= 5;
                var inc = Math.Min(6, TotalAmmo);
                AmmoCount = inc;
                TotalAmmo = Math.Max(0, TotalAmmo - 6);
                _gameplayScreen.ScreenManager.SoundManager.PlaySound(_gunreloadsound);
                //GunReloading = false;
            }
            else
            {
                GunReloading = true;
                _gameplayScreen.ScreenManager.SoundManager.PlaySound("noammo");
            }
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
