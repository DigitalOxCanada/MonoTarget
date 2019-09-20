using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using MonoTarget.Screens;

namespace MonoTarget
{
    
    public class SceneManager
    {
        public string Name { get; set; }
        public string PackageName { get; set; }
        public Background BackGrnd { get; set; }
        public GameTimeLine gameTimeLine { get; set; }

        public string[] scenes = new string[] { "practiceScene1", "scene1", "scene2" };
        public int currentSceneIndex;

        private GameplayScreen _gameplayScreen;

        List<GameObject> objects { get; set; }

        public SceneManager(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;

            objects = new List<GameObject>();
            BackGrnd = new Background(_gameplayScreen);
            gameTimeLine = new GameTimeLine(_gameplayScreen);
        }

        internal void Initialize()
        {
            gameTimeLine.Initialize();
        }

        internal void LoadContent()
        {
            if (currentSceneIndex == 0)
            {
                BackGrnd.Initialize(scenes[currentSceneIndex], scenes[currentSceneIndex] + "Mask");
                _gameplayScreen.VisualLayers[VisualLayerID.BG].Add(BackGrnd);
            }
            //eventually make these generics that are loaded from a scene file for dynamic scenes
            else if (currentSceneIndex == 1)
            {
                BackGrnd.Initialize(scenes[currentSceneIndex], scenes[currentSceneIndex] + "Mask");
                _gameplayScreen.VisualLayers[VisualLayerID.BG].Add(BackGrnd);

                BarberPole BarbersPole = new BarberPole(_gameplayScreen);
                BarbersPole.Initialize("barberpole", new Vector2(538, 307));
                _gameplayScreen.VisualLayers[VisualLayerID.BG1].Add(BarbersPole);

                MetalSign sg = new MetalSign(_gameplayScreen);
                sg.Initialize("MetalPlate1");
                sg.Position = new Vector2(696, 322);
                sg.Life = 2;
                sg.Destination = new Vector2(sg.Position.X + new Random().Next(-20, 20), sg.Position.Y + 140);
                _gameplayScreen.VisualLayers[VisualLayerID.BG1].Add(sg);

                MetalSign sg1 = new MetalSign(_gameplayScreen);
                sg1.Initialize("bathhousesign1");
                sg1.Position = new Vector2(867, 218);
                sg1.Life = 10;
                sg1.Destination = new Vector2(sg1.Position.X + new Random().Next(-50, 50), sg1.Position.Y + 200);
                _gameplayScreen.VisualLayers[VisualLayerID.BG1].Add(sg1);

                MetalSign sg2 = new MetalSign(_gameplayScreen);
                sg2.Initialize("statue1");
                sg2.Position = new Vector2(855, 159);
                sg2.Life = 6;
                sg2.Destination = new Vector2(sg2.Position.X + new Random().Next(-20, 20), sg2.Position.Y + 300);
                _gameplayScreen.VisualLayers[VisualLayerID.BG1].Add(sg2);

                PioneerTownSign pts = new PioneerTownSign(_gameplayScreen);
                pts.Initialize("sign1");
                pts.Position = new Vector2(375, 100);
                _gameplayScreen.VisualLayers[VisualLayerID.BG1].Add(pts);
            }
            else if(currentSceneIndex == 2)
            {
                BackGrnd.Initialize(scenes[currentSceneIndex], scenes[currentSceneIndex] + "Mask");
                _gameplayScreen.VisualLayers[VisualLayerID.BG].Add(BackGrnd);
            }
        }


        internal void Update()
        {
            gameTimeLine.Update();

            if (_gameplayScreen.player.HealthCount < 1)
            {
                _gameplayScreen.ScreenManager.SoundManager.StopAllSounds();
                LoadingScreen.Load(_gameplayScreen.ScreenManager, false, null, new DeadScreen());

                return;
            }

            if (_gameplayScreen.player.PlayerFired)
            {
                bool hitSomething = false;

                foreach(BackgroundHitObject o in _gameplayScreen.VisualLayers[VisualLayerID.BG1])
                {
                    //first check if the bullet hit an object on top of the background (ie. sign)
                    if (_gameplayScreen.ScreenManager.input.CurrentMouseState.X >= o.Position.X && _gameplayScreen.ScreenManager.input.CurrentMouseState.X <= o.Position.X + o.Size.X)
                    {
                        if (_gameplayScreen.ScreenManager.input.CurrentMouseState.Y >= o.Position.Y && _gameplayScreen.ScreenManager.input.CurrentMouseState.Y <= o.Position.Y + o.Size.Y)
                        {
                            var p = o.GetColorAtPos((_gameplayScreen.ScreenManager.input.CurrentMouseState.X - (int)o.Position.X), (_gameplayScreen.ScreenManager.input.CurrentMouseState.Y - (int)o.Position.Y));

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
                    Debug.WriteLine("Hit something");
                    Color pixel = BackGrnd.GetColorAtPos(_gameplayScreen.ScreenManager.input.CurrentMouseState.X, _gameplayScreen.ScreenManager.input.CurrentMouseState.Y);
                    ////Debug.WriteLine($"Player clicked on color: {pixel}");
                    BackGrnd.CheckHitMaterial(pixel);
                }
            }

        }

        internal void Begin()
        {
            gameTimeLine.Begin();
        }
    }

}
