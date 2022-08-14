using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    [Serializable]
    public struct ReverbFilterProperties : IFilterProperty
    {
        public static readonly ReverbFilterProperties DefaultValues = new ReverbFilterProperties(AudioReverbPreset.User);

        [SerializeField] private AudioReverbPreset _reverbPreset;
        [Range(-10000, 0)] [SerializeField] private float _dryLevel;
        [Range(-10000, 0)] [SerializeField] private float _room;
        [Range(-10000, 0)] [SerializeField] private float _roomHF;
        [Range(-10000, 0)] [SerializeField] private float _roomLF;
        [Range(0.1f, 20f)] [SerializeField] private float _decayTime;
        [Range(0.1f, 2f)] [SerializeField] private float _decayHFRatio;
        [Range(-10000, 1000)] [SerializeField] private float _reflectionsLevel;
        [Range(0, 0.3f)] [SerializeField] private float _reflectionsDelay;
        [Range(-10000, 2000)] [SerializeField] private float _reverbLevel;
        [Range(0f, 0.1f)] [SerializeField] private float _reverbDelay;
        [Range(1000, 20000)] [SerializeField] private float _HFReference;
        [Range(20, 1000)] [SerializeField] private float _LFReference;
        [Range(0, 100)] [SerializeField] private float _diffusion;
        [Range(0, 100)] [SerializeField] private float _density;

        public ReverbFilterProperties(AudioReverbPreset preset = AudioReverbPreset.User)
        {
            _reverbPreset = preset;
            _dryLevel = 0;
            _room = 0;
            _roomHF = 0;
            _roomLF = 0;
            _decayTime = 1.0f;
            _decayHFRatio = 0.5f;
            _reflectionsLevel = -10000;
            _reflectionsDelay = 0;
            _reverbLevel = 0;
            _reverbDelay = 0.04f;
            _HFReference = 5000;
            _LFReference = 1000;
            _diffusion = 100;
            _density = 100;
        }

        public IFilterProperty DefaultValue => DefaultValues;
        public Type FilterType => typeof(AudioReverbFilter);
        public AudioReverbPreset ReverbPreset => _reverbPreset;
        public float DryLevel => _dryLevel;
        public float Room => _room;
        public float RoomHF => _roomHF;
        public float RoomLF => _roomLF;
        public float DecayTime => _decayTime;
        public float DecayHFRatio => _decayHFRatio;
        public float ReflectionsLevel => _reflectionsLevel;
        public float ReflectionsDelay => _reflectionsDelay;
        public float ReverbLevel => _reverbLevel;
        public float ReverbDelay => _reverbDelay;
        public float HFReference => _HFReference;
        public float LFReference => _LFReference;
        public float Diffusion => _diffusion;
        public float Density => _density;

        public void SetupFilter(ref Component component)
        {
            AudioReverbFilter filter = (AudioReverbFilter)component;

            if (_reverbPreset != AudioReverbPreset.User)
            {
                filter.reverbPreset = _reverbPreset;
                return;
            }

            filter.dryLevel = _dryLevel;
            filter.room = _room;
            filter.roomHF = _roomHF;
            filter.roomLF = _roomLF;
            filter.decayTime = _decayTime;
            filter.decayHFRatio = _decayHFRatio;
            filter.reflectionsLevel = _reflectionsLevel;
            filter.reflectionsDelay = _reflectionsDelay;
            filter.reverbLevel = _reverbLevel;
            filter.reverbDelay = _reverbDelay;
            filter.hfReference = _HFReference;
            filter.lfReference = _LFReference;
            filter.diffusion = _diffusion;
            filter.density = _density;
        }
    }
}
