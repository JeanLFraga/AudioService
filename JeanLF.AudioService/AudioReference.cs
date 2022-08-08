using UnityEngine;

namespace JeanLF.AudioService
{
    [System.Serializable]
    public struct AudioReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        internal const string EntryMemberPath = nameof(_entryId);
        internal const string GroupMemberPath = nameof(_groupId);
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
            _entryString = _entryId.ToString();
            if (_entryId == EntryId.Invalid)
            {
                _entryString = null;
            }

            _groupString = _groupId.ToString();
            if (_groupId == GroupId.Invalid)
            {
                _groupString = null;
            }
        }

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrWhiteSpace(_entryString))
            {
                if (!EntryId.TryParse(_entryString, out _entryId))
                {
                    _entryId = EntryId.Invalid;
                }
            }
            else
            {
                _entryId = EntryId.Invalid;
            }

            if (!string.IsNullOrWhiteSpace(_groupString))
            {
                if (!GroupId.TryParse(_groupString, out _entryId))
                {
                    _groupId = GroupId.Invalid;
                }
            }
            else
            {
                _groupId = GroupId.Invalid;
            }
        }
    }
}
