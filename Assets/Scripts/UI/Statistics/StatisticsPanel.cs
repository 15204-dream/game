using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 统计面板基类 - 所有统计面板的基类
    /// </summary>
    public class StatisticsPanel : MonoBehaviour
    {
        [Header("面板基础配置")]
        [SerializeField] protected bool autoUpdate = true;
        [SerializeField] protected float updateInterval = 0.5f;
        [SerializeField] protected bool showAnimation = true;
        [SerializeField] protected AnimationCurve showCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("面板样式")]
        [SerializeField] protected Color panelBackgroundColor = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        [SerializeField] protected Color headerColor = new Color(0.2f, 0.3f, 0.5f, 1f);
        [SerializeField] protected Color borderColor = new Color(0.3f, 0.5f, 0.8f, 1f);
        [SerializeField] protected float borderWidth = 2f;
        [SerializeField] protected float cornerRadius = 10f;
        
        [Header("动画配置")]
        [SerializeField] protected bool enableFadeAnimation = true;
        [SerializeField] protected float fadeDuration = 0.3f;
        [SerializeField] protected bool enableSlideAnimation = true;
        [SerializeField] protected float slideDistance = 50f;
        
        protected RectTransform rectTransform;
        protected CanvasGroup canvasGroup;
        protected bool isVisible;
        protected bool isAnimating;
        protected float animationProgress;
        protected float lastUpdateTime;
        protected Vector2 showPosition;
        protected Vector2 hidePosition;
        
        public bool IsVisible => isVisible;
        
        public event Action OnPanelShow;
        public event Action OnPanelHide;
        public event Action OnPanelUpdate;
        
        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            CalculatePositions();
        }
        
        protected virtual void Start()
        {
            Initialize();
        }
        
        protected virtual void Update()
        {
            if (autoUpdate && isVisible && !isAnimating)
            {
                if (Time.time - lastUpdateTime >= updateInterval)
                {
                    UpdatePanel();
                    lastUpdateTime = Time.time;
                }
            }
            
            if (isAnimating)
            {
                UpdateAnimation();
            }
        }
        
        /// <summary>
        /// 初始化面板
        /// </summary>
        public virtual void Initialize()
        {
            if (enableFadeAnimation)
            {
                canvasGroup.alpha = 0f;
            }
            
            if (enableSlideAnimation)
            {
                rectTransform.anchoredPosition = hidePosition;
            }
            
            isVisible = false;
            isAnimating = false;
        }
        
        /// <summary>
        /// 计算显示和隐藏位置
        /// </summary>
        protected virtual void CalculatePositions()
        {
            showPosition = rectTransform.anchoredPosition;
            hidePosition = showPosition + new Vector2(0, slideDistance);
        }
        
        /// <summary>
        /// 显示面板
        /// </summary>
        public virtual void Show()
        {
            if (isVisible && !isAnimating)
                return;
            
            isVisible = true;
            isAnimating = true;
            animationProgress = 0f;
            
            OnPanelShow?.Invoke();
            
            if (!enableSlideAnimation && !enableFadeAnimation)
            {
                CompleteAnimation();
            }
        }
        
        /// <summary>
        /// 隐藏面板
        /// </summary>
        public virtual void Hide()
        {
            if (!isVisible && !isAnimating)
                return;
            
            isAnimating = true;
            animationProgress = 0f;
            
            if (!enableSlideAnimation && !enableFadeAnimation)
            {
                CompleteHideAnimation();
            }
        }
        
        /// <summary>
        /// 切换面板显示状态
        /// </summary>
        public virtual void Toggle()
        {
            if (isVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
        
        /// <summary>
        /// 更新面板内容
        /// </summary>
        protected virtual void UpdatePanel()
        {
            OnPanelUpdate?.Invoke();
        }
        
        /// <summary>
        /// 更新动画
        /// </summary>
        protected virtual void UpdateAnimation()
        {
            animationProgress += Time.deltaTime / fadeDuration;
            animationProgress = Mathf.Clamp01(animationProgress);
            
            float curveValue = showCurve.Evaluate(animationProgress);
            
            if (enableFadeAnimation)
            {
                float targetAlpha = isVisible ? 1f : 0f;
                canvasGroup.alpha = Mathf.Lerp(1f - targetAlpha, targetAlpha, curveValue);
            }
            
            if (enableSlideAnimation)
            {
                Vector2 targetPos = isVisible ? showPosition : hidePosition;
                rectTransform.anchoredPosition = Vector2.Lerp(showPosition, hidePosition, curveValue);
            }
            
            if (animationProgress >= 1f)
            {
                CompleteAnimation();
            }
        }
        
        /// <summary>
        /// 完成显示动画
        /// </summary>
        protected virtual void CompleteAnimation()
        {
            isAnimating = false;
            isVisible = true;
            
            if (enableFadeAnimation)
            {
                canvasGroup.alpha = 1f;
            }
            
            if (enableSlideAnimation)
            {
                rectTransform.anchoredPosition = showPosition;
            }
            
            UpdatePanel();
        }
        
        /// <summary>
        /// 完成隐藏动画
        /// </summary>
        protected virtual void CompleteHideAnimation()
        {
            isAnimating = false;
            isVisible = false;
            
            if (enableFadeAnimation)
            {
                canvasGroup.alpha = 0f;
            }
            
            if (enableSlideAnimation)
            {
                rectTransform.anchoredPosition = hidePosition;
            }
            
            OnPanelHide?.Invoke();
        }
        
        /// <summary>
        /// 刷新面板
        /// </summary>
        public virtual void Refresh()
        {
            UpdatePanel();
        }
        
        /// <summary>
        /// 设置自动更新
        /// </summary>
        public void SetAutoUpdate(bool enabled)
        {
            autoUpdate = enabled;
        }
        
        /// <summary>
        /// 设置更新间隔
        /// </summary>
        public void SetUpdateInterval(float interval)
        {
            updateInterval = Mathf.Max(0.1f, interval);
        }
    }
    
    /// <summary>
    /// 面板配置
    /// </summary>
    [Serializable]
    public class PanelConfig
    {
        public string panelId;
        public string panelName;
        public bool isVisible;
        public int layer;
        public Vector2 position;
        public Vector2 size;
        public PanelAnimationType animationType;
        public float animationDuration;
        
        public enum PanelAnimationType
        {
            None,
            Fade,
            Slide,
            Scale,
            FadeSlide
        }
    }
    
    /// <summary>
    /// 面板样式配置
    /// </summary>
    [Serializable]
    public class PanelStyle
    {
        public Color backgroundColor;
        public Color borderColor;
        public Color headerColor;
        public Color textColor;
        public float borderWidth;
        public float cornerRadius;
        public Font font;
        public float fontSize;
        
        public static PanelStyle CreateDefault()
        {
            return new PanelStyle
            {
                backgroundColor = new Color(0.1f, 0.1f, 0.15f, 0.95f),
                borderColor = new Color(0.3f, 0.5f, 0.8f, 1f),
                headerColor = new Color(0.2f, 0.3f, 0.5f, 1f),
                textColor = Color.white,
                borderWidth = 2f,
                cornerRadius = 10f,
                fontSize = 14f
            };
        }
    }
}
