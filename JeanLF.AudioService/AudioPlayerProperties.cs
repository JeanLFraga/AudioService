using System;
using UnityEngine;

namespace JeanLF.AudioService
{
    [Serializable]
    public struct AudioPlayerProperties
    {
        public static readonly AudioPlayerProperties DefaultValues = new AudioPlayerProperties(volume:1f);

        [SerializeField] private bool _bypassEffects;
        [SerializeField] private bool _loop;
        [Range(0, 256)] [SerializeField] private int _priority;
        [Range(0, 1)] [SerializeField] private float _volume;
        [Range(-3, 3)] [SerializeField] private float _minPitch;
        [Range(-3, 3)] [SerializeField] private float _maxPitch;
        [Range(0, 1)] [SerializeField] private float _spatialBlend;
        [Range(-1, 1)] [SerializeField] private float _stereoPan;
        [Range(0, 1.1f)] [SerializeField] private float _reverbZoneMix;
        [Range(0, 5)] [SerializeField] private float _dopplerLevel;
        [Range(0, 360)] [SerializeField] private float _spread;
        [SerializeField] private AudioRolloffMode _volumeRolloff;
        [SerializeField] private float _minRolloff;
        [SerializeField] private float _maxRolloff;

        public AudioPlayerProperties(bool bypassEffects = false,
            bool loop = false,
            int priority = 128,
            float volume = 1f,
            float minPitch = 1f,
            float maxPitch = 1f,
            float spatialBlend = 0,
            float stereoPan = 0,
            float reverbZoneMix = 1,
            float dopplerLevel = 1,
            float spread = 0,
            AudioRolloffMode volumeRolloff = AudioRolloffMode.Logarithmic,
            float minRolloff = 1,
            float maxRolloff = 500)
        {
            _bypassEffects = bypassEffects;
            _loop = loop;
            _priority = priority;
            _volume = volume;
            _minPitch = minPitch;
            _maxPitch = maxPitch;
            _spatialBlend = spatialBlend;
            _stereoPan = stereoPan;
            _reverbZoneMix = reverbZoneMix;
            _dopplerLevel = dopplerLevel;
            _spread = spread;
            _volumeRolloff = volumeRolloff;
            _minRolloff = minRolloff;
            _maxRolloff = maxRolloff;
        }

        public bool BypassEffects => _bypassEffects;
        public bool Loop => _loop;
        public int Priority => _priority;
        public float Volume => _volume;
        public float MinPitch => _minPitch;
        public float MaxPitch => _maxPitch;
        public float SpatialBlend => _spatialBlend;
        public float StereoPan => _stereoPan;
        public float ReverbZoneMix => _reverbZoneMix;
        public float DopplerLevel => _dopplerLevel;
        public float Spread => _spread;
        public AudioRolloffMode VolumeRolloff => _volumeRolloff;
        public float MinRolloff => _minRolloff;
        public float MaxRolloff => _maxRolloff;
    }
}
