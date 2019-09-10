using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TexturePackerLoader;

namespace MonoTarget
{
    public class RicochetManager
    {
        public List<Ricochet> ricochets { get; set; }

        private GameplayScreen _gameplayScreen;

        public SpriteSheet Sheet { get; set; }
        public string[] sprites;

        public RicochetManager(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
            ricochets = new List<Ricochet>();
            sprites = new[]
            {
                RicochetFrames.Anim_001,
                RicochetFrames.Anim_002,
                RicochetFrames.Anim_003,
                RicochetFrames.Anim_004
            };

        }


        public void Initialize(string tex)
        {
            Sheet = _gameplayScreen.ScreenManager.spriteSheetLoader.Load(tex);
        }

        public Ricochet CreateRicochet(int x, int y)
        {
            Ricochet r = new Ricochet(this);

            r.Initialize(_gameplayScreen, new Vector2(x, y));
            ricochets.Add(r);
            _gameplayScreen.VisualLayers[VisualLayerID.NonInteractiveHUD].Add(r);
            return r;
        }
    }

    public class RicochetFrames
    {
        public const string Anim_001 = "0001";
        public const string Anim_002 = "0002";
        public const string Anim_003 = "0003";
        public const string Anim_004 = "0004";
    }


    public class Ricochet : GameObject
    {
        TimeSpan previousFrameChangeTime = TimeSpan.Zero;
        private RicochetManager _mgr;
        public AnimationManager animationManager;
        Animation animation;

        public Ricochet(RicochetManager mgr)
        {
            _mgr = mgr;
            IsAnimated = true;
        }


        public void Initialize(GameplayScreen gameplayScreen, Vector2 pos)
        {
            base.Initialize(gameplayScreen.ScreenManager.Game.Content);
            IsAnimated = true;
            Position = pos;

            animation =
                new Animation(
                    new Vector2(0, 0),
                        TimeSpan.FromSeconds(2f / 30f), SpriteEffects.None, _mgr.sprites);

            var anims = new[]
            {
                animation
            };

            animationManager = new AnimationManager(_mgr.Sheet, Position, anims);
        }


        public override void Update(GameTime gameTime)
        {
            if (!animationManager.PlayedOnceAlready)
            {
                animationManager.Update(gameTime);
            }
            else
            {
                IsActive = false;
            }            
        }

        public override void Draw(SpriteRender spriteRender)
        {
            if (IsActive)
            {
                spriteRender.Draw(
                    animationManager.CurrentSprite,
                    animationManager.CurrentPosition,
                    Color.White, 0, 1,
                    animationManager.CurrentSpriteEffects);
            }
        }

    }
}