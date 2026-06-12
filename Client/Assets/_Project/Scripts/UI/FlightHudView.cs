using UnityEngine;
using UnityEngine.UI;

namespace IronExiles.UI
{
    public sealed class FlightHudView
    {
        readonly Canvas _canvas;
        readonly Text _speedText;
        readonly Text _headingText;
        readonly Image _hullFill;

        public Canvas Canvas => _canvas;

        public static FlightHudView Create(Transform parent)
        {
            var canvasGo = new GameObject("FlightHUD");
            canvasGo.transform.SetParent(parent, false);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            canvasGo.AddComponent<GraphicRaycaster>();

            var panel = CreatePanel(canvasGo.transform);
            var speedText = CreateLabel(panel, "SpeedLabel", new Vector2(16f, 16f), TextAnchor.LowerLeft);
            var headingText = CreateLabel(panel, "HeadingLabel", new Vector2(16f, 48f), TextAnchor.LowerLeft);
            var hullBackground = CreateHullBarBackground(panel);
            var hullFill = CreateHullBarFill(hullBackground);

            return new FlightHudView(canvas, speedText, headingText, hullFill);
        }

        FlightHudView(Canvas canvas, Text speedText, Text headingText, Image hullFill)
        {
            _canvas = canvas;
            _speedText = speedText;
            _headingText = headingText;
            _hullFill = hullFill;
        }

        public void Apply(FlightHudDisplayState state)
        {
            var visible = !string.IsNullOrEmpty(state.SpeedText);
            _canvas.enabled = visible;
            if (!visible)
            {
                return;
            }

            _speedText.text = state.SpeedText;
            _headingText.text = state.HeadingText;
            _hullFill.fillAmount = Mathf.Clamp01(state.HullFill01);
        }

        static RectTransform CreatePanel(Transform parent)
        {
            var panelGo = new GameObject("Panel");
            panelGo.transform.SetParent(parent, false);
            var rect = panelGo.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        static Text CreateLabel(RectTransform parent, string name, Vector2 anchoredPosition, TextAnchor anchor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = PivotForTextAnchor(anchor);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(320f, 28f);

            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 22;
            text.color = Color.white;
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            return text;
        }

        static Vector2 PivotForTextAnchor(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft: return new Vector2(0f, 1f);
                case TextAnchor.UpperCenter: return new Vector2(0.5f, 1f);
                case TextAnchor.UpperRight: return new Vector2(1f, 1f);
                case TextAnchor.MiddleLeft: return new Vector2(0f, 0.5f);
                case TextAnchor.MiddleCenter: return new Vector2(0.5f, 0.5f);
                case TextAnchor.MiddleRight: return new Vector2(1f, 0.5f);
                case TextAnchor.LowerCenter: return new Vector2(0.5f, 0f);
                case TextAnchor.LowerRight: return new Vector2(1f, 0f);
                default: return new Vector2(0f, 0f);
            }
        }

        static RectTransform CreateHullBarBackground(RectTransform parent)
        {
            var go = new GameObject("HullBarBackground");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(16f, 88f);
            rect.sizeDelta = new Vector2(220f, 18f);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.1f, 0.1f, 0.12f, 0.85f);
            return rect;
        }

        static Image CreateHullBarFill(RectTransform background)
        {
            var go = new GameObject("HullBarFill");
            go.transform.SetParent(background, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(2f, 2f);
            rect.offsetMax = new Vector2(-2f, -2f);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.2f, 0.85f, 0.35f, 1f);
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
            image.fillAmount = 1f;
            return image;
        }
    }
}
