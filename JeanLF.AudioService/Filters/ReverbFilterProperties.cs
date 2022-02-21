using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    [Serializable]
    public struct ReverbFilterProperties : IFilterProperty
    {
        public static readonly ReverbFilterProperties DefaultValues = new ReverbFilterProperties(AudioReverbPreset.User);
        
        [SerializeField] private AudioReverbPreset _reverbPreset;
        [Range(-10000, 0)] [SerializeField] private float _DryLevel;
        [Range(-10000, 0)] [SerializeField] private float _Room;
        [Range(-10000, 0)] [SerializeField] private float _RoomHF;
        [Range(-10000, 0)] [SerializeField] private float _RoomLF;
        [Range(0.1f, 20f)] [SerializeField] private float _DecayTime;
        [Range(0.1f, 2f)] [SerializeField] private float _DecayHFRatio;
        [Range(-10000, 1000)] [SerializeField] private float _ReflectionsLevel;
        [Range(0, 0.3f)] [SerializeField] private float _ReflectionsDelay;
        [Range(-10000, 2000)] [SerializeField] private float _ReverbLevel;
        [Range(0f, 0.1f)] [SerializeField] private float _ReverbDelay;
        [Range(1000, 20000)] [SerializeField] private float _HFReference;
        [Range(20, 1000)] [SerializeField] private float _LFReference;
        [Range(0, 100)] [SerializeField] private float _Diffusion;
        [Range(0, 100)] [SerializeField] private float _Density;

        public ReverbFilterProperties(AudioReverbPreset preset)
        {
            _reverbPreset = preset;
            _DryLevel = 0;
            _Room = 0;
            _RoomHF = 0;
            _RoomLF = 0;
            _DecayTime = 1.0f;
            _DecayHFRatio = 0.5f;
            _ReflectionsLevel = -10000;
            _ReflectionsDelay = 0;
            _ReverbLevel = 0;
            _ReverbDelay = 0.04f;
            _HFReference = 5000;
            _LFReference = 1000;
            _Diffusion = 100;
            _Density = 100;
        }

        public Type FilterType => typeof(AudioReverbFilter);
        public AudioReverbPreset ReverbPreset => _reverbPreset;
        public float DryLevel => _DryLevel;
        public float Room => _Room;
        public float RoomHF => _RoomHF;
        public float RoomLF => _RoomLF;
        public float DecayTime => _DecayTime;
        public float DecayHFRatio => _DecayHFRatio;
        public float ReflectionsLevel => _ReflectionsLevel;
        public float ReflectionsDelay => _ReflectionsDelay;
        public float ReverbLevel => _ReverbLevel;
        public float ReverbDelay => _ReverbDelay;
        public float HFReference => _HFReference;
        public float LFReference => _LFReference;
        public float Diffusion => _Diffusion;
        public float Density => _Density;
    }
}