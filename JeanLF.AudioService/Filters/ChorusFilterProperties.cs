using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    [Serializable]
    public struct ChorusFilterProperties : IFilterProperty
    {
        public static readonly ChorusFilterProperties DefaultValues = new ChorusFilterProperties(0.5f);
        
        [Range(0,1)] [SerializeField] private float _dryMix;
        [Range(0,1)] [SerializeField] private float _wetMix1;
        [Range(0,1)] [SerializeField] private float _wetMix2;
        [Range(0,1)] [SerializeField] private float _wetMix3;
        [Range(0.1f,100f)] [SerializeField] private float _delay;
        [Range(0,20)] [SerializeField] private float _rate;
        [Range(0,1)] [SerializeField] private float _depth;
        [Range(0,1)] [SerializeField] private float _feedback;

        public ChorusFilterProperties(float dryMix = 0.5f)
        {
            _dryMix = dryMix;
            _wetMix1 = 0.5f;
            _wetMix2 = 0.5f;
            _wetMix3 = 0.5f;
            _delay = 40;
            _rate = 0.8f;
            _depth = 0.03f;
            _feedback = 0;
        }

        public Type FilterType => typeof(AudioChorusFilter);
        public float DryMix => _dryMix;
        public float WetMix1 => _wetMix1;
        public float WetMix2 => _wetMix2;
        public float WetMix3 => _wetMix3;
        public float Delay => _delay;
        public float Rate => _rate;
        public float Depth => _depth;
        public float Feedback => _feedback;
    }
}