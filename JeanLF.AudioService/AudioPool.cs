using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JeanLF.AudioService
{
    internal class AudioPool : IDisposable
    {
        private readonly Queue<AudioPlayer> _pool = new Queue<AudioPlayer>();
        private readonly Dictionary<string, Queue<AudioPlayer>> _filterPlayers = new Dictionary<string, Queue<AudioPlayer>>();

        public AudioPool(AudioConfig config, int poolSize, int filterPool)
        {
            GameObject parent = new GameObject("AudioPool")
            {
                hideFlags = HideFlags.HideAndDontSave,
            };
            UnityEngine.Object.DontDestroyOnLoad(parent);

            for (int i = 0; i < poolSize; i++)
            {
                GameObject gameObject = new GameObject($"AudioPlayer {i}")
                {
                    hideFlags = HideFlags.HideAndDontSave,
                };

                gameObject.transform.parent = parent.transform;
                UnityEngine.Object.DontDestroyOnLoad(gameObject);

                AudioPlayer player = gameObject.AddComponent<AudioPlayer>();
                player.Setup();

                _pool.Enqueue(player);
            }

            IReadOnlyList<AudioEntry> audioEntries = config.AudioEntries;
            for (int i = 0; i < audioEntries.Count; i++)
            {
                if (audioEntries[i].Filters.Length > 0)
                {
                    _filterPlayers.Add(audioEntries[i].Id, new Queue<AudioPlayer>());
                    for (int j = 0; j < filterPool; j++)
                    {
                        GameObject gameObject = new GameObject($"FilterAudioPlayer_{audioEntries[i].Id} {j}")
                        {
                            hideFlags = HideFlags.HideAndDontSave,
                        };

                        gameObject.transform.parent = parent.transform;
                        UnityEngine.Object.DontDestroyOnLoad(gameObject);

                        AudioPlayer player = gameObject.AddComponent<AudioPlayer>();
                        player.Setup(audioEntries[i].Filters);
                        _filterPlayers[audioEntries[i].Id].Enqueue(player);
                    }
                }
            }
        }

        public AudioPlayer GetAudioPlayer()
        {
            if (_pool.Count == 0)
            {
                throw new InvalidOperationException("There is no more players in the pool.");
            }

            return _pool.Dequeue();
        }

        public AudioPlayer GetFilterPlayer(string id)
        {
            if (_filterPlayers[id].Count == 0)
            {
                throw new InvalidOperationException("There is no more players in the Filters pool.");
            }

            return _filterPlayers[id].Dequeue();
        }

        public void ReturnFilterToPool(string id, AudioPlayer player)
        {
            _filterPlayers[id].Enqueue(player);
        }

        public void ReturnToPool(AudioPlayer player)
        {
            _pool.Enqueue(player);
        }

        public void Dispose()
        {
            foreach (AudioPlayer player in _pool)
            {
                player.Dispose();
                UnityEngine.Object.Destroy(player.gameObject);
            }

            foreach (KeyValuePair<string, Queue<AudioPlayer>> keyValue in _filterPlayers)
            {
                foreach (AudioPlayer player in keyValue.Value)
                {
                    player.Dispose();
                    UnityEngine.Object.Destroy(player.gameObject);
                }
            }
        }
    }
}
