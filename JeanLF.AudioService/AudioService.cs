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
        
        private void Test()
        {
            AudioMixerGroup group = default;
            AudioPlayerGroup playerGroup = new AudioPlayerGroup("te", group, new AudioPool(null,10,10));

            List<AudioPlayer> test = playerGroup.GetPlayingAudio();
        }

        public AudioService(AudioConfig configuration)
        {
            _configuration = configuration;
        }

        public void Play()
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
