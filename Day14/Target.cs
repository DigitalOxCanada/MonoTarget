using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TexturePackerLoader;

namespace MonoTarget
{
    //TODO add a timer for the dieing animation
    public enum StateOfExistenceType
    {
        Arriving,
        Alive,
        Dieing,
        Expiring,
        Dead
    }

    public class Target : GameObject
    {
        public SpriteSheet Sheet { get; set; }

        public string[] sprites;
        public AnimationManager animationManager;
        Animation animation;

        public RenderTarget2D renderTarget2D;
        private GameObject muzzleFlash;

        private const int TARGETHIT_1 = 25;
        private const int TARGETHIT_2 = 15;
        private const int TARGETHIT_3 = 5;
        private const int TARGETHIT_LIFELOSS_1 = 3;
        private const int TARGETHIT_LIFELOSS_2 = 2;
        private const int TARGETHIT_LIFELOSS_3 = 1;

        public Target(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
        }
        public bool WasHit;
        public StateOfExistenceType State { get; set; }
        public Color[] BGMaterialColorData { get; set; }
        private int Life { get; set; } = 3;
        private bool IsAlive { get { return Life > 0 ? true : false; } }

        Rectangle DieingRect;
        private GameplayScreen _gameplayScreen;

        private DateTime Created;
        public bool IsEnemy = false;
        private TimeSpan TimeToShootBack;
        private TimeSpan EndOfLife;
        private TimeSpan muzzleFlashDuration;
        private TimeSpan previousFrameChangeTime = TimeSpan.Zero;
        private DateTime muzzleFlashActive;

        public void Initialize(string tex, float X = 0.0f, float Y = 0.0f )
        {
            if (IsAnimated)
            {
                Sheet = _gameplayScreen.ScreenManager.spriteSheetLoader.Load(tex);
                base.Initialize(_gameplayScreen.ScreenManager.Game.Content, null, X, Y);


                sprites = new string[Sheet.SpriteCount()];
                for(int i=0; i< Sheet.SpriteCount(); i++)
                {
                    sprites[i] = string.Format("{0}", i + 1).PadLeft(4,'0');
                }

                animation = new Animation( new Vector2(0, 0), TimeSpan.FromSeconds(10f / 60f), SpriteEffects.None, sprites);
                var anims = new[] { animation };
                animationManager = new AnimationManager(Sheet, Position, anims);

            }
            else
            {
                base.Initialize(_gameplayScreen.ScreenManager.Game.Content, tex, X, Y);

                //convert the texture into a color map for pixel accuracy hit testing
                BGMaterialColorData = new Color[Texture.Width * Texture.Height];
                Texture.GetData<Color>(BGMaterialColorData);
                renderTarget2D = new RenderTarget2D(_gameplayScreen.ScreenManager.GraphicsDevice, Texture.Width, Texture.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget2D);
                _gameplayScreen.ScreenManager.SpriteBatch.Begin();
                _gameplayScreen.ScreenManager.SpriteBatch.Draw(this.Texture, Vector2.Zero, Color.White);
                _gameplayScreen.ScreenManager.SpriteBatch.End();
                _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(null);
            }
            if (IsEnemy)
            {
                muzzleFlash = new GameObject();
                muzzleFlash.Initialize(_gameplayScreen.ScreenManager.Game.Content, "muzzleflash");
                muzzleFlashDuration = TimeSpan.FromSeconds(2f / 60f);
            }

            State = StateOfExistenceType.Alive;
            Created = DateTime.Now;
            EndOfLife = TimeSpan.FromSeconds(2.5);
            if (IsEnemy)
            {
                TimeToShootBack = TimeSpan.FromSeconds(1.5);
            }

        }


