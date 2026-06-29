using IronExiles.Combat;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace IronExiles.Networking
{
    public static class NetworkPlayerShipFactory
    {
        const string PrefabRootName = "NetworkPlayerShipPrefab";

        public static GameObject CreatePrefab()
        {
            var existing = GameObject.Find(PrefabRootName);
            if (existing != null)
            {
                RuntimeNetworkPrefabUtility.EnsurePlayerShipPrefabHash(existing.GetComponent<NetworkObject>());
                return existing;
            }

            var ship = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ship.name = PrefabRootName;
            ship.transform.localScale = new Vector3(2f, 0.5f, 1f);
            ship.SetActive(false);
            Object.DontDestroyOnLoad(ship);

            var collider = ship.GetComponent<BoxCollider>();
            collider.size = Vector3.one;

            // Build detailed high-quality procedural spaceship visuals
            ConstructSpaceshipVisuals(ship);

            ship.AddComponent<NetworkObject>();
            RuntimeNetworkPrefabUtility.EnsurePlayerShipPrefabHash(ship.GetComponent<NetworkObject>());
            ship.AddComponent<NetworkTransform>();
            var movement = ship.AddComponent<ShipMovementController>();
            movement.SetSectorBoundsExtent(new Vector3(5000f, 5000f, 5000f));
            ship.AddComponent<ShipInputController>();
            ship.AddComponent<NetworkShipMovementController>();
            ship.AddComponent<NetworkShipReactorPowerController>();
            var targetable = ship.AddComponent<TargetableEntity>();
            targetable.Configure("Player Ship", TargetAffiliation.Hostile, 100f);
            ship.AddComponent<NetworkDamageableHealth>();
            ship.AddComponent<NetworkShipTargetingController>();
            ship.AddComponent<ShipTargetInputController>();
            ship.AddComponent<NetworkShipBeamWeaponController>();
            ship.AddComponent<ShipBeamWeaponInputController>();
            ship.AddComponent<BeamWeaponVfx>();
            ship.AddComponent<NetworkedShipSetup>();

            return ship;
        }

        public static void RegisterPrefab(GameObject prefab, NetworkManager networkManager = null)
        {
            RuntimeNetworkPrefabUtility.RegisterPrefab(prefab, networkManager ?? NetworkManager.Singleton);
        }

        private static void ConstructSpaceshipVisuals(GameObject shipRoot)
        {
            // Disable MeshFilter & MeshRenderer on root to not show the plain cube
            var rootRenderer = shipRoot.GetComponent<MeshRenderer>();
            if (rootRenderer != null) rootRenderer.enabled = false;

            // Create a Visuals container
            var visuals = new GameObject("Visuals");
            visuals.transform.SetParent(shipRoot.transform, false);

            // Find URP Lit shader or fallback
            var litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            // Create Materials
            var hullMat = new Material(litShader);
            hullMat.name = "ShipHull";
            hullMat.color = new Color(0.2f, 0.25f, 0.35f, 1f); // Metallic blue-grey
            hullMat.SetFloat("_Metallic", 0.8f);
            hullMat.SetFloat("_Smoothness", 0.6f);

            var cockpitMat = new Material(litShader);
            cockpitMat.name = "ShipCockpit";
            cockpitMat.color = new Color(1f, 0.45f, 0.05f, 1f); // Golden amber
            cockpitMat.SetFloat("_Metallic", 0.9f);
            cockpitMat.SetFloat("_Smoothness", 0.9f);
            cockpitMat.SetColor("_EmissionColor", new Color(1f, 0.45f, 0.05f) * 1.5f);
            cockpitMat.EnableKeyword("_EMISSION");

            var trimMat = new Material(litShader);
            trimMat.name = "ShipTrim";
            trimMat.color = new Color(0.1f, 0.12f, 0.15f, 1f); // Dark carbon/dark grey
            trimMat.SetFloat("_Metallic", 0.5f);
            trimMat.SetFloat("_Smoothness", 0.4f);

            var thrusterMat = new Material(litShader);
            thrusterMat.name = "ShipThruster";
            thrusterMat.color = new Color(0.0f, 0.7f, 1.0f, 1f); // Neon blue glow
            thrusterMat.SetColor("_EmissionColor", new Color(0.0f, 0.7f, 1.0f) * 3f);
            thrusterMat.EnableKeyword("_EMISSION");

            // 1. Main Fuselage
            var fuselage = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.DestroyImmediate(fuselage.GetComponent<Collider>());
            fuselage.name = "Fuselage";
            fuselage.transform.SetParent(visuals.transform, false);
            fuselage.transform.localPosition = new Vector3(0f, 0f, 0f);
            fuselage.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Point forward along Z
            fuselage.transform.localScale = new Vector3(0.5f, 1.4f, 0.5f); // Stretched cylinder
            fuselage.GetComponent<Renderer>().sharedMaterial = hullMat;

            // 2. Cockpit Canopy
            var cockpit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.DestroyImmediate(cockpit.GetComponent<Collider>());
            cockpit.name = "Cockpit";
            cockpit.transform.SetParent(visuals.transform, false);
            cockpit.transform.localPosition = new Vector3(0f, 0.25f, 0.3f);
            cockpit.transform.localScale = new Vector3(0.35f, 0.25f, 0.7f);
            cockpit.GetComponent<Renderer>().sharedMaterial = cockpitMat;

            // 3. Nose Cone
            var nose = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.DestroyImmediate(nose.GetComponent<Collider>());
            nose.name = "NoseCone";
            nose.transform.SetParent(visuals.transform, false);
            nose.transform.localPosition = new Vector3(0f, 0f, 1.3f);
            nose.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            nose.transform.localScale = new Vector3(0.35f, 0.3f, 0.35f);
            nose.GetComponent<Renderer>().sharedMaterial = trimMat;

            // 4. Left Wing
            var leftWing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DestroyImmediate(leftWing.GetComponent<Collider>());
            leftWing.name = "LeftWing";
            leftWing.transform.SetParent(visuals.transform, false);
            leftWing.transform.localPosition = new Vector3(-0.9f, -0.05f, -0.2f);
            leftWing.transform.localRotation = Quaternion.Euler(0f, -20f, -10f); // Swept back & slightly dihedral
            leftWing.transform.localScale = new Vector3(1.4f, 0.08f, 1.0f);
            leftWing.GetComponent<Renderer>().sharedMaterial = hullMat;

            // Left Wing Trim/Fin tip
            var leftFin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DestroyImmediate(leftFin.GetComponent<Collider>());
            leftFin.name = "LeftFin";
            leftFin.transform.SetParent(leftWing.transform, false);
            leftFin.transform.localPosition = new Vector3(-0.5f, 1.5f, -0.1f);
            leftFin.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            leftFin.transform.localScale = new Vector3(1.2f, 0.15f, 0.4f);
            leftFin.GetComponent<Renderer>().sharedMaterial = trimMat;

            // 5. Right Wing
            var rightWing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DestroyImmediate(rightWing.GetComponent<Collider>());
            rightWing.name = "RightWing";
            rightWing.transform.SetParent(visuals.transform, false);
            rightWing.transform.localPosition = new Vector3(0.9f, -0.05f, -0.2f);
            rightWing.transform.localRotation = Quaternion.Euler(0f, 20f, 10f); // Swept back & slightly dihedral
            rightWing.transform.localScale = new Vector3(1.4f, 0.08f, 1.0f);
            rightWing.GetComponent<Renderer>().sharedMaterial = hullMat;

            // Right Wing Trim/Fin tip
            var rightFin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DestroyImmediate(rightFin.GetComponent<Collider>());
            rightFin.name = "RightFin";
            rightFin.transform.SetParent(rightWing.transform, false);
            rightFin.transform.localPosition = new Vector3(0.5f, 1.5f, -0.1f);
            rightFin.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
            rightFin.transform.localScale = new Vector3(1.2f, 0.15f, 0.4f);
            rightFin.GetComponent<Renderer>().sharedMaterial = trimMat;

            // 6. Left Engine
            var leftEngine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.DestroyImmediate(leftEngine.GetComponent<Collider>());
            leftEngine.name = "LeftEngine";
            leftEngine.transform.SetParent(visuals.transform, false);
            leftEngine.transform.localPosition = new Vector3(-0.3f, -0.1f, -1.1f);
            leftEngine.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            leftEngine.transform.localScale = new Vector3(0.25f, 0.4f, 0.25f);
            leftEngine.GetComponent<Renderer>().sharedMaterial = trimMat;

            // Left Engine Glow
            var leftPlume = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.DestroyImmediate(leftPlume.GetComponent<Collider>());
            leftPlume.name = "LeftPlume";
            leftPlume.transform.SetParent(leftEngine.transform, false);
            leftPlume.transform.localPosition = new Vector3(0f, -1.05f, 0f); // Placed at rear of cylinder
            leftPlume.transform.localScale = new Vector3(0.7f, 0.15f, 0.7f);
            leftPlume.GetComponent<Renderer>().sharedMaterial = thrusterMat;

            // 7. Right Engine
            var rightEngine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.DestroyImmediate(rightEngine.GetComponent<Collider>());
            rightEngine.name = "RightEngine";
            rightEngine.transform.SetParent(visuals.transform, false);
            rightEngine.transform.localPosition = new Vector3(0.3f, -0.1f, -1.1f);
            rightEngine.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            rightEngine.transform.localScale = new Vector3(0.25f, 0.4f, 0.25f);
            rightEngine.GetComponent<Renderer>().sharedMaterial = trimMat;

            // Right Engine Glow
            var rightPlume = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.DestroyImmediate(rightPlume.GetComponent<Collider>());
            rightPlume.name = "RightPlume";
            rightPlume.transform.SetParent(rightEngine.transform, false);
            rightPlume.transform.localPosition = new Vector3(0f, -1.05f, 0f); // Placed at rear of cylinder
            rightPlume.transform.localScale = new Vector3(0.7f, 0.15f, 0.7f);
            rightPlume.GetComponent<Renderer>().sharedMaterial = thrusterMat;

            // 8. Dorsal Stabilizer (Vertical Fin)
            var dorsalFin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DestroyImmediate(dorsalFin.GetComponent<Collider>());
            dorsalFin.name = "DorsalFin";
            dorsalFin.transform.SetParent(visuals.transform, false);
            dorsalFin.transform.localPosition = new Vector3(0f, 0.5f, -0.8f);
            dorsalFin.transform.localRotation = Quaternion.Euler(-25f, 0f, 0f); // Angled back
            dorsalFin.transform.localScale = new Vector3(0.08f, 0.7f, 0.6f);
            dorsalFin.GetComponent<Renderer>().sharedMaterial = trimMat;
        }
    }
}
