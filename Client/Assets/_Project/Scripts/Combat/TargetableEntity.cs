using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    public sealed class TargetableEntity : MonoBehaviour
    {
        [SerializeField] string _displayName = "Unknown";
        [SerializeField] TargetAffiliation _affiliation = TargetAffiliation.Hostile;
        [SerializeField] float _hullPercent = 100f;

        NetworkObject _networkObject;
        ulong _testNetworkObjectId;

        public string DisplayName => _displayName;
        public TargetAffiliation Affiliation => _affiliation;
        public float HullPercent => _hullPercent;
        public NetworkObject NetworkObject => _networkObject;

        void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
        }

        public void Configure(string displayName, TargetAffiliation affiliation, float hullPercent = 100f)
        {
            _displayName = displayName;
            _affiliation = affiliation;
            _hullPercent = hullPercent;
        }

        public void AssignNetworkObjectIdForTests(ulong networkObjectId)
        {
            _testNetworkObjectId = networkObjectId;
        }

        public ulong GetNetworkObjectId()
        {
            if (_testNetworkObjectId != 0UL)
            {
                return _testNetworkObjectId;
            }

            return _networkObject != null ? _networkObject.NetworkObjectId : 0UL;
        }
    }
}
