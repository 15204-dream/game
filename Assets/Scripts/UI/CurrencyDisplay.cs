using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 货币显示组件 - 显示游戏内货币
/// </summary>
public class CurrencyDisplay : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private Image currencyIcon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI changeText;
    [SerializeField] private Image backgroundImage;
    
    [Header("货币图标配置")]
    [SerializeField] private Sprite heartCoinIcon;
    [SerializeField] private Sprite starIcon;
    [SerializeField] private Sprite diamondIcon;
    
    [Header("货币颜色配置")]
    [SerializeField] private Color heartCoinColor = new Color(1f, 0.6f, 0.7f, 1f);
    [SerializeField] private Color starColor = new Color(1f, 0.9f, 0.4f, 1f);
    [SerializeField] private Color diamondColor = new Color(0.6f, 0.9f, 1f, 1f);
    
    [Header("动画配置")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private float bounceIntensity = 0.2f;
    [SerializeField] private float changeDisplayDuration = 1.5f;
    [SerializeField] private AnimationCurve bounceCurve = AnimationCurve.EaseOutBack(0f, 0f, 1f, 1f);
    
    [Header("特效配置")]
    [SerializeField] private GameObject gainEffectPrefab;
    [SerializeField] private GameObject particleEffect;
    [SerializeField] private Transform effectSpawnPoint;
    
    // 当前货币数据
    private CurrencyType currentType = CurrencyType.HeartCoin;
    private int currentAmount = 0;
    private int displayedAmount = 0;
    
    // 动画状态
    private bool isAnimating = false;
    private Coroutine countCoroutine;
    private Queue<int> pendingChanges = new Queue<int>();
    
    // 货币类型枚举
    public enum CurrencyType
    {
        HeartCoin,  // 心动币
        Star,       // 星星
        Diamond     // 钻石
    }
    
    private void Start()
    {
        Initialize();
    }
    
    /// <summary>
    /// 初始化
    /// </summary>
    private void Initialize()
    {
        UpdateVisualStyle();
    }
    
    /// <summary>
    /// 设置货币数量
    /// </summary>
    public void SetAmount(int amount)
    {
        SetAmount(amount, currentType);
    }
    
    /// <summary>
    /// 设置货币数量和类型
    /// </summary>
    public void SetAmount(int amount, CurrencyType type)
    {
        int oldAmount = currentAmount;
        currentAmount = Mathf.Max(0, amount);
        currentType = type;
        
        UpdateVisualStyle();
        
        if (useAnimation && oldAmount != currentAmount)
        {
            AnimateAmountChange(oldAmount, currentAmount);
        }
        else
        {
            displayedAmount = currentAmount;
            UpdateDisplayText();
        }
    }
    
    /// <summary>
    /// 增加货币
    /// </summary>
    public void AddAmount(int amount)
    {
        AddAmount(amount, currentType);
    }
    
    /// <summary>
    /// 增加货币（指定类型）
    /// </summary>
    public void AddAmount(int amount, CurrencyType type)
    {
        if (amount < 0) return;
        
        int oldAmount = currentAmount;
        currentAmount += amount;
        
        if (type != currentType)
        {
            currentType = type;
            UpdateVisualStyle();
        }
        
        if (useAnimation)
        {
            ShowChangeEffect(amount, true);
            AnimateAmountChange(oldAmount, currentAmount);
        }
        else
        {
            displayedAmount = currentAmount;
            UpdateDisplayText();
        }
    }
    
    /// <summary>
    /// 减少货币
    /// </summary>
    public bool SubtractAmount(int amount)
    {
        return SubtractAmount(amount, currentType);
    }
    
    /// <summary>
    /// 减少货币（指定类型）
    /// </summary>
    public bool SubtractAmount(int amount, CurrencyType type)
    {
        if (amount < 0 || currentAmount < amount) return false;
        
        int oldAmount = currentAmount;
        currentAmount -= amount;
        
        if (type != currentType)
        {
            currentType = type;
            UpdateVisualStyle();
        }
        
        if (useAnimation)
        {
            ShowChangeEffect(-amount, false);
            AnimateAmountChange(oldAmount, currentAmount);
        }
        else
        {
            displayedAmount = currentAmount;
            UpdateDisplayText();
        }
        
        return true;
    }
    
    /// <summary>
    /// 检查是否有足够的货币
    /// </summary>
    public bool HasEnough(int amount)
    {
        return currentAmount >= amount;
    }
    
    /// <summary>
    /// 数量变化动画
    /// </summary>
    private void AnimateAmountChange(int from, int to)
    {
        if (countCoroutine != null)
        {
            StopCoroutine(countCoroutine);
        }
        
        countCoroutine = StartCoroutine(CountAnimation(from, to));
    }
    
    private IEnumerator CountAnimation(int from, int to)
    {
        isAnimating = true;
        float elapsed = 0f;
        float duration = 0.5f;
        float bounceElapsed = 0f;
        float bounceDuration = 0.3f;
        
        Vector3 originalScale = transform.localScale;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            displayedAmount = Mathf.RoundToInt(Mathf.Lerp(from, to, smoothT));
            UpdateDisplayText();
            
            yield return null;
        }
        
        displayedAmount = to;
        UpdateDisplayText();
        
        // 弹跳效果
        while (bounceElapsed < bounceDuration)
        {
            bounceElapsed += Time.deltaTime;
            float t = bounceElapsed / bounceDuration;
            float curveValue = bounceCurve.Evaluate(t);
            
            float bounce = 1f + bounceIntensity * (1f - t);
            transform.localScale = originalScale * bounce;
            
            yield return null;
        }
        
        transform.localScale = originalScale;
        isAnimating = false;
    }
    
    /// <summary>
    /// 显示变化效果
    /// </summary>
    private void ShowChangeEffect(int amount, bool isGain)
    {
        if (changeText == null) return;
        
        string prefix = isGain ? "+" : "-";
        changeText.text = $"{prefix}{amount}";
        changeText.color = isGain ? GetCurrencyColor() : Color.red;
        
        StartCoroutine(ShowChangeTextAnimation());
    }
    
    private IEnumerator ShowChangeTextAnimation()
    {
        if (changeText == null) yield break;
        
        changeText.gameObject.SetActive(true);
        
        float elapsed = 0f;
        float duration = changeDisplayDuration;
        
        Vector3 startPos = changeText.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0f, 30f, 0f);
        
        Color startColor = changeText.color;
        startColor.a = 1f;
        Color endColor = startColor;
        endColor.a = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            changeText.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            changeText.color = Color.Lerp(startColor, endColor, t);
            
            yield return null;
        }
        
        changeText.gameObject.SetActive(false);
        changeText.transform.localPosition = startPos;
    }
    
    /// <summary>
    /// 显示获得效果
    /// </summary>
    public void ShowGainEffect(string text)
    {
        if (changeText != null)
        {
            changeText.text = text;
            changeText.color = GetCurrencyColor();
            StartCoroutine(ShowChangeTextAnimation());
        }
        
        if (particleEffect != null)
        {
            StartCoroutine(SpawnParticles());
        }
    }
    
    private IEnumerator SpawnParticles()
    {
        if (particleEffect == null) yield break;
        
        Transform spawnPoint = effectSpawnPoint != null ? effectSpawnPoint : transform;
        GameObject particles = Instantiate(particleEffect, spawnPoint.position, Quaternion.identity, transform);
        
        yield return new WaitForSeconds(1f);
        
        Destroy(particles);
    }
    
    /// <summary>
    /// 更新显示文本
    /// </summary>
    private void UpdateDisplayText()
    {
        if (amountText != null)
        {
            amountText.text = displayedAmount.ToString();
        }
    }
    
    /// <summary>
    /// 更新视觉样式
    /// </summary>
    private void UpdateVisualStyle()
    {
        Sprite icon = GetCurrencyIcon();
        Color color = GetCurrencyColor();
        
        if (currencyIcon != null && icon != null)
        {
            currencyIcon.sprite = icon;
        }
        
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(color.r, color.g, color.b, 0.3f);
        }
        
        if (amountText != null)
        {
            amountText.color = color;
        }
    }
    
    /// <summary>
    /// 获取货币图标
    /// </summary>
    private Sprite GetCurrencyIcon()
    {
        switch (currentType)
        {
            case CurrencyType.HeartCoin:
                return heartCoinIcon;
            case CurrencyType.Star:
                return starIcon;
            case CurrencyType.Diamond:
                return diamondIcon;
            default:
                return heartCoinIcon;
        }
    }
    
    /// <summary>
    /// 获取货币颜色
    /// </summary>
    private Color GetCurrencyColor()
    {
        switch (currentType)
        {
            case CurrencyType.HeartCoin:
                return heartCoinColor;
            case CurrencyType.Star:
                return starColor;
            case CurrencyType.Diamond:
                return diamondColor;
            default:
                return heartCoinColor;
        }
    }
    
    /// <summary>
    /// 获取当前货币类型
    /// </summary>
    public CurrencyType GetCurrencyType()
    {
        return currentType;
    }
    
    /// <summary>
    /// 获取当前货币数量
    /// </summary>
    public int GetCurrentAmount()
    {
        return currentAmount;
    }
    
    /// <summary>
    /// 设置货币类型
    /// </summary>
    public void SetCurrencyType(CurrencyType type)
    {
        if (currentType != type)
        {
            currentType = type;
            UpdateVisualStyle();
        }
    }
    
    /// <summary>
    /// 闪烁效果
    /// </summary>
    public void Flash()
    {
        StartCoroutine(FlashAnimation());
    }
    
    private IEnumerator FlashAnimation()
    {
        if (backgroundImage == null) yield break;
        
        Color originalColor = backgroundImage.color;
        float elapsed = 0f;
        float duration = 0.2f;
        
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            
            backgroundImage.color = Color.Lerp(originalColor, GetCurrencyColor(), t);
            yield return null;
        }
        
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            
            backgroundImage.color = Color.Lerp(GetCurrencyColor(), originalColor, t);
            yield return null;
        }
        
        backgroundImage.color = originalColor;
    }
    
    /// <summary>
    /// 震动效果
    /// </summary>
    public void Shake()
    {
        StartCoroutine(ShakeAnimation());
    }
    
    private IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        float duration = 0.3f;
        float intensity = 5f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * intensity * (1f - elapsed / duration);
            float y = Random.Range(-1f, 1f) * intensity * (1f - elapsed / duration);
            
            transform.localPosition = originalPos + new Vector3(x, y, 0f);
            yield return null;
        }
        
        transform.localPosition = originalPos;
    }
}
