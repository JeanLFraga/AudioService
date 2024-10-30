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

        internal AudioPlayer PlayAudio(AudioEntry entry, AudioPlayerProperties playerProperties)
        {
            AudioPlayer player;
            if (entry.HasFilters)
            {
                player = _pool.GetFilteredPlayer(entry.ConvertedId);
            }
            else
            {
                player = _pool.GetPlayer();
            }

            player.Play(entry, playerProperties, _mixerGroup).Forget();
            _playingAudios.Add(player);
            AwaitFinish(entry, player).Forget();

            return player;
        }

        internal void PauseAudio(EntryId audioId)
        {
            foreach (AudioPlayer player in _playingAudios)
            {
                if (player.CurrentId == audioId)
                {
                    player.Pause();
                }
            }
        }

        internal void ResumeAudio(EntryId audioId)
        {
            foreach (AudioPlayer player in _playingAudios)
            {
                if (player.CurrentId == audioId)
                {
                    player.Resume();
                }
            }
        }

        internal void PauseAll()
        {
            foreach (AudioPlayer player in _playingAudios)
            {
                player.Pause();
            }
        }

        internal void Resume()
        {
            foreach (AudioPlayer player in _playingAudios)
            {
                player.Resume();
            }
        }

        internal void StopAudio(EntryId audioId)
        {
            foreach (AudioPlayer player in _playingAudios)
            {
                if (player.CurrentId == audioId)
                {
                    player.Stop();
                }
            }
        }

        internal void StopAll()
        {
            foreach (AudioPlayer player in _playingAudios)
            {
                player.Stop();
            }
            _playingAudios.Clear();
        }

        internal void FadeGroup(float to, float duration)
        {
            
        }

        internal void FadeGroup(float from, float to, float duration)
        {
            
        }

        [Pure]
        internal IReadOnlyList<AudioPlayer> GetPlayingAudios()
        {
            return _playingAudios.ToList();
        }

        private async UniTaskVoid AwaitFinish(AudioEntry entry, AudioPlayer player)
        {
            await player;
            player.Dispose();
            _playingAudios.Remove(player);
            
            if (entry.HasFilters)
            {
                _pool.ReleaseFilteredPlayer(entry.ConvertedId, player);
            }
            else
            {
                _pool.ReleasePlayer(player);
            }
        }
    }
}
