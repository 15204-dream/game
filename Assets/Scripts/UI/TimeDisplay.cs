using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 时间显示组件 - 显示游戏内时间
/// </summary>
public class TimeDisplay : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private Image timeIcon;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI periodText;
    [SerializeField] private Image dayNightIndicator;
    [SerializeField] private Image[] hourIndicators;
    
    [Header("时间段配置")]
    [SerializeField] private Sprite morningIcon;
    [SerializeField] private Sprite afternoonIcon;
    [SerializeField] private Sprite eveningIcon;
    [SerializeField] private Sprite nightIcon;
    
    [Header("时间段颜色")]
    [SerializeField] private Color morningColor = new Color(1f, 0.9f, 0.6f, 1f);
    [SerializeField] private Color afternoonColor = new Color(1f, 0.8f, 0.5f, 1f);
    [SerializeField] private Color eveningColor = new Color(1f, 0.6f, 0.5f, 1f);
    [SerializeField] private Color nightColor = new Color(0.4f, 0.4f, 0.7f, 1f);
    
    [Header("动画配置")]
    [SerializeField] private bool useTimeTransition = true;
    [SerializeField] private float transitionDuration = 0.5f;
    [SerializeField] private bool showHourIndicators = true;
    [SerializeField] private int currentHourIndicator = -1;
    
    // 当前时间
    private int currentHour = 6;
    private int currentMinute = 0;
    private TimePeriod currentPeriod = TimePeriod.Morning;
    
    // 时间段枚举
    public enum TimePeriod
    {
        Morning,   // 早上 (6:00 - 11:59)
        Afternoon, // 下午 (12:00 - 17:59)
        Evening,   // 傍晚 (18:00 - 20:59)
        Night      // 夜晚 (21:00 - 5:59)
    }
    
    /// <summary>
    /// 设置时间
    /// </summary>
    public void SetTime(int hour, int minute)
    {
        currentHour = Mathf.Clamp(hour, 0, 23);
        currentMinute = Mathf.Clamp(minute, 0, 59);
        
        UpdateTimeDisplay();
        UpdateTimePeriod();
        UpdateVisualStyle();
    }
    
    /// <summary>
    /// 设置时间段
    /// </summary>
    public void SetTimePeriod(TimePeriod period)
    {
        if (currentPeriod == period && !useTimeTransition) return;
        
        TimePeriod oldPeriod = currentPeriod;
        currentPeriod = period;
        
        // 更新时间到对应时间段的默认时间
        switch (period)
        {
            case TimePeriod.Morning:
                currentHour = 7;
                currentMinute = 0;
                break;
            case TimePeriod.Afternoon:
                currentHour = 14;
                currentMinute = 0;
                break;
            case TimePeriod.Evening:
                currentHour = 18;
                currentMinute = 30;
                break;
            case TimePeriod.Night:
                currentHour = 21;
                currentMinute = 0;
                break;
        }
        
        UpdateTimeDisplay();
        UpdateVisualStyle();
        
        if (useTimeTransition)
        {
            StartCoroutine(TransitionAnimation(oldPeriod, period));
        }
    }
    
    /// <summary>
    /// 增加时间
    /// </summary>
    public void AddTime(int hours, int minutes = 0)
    {
        currentMinute += minutes;
        if (currentMinute >= 60)
        {
            hours += currentMinute / 60;
            currentMinute %= 60;
        }
        
        currentHour += hours;
        if (currentHour >= 24)
        {
            currentHour %= 24;
        }
        
        UpdateTimeDisplay();
        UpdateTimePeriod();
        UpdateVisualStyle();
    }
    
    /// <summary>
    /// 更新时间显示文本
    /// </summary>
    private void UpdateTimeDisplay()
    {
        if (timeText != null)
        {
            timeText.text = $"{currentHour:D2}:{currentMinute:D2}";
        }
    }
    
    /// <summary>
    /// 更新时间段
    /// </summary>
    private void UpdateTimePeriod()
    {
        TimePeriod newPeriod;
        
        if (currentHour >= 6 && currentHour < 12)
        {
            newPeriod = TimePeriod.Morning;
        }
        else if (currentHour >= 12 && currentHour < 18)
        {
            newPeriod = TimePeriod.Afternoon;
        }
        else if (currentHour >= 18 && currentHour < 21)
        {
            newPeriod = TimePeriod.Evening;
        }
        else
        {
            newPeriod = TimePeriod.Night;
        }
        
        if (newPeriod != currentPeriod)
        {
            currentPeriod = newPeriod;
            UpdateVisualStyle();
        }
    }
    
    /// <summary>
    /// 更新视觉样式
    /// </summary>
    private void UpdateVisualStyle()
    {
        Color periodColor = GetPeriodColor();
        Sprite periodIcon = GetPeriodIcon();
        string periodName = GetPeriodName();
        
        if (timeIcon != null && periodIcon != null)
        {
            timeIcon.sprite = periodIcon;
        }
        
        if (periodText != null)
        {
            periodText.text = periodName;
            periodText.color = periodColor;
        }
        
        if (dayNightIndicator != null)
        {
            dayNightIndicator.color = periodColor;
        }
        
        // 更新小时指示器
        if (showHourIndicators && hourIndicators != null)
        {
            int hourIndex = currentHour;
            for (int i = 0; i < hourIndicators.Length; i++)
            {
                if (hourIndicators[i] != null)
                {
                    bool isActive = (i == hourIndex % 24);
                    hourIndicators[i].gameObject.SetActive(isActive);
                }
            }
        }
    }
    
    /// <summary>
    /// 获取时间段颜色
    /// </summary>
    private Color GetPeriodColor()
    {
        switch (currentPeriod)
        {
            case TimePeriod.Morning:
                return morningColor;
            case TimePeriod.Afternoon:
                return afternoonColor;
            case TimePeriod.Evening:
                return eveningColor;
            case TimePeriod.Night:
                return nightColor;
            default:
                return morningColor;
        }
    }
    
    /// <summary>
    /// 获取时间段图标
    /// </summary>
    private Sprite GetPeriodIcon()
    {
        switch (currentPeriod)
        {
            case TimePeriod.Morning:
                return morningIcon;
            case TimePeriod.Afternoon:
                return afternoonIcon;
            case TimePeriod.Evening:
                return eveningIcon;
            case TimePeriod.Night:
                return nightIcon;
            default:
                return morningIcon;
        }
    }
    
    /// <summary>
    /// 获取时间段名称
    /// </summary>
    private string GetPeriodName()
    {
        switch (currentPeriod)
        {
            case TimePeriod.Morning:
                return "上午";
            case TimePeriod.Afternoon:
                return "下午";
            case TimePeriod.Evening:
                return "傍晚";
            case TimePeriod.Night:
                return "夜晚";
            default:
                return "上午";
        }
    }
    
    /// <summary>
    /// 过渡动画
    /// </summary>
    private IEnumerator TransitionAnimation(TimePeriod from, TimePeriod to)
    {
        float elapsed = 0f;
        Color fromColor = GetPeriodColorForPeriod(from);
        Color toColor = GetPeriodColorForPeriod(to);
        
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            Color currentColor = Color.Lerp(fromColor, toColor, smoothT);
            
            if (dayNightIndicator != null)
            {
                dayNightIndicator.color = currentColor;
            }
            
            yield return null;
        }
        
        if (dayNightIndicator != null)
        {
            dayNightIndicator.color = toColor;
        }
    }
    
    /// <summary>
    /// 获取指定时间段对应的颜色
    /// </summary>
    private Color GetPeriodColorForPeriod(TimePeriod period)
    {
        switch (period)
        {
            case TimePeriod.Morning:
                return morningColor;
            case TimePeriod.Afternoon:
                return afternoonColor;
            case TimePeriod.Evening:
                return eveningColor;
            case TimePeriod.Night:
                return nightColor;
            default:
                return morningColor;
        }
    }
    
    /// <summary>
    /// 获取当前小时
    /// </summary>
    public int GetCurrentHour()
    {
        return currentHour;
    }
    
    /// <summary>
    /// 获取当前分钟
    /// </summary>
    public int GetCurrentMinute()
    {
        return currentMinute;
    }
    
    /// <summary>
    /// 获取当前时间段
    /// </summary>
    public TimePeriod GetCurrentPeriod()
    {
        return currentPeriod;
    }
    
    /// <summary>
    /// 获取格式化的时间字符串
    /// </summary>
    public string GetFormattedTime()
    {
        return $"{currentHour:D2}:{currentMinute:D2}";
    }
    
    /// <summary>
    /// 获取格式化的时间字符串（带时间段）
    /// </summary>
    public string GetFormattedTimeWithPeriod()
    {
        return $"{GetFormattedTime()} {GetPeriodName()}";
    }
    
    /// <summary>
    /// 设置时间显示是否可见
    /// </summary>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
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
        if (timeText == null) yield break;
        
        Color originalColor = timeText.color;
        float elapsed = 0f;
        float duration = 0.3f;
        
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            
            timeText.color = Color.Lerp(originalColor, Color.white, t);
            yield return null;
        }
        
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            
            timeText.color = Color.Lerp(Color.white, originalColor, t);
            yield return null;
        }
        
        timeText.color = originalColor;
    }
}
