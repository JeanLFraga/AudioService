using UnityEngine;

namespace JeanLF.AudioService
{
    public interface IAudioService
    {
        void Play();
        void Pause();
        void PauseGroup();
        void Stop();
        void StopGroup();
        void StopAll();
    }
}
