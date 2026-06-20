using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    public sealed class EmptySectorFlightSetup : MonoBehaviour
    {
        [SerializeField] Vector3 _spawnPosition = Vector3.zero;
        [SerializeField] Vector3 _sectorBoundsExtent = new Vector3(5000f, 5000f, 5000f);
        [SerializeField] Vector3 _hullScale = new Vector3(2f, 0.5f, 1f);

        const string StarfieldShaderName = "IronExiles/ProceduralStarfield";

        void Start()
        {
            SetupSpaceEnvironment();
            var ship = CreateShip();
            AttachCockpitCamera(ship.transform);
        }

        void SetupSpaceEnvironment()
        {
            var shader = Shader.Find(StarfieldShaderName);
            if (shader != null)
            {
                var skyboxMat = new Material(shader);
                RenderSettings.skybox = skyboxMat;
            }
            else
            {
                RenderSettings.skybox = null;
                var camera = Camera.main;
                if (camera != null)
                {
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    camera.backgroundColor = new Color(0.005f, 0.005f, 0.015f, 1f);
                }
            }

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.03f, 0.03f, 0.05f, 1f);
        }

        GameObject CreateShip()
        {
            var ship = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ship.name = "PlayerShip";
            ship.tag = "Player";
            ship.transform.SetPositionAndRotation(_spawnPosition, Quaternion.identity);
            ship.transform.localScale = _hullScale;

            var collider = ship.GetComponent<BoxCollider>();
            collider.size = Vector3.one;

            var renderer = ship.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            var movement = ship.AddComponent<ShipMovementController>();
            movement.SetSectorBoundsExtent(_sectorBoundsExtent);
            ship.AddComponent<ShipInputController>();
            ship.AddComponent<ShipFlightTelemetryAdapter>();

            return ship;
        }

        void AttachCockpitCamera(Transform shipTransform)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            var rig = camera.gameObject.GetComponent<CockpitCameraRig>();
            if (rig == null)
            {
                rig = camera.gameObject.AddComponent<CockpitCameraRig>();
            }

            rig.SetTarget(shipTransform);
        }
    }
}