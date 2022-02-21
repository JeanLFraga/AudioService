using System;
using UnityEngine;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    [Serializable]
    public struct AudioGroup
    {
        [SerializeField] private string _id;
        [SerializeField] private AudioMixerGroup _mixerGroup;
    }
}
