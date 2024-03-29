﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TexturePackerLoader;

namespace MonoTarget
{
    public class GameObject
    {
        public float Rotation;
        public bool IsActive;
        public bool IsAnimated;
        public Color Color;
        public Texture2D Texture;
        public Game1 _game;
        public Vector2 Position;
        public Vector2 Origin;

        public GameObject()
        {
            IsAnimated = false;
            IsActive = false;
        }

        public virtual void Initialize(Game1 game, string TextureName = "imagemissing", float x = 0.0f, float y = 0.0f)
        {
            _game = game;
            Rotation = 0.0f;
            Color = Color.White;
            Position = new Vector2(x, y);
            Texture = _game.Content.Load<Texture2D>(TextureName);
            IsActive = true;
        }

        public virtual void Update(GameTime gametime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public virtual void Draw(SpriteRender spriteRender)
        {

        }
    }
}
