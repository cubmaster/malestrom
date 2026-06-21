using IronExiles.Combat;
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
        readonly Image _shieldFill;
        readonly Image _jumpChargeFill;
        readonly Text _jumpStatusText;
        readonly Image[] _powerBars;
        readonly Text[] _powerLabels;
        readonly HardpointSlotUI[] _hardpointSlots;
        readonly RectTransform _radarPanel;
        readonly Text _radarCountText;
        readonly Image[] _radarBlips;
        readonly Text _lockedTargetNameText;
        readonly Text _lockedTargetDistanceText;
        readonly Image _lockedTargetHullFill;
        readonly Image _reticle;

        public Canvas Canvas => _canvas;

        struct HardpointSlotUI
        {
            public Text Label;
            public Image ChargeFill;
            public Image Background;
        }

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

            var speedText = CreateLabel(canvasGo.transform, "SpeedLabel",
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(80f, 200f), 24, TextAnchor.LowerLeft);
            var headingText = CreateLabel(canvasGo.transform, "HeadingLabel",
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(80f, 230f), 20, TextAnchor.LowerLeft);

            var hullFill = CreateStatusBar(canvasGo.transform, "HullBar", new Vector2(80f, 260f), 200f, 14f,
                new Color(0.2f, 0.85f, 0.35f, 1f), "HULL");
            var shieldFill = CreateStatusBar(canvasGo.transform, "ShieldBar", new Vector2(80f, 282f), 200f, 14f,
                new Color(0.3f, 0.6f, 1f, 1f), "SHLD");

            var jumpChargeFill = CreateStatusBar(canvasGo.transform, "JumpBar", new Vector2(80f, 310f), 160f, 12f,
                new Color(0.8f, 0.4f, 1f, 1f), "");
            var jumpStatusText = CreateLabel(canvasGo.transform, "JumpStatus",
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(250f, 310f), 16, TextAnchor.LowerLeft);
            jumpStatusText.color = new Color(0.8f, 0.4f, 1f, 1f);

            var powerBars = new Image[4];
            var powerLabels = new Text[4];
            string[] powerNames = { "WPN", "SHD", "ENG", "ECM" };
            Color[] powerColors =
            {
                new Color(1f, 0.4f, 0.3f, 1f),
                new Color(0.3f, 0.6f, 1f, 1f),
                new Color(0.4f, 1f, 0.5f, 1f),
                new Color(1f, 0.8f, 0.2f, 1f)
            };

            for (int i = 0; i < 4; i++)
            {
                float yPos = 200f + i * 22f;
                powerLabels[i] = CreateLabel(canvasGo.transform, $"PowerLabel_{powerNames[i]}",
                    new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-280f, yPos), 14, TextAnchor.LowerRight);
                powerLabels[i].text = powerNames[i];
                powerLabels[i].color = powerColors[i];
                powerBars[i] = CreateStatusBar(canvasGo.transform, $"PowerBar_{powerNames[i]}",
                    new Vector2(-260f, yPos), 120f, 12f, powerColors[i], "",
                    new Vector2(1f, 0f), new Vector2(1f, 0f));
            }

            var hardpointSlots = CreateHardpointPanel(canvasGo.transform);

            var radarPanel = CreateRadarPanel(canvasGo.transform);
            var radarBlips = CreateRadarBlips(radarPanel, TargetingSensorSettings.DefaultMaxRadarContacts);
            var radarCountText = CreateLabel(canvasGo.transform, "RadarCount",
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-80f, -90f), 14, TextAnchor.UpperRight);
            radarCountText.color = new Color(0.4f, 1f, 0.6f, 0.8f);

            var lockedTargetNameText = CreateLabel(canvasGo.transform, "LockedTargetName",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), 18, TextAnchor.UpperCenter);
            lockedTargetNameText.color = new Color(1f, 0.75f, 0.35f, 1f);
            var lockedTargetDistanceText = CreateLabel(canvasGo.transform, "LockedTargetDistance",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -62f), 14, TextAnchor.UpperCenter);
            lockedTargetDistanceText.color = new Color(0.9f, 0.9f, 0.9f, 0.85f);
            var lockedTargetHullFill = CreateStatusBar(canvasGo.transform, "LockedTargetHull", new Vector2(-100f, -82f), 200f, 12f,
                new Color(0.95f, 0.35f, 0.25f, 1f), "",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            var reticle = CreateReticle(canvasGo.transform);

            return new FlightHudView(canvas, speedText, headingText, hullFill, shieldFill,
                jumpChargeFill, jumpStatusText, powerBars, powerLabels, hardpointSlots,
                radarPanel, radarCountText, radarBlips, lockedTargetNameText, lockedTargetDistanceText,
                lockedTargetHullFill, reticle);
        }

        FlightHudView(Canvas canvas, Text speedText, Text headingText, Image hullFill,
            Image shieldFill, Image jumpChargeFill, Text jumpStatusText,
            Image[] powerBars, Text[] powerLabels, HardpointSlotUI[] hardpointSlots,
            RectTransform radarPanel, Text radarCountText, Image[] radarBlips,
            Text lockedTargetNameText, Text lockedTargetDistanceText, Image lockedTargetHullFill,
            Image reticle)
        {
            _canvas = canvas;
            _speedText = speedText;
            _headingText = headingText;
            _hullFill = hullFill;
            _shieldFill = shieldFill;
            _jumpChargeFill = jumpChargeFill;
            _jumpStatusText = jumpStatusText;
            _powerBars = powerBars;
            _powerLabels = powerLabels;
            _hardpointSlots = hardpointSlots;
            _radarPanel = radarPanel;
            _radarCountText = radarCountText;
            _radarBlips = radarBlips;
            _lockedTargetNameText = lockedTargetNameText;
            _lockedTargetDistanceText = lockedTargetDistanceText;
            _lockedTargetHullFill = lockedTargetHullFill;
            _reticle = reticle;
        }

        public void Apply(FlightHudDisplayState state)
        {
            _canvas.enabled = state.IsVisible;
            if (!state.IsVisible) return;

            _speedText.text = state.SpeedText;
            _headingText.text = state.HeadingText;
            _hullFill.fillAmount = Mathf.Clamp01(state.HullFill01);
            _shieldFill.fillAmount = Mathf.Clamp01(state.ShieldFill01);
            _jumpChargeFill.fillAmount = Mathf.Clamp01(state.JumpChargeFill01);
            _jumpStatusText.text = state.JumpStatusText;
            _jumpStatusText.color = state.JumpReady
                ? new Color(0.5f, 1f, 0.7f, 1f)
                : new Color(0.8f, 0.4f, 1f, 1f);

            _powerBars[0].fillAmount = state.PowerWeapons;
            _powerBars[1].fillAmount = state.PowerShields;
            _powerBars[2].fillAmount = state.PowerEngines;
            _powerBars[3].fillAmount = state.PowerEcm;

            if (state.Hardpoints != null)
            {
                for (int i = 0; i < _hardpointSlots.Length && i < state.Hardpoints.Length; i++)
                {
                    var hp = state.Hardpoints[i];
                    _hardpointSlots[i].Label.text = hp.Label;
                    _hardpointSlots[i].ChargeFill.fillAmount = hp.ChargeFill01;
                    _hardpointSlots[i].Background.color = hp.IsActive
                        ? new Color(0.08f, 0.12f, 0.18f, 0.9f)
                        : new Color(0.15f, 0.05f, 0.05f, 0.9f);
                    _hardpointSlots[i].ChargeFill.color = GetHardpointColor(hp.Type);
                }
            }

            _radarCountText.text = state.RadarContactCount > 0
                ? $"{state.RadarContactCount} CONTACT{(state.RadarContactCount > 1 ? "S" : "")}"
                : "NO CONTACTS";

            ApplyRadarBlips(state.RadarContacts, state.RadarContactCount);
            ApplyLockedTargetPanel(state);
        }

        void ApplyLockedTargetPanel(FlightHudDisplayState state)
        {
            var hasLock = !string.IsNullOrEmpty(state.LockedTargetName);
            _lockedTargetNameText.gameObject.SetActive(hasLock);
            _lockedTargetDistanceText.gameObject.SetActive(hasLock);
            _lockedTargetHullFill.transform.parent.gameObject.SetActive(hasLock);

            if (!hasLock)
            {
                return;
            }

            _lockedTargetNameText.text = state.LockedTargetName;
            _lockedTargetDistanceText.text = $"{state.LockedTargetDistanceMeters:F0} m";
            _lockedTargetHullFill.fillAmount = Mathf.Clamp01(state.LockedTargetHullFill01);
        }

        void ApplyRadarBlips(Vector3[] contacts, int count)
        {
            for (var i = 0; i < _radarBlips.Length; i++)
            {
                var blip = _radarBlips[i];
                if (contacts == null || i >= count)
                {
                    blip.enabled = false;
                    continue;
                }

                var contact = contacts[i];
                blip.enabled = true;
                var rect = blip.rectTransform;
                var half = _radarPanel.rect.width * 0.5f - 8f;
                rect.anchoredPosition = new Vector2(contact.x * half, contact.y * half);
            }
        }

        static Color GetHardpointColor(HardpointType type)
        {
            switch (type)
            {
                case HardpointType.Weapon: return new Color(1f, 0.5f, 0.3f, 1f);
                case HardpointType.Missile: return new Color(1f, 0.2f, 0.2f, 1f);
                case HardpointType.PointDefense: return new Color(1f, 1f, 0.3f, 1f);
                case HardpointType.Shield: return new Color(0.3f, 0.6f, 1f, 1f);
                default: return Color.white;
            }
        }

        static Text CreateLabel(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, int fontSize, TextAnchor anchor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(320f, 28f);

            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            return text;
        }

        static Image CreateStatusBar(Transform parent, string name, Vector2 position, float width, float height,
            Color fillColor, string label, Vector2? anchorMin = null, Vector2? anchorMax = null)
        {
            var aMin = anchorMin ?? new Vector2(0f, 0f);
            var aMax = anchorMax ?? new Vector2(0f, 0f);

            var bgGo = new GameObject($"{name}_BG");
            bgGo.transform.SetParent(parent, false);
            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = aMin;
            bgRect.anchorMax = aMax;
            bgRect.pivot = new Vector2(0f, 0f);
            bgRect.anchoredPosition = position;
            bgRect.sizeDelta = new Vector2(width, height);
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = new Color(0.08f, 0.08f, 0.1f, 0.85f);
            bgImg.raycastTarget = false;

            var fillGo = new GameObject($"{name}_Fill");
            fillGo.transform.SetParent(bgGo.transform, false);
            var fillRect = fillGo.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(1f, 1f);
            fillRect.offsetMax = new Vector2(-1f, -1f);
            var fillImg = fillGo.AddComponent<Image>();
            fillImg.color = fillColor;
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImg.fillAmount = 1f;
            fillImg.raycastTarget = false;

            if (!string.IsNullOrEmpty(label))
            {
                var labelGo = new GameObject($"{name}_Label");
                labelGo.transform.SetParent(bgGo.transform, false);
                var labelRect = labelGo.AddComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = new Vector2(4f, 0f);
                labelRect.offsetMax = Vector2.zero;
                var labelText = labelGo.AddComponent<Text>();
                labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                labelText.fontSize = (int)(height - 3);
                labelText.color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
                labelText.alignment = TextAnchor.MiddleLeft;
                labelText.text = label;
                labelText.raycastTarget = false;
            }

            return fillImg;
        }

        static HardpointSlotUI[] CreateHardpointPanel(Transform parent)
        {
            int slotCount = 7;
            var slots = new HardpointSlotUI[slotCount];
            float slotWidth = 70f;
            float slotHeight = 40f;
            float spacing = 4f;
            float totalWidth = slotCount * slotWidth + (slotCount - 1) * spacing;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < slotCount; i++)
            {
                float xPos = startX + i * (slotWidth + spacing);

                var slotGo = new GameObject($"Hardpoint_{i}");
                slotGo.transform.SetParent(parent, false);
                var slotRect = slotGo.AddComponent<RectTransform>();
                slotRect.anchorMin = new Vector2(0.5f, 0f);
                slotRect.anchorMax = new Vector2(0.5f, 0f);
                slotRect.pivot = new Vector2(0.5f, 0f);
                slotRect.anchoredPosition = new Vector2(xPos + slotWidth / 2f, 170f);
                slotRect.sizeDelta = new Vector2(slotWidth, slotHeight);

                var bgImg = slotGo.AddComponent<Image>();
                bgImg.color = new Color(0.08f, 0.12f, 0.18f, 0.9f);
                bgImg.raycastTarget = false;

                var fillGo = new GameObject("Fill");
                fillGo.transform.SetParent(slotGo.transform, false);
                var fillRect = fillGo.AddComponent<RectTransform>();
                fillRect.anchorMin = new Vector2(0f, 0f);
                fillRect.anchorMax = new Vector2(1f, 0.3f);
                fillRect.offsetMin = new Vector2(2f, 2f);
                fillRect.offsetMax = new Vector2(-2f, 0f);
                var fillImg = fillGo.AddComponent<Image>();
                fillImg.type = Image.Type.Filled;
                fillImg.fillMethod = Image.FillMethod.Horizontal;
                fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
                fillImg.fillAmount = 1f;
                fillImg.color = Color.white;
                fillImg.raycastTarget = false;

                var labelGo = new GameObject("Label");
                labelGo.transform.SetParent(slotGo.transform, false);
                var labelRect = labelGo.AddComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;
                var labelText = labelGo.AddComponent<Text>();
                labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                labelText.fontSize = 11;
                labelText.color = new Color(0.9f, 0.9f, 0.9f, 0.9f);
                labelText.alignment = TextAnchor.MiddleCenter;
                labelText.raycastTarget = false;

                slots[i] = new HardpointSlotUI
                {
                    Label = labelText,
                    ChargeFill = fillImg,
                    Background = bgImg
                };
            }

            return slots;
        }

        static RectTransform CreateRadarPanel(Transform parent)
        {
            var go = new GameObject("RadarPanel");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-20f, -20f);
            rect.sizeDelta = new Vector2(160f, 160f);

            var bg = go.AddComponent<Image>();
            bg.color = new Color(0.02f, 0.06f, 0.04f, 0.85f);
            bg.raycastTarget = false;

            var borderGo = new GameObject("RadarBorder");
            borderGo.transform.SetParent(go.transform, false);
            var borderRect = borderGo.AddComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = Vector2.zero;
            borderRect.offsetMax = Vector2.zero;
            var outline = borderGo.AddComponent<Outline>();
            var borderImg = borderGo.AddComponent<Image>();
            borderImg.color = Color.clear;
            borderImg.raycastTarget = false;
            outline.effectColor = new Color(0.2f, 0.7f, 0.4f, 0.6f);
            outline.effectDistance = new Vector2(2f, 2f);

            var crosshairH = new GameObject("CrossH");
            crosshairH.transform.SetParent(go.transform, false);
            var chRect = crosshairH.AddComponent<RectTransform>();
            chRect.anchorMin = new Vector2(0f, 0.5f);
            chRect.anchorMax = new Vector2(1f, 0.5f);
            chRect.sizeDelta = new Vector2(0f, 1f);
            var chImg = crosshairH.AddComponent<Image>();
            chImg.color = new Color(0.2f, 0.5f, 0.3f, 0.4f);
            chImg.raycastTarget = false;

            var crosshairV = new GameObject("CrossV");
            crosshairV.transform.SetParent(go.transform, false);
            var cvRect = crosshairV.AddComponent<RectTransform>();
            cvRect.anchorMin = new Vector2(0.5f, 0f);
            cvRect.anchorMax = new Vector2(0.5f, 1f);
            cvRect.sizeDelta = new Vector2(1f, 0f);
            var cvImg = crosshairV.AddComponent<Image>();
            cvImg.color = new Color(0.2f, 0.5f, 0.3f, 0.4f);
            cvImg.raycastTarget = false;

            var titleText = CreateLabel(go.transform, "RadarTitle",
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(4f, -16f), 10, TextAnchor.UpperLeft);
            titleText.text = "RADAR";
            titleText.color = new Color(0.3f, 0.8f, 0.5f, 0.7f);

            var forwardMarker = new GameObject("RadarForwardMarker");
            forwardMarker.transform.SetParent(go.transform, false);
            var forwardRect = forwardMarker.AddComponent<RectTransform>();
            forwardRect.anchorMin = new Vector2(0.5f, 0.5f);
            forwardRect.anchorMax = new Vector2(0.5f, 0.5f);
            forwardRect.pivot = new Vector2(0.5f, 0f);
            forwardRect.anchoredPosition = new Vector2(0f, 2f);
            forwardRect.sizeDelta = new Vector2(4f, 18f);
            var forwardImg = forwardMarker.AddComponent<Image>();
            forwardImg.color = new Color(0.35f, 1f, 0.55f, 0.85f);
            forwardImg.raycastTarget = false;

            return rect;
        }

        static Image[] CreateRadarBlips(RectTransform radarPanel, int maxBlips)
        {
            var blips = new Image[maxBlips];
            for (var i = 0; i < maxBlips; i++)
            {
                var go = new GameObject($"RadarBlip_{i}");
                go.transform.SetParent(radarPanel, false);
                var rect = go.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(6f, 6f);
                var img = go.AddComponent<Image>();
                img.color = new Color(1f, 0.45f, 0.25f, 0.95f);
                img.raycastTarget = false;
                img.enabled = false;
                blips[i] = img;
            }

            return blips;
        }

        static Image CreateReticle(Transform parent)
        {
            var go = new GameObject("Reticle");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(40f, 40f);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.4f, 1f, 0.6f, 0.5f);
            img.raycastTarget = false;

            var topGo = new GameObject("Top");
            topGo.transform.SetParent(go.transform, false);
            var topRect = topGo.AddComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0.5f, 1f);
            topRect.anchorMax = new Vector2(0.5f, 1f);
            topRect.pivot = new Vector2(0.5f, 1f);
            topRect.anchoredPosition = new Vector2(0f, 4f);
            topRect.sizeDelta = new Vector2(2f, 10f);
            var topImg = topGo.AddComponent<Image>();
            topImg.color = new Color(0.4f, 1f, 0.6f, 0.7f);
            topImg.raycastTarget = false;

            var botGo = new GameObject("Bottom");
            botGo.transform.SetParent(go.transform, false);
            var botRect = botGo.AddComponent<RectTransform>();
            botRect.anchorMin = new Vector2(0.5f, 0f);
            botRect.anchorMax = new Vector2(0.5f, 0f);
            botRect.pivot = new Vector2(0.5f, 0f);
            botRect.anchoredPosition = new Vector2(0f, -4f);
            botRect.sizeDelta = new Vector2(2f, 10f);
            var botImg = botGo.AddComponent<Image>();
            botImg.color = new Color(0.4f, 1f, 0.6f, 0.7f);
            botImg.raycastTarget = false;

            var leftGo = new GameObject("Left");
            leftGo.transform.SetParent(go.transform, false);
            var leftRect = leftGo.AddComponent<RectTransform>();
            leftRect.anchorMin = new Vector2(0f, 0.5f);
            leftRect.anchorMax = new Vector2(0f, 0.5f);
            leftRect.pivot = new Vector2(0f, 0.5f);
            leftRect.anchoredPosition = new Vector2(-4f, 0f);
            leftRect.sizeDelta = new Vector2(10f, 2f);
            var leftImg = leftGo.AddComponent<Image>();
            leftImg.color = new Color(0.4f, 1f, 0.6f, 0.7f);
            leftImg.raycastTarget = false;

            var rightGo = new GameObject("Right");
            rightGo.transform.SetParent(go.transform, false);
            var rightRect = rightGo.AddComponent<RectTransform>();
            rightRect.anchorMin = new Vector2(1f, 0.5f);
            rightRect.anchorMax = new Vector2(1f, 0.5f);
            rightRect.pivot = new Vector2(1f, 0.5f);
            rightRect.anchoredPosition = new Vector2(4f, 0f);
            rightRect.sizeDelta = new Vector2(10f, 2f);
            var rightImg = rightGo.AddComponent<Image>();
            rightImg.color = new Color(0.4f, 1f, 0.6f, 0.7f);
            rightImg.raycastTarget = false;

            img.enabled = false;

            return img;
        }
    }
}
