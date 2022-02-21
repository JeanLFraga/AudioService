using System;
using UnityEngine;

namespace JeanLF.AudioService
{
    [Serializable]
    public struct AudioPlayerProperties
    {
        public static readonly AudioPlayerProperties DefaultValues = new AudioPlayerProperties(1f);
        
        [SerializeField] private bool _bypassListenerEffects;
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

        public AudioPlayerProperties(float volume = 1f)
        {
            _bypassListenerEffects = false;
            _loop = false;
            _priority = 128;
            _volume = volume;
            _minPitch = 1;
            _maxPitch = 1;
            _spatialBlend = 0;
            _stereoPan = 0;
            _reverbZoneMix = 1;
            _dopplerLevel = 1;
            _spread = 0;
            _volumeRolloff = AudioRolloffMode.Logarithmic;
            _minRolloff = 1;
            _maxRolloff = 500;
        }

        public AudioPlayerProperties(bool bypassListenerEffects, bool loop, int priority, float volume, float minPitch, float maxPitch, float spatialBlend, float stereoPan, float reverbZoneMix, float dopplerLevel, float spread, AudioRolloffMode volumeRolloff, float minRolloff, float maxRolloff)
        {
            _bypassListenerEffects = bypassListenerEffects;
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

        public bool BypassListenerEffects => _bypassListenerEffects;
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