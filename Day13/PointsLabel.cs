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

        public string _text = "";
        public float yoffset = 0.0f;
        private Color PointsColor = Color.Green;

        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.Zero;
        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 0;


        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 1 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        public PointsLabel(PointsLabelManager mgr)
        {
            _mgr = mgr;
            TransitionOffTime = TimeSpan.FromSeconds(3.0);
        }

        public void Initialize(string text, float X = 0.0f, float Y = 0.0f)
        {
            _text = text;
            Rotation = -0.2f;
            Position = new Vector2(X, Y);
            IsActive = true;

            PointsColor = Color.Green;
            yoffset = -0.75f;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                Position.Y += TransitionAlpha * yoffset;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    IsActive = false;
                }
            }
        }

        internal void Negative()
        {
            PointsColor = Color.Red;
            yoffset = +0.25f;
            Rotation = +0.2f;
        }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.DrawString(_mgr.spriteFont, _text, Position, PointsColor * TransitionAlpha, Rotation, Origin, 1.0f, SpriteEffects.None, 0);
            }
        }

    }
}
