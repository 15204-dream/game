using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI主题配置 - 主题化UI配置管理
/// </summary>
[CreateAssetMenu(fileName = "UITheme", menuName = "UI/Create UI Theme")]
public class UIThemes : ScriptableObject
{
    [Header("主题信息")]
    [SerializeField] private string themeName = "默认主题";
    [SerializeField] private string themeDescription = "温馨浪漫的默认主题";
    [SerializeField] private bool isDefault = true;
    
    [Header("主色调")]
    [SerializeField] private Color primaryColor = new Color(1f, 0.6f, 0.7f, 1f);
    [SerializeField] private Color primaryLightColor = new Color(1f, 0.8f, 0.85f, 1f);
    [SerializeField] private Color primaryDarkColor = new Color(0.9f, 0.4f, 0.5f, 1f);
    
    [Header("背景色")]
    [SerializeField] private Color backgroundColor = new Color(1f, 0.98f, 0.98f, 1f);
    [SerializeField] private Color surfaceColor = new Color(1f, 0.97f, 0.97f, 0.95f);
    [SerializeField] private Color overlayColor = new Color(0f, 0f, 0f, 0.5f);
    
    [Header("文字色")]
    [SerializeField] private Color textPrimaryColor = new Color(0.3f, 0.2f, 0.25f, 1f);
    [SerializeField] private Color textSecondaryColor = new Color(0.5f, 0.4f, 0.45f, 1f);
    [SerializeField] private Color textAccentColor = new Color(1f, 0.4f, 0.6f, 1f);
    
    [Header("按钮色")]
    [SerializeField] private Color buttonNormalColor = new Color(1f, 0.8f, 0.85f, 1f);
    [SerializeField] private Color buttonHighlightColor = new Color(1f, 0.6f, 0.7f, 1f);
    [SerializeField] private Color buttonPressedColor = new Color(0.9f, 0.5f, 0.6f, 1f);
    [SerializeField] private Color buttonDisabledColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
    
    [Header("进度条色")]
    [SerializeField] private Color progressBarFillColor = new Color(1f, 0.6f, 0.7f, 1f);
    [SerializeField] private Color progressBarBackgroundColor = new Color(1f, 0.85f, 0.9f, 0.3f);
    [SerializeField] private Color progressBarGlowColor = new Color(1f, 0.4f, 0.5f, 0.5f);
    
    [Header("边框和装饰")]
    [SerializeField] private Color borderColor = new Color(1f, 0.75f, 0.8f, 1f);
    [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.1f);
    [SerializeField] private Color heartGlowColor = new Color(1f, 0.3f, 0.4f, 0.3f);
    
    [Header("状态色")]
    [SerializeField] private Color successColor = new Color(0.4f, 1f, 0.5f, 1f);
    [SerializeField] private Color warningColor = new Color(1f, 0.8f, 0.3f, 1f);
    [SerializeField] private Color errorColor = new Color(1f, 0.4f, 0.4f, 1f);
    [SerializeField] private Color infoColor = new Color(0.4f, 0.7f, 1f, 1f);
    
    [Header("特效配置")]
    [SerializeField] private float transitionSpeed = 0.3f;
    [SerializeField] private float bounceIntensity = 0.2f;
    [SerializeField] private float glowIntensity = 0.5f;
    [SerializeField] private float particleDensity = 1f;
    
    [Header("圆角配置")]
    [SerializeField] private float smallCornerRadius = 8f;
    [SerializeField] private float mediumCornerRadius = 15f;
    [SerializeField] private float largeCornerRadius = 25f;
    
    // 属性访问器
    #region 主色调属性
    
    public Color PrimaryColor => primaryColor;
    public Color PrimaryLightColor => primaryLightColor;
    public Color PrimaryDarkColor => primaryDarkColor;
    
    #endregion
    
    #region 背景色属性
    
    public Color BackgroundColor => backgroundColor;
    public Color SurfaceColor => surfaceColor;
    public Color OverlayColor => overlayColor;
    
    #endregion
    
    #region 文字色属性
    
    public Color TextPrimaryColor => textPrimaryColor;
    public Color TextSecondaryColor => textSecondaryColor;
    public Color TextAccentColor => textAccentColor;
    
