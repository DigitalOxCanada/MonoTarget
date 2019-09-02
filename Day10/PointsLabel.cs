using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TexturePackerLoader;

namespace MonoTarget
{
    
    public class PointsLabelManager
    {
        public List<PointsLabel> labels { get; set; }
        public SpriteFont spriteFont;
        private GameplayScreen _gameplayScreen;

        public PointsLabelManager(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
        }

        public void Initialize(string font)
        {
            spriteFont = _gameplayScreen.ScreenManager.Game.Content.Load<SpriteFont>(font);
        }


        public PointsLabel CreatePointsLabel(string text, int x, int y)
        {
            PointsLabel l = new PointsLabel(this);
            l.Initialize(text, x, y);
            _gameplayScreen.VisualLayers[VisualLayerID.NonInteractiveHUD].Add(l);

            return l;
        }
    }


    public class PointsLabel : GameObject
    {
        private PointsLabelManager _mgr;

        public DateTime EndOfLife { get; private set; }
        public string _text = "";
        public float yoffset;

        public PointsLabel(PointsLabelManager mgr)
        {
            _mgr = mgr;
        }

        public void Initialize(string text, float X = 0.0f, float Y = 0.0f)
        {
            _text = text;
            Rotation = -0.2f;
            Position = new Vector2(X, Y);
            IsActive = true;

            Color = Color.Green;
            yoffset = -0.75f;

            EndOfLife = DateTime.Now.AddSeconds(2); //life until object expires
        }

        public override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                Position.Y+=yoffset;
                if ( DateTime.Now > EndOfLife) {
                    IsActive = false;
                }
            }
        }

        internal void Negative()
        {
            Color = Color.Red;
            yoffset = +0.25f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.DrawString(_mgr.spriteFont, _text, Position, Color, Rotation, Origin, 1.0f, SpriteEffects.None, 0);
            }
        }

    }
}
