using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JeanLF.AudioService
{
    public partial class AudioServiceSettings : ScriptableObject
    {

#if UNITY_EDITOR
        internal const string ConfigMemberPath = nameof(_config);
        internal const string PoolSizeMemberPath = nameof(_poolSize);
        internal const string FilterCountMemberPath = nameof(_filteredSources);
#endif

        [SerializeField][Tooltip("The configuration used in the service.")]
        private AudioConfig _config;
        [SerializeField][Tooltip("The count of AudioSources for simultaneous uses of non-containing filter AudioEntry")]
        private int _poolSize;
        [SerializeField][Tooltip("The count of AudioSources for a single AudioEntry containing filters.")]
        private int _filteredSources;

        internal bool HasConfiguration => _config != null;

        public AudioServiceSettings()
        {
            _poolSize = 15;
            _filteredSources = 3;
        }

        public int PoolSize => _poolSize;
        public int FilteredSources => _filteredSources;
        public AudioConfig Configuration => _config;

        public void OverrideConfiguration(AudioConfig config)
        {
            _config = config;
        }
    }
}
