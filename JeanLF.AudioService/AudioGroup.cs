using System;
using UnityEngine;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    [Serializable]
    internal struct AudioGroup
    {
        #if UNITY_EDITOR
        internal static readonly string IdPropertyPath = nameof(_id);
        #endif

        [SerializeField] private string _id;
        [SerializeField] private AudioMixerGroup _mixerGroup;

        public string ID => _id;
        public AudioMixerGroup MixerGroup => _mixerGroup;
    }
}
