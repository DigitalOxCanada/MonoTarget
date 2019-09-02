using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameStateManagement;
using Microsoft.Xna.Framework;

namespace MonoTarget
{
    public enum GameTimeLineActionType
    {
        Noop = 0,
        GenerateEnemy = 1,
        GenerateTarget = 2,
        GenerateFx = 3,
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
        private GameplayScreen _gameplayScreen;

        //list of actions and at what timepoint
        private List<GameTimeLineAction> TimeLineActions { get; set; }
        private DateTime StartTime { get; set; }
        private string[] enemies = new string[] { "targetman1", "targetman2", "target1" };
        private string[] targets = new string[] { "target-100", "target-200", "target2-100", "target2-200" };

        public GameTimeLine(GameplayScreen gameplayScreen)
        {
            _gameplayScreen = gameplayScreen;
            TimeLineActions = new List<GameTimeLineAction>();
        }

        public void Initialize()
        {
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateTarget, When = TimeSpan.FromSeconds(3.0), GetAction = MakeTarget });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateTarget, When = TimeSpan.FromSeconds(6.5), GetAction = MakeTarget });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateTarget, When = TimeSpan.FromSeconds(9.0), GetAction = MakeTarget });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateTarget, When = TimeSpan.FromSeconds(12.0), GetAction = MakeTarget });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateTarget, When = TimeSpan.FromSeconds(15.0), GetAction = MakeTarget });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(12.0), GetAction = MakeEnemy });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(8.0), GetAction = MakeEnemy });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(2.5), GetAction = MakeEnemy });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(6.0), GetAction = MakeEnemy });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(5.0), GetAction = MakeEnemy });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(4.0), GetAction = MakeEnemy });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(2.0), GetAction = MakeEnemy });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(5.0), GetAction = MakeBirds });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateFx, When = TimeSpan.FromSeconds(15.0), GetAction = Thunder });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateFx, When = TimeSpan.FromSeconds(28.0), GetAction = Thunder });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateFx, When = TimeSpan.FromSeconds(10.0), GetAction = Crow });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateFx, When = TimeSpan.FromSeconds(20.0), GetAction = Crow });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateFx, When = TimeSpan.FromSeconds(32.0), GetAction = Crow });

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
            var rand = new Random();
            Target t = new Target(_gameplayScreen);
            t.Initialize(targets[rand.Next(0, targets.Count())]);  //pick a random target
            t.Position = new Vector2(rand.Next(100, ((Game1)_gameplayScreen.ScreenManager.Game).BufferWidth - 100 - t.Texture.Width), rand.Next((int)((Game1)_gameplayScreen.ScreenManager.Game).BufferHeight/2 - 200, 
                (int)((Game1)_gameplayScreen.ScreenManager.Game).BufferHeight/2 + 100));
            _gameplayScreen.Targets.Add(t);
            _gameplayScreen.VisualLayers[VisualLayerID.Objects].Add(t);
        }

        private void MakeEnemy()
        {
            var rand = new Random();
            Target t = new Target(_gameplayScreen);
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
