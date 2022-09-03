using Cysharp.Threading.Tasks;
using JeanLF.AudioService.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour, IDisposable
    {
        private AudioSource _audioSource;
        private Dictionary<Type, Component> _filters;
        private AudioEntry _currentEntry;
        private Transform _cachedTransform;

        internal Action onKill;

        public EntryId CurrentId => _currentEntry.ConvertedId;
        public bool IsPaused { get; private set; }
        public bool IsPlaying => _audioSource.isPlaying;
        internal bool IsActive => _audioSource.isPlaying || IsPaused;

        public AudioPlayer Attach(Transform parent)
        {
            _cachedTransform.parent = parent;
            _cachedTransform.localPosition = Vector3.zero;

            return this;
        }

        public AudioPlayer Position(Vector3 position)
        {
            _cachedTransform.position = position;

            return this;
        }

        public AudioPlayer LocalPosition(Vector3 localPosition)
        {
            _cachedTransform.localPosition = localPosition;

            return this;
        }

        public AudioPlayer Pause()
        {
            _audioSource.Pause();
            IsPaused = true;

            return this;
        }

        public AudioPlayer Resume()
        {
            _audioSource.UnPause();
            IsPaused = false;

            return this;
        }

        public void Dispose()
        {
            for (int i = 0; i < _currentEntry.Clips.Length; i++)
            {
                if (_currentEntry.Clips[i].Asset != null)
                {
                    _currentEntry.Clips[i].ReleaseAsset();
                }
            }

            _cachedTransform.position = Vector3.zero;
            _cachedTransform.parent = null;
            IsPaused = false;

            onKill?.Invoke();
        }

        internal void Setup()
        {
            _audioSource = GetComponent<AudioSource>();
            _cachedTransform = transform;
            
        }

        internal void Setup(IFilterProperty[] filters)
        {
            Setup();
            _filters = new Dictionary<Type, Component>();
            for (int i = 0; i < filters.Length; i++)
            {
                Component filterComponent = gameObject.AddComponent(filters[i].FilterType);
                filters[i].SetupFilter(ref filterComponent);
                _filters.Add(filters[i].FilterType, filterComponent);
            }
        }

        internal async UniTask Play(AudioEntry entry, AudioPlayerProperties playerProperties, AudioMixerGroup mixerGroup)
        {
            _currentEntry = entry;

            _audioSource.outputAudioMixerGroup = mixerGroup;
            SetAudioProperties(playerProperties);
            SetFilterProperties(entry.Filters);

            switch (entry.Mode)
            {
                case AudioEntry.PlayMode.Random:
                {
                    AssetReference assetReference = entry.Clips[UnityEngine.Random.Range(0, entry.Clips.Length)];
                    _audioSource.clip = LoadClip(assetReference);
                    _audioSource.Play();
                    await UniTask.WaitWhile(() => _audioSource.isPlaying, cancellationToken: this.GetCancellationTokenOnDestroy());
                    assetReference.ReleaseAsset();
                    break;
                }

                case AudioEntry.PlayMode.Sequential:
                {
                    for (int i = 0; i < entry.Clips.Length; i++)
                    {
                        AssetReference assetReference = entry.Clips[i];
                        _audioSource.clip = LoadClip(assetReference);
                        _audioSource.Play();
                        await UniTask.WaitWhile(() => _audioSource.isPlaying, cancellationToken: this.GetCancellationTokenOnDestroy());
                        assetReference.ReleaseAsset();
                    }

                    break;
                }

                case AudioEntry.PlayMode.SequentialLoop:
                {
                    int index = 0;

                    do
                    {
                        _audioSource.clip = LoadClip(entry.Clips[index]);
                        _audioSource.Play();
                        await UniTask.WaitWhile(() => _audioSource.isPlaying, cancellationToken: this.GetCancellationTokenOnDestroy());
                        index = (index + 1) % entry.Clips.Length;
                    }
                    while (_audioSource.isPlaying);

                    break;
                }
            }
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

        private void SetFilterProperties(IFilterProperty[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                Component component = _filters[properties[i].FilterType];
                properties[i].SetupFilter(ref component);
            }
        }

        private AudioClip LoadClip(AssetReference assetReference)
        {
            if (assetReference.Asset == null)
            {
                return assetReference.LoadAssetAsync<AudioClip>().WaitForCompletion();
            }

            return assetReference.Asset as AudioClip;
        }
    }
}
