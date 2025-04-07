using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JeanLF.AudioService
{
    internal class AudioPool : IDisposable
    {
        private class FilterEntry
        {
            public readonly AudioEntry Entry;
            public readonly Queue<AudioPlayer> Players = new Queue<AudioPlayer>();

            public FilterEntry(AudioEntry entry)
            {
                Entry = entry;
            }
        }

        private readonly Queue<AudioPlayer> _pool = new Queue<AudioPlayer>();
        private readonly Dictionary<EntryId, FilterEntry> _filterPlayers = new Dictionary<EntryId, FilterEntry>();
        private readonly GameObject _poolParent;
        private readonly PoolSettings _settings;

        private bool ShouldExpand => _settings.ExpandCount > 0;
        private bool ShouldShrink => _settings.ShrinkCount > 0;

        private bool _isQuitting = false;

        public AudioPool(AudioDatabase database, PoolSettings poolSettings)
        {
            _settings = poolSettings;

            _poolParent = new GameObject("AudioPool")
            {
                hideFlags = HideFlags.HideInHierarchy
            };

            UnityEngine.Object.DontDestroyOnLoad(_poolParent);

            SpawnPlayer(_settings.PlayerPoolCount);

            IReadOnlyList<AudioEntry> audioEntries = database.AudioEntries;

            for (int i = 0; i < audioEntries.Count; i++)
            {
                if (audioEntries[i].Filters.Length > 0)
                {
                    _filterPlayers.Add(audioEntries[i].ConvertedId, new FilterEntry(audioEntries[i]));
                    SpawnFilteredPlayer(_settings.FilterPlayerPoolCount, audioEntries[i]);
                }
            }

            Application.quitting += ApplicationQuitting;
        }

        private void ApplicationQuitting()
        {
            _isQuitting = true;
        }

        public AudioPlayer GetPlayer()
        {
            if (_pool.Count == 0)
            {
                if (!ShouldExpand)
                {
                    throw new InvalidOperationException("There is no more players in the pool and the expand count is zero.");
                }

                int maxStep = Mathf.Min(_pool.Count + _settings.ExpandCount, _settings.PlayerPoolCount);
                SpawnPlayer(maxStep);
            }

            return _pool.Dequeue();
        }

        public AudioPlayer GetFilteredPlayer(EntryId id)
        {
            if (_filterPlayers[id].Players.Count == 0)
            {
                if (!ShouldExpand)
                {
                    throw new InvalidOperationException("There is no more players in the pool and the expand count is zero. Review your pool settings in Project Settings/JeanLF/AudioService");
                }

                int maxStep = Mathf.Min(_filterPlayers[id].Players.Count + _settings.ExpandCount, _settings.FilterPlayerPoolCount);
                SpawnFilteredPlayer(maxStep, _filterPlayers[id].Entry);
            }

            return _filterPlayers[id].Players.Dequeue();
        }

        public void ReleasePlayer(AudioPlayer player)
        {
            if (_isQuitting)
            {
                return;
            }
            
            player.transform.parent = _poolParent.transform;
            _pool.Enqueue(player);

            if (_pool.Count >= _settings.PlayerPoolCount + _settings.ShrinkCount && ShouldShrink)
            {
                for (int i = 0; i < _settings.ShrinkCount; i++)
                {
                    AudioPlayer instance = _pool.Dequeue();
                    instance.Dispose();
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        UnityEngine.Object.DestroyImmediate(instance.gameObject);
                    }
#endif
                    UnityEngine.Object.Destroy(instance.gameObject);
                }
            }
        }

        public void ReleaseFilteredPlayer(EntryId id, AudioPlayer player)
        {
            if (_isQuitting)
            {
                return;
            }

            player.transform.parent = _poolParent.transform;
            _filterPlayers[id].Players.Enqueue(player);

            if (_filterPlayers[id].Players.Count >= _settings.FilterPlayerPoolCount + _settings.ShrinkCount && ShouldShrink)
            {
                for (int i = 0; i < _settings.ShrinkCount; i++)
                {
                    AudioPlayer instance = _filterPlayers[id].Players.Dequeue();
                    instance.Dispose();
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        UnityEngine.Object.DestroyImmediate(instance.gameObject);
                    }
#endif
                    UnityEngine.Object.Destroy(instance.gameObject);
                }
            }
        }

        public void Dispose()
        {
            foreach (AudioPlayer player in _pool)
            {
                player.Dispose();
                player.OnDestroyed = null;

                UnityEngine.Object.Destroy(player.gameObject);
            }

            foreach (KeyValuePair<EntryId, FilterEntry> keyValue in _filterPlayers)
            {
                foreach (AudioPlayer player in keyValue.Value.Players)
                {
                    player.Dispose();
                    player.OnDestroyed = null;

                    UnityEngine.Object.Destroy(player.gameObject);
                }
            }
        }

        public void HandleFilteredPlayerDestroyed(AudioEntry entry)
        {
            if (_isQuitting)
            {
                return;
            }
            SpawnFilteredPlayer(1, entry);
        }

        public void HandlePlayerDestroyed()
        {
            if (_isQuitting)
            {
                return;
            }
            SpawnPlayer(1);
        }

        internal int GetPlayerInstanceCount()
        {
            return _pool.Count;
        }

        internal int GetFilteredPlayerInstanceCount(EntryId entryId)
        {
            return _filterPlayers[entryId].Players.Count;
        }

        private void SpawnPlayer(int spawnCount)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                GameObject gameObject = new GameObject($"AudioPlayer {i}")
                {
                    hideFlags = HideFlags.HideInHierarchy
                };

                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                gameObject.transform.parent = _poolParent.transform;

                AudioPlayer player = gameObject.AddComponent<AudioPlayer>();

                player.OnDestroyed += HandlePlayerDestroyed;
                player.Setup();
                _pool.Enqueue(player);
            }
        }

        private void SpawnFilteredPlayer(int spawnCount, AudioEntry entry)
        {
            for (int j = 0; j < spawnCount; j++)
            {
                GameObject gameObject = new GameObject($"FilterAudioPlayer_{entry.Id} {j}")
                {
                    hideFlags = HideFlags.HideInHierarchy
                };

                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                gameObject.transform.parent = _poolParent.transform;

                AudioPlayer player = gameObject.AddComponent<AudioPlayer>();
                player.OnDestroyed = () => HandleFilteredPlayerDestroyed(entry);
                player.Setup(entry.Filters);
                _filterPlayers[entry.ConvertedId].Players.Enqueue(player);
            }
        }
    }
}