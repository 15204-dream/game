using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 模式指示器 - 显示当前游戏模式（嘉宾模式/导演模式）
/// </summary>
public class ModeIndicator : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private Image glowEffect;
    
    [Header("模式配置")]
    [SerializeField] private Sprite guestModeIcon;
    [SerializeField] private Sprite directorModeIcon;
    
    [Header("嘉宾模式颜色")]
    [SerializeField] private Color guestModeColor = new Color(1f, 0.7f, 0.8f, 1f);
    [SerializeField] private Color guestGlowColor = new Color(1f, 0.4f, 0.5f, 0.5f);
    
    [Header("导演模式颜色")]
    [SerializeField] private Color directorModeColor = new Color(0.7f, 0.8f, 1f, 1f);
    [SerializeField] private Color directorGlowColor = new Color(0.4f, 0.5f, 1f, 0.5f);
    
    [Header("动画配置")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private float pulseIntensity = 0.2f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    // 当前模式
    private MainMenuScene.GameMode currentMode = MainMenuScene.GameMode.Guest;
    
    // 动画状态
    private bool isAnimating = false;
    private float pulseTimer = 0f;
    
    /// <summary>
    /// 初始化模式指示器
    /// </summary>
    public void Initialize(MainMenuScene.GameMode mode)
    {
        SetMode(mode, false);
    }
    
    /// <summary>
    /// 设置游戏模式
    /// </summary>
    public void SetMode(MainMenuScene.GameMode mode)
    {
        SetMode(mode, true);
    }
    
    /// <summary>
    /// 设置游戏模式（带动画）
    /// </summary>
    public void SetMode(MainMenuScene.GameMode mode, bool animate)
    {
        if (currentMode == mode && !animate) return;
        
        currentMode = mode;
        
        if (animate && useAnimation)
        {
            StartCoroutine(TransitionToMode(mode));
        }
        else
        {
            ApplyModeVisual(mode);
        }
    }
    
    /// <summary>
    /// 应用模式视觉
    /// </summary>
    private void ApplyModeVisual(MainMenuScene.GameMode mode)
    {
        switch (mode)
        {
            case MainMenuScene.GameMode.Guest:
                ApplyGuestModeVisual();
                break;
            case MainMenuScene.GameMode.Director:
                ApplyDirectorModeVisual();
                break;
        }
    }
    
    /// <summary>
    /// 应用嘉宾模式视觉
    /// </summary>
    private void ApplyGuestModeVisual()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = guestModeColor;
        }
        
        if (iconImage != null && guestModeIcon != null)
        {
            iconImage.sprite = guestModeIcon;
        }
        
        if (modeText != null)
        {
            modeText.text = "嘉宾模式";
            modeText.color = Color.white;
        }
        
        if (glowEffect != null)
        {
            glowEffect.color = guestGlowColor;
        }
    }
    
    /// <summary>
    /// 应用导演模式视觉
    /// </summary>
    private void ApplyDirectorModeVisual()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = directorModeColor;
        }
        
        if (iconImage != null && directorModeIcon != null)
        {
            iconImage.sprite = directorModeIcon;
        }
        
        if (modeText != null)
        {
            modeText.text = "导演模式";
            modeText.color = Color.white;
        }
        
        if (glowEffect != null)
        {
            glowEffect.color = directorGlowColor;
        }
    }
    
    /// <summary>
    /// 模式切换过渡动画
    /// </summary>
    private System.Collections.IEnumerator TransitionToMode(MainMenuScene.GameMode mode)
    {
        if (isAnimating) yield break;
        isAnimating = true;
        
        float elapsed = 0f;
        float duration = 0.3f;
        
        // 缩小并淡出
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            float curveValue = transitionCurve.Evaluate(t);
            
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, curveValue);
            
            if (backgroundImage != null)
            {
                Color color = backgroundImage.color;
                color.a = 1f - curveValue;
                backgroundImage.color = color;
            }
            
            yield return null;
        }
        
        // 切换视觉
        ApplyModeVisual(mode);
        
        // 放大并淡入
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            float curveValue = transitionCurve.Evaluate(t);
            
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, curveValue);
            
            if (backgroundImage != null)
            {
                Color color = backgroundImage.color;
                color.a = curveValue;
                backgroundImage.color = color;
            }
            
            yield return null;
        }
        
        transform.localScale = Vector3.one;
        if (backgroundImage != null)
        {
            Color color = backgroundImage.color;
            color.a = 1f;
            backgroundImage.color = color;
        }
        
        isAnimating = false;
    }
    
    private void Update()
    {
        if (!useAnimation || isAnimating) return;
        
        // 脉冲发光效果
        if (glowEffect != null)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float pulse = Mathf.Sin(pulseTimer) * 0.5f + 0.5f;
            float alpha = pulse * pulseIntensity;
            
            Color glowColor = currentMode == MainMenuScene.GameMode.Guest ? 
                guestGlowColor : directorGlowColor;
            glowColor.a = alpha;
            glowEffect.color = glowColor;
        }
    }
    
    /// <summary>
    /// 获取当前模式
    /// </summary>
    public MainMenuScene.GameMode GetCurrentMode()
    {
        return currentMode;
    }
    
    /// <summary>
    /// 设置脉冲动画速度
    /// </summary>
    public void SetPulseSpeed(float speed)
    {
        pulseSpeed = speed;
    }
    
    /// <summary>
    /// 启用/禁用脉冲效果
    /// </summary>
    public void SetPulseEnabled(bool enabled)
    {
        useAnimation = enabled;
        if (!enabled && glowEffect != null)
        {
            glowEffect.color = Color.clear;
        }
    }
    
    /// <summary>
    /// 闪烁效果
    /// </summary>
    public void Flash()
    {
        StartCoroutine(FlashAnimation());
    }
    
    private System.Collections.IEnumerator FlashAnimation()
    {
        if (backgroundImage == null) yield break;
        
        Color originalColor = backgroundImage.color;
        float elapsed = 0f;
        float duration = 0.2f;
        
        // 变亮
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            
            backgroundImage.color = Color.Lerp(originalColor, Color.white, t);
            yield return null;
        }
        
        // 恢复
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            
            backgroundImage.color = Color.Lerp(Color.white, originalColor, t);
            yield return null;
        }
        
        backgroundImage.color = originalColor;
    }
}
