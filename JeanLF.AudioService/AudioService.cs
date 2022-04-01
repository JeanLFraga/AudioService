using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    internal class AudioService
    {
        private void Test()
        {
            AudioMixerGroup group = default;
            AudioPlayerGroup playerGroup = new AudioPlayerGroup("te", group, new AudioPool(10));

            List<AudioPlayer> test = playerGroup.GetPlayingAudio();
        }
    }
}
