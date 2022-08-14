using UnityEngine;

namespace JeanLF.AudioService
{
    [System.Serializable]
    public struct AudioReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        internal const string EntryStringPath = nameof(_entryString);
        internal const string GroupStringPath = nameof(_groupString);
#endif


        [SerializeField] private EntryId _entryId;
        [SerializeField] private string _entryString;

        [SerializeField] private GroupId _groupId;
        [SerializeField] private string _groupString;


        public EntryId EntryId => _entryId;

        public GroupId GroupId => _groupId;

        public AudioReference(EntryId entryId, GroupId groupId)
        {
            _entryId = entryId;
            _groupId = groupId;
            _entryString = null;
            _groupString = null;
        }

        public void OnBeforeSerialize()
        {
            EntryId.TryParse(_entryString, out _entryId);
            GroupId.TryParse(_groupString, out _groupId);
        }

        public void OnAfterDeserialize()
        {
            if (!EntryId.TryParse(_entryString, out _entryId))
            {
                _entryId = EntryId.Invalid;
            }

            if (!GroupId.TryParse(_groupString, out _groupId))
            {
                _groupId = GroupId.Invalid;
            }

        }
    }
}
