using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JeanLF.AudioService
{
    internal class AudioPool
    {
        private readonly Queue<AudioPlayer> _pool = new Queue<AudioPlayer>();

        public AudioPool(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject gameObject = new GameObject($"AudioPlayer {i}")
                {
                    hideFlags = HideFlags.HideAndDontSave,
                };

                AudioPlayer player = gameObject.AddComponent<AudioPlayer>();

                _pool.Enqueue(player);
            }
        }

        public AudioPlayer GetAudioPlayer()
        {
            if (_pool.Count == 0)
            {
                Debug.Log("No player in pool");
                return null;
            }

            return _pool.Dequeue();
        }

        public void ReturnToPool(AudioPlayer player)
        {
            _pool.Enqueue(player);
        }
    }
}
