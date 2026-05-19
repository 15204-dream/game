using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 弹窗组件 - 提示、确认、选择等弹窗
/// </summary>
public class UIPopup : MonoBehaviour
{
    [Header("弹窗配置")]
    [SerializeField] private string popupId;
    [SerializeField] private PopupType popupType = PopupType.Message;
    [SerializeField] private bool closeOnClickOutside = true;
    [SerializeField] private bool closeOnEscape = true;
    [SerializeField] private bool showCloseButton = true;
    
    [Header("组件引用")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI confirmButtonText;
    [SerializeField] private TextMeshProUGUI cancelButtonText;
    [SerializeField] private TextMeshProUGUI secondaryButtonText;
    [SerializeField] private UIButton confirmButton;
    [SerializeField] private UIButton cancelButton;
    [SerializeField] private UIButton secondaryButton;
    [SerializeField] private UIButton closeButton;
    [SerializeField] private Image iconBadge;
    
    [Header("动画配置")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private PopupAnimationType animationType = PopupAnimationType.Popup;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseOutBack(0f, 0f, 1f, 1f);
    
    [Header("视觉配置")]
    [SerializeField] private Color backgroundColor = new Color(1f, 0.97f, 0.97f, 0.98f);
    [SerializeField] private Color accentColor = new Color(1f, 0.6f, 0.7f, 1f);
    [SerializeField] private Color warningColor = new Color(1f, 0.8f, 0.4f, 1f);
    [SerializeField] private Color errorColor = new Color(1f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color successColor = new Color(0.6f, 1f, 0.6f, 1f);
    
    [Header("预设图标")]
    [SerializeField] private Sprite heartIcon;
    [SerializeField] private Sprite warningIcon;
    [SerializeField] private Sprite errorIcon;
    [SerializeField] private Sprite successIcon;
    [SerializeField] private Sprite questionIcon;
    [SerializeField] private Sprite starIcon;
    [SerializeField] private Sprite giftIcon;
    
    // 私有变量
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isShowing = false;
    private bool isAnimating = false;
    private Coroutine animationCoroutine;
    
    // 事件回调
    private System.Action onConfirm;
    private System.Action onCancel;
    private System.Action onSecondary;
    
    // 弹窗类型
    public enum PopupType
    {
        Message,
        Confirm,
        Choice,
        Warning,
        Error,
        Success,
        Custom
    }
    
    // 动画类型
    public enum PopupAnimationType
    {
        None,
        Popup,
        Fade,
        SlideUp,
        SlideDown
    }
    
    // 属性
    public string PopupId
    {
        get { return popupId; }
        set { popupId = value; }
    }
    
    public bool IsShowing
    {
        get { return isShowing; }
    }
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // 初始化按钮事件
        SetupButtonEvents();
        
        // 初始状态
        if (useAnimation)
        {
            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }
        else
        {
            canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one;
        }
    }
    
    private void Start()
    {
        ApplyVisualStyle();
    }
    
    private void Update()
    {
        if ((closeOnEscape || closeOnClickOutside) && isShowing && Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }
    
    /// <summary>
    /// 显示消息弹窗
    /// </summary>
    public void ShowMessage(string title, string message, Sprite icon = null)
    {
        SetupPopup(PopupType.Message, title, message);
        if (icon != null)
        {
            SetIcon(icon);
        }
        else
        {
            SetIcon(heartIcon);
        }
        Show(null);
    }
    
    /// <summary>
    /// 显示确认弹窗
    /// </summary>
    public void ShowConfirm(string title, string message, System.Action onConfirm, System.Action onCancel = null)
    {
        SetupPopup(PopupType.Confirm, title, message);
        SetIcon(questionIcon);
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        Show(null);
    }
    
    /// <summary>
    /// 显示选择弹窗
    /// </summary>
    public void ShowChoice(string title, string message, string confirmText, string cancelText, 
                          System.Action onConfirm, System.Action onCancel = null)
    {
        SetupPopup(PopupType.Choice, title, message);
        SetIcon(questionIcon);
        
        if (confirmButtonText != null)
        {
            confirmButtonText.text = confirmText;
        }
        if (cancelButtonText != null)
        {
            cancelButtonText.text = cancelText;
        }
        
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        Show(null);
    }
    
    /// <summary>
    /// 显示警告弹窗
    /// </summary>
    public void ShowWarning(string title, string message, System.Action onConfirm = null)
    {
        SetupPopup(PopupType.Warning, title, message);
        SetIcon(warningIcon);
        SetAccentColor(warningColor);
        this.onConfirm = onConfirm;
        Show(null);
    }
    
    /// <summary>
    /// 显示错误弹窗
    /// </summary>
    public void ShowError(string title, string message, System.Action onConfirm = null)
    {
        SetupPopup(PopupType.Error, title, message);
        SetIcon(errorIcon);
        SetAccentColor(errorColor);
        this.onConfirm = onConfirm;
        Show(null);
    }
    
    /// <summary>
    /// 显示成功弹窗
    /// </summary>
    public void ShowSuccess(string title, string message, System.Action onConfirm = null)
    {
        SetupPopup(PopupType.Success, title, message);
        SetIcon(successIcon);
        SetAccentColor(successColor);
        this.onConfirm = onConfirm;
        Show(null);
    }
    
    /// <summary>
    /// 显示自定义弹窗
    /// </summary>
    public void ShowCustom(string title, string message, string confirmText, string cancelText, 
                          string secondaryText, System.Action onConfirm, 
                          System.Action onCancel = null, System.Action onSecondary = null)
    {
        SetupPopup(PopupType.Custom, title, message);
        
        if (confirmButtonText != null)
        {
            confirmButtonText.text = confirmText;
        }
        if (cancelButtonText != null)
        {
            cancelButtonText.text = cancelText;
        }
        if (secondaryButtonText != null)
        {
            secondaryButtonText.text = secondaryText;
            secondaryButton?.gameObject.SetActive(!string.IsNullOrEmpty(secondaryText));
        }
        
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        this.onSecondary = onSecondary;
        Show(null);
    }
    
    /// <summary>
    /// 隐藏弹窗
    /// </summary>
    public void Hide()
    {
        if (!isShowing || isAnimating) return;
        
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        animationCoroutine = StartCoroutine(HideAnimation());
    }
    
    /// <summary>
    /// 确认操作
    /// </summary>
    public void Confirm()
    {
        if (!isShowing) return;
        
        onConfirm?.Invoke();
        Hide();
    }
    
    /// <summary>
    /// 取消操作
    /// </summary>
    public void Cancel()
    {
        if (!isShowing) return;
        
        onCancel?.Invoke();
        Hide();
    }
    
    /// <summary>
    /// 次要操作
    /// </summary>
    public void Secondary()
    {
        if (!isShowing) return;
        
        onSecondary?.Invoke();
    }
    
    /// <summary>
    /// 设置标题
    /// </summary>
    public void SetTitle(string title)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }
    }
    
    /// <summary>
    /// 设置消息内容
    /// </summary>
    public void SetMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }
    
