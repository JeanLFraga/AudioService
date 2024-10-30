using Cysharp.Threading.Tasks;
using JeanLF.AudioService.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioPlayer : MonoBehaviour, IDisposable
    {
        public delegate void TempoEvent(int current);
        public event Action<AudioClip> OnTrackChanged;
        public event Action OnEnd;
        public event TempoEvent OnBar;
        public event TempoEvent OnBeat;

        private AudioSource _audioSource;
        private Dictionary<Type, Component> _filters;
        private AudioEntry? _currentEntry;
        private Transform _cachedTransform;

        internal Action OnFinished;
        internal Action OnDestroyed;

        public EntryId CurrentId => _currentEntry?.ConvertedId ?? EntryId.Invalid;

        public bool IsPaused { get; private set; }
        public bool IsPlaying => _audioSource.isPlaying && !IsPaused;
        internal bool IsActive => _currentEntry.HasValue && _audioSource != null;
        internal AudioEntry? CurrentEntry => _currentEntry;

        private void OnDestroy()
        {
            ReleaseEntryClips();

            if (_audioSource != null)
            {
                _audioSource.Stop();
            }
            
            OnDestroyed?.Invoke();
        }

        public AudioPlayer Attach(Transform parent, Vector3 localPosition = default)
        {
            _cachedTransform.parent = parent;
            _cachedTransform.localPosition = localPosition;

            return this;
        }

        public AudioPlayer Detach()
        {
            _cachedTransform.parent = null;

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

        public AudioPlayer Fade(float from, float to, float duration)
        {
            FadeAsync(from, to, duration).Forget();

            return this;
        }

        public AudioPlayer Fade(float to, float duration)
        {
            FadeAsync(_audioSource.volume, to, duration).Forget();

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

        public AudioPlayer SetOnBeat(TempoEvent action)
        {
            OnBeat = action;

            return this;
        }

        public AudioPlayer SetOnBar(TempoEvent action)
        {
            OnBar = action;

            return this;
        }

        public void Dispose()
        {
            ReleaseEntryClips();

            _cachedTransform.position = Vector3.zero;
            _cachedTransform.parent = null;
            
            IsPaused = false;
            _audioSource.Stop();
            
            OnBar = null;
            OnBeat = null;
            OnEnd = null;
            OnTrackChanged = null;
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
            bool isPlayingInternal()
            {
                return _audioSource.isPlaying || IsPaused;
            }

            switch (entry.Mode)
            {
                case AudioEntry.PlayMode.Random:
                {
                    AssetReference assetReference = entry.Clips[UnityEngine.Random.Range(0, entry.Clips.Length)];
                    _audioSource.clip = LoadClip(assetReference);
                    _audioSource.Play();
                    InvokeTempoEvents().Forget();
                    await UniTask.WaitWhile(isPlayingInternal, cancellationToken: this.GetCancellationTokenOnDestroy());
                    
                    OnTrackChanged?.Invoke(_audioSource.clip);
                    if (assetReference.Asset != null)
                    {
                        assetReference.ReleaseAsset();
                    }
                    OnEnd?.Invoke();

                    break;
                }

                case AudioEntry.PlayMode.Sequential:
                {
                    for (int i = 0; i < entry.Clips.Length; i++)
                    {
                        AssetReference assetReference = entry.Clips[i];
                        _audioSource.clip = LoadClip(assetReference);
                        _audioSource.Play();
                        InvokeTempoEvents().Forget();
                        await UniTask.WaitWhile(isPlayingInternal, cancellationToken: this.GetCancellationTokenOnDestroy());
                        
                        OnTrackChanged?.Invoke(_audioSource.clip);
                        if (assetReference.Asset)
                        {
                            assetReference.ReleaseAsset();
                        }
                        
                    }
                    OnEnd?.Invoke();

                    break;
                }

                case AudioEntry.PlayMode.SequentialLoop:
                {
                    int index = 0;

                    do
                    {
                        _audioSource.clip = LoadClip(entry.Clips[index]);
                        _audioSource.Play();
                        InvokeTempoEvents().Forget();
                        
                        await UniTask.WaitWhile(isPlayingInternal, cancellationToken: this.GetCancellationTokenOnDestroy());
                        
                        OnTrackChanged?.Invoke(_audioSource.clip);
                        index = (index + 1) % entry.Clips.Length;
                    }
                    while (!this.GetCancellationTokenOnDestroy().IsCancellationRequested);
                    OnEnd?.Invoke();
                    break;
                }
            }

            _currentEntry = null;
            OnFinished?.Invoke();
        }

        internal void Stop()
        {
            _audioSource.Stop();
            _currentEntry = null;
        }

        private async UniTaskVoid InvokeTempoEvents()
        {
            if (_currentEntry == null)
            {
                return;
            }

            AudioDescription description = _currentEntry.Value.AudioDescription;

            if (!description.IsValid)
            {
                return;
            }

            double barPerSec = description.BarPerSecond;
            double beatPerSec = description.BeatPerSecond;

            double findNextEvent(double evtTempo)
            {
                double remainder = ((double)_audioSource.timeSamples / _audioSource.clip.frequency) % evtTempo + double.Epsilon;
                return AudioSettings.dspTime + (evtTempo - remainder);
            }

            double nextBeat = AudioSettings.dspTime;
            double nextBar = AudioSettings.dspTime;

            CancellationToken cancelToken = this.GetCancellationTokenOnDestroy();

            while (IsActive && !cancelToken.IsCancellationRequested)
            {
                if (!IsPlaying)
                {
                    await UniTask.Yield(PlayerLoopTiming.TimeUpdate, cancelToken);
                    continue;
                }

                await UniTask.Yield(PlayerLoopTiming.TimeUpdate, cancelToken);

                if (IsActive || cancelToken.IsCancellationRequested)
                {
                    break;
                }

                double currentTimeNorm = ((double)_audioSource.timeSamples / _audioSource.clip.samples);

                if (AudioSettings.dspTime >= nextBeat && currentTimeNorm < 1)
                {
                    double currentTimeSec = (double)_audioSource.timeSamples / _audioSource.clip.frequency;
                    nextBeat = findNextEvent(beatPerSec);
                    OnBeat?.Invoke( (int)(currentTimeSec / beatPerSec));
                }

                if (AudioSettings.dspTime >= nextBar && currentTimeNorm < 1)
                {
                    double currentTimeSec = (double)_audioSource.timeSamples / _audioSource.clip.frequency;
                    nextBar = findNextEvent(barPerSec);
                    OnBar?.Invoke( (int)(currentTimeSec / barPerSec));
                }
            }
            OnEnd?.Invoke();
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

        private async UniTaskVoid FadeAsync(float from, float to, float duration)
        {
            float time = 0f;

            while (time <= duration && !this.GetCancellationTokenOnDestroy().IsCancellationRequested)
            {
                time += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(from, to, time);
                await UniTask.NextFrame();
            }
        }
        
        private void ReleaseEntryClips()
        {
            if (_currentEntry.HasValue)
            {
                foreach (var clip in _currentEntry.Value.Clips)
                {
                    if (clip.Asset != null)
                    {
                        clip.ReleaseAsset();
                    }
                }
            }
        }
    }
}
