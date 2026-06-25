using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    public sealed class EmptySectorFlightSetup : MonoBehaviour
    {
        [SerializeField] Vector3 _spawnPosition = Vector3.zero;
        [SerializeField] Vector3 _sectorBoundsExtent = new Vector3(5000f, 5000f, 5000f);
        [SerializeField] Vector3 _hullScale = new Vector3(2f, 0.5f, 1f);
        [SerializeField] Vector3 _trainingDummyPosition = new Vector3(40f, 0f, 40f);

        void Start()
        {
            ProceduralStarfieldEnvironment.Apply();
            SpawnSectorObjects();
            SpawnTrainingDummy();
            var ship = CreateShip();
            AttachCockpitCamera(ship.transform);
        }

        void SpawnSectorObjects()
        {
            var parent = new GameObject("SectorObjects");

            SpawnAsteroidField(parent.transform);
            SpawnSpaceStation(parent.transform);
            SpawnDebrisRing(parent.transform);
            SpawnNavigationBeacons(parent.transform);
            SpawnLargeAsteroids(parent.transform);
            SpawnCargoContainers(parent.transform);
        }

        void SpawnAsteroidField(Transform parent)
        {
            var fieldParent = new GameObject("AsteroidField");
            fieldParent.transform.SetParent(parent);

            var rng = new System.Random(42);
            for (int i = 0; i < 40; i++)
            {
                var asteroid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                asteroid.name = $"Asteroid_{i:D2}";
                asteroid.transform.SetParent(fieldParent.transform);

                float x = (float)(rng.NextDouble() * 600 - 300) + 200f;
                float y = (float)(rng.NextDouble() * 200 - 100);
                float z = (float)(rng.NextDouble() * 600 - 300) + 150f;
                asteroid.transform.position = new Vector3(x, y, z);

                float scale = (float)(rng.NextDouble() * 8 + 2);
                asteroid.transform.localScale = new Vector3(
                    scale * (float)(0.7 + rng.NextDouble() * 0.6),
                    scale * (float)(0.7 + rng.NextDouble() * 0.6),
                    scale * (float)(0.7 + rng.NextDouble() * 0.6));

                asteroid.transform.rotation = Random.rotation;

                var renderer = asteroid.GetComponent<Renderer>();
                float grey = (float)(rng.NextDouble() * 0.3 + 0.2);
                renderer.material.color = new Color(grey, grey * 0.9f, grey * 0.8f);
            }
        }

        void SpawnSpaceStation(Transform parent)
        {
            var station = new GameObject("SpaceStation");
            station.transform.SetParent(parent);
            station.transform.position = new Vector3(-150f, 30f, 250f);

            var hub = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hub.name = "StationHub";
            hub.transform.SetParent(station.transform);
            hub.transform.localPosition = Vector3.zero;
            hub.transform.localScale = new Vector3(20f, 20f, 20f);
            hub.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.7f);

            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "StationRing";
            ring.transform.SetParent(station.transform);
            ring.transform.localPosition = Vector3.zero;
            ring.transform.localScale = new Vector3(40f, 2f, 40f);
            ring.GetComponent<Renderer>().material.color = new Color(0.5f, 0.55f, 0.6f);

            for (int i = 0; i < 4; i++)
            {
                var arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
                arm.name = $"StationArm_{i}";
                arm.transform.SetParent(station.transform);
                float angle = i * 90f * Mathf.Deg2Rad;
                arm.transform.localPosition = new Vector3(Mathf.Cos(angle) * 25f, 0f, Mathf.Sin(angle) * 25f);
                arm.transform.localScale = new Vector3(3f, 3f, 15f);
                arm.transform.LookAt(station.transform.position);
                arm.GetComponent<Renderer>().material.color = new Color(0.4f, 0.45f, 0.5f);
            }

            var tower = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tower.name = "StationTower";
            tower.transform.SetParent(station.transform);
            tower.transform.localPosition = new Vector3(0f, 18f, 0f);
            tower.transform.localScale = new Vector3(5f, 10f, 5f);
            tower.GetComponent<Renderer>().material.color = new Color(0.7f, 0.3f, 0.3f);
        }

        void SpawnDebrisRing(Transform parent)
        {
            var debrisParent = new GameObject("DebrisRing");
            debrisParent.transform.SetParent(parent);

            var rng = new System.Random(77);
            float ringRadius = 120f;
            Vector3 ringCenter = new Vector3(0f, -50f, -200f);

            for (int i = 0; i < 25; i++)
            {
                float angle = (i / 25f) * Mathf.PI * 2f + (float)(rng.NextDouble() * 0.3);
                float r = ringRadius + (float)(rng.NextDouble() * 30 - 15);
                float y = (float)(rng.NextDouble() * 10 - 5);

                var debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
                debris.name = $"Debris_{i:D2}";
                debris.transform.SetParent(debrisParent.transform);
                debris.transform.position = ringCenter + new Vector3(Mathf.Cos(angle) * r, y, Mathf.Sin(angle) * r);

                float scale = (float)(rng.NextDouble() * 3 + 1);
                debris.transform.localScale = new Vector3(
                    scale * (float)(0.5 + rng.NextDouble()),
                    scale * (float)(0.3 + rng.NextDouble() * 0.5),
                    scale * (float)(0.5 + rng.NextDouble()));
                debris.transform.rotation = Random.rotation;

                var renderer = debris.GetComponent<Renderer>();
                renderer.material.color = new Color(0.35f, 0.3f, 0.25f);
            }
        }

        void SpawnNavigationBeacons(Transform parent)
        {
            var beaconParent = new GameObject("NavigationBeacons");
            beaconParent.transform.SetParent(parent);

            Vector3[] beaconPositions =
            {
                new Vector3(100f, 0f, 0f),
                new Vector3(-100f, 0f, 0f),
                new Vector3(0f, 100f, 0f),
                new Vector3(0f, -100f, 0f),
                new Vector3(0f, 0f, 100f),
                new Vector3(0f, 0f, -100f),
            };

            Color[] beaconColors =
            {
                Color.green, Color.red, Color.cyan, Color.magenta, Color.yellow, Color.blue,
            };

            for (int i = 0; i < beaconPositions.Length; i++)
            {
                var beacon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                beacon.name = $"NavBeacon_{i}";
                beacon.transform.SetParent(beaconParent.transform);
                beacon.transform.position = beaconPositions[i];
                beacon.transform.localScale = new Vector3(2f, 2f, 2f);

                var renderer = beacon.GetComponent<Renderer>();
                renderer.material.color = beaconColors[i];
                renderer.material.SetColor("_EmissionColor", beaconColors[i] * 2f);
                renderer.material.EnableKeyword("_EMISSION");
            }
        }

        void SpawnLargeAsteroids(Transform parent)
        {
            var largeParent = new GameObject("LargeAsteroids");
            largeParent.transform.SetParent(parent);

            Vector3[] positions =
            {
                new Vector3(500f, 80f, 400f),
                new Vector3(-400f, -60f, 600f),
                new Vector3(300f, -120f, -500f),
            };

            for (int i = 0; i < positions.Length; i++)
            {
                var asteroid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                asteroid.name = $"LargeAsteroid_{i}";
                asteroid.transform.SetParent(largeParent.transform);
                asteroid.transform.position = positions[i];

                float baseScale = 30f + i * 15f;
                asteroid.transform.localScale = new Vector3(baseScale * 1.2f, baseScale * 0.8f, baseScale);
                asteroid.transform.rotation = Random.rotation;

                var renderer = asteroid.GetComponent<Renderer>();
                renderer.material.color = new Color(0.25f + i * 0.05f, 0.2f + i * 0.03f, 0.15f + i * 0.02f);
            }
        }

        void SpawnCargoContainers(Transform parent)
        {
            var cargoParent = new GameObject("CargoContainers");
            cargoParent.transform.SetParent(parent);

            var rng = new System.Random(99);
            Vector3 clusterCenter = new Vector3(-80f, 20f, 80f);

            for (int i = 0; i < 8; i++)
            {
                var container = GameObject.CreatePrimitive(PrimitiveType.Cube);
                container.name = $"Cargo_{i:D2}";
                container.transform.SetParent(cargoParent.transform);

                float x = (float)(rng.NextDouble() * 40 - 20);
                float y = (float)(rng.NextDouble() * 40 - 20);
                float z = (float)(rng.NextDouble() * 40 - 20);
                container.transform.position = clusterCenter + new Vector3(x, y, z);
                container.transform.localScale = new Vector3(4f, 2f, 2f);
                container.transform.rotation = Random.rotation;

                var renderer = container.GetComponent<Renderer>();
                renderer.material.color = new Color(0.8f, 0.6f, 0.1f);
            }
        }

        void SpawnTrainingDummy()
        {
            var dummy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dummy.name = "TrainingDummy";
            dummy.transform.SetPositionAndRotation(_trainingDummyPosition, Quaternion.identity);
            dummy.transform.localScale = new Vector3(1.5f, 0.5f, 1.5f);

            var renderer = dummy.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.9f, 0.35f, 0.2f, 1f);
            }

            var targetable = dummy.AddComponent<TargetableEntity>();
            targetable.Configure("Training Dummy", TargetAffiliation.Neutral, 100f);
            targetable.AssignNetworkObjectIdForTests(9001UL);
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
            ship.AddComponent<ShipReactorPowerController>();
            ship.AddComponent<LocalShipRadarSensor>();

            var targetable = ship.AddComponent<TargetableEntity>();
            targetable.Configure("Player Ship", TargetAffiliation.Friendly, 100f);
            targetable.AssignNetworkObjectIdForTests(1000UL);

            ship.AddComponent<NetworkDamageableHealth>();
            ship.AddComponent<NetworkShipTargetingController>();
            ship.AddComponent<ShipTargetInputController>();
            ship.AddComponent<NetworkShipBeamWeaponController>();
            ship.AddComponent<ShipBeamWeaponInputController>();
            ship.AddComponent<BeamWeaponVfx>();

            ship.AddComponent<ShipFlightTelemetryAdapter>();
            ship.AddComponent<ShipWeaponsInputController>();

            LocalPlayerSystemsEvents.NotifyLocalPlayerShipReady(ship);

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
