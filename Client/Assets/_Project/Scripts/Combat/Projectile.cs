using UnityEngine;

namespace IronExiles.Combat
{
    public sealed class Projectile : MonoBehaviour
    {
        public enum ProjectileType { Rail, Missile }

        private ProjectileType _type;
        private Transform _targetTransform;
        private Vector3 _targetPosition;
        private Vector3 _direction;
        private float _speed;
        private float _damage;
        private NetworkDamageableHealth _targetHealth;
        private bool _hasTarget;

        private GameObject _visuals;
        private Transform _flameTail;

        public static Projectile Create(ProjectileType type, Vector3 spawnPos, Transform targetTransform, NetworkDamageableHealth targetHealth, float damage, Vector3 fallbackDir)
        {
            var go = new GameObject(type == ProjectileType.Rail ? "RailProjectile" : "MissileProjectile");
            go.transform.position = spawnPos;

            var proj = go.AddComponent<Projectile>();
            proj.Initialize(type, targetTransform, targetHealth, damage, fallbackDir);
            return proj;
        }

        private void Initialize(ProjectileType type, Transform target, NetworkDamageableHealth health, float damage, Vector3 fallbackDir)
        {
            _type = type;
            _targetTransform = target;
            _targetHealth = health;
            _damage = damage;
            _direction = fallbackDir.normalized;

            if (target != null)
            {
                _targetPosition = target.position;
                _hasTarget = true;
                _direction = (_targetPosition - transform.position).normalized;
            }
            else
            {
                _hasTarget = false;
            }

            if (type == ProjectileType.Rail)
            {
                _speed = 350f; // Very fast
                BuildRailVisuals();
            }
            else
            {
                _speed = 120f; // Slower, rocket-like speed
                BuildMissileVisuals();
            }

            // Face the direction of travel
            if (_direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_direction);
            }

            // Autodestruct after 8 seconds of flight to prevent orphans
            SafeDestroy(gameObject, 8f);
        }

        private static void SafeDestroy(Object obj, float delay = 0f)
        {
            if (obj == null) return;
            if (Application.isPlaying)
            {
                if (delay > 0f)
                {
                    Destroy(obj, delay);
                }
                else
                {
                    Destroy(obj);
                }
            }
            else
            {
                DestroyImmediate(obj);
            }
        }

        private void BuildRailVisuals()
        {
            _visuals = new GameObject("Visuals");
            _visuals.transform.SetParent(transform, false);

            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.color = Color.white;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.white * 3f);

            // Create three thin, crossed cylinders to form a 3D star
            CreateStarSpoke(Vector3.up, new Vector3(0.15f, 1.2f, 0.15f), mat);
            CreateStarSpoke(Vector3.right, new Vector3(1.2f, 0.15f, 0.15f), mat);
            CreateStarSpoke(Vector3.forward, new Vector3(0.15f, 0.15f, 1.2f), mat);

