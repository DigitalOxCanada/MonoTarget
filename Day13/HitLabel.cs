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

    public class HitLabelManager
    {
        public GameplayScreen _gameplayScreen;

        public List<HitLabel> Labels { get; set; }

        public SpriteSheet Sheet { get; set; }
        string[] sprites;


        public HitLabelManager(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
            Labels = new List<HitLabel>();
            sprites = new[]
            {
                HitLabelFrames.HitLabel_Headshot,
                HitLabelFrames.HitLabel_Boom,
                HitLabelFrames.HitLabel_Wow,
                HitLabelFrames.HitLabel_Kaboom,
                HitLabelFrames.HitLabel_ByeBye,
                HitLabelFrames.HitLabel_Gotcha
            };
        }

        public void Initialize(string tex)
        {
            Sheet = _gameplayScreen.ScreenManager.spriteSheetLoader.Load(tex);
        }

        public HitLabel CreateHitLabel(string frame, int x, int y)
        {
            HitLabel l = new HitLabel(this);
            l.Initialize(frame, x, y);
            _gameplayScreen.VisualLayers[VisualLayerID.NonInteractiveHUD].Add(l);

            return l;
        }
    }

    public class HitLabelFrames
    {
        public const string HitLabel_Headshot = "0001";
        public const string HitLabel_Boom = "0002";
        public const string HitLabel_Wow = "0003";
        public const string HitLabel_Kaboom = "0004";
        public const string HitLabel_ByeBye = "0005";
        public const string HitLabel_Gotcha = "0006";
    }

    public class HitLabel : GameObject
    {
        private HitLabelManager _mgr;
        public DateTime EndOfLife { get; private set; }
        public string frame = HitLabelFrames.HitLabel_Headshot;

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


        public HitLabel(HitLabelManager mgr)
        {
            _mgr = mgr;
            IsAnimated = true;  //just to over Draw
            TransitionOffTime = TimeSpan.FromSeconds(3.0);
        }

        public void Initialize(string tex = null, float X = 0.0f, float Y = 0.0f)
        {
            base.Initialize(_mgr._gameplayScreen.ScreenManager.Game.Content, null, X, Y);

            EndOfLife = DateTime.Now.AddSeconds(2); //life until object expires
        }

        public override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    IsActive = false;
                }
            }
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

        public override void Draw(SpriteRender spriteRender)
        {
            if (IsActive)
            {
                spriteRender.Draw(_mgr.Sheet.Sprite(HitLabelFrames.HitLabel_Kaboom), Position, Color.White*TransitionAlpha, -0.2f, 1, SpriteEffects.None);
            }
        }

    }
}
