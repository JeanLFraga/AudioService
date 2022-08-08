using Cysharp.Threading.Tasks;
using JeanLF.AudioService.Filters;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    internal class AudioPlayerGroup
    {
        private readonly HashSet<AudioPlayer> _playingAudios = new HashSet<AudioPlayer>();
        private readonly AudioMixerGroup _mixerGroup;
        private readonly AudioPool _pool;
        private readonly string _id;

        internal AudioPlayerGroup(string id, AudioMixerGroup mixerGroup, AudioPool pool)
        {
            _id = id;
            _mixerGroup = mixerGroup;
            _pool = pool;
        }

        internal AudioPlayer PlayAudio(EntryId audioId, AudioClip clip, AudioPlayerProperties playerProperties, IFilterProperty[] filterProperties)
        {
            AudioPlayer player = _pool.GetAudioPlayer();

            UniTask task = player.Play(audioId, clip, playerProperties, filterProperties);
            _playingAudios.Add(player);
            AwaitFinish(player, task).Forget();

            return player;
        }

        internal void PauseAudio(EntryId audioId)
        {
            foreach (AudioPlayer variable in _playingAudios)
            {
                if (variable.CurrentId == audioId)
                {
                    variable.Stop();
                }
            }
        }

        internal void StopAudio(EntryId audioId)
        {
            foreach (AudioPlayer variable in _playingAudios)
            {
                if (variable.CurrentId == audioId)
                {
                    variable.Stop();
                }
            }
        }

        internal void StopAll()
        {
            foreach (AudioPlayer variable in _playingAudios)
            {
                variable.Stop();
            }
            _playingAudios.Clear();
        }

        internal IReadOnlyList<AudioPlayer> GetPlayingAudio()
        {
            return _playingAudios.ToList();
        }

        private async UniTaskVoid AwaitFinish(AudioPlayer player, UniTask task)
        {
            await task;
            _playingAudios.Remove(player);
        }
    }
}
