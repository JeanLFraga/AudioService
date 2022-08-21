using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JeanLF.AudioService
{
    internal class AudioPool
    {
        private readonly Queue<AudioPlayer> _pool = new Queue<AudioPlayer>();
        private readonly Dictionary<string, Queue<AudioPlayer>> _filterPlayers = new Dictionary<string, Queue<AudioPlayer>>();

        public AudioPool(AudioConfig config, int poolSize, int filterPool)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject gameObject = new GameObject($"AudioPlayer {i}")
                {
                    hideFlags = HideFlags.HideAndDontSave,
                };

                AudioPlayer player = gameObject.AddComponent<AudioPlayer>();
                player.Setup();

                _pool.Enqueue(player);
            }

            IReadOnlyList<AudioEntry> audioEntries = config.AudioEntries;
            for (int i = 0; i < audioEntries.Count; i++)
            {
                if (audioEntries[i].Filters.Length > 0)
                {
                    for (int j = 0; j < filterPool; j++)
                    {
                        GameObject gameObject = new GameObject($"FilterAudioPlayer {i}")
                        {
                            hideFlags = HideFlags.HideAndDontSave,
                        };

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
    }
}
