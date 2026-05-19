using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LoveBeatGuide
{
    /// <summary>
    /// 提示类型
    /// </summary>
    public enum HintType
    {
        None,
        Text,
        Icon,
        Animated,
        SpeechBubble
    }

    /// <summary>
    /// 提示信息
    /// </summary>
    [Serializable]
    public class HintInfo
    {
        public string hintId;
        public HintType hintType;
        public string text;
        public Sprite icon;
        public Vector2 position;
        public float displayTime;
        public bool showArrow;
        public GuideArrowType arrowType;
        public float arrowOffset;
        public Color backgroundColor;
        public Action onHintShown;
        public Action onHintHidden;
    }

    /// <summary>
    /// 提示系统
    /// </summary>
    public class GuideHintSystem : MonoBehaviour
    {
        private static GuideHintSystem instance;
        public static GuideHintSystem Instance => instance;

        [Header("UI引用")]
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private Image hintIcon;
        [SerializeField] private Image hintBackground;
        [SerializeField] private GameObject arrowObject;
        [SerializeField] private GuideArrow guideArrow;

        [Header("配置")]
        [SerializeField] private float defaultDisplayTime = 3f;
        [SerializeField] private float fadeSpeed = 0.3f;
        [SerializeField] private Vector2 defaultPosition = new Vector2(0.5f, 0.8f);
        [SerializeField] private Color defaultBackgroundColor = new Color(1f, 0.95f, 0.9f, 0.95f);

        [Header("预设提示")]
        [SerializeField] private List<HintInfo> presetHints = new List<HintInfo>();

        private Dictionary<string, HintInfo> hints = new Dictionary<string, HintInfo>();
        private HintInfo currentHint;
        private bool isHintVisible;
        private float timer;
        private Transform targetTransform;

        public bool IsHintVisible => isHintVisible;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            InitializeHints();
        }

        private void Start()
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }

            if (arrowObject != null)
            {
                arrowObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (!isHintVisible)
                return;

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                HideCurrentHint();
            }

            if (targetTransform != null)
            {
                UpdateHintPosition();
            }
        }

        /// <summary>
        /// 初始化提示
        /// </summary>
        private void InitializeHints()
        {
            hints.Clear();

            foreach (var hint in presetHints)
            {
                if (!string.IsNullOrEmpty(hint.hintId))
                {
                    hints[hint.hintId] = hint;
                }
            }
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        public void ShowHint(string hintId)
        {
            if (hints.ContainsKey(hintId))
            {
                ShowHint(hints[hintId]);
            }
            else
            {
                Debug.LogWarning($"未找到预设提示: {hintId}");
            }
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        public void ShowHint(string hintId, string customText)
        {
            HintInfo hint;

            if (hints.ContainsKey(hintId))
            {
                hint = hints[hintId];
                hint.text = customText;
            }
            else
            {
                hint = new HintInfo
                {
                    hintId = hintId,
                    hintType = HintType.Text,
                    text = customText,
                    displayTime = defaultDisplayTime
                };
            }

            ShowHint(hint);
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        public void ShowHint(string text, Vector2 position, float displayTime = 0)
        {
            HintInfo hint = new HintInfo
            {
                hintId = Guid.NewGuid().ToString(),
                hintType = HintType.Text,
                text = text,
                position = position,
                displayTime = displayTime > 0 ? displayTime : defaultDisplayTime,
                backgroundColor = defaultBackgroundColor
            };

            ShowHint(hint);
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        private void ShowHint(HintInfo hint)
        {
            if (hint == null || string.IsNullOrEmpty(hint.text))
                return;

            if (isHintVisible)
            {
                HideCurrentHint();
            }

            currentHint = hint;
            isHintVisible = true;

            if (hintPanel != null)
            {
                hintPanel.SetActive(true);
                UpdateHintUI(hint);
            }

            if (hint.showArrow && arrowObject != null)
            {
                arrowObject.SetActive(true);
                if (guideArrow != null)
                {
                    guideArrow.SetArrowType(hint.arrowType);
                }
            }

            timer = hint.displayTime > 0 ? hint.displayTime : defaultDisplayTime;

            hint.onHintShown?.Invoke();
        }

        /// <summary>
        /// 更新提示UI
        /// </summary>
        private void UpdateHintUI(HintInfo hint)
        {
            if (hintText != null)
            {
                hintText.text = hint.text;
            }

            if (hintIcon != null)
            {
                if (hint.icon != null)
                {
                    hintIcon.sprite = hint.icon;
                    hintIcon.gameObject.SetActive(true);
                }
                else
                {
                    hintIcon.gameObject.SetActive(false);
                }
            }

            if (hintBackground != null)
            {
                hintBackground.color = hint.backgroundColor;
            }

            RectTransform rectTransform = hintPanel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = hint.position;
                rectTransform.anchorMax = hint.position;
                rectTransform.pivot = hint.position;
            }
        }

        /// <summary>
        /// 更新提示位置
        /// </summary>
        private void UpdateHintPosition()
        {
            if (currentHint == null || hintPanel == null)
                return;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetTransform.position);
            RectTransform rectTransform = hintPanel.GetComponent<RectTransform>();
            
            if (rectTransform != null)
            {
                Vector2 localPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform.parent as RectTransform,
                    screenPos,
                    null,
                    out localPos
                );

                rectTransform.localPosition = localPos;
            }
        }

        /// <summary>
        /// 设置提示目标
        /// </summary>
        public void SetHintTarget(Transform target)
        {
            targetTransform = target;

            if (target != null && currentHint != null)
            {
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);
                currentHint.position = screenPos / new Vector2(Screen.width, Screen.height);
            }
        }

        /// <summary>
        /// 隐藏当前提示
        /// </summary>
        public void HideCurrentHint()
        {
            if (!isHintVisible)
                return;

            currentHint?.onHintHidden?.Invoke();

            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }

            if (arrowObject != null)
            {
                arrowObject.SetActive(false);
            }

            isHintVisible = false;
            currentHint = null;
            targetTransform = null;
        }

        /// <summary>
        /// 隐藏所有提示
        /// </summary>
        public void HideAllHints()
        {
            HideCurrentHint();
        }

        /// <summary>
        /// 注册预设提示
        /// </summary>
        public void RegisterPresetHint(HintInfo hint)
        {
            if (!string.IsNullOrEmpty(hint.hintId))
            {
                hints[hint.hintId] = hint;
            }
        }

        /// <summary>
        /// 移除预设提示
        /// </summary>
        public void RemovePresetHint(string hintId)
        {
            if (hints.ContainsKey(hintId))
            {
                hints.Remove(hintId);
            }
        }

        /// <summary>
        /// 显示瞄准提示
        /// </summary>
        public void ShowTargetingHint(string text, Transform target)
        {
            HintInfo hint = new HintInfo
            {
                hintId = "targeting",
                hintType = HintType.Animated,
                text = text,
                showArrow = true,
                arrowType = GuideArrowType.Pulse,
                displayTime = 0,
                backgroundColor = defaultBackgroundColor
            };

            ShowHint(hint);
            SetHintTarget(target);
        }

        /// <summary>
        /// 显示点击提示
        /// </summary>
        public void ShowClickHint(string text, Transform target)
        {
            HintInfo hint = new HintInfo
            {
                hintId = "click",
                hintType = HintType.SpeechBubble,
                text = text,
                showArrow = true,
                arrowType = GuideArrowType.Down,
                displayTime = 0,
                backgroundColor = defaultBackgroundColor
            };

            ShowHint(hint);
            SetHintTarget(target);
        }

        /// <summary>
        /// 延长显示时间
        /// </summary>
        public void ExtendDisplayTime(float additionalTime)
        {
            if (isHintVisible)
            {
                timer += additionalTime;
            }
        }

        /// <summary>
        /// 获取当前提示信息
        /// </summary>
        public HintInfo GetCurrentHint()
        {
            return currentHint;
        }
    }
}
