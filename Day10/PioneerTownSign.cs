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
    
    public class PioneerTownSign : BackgroundHitObject
    {
        private enum StatusType
        {
            Waiting = 1,
            Transitioning = 2,
            Complete = 3
        }

        private int Life = 3;
        private StatusType Status = StatusType.Waiting;
        private Vector2 Destination;
        private GameplayScreen _gameplayScreen;

        public PioneerTownSign(GameplayScreen gameplayScreen)
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
                    Destination = new Vector2(Position.X + new Random().Next(-10, 10), Position.Y + 14);
                }
            }
            else if (Status == StatusType.Transitioning)
            {
                if (Position.Y < Destination.Y)
                {
                    Position.Y += 3;
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
            else if(Status == StatusType.Transitioning)
            {
                spriteBatch.Draw(Texture, Position+new Vector2(Texture.Width, Texture.Height), null, null, new Vector2(Texture.Width, Texture.Height), -0.20f, null, Color.White, SpriteEffects.None, 0);
            }
            else if(Status == StatusType.Complete)
            {
                spriteBatch.Draw(Texture, Position + new Vector2(Texture.Width, Texture.Height), null, null, new Vector2(Texture.Width, Texture.Height), -0.20f, null, Color.White, SpriteEffects.None, 0);
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
