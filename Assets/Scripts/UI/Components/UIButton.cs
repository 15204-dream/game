using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// 自定义按钮组件 - 恋综风格按钮
/// 支持普通、心动（悬停）、按下三种状态
/// </summary>
public class UIButton : Selectable
{
    [Header("按钮配置")]
    [SerializeField] private ButtonClickedEvent onClick = new ButtonClickedEvent();
    [SerializeField] private bool enableLongPress = false;
    [SerializeField] private float longPressThreshold = 0.5f;
    
    [Header("视觉配置")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text buttonText;
    
    [Header("状态颜色")]
    [SerializeField] private Color normalColor = new Color(1f, 0.8f, 0.85f, 1f);
    [SerializeField] private Color highlightColor = new Color(1f, 0.6f, 0.7f, 1f);
    [SerializeField] private Color pressedColor = new Color(0.9f, 0.5f, 0.6f, 1f);
    [SerializeField] private Color disabledColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
    
    [Header("动画配置")]
    [SerializeField] private bool enableScaleAnimation = true;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float animationDuration = 0.2f;
    
    [Header("特效配置")]
    [SerializeField] private bool enableHeartParticles = false;
    [SerializeField] private GameObject heartParticlePrefab;
    [SerializeField] private Transform particleSpawnPoint;
    
    // 私有变量
    private Vector3 originalScale;
    private bool isLongPressing = false;
    private float pressStartTime = 0f;
    private Coroutine animationCoroutine;
    
    // 事件定义
    public ButtonClickedEvent OnClick
    {
        get { return onClick; }
        set { onClick = value; }
    }
    
    // 按钮点击事件
    [System.Serializable]
    public class ButtonClickedEvent : UnityEvent { }
    
    protected override void Awake()
    {
        base.Awake();
        originalScale = transform.localScale;
        
        // 初始化背景图
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateVisualState(true);
    }
    
    /// <summary>
    /// 设置按钮文本
    /// </summary>
    public void SetText(string text)
    {
        if (buttonText != null)
        {
            buttonText.text = text;
        }
    }
    
    /// <summary>
    /// 获取按钮文本
    /// </summary>
    public string GetText()
    {
        return buttonText != null ? buttonText.text : "";
    }
    
    /// <summary>
    /// 设置按钮图标
    /// </summary>
    public void SetIcon(Sprite icon)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(icon != null);
        }
    }
    
    /// <summary>
    /// 设置按钮颜色
    /// </summary>
    public void SetButtonColor(Color color)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
    }
    
    /// <summary>
    /// 模拟点击按钮
    /// </summary>
    public void SimulateClick()
    {
        if (!IsInteractable()) return;
        
        onClick.Invoke();
        PlayClickEffect();
    }
    
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        UpdateVisualState(instant);
    }
    
    private void UpdateVisualState(bool instant)
    {
        Color targetColor;
        float targetScale = 1f;
        
        switch (currentSelectionState)
        {
            case SelectionState.Normal:
                targetColor = normalColor;
                targetScale = 1f;
                break;
            case SelectionState.Highlighted:
                targetColor = highlightColor;
                targetScale = enableScaleAnimation ? hoverScale : 1f;
                break;
            case SelectionState.Pressed:
                targetColor = pressedColor;
                targetScale = enableScaleAnimation ? pressScale : 1f;
                break;
            case SelectionState.Disabled:
                targetColor = disabledColor;
                targetScale = 1f;
                break;
            default:
                targetColor = normalColor;
                targetScale = 1f;
                break;
        }
        
        if (instant)
        {
            ApplyVisualState(targetColor, targetScale);
        }
        else
        {
            AnimateToState(targetColor, targetScale);
        }
    }
    
    private void ApplyVisualState(Color color, float scale)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
        transform.localScale = originalScale * scale;
    }
    
    private void AnimateToState(Color targetColor, float targetScale)
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateTransition(targetColor, originalScale * targetScale));
    }
    
    private IEnumerator AnimateTransition(Color targetColor, Vector3 targetScale)
    {
        float elapsed = 0f;
        Color startColor = backgroundImage != null ? backgroundImage.color : Color.white;
        Vector3 startScale = transform.localScale;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            if (backgroundImage != null)
            {
                backgroundImage.color = Color.Lerp(startColor, targetColor, smoothT);
            }
            transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);
            
            yield return null;
        }
        
        if (backgroundImage != null)
        {
            backgroundImage.color = targetColor;
        }
        transform.localScale = targetScale;
    }
    
    private void PlayClickEffect()
    {
        // 心形粒子效果
        if (enableHeartParticles && heartParticlePrefab != null)
        {
            Transform spawnPoint = particleSpawnPoint != null ? particleSpawnPoint : transform;
            GameObject particles = Instantiate(heartParticlePrefab, spawnPoint.position, Quaternion.identity, transform);
            Destroy(particles, 2f);
        }
        
        // 音效可以在这里触发
        // AudioManager.Instance.PlaySound("ButtonClick");
    }
    
    // 长按功能
    private void Update()
    {
        if (!enableLongPress || !IsInteractable()) return;
        
        if (IsPressed() && !isLongPressing)
        {
            isLongPressing = true;
            pressStartTime = Time.time;
        }
        else if (!IsPressed() && isLongPressing)
        {
            isLongPressing = false;
        }
        
        if (isLongPressing && IsPressed())
        {
            if (Time.time - pressStartTime >= longPressThreshold)
            {
                // 长按触发事件
                isLongPressing = false;
            }
        }
    }
}
