using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    [Serializable]
    public struct HighPassFilterProperties : IFilterProperty
    {
        public static readonly HighPassFilterProperties DefaultValues = new HighPassFilterProperties(5000);

        [Range(0,22000)] [SerializeField] private float _cutoffFrequency;
        [Range(1,10)] [SerializeField] private float _resonanceQ;

        public HighPassFilterProperties(float cutoffFrequency = 5000)
        {
            _cutoffFrequency = cutoffFrequency;
            _resonanceQ = 1.0f;
        }

        public float CutoffFrequency => _cutoffFrequency;
        public float ResonanceQ => _resonanceQ;
        public Type FilterType => typeof(AudioHighPassFilter);

        public void SetupFilter(ref Component component)
        {
            AudioHighPassFilter filter = (AudioHighPassFilter)component;
            filter.cutoffFrequency = _cutoffFrequency;
            filter.highpassResonanceQ = _resonanceQ;
        }
    }
}
