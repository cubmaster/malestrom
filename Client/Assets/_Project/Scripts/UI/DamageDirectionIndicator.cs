using UnityEngine;
using UnityEngine.UI;

namespace IronExiles.UI
{
    public sealed class DamageDirectionIndicator : MonoBehaviour
    {
        const float FadeDuration = 1.5f;
        const float IndicatorOffset = 200f;
        const float IndicatorSize = 40f;

        struct HitMarker
        {
            public Image Image;
            public float Timer;
            public Vector3 WorldDirection;
        }

        HitMarker[] _markers;
        Canvas _canvas;
        Transform _playerTransform;

        public void Initialize(Canvas canvas, Transform playerTransform)
        {
            _canvas = canvas;
            _playerTransform = playerTransform;
            _markers = new HitMarker[8];

            for (int i = 0; i < _markers.Length; i++)
            {
                var go = new GameObject($"DamageDir_{i}");
                go.transform.SetParent(canvas.transform, false);
                var rect = go.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(IndicatorSize, 8f);

                var img = go.AddComponent<Image>();
                img.color = new Color(1f, 0.2f, 0.1f, 0f);
                img.raycastTarget = false;
                go.SetActive(false);

                _markers[i] = new HitMarker { Image = img, Timer = 0f };
            }
        }

        public void ShowHit(Vector3 attackerWorldPosition)
        {
            if (_playerTransform == null) return;

            var direction = (attackerWorldPosition - _playerTransform.position).normalized;

            int slot = -1;
            for (int i = 0; i < _markers.Length; i++)
            {
                if (_markers[i].Timer <= 0f)
                {
                    slot = i;
                    break;
                }
            }

            if (slot < 0)
            {
                float oldest = float.MaxValue;
                for (int i = 0; i < _markers.Length; i++)
                {
                    if (_markers[i].Timer < oldest)
                    {
                        oldest = _markers[i].Timer;
                        slot = i;
                    }
                }
            }

            _markers[slot].Timer = FadeDuration;
            _markers[slot].WorldDirection = direction;
            _markers[slot].Image.gameObject.SetActive(true);
        }

        void Update()
        {
            if (_markers == null || _playerTransform == null) return;

            var cam = Camera.main;
            if (cam == null) return;

            for (int i = 0; i < _markers.Length; i++)
            {
                if (_markers[i].Timer <= 0f)
                {
                    if (_markers[i].Image.gameObject.activeSelf)
                        _markers[i].Image.gameObject.SetActive(false);
                    continue;
                }

                _markers[i].Timer -= Time.deltaTime;
                var alpha = Mathf.Clamp01(_markers[i].Timer / FadeDuration);
                _markers[i].Image.color = new Color(1f, 0.2f, 0.1f, alpha * 0.8f);

                var dir = _markers[i].WorldDirection;
                var localDir = cam.transform.InverseTransformDirection(dir);
                var angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

                var rect = _markers[i].Image.rectTransform;
                var rad = (-angle + 90f) * Mathf.Deg2Rad;
                rect.anchoredPosition = new Vector2(
                    Mathf.Cos(rad) * IndicatorOffset,
                    Mathf.Sin(rad) * IndicatorOffset);
                rect.localRotation = Quaternion.Euler(0f, 0f, -angle);
            }
        }
    }
}
