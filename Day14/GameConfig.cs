using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MonoTarget
{
    public class GameConfig
    {
        public bool MusicEnabled { get; set; } = true;
        public bool SoundEnabled { get; set; } = true;
        public bool PracticeCompleted { get; set; } = false;
        public int Crosshair { get; internal set; } = 0;
        public int Gun { get; internal set; } = 0;
    }

    public class GameConfigManager
    {
        public GameConfig gameConfig { get; set; }
        private string gameConfigFN = "gameconfig.json";

        public GameConfigManager()
        {

        }


        public void Load()
        {
            if (!System.IO.File.Exists(gameConfigFN))
            {
                System.IO.File.WriteAllText(gameConfigFN, JsonConvert.SerializeObject(gameConfig));
            }
            else
            {
                string cfgjson = System.IO.File.ReadAllText(gameConfigFN);
                gameConfig = JsonConvert.DeserializeObject<GameConfig>(cfgjson);
            }
        }

        public void Save()
        {
            System.IO.File.WriteAllText(gameConfigFN, JsonConvert.SerializeObject(gameConfig));
        }
    }

}
