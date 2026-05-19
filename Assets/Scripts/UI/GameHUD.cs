using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏抬头显示（HUD）管理器
/// </summary>
public class GameHUD : MonoBehaviour
{
    [Header("HUD配置")]
    [SerializeField] private bool showHUD = true;
    [SerializeField] private bool autoHideOnDialogue = true;
    
    [Header("HUD组件")]
    [SerializeField] private GameObject hudContainer;
    [SerializeField] private ModeIndicator modeIndicator;
    [SerializeField] private TimeDisplay timeDisplay;
    [SerializeField] private CurrencyDisplay currencyDisplay;
    [SerializeField] private UIProgressBar dayProgressBar;
    [SerializeField] private UIButton menuButton;
    [SerializeField] private UIButton quickSaveButton;
    
    [Header("顶部栏")]
    [SerializeField] private GameObject topBar;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI episodeText;
    
    [Header("底部栏")]
    [SerializeField] private GameObject bottomBar;
    [SerializeField] private UIProgressBar affectionBar;
    [SerializeField] private TextMeshProUGUI affectionText;
    [SerializeField] private UIProgressBar energyBar;
    [SerializeField] private TextMeshProUGUI energyText;
    
    [Header("侧边栏")]
    [SerializeField] private GameObject sideBar;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI weatherText;
    
    [Header("快捷操作栏")]
    [SerializeField] private GameObject quickActionBar;
    [SerializeField] private UIButton phoneButton;
    [SerializeField] private UIButton inventoryButton;
    [SerializeField] private UIButton relationshipButton;
    [SerializeField] private UIButton diaryButton;
    
    [Header("动画配置")]
    [SerializeField] private float hudShowDuration = 0.3f;
    [SerializeField] private float hudHideDuration = 0.2f;
    [SerializeField] private AnimationCurve showCurve = AnimationCurve.EaseOutBack(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve hideCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    
    // 单例
    private static GameHUD instance;
    public static GameHUD Instance
    {
        get { return instance; }
    }
    
    // 状态
    private bool isVisible = true;
    private bool isAnimating = false;
    private MainMenuScene.GameMode currentMode = MainMenuScene.GameMode.Guest;
    
    // 回调
    public System.Action OnMenuButtonClicked;
    public System.Action OnQuickSaveClicked;
    public System.Action OnPhoneButtonClicked;
    public System.Action OnInventoryButtonClicked;
    public System.Action OnRelationshipButtonClicked;
    public System.Action OnDiaryButtonClicked;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        Initialize();
        SetupEvents();
    }
    
    private void Update()
    {
        // ESC键打开菜单
        if (Input.GetKeyDown(KeyCode.Escape) && isVisible)
        {
            OnMenuClicked();
        }
    }
    
    /// <summary>
    /// 初始化HUD
    /// </summary>
    private void Initialize()
    {
        if (modeIndicator != null)
        {
            modeIndicator.Initialize(currentMode);
        }
        
        UpdateDayDisplay(1, 1);
        UpdateLocation("小屋");
        UpdateWeather("晴朗");
    }
    
    /// <summary>
    /// 设置事件
    /// </summary>
    private void SetupEvents()
    {
        if (menuButton != null)
        {
            menuButton.OnClick.AddListener(OnMenuClicked);
        }
        
        if (quickSaveButton != null)
        {
            quickSaveButton.OnClick.AddListener(OnQuickSave);
        }
        
        if (phoneButton != null)
        {
            phoneButton.OnClick.AddListener(OnPhoneClicked);
        }
        
        if (inventoryButton != null)
        {
            inventoryButton.OnClick.AddListener(OnInventoryClicked);
        }
        
        if (relationshipButton != null)
        {
            relationshipButton.OnClick.AddListener(OnRelationshipClicked);
        }
        
        if (diaryButton != null)
        {
            diaryButton.OnClick.AddListener(OnDiaryClicked);
        }
    }
    
    #region 显示控制
    
    /// <summary>
    /// 显示HUD
    /// </summary>
    public void Show()
    {
        if (isVisible || isAnimating) return;
        
        isVisible = true;
        StartCoroutine(ShowHUDAnimation());
    }
    
    /// <summary>
    /// 隐藏HUD
    /// </summary>
    public void Hide()
    {
        if (!isVisible || isAnimating) return;
        
        isVisible = false;
        StartCoroutine(HideHUDAnimation());
    }
    
    /// <summary>
    /// 切换HUD显示状态
    /// </summary>
    public void Toggle()
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
    
    private IEnumerator ShowHUDAnimation()
    {
        isAnimating = true;
        
        if (hudContainer != null)
        {
            hudContainer.SetActive(true);
        }
        
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.9f;
        Vector3 endScale = Vector3.one;
        
        while (elapsed < hudShowDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hudShowDuration;
            float curveValue = showCurve.Evaluate(t);
            
            if (hudContainer != null)
            {
                hudContainer.transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);
            }
            
            yield return null;
        }
        
        if (hudContainer != null)
        {
            hudContainer.transform.localScale = endScale;
        }
        
