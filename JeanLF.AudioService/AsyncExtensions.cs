using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace JeanLF.AudioService
{
    public class PooledWaitForPlayer : IEnumerator
    {
        private static readonly ConcurrentQueue<PooledWaitForPlayer> Pool = new ConcurrentQueue<PooledWaitForPlayer>();

        private AudioPlayer _player;

        public static PooledWaitForPlayer Create(AudioPlayer player)
        {
            if (!Pool.TryDequeue(out PooledWaitForPlayer wait))
            {
                wait = new PooledWaitForPlayer();
            }

            wait._player = player;

            return wait;
        }

        public bool MoveNext()
        {
            if (!_player.IsActive)
            {
                Pool.Enqueue(this);
            }

            return _player.IsActive;
        }

        public void Reset()
        {
            Pool.Enqueue(this);
        }

        public object Current => null;
    }

    public static class AsyncExtensions
    {
        public static PooledWaitForPlayer Yield(this AudioPlayer player)
        {
            return PooledWaitForPlayer.Create(player);
        }

        public static AudioPlayerAwaiter GetAwaiter(this AudioPlayer player)
        {
            return new AudioPlayerAwaiter(player);
        }
    }

    public struct AudioPlayerAwaiter : ICriticalNotifyCompletion
    {
        private readonly AudioPlayer _player;

        public AudioPlayerAwaiter(AudioPlayer player)
        {
            _player = player;
        }

        public bool IsCompleted => _player == null || !_player.IsActive;

        public AudioPlayerAwaiter GetAwaiter()
        {
            return this;
        }

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            _player.OnFinished = continuation;
        }
    }
}
