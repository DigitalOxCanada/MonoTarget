using Microsoft.Xna.Framework;

namespace MonoTarget
{
    public class BackgroundHitObject : GameObject
    {

        public virtual Color GetColorAtPos(int x, int y)
        {

            return Color.Black;
        }

        public virtual void Hit()
        {

        }

    }
}