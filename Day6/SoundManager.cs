using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTarget
{
    public class SoundManager
    {
        private Game1 _game;

        public Dictionary<string, SoundEffect> SoundLibrary { get; set; } = new Dictionary<string, SoundEffect>();
        public Dictionary<string, Song> MusicLibrary { get; set; } = new Dictionary<string, Song>();

        public SoundManager(Game1 game)
        {
            _game = game;
        }

        internal void AddSound(string nameOfSound, string nameOfResource)
        {
            SoundLibrary.Add(nameOfSound, _game.Content.Load<SoundEffect>(nameOfResource));
        }
        internal void AddMusic(string nameOfSong, string nameOfResource)
        {
            MusicLibrary.Add(nameOfSong, _game.Content.Load<Song>(nameOfResource));
        }
    }
}
