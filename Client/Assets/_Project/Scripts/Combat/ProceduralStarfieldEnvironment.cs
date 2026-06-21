using UnityEngine;

namespace IronExiles.Combat
{
    public static class ProceduralStarfieldEnvironment
    {
        public const string MaterialResourcePath = "ProceduralStarfield";
        static readonly int ParallaxEulerId = Shader.PropertyToID("_ParallaxEuler");

        public static Material ActiveSkyboxMaterial { get; private set; }

        public static void Apply()
        {
            var material = LoadStarfieldMaterial();
            if (material == null)
            {
                ApplyFallbackSky();
                return;
            }

            ActiveSkyboxMaterial = material;
            material.SetVector(ParallaxEulerId, Vector4.zero);
            RenderSettings.skybox = material;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.03f, 0.03f, 0.05f, 1f);

            var camera = Camera.main;
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.Skybox;
                EnsureController(camera);
            }
        }

        public static void SetParallaxEuler(Vector2 parallaxRadians)
        {
            if (ActiveSkyboxMaterial == null)
            {
                return;
            }

            ActiveSkyboxMaterial.SetVector(ParallaxEulerId, new Vector4(parallaxRadians.x, parallaxRadians.y, 0f, 0f));
        }

        static Material LoadStarfieldMaterial()
        {
            var fromResources = Resources.Load<Material>(MaterialResourcePath);
            if (fromResources != null)
            {
                return Object.Instantiate(fromResources);
            }

            const string shaderName = "IronExiles/ProceduralStarfield";
            var shader = Shader.Find(shaderName);
            return shader != null ? new Material(shader) : null;
        }

        static void ApplyFallbackSky()
        {
            RenderSettings.skybox = null;
            var camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.005f, 0.005f, 0.015f, 1f);
        }

        static void EnsureController(Camera camera)
        {
            if (camera.GetComponent<ProceduralStarfieldController>() == null)
            {
                camera.gameObject.AddComponent<ProceduralStarfieldController>();
            }
        }
    }
}
