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
        public AudioDatabase Database => _database;

#if UNITY_INCLUDE_TESTS
        internal void OverrideDatabase(AudioDatabase database)
        {
            _database = database;
        }

        internal void OverridePoolSettings(PoolSettings settings)
        {
            _poolSettings = settings;
        }
    }
#endif
}
