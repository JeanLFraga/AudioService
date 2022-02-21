using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    public struct SoundGroupConfiguration
    {
        [SerializeField] private string _id;
        [SerializeField] private AudioMixerGroup _mixerGroup;
    }
}