        isAnimating = false;
    }
    
    private IEnumerator HideHUDAnimation()
    {
        isAnimating = true;
        
        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.one * 0.9f;
        
        while (elapsed < hudHideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hudHideDuration;
            float curveValue = hideCurve.Evaluate(t);
            
            if (hudContainer != null)
            {
                hudContainer.transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);
            }
            
            yield return null;
        }
        
        if (hudContainer != null)
        {
            hudContainer.transform.localScale = endScale;
            hudContainer.SetActive(false);
        }
        
        isAnimating = false;
    }
    
    #endregion
    
    #region 数据更新
    
    /// <summary>
    /// 更新游戏模式显示
    /// </summary>
    public void UpdateGameMode(MainMenuScene.GameMode mode)
    {
        currentMode = mode;
        if (modeIndicator != null)
        {
            modeIndicator.SetMode(mode);
        }
    }
    
    /// <summary>
    /// 更新天数显示
    /// </summary>
    public void UpdateDayDisplay(int day, int totalDays)
    {
        if (dayText != null)
        {
            dayText.text = $"第 {day} 天";
        }
        
        if (episodeText != null)
        {
            episodeText.text = $"第 {(day + 2) / 3} 期";
        }
        
        if (dayProgressBar != null)
        {
            dayProgressBar.SetFill((float)day / totalDays);
        }
    }
    
    /// <summary>
    /// 更新好感度显示
    /// </summary>
    public void UpdateAffection(float value, float maxValue)
    {
        if (affectionBar != null)
        {
            affectionBar.SetValue(value);
        }
        
        if (affectionText != null)
        {
            affectionText.text = $"好感度: {Mathf.RoundToInt(value)}/{Mathf.RoundToInt(maxValue)}";
        }
    }
    
    /// <summary>
    /// 更新体力显示
    /// </summary>
    public void UpdateEnergy(float value, float maxValue)
    {
        if (energyBar != null)
        {
            energyBar.SetValue(value);
        }
        
        if (energyText != null)
        {
            energyText.text = $"体力: {Mathf.RoundToInt(value)}/{Mathf.RoundToInt(maxValue)}";
        }
    }
    
    /// <summary>
    /// 更新位置显示
    /// </summary>
    public void UpdateLocation(string location)
    {
        if (locationText != null)
        {
            locationText.text = location;
        }
    }
    
    /// <summary>
    /// 更新天气显示
    /// </summary>
    public void UpdateWeather(string weather)
    {
        if (weatherText != null)
        {
            weatherText.text = weather;
        }
    }
    
    /// <summary>
    /// 更新时间显示
    /// </summary>
    public void UpdateTime(int hour, int minute)
    {
        if (timeDisplay != null)
        {
            timeDisplay.SetTime(hour, minute);
        }
    }
    
    /// <summary>
    /// 更新时间段
    /// </summary>
    public void UpdateTimePeriod(TimeDisplay.TimePeriod period)
    {
        if (timeDisplay != null)
        {
            timeDisplay.SetTimePeriod(period);
        }
    }
    
    /// <summary>
    /// 更新货币显示
    /// </summary>
    public void UpdateCurrency(int amount, CurrencyDisplay.CurrencyType type = CurrencyDisplay.CurrencyType.HeartCoin)
    {
        if (currencyDisplay != null)
        {
            currencyDisplay.SetAmount(amount, type);
        }
    }
    
    #endregion
    
    #region 按钮事件
    
    private void OnMenuClicked()
    {
        OnMenuButtonClicked?.Invoke();
    }
    
    private void OnQuickSave()
    {
        OnQuickSaveClicked?.Invoke();
    }
    
    private void OnPhoneClicked()
    {
        OnPhoneButtonClicked?.Invoke();
    }
    
    private void OnInventoryClicked()
    {
        OnInventoryButtonClicked?.Invoke();
    }
    
    private void OnRelationshipClicked()
    {
        OnRelationshipButtonClicked?.Invoke();
    }
    
    private void OnDiaryClicked()
    {
        OnDiaryButtonClicked?.Invoke();
    }
    
    #endregion
    
    #region 特效
    
    /// <summary>
    /// 显示获得货币提示
    /// </summary>
    public void ShowCurrencyGain(int amount, CurrencyDisplay.CurrencyType type)
    {
        StartCoroutine(ShowCurrencyGainEffect(amount, type));
    }
    
    private IEnumerator ShowCurrencyGainEffect(int amount, CurrencyDisplay.CurrencyType type)
    {
        if (currencyDisplay == null) yield break;
        
        string prefix = type == CurrencyDisplay.CurrencyType.HeartCoin ? "+" : "+";
        currencyDisplay.ShowGainEffect($"{prefix}{amount}");
        
        yield return new WaitForSeconds(1f);
    }
    
    /// <summary>
    /// 显示好感度变化
    /// </summary>
    public void ShowAffectionChange(float change)
    {
        if (affectionBar != null)
        {
            affectionBar.Shake();
        }
    }
    
    /// <summary>
    /// HUD闪烁效果
    /// </summary>
    public void FlashHUD()
    {
        StartCoroutine(FlashEffect());
    }
    
    private IEnumerator FlashEffect()
    {
        Color originalColor = Color.white;
        if (hudContainer != null)
        {
            Image[] images = hudContainer.GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                img.color = new Color(1f, 0.8f, 0.8f, 1f);
            }
        }
        
        yield return new WaitForSeconds(0.1f);
        
        if (hudContainer != null)
        {
            Image[] images = hudContainer.GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                img.color = originalColor;
            }
        }
    }
    
    #endregion
    
    /// <summary>
    /// 获取当前游戏模式
    /// </summary>
    public MainMenuScene.GameMode GetCurrentMode()
    {
        return currentMode;
    }
    
    /// <summary>
    /// HUD是否可见
    /// </summary>
    public bool IsVisible()
    {
        return isVisible;
    }
}
