using UnityEngine;

namespace IronExiles.Networking
{
    [DisallowMultipleComponent]
    public sealed class SpawnPointManager : MonoBehaviour
    {
        [SerializeField] Transform[] _spawnPoints;

        int _nextIndex;

        public int SpawnPointCount => _spawnPoints != null ? _spawnPoints.Length : 0;

        public Vector3 GetNextSpawnPosition()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                return Vector3.zero;
            }

            var point = _spawnPoints[_nextIndex];
            _nextIndex = (_nextIndex + 1) % _spawnPoints.Length;
            return point != null ? point.position : Vector3.zero;
        }

        public Quaternion GetNextSpawnRotation()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                return Quaternion.identity;
            }

            var idx = (_nextIndex == 0 ? _spawnPoints.Length : _nextIndex) - 1;
            var point = _spawnPoints[idx];
            return point != null ? point.rotation : Quaternion.identity;
        }

        public void ResetIndex() => _nextIndex = 0;

        public void ConfigureSpawnPoints(Transform[] spawnPoints)
        {
            _spawnPoints = spawnPoints;
            _nextIndex = 0;
        }
    }
}
