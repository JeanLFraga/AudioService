using System;
using System.Runtime.CompilerServices;

namespace JeanLF.AudioService
{
    public static class AsyncExtensions
    {
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

        public bool IsCompleted => !_player.IsActive;

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
            _player.onKill = continuation;
        }
    }
}
