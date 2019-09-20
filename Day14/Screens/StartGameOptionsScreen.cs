using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTarget.Screens
{

    public class UIButton
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public bool IsSelected { get; set; }

        public Action Action { get; set; }
        public DateTime Alive { get; internal set; }
    }


    class StartGameOptionsScreen : MenuScreen
    {
        ContentManager content;
        Texture2D backgroundTexture;
        string message = "Choose Options";

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;
        Texture2D[] crosshairs = new Texture2D[7];
        Texture2D[] guns = new Texture2D[2];
        UIButton[] uiButtons = new UIButton[5];

        private int currentCrosshair;
        private int currentGun;
        private Vector2 crosshairPosition = new Vector2(234, 400);
        private Vector2 gunPosition = new Vector2(614, 260);

        public StartGameOptionsScreen() : base("Choose Options")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            IsPopup = true;

        }

        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            //if(input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Left, PlayerIndex.One, out playerIndex))
            //{
            //    ScreenManager.SoundManager.PlaySound("MenuMove");
            //    if (currentCrosshair > 0) currentCrosshair--;
            //}
            //else if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Right, PlayerIndex.One, out playerIndex))
            //{
            //    ScreenManager.SoundManager.PlaySound("MenuMove");
            //    if (currentCrosshair < 6) currentCrosshair++;
            //}
            foreach(var btn in uiButtons)
            {
                if(btn.IsSelected)
                {
                    if (DateTime.Now > btn.Alive + TimeSpan.FromSeconds(0.2))
                    {
                        btn.IsSelected = false;
                    }
                }                
            }
            
            if (input.LastMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && input.CurrentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                foreach (var btn in uiButtons)
                {
                    Rectangle r = new Rectangle((int)btn.Position.X, (int)btn.Position.Y, btn.Texture.Width, btn.Texture.Height);
                    if (r.Contains(input.CurrentMouseState.Position))
                    {
                        btn.IsSelected = true;
                        btn.Alive = DateTime.Now;
                        //execute this action.
                        btn.Action();
                    }
                }
            }

            base.HandleInput(input);
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            // The background includes a border somewhat larger than the text itself.
            //const int hPad = 48; // 32;
            //const int vPad = 32; // 16;

            //Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
            //                                              (int)textPosition.Y - vPad,
            //                                              (int)textSize.X + hPad * 2,
            //                                              (int)textSize.Y + vPad * 2);

            Rectangle backgroundRectangle = new Rectangle(viewport.Width / 2 - backgroundTexture.Width / 2, 
                viewport.Height / 2 - backgroundTexture.Height / 2, 
                backgroundTexture.Width, 
                backgroundTexture.Height);

            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(backgroundTexture, backgroundRectangle, color);

            for(int i = 0; i<5; i++)
            {
                Vector2 offset = Vector2.Zero;
                if(uiButtons[i].IsSelected)
                {
                    offset = new Vector2(4, 4);
                }
                spriteBatch.Draw(uiButtons[i].Texture, uiButtons[i].Position + offset, color);
            }


            if (currentCrosshair > 0) spriteBatch.Draw(crosshairs[currentCrosshair-1], crosshairPosition + new Vector2(-186/4, 186/2), null, color, 0.0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0);
            spriteBatch.Draw(crosshairs[currentCrosshair], crosshairPosition, null, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            if (currentCrosshair < 6) spriteBatch.Draw(crosshairs[currentCrosshair+1], crosshairPosition + new Vector2(186,186/2), null, color, 0.0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0);

            if (currentGun > 0) spriteBatch.Draw(guns[currentGun - 1], gunPosition + new Vector2(-232 / 3, 186 / 2), null, color, 0.0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0);
            spriteBatch.Draw(guns[currentGun], gunPosition, null, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            if (currentGun < 1) spriteBatch.Draw(guns[currentGun + 1], gunPosition + new Vector2(232, 186 / 2), null, color, 0.0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0);

            spriteBatch.Draw(ScreenManager.cursorTexture, new Vector2(ScreenManager.input.CurrentMouseState.Position.X, ScreenManager.input.CurrentMouseState.Position.Y), Color.White);

            // Draw the message box text.
            //spriteBatch.DrawString(font, message, textPosition, color);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("startgamepicker");
            for(int i=0; i<7; i++)
            {
                crosshairs[i] = content.Load<Texture2D>($"crosshair{i+1}");
            }
            for (int i = 0; i < 2; i++)
            {
                guns[i] = content.Load<Texture2D>($"gun{i + 1}");
            }

            for (int i=0; i<5; i++)
            {
                uiButtons[i] = new UIButton();
            }

            uiButtons[0].Texture = content.Load<Texture2D>("leftbutton");
            uiButtons[0].Position = new Vector2(260, 600);
            uiButtons[0].Action = CrosshairLeft;
            uiButtons[1].Texture = content.Load<Texture2D>("rightbutton");
            uiButtons[1].Position = new Vector2(260+80, 600);
            uiButtons[1].Action = CrosshairRight;
            uiButtons[2].Texture = content.Load<Texture2D>("leftbutton");
            uiButtons[2].Position = new Vector2(600, 600);
            uiButtons[2].Action = GunLeft;
            uiButtons[3].Texture = content.Load<Texture2D>("rightbutton");
            uiButtons[3].Position = new Vector2(600+80, 600);
            uiButtons[3].Action = GunRight;
            uiButtons[4].Texture = content.Load<Texture2D>("3rightbutton");
            uiButtons[4].Position = new Vector2(1060, 600);
            uiButtons[4].Action = StartGame;


            if (((Game1)ScreenManager.Game).ConfigMGR.gameConfig.Crosshair > -1 && ((Game1)ScreenManager.Game).ConfigMGR.gameConfig.Crosshair < 7)
            {
                currentCrosshair = ((Game1)ScreenManager.Game).ConfigMGR.gameConfig.Crosshair;
            }

            base.LoadContent();
        }

        private void GunRight()
        {
            if (currentGun < 1)
            {
                currentGun++;
                ScreenManager.SoundManager.PlaySound("MenuMove");
            }
        }

        private void GunLeft()
        {
            if (currentGun > 0)
            {
                ScreenManager.SoundManager.PlaySound("MenuMove");
                currentGun--;
            }
        }

        private void StartGame()
        {
            ScreenManager.SoundManager.StopAllSounds();
            ScreenManager.SoundManager.PlaySound("MenuMove");
            ((Game1)ScreenManager.Game).ConfigMGR.gameConfig.Crosshair = currentCrosshair;
            ((Game1)ScreenManager.Game).ConfigMGR.gameConfig.Gun = currentGun;
            ((Game1)ScreenManager.Game).ConfigMGR.Save();
            LoadingScreen.Load(ScreenManager, false, 0, new JournalMenuScreen()); // GameplayScreen());
        }

        private void CrosshairRight()
        {

            if (currentCrosshair < 6)
            {
                currentCrosshair++;
                ScreenManager.SoundManager.PlaySound("MenuMove");
            }
        }

        private void CrosshairLeft()
        {

            if (currentCrosshair > 0)
            {
                ScreenManager.SoundManager.PlaySound("MenuMove");
                currentCrosshair--;
            }
        }

        public override void UnloadContent()
        {
            content.Unload();
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.SoundManager.PlaySound("MenuMove");
            base.OnCancel(playerIndex);
        }

        protected override void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            //ScreenManager.SoundManager.StopAllSounds();
            //ScreenManager.SoundManager.PlaySound("MenuMove");
            //((Game1)ScreenManager.Game).ConfigMGR.gameConfig.Crosshair = currentCrosshair;
            //((Game1)ScreenManager.Game).ConfigMGR.Save();
            //LoadingScreen.Load(ScreenManager, false, playerIndex, new JournalMenuScreen()); // GameplayScreen());

            //base.OnSelectEntry(entryIndex, playerIndex);
        }

    }
}
