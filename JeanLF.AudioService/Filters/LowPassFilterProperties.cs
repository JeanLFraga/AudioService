using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    [Serializable]
    public struct LowPassFilterProperties : IFilterProperty
    {
        public static readonly LowPassFilterProperties DefaultValues = new LowPassFilterProperties(5000);

        [Range(0,22000)] [SerializeField] private float _cutoffFrequency;
        [Range(1,10)] [SerializeField] private float _resonanceQ;

        public LowPassFilterProperties(float cutoffFrequency = 5000)
        {
            _cutoffFrequency = cutoffFrequency;
            _resonanceQ = 1.0f;
        }

        public float CutoffFrequency => _cutoffFrequency;
        public float ResonanceQ => _resonanceQ;
        public Type FilterType => typeof(AudioLowPassFilter);

        public void SetupFilter(ref Component component)
        {
            AudioLowPassFilter filter = (AudioLowPassFilter)component;
            filter.cutoffFrequency = _cutoffFrequency;
            filter.lowpassResonanceQ = _resonanceQ;
        }
    }
}