            // Add bright white light
            var light = _visuals.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = Color.white;
            light.range = 15f;
            light.intensity = 6f;
        }

        private void CreateStarSpoke(Vector3 direction, Vector3 scale, Material mat)
        {
            var spoke = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            SafeDestroy(spoke.GetComponent<Collider>());
            spoke.transform.SetParent(_visuals.transform, false);
            spoke.transform.localScale = scale;
            spoke.transform.rotation = Quaternion.LookRotation(direction);
            var rend = spoke.GetComponent<Renderer>();
            if (rend != null) rend.sharedMaterial = mat;
        }

        private void BuildMissileVisuals()
        {
            _visuals = new GameObject("Visuals");
            _visuals.transform.SetParent(transform, false);

            // Rocket Body (Cylinder)
            var body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            SafeDestroy(body.GetComponent<Collider>());
            body.transform.SetParent(_visuals.transform, false);
            body.transform.localScale = new Vector3(0.4f, 1.2f, 0.4f);
            body.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            var bodyRend = body.GetComponent<Renderer>();
            var bodyMat = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
            bodyMat.color = new Color(0.7f, 0.73f, 0.75f);
            if (bodyRend != null) bodyRend.sharedMaterial = bodyMat;

            // Rocket Nose (Cone - represented beautifully as a red Sphere shifted forward)
            var nose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            SafeDestroy(nose.GetComponent<Collider>());
            nose.transform.SetParent(_visuals.transform, false);
            nose.transform.localScale = new Vector3(0.42f, 0.6f, 0.42f);
            nose.transform.localPosition = new Vector3(0f, 0f, 1.2f);
            var noseRend = nose.GetComponent<Renderer>();
            var noseMat = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
            noseMat.color = Color.red;
            if (noseRend != null) noseRend.sharedMaterial = noseMat;

            // Rocket Fins (using 2 scaled cubes at the tail)
            var finMat = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
            finMat.color = new Color(0.2f, 0.2f, 0.2f);

            CreateFin(new Vector3(0.8f, 0.1f, 0.4f), new Vector3(0f, 0f, -1.0f));
            CreateFin(new Vector3(0.1f, 0.8f, 0.4f), new Vector3(0f, 0f, -1.0f));

            // Engine Flame (represented as an orange cylinder trail)
            var flame = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            SafeDestroy(flame.GetComponent<Collider>());
            flame.transform.SetParent(_visuals.transform, false);
            flame.transform.localScale = new Vector3(0.25f, 0.6f, 0.25f);
            flame.transform.localPosition = new Vector3(0f, 0f, -1.6f);
            flame.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            _flameTail = flame.transform;

            var flameRend = flame.GetComponent<Renderer>();
            var flameMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            flameMat.color = new Color(1f, 0.5f, 0f);
            flameMat.EnableKeyword("_EMISSION");
            flameMat.SetColor("_EmissionColor", new Color(1f, 0.4f, 0f) * 4f);
            if (flameRend != null) flameRend.sharedMaterial = flameMat;

            // Engine light glow
            var light = _visuals.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.45f, 0.1f);
            light.range = 12f;
            light.intensity = 5f;
        }

        private void CreateFin(Vector3 scale, Vector3 localPos)
        {
            var fin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            SafeDestroy(fin.GetComponent<Collider>());
            fin.transform.SetParent(_visuals.transform, false);
            fin.transform.localScale = scale;
            fin.transform.localPosition = localPos;
            var rend = fin.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
            mat.color = new Color(0.15f, 0.15f, 0.15f);
            if (rend != null) rend.sharedMaterial = mat;
        }

        void Update()
        {
            if (_hasTarget && _targetTransform != null)
            {
                _targetPosition = _targetTransform.position;
                _direction = (_targetPosition - transform.position).normalized;
            }

            // Move forward
            float step = _speed * Time.deltaTime;
            Vector3 currentPos = transform.position;
            Vector3 nextPos = currentPos + _direction * step;

            // Check if we will reach or pass the target this frame
            if (_hasTarget)
            {
                float prevDist = Vector3.Distance(currentPos, _targetPosition);
                float nextDist = Vector3.Distance(nextPos, _targetPosition);

                if (nextDist >= prevDist || nextDist < 1f)
                {
                    OnHitTarget();
                    return;
                }
            }

            transform.position = nextPos;

            // Rotate towards direction
            if (_direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_direction);
            }

            // Dynamic flame tail flickering
            if (_flameTail != null)
            {
                float flicker = Random.Range(0.85f, 1.2f);
                _flameTail.localScale = new Vector3(0.25f, 0.6f * flicker, 0.25f);
            }
        }

        private void OnHitTarget()
        {
            if (_targetHealth != null && !_targetHealth.IsDestroyed)
            {
                var hullDamage = _damage;

                var shieldController = _targetHealth.GetComponent<NetworkShipShieldController>();
                if (shieldController != null)
                {
                    var worldAttackDirection = _direction.normalized;
                    hullDamage = shieldController.ApplyDirectionalDamage(worldAttackDirection, _damage);
                }

                if (hullDamage > 0f)
                {
                    _targetHealth.ApplyDamage(hullDamage);
                }

                Debug.Log($"[Weapons] Projectile hit target for {_damage} damage (shield absorbed {_damage - hullDamage:F1})!");
            }

            // Spawn explosive flash visual
            var flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            SafeDestroy(flash.GetComponent<Collider>());
            flash.transform.position = _hasTarget ? _targetPosition : transform.position;
            flash.transform.localScale = Vector3.one * (_type == ProjectileType.Rail ? 2.5f : 4f);

            var rend = flash.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.color = _type == ProjectileType.Rail ? Color.white : new Color(1f, 0.6f, 0.1f);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", mat.color * 5f);
            if (rend != null) rend.sharedMaterial = mat;

            var light = flash.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = mat.color;
            light.range = _type == ProjectileType.Rail ? 15f : 25f;
            light.intensity = 8f;

            // Destroy visual flash quickly
            SafeDestroy(flash, 0.15f);
            SafeDestroy(gameObject);
        }
    }
}
