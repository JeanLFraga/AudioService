using System;
using System.Collections;
using System.Collections.Generic;
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
        private HashSet<AudioEntry> _entries;

        private PoolSettings _settings;

        private int _poolSize;
        private int _filterPool;
        private GameObject _poolParent;

        private bool ShouldExpand => _settings.ExpandCount > 0;

        public AudioPool(AudioDatabase database, PoolSettings poolSettings)
        {
            _settings = poolSettings;

            _poolParent = new GameObject("AudioPool")
            {
                hideFlags = HideFlags.HideAndDontSave,
            };

            UnityEngine.Object.DontDestroyOnLoad(_poolParent);

            SpawnPlayer(_poolSize);

            IReadOnlyList<AudioEntry> audioEntries = database.AudioEntries;

            for (int i = 0; i < audioEntries.Count; i++)
            {
                if (audioEntries[i].Filters.Length > 0)
                {
                    _filterPlayers.Add(audioEntries[i].ConvertedId, new FilterEntry(audioEntries[i]));
                    SpawnFilteredPlayer(_filterPool, audioEntries[i]);
                }
            }
        }

        private void SpawnPlayer(int spawnCount)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                GameObject gameObject = new GameObject($"AudioPlayer {i}")
                {
                    hideFlags = HideFlags.HideAndDontSave,
                };

                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                gameObject.transform.parent = _poolParent.transform;

                AudioPlayer player = gameObject.AddComponent<AudioPlayer>();

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
                    hideFlags = HideFlags.HideAndDontSave,
                };

                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                gameObject.transform.parent = _poolParent.transform;

                AudioPlayer player = gameObject.AddComponent<AudioPlayer>();
                player.Setup(entry.Filters);
                _filterPlayers[entry.ConvertedId].Players.Enqueue(player);
            }
        }

        public AudioPlayer GetAudioPlayer()
        {
            if (_pool.Count == 0)
            {
                if (!ShouldExpand)
                {
                    throw new InvalidOperationException("There is no more players in the pool and the expand count is zero.");
                }

                int maxUntilLimit = (_pool.Count + _settings.ExpandCount) % _settings.PlayerPoolCount;
                SpawnPlayer(_settings.ExpandCount - maxUntilLimit);
            }

            return _pool.Dequeue();
        }

        public AudioPlayer GetFilterPlayer(EntryId id)
        {
            if (_filterPlayers[id].Players.Count == 0)
            {
                if (!ShouldExpand)
                {
                    throw new InvalidOperationException("There is no more players in the pool and the expand count is zero.");
                }

                int maxUntilLimit = (_filterPlayers[id].Players.Count + _settings.ExpandCount) % _settings.FilterPlayerPoolCount;
                SpawnFilteredPlayer(_settings.ExpandCount - maxUntilLimit, _filterPlayers[id].Entry);
            }

            return _filterPlayers[id].Players.Dequeue();
        }

        public void ReturnFilterToPool(EntryId id, AudioPlayer player)
        {
            //TODO Pool shrinking

            _filterPlayers[id].Players.Enqueue(player);
        }

        public void ReturnToPool(AudioPlayer player)
        {
            //TODO Pool shrinking

            _pool.Enqueue(player);
        }

        public void Dispose()
        {
            foreach (AudioPlayer player in _pool)
            {
                player.Dispose();
                UnityEngine.Object.Destroy(player.gameObject);
            }

            foreach (KeyValuePair<EntryId, FilterEntry> keyValue in _filterPlayers)
            {
                foreach (AudioPlayer player in keyValue.Value.Players)
                {
                    player.Dispose();
                    UnityEngine.Object.Destroy(player.gameObject);
                }
            }
        }
    }
}