        public override void Update(GameTime gameTime)
        {
            var nowTime = gameTime.TotalGameTime;
            //var dtFrame = nowTime - this.previousFrameChangeTime;
            //if (dtFrame >= muzzleFlashDuration)
            //{
            //    this.previousFrameChangeTime = nowTime;
            //}
            if (IsEnemy)
            {
                if (DateTime.Now > muzzleFlashActive + muzzleFlashDuration)
                {
                    muzzleFlash.IsActive = false;
                }
            }

            Rectangle r;

            if (IsAnimated)
            {
                animationManager.Update(gameTime);
                r = new Rectangle((int)animationManager.CurrentPosition.X, 
                    (int)animationManager.CurrentPosition.Y, 
                    (int)animationManager.CurrentSprite.Size.X, 
                    (int)animationManager.CurrentSprite.Size.Y);
                //Debug.WriteLine($"{r.X}x{r.Y}");
            }
            else
            {
                r = new Rectangle((int)Position.X, (int)Position.Y, (int)Texture.Width, (int)Texture.Height);
            }

            WasHit = false;


            if (State == StateOfExistenceType.Alive)
            {
                //if target expired...
                if (DateTime.Now > Created + EndOfLife)
                {
                    State = StateOfExistenceType.Expiring;
                    EndOfLife += TimeSpan.FromSeconds(1.5); //live a bit longer to allow expiring animation
                    //DieingRect = r;
                    Debug.WriteLine("Target Expiring");
                }
                else
                {
                    if (IsEnemy)
                    {
                        //todo if enemy shoot back
                        if (DateTime.Now > Created + TimeToShootBack)
                        {
                            //shoot at player
                            //muzzle flash
                            muzzleFlash.Position = new Vector2(Position.X + muzzleFlash.Origin.X, Position.Y);
                            muzzleFlash.IsActive = true;
                            muzzleFlashActive = DateTime.Now;

                            // shot sound
                            _gameplayScreen.ScreenManager.SoundManager.PlaySound("gunshot1");
                            // player lose health
                            _gameplayScreen.player.HealthCount--;

                            TimeToShootBack = TimeSpan.FromDays(1000);
                        }
                    }

                    //if the mouse is within the image
                    if (_gameplayScreen.player.PlayerFired)   //shouldn't check click, should check fired
                    {
                        Debug.WriteLine($"Fired at {_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.X}x{_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.Y}");
                        if (r.Contains(_gameplayScreen.ScreenManager.input.CurrentMouseState.Position))
                        {
                            Debug.WriteLine($"Target Rect at {r.X}x{r.Y}:{r.Width}x{r.Height}");
                            if (IsAnimated)
                            {
                                var col = GetColorAtPos(_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.X - r.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Position.Y - r.Y);
                                Debug.WriteLine($"col at x,y : {col}");
                                if (col != Color.Transparent) // & 0xFF000000) >> 24) > 20)
                                {
                                    if (col == Color.Red)
                                    {
                                        Life -= TARGETHIT_LIFELOSS_1;
                                        _gameplayScreen.player.Score += TARGETHIT_1;
                                        HitLabel l = _gameplayScreen.HitLabelManager.CreateHitLabel(HitLabelFrames.HitLabel_Headshot, r.X + (r.Width / 2), r.Y);
                                        _gameplayScreen.PointsLabelManager.CreatePointsLabel($"+{TARGETHIT_1}", r.X + (r.Width / 2), r.Y - 25);
                                        WasHit = true;
                                    }
                                    else if (col == Color.Purple)
                                    {
                                        Life -= TARGETHIT_LIFELOSS_2;
                                        _gameplayScreen.player.Score += TARGETHIT_2;
                                        HitLabel l = _gameplayScreen.HitLabelManager.CreateHitLabel(HitLabelFrames.HitLabel_Gotcha, r.X + (r.Width / 2), r.Y);
                                        _gameplayScreen.PointsLabelManager.CreatePointsLabel($"+{TARGETHIT_2}", r.X + (r.Width / 2), r.Y - 25);
                                        WasHit = true;
                                    }
                                    else if (col == Color.Green)
                                    {
                                        Life -= TARGETHIT_LIFELOSS_3;
                                        _gameplayScreen.player.Score += TARGETHIT_3;
                                        _gameplayScreen.PointsLabelManager.CreatePointsLabel($"+{TARGETHIT_3}", r.X + (r.Width / 2), r.Y - 25);
                                        WasHit = true;
                                    }
                                    else if (col == Color.Gray)
                                    {

                                    }
                                    //Debug.WriteLine($"target hit {_gameplayScreen.CurrentMouseState.Position.X}x{_gameplayScreen.CurrentMouseState.Position.Y}");
                                }
                            }
                            else
                            {
                                if (GetColorAtPos(_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.X - r.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Position.Y - r.Y) != Color.Transparent)
                                {
                                    Debug.WriteLine($"Target hit {_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.X}x{_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.Y}");
                                    WasHit = true;

                                    Bullet b = _gameplayScreen.BulletManager.bullets[0];
                                    _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget2D);
                                    _gameplayScreen.ScreenManager.SpriteBatch.Begin();
                                    _gameplayScreen.ScreenManager.SpriteBatch.Draw(b.Texture, new Vector2(_gameplayScreen.ScreenManager.input.CurrentMouseState.Position.X - r.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Position.Y - r.Y) - b.Origin, Color.White);
                                    _gameplayScreen.ScreenManager.SpriteBatch.End();
                                    _gameplayScreen.ScreenManager.GraphicsDevice.SetRenderTarget(null);

                                    Life -= TARGETHIT_LIFELOSS_3;
                                    _gameplayScreen.PointsLabelManager.CreatePointsLabel($"+{TARGETHIT_3}", r.X + (r.Width / 2), r.Y - 25);
                                }

                            }
                            if (Life < 1)
                            {
                                State = StateOfExistenceType.Dieing;
                                EndOfLife += TimeSpan.FromSeconds(0.5);
                            }
                        }
                    }
                }
            }
            else
            {
                if(State==StateOfExistenceType.Dieing || State==StateOfExistenceType.Expiring)
                {
                    if (DateTime.Now > Created + EndOfLife)
                    {
                        State = StateOfExistenceType.Dead;
                        Debug.WriteLine("Target Dead");
                    }
                    //DieingRect.Height--;
                    //DieingRect.Y++;
                    //if(DieingRect.Height<0)
                    //{
                    //    State = StateOfExistenceType.Dead;
                    //}
                }
            }
        }

