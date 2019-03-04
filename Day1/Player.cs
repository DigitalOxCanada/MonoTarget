using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoTarget
{
    public class Player
    {
        public Player()
        {

        }

        Texture2D image;
        Vector2 pos;

        public SoundEffect gunShot;
        SoundEffectInstance instance;

        public void Initialize(Texture2D tex)
        {
            instance = gunShot.CreateInstance();
            instance.IsLooped = false;

            image = tex;
            pos = new Vector2(0, 0);
        }

        public void Update(GameTime gameTime, Game1 game)
        {

            pos.X = game.currentMouseState.Position.X - image.Width/2;
            pos.Y = game.currentMouseState.Position.Y - image.Height/2;
            if(game.currentMouseState.Position.X>-1 && game.currentMouseState.Position.X<game.graphics.PreferredBackBufferWidth)
            {
                if (game.lastMouseState.LeftButton == ButtonState.Released && game.currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    // React to the click
                    // ...
                    //clickOccurred = true;
                    float pan = 0.0f;
                    float half = game.Window.ClientBounds.Width / 2;
                    float deltax = game.currentMouseState.Position.X - half;

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

                    gunShot.Play(1.0f, -1.0f, pan); // 0.1f, 0.0f, 0.0f);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //float scale = 2.5f; //50% smaller
            //spriteBatch.Draw(image, pos, null, null, null, 0, new Vector2(scale, scale), Color.White, SpriteEffects.None, 0);
            //if we add scaling then our center calculation needs to adjust

            spriteBatch.Draw(image, pos, Color.White);

        }
    }
}
