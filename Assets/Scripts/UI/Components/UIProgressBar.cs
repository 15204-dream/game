using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 进度条组件 - 心动值/好感度/时间条等
/// </summary>
public class UIProgressBar : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Image iconImage;
    
    [Header("进度条配置")]
    [Range(0f, 1f)]
    [SerializeField] private float fillAmount = 0f;
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 100f;
    [SerializeField] private bool showPercentage = true;
    [SerializeField] private bool showValue = false;
    
    [Header("动画配置")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("视觉配置")]
    [SerializeField] private ProgressBarType barType = ProgressBarType.Normal;
    [SerializeField] private Color fillColor = new Color(1f, 0.6f, 0.7f, 1f);
    [SerializeField] private Color backgroundColor = new Color(1f, 0.85f, 0.9f, 0.5f);
    [SerializeField] private Color glowColor = new Color(1f, 0.4f, 0.5f, 0.5f);
    [SerializeField] private bool enableGlow = true;
    [SerializeField] private bool enablePulse = false;
    
    [Header("阶段颜色")]
    [SerializeField] private Color lowColor = new Color(0.8f, 0.4f, 0.4f, 1f);
    [SerializeField] private Color mediumColor = new Color(1f, 0.8f, 0.4f, 1f);
    [SerializeField] private Color highColor = new Color(0.6f, 1f, 0.6f, 1f);
    [SerializeField] private Color maxColor = new Color(1f, 0.4f, 0.6f, 1f);
    
    // 私有变量
    private float targetFillAmount = 0f;
    private float currentFillAmount = 0f;
    private Coroutine animationCoroutine;
    private float pulseTimer = 0f;
    
    // 属性
    public float FillAmount
    {
        get { return fillAmount; }
        set
        {
            targetFillAmount = Mathf.Clamp01(value);
            if (!useAnimation)
            {
                fillAmount = targetFillAmount;
                UpdateFill();
            }
            else if (animationCoroutine == null)
            {
                animationCoroutine = StartCoroutine(AnimateFill());
            }
        }
    }
    
    public float Value
    {
        get { return Mathf.Lerp(minValue, maxValue, fillAmount); }
        set
        {
            float normalizedValue = (value - minValue) / (maxValue - minValue);
            FillAmount = normalizedValue;
        }
    }
    
    public float NormalizedValue
    {
        get { return fillAmount; }
        set { FillAmount = value; }
    }
    
    // 进度条类型枚举
    public enum ProgressBarType
    {
        Normal,         // 普通进度条
        Heart,          // 心动值条
        Affection,      // 好感度条
        Time,           // 时间条
        Energy,         // 体力条
        Experience      // 经验条
    }
    
    private void Awake()
    {
        // 获取组件引用
        if (fillImage == null)
        {
            Transform fill = transform.Find("Fill");
            if (fill != null)
            {
                fillImage = fill.GetComponent<Image>();
            }
        }
        
        if (backgroundImage == null)
        {
            Transform bg = transform.Find("Background");
            if (bg != null)
            {
                backgroundImage = bg.GetComponent<Image>();
            }
            else
            {
                backgroundImage = GetComponent<Image>();
            }
        }
        
        // 初始化显示
        UpdateFill();
    }
    
    private void Update()
    {
        // 脉冲动画
        if (enablePulse && fillAmount > 0f)
        {
            pulseTimer += Time.deltaTime * 2f;
            float pulse = Mathf.Sin(pulseTimer) * 0.1f + 1f;
            if (fillImage != null)
            {
                fillImage.transform.localScale = new Vector3(pulse, 1f, 1f);
            }
        }
    }
    
    /// <summary>
    /// 设置进度条数值
    /// </summary>
    public void SetFill(float value, bool instant = false)
    {
        targetFillAmount = Mathf.Clamp01(value);
        
        if (instant || !useAnimation)
        {
            fillAmount = targetFillAmount;
            currentFillAmount = targetFillAmount;
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
            UpdateFill();
        }
        else if (animationCoroutine == null)
        {
            animationCoroutine = StartCoroutine(AnimateFill());
        }
    }
    
    /// <summary>
    /// 设置实际数值
    /// </summary>
    public void SetValue(float value, bool instant = false)
    {
        float normalized = (value - minValue) / (maxValue - minValue);
        SetFill(normalized, instant);
    }
    
    /// <summary>
    /// 增加数值
    /// </summary>
    public void AddValue(float amount)
    {
        Value = Value + amount;
    }
    
    /// <summary>
    /// 减少数值
    /// </summary>
    public void SubtractValue(float amount)
    {
        Value = Value - amount;
    }
    
    /// <summary>
    /// 设置标签文本
    /// </summary>
    public void SetLabel(string label)
    {
        if (labelText != null)
        {
            labelText.text = label;
        }
    }
    
    /// <summary>
    /// 设置进度条类型
    /// </summary>
    public void SetBarType(ProgressBarType type)
    {
        barType = type;
        UpdateVisualStyle();
    }
    
    /// <summary>
    /// 设置进度条颜色
    /// </summary>
    public void SetFillColor(Color color)
    {
        fillColor = color;
        UpdateFillColor();
    }
    
    /// <summary>
    /// 启用或禁用发光效果
    /// </summary>
    public void SetGlowEnabled(bool enabled)
    {
        enableGlow = enabled;
        UpdateFillColor();
    }
    
    /// <summary>
    /// 启用或禁用脉冲动画
    /// </summary>
    public void SetPulseEnabled(bool enabled)
    {
        enablePulse = enabled;
        if (!enabled && fillImage != null)
        {
            fillImage.transform.localScale = Vector3.one;
        }
    }
    
    private void UpdateFill()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = fillAmount;
        }
        
        UpdateFillColor();
        UpdateText();
    }
    
    private void UpdateFillColor()
    {
        if (fillImage == null) return;
        
        // 根据当前阶段选择颜色
        Color targetColor = fillColor;
        
        if (barType == ProgressBarType.Affection || barType == ProgressBarType.Heart)
        {
            if (fillAmount < 0.25f)
            {
                targetColor = lowColor;
            }
            else if (fillAmount < 0.5f)
            {
                targetColor = mediumColor;
            }
            else if (fillAmount < 0.75f)
            {
                targetColor = highColor;
            }
            else
            {
                targetColor = maxColor;
            }
        }
        
        fillImage.color = targetColor;
        
        // 发光效果
        if (enableGlow)
        {
            Color glow = targetColor * glowColor.a;
            // 可以通过MaterialPropertyBlock来设置发光效果
        }
    }
    
    private void UpdateText()
    {
        if (valueText == null) return;
        
        if (showPercentage)
        {
            valueText.text = Mathf.RoundToInt(fillAmount * 100) + "%";
        }
        else if (showValue)
        {
            float currentValue = Mathf.Lerp(minValue, maxValue, fillAmount);
            valueText.text = Mathf.RoundToInt(currentValue).ToString();
        }
    }
    
    private void UpdateVisualStyle()
    {
        switch (barType)
        {
            case ProgressBarType.Heart:
                fillColor = new Color(1f, 0.4f, 0.5f, 1f);
                enablePulse = true;
                break;
            case ProgressBarType.Affection:
                fillColor = new Color(1f, 0.6f, 0.7f, 1f);
                enableGlow = true;
                break;
            case ProgressBarType.Time:
                fillColor = new Color(0.6f, 0.8f, 1f, 1f);
                break;
            case ProgressBarType.Energy:
                fillColor = new Color(1f, 0.9f, 0.4f, 1f);
                break;
            case ProgressBarType.Experience:
                fillColor = new Color(0.6f, 1f, 0.8f, 1f);
                break;
        }
    }
    
    private System.Collections.IEnumerator AnimateFill()
    {
        float startFill = currentFillAmount;
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curveValue = animationCurve.Evaluate(t);
            
            currentFillAmount = Mathf.Lerp(startFill, targetFillAmount, curveValue);
            fillAmount = currentFillAmount;
            UpdateFill();
            
            yield return null;
        }
        
        currentFillAmount = targetFillAmount;
        fillAmount = targetFillAmount;
        UpdateFill();
        
        animationCoroutine = null;
    }
    
    /// <summary>
    /// 震动效果
    /// </summary>
    public void Shake()
    {
        StartCoroutine(ShakeAnimation());
    }
    
    private System.Collections.IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.localPosition;
        float shakeDuration = 0.3f;
        float shakeIntensity = 5f;
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * shakeIntensity * (1f - elapsed / shakeDuration);
            float y = Random.Range(-1f, 1f) * shakeIntensity * (1f - elapsed / shakeDuration);
            transform.localPosition = originalPos + new Vector3(x, y, 0f);
            yield return null;
        }
        
        transform.localPosition = originalPos;
    }
}