    #endregion
    
    #region 按钮色属性
    
    public Color ButtonNormalColor => buttonNormalColor;
    public Color ButtonHighlightColor => buttonHighlightColor;
    public Color ButtonPressedColor => buttonPressedColor;
    public Color ButtonDisabledColor => buttonDisabledColor;
    
    #endregion
    
    #region 进度条色属性
    
    public Color ProgressBarFillColor => progressBarFillColor;
    public Color ProgressBarBackgroundColor => progressBarBackgroundColor;
    public Color ProgressBarGlowColor => progressBarGlowColor;
    
    #endregion
    
    #region 边框和装饰属性
    
    public Color BorderColor => borderColor;
    public Color ShadowColor => shadowColor;
    public Color HeartGlowColor => heartGlowColor;
    
    #endregion
    
    #region 状态色属性
    
    public Color SuccessColor => successColor;
    public Color WarningColor => warningColor;
    public Color ErrorColor => errorColor;
    public Color InfoColor => infoColor;
    
    #endregion
    
    #region 特效配置属性
    
    public float TransitionSpeed => transitionSpeed;
    public float BounceIntensity => bounceIntensity;
    public float GlowIntensity => glowIntensity;
    public float ParticleDensity => particleDensity;
    
    #endregion
    
    #region 圆角配置属性
    
    public float SmallCornerRadius => smallCornerRadius;
    public float MediumCornerRadius => mediumCornerRadius;
    public float LargeCornerRadius => largeCornerRadius;
    
    #endregion
    
    /// <summary>
    /// 获取主题名称
    /// </summary>
    public string ThemeName => themeName;
    
    /// <summary>
    /// 是否为默认主题
    /// </summary>
    public bool IsDefault => isDefault;
    
    /// <summary>
    /// 根据好感度级别获取颜色
    /// </summary>
    public Color GetAffectionColor(int level)
    {
        switch (level)
        {
            case 0: return new Color(0.7f, 0.8f, 1f, 1f);      // 冷淡
            case 1: return new Color(1f, 0.85f, 0.7f, 1f);    // 一般
            case 2: return new Color(1f, 0.7f, 0.6f, 1f);     // 温暖
            case 3: return new Color(1f, 0.5f, 0.5f, 1f);    // 热烈
            case 4: return new Color(1f, 0.3f, 0.3f, 1f);     // 燃烧
            default: return primaryColor;
        }
    }
    
    /// <summary>
    /// 根据时间段获取背景颜色
    /// </summary>
    public Color GetTimePeriodBackground(int hour)
    {
        if (hour >= 6 && hour < 12)
        {
            return new Color(1f, 0.98f, 0.95f, 1f);  // 早晨
        }
        else if (hour >= 12 && hour < 18)
        {
            return new Color(1f, 0.97f, 0.95f, 1f);  // 下午
        }
        else if (hour >= 18 && hour < 21)
        {
            return new Color(1f, 0.92f, 0.9f, 1f);    // 傍晚
        }
        else
        {
            return new Color(0.3f, 0.3f, 0.4f, 1f);   // 夜晚
        }
    }
    
    /// <summary>
    /// 根据数值获取颜色渐变
    /// </summary>
    public Color GetProgressColor(float progress)
    {
        if (progress < 0.25f)
        {
            return Color.Lerp(errorColor, warningColor, progress * 4f);
        }
        else if (progress < 0.5f)
        {
            return Color.Lerp(warningColor, new Color(1f, 1f, 0.5f, 1f), (progress - 0.25f) * 4f);
        }
        else if (progress < 0.75f)
        {
            return Color.Lerp(new Color(1f, 1f, 0.5f, 1f), successColor, (progress - 0.5f) * 4f);
        }
        else
        {
            return Color.Lerp(successColor, primaryColor, (progress - 0.75f) * 4f);
        }
    }
    
    /// <summary>
    /// 颜色插值
    /// </summary>
    public Color Lerp(Color from, Color to, float t)
    {
        return Color.Lerp(from, to, t);
    }
    
