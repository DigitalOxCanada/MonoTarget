using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTarget
{
    public class MetalSign : BackgroundHitObject
    {
        private enum StatusType
        {
            Waiting = 1,
            Transitioning = 2,
            Complete = 3
        }

        public int Life = 2;
        private StatusType Status = StatusType.Waiting;
        public Vector2 Destination;
        public float rot = 0.0f;
        private GameplayScreen _gameplayScreen;

        public MetalSign(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
        }

        public Color[] BGMaterialColorData { get; set; }

        public void Initialize(string textureName)
        {
            base.Initialize(_gameplayScreen.ScreenManager.Game.Content, textureName);

            //make colour map data of bg materials image
            //? we don't really need the texture anymore
            BGMaterialColorData = new Color[Texture.Width * Texture.Height];
            Texture.GetData<Color>(BGMaterialColorData);
        }

        public override void Update(GameTime gameTime)
        {
            if (Status == StatusType.Waiting)
            {
                if (Life < 1)
                {
                    Status = StatusType.Transitioning;
                }
            }
            else if (Status == StatusType.Transitioning)
            {
                if(Position.Y<Destination.Y)
                {
                    Position.Y += 5;
                }
                else
                {
                    Status = StatusType.Complete;
                }
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Status == StatusType.Waiting)
            {
                spriteBatch.Draw(Texture, Position, Color.White);
            }
            else if (Status == StatusType.Transitioning)
            {
                rot += 0.10f;
                spriteBatch.Draw(Texture, Position, null, null, null, rot, null, Color.Beige, SpriteEffects.None, 0);
            }
            else if (Status == StatusType.Complete)
            {
                spriteBatch.Draw(Texture, Position, null, null, null, rot, null, Color.SandyBrown, SpriteEffects.None, 0);
            }
        }

        public override Color GetColorAtPos(int x, int y)
        {
            //formula for 2d coords on a 1d array = y * width + x;
            return BGMaterialColorData[y * Texture.Width + x];
        }

        public override void Hit()
        {
            Life--;

        }
    }
}