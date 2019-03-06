using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TexturePackerLoader;

namespace MonoTarget
{
    public class BarberPole : GameObject
    {
        public AnimationManager barberPoleAnimationManager;
        public SpriteSheet barberPoleSheet;
        string[] barberPoleSprites;
        public bool Hit = false;
        int life = 10;
        Animation animationBarberPole;
        TimeSpan previousFrameChangeTime = TimeSpan.Zero;

        public BarberPole()
        {

            barberPoleSprites = new[]
            {
                BarberPoleAnim.BarberPoleAnim_0001,
                BarberPoleAnim.BarberPoleAnim_0002,
                BarberPoleAnim.BarberPoleAnim_0003,
                BarberPoleAnim.BarberPoleAnim_0004,
                BarberPoleAnim.BarberPoleAnim_0005
            };

        }

        public void Initialize(Game1 game, SpriteSheetLoader spl, string imagefn)
        {
            base.Initialize(game);
            IsAnimated = true;
            Position = new Vector2(538, 307);

            barberPoleSheet = spl.Load(imagefn);
            animationBarberPole =
                new Animation(
                    new Vector2(0, 0),
                        TimeSpan.FromSeconds(5f / 30f), SpriteEffects.None, barberPoleSprites);

            var barberPoleAnimations = new[]
            {
                animationBarberPole
            };

            barberPoleAnimationManager = new AnimationManager(barberPoleSheet, Position, barberPoleAnimations);

        }

        public override void Update(GameTime gameTime)
        {
            if (Hit)
            {
                life--;
                if (life < 1)
                {
                    animationBarberPole.TimePerFrame = TimeSpan.FromSeconds(15f / 30f);
                    life = 0;
                }
                else
                {
                    animationBarberPole.TimePerFrame = TimeSpan.FromSeconds(1f / 30f);
                }
                Hit = false;
            }
            var nowTime = gameTime.TotalGameTime;
            var dtFrame = nowTime - previousFrameChangeTime;

            //once a second check if we should slow down the animation speed
            if (dtFrame >= TimeSpan.FromSeconds(1f))
            {
                previousFrameChangeTime = nowTime;
                if(animationBarberPole.TimePerFrame < TimeSpan.FromSeconds(5f/30f))
                {
                    animationBarberPole.TimePerFrame += TimeSpan.FromSeconds(1f / 30f);
                }
            }

            barberPoleAnimationManager.Update(gameTime);
        }

        public override void Draw(SpriteRender spriteRender)
        {
            spriteRender.Draw(
                this.barberPoleAnimationManager.CurrentSprite,
                this.barberPoleAnimationManager.CurrentPosition,
                Color.White, 0, 1,
                this.barberPoleAnimationManager.CurrentSpriteEffects);
        }
    }

    public class BarberPoleAnim
    {
        public const string BarberPoleAnim_0001 = "0001";
        public const string BarberPoleAnim_0002 = "0002";
        public const string BarberPoleAnim_0003 = "0003";
        public const string BarberPoleAnim_0004 = "0004";
        public const string BarberPoleAnim_0005 = "0005";
    }
}