    /// <summary>
    /// 设置图标
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
    /// 设置强调色
    /// </summary>
    public void SetAccentColor(Color color)
    {
        accentColor = color;
        
        if (confirmButton != null)
        {
            // 可以根据需要设置按钮颜色
        }
    }
    
    private void SetupPopup(PopupType type, string title, string message)
    {
        popupType = type;
        
        if (titleText != null)
        {
            titleText.text = title;
        }
        
        if (messageText != null)
        {
            messageText.text = message;
        }
        
        // 根据类型配置按钮
        ConfigureButtonsByType(type);
    }
    
    private void ConfigureButtonsByType(PopupType type)
    {
        // 默认隐藏所有按钮
        if (confirmButton != null) confirmButton.gameObject.SetActive(false);
        if (cancelButton != null) cancelButton.gameObject.SetActive(false);
        if (secondaryButton != null) secondaryButton.gameObject.SetActive(false);
        if (closeButton != null) closeButton.gameObject.SetActive(showCloseButton);
        
        switch (type)
        {
            case PopupType.Message:
                if (confirmButton != null)
                {
                    confirmButton.gameObject.SetActive(true);
                    if (confirmButtonText != null) confirmButtonText.text = "确定";
                }
                break;
                
            case PopupType.Confirm:
            case PopupType.Choice:
                if (confirmButton != null)
                {
                    confirmButton.gameObject.SetActive(true);
                    if (confirmButtonText != null) confirmButtonText.text = "确定";
                }
                if (cancelButton != null)
                {
                    cancelButton.gameObject.SetActive(true);
                    if (cancelButtonText != null) cancelButtonText.text = "取消";
                }
                break;
                
            case PopupType.Warning:
            case PopupType.Error:
            case PopupType.Success:
                if (confirmButton != null)
                {
                    confirmButton.gameObject.SetActive(true);
                    if (confirmButtonText != null) confirmButtonText.text = "好的";
                }
                if (cancelButton != null)
                {
                    cancelButton.gameObject.SetActive(true);
                    if (cancelButtonText != null) cancelButtonText.text = "取消";
                }
                break;
        }
    }
    
    private void SetupButtonEvents()
    {
        if (confirmButton != null)
        {
            confirmButton.OnClick.AddListener(Confirm);
        }
        
        if (cancelButton != null)
        {
            cancelButton.OnClick.AddListener(Cancel);
        }
        
        if (secondaryButton != null)
        {
            secondaryButton.OnClick.AddListener(Secondary);
        }
        
        if (closeButton != null)
        {
            closeButton.OnClick.AddListener(Cancel);
        }
    }
    
    private void Show(System.Action onComplete)
    {
        if (isShowing || isAnimating) return;
        
        gameObject.SetActive(true);
        isShowing = true;
        isAnimating = true;
        
        if (useAnimation)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            animationCoroutine = StartCoroutine(ShowAnimation(onComplete));
        }
        else
        {
            canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one;
            isAnimating = false;
        }
    }
    
    private IEnumerator ShowAnimation(System.Action onComplete)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        Vector3 startScale = transform.localScale;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curveValue = animationCurve.Evaluate(t);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, curveValue);
            transform.localScale = Vector3.Lerp(startScale, Vector3.one, curveValue);
            
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
        isAnimating = false;
        
        onComplete?.Invoke();
    }
    
    private IEnumerator HideAnimation()
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        Vector3 startScale = transform.localScale;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
        isShowing = false;
        
        // 重置回调
        onConfirm = null;
        onCancel = null;
        onSecondary = null;
    }
    
    private void ApplyVisualStyle()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }
    }
}
