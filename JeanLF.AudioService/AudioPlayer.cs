using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JeanLF.AudioService
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioChorusFilter))]
    [RequireComponent(typeof(AudioDistortionFilter))]
    [RequireComponent(typeof(AudioEchoFilter))]
    [RequireComponent(typeof(AudioHighPassFilter))]
    [RequireComponent(typeof(AudioLowPassFilter))]
    [RequireComponent(typeof(AudioReverbFilter))]
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;
        public string CurrentId { get; private set; }

        public void Setup()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(string audioId, AudioClip clip, AudioPlayerProperties properties)
        {
            CurrentId = audioId;

            SetProperties(properties);

            _audioSource.clip = clip;
            _audioSource.Play();
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void Pause()
        {
            _audioSource.Pause();
        }

        private void SetProperties(AudioPlayerProperties properties)
        {
            _audioSource.bypassEffects = properties.BypassEffects;
            _audioSource.loop = properties.Loop;
            _audioSource.priority = properties.Priority;
            _audioSource.volume = properties.Volume;
            _audioSource.pitch = Random.Range(properties.MinPitch, properties.MaxPitch);
            _audioSource.spatialBlend = properties.SpatialBlend;
            _audioSource.panStereo = properties.StereoPan;
            _audioSource.reverbZoneMix = properties.ReverbZoneMix;
            _audioSource.dopplerLevel = properties.DopplerLevel;
            _audioSource.spread = properties.Spread;
            _audioSource.rolloffMode = properties.VolumeRolloff;
            _audioSource.minDistance = properties.MinRolloff;
            _audioSource.maxDistance = properties.MaxRolloff;
        }
    }
}
