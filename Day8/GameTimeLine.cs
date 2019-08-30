using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoTarget
{
    public enum GameTimeLineActionType
    {
        Noop = 0,
        GenerateEnemy = 1
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
        private Game1 _game;
        //list of actions and at what timepoint
        private List<GameTimeLineAction> TimeLineActions { get; set; }
        private DateTime StartTime { get; set; }

        public GameTimeLine(Game1 game)
        {
            _game = game;
            TimeLineActions = new List<GameTimeLineAction>();

            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(8.0), GetAction = MakeEnemy });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(3.0), GetAction = MakeEnemy });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(5.0), GetAction = MakeBirds });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(15.0), GetAction = Thunder });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(28.0), GetAction = Thunder });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(10.0), GetAction = Crow });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(20.0), GetAction = Crow });
            TimeLineActions.Add(new GameTimeLineAction() { ActionType = GameTimeLineActionType.GenerateEnemy, When = TimeSpan.FromSeconds(32.0), GetAction = Crow });

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

                    //trigger action
                    action.GetAction();

                }
            }
        }

        private void MakeEnemy()
        {
            Target t = new Target();
            t.Initialize(_game, "target1", _game.centerPosition.X, _game.centerPosition.Y);
            _game.Targets.Add(t);
            _game.VisualLayers[VisualLayerID.Objects].Add(t);
        }

        private void MakeBirds()
        {

        }

        private void Thunder()
        {
            //play thunder sound
            _game.SoundManager.PlaySound("thunder1");
        }
        private void Crow()
        {
            _game.SoundManager.PlaySound("crowcaw1");
        }
    }
}
