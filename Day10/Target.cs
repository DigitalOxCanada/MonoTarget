﻿using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Target(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
        }

        public StateOfExistenceType State { get; set; }
        public uint[] BGMaterialColorData { get; set; }
        private int Life { get; set; } = 1;
        private bool IsAlive { get { return Life > 0 ? true : false; } }
        Rectangle DieingRect;
        private DateTime EndOfLife;
        private GameplayScreen _gameplayScreen;

        public void Initialize(string tex, float X = 0.0f, float Y = 0.0f )
        {
            base.Initialize(_gameplayScreen.ScreenManager.Game.Content, tex, X, Y);

            State = StateOfExistenceType.Alive;

            //convert the texture into a color map for pixel accuracy hit testing
            BGMaterialColorData = new uint[Texture.Width * Texture.Height];
            Texture.GetData<uint>(BGMaterialColorData);

            EndOfLife = DateTime.Now.AddSeconds(2); //life until target expires
        }


        public override void Update(GameTime gameTime)
        {
            Rectangle r = new Rectangle((int)Position.X, (int)Position.Y, (int)Texture.Width, (int)Texture.Height);

            if (State == StateOfExistenceType.Alive)
            {
                //if target expired...
                if (DateTime.Now > EndOfLife)
                {
                    State = StateOfExistenceType.Expiring;
                    DieingRect = r;
                    Debug.WriteLine("Target Expiring");
                }
                else
                {

                    //if the mouse is within the image
                    if (r.Contains(_gameplayScreen.CurrentMouseState.Position))
                    {
                        if (((GetColorAtPos(_gameplayScreen.CurrentMouseState.Position.X - r.X, _gameplayScreen.CurrentMouseState.Position.Y - r.Y) & 0xFF000000) >> 24) > 20)
                        {
                            if (_gameplayScreen.player.PlayerFired)   //shouldn't check click, should check fired
                            {
                                Debug.WriteLine($"Target hit {_gameplayScreen.CurrentMouseState.Position.X}x{_gameplayScreen.CurrentMouseState.Position.Y}");

                                //TODO decrement by a variable amount based on weapon damage, ie. shotgun = 5
                                Life--;
                                if (Life < 1)
                                {
                                    State = StateOfExistenceType.Dieing;
                                    DieingRect = r;

                                    HitLabel l = _gameplayScreen.HitLabelManager.CreateHitLabel(HitLabelFrames.HitLabel_Gotcha, r.X + (r.Width/2), r.Y);

                                    _gameplayScreen.PointsLabelManager.CreatePointsLabel("+10", r.X + (r.Width / 2), r.Y - 25);

                                    Debug.WriteLine("Target Dieing");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if(State==StateOfExistenceType.Dieing || State==StateOfExistenceType.Expiring)
                {
                    //TODO calculate rate based on timer
                    DieingRect.Height--;
                    DieingRect.Y++;
                    if(DieingRect.Height<0)
                    {
                        State = StateOfExistenceType.Dead;
                    }
                }
            }
        }

        /// <summary>
        /// Lookup the color value on the 1d array of color data based on x,y position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal uint GetColorAtPos(int x, int y)
        {
            if (BGMaterialColorData == null) return 0;
            //formula for 2d coords on a 1d array = y * width + x;
            return BGMaterialColorData[y * Texture.Width + x];
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
                spriteBatch.Draw(Texture, DieingRect, Color.Red);
                return;
            }

            if (State == StateOfExistenceType.Expiring)
            {
                spriteBatch.Draw(Texture, DieingRect, Color.DarkGray);
                return;
            }

            //if target is still alive draw normally.
            if (IsAlive)
            {
                spriteBatch.Draw(Texture, Position, Color.White);
            }
        }
    }
}
