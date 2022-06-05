using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JeanLF.AudioService
{
    public interface IAudioService
    {
        UniTask Play(AudioReference audio, AudioPlayerProperties? overrideProperties = null);
        void Pause();
        void PauseGroup();
        void Stop();
        void StopGroup();
        void StopAll();
    }
}
