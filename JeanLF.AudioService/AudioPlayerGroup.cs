using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    internal class AudioPlayerGroup
    {
        private readonly HashSet<AudioPlayer> _playingAudios = new HashSet<AudioPlayer>();
        private readonly AudioMixerGroup _mixerGroup;
        private readonly AudioPool _pool;
        private readonly string _id;

        public AudioPlayerGroup(string id, AudioMixerGroup mixerGroup, AudioPool pool)
        {
            _id = id;
            _mixerGroup = mixerGroup;
            _pool = pool;
        }

        public void PlayAudio(string audioId, AudioClip clip, AudioPlayerProperties properties)
        {
            AudioPlayer player = _pool.GetAudioPlayer();

            player.Play(audioId, clip, properties);

            _playingAudios.Add(player);
        }

        private void StopAudio(string audioId)
        {
            foreach (AudioPlayer variable in _playingAudios)
            {
                if (variable.CurrentId == audioId)
                {
                    variable.Stop();
                }
            }
        }

        private void StopAll()
        {
            foreach (AudioPlayer variable in _playingAudios)
            {
                variable.Stop();
            }
        }

        [LinqTunnel]
        public List<AudioPlayer> GetPlayingAudio()
        {
            return _playingAudios.ToList();
        }
    }
}
