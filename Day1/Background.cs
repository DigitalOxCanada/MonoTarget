using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoTarget
{
    public class Background
    {
        public Background()
        {

        }

        public Texture2D bgimage { get; set; }

        public void Initialize(Texture2D tex)
        {
            bgimage = tex;
        }

        public void Update(GameTime gameTime)
        {
            //TODO check if the mouse has moved to offset maybe?
            

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgimage, new Vector2(0, 0), Color.White);
        }
    }
}
