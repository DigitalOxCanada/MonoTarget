using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTarget
{
    public class SoundManager : GameComponent
    {
        public float SoundVolume { get; set; }
        public float MusicVolume { get; set; }
        public float MasterVolume { get; set; }

        public Dictionary<string, SoundEffect> SoundLibrary { get; set; } = new Dictionary<string, SoundEffect>();
        public Dictionary<string, Song> MusicLibrary { get; set; } = new Dictionary<string, Song>();
        public List<SoundEffectInstance> SoundEffects = new List<SoundEffectInstance>();

        public SoundManager(Game game) : base(game)
        {
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
            if (v < 0.0f || v > 1.0f)
                throw new ArgumentOutOfRangeException();

            if (MasterVolume == v)
                return;

            MasterVolume = v;
            //overrides sound and music volume
            MediaPlayer.Volume = v;
        }

        public SoundEffectInstance PlaySound(string nameOfSound, float volume = 1.0f)
        {
            if (string.IsNullOrEmpty(nameOfSound)) return null;

            var newvol = Math.Min(volume, SoundVolume);
            newvol = Math.Min(newvol, MasterVolume);

            SoundEffectInstance s;
            s = SoundLibrary[nameOfSound].CreateInstance();
            s.Volume = newvol;
            s.Play();
            SoundEffects.Add(s);
            return s;
        }

        public void PlayMusic(string nameOfSong, float volume = 1.0f)
        {
            var newvol = Math.Min(volume, SoundVolume);
            newvol = Math.Min(newvol, MasterVolume);

            MediaPlayer.Volume = MasterVolume;
            MediaPlayer.Play(MusicLibrary[nameOfSong]);
        }

        internal void AddSound(string nameOfSound, string nameOfResource)
        {
            ContentManager content = Game.Content;
            if (!SoundLibrary.ContainsKey(nameOfSound))
            {
                SoundLibrary.Add(nameOfSound, content.Load<SoundEffect>(nameOfResource));
            }
        }
        internal void AddMusic(string nameOfSong, string nameOfResource)
        {
            ContentManager content = Game.Content;
            if (!MusicLibrary.ContainsKey(nameOfSong))
            {
                MusicLibrary.Add(nameOfSong, content.Load<Song>(nameOfResource));
            }
        }

        internal void StopAllSounds()
        {
            MediaPlayer.Stop();
            foreach(var s in SoundEffects)
            {
                if(s.State == SoundState.Playing)
                {
                    s.Stop();
                }
            }
            SoundEffects.Clear();
        }
    }
}
