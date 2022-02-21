using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    [Serializable]
    public struct EchoFilterProperties : IFilterProperty
    {
        [Min(10)] [SerializeField] private float _delay;
        [Range(0, 1f)] [SerializeField] private float _decayRatio;
        [Range(0, 1f)] [SerializeField] private float _wetMix;
        [Range(0, 1f)] [SerializeField] private float _dryMix;

        public Type FilterType => typeof(AudioEchoFilter);
        public float Delay => _delay;
        public float DecayRatio => _decayRatio;
        public float WetMix => _wetMix;
        public float DryMix => _dryMix;
    }
}