﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTarget
{
    public class Bullet : GameObject
    {
        public Bullet()
        {

        }

        public void Initialize(Game1 game, string texture)
        {
            base.Initialize(game, texture);
            Origin = new Vector2(this.Texture.Width / 2, this.Texture.Height / 2);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                //spriteBatch.Draw(Texture, Position, Color.White);
                spriteBatch.Draw(Texture, Position, null, null, Origin, Rotation, null, Color.White);
            }
            //spriteBatch.Draw(Texture, 
            //    new Rectangle( new Point() { X = (int)Position.X, Y = (int)Position.Y },
            //    new Point() { X = 20, Y = 20 }), 
            //    Color.White);
        }
    }
}
