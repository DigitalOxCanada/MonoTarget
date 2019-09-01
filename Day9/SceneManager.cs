using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoTarget
{
    public class SceneManager
    {
        public string Name { get; set; }
        public string PackageName { get; set; }
        public Background BackGrnd { get; set; }

        public string[] scenes = new string[] { "scene1", "scene2" };
        public string currentScene = "scene2";

        private Game1 _game;

        List<GameObject> objects { get; set; }

        public SceneManager(Game1 game1)
        {
            _game = game1;
            objects = new List<GameObject>();
        }

        internal void Initialize()
        {
            BackGrnd = new Background();
        }

        internal void LoadContent()
        {
            _game.SoundManager.AddSound("thunder1", "thunder1");
            _game.SoundManager.AddSound("crowcaw1", "crowcaw1");
            _game.SoundManager.AddMusic("song1", "song1");

            //eventually make these generics that are loaded from a scene file for dynamic scenes
            if (currentScene == "scene1")
            {
                BackGrnd.Initialize(_game, "scene1", "scene1Mask");
                _game.VisualLayers[VisualLayerID.BG].Add(BackGrnd);

                BarberPole BarbersPole = new BarberPole();
                BarbersPole.Initialize(_game, _game.spriteSheetLoader, "barberpole.png", new Vector2(538, 307));
                _game.VisualLayers[VisualLayerID.BG1].Add(BarbersPole);

                MetalSign sg = new MetalSign();
                sg.Initialize(_game, "MetalPlate1");
                sg.Position = new Vector2(696, 322);
                sg.Life = 2;
                sg.Destination = new Vector2(sg.Position.X + new Random().Next(-20, 20), sg.Position.Y + 140);
                _game.VisualLayers[VisualLayerID.BG1].Add(sg);

                MetalSign sg1 = new MetalSign();
                sg1.Initialize(_game, "bathhousesign1");
                sg1.Position = new Vector2(867, 218);
                sg1.Life = 10;
                sg1.Destination = new Vector2(sg1.Position.X + new Random().Next(-50, 50), sg1.Position.Y + 200);
                _game.VisualLayers[VisualLayerID.BG1].Add(sg1);

                MetalSign sg2 = new MetalSign();
                sg2.Initialize(_game, "statue1");
                sg2.Position = new Vector2(855, 159);
                sg2.Life = 6;
                sg2.Destination = new Vector2(sg2.Position.X + new Random().Next(-20, 20), sg2.Position.Y + 300);
                _game.VisualLayers[VisualLayerID.BG1].Add(sg2);

                PioneerTownSign pts = new PioneerTownSign();
                pts.Initialize(_game, "sign1");
                pts.Position = new Vector2(375, 100);
                _game.VisualLayers[VisualLayerID.BG1].Add(pts);
            }
            else if(currentScene == "scene2")
            {
                BackGrnd.Initialize(_game, "scene2", "scene2Mask");
                _game.VisualLayers[VisualLayerID.BG].Add(BackGrnd);

            }
        }


        internal void Update()
        {
            if (_game.player.PlayerFired)
            {
                bool hitSomething = false;

                foreach(BackgroundHitObject o in _game.VisualLayers[VisualLayerID.BG1])
                {
                    //first check if the bullet hit an object on top of the background (ie. sign)
                    if (_game.CurrentMouseState.X >= o.Position.X && _game.CurrentMouseState.X <= o.Position.X + o.Texture.Width)
                    {
                        if (_game.CurrentMouseState.Y >= o.Position.Y && _game.CurrentMouseState.Y <= o.Position.Y + o.Texture.Height)
                        {
                            var p = o.GetColorAtPos((_game.CurrentMouseState.X - (int)o.Position.X), (_game.CurrentMouseState.Y - (int)o.Position.Y));

                            //see through
                            if (p.A != 0)
                            {
                                o.Hit();
                                hitSomething = true;
                            }
                        }
                    }

                }


                //if we missed all the background objects then check against the background directly.
                if (!hitSomething)
                {
                    Color pixel = BackGrnd.GetColorAtPos(_game.CurrentMouseState.X, _game.CurrentMouseState.Y);
                    //Debug.WriteLine($"Player clicked on color: {pixel}");
                    BackGrnd.CheckHitMaterial(pixel);
                }
            }

        }

    }
}
