using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GameStateManagement;
using Microsoft.Xna.Framework;
using MonoTarget.Screens;
using Newtonsoft.Json;

namespace MonoTarget
{

    public class EventsItem
    {
        public float time_offset { get; set; }
        public string obj_type { get; set; }
        public string obj_subtype { get; set; }
        public float lifetime { get; set; }
    }


    public enum GameTimeLineActionType
    {
        Noop = 0,
        GenerateEnemy = 1,
        GenerateTarget = 2,
        GenerateFx = 3,
        GenerateCrate = 4,
        TriggerEnd
    }
    public enum GameTimeLineActionStatus
    {
        Alive = 1,
        Fired = 2
    }

    public class GameTimeLineAction
    {
        public GameTimeLineActionType ActionType { get; set; } = GameTimeLineActionType.Noop;
        public TimeSpan When { get; set; } = TimeSpan.Zero;
        public Action GetAction { get; set; }
        public GameTimeLineActionStatus Status { get; set; }
    }


    public class GameTimeLine
    {
        public List<EventsItem> events { get; set; }

        private Random rand = new Random();
        private GameplayScreen _gameplayScreen;

        //list of actions and at what timepoint
        private List<GameTimeLineAction> TimeLineActions { get; set; }
        private DateTime StartTime { get; set; }
        private string[] enemies = new string[] { "targetman2", "target1" };
        private string[] targets = new string[] { "targetman1", "target-200" }; //, "target-200", "manrun1", }; //"target-100", "target-200", "target2-100", "target2-200" };

        public GameTimeLine(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
            TimeLineActions = new List<GameTimeLineAction>();
        }

        public void Initialize()
        {
            if (_gameplayScreen.ScreenManager.PracticeMode)
            {
                var filecontents = File.ReadAllText(@"Content\tl-practice.txt");
                events = JsonConvert.DeserializeObject<List<EventsItem>>(filecontents);
            }
            else
            {
                var filecontents = File.ReadAllText(@"Content\tl-scene1.txt");
                events = JsonConvert.DeserializeObject<List<EventsItem>>(filecontents);
            }

            if (events != null)
            {
                TimeSpan latest = TimeSpan.Zero;
                foreach (var e in events)
                {
                    GameTimeLineAction a = new GameTimeLineAction();
                    a.When = TimeSpan.FromSeconds(e.time_offset);
                    if (e.obj_type == "target")
                    {
                        a.ActionType = GameTimeLineActionType.GenerateTarget;
                        a.GetAction = MakeTarget;
                    }
                    else if (e.obj_type == "enemy")
                    {
                        a.ActionType = GameTimeLineActionType.GenerateEnemy;
                        a.GetAction = MakeEnemy;
                    }
                    else if (e.obj_type == "fx")
                    {
                        a.ActionType = GameTimeLineActionType.GenerateFx;
                        if (e.obj_subtype == "thunder")
                        {
                            a.GetAction = Thunder;
                        }
                        else if (e.obj_subtype == "crow")
                        {
                            a.GetAction = Crow;
                        }
                    }
                    else if (e.obj_type == "ammo")
                    {
                        a.ActionType = GameTimeLineActionType.GenerateCrate;
                        a.GetAction = MakeAmmo;
                    }
                    TimeLineActions.Add(a);
                    if (a.When > latest) latest = a.When;
                }
                
                //add an event that is for triggering the end
                GameTimeLineAction end = new GameTimeLineAction();
                end.When = latest + TimeSpan.FromSeconds(5);
                end.GetAction = EndLevel;
                TimeLineActions.Add(end);

                //TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateFx, When = TimeSpan.FromSeconds(20.0), GetAction = Crow });
                //TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateFx, When = TimeSpan.FromSeconds(32.0), GetAction = Crow });

            }
        }

        private void EndLevel()
        {
            _gameplayScreen.ScreenManager.SoundManager.StopAllSounds();
            LoadingScreen.Load(_gameplayScreen.ScreenManager, false, null, new LevelCompleteBackgroundScreen());
        }

        private void MakeAmmo()
        {
            AmmBox ab = new AmmBox(_gameplayScreen);
            ab.Initialize("ammobox", rand.Next(50, 1000), rand.Next(600, 650));
            _gameplayScreen.VisualLayers[VisualLayerID.Objects].Add(ab);
        }

        public void Begin()
        {
            StartTime = DateTime.Now;
            TimeLineActions = TimeLineActions.OrderBy(p => p.When).ToList();
        }

        public void Update()
        {
            foreach(var action in TimeLineActions.Where(x => x.Status != GameTimeLineActionStatus.Fired))
            {
                if( (StartTime.Ticks + action.When.Ticks) < DateTime.Now.Ticks )
                {
                    action.Status = GameTimeLineActionStatus.Fired;

                    Debug.WriteLine("TimeLineAction: {0} : {1}", action.ActionType, action.When);
                    //trigger action
                    action.GetAction();

                }
            }
        }

        private void MakeTarget()
        {
            Target t = new Target(_gameplayScreen);
            t.IsAnimated = true;
            if(t.IsAnimated)
            {
                t.Initialize(targets[rand.Next(0, targets.Count())],
                    //0,500
                    rand.Next(5, 1100),
                    rand.Next((int)((Game1)_gameplayScreen.ScreenManager.Game).BufferHeight / 2 - 150,
                              (int)((Game1)_gameplayScreen.ScreenManager.Game).BufferHeight - 250)
                    );
                t.animationManager.PlayOnce = true;

                _gameplayScreen.Targets.Add(t);
                _gameplayScreen.ScreenManager.SoundManager.PlaySound("catapult");
                _gameplayScreen.VisualLayers[VisualLayerID.Objects].Add(t);
            }
            else
            {
                //t.Position = new Vector2(rand.Next(100, ((Game1)_gameplayScreen.ScreenManager.Game).BufferWidth - 100 - t.Texture.Width), rand.Next((int)((Game1)_gameplayScreen.ScreenManager.Game).BufferHeight / 2 - 200,
                //    (int)((Game1)_gameplayScreen.ScreenManager.Game).BufferHeight / 2 + 100));
            }
        }

        private void MakeEnemy()
        {
            Target t = new Target(_gameplayScreen);
            t.IsEnemy = true;
            t.Initialize(enemies[rand.Next(0, enemies.Count())]);  //pick a random targetman
            t.Position = new Vector2(rand.Next(40, ((Game1)_gameplayScreen.ScreenManager.Game).BufferWidth - 40 - t.Texture.Width), rand.Next((int)((Game1)_gameplayScreen.ScreenManager.Game).BufferHeight/2 - 100,
                (int)((Game1)_gameplayScreen.ScreenManager.Game).BufferHeight/2 + 100));
            _gameplayScreen.Targets.Add(t);
            _gameplayScreen.VisualLayers[VisualLayerID.Objects].Add(t);
        }

        private void MakeBirds()
        {
        }

        private void Thunder()
        {
            //play thunder sound
            _gameplayScreen.ScreenManager.SoundManager.PlaySound("thunder1");
        }
        private void Crow()
        {
            _gameplayScreen.ScreenManager.SoundManager.PlaySound("crowcaw1");
        }
    }
}
