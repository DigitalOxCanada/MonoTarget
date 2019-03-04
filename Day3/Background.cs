using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MonoTarget
{
    public class Background
    {
        public Background()
        {
            MaterialAction = new Dictionary<Color, Action>
            {
                [Color.White] = () => ShotBreakGlass(),
                [Color.Purple] = () => ShotMissed(),
                [new Color(255,216,0,255)] = () => ShotRicochet(),

            };
        }

        private void ShotRicochet()
        {
            Debug.WriteLine("Action: ShotRicochet");
            
            _game.SoundLibrary["ricochet1"].Play(1.0f, 0.0f, 0.0f);
        }

        private void ShotMissed()
        {
            Debug.WriteLine("Action: ShotMissed");
        }

        private void ShotBreakGlass()
        {
            Debug.WriteLine("Action: ShotBreakGlass");
            //play breaking glass sound
            _game.SoundLibrary["breakglass1"].Play(1.0f, 0.0f, 0.0f);
        }

        private Game1 _game;

        public Texture2D BGImage { get; set; }
        public Texture2D BGMaterials { get; set; }
        public Color[] BGMaterialColorData { get; set; }
        public Dictionary<Color, Action> MaterialAction { get; set; }

        public void Initialize(Texture2D tex, Texture2D mat, Game1 game)
        {
            _game = game;
            BGImage = tex;
            BGMaterials = mat;
            //make colour map data of bg materials image
            //? we don't really need the texture anymore
            BGMaterialColorData = new Color[BGMaterials.Width * BGMaterials.Height];
            BGMaterials.GetData<Color>(BGMaterialColorData);

        }

        public void Update(GameTime gameTime)
        {
            

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BGImage, new Vector2(0, 0), Color.White);
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
