using System;
using UnityEngine;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    [Serializable]
    internal struct AudioGroup
    {
        [SerializeField] private string _id;
        [SerializeField] private AudioMixerGroup _mixerGroup;

        public string ID => _id;
        public AudioMixerGroup MixerGroup => _mixerGroup;
    }
}
