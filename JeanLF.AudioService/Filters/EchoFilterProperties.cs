using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    [Serializable]
    public struct EchoFilterProperties : IFilterProperty
    {
        public static readonly EchoFilterProperties DefaultValues = new EchoFilterProperties(500);

        [Min(10)] [SerializeField] private float _delay;
        [Range(0, 1f)] [SerializeField] private float _decayRatio;
        [Range(0, 1f)] [SerializeField] private float _wetMix;
        [Range(0, 1f)] [SerializeField] private float _dryMix;

        public EchoFilterProperties(float delay = 500)
        {
            _delay = delay;
            _decayRatio = 0.5f;
            _wetMix = 1.0f;
            _dryMix = 1.0f;
        }

        public IFilterProperty DefaultValue => DefaultValues;
        public Type FilterType => typeof(AudioEchoFilter);
        public float Delay => _delay;
        public float DecayRatio => _decayRatio;
        public float WetMix => _wetMix;
        public float DryMix => _dryMix;

        public void SetupFilter(ref Component component)
        {
            AudioEchoFilter filter = (AudioEchoFilter)component;
            filter.delay = _delay;
            filter.decayRatio = _decayRatio;
            filter.wetMix = _wetMix;
            filter.dryMix = _dryMix;
        }

        
    }
}
