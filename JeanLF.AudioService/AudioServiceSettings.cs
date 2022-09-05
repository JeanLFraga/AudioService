using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JeanLF.AudioService
{
    public class AudioServiceSettings : ScriptableObject
    {
        internal const string FileName = "JeanLF_AS_Settings";

#if UNITY_EDITOR
        internal const string DatabaseName = nameof(_database);
        internal const string PoolSettingsName = nameof(_poolSettings);
        internal const string PoolSizeName = nameof(JeanLF.AudioService.PoolSettings.PlayerPoolCount);
        internal const string FilterCountName = nameof(JeanLF.AudioService.PoolSettings.FilterPlayerPoolCount);
        internal const string ExpandCountName = nameof(JeanLF.AudioService.PoolSettings.ExpandCount);
        internal const string ShrinkCountName = nameof(JeanLF.AudioService.PoolSettings.ShrinkCount);
#endif
        [SerializeField]
        private AudioDatabase _database;
        [SerializeField]
        private PoolSettings _poolSettings;

        internal bool HasConfiguration => _database != null;

        public AudioServiceSettings()
        {
            _poolSettings = new PoolSettings(15, 3, 2, 2);
        }

        internal PoolSettings PoolSettings => _poolSettings;
        public AudioDatabase Configuration => _database;


        internal void OverrideConfiguration(AudioDatabase database)
        {
            _database = database;
        }
    }
}