    /// <summary>
    /// 创建主题变体
    /// </summary>
    public UIThemes CreateVariant(string variantName, System.Action<UIThemes> modifier)
    {
        UIThemes variant = Instantiate(this);
        variant.themeName = $"{themeName} - {variantName}";
        variant.isDefault = false;
        modifier?.Invoke(variant);
        return variant;
    }
}

/// <summary>
/// 预设主题库
/// </summary>
public static class PresetThemes
{
    /// <summary>
    /// 默认浪漫主题
    /// </summary>
    public static UIThemes CreateDefaultTheme()
    {
        UIThemes theme = ScriptableObject.CreateInstance<UIThemes>();
        theme.name = "DefaultRomance";
        return theme;
    }
    
    /// <summary>
    /// 梦幻主题
    /// </summary>
    public static UIThemes CreateDreamyTheme()
    {
        UIThemes theme = ScriptableObject.CreateInstance<UIThemes>();
        theme.name = "Dreamy";
        
        // 可以在这里设置梦幻主题的特殊颜色
        return theme;
    }
    
    /// <summary>
    /// 复古主题
    /// </summary>
    public static UIThemes CreateRetroTheme()
    {
        UIThemes theme = ScriptableObject.CreateInstance<UIThemes>();
        theme.name = "Retro";
        
        // 可以在这里设置复古主题的特殊颜色
        return theme;
    }
    
    /// <summary>
    /// 简约主题
    /// </summary>
    public static UIThemes CreateMinimalTheme()
    {
        UIThemes theme = ScriptableObject.CreateInstance<UIThemes>();
        theme.name = "Minimal";
        
        // 可以在这里设置简约主题的特殊颜色
        return theme;
    }
}

/// <summary>
/// 主题管理器
/// </summary>
public class ThemeManager : MonoBehaviour
{
    [Header("主题配置")]
    [SerializeField] private List<UIThemes> availableThemes = new List<UIThemes>();
    [SerializeField] private UIThemes currentTheme;
    
    private static ThemeManager instance;
    public static ThemeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ThemeManager>();
                if (instance == null)
                {
                    GameObject themeManagerObj = new GameObject("ThemeManager");
                    instance = themeManagerObj.AddComponent<ThemeManager>();
                }
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        LoadSavedTheme();
    }
    
    /// <summary>
    /// 获取当前主题
    /// </summary>
    public UIThemes GetCurrentTheme()
    {
        return currentTheme;
    }
    
    /// <summary>
    /// 设置主题
    /// </summary>
    public void SetTheme(UIThemes theme)
    {
        if (theme == null) return;
        
        currentTheme = theme;
        PlayerPrefs.SetString("CurrentTheme", theme.name);
        OnThemeChanged();
    }
    
    /// <summary>
    /// 设置主题通过名称
    /// </summary>
    public void SetThemeByName(string themeName)
    {
        UIThemes theme = availableThemes.Find(t => t.name == themeName);
        if (theme != null)
        {
            SetTheme(theme);
        }
    }
    
    /// <summary>
    /// 加载保存的主题
    /// </summary>
    private void LoadSavedTheme()
    {
        string savedThemeName = PlayerPrefs.GetString("CurrentTheme", "");
        
        if (!string.IsNullOrEmpty(savedThemeName))
        {
            UIThemes savedTheme = availableThemes.Find(t => t.name == savedThemeName);
            if (savedTheme != null)
            {
                currentTheme = savedTheme;
                return;
            }
        }
        
        // 使用默认主题
        currentTheme = availableThemes.Find(t => t.IsDefault) ?? availableThemes[0];
    }
    
    /// <summary>
    /// 主题改变回调
    /// </summary>
    private void OnThemeChanged()
    {
        // 通知所有监听者主题已更改
        // 可以在这里更新所有UI组件的颜色等
    }
    
    /// <summary>
    /// 获取所有可用主题
    /// </summary>
    public List<UIThemes> GetAvailableThemes()
    {
        return availableThemes;
    }
    
    /// <summary>
    /// 添加主题
    /// </summary>
    public void AddTheme(UIThemes theme)
    {
        if (!availableThemes.Contains(theme))
        {
            availableThemes.Add(theme);
        }
    }
    
    /// <summary>
    /// 移除主题
    /// </summary>
    public void RemoveTheme(UIThemes theme)
    {
        availableThemes.Remove(theme);
    }
}
