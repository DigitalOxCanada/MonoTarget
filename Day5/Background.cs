using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MonoTarget
{
    public class Background : GameObject
    {
        public Background()
        {
            MaterialAction = new Dictionary<Color, Action>
            {
                [Color.White] = () => ShotBreakGlass(),
                [Color.Purple] = () => ShotMissed(),
                [new Color(255,216,0,255)] = () => ShotRicochet(),
                [new Color(72,0,255,255)] = () => ShotBarberPole()

            };
        }

        private void ShotBarberPole()
        {
            Debug.WriteLine("Action: ShotBarberPole");
            _game.BarbersPole.Hit = true;
        }

        private void ShotRicochet()
        {
            Debug.WriteLine("Action: ShotRicochet");
            
            _game.SoundManager.SoundLibrary["ricochet1"].Play(1.0f, 0.0f, 0.0f);
        }

        private void ShotMissed()
        {
            Debug.WriteLine("Action: ShotMissed");
        }

        private void ShotBreakGlass()
        {
            Debug.WriteLine("Action: ShotBreakGlass");
            //play breaking glass sound
            _game.SoundManager.SoundLibrary["breakglass1"].Play(1.0f, 0.0f, 0.0f);
        }

        public Texture2D BGMaterials { get; set; }
        public Color[] BGMaterialColorData { get; set; }
        public Dictionary<Color, Action> MaterialAction { get; set; }

        public void Initialize(Game1 game, string textureName, string bgMaterials = null)
        {
            base.Initialize(game, textureName);
            if (bgMaterials != null)
            {
                BGMaterials = game.Content.Load<Texture2D>(bgMaterials);

                //make colour map data of bg materials image
                //? we don't really need the texture anymore
                BGMaterialColorData = new Color[BGMaterials.Width * BGMaterials.Height];
                BGMaterials.GetData<Color>(BGMaterialColorData);
            }
        }

        public override void Update(GameTime gameTime)
        {
            

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        internal Color GetColorAtPos(int x, int y)
        {
            //formula for 2d coords on a 1d array = y * width + x;
            return BGMaterialColorData[y * BGMaterials.Width + x];
        }

        internal void CheckHitMaterial(Color pixel)
        {
            if(MaterialAction.ContainsKey(pixel))
            {
                //we have something defined for that color so call the action
                MaterialAction[pixel]();
            }
        }
    }
}
