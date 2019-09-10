using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        public Vector2 Position;
        public Vector2 Origin;
        public Vector2 Size;
        public float Scale;

        public GameObject()
        {
            IsAnimated = false;
            IsActive = false;
        }

        public virtual void Initialize(ContentManager content, string TextureName = null, float x = 0.0f, float y = 0.0f)
        {
            Scale = 1.0f;
            Rotation = 0.0f;
            Color = Color.White;
            Position = new Vector2(x, y);
            if (TextureName != null)
            {
                Texture = content.Load<Texture2D>(TextureName);
                Size = new Vector2(Texture.Width, Texture.Height);
                Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            }
            IsActive = true;
        }

        public virtual void Update(GameTime gametime)
        {

        }

        public virtual void Draw(GameTime gametime)
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
