using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace JeanLF.AudioService
{
    public interface IAudioService : IDisposable
    {
        AudioPlayer Play(AudioReference audio, AudioPlayerProperties? overrideProperties = null);
        AudioPlayer Play(EntryId entryId, GroupId groupId, AudioPlayerProperties? overrideProperties = null);
        void Pause(AudioReference audio);
        void Pause(EntryId entryId, GroupId groupId);
        void Resume(AudioReference audio);
        void Resume(EntryId entryId, GroupId groupId);
        void PauseGroup(GroupId groupId);
        void ResumeGroup(GroupId groupId);
        void Stop(AudioReference audio);
        void Stop(EntryId entryId, GroupId groupId);
        void StopGroup(GroupId groupId);
        void StopAll();
    }
}
