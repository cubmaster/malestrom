using UnityEngine;

namespace IronExiles.Combat
{
    [CreateAssetMenu(fileName = "BeamWeaponDefinition", menuName = "Iron Exiles/Combat/Beam Weapon Definition")]
    public sealed class BeamWeaponDefinition : ScriptableObject
    {
        [SerializeField] float _baseDps = BeamWeaponSettings.DefaultTier1BaseDps;
        [SerializeField] float _rangeMeters = TargetingSensorSettings.DefaultLockRangeMeters;
        [SerializeField] float _energyDrawPerSecond = 10f;

        public float BaseDps => _baseDps;
        public float RangeMeters => _rangeMeters;
        public float EnergyDrawPerSecond => _energyDrawPerSecond;
    }
}
