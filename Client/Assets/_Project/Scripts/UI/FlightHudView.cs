using System;
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
        readonly Slider[] _powerSliders;
        readonly Button[] _powerPresetButtons;
        readonly Text _powerTotalText;
        readonly HardpointSlotUI[] _hardpointSlots;
        readonly RectTransform _radarPanel;
        readonly Text _radarCountText;
        readonly Image[] _radarBlips;
        readonly Text _lockedTargetNameText;
        readonly Text _lockedTargetDistanceText;
        readonly Image _lockedTargetHullFill;
        readonly Image _reticle;
        readonly RectTransform _targetBracket;
        readonly Image _shieldFrontFill;
        readonly Image _shieldRearFill;
        readonly Image _shieldPortFill;
        readonly Image _shieldStarboardFill;

        IShipReactorPowerControl _powerControl;
        bool _suppressPowerCallbacks;

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

            // Directional shield bars arranged as a diamond/cross near the shield bar area
            // Center point at (320, 282) - to the right of the main shield bar
            var shieldFrontFill = CreateStatusBar(canvasGo.transform, "ShieldFront",
                new Vector2(310f, 300f), 40f, 8f, Color.cyan, "F");
            var shieldRearFill = CreateStatusBar(canvasGo.transform, "ShieldRear",
                new Vector2(310f, 264f), 40f, 8f, Color.cyan, "R");
            var shieldPortFill = CreateStatusBar(canvasGo.transform, "ShieldPort",
                new Vector2(286f, 282f), 40f, 8f, Color.cyan, "P");
            var shieldStarboardFill = CreateStatusBar(canvasGo.transform, "ShieldStbd",
                new Vector2(334f, 282f), 40f, 8f, Color.cyan, "S");

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

            var powerSliders = CreatePowerSliders(canvasGo.transform, powerColors);
            var powerPresetButtons = CreatePowerPresetButtons(canvasGo.transform);
            var powerTotalText = CreateLabel(canvasGo.transform, "PowerTotal",
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-280f, 118f), 12, TextAnchor.LowerRight);
            powerTotalText.color = new Color(0.7f, 0.9f, 0.75f, 0.9f);

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
            var targetBracket = CreateTargetBracket(canvasGo.transform);

            return new FlightHudView(canvas, speedText, headingText, hullFill, shieldFill,
                jumpChargeFill, jumpStatusText, powerBars, powerLabels, powerSliders, powerPresetButtons,
                powerTotalText, hardpointSlots,
                radarPanel, radarCountText, radarBlips, lockedTargetNameText, lockedTargetDistanceText,
                lockedTargetHullFill, reticle, targetBracket,
                shieldFrontFill, shieldRearFill, shieldPortFill, shieldStarboardFill);
        }

        FlightHudView(Canvas canvas, Text speedText, Text headingText, Image hullFill,
            Image shieldFill, Image jumpChargeFill, Text jumpStatusText,
            Image[] powerBars, Text[] powerLabels, Slider[] powerSliders, Button[] powerPresetButtons,
            Text powerTotalText, HardpointSlotUI[] hardpointSlots,
            RectTransform radarPanel, Text radarCountText, Image[] radarBlips,
            Text lockedTargetNameText, Text lockedTargetDistanceText, Image lockedTargetHullFill,
            Image reticle, RectTransform targetBracket,
            Image shieldFrontFill, Image shieldRearFill, Image shieldPortFill, Image shieldStarboardFill)
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
            _powerSliders = powerSliders;
            _powerPresetButtons = powerPresetButtons;
            _powerTotalText = powerTotalText;
            _hardpointSlots = hardpointSlots;
            _radarPanel = radarPanel;
            _radarCountText = radarCountText;
            _radarBlips = radarBlips;
            _lockedTargetNameText = lockedTargetNameText;
            _lockedTargetDistanceText = lockedTargetDistanceText;
            _lockedTargetHullFill = lockedTargetHullFill;
            _reticle = reticle;
            _targetBracket = targetBracket;
            _shieldFrontFill = shieldFrontFill;
            _shieldRearFill = shieldRearFill;
            _shieldPortFill = shieldPortFill;
            _shieldStarboardFill = shieldStarboardFill;
        }

        public void BindPowerControl(IShipReactorPowerControl control)
        {
            if (_powerControl != null)
            {
                _powerControl.AllocationChanged -= OnPowerAllocationChanged;
            }

            _powerControl = control;
            var hasControl = _powerControl != null;
            SetPowerControlsVisible(hasControl);

            if (!hasControl)
            {
                return;
            }

            _powerControl.AllocationChanged += OnPowerAllocationChanged;
            SyncPowerSliders(_powerControl.Current);
        }

        void SetPowerControlsVisible(bool visible)
        {
            for (var i = 0; i < _powerSliders.Length; i++)
            {
                _powerSliders[i].gameObject.SetActive(visible);
            }

            for (var i = 0; i < _powerPresetButtons.Length; i++)
            {
                _powerPresetButtons[i].gameObject.SetActive(visible);
            }

            _powerTotalText.gameObject.SetActive(visible);
        }

        void OnPowerAllocationChanged(PowerAllocation allocation) => SyncPowerSliders(allocation);

        void SyncPowerSliders(PowerAllocation allocation)
        {
            _suppressPowerCallbacks = true;
            _powerSliders[0].value = allocation.Weapons;
            _powerSliders[1].value = allocation.Shields;
            _powerSliders[2].value = allocation.Engines;
            _powerSliders[3].value = allocation.Ecm;
            _suppressPowerCallbacks = false;
            UpdatePowerTotalLabel(allocation);
        }

        void UpdatePowerTotalLabel(PowerAllocation allocation)
        {
            var total = (allocation.Weapons + allocation.Shields + allocation.Engines + allocation.Ecm) * 100f;
            var valid = ReactorPowerAllocationMath.IsValid(allocation);
            _powerTotalText.text = valid ? $"PWR {total:F0}%" : $"PWR {total:F0}% !";
            _powerTotalText.color = valid
                ? new Color(0.7f, 0.9f, 0.75f, 0.9f)
                : new Color(1f, 0.45f, 0.35f, 0.95f);
        }

        internal void OnPowerSliderChanged(int channelIndex)
        {
            if (_suppressPowerCallbacks || _powerControl == null)
            {
                return;
            }

            var adjusted = ReactorPowerAllocationMath.AdjustChannel(
                _powerControl.Current,
                (ReactorPowerAllocationMath.PowerChannel)channelIndex,
                _powerSliders[channelIndex].value);

            SyncPowerSliders(adjusted);
            _powerControl.RequestAllocation(adjusted);
        }

        internal void OnPowerPresetClicked(PowerAllocation preset)
        {
            if (_powerControl == null)
            {
                return;
            }

            SyncPowerSliders(preset);
            _powerControl.RequestAllocation(preset);
        }

        public void Apply(FlightHudDisplayState state)
        {
            _canvas.enabled = state.IsVisible;
            if (!state.IsVisible) return;

            _speedText.text = state.SpeedText;
            _headingText.text = state.HeadingText;
            _hullFill.fillAmount = Mathf.Clamp01(state.HullFill01);
            _shieldFill.fillAmount = Mathf.Clamp01(state.ShieldFill01);

            if (state.ShieldFacings != null && state.ShieldFacings.Length >= 4)
            {
                _shieldFrontFill.fillAmount = Mathf.Clamp01(state.ShieldFacings[0]);
                _shieldRearFill.fillAmount = Mathf.Clamp01(state.ShieldFacings[1]);
                _shieldPortFill.fillAmount = Mathf.Clamp01(state.ShieldFacings[2]);
                _shieldStarboardFill.fillAmount = Mathf.Clamp01(state.ShieldFacings[3]);
            }

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

            ApplyRadarBlips(state.RadarContacts, state.RadarContactIds, state.RadarContactCount, state.LockedTargetNetworkObjectId);
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
                _targetBracket.gameObject.SetActive(false);
                return;
            }

            _lockedTargetNameText.text = state.LockedTargetName;
            _lockedTargetDistanceText.text = $"{state.LockedTargetDistanceMeters:F0} m";
            _lockedTargetHullFill.fillAmount = Mathf.Clamp01(state.LockedTargetHullFill01);

            // Update Target Brackets
            var target = FindTargetable(state.LockedTargetNetworkObjectId);
            if (target != null && Camera.main != null)
            {
                var screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);
                if (screenPoint.z > 0f)
                {
                    _targetBracket.gameObject.SetActive(true);
                    var canvasRect = _canvas.GetComponent<RectTransform>();
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, _canvas.worldCamera, out var localPoint))
                    {
                        _targetBracket.anchoredPosition = localPoint;
                    }
                }
                else
                {
                    _targetBracket.gameObject.SetActive(false);
                }
            }
            else
            {
                _targetBracket.gameObject.SetActive(false);
            }
        }

        void ApplyRadarBlips(Vector3[] contacts, ulong[] ids, int count, ulong lockedId)
        {
            var flash = (Time.time % 0.4f) < 0.2f;

            for (var i = 0; i < _radarBlips.Length; i++)
            {
                var blip = _radarBlips[i];
                if (contacts == null || i >= count)
                {
                    blip.enabled = false;
                    continue;
                }

                var contact = contacts[i];
                var contactId = ids != null && i < ids.Length ? ids[i] : 0UL;
                var isLocked = lockedId != 0UL && contactId == lockedId;

                if (isLocked)
                {
                    blip.enabled = flash;
                    blip.color = new Color(1f, 0.15f, 0.15f, 0.95f); // Red flash
                    blip.rectTransform.sizeDelta = new Vector2(10f, 10f); // Larger
                }
                else
                {
                    blip.enabled = true;
                    blip.color = new Color(1f, 0.45f, 0.25f, 0.95f); // Standard
                    blip.rectTransform.sizeDelta = new Vector2(6f, 6f);
                }

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

        static Slider[] CreatePowerSliders(Transform parent, Color[] channelColors)
        {
            var sliders = new Slider[4];
            for (var i = 0; i < sliders.Length; i++)
            {
                var yPos = 200f + i * 22f;
                var sliderGo = new GameObject($"PowerSlider_{i}");
                sliderGo.transform.SetParent(parent, false);
                var rect = sliderGo.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(1f, 0f);
                rect.anchorMax = new Vector2(1f, 0f);
                rect.pivot = new Vector2(1f, 0.5f);
                rect.anchoredPosition = new Vector2(-130f, yPos + 6f);
                rect.sizeDelta = new Vector2(120f, 16f);

                var bgGo = new GameObject("Background");
                bgGo.transform.SetParent(sliderGo.transform, false);
                var bgRect = bgGo.AddComponent<RectTransform>();
                bgRect.anchorMin = Vector2.zero;
                bgRect.anchorMax = Vector2.one;
                bgRect.offsetMin = Vector2.zero;
                bgRect.offsetMax = Vector2.zero;
                var bgImg = bgGo.AddComponent<Image>();
                bgImg.color = new Color(0.08f, 0.08f, 0.1f, 0.85f);

                var fillAreaGo = new GameObject("Fill Area");
                fillAreaGo.transform.SetParent(sliderGo.transform, false);
                var fillAreaRect = fillAreaGo.AddComponent<RectTransform>();
                fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
                fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
                fillAreaRect.offsetMin = new Vector2(6f, 0f);
                fillAreaRect.offsetMax = new Vector2(-6f, 0f);

                var fillGo = new GameObject("Fill");
                fillGo.transform.SetParent(fillAreaGo.transform, false);
                var fillRect = fillGo.AddComponent<RectTransform>();
                fillRect.anchorMin = Vector2.zero;
                fillRect.anchorMax = Vector2.one;
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
                var fillImg = fillGo.AddComponent<Image>();
                fillImg.color = channelColors[i];

                var handleSlideAreaGo = new GameObject("Handle Slide Area");
                handleSlideAreaGo.transform.SetParent(sliderGo.transform, false);
                var handleSlideAreaRect = handleSlideAreaGo.AddComponent<RectTransform>();
                handleSlideAreaRect.anchorMin = Vector2.zero;
                handleSlideAreaRect.anchorMax = Vector2.one;
                handleSlideAreaRect.offsetMin = new Vector2(6f, 0f);
                handleSlideAreaRect.offsetMax = new Vector2(-6f, 0f);

                var handleGo = new GameObject("Handle");
                handleGo.transform.SetParent(handleSlideAreaGo.transform, false);
                var handleRect = handleGo.AddComponent<RectTransform>();
                handleRect.sizeDelta = new Vector2(12f, 18f);
                var handleImg = handleGo.AddComponent<Image>();
                handleImg.color = new Color(0.95f, 0.95f, 0.95f, 0.95f);

                var slider = sliderGo.AddComponent<Slider>();
                slider.targetGraphic = handleImg;
                slider.fillRect = fillRect;
                slider.handleRect = handleRect;
                slider.direction = Slider.Direction.LeftToRight;
                slider.minValue = 0f;
                slider.maxValue = 1f;
                slider.wholeNumbers = false;
                sliders[i] = slider;
            }

            return sliders;
        }

        static Button[] CreatePowerPresetButtons(Transform parent)
        {
            var presets = new[]
            {
                ("CMB", ReactorPowerAllocationMath.CombatPreset),
                ("TRV", ReactorPowerAllocationMath.TravelPreset),
                ("BAL", ReactorPowerAllocationMath.BalancedPreset)
            };

            var buttons = new Button[presets.Length];
            for (var i = 0; i < presets.Length; i++)
            {
                var xPos = -280f + i * 52f;
                var buttonGo = new GameObject($"PowerPreset_{presets[i].Item1}");
                buttonGo.transform.SetParent(parent, false);
                var rect = buttonGo.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(1f, 0f);
                rect.anchorMax = new Vector2(1f, 0f);
                rect.pivot = new Vector2(1f, 0f);
                rect.anchoredPosition = new Vector2(xPos, 92f);
                rect.sizeDelta = new Vector2(48f, 22f);

                var image = buttonGo.AddComponent<Image>();
                image.color = new Color(0.12f, 0.16f, 0.22f, 0.95f);

                var button = buttonGo.AddComponent<Button>();
                button.targetGraphic = image;

                var label = CreateLabel(buttonGo.transform, "Label",
                    Vector2.zero, Vector2.one, Vector2.zero, 11, TextAnchor.MiddleCenter);
                label.text = presets[i].Item1;
                label.color = new Color(0.85f, 0.95f, 0.9f, 0.95f);
                buttons[i] = button;
            }

            return buttons;
        }

        internal void WirePowerSliderCallbacks(Action<int> onSliderChanged, Action<PowerAllocation> onPresetClicked)
        {
            for (var i = 0; i < _powerSliders.Length; i++)
            {
                var channelIndex = i;
                _powerSliders[i].onValueChanged.RemoveAllListeners();
                _powerSliders[i].onValueChanged.AddListener(_ => onSliderChanged(channelIndex));
            }

            var presets = new[]
            {
                ReactorPowerAllocationMath.CombatPreset,
                ReactorPowerAllocationMath.TravelPreset,
                ReactorPowerAllocationMath.BalancedPreset
            };

            for (var i = 0; i < _powerPresetButtons.Length; i++)
            {
                var preset = presets[i];
                _powerPresetButtons[i].onClick.RemoveAllListeners();
                _powerPresetButtons[i].onClick.AddListener(() => onPresetClicked(preset));
            }
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

        static RectTransform CreateTargetBracket(Transform parent)
        {
            var go = new GameObject("TargetBracket");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(60f, 60f);

            Color bracketColor = new Color(1f, 0.15f, 0.15f, 0.9f);

            // Create four corner brackets
            CreateLine(go.transform, "TL_H", new Vector2(-25f, 30f), new Vector2(10f, 2f), bracketColor);
            CreateLine(go.transform, "TL_V", new Vector2(-30f, 25f), new Vector2(2f, 10f), bracketColor);

            CreateLine(go.transform, "TR_H", new Vector2(25f, 30f), new Vector2(10f, 2f), bracketColor);
            CreateLine(go.transform, "TR_V", new Vector2(30f, 25f), new Vector2(2f, 10f), bracketColor);

            CreateLine(go.transform, "BL_H", new Vector2(-25f, -30f), new Vector2(10f, 2f), bracketColor);
            CreateLine(go.transform, "BL_V", new Vector2(-30f, -25f), new Vector2(2f, 10f), bracketColor);

            CreateLine(go.transform, "BR_H", new Vector2(25f, -30f), new Vector2(10f, 2f), bracketColor);
            CreateLine(go.transform, "BR_V", new Vector2(30f, -25f), new Vector2(2f, 10f), bracketColor);

            go.SetActive(false);
            return rect;
        }

        static void CreateLine(Transform parent, string name, Vector2 localPos, Vector2 size, Color color)
        {
            var lineGo = new GameObject(name);
            lineGo.transform.SetParent(parent, false);
            var rect = lineGo.AddComponent<RectTransform>();
            rect.anchoredPosition = localPos;
            rect.sizeDelta = size;
            var img = lineGo.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;
        }

        TargetableEntity FindTargetable(ulong networkObjectId)
        {
            if (networkObjectId == 0UL) return null;
            var all = UnityEngine.Object.FindObjectsByType<TargetableEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < all.Length; i++)
            {
                var entity = all[i];
                if (entity != null && entity.GetNetworkObjectId() == networkObjectId)
                {
                    return entity;
                }
            }
            return null;
        }
    }
}
