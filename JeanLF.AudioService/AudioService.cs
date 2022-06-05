using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    public sealed class AudioService : IAudioService
    {
        private readonly AudioConfig _configuration;
        private readonly AudioPool _pool;
        private readonly HashSet<AudioPlayerGroup> _audioGroups = new HashSet<AudioPlayerGroup>();

        private void Test()
        {
            AudioMixerGroup group = default;
            AudioPlayerGroup playerGroup = new AudioPlayerGroup("te", group, new AudioPool(null,10,10));

            IReadOnlyList<AudioPlayer> test = playerGroup.GetPlayingAudio();
        }

        public AudioService()
        {
            AudioServiceSettings settings = Resources.Load<AudioServiceSettings>("JeanLF_AS_Settings.asset");
            _configuration = settings.Configuration;

            _pool = new AudioPool(_configuration, settings.PoolSize, settings.FilteredSources);
            IReadOnlyList<AudioGroup> groups = _configuration.AudioGroups;

            for (int i = 0; i < groups.Count; i++)
            {
                _audioGroups.Add(new AudioPlayerGroup(groups[i].ID, groups[i].MixerGroup, _pool));
            }
        }

        public UniTask Play(AudioReference audio, AudioPlayerProperties? overrideProperties = null)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void PauseGroup()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void StopGroup()
        {
            throw new NotImplementedException();
        }

        public void StopAll()
        {
            throw new NotImplementedException();
        }
    }
}
