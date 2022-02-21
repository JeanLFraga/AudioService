using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    [Serializable]
    public struct HighPassFilterProperties : IFilterProperty
    {
        [SerializeField] private float _cutoffFrequency;
        [SerializeField] private float _resonanceQ;

        public float CutoffFrequency => _cutoffFrequency;
        public float ResonanceQ => _resonanceQ;
        public Type FilterType => typeof(AudioHighPassFilter);
    }
}
