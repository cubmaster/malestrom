using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipMovementController))]
    [RequireComponent(typeof(ShipInputController))]
    public sealed class NetworkShipMovementController : NetworkBehaviour
    {
        [SerializeField] float _positionReconcileThresholdMeters = 2f;
        [SerializeField] float _rotationReconcileThresholdDegrees = 15f;
        [SerializeField] float _reconcileBlendSpeed = 8f;
        [SerializeField] float _serverTickRateHz = 30f;

        ShipMovementController _movement;
        ShipInputController _input;
        ShipMovementModel _ownerPredictedModel;
        ShipMovementInput _serverInput;
        bool _hasServerInput;

        void Awake()
        {
            _movement = GetComponent<ShipMovementController>();
            _input = GetComponent<ShipInputController>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _movement.SetNetworkDriverActive(true);

            if (IsOwner && IsClient)
            {
                _ownerPredictedModel = new ShipMovementModel();
                _ownerPredictedModel.SetSectorBoundsExtent(_movement.Model.SectorBoundsExtent);
                _ownerPredictedModel.SetStats(ShipStatsDefinition.HumanStarterFighterDefaults());
                _ownerPredictedModel.Reset(transform.position, transform.rotation);
            }

            if (!IsOwner)
            {
                _movement.enabled = false;
                _input.enabled = false;
            }
        }

        void Update()
        {
            if (!IsOwner || !IsClient || !IsSpawned)
            {
                return;
            }

            var input = _input.CaptureInput();
            SubmitMovementInputServerRpc(input);
            ApplyInputToModel(_ownerPredictedModel, input, Time.deltaTime);
            transform.SetPositionAndRotation(_ownerPredictedModel.Position, _ownerPredictedModel.Rotation);
        }

        void LateUpdate()
        {
            if (!IsOwner || !IsClient || !IsSpawned)
            {
                return;
            }

            ReconcileOwnerPrediction();
        }

        void FixedUpdate()
        {
            if (!IsServer || !IsSpawned || !_hasServerInput)
            {
                return;
            }

            _movement.SimulateInput(_serverInput, Time.fixedDeltaTime);
            transform.SetPositionAndRotation(_movement.Model.Position, _movement.Model.Rotation);
        }

        [ServerRpc]
        void SubmitMovementInputServerRpc(ShipMovementInput input)
        {
            _serverInput = input;
            _hasServerInput = true;
        }

        static void ApplyInputToModel(ShipMovementModel model, ShipMovementInput input, float deltaTime)
        {
            model.SetMovementInput(input.LocalThrust, input.LocalRotation, input.Brake);
            model.Simulate(deltaTime);
        }

        void ReconcileOwnerPrediction()
        {
            var mode = ShipMovementReplicationMath.EvaluateReconcileMode(
                transform.position,
                transform.rotation,
                _ownerPredictedModel.Position,
                _ownerPredictedModel.Rotation,
                _positionReconcileThresholdMeters,
                _rotationReconcileThresholdDegrees);

            switch (mode)
            {
                case MovementReconcileMode.Snap:
                    _ownerPredictedModel.Reset(transform.position, transform.rotation);
                    break;
                case MovementReconcileMode.Blend:
                    var blendedPosition = ShipMovementReplicationMath.BlendPosition(
                        _ownerPredictedModel.Position,
                        transform.position,
                        Time.deltaTime,
                        _reconcileBlendSpeed);
                    var blendedRotation = ShipMovementReplicationMath.BlendRotation(
                        _ownerPredictedModel.Rotation,
                        transform.rotation,
                        Time.deltaTime,
                        _reconcileBlendSpeed);
                    _ownerPredictedModel.Reset(blendedPosition, blendedRotation);
                    transform.SetPositionAndRotation(blendedPosition, blendedRotation);
                    break;
            }
        }
    }
}
