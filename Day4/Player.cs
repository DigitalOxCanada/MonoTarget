using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoTarget
{
    public class Player
    {
        Game1 _game;
        public bool PlayerClicked { get; set; }

        public Player()
        {
            PlayerClicked = false;
        }

        Texture2D image;
        Vector2 pos;

        public SoundEffect GunSFX;
        SoundEffectInstance instance;

        public void Initialize(Texture2D tex, Game1 game)
        {
            _game = game;
            instance = GunSFX.CreateInstance();
            instance.IsLooped = false;

            image = tex;
            pos = new Vector2(0, 0);
        }

        public void Update(GameTime gameTime)
        {
            PlayerClicked = false;
            pos.X = _game.CurrentMouseState.Position.X - image.Width/2;
            pos.Y = _game.CurrentMouseState.Position.Y - image.Height/2;
            if (_game.CurrentMouseState.Position.X > -1 
                && _game.CurrentMouseState.Position.Y > -1 
                && _game.CurrentMouseState.Position.X < _game.Graphics.PreferredBackBufferWidth 
                && _game.CurrentMouseState.Position.Y < _game.Graphics.PreferredBackBufferHeight)
            {
                if(_game.LeftMouseButtonClicked)
                {
                    PlayerClicked = true;

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

                    GunSFX.Play(1.0f, 0.0f, pan); // 0.1f, 0.0f, 0.0f);

                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (PlayerClicked)
            {
                spriteBatch.Draw(image, pos, Color.Green);
            }
            else
            {
                spriteBatch.Draw(image, pos, Color.White);
            }

        }
    }
}
