using UnityEngine;

namespace IronExiles.Combat
{
    /// <summary>
    /// Applies <see cref="ShipMovementModel"/> to a ship transform each frame.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ShipMovementController : MonoBehaviour
    {
        [SerializeField] ShipStatsDefinition _statsDefinition;
        [SerializeField] Vector3 _sectorBoundsExtent = new Vector3(5000f, 5000f, 5000f);

        readonly ShipMovementModel _model = new ShipMovementModel();
        ShipInputController _input;
        bool _networkDriverActive;

        public ShipMovementModel Model => _model;
        public Vector3 Velocity => _model.Velocity;

        public void SetNetworkDriverActive(bool active) => _networkDriverActive = active;

        public void SimulateInput(ShipMovementInput input, float deltaTime)
        {
            _model.SetMovementInput(input.LocalThrust, input.LocalRotation);
            _model.Simulate(deltaTime);
        }

        void Awake()
        {
            _input = GetComponent<ShipInputController>();
            ApplyStats();
            _model.SetSectorBoundsExtent(_sectorBoundsExtent);
            _model.Reset(transform.position, transform.rotation);
        }

        void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void Update()
        {
            if (_networkDriverActive)
            {
                return;
            }

            if (_input != null)
            {
                _input.ReadInto(_model);
            }

            _model.Simulate(Time.deltaTime);
            transform.SetPositionAndRotation(_model.Position, _model.Rotation);
        }

        public void ApplyStats()
        {
            var stats = _statsDefinition != null
                ? _statsDefinition.ToSnapshot()
                : ShipStatsDefinition.HumanStarterFighterDefaults();
            _model.SetStats(stats);
        }

        public void SetSectorBoundsExtent(Vector3 extent) => _model.SetSectorBoundsExtent(extent);
    }
}
