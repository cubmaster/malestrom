using UnityEngine;
using UnityEngine.UI;

namespace IronExiles.UI
{
    public sealed class CockpitFrameView
    {
        readonly Canvas _canvas;

        public Canvas Canvas => _canvas;

        public static CockpitFrameView Create(Transform parent)
        {
            var canvasGo = new GameObject("CockpitFrame");
            canvasGo.transform.SetParent(parent, false);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 90;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            CreateStrut(canvasGo.transform, "TopStrut", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -60f), new Vector2(0f, 0f));
            CreateStrut(canvasGo.transform, "BottomStrut", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 80f));
            CreateStrut(canvasGo.transform, "LeftStrut", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(50f, 0f));
            CreateStrut(canvasGo.transform, "RightStrut", new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-50f, 0f), new Vector2(0f, 0f));

            CreateCornerStrut(canvasGo.transform, "TopLeftCorner", new Vector2(0f, 1f), new Vector2(0.15f, 1f), new Vector2(0f, -120f), new Vector2(0f, 0f));
            CreateCornerStrut(canvasGo.transform, "TopRightCorner", new Vector2(0.85f, 1f), new Vector2(1f, 1f), new Vector2(0f, -120f), new Vector2(0f, 0f));
            CreateCornerStrut(canvasGo.transform, "BottomLeftCorner", new Vector2(0f, 0f), new Vector2(0.2f, 0f), new Vector2(0f, 0f), new Vector2(0f, 140f));
            CreateCornerStrut(canvasGo.transform, "BottomRightCorner", new Vector2(0.8f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 140f));

            CreateConsolePanel(canvasGo.transform);

            return new CockpitFrameView(canvas);
        }

        CockpitFrameView(Canvas canvas)
        {
            _canvas = canvas;
        }

        static void CreateStrut(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            var img = go.AddComponent<Image>();
            img.color = new Color(0.02f, 0.02f, 0.04f, 0.95f);
            img.raycastTarget = false;
        }

        static void CreateCornerStrut(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            var img = go.AddComponent<Image>();
            img.color = new Color(0.03f, 0.03f, 0.06f, 0.85f);
            img.raycastTarget = false;
        }

        static void CreateConsolePanel(Transform parent)
        {
            var go = new GameObject("ConsolePanel");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.25f, 0f);
            rect.anchorMax = new Vector2(0.75f, 0f);
            rect.offsetMin = new Vector2(0f, 80f);
            rect.offsetMax = new Vector2(0f, 160f);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.015f, 0.02f, 0.035f, 0.9f);
            img.raycastTarget = false;

            var outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(0.1f, 0.3f, 0.4f, 0.6f);
            outline.effectDistance = new Vector2(1f, 1f);
        }
    }
}
