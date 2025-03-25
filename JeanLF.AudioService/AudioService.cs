using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    public sealed partial class AudioService : IAudioService
    {
        public const string RuntimeAssetsPath = "Assets/Plugins/JeanLF.AudioService";
        private const string SettingsAssetName = "JeanLF_AS_Settings";
        public const string SettingsAssetPath = RuntimeAssetsPath + "/Resources/" + SettingsAssetName + ".asset";
        
        private readonly AudioDatabase _database;
        private readonly AudioPool _pool;
        private readonly Dictionary<GroupId, AudioPlayerGroup> _audioGroups = new();
        private readonly Dictionary<EntryId, AudioEntry> _audioEntries = new();
        private readonly Stack<AudioMixerSnapshot> _snapshotsStack = new();

        public AudioService()
        {
            AudioServiceSettings settings = Resources.Load<AudioServiceSettings>(SettingsAssetName);
            _database = settings.Database;

            _pool = new AudioPool(_database, settings.PoolSettings);
            CreateIdCache(settings);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        internal AudioService(AudioServiceSettings settings)
        {
            _database = settings.Database;

            _pool = new AudioPool(_database, settings.PoolSettings);
            CreateIdCache(settings);
        }

        private void CreateIdCache(AudioServiceSettings settings)
        {
            if (_database == null)
            {
                throw new NullReferenceException(@"Audio Service configuration can't be null.\n
                You can set the configuration on the service settings in <b>Project Settings/JeanLF/Audio Service</b>");
            }

            IReadOnlyList<AudioGroup> groups = _database.AudioGroups;

            for (int i = 0; i < groups.Count; i++)
            {
                _audioGroups.Add(groups[i].ConvertedId, new AudioPlayerGroup(groups[i].Id, groups[i].MixerGroup, _pool));
            }

            IReadOnlyList<AudioEntry> entries = _database.AudioEntries;
            for (int i = 0; i < entries.Count; i++)
            {
                _audioEntries.Add(entries[i].ConvertedId, entries[i]);
            }
        }

        public AudioPlayer Play(AudioReference audio, AudioPlayerProperties? overrideProperties = null)
        {
            return Play(audio.EntryId, audio.GroupId, overrideProperties);
        }

        public AudioPlayer Play(EntryId entryId, GroupId groupId, AudioPlayerProperties? overrideProperties = null)
        {
            if (entryId == EntryId.Invalid || groupId == GroupId.Invalid)
            {
                throw new InvalidEnumArgumentException();
            }

            AudioEntry entry = _audioEntries[entryId];
            AudioPlayer player = _audioGroups[groupId].PlayAudio(entry, overrideProperties == null ? entry.AudioProperties : overrideProperties.Value);
            return player;
        }

        public void Resume(AudioReference audio)
        {
            Resume(audio.EntryId, audio.GroupId);
        }

        public void Resume(EntryId entryId, GroupId groupId)
        {
            if (entryId == EntryId.Invalid || groupId == GroupId.Invalid)
            {
                throw new InvalidEnumArgumentException();
            }

            _audioGroups[groupId].ResumeAudio(entryId);
        }
        
        public void ResumeGroup(GroupId groupId)
        {
            if (groupId == GroupId.Invalid)
            {
                throw new InvalidEnumArgumentException();
            }

            _audioGroups[groupId].Resume();
        }

        public void ResumeAll()
        {
            foreach (KeyValuePair<GroupId, AudioPlayerGroup> keyPair in _audioGroups)
            {
                keyPair.Value.Resume();
            }
        }
        
        public void Pause(AudioReference audio)
        {
            Pause(audio.EntryId, audio.GroupId);
        }

        public void Pause(EntryId entryId, GroupId groupId)
        {
            if (entryId == EntryId.Invalid || groupId == GroupId.Invalid)
            {
                throw new InvalidEnumArgumentException();
            }

            _audioGroups[groupId].PauseAudio(entryId);
        }

        public void PauseGroup(GroupId groupId)
        {
            if (groupId == GroupId.Invalid)
            {
                throw new InvalidEnumArgumentException();
            }

            _audioGroups[groupId].Pause();
        }
        
        public void PauseAll()
        {
            foreach (KeyValuePair<GroupId, AudioPlayerGroup> keyPair in _audioGroups)
            {
                keyPair.Value.Pause();
            }
        }

        public void Stop(AudioReference audio)
        {
            Stop(audio.EntryId, audio.GroupId);
        }

        public void Stop(EntryId entryId, GroupId groupId)
        {
            if (entryId == EntryId.Invalid || groupId == GroupId.Invalid)
            {
                throw new InvalidEnumArgumentException();
            }

            _audioGroups[groupId].StopAudio(entryId);
        }

        public void StopGroup(GroupId groupId)
        {
            _audioGroups[groupId].StopAll();
        }

        public void StopAll()
        {
            foreach (KeyValuePair<GroupId, AudioPlayerGroup> keyPair in _audioGroups)
            {
                keyPair.Value.StopAll();
            }
        }

        public void FadeGroup(GroupId groupId, float to, float duration)
        {
            throw new NotImplementedException();
            _audioGroups[groupId].FadeGroup(to, duration);
        }

        public void FadeGroup(GroupId groupId, float from, float to, float duration)
        {
            throw new NotImplementedException();
            _audioGroups[groupId].FadeGroup(from, to, duration);
        }

        public void Dispose()
        {
            _pool?.Dispose();
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange newState)
        {
            switch (newState)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    ResumeAll();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    Dispose();
                    break;
            }
            
            if (EditorApplication.isPaused)
            {
                PauseAll();
            } 
        }
#endif
        
    }
}
