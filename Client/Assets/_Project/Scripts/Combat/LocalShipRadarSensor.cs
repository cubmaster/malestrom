using System.Collections.Generic;
using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    public sealed class LocalShipRadarSensor : MonoBehaviour
    {
        [SerializeField] float _lockRangeMeters = TargetingSensorSettings.DefaultLockRangeMeters;
        [SerializeField] int _maxRadarContacts = TargetingSensorSettings.DefaultMaxRadarContacts;

        TargetableEntity _selfTargetable;

        void Awake()
        {
            _selfTargetable = GetComponent<TargetableEntity>();
        }

        public IReadOnlyList<RadarContact> GetRadarContacts()
        {
            var selfId = _selfTargetable != null ? _selfTargetable.GetNetworkObjectId() : 0UL;
            return TargetSelectionMath.CollectRadarContacts(
                transform.position,
                transform.forward,
                selfId,
                Object.FindObjectsByType<TargetableEntity>(FindObjectsSortMode.None),
                _lockRangeMeters,
                _maxRadarContacts);
        }
    }
}