        /// <summary>
        /// Lookup the color value on the 1d array of color data based on x,y position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal Color GetColorAtPos(int x, int y)
        {
            if (y < 0 || x < 0) return Color.TransparentBlack;
            if (IsAnimated)
            {
                if (animationManager.CurrentSprite.BGMaterialColorData == null) return Color.TransparentBlack;
                return animationManager.CurrentSprite.BGMaterialColorData[y * (int)animationManager.CurrentSprite.Size.X + x];
            }
            else
            {
                if (BGMaterialColorData == null) return Color.Transparent;
                //formula for 2d coords on a 1d array = y * width + x;
                return BGMaterialColorData[y * Texture.Width + x];
            }
            return Color.Transparent;
        }

        public override void Draw(SpriteRender spriteRender)
        {
            //if the target is dead then nothing to draw
            if (State == StateOfExistenceType.Dead)
            {
                return;
            }

            if (!IsAlive && State == StateOfExistenceType.Dieing)
            {
                spriteRender.Draw(animationManager.CurrentSprite, animationManager.CurrentPosition, Color.Red*0.5f, 0, 1, animationManager.CurrentSpriteEffects);
                return;
            }

            if (State == StateOfExistenceType.Expiring)
            {
                spriteRender.Draw(animationManager.CurrentSprite, animationManager.CurrentPosition, Color.DarkGray*0.5f, 0, 1, animationManager.CurrentSpriteEffects);
                return;
            }

            if (IsAlive && IsActive && IsAnimated)
            {
                spriteRender.Draw(animationManager.CurrentSprite, animationManager.CurrentPosition, Color.White, 0, 1, animationManager.CurrentSpriteEffects);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //if the target is dead then nothing to draw
            if(State==StateOfExistenceType.Dead) { 
                return;
            }

            //if the target is dieing then draw with red
            if(!IsAlive && State==StateOfExistenceType.Dieing)
            {
                spriteBatch.Draw(renderTarget2D, DieingRect, Color.Red * 0.5f);
                return;
            }

            if (State == StateOfExistenceType.Expiring)
            {
                spriteBatch.Draw(renderTarget2D, DieingRect, Color.DarkGray *0.5f);
                return;
            }

            //if target is still alive draw normally.
            if (IsAlive)
            {
                spriteBatch.Draw(renderTarget2D, Position, Color.White);
            }
            if (IsEnemy)
            {
                if (muzzleFlash.IsActive)
                {
                    spriteBatch.Draw(muzzleFlash.Texture, muzzleFlash.Position, Color.White);
                }
            }
        }
    }
}
