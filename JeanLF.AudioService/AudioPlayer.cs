using Cysharp.Threading.Tasks;
using JeanLF.AudioService.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JeanLF.AudioService
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;
        private Dictionary<Type, Component> _filters;
        public string CurrentId { get; private set; }
        public bool IsPlaying => _audioSource.isPlaying;

        internal void Setup()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        internal void Setup(IFilterProperty[] filters)
        {
            _filters = new Dictionary<Type, Component>();
            for (int i = 0; i < filters.Length; i++)
            {
                Component filterComponent = gameObject.AddComponent(filters[i].FilterType);
                filters[i].SetupFilter(ref filterComponent);
                _filters.Add(filters[i].FilterType, filterComponent);
            }
        }

        public void Pause()
        {
            _audioSource.Pause();
        }

        public void UnPause()
        {
            _audioSource.UnPause();
        }

        internal async UniTask Play(string audioId, AudioClip clip, AudioPlayerProperties playerProperties, IFilterProperty[] filterProperties)
        {
            CurrentId = audioId;

            SetAudioProperties(playerProperties);

            _audioSource.clip = clip;
            _audioSource.Play();
            await UniTask.WaitWhile(() => IsPlaying);

        }

        internal void Stop()
        {
            _audioSource.Stop();
        }

        private void SetAudioProperties(AudioPlayerProperties properties)
        {
            _audioSource.bypassEffects = properties.BypassEffects;
            _audioSource.loop = properties.Loop;
            _audioSource.priority = properties.Priority;
            _audioSource.volume = properties.Volume;
            _audioSource.pitch = UnityEngine.Random.Range(properties.MinPitch, properties.MaxPitch);
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
