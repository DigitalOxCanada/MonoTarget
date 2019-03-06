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
        public float SoundVolume { get; set; }
        public float MusicVolume { get; set; }
        public float MasterVolume { get; set; }

        public Dictionary<string, SoundEffect> SoundLibrary { get; set; } = new Dictionary<string, SoundEffect>();
        public Dictionary<string, Song> MusicLibrary { get; set; } = new Dictionary<string, Song>();

        public SoundManager(Game1 game)
        {
            _game = game;
            SoundVolume = 1.0f;
            MusicVolume = 1.0f;
            MasterVolume = 1.0f;
        }

        public void SetSoundVolume(float v)
        {
            SoundVolume = v;
        }

        public void SetMusicVolume(float v)
        {
            MusicVolume = v;
        }

        public void SetMasterVolume(float v)
        {
            MasterVolume = v;
            //overrides sound and music volume
            MediaPlayer.Volume = v;
        }

        public void PlaySound(string nameOfSound, float volume)
        {
            var newvol = Math.Min(volume, SoundVolume);
            newvol = Math.Min(newvol, MasterVolume);

            SoundEffectInstance s;
            s = SoundLibrary[nameOfSound].CreateInstance();
            s.Volume = newvol;
            s.Play();
        }
        public void PlayMusic(string nameOfSong, float volume)
        {
            var newvol = Math.Min(volume, SoundVolume);
            newvol = Math.Min(newvol, MasterVolume);

            MediaPlayer.Play(MusicLibrary[nameOfSong]);
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
