using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 面板组件 - 可复用的UI面板容器
/// </summary>
public class UIPanel : MonoBehaviour
{
    [Header("面板配置")]
    [SerializeField] private string panelId;
    [SerializeField] private bool closeOnClickOutside = false;
    [SerializeField] private bool closeOnEscape = true;
    [SerializeField] private bool blockRaycasts = true;
    
    [Header("动画配置")]
    [SerializeField] private bool useShowAnimation = true;
    [SerializeField] private bool useHideAnimation = true;
    [SerializeField] private PanelAnimationType animationType = PanelAnimationType.FadeScale;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseOutBack(0f, 0f, 1f, 1f);
    
    [Header("背景遮罩")]
    [SerializeField] private bool useBackgroundOverlay = true;
    [SerializeField] private Image backgroundOverlay;
    [SerializeField] private Color overlayColor = new Color(0f, 0f, 0f, 0.5f);
    [SerializeField] private float overlayFadeDuration = 0.2f;
    
    [Header("视觉配置")]
    [SerializeField] private Color panelColor = new Color(1f, 0.98f, 0.98f, 0.98f);
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Color borderColor = new Color(1f, 0.75f, 0.8f, 1f);
    
    // 私有变量
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 showScale;
    private Vector3 hideScale;
    private bool isShowing = false;
    private bool isAnimating = false;
    private Coroutine animationCoroutine;
    private List<CanvasGroup> raycastTargets = new List<CanvasGroup>();
    
    // 事件
    public System.Action OnPanelShow;
    public System.Action OnPanelHide;
    public System.Action OnPanelShowComplete;
    public System.Action OnPanelHideComplete;
    
    // 属性
    public string PanelId
    {
        get { return panelId; }
        set { panelId = value; }
    }
    
    public bool IsShowing
    {
        get { return isShowing; }
    }
    
    // 面板动画类型
    public enum PanelAnimationType
    {
        None,
        Fade,
        FadeScale,
        SlideLeft,
        SlideRight,
        SlideUp,
        SlideDown,
        Bounce
    }
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // 初始化状态
        showScale = Vector3.one;
        hideScale = Vector3.zero;
        
        // 获取需要管理的Raycast目标
        CacheRaycastTargets();
        
        // 设置初始状态
        if (useShowAnimation || useHideAnimation)
        {
            canvasGroup.alpha = 0f;
            transform.localScale = hideScale;
            gameObject.SetActive(false);
        }
        else
        {
            canvasGroup.alpha = 1f;
            transform.localScale = showScale;
        }
    }
    
    private void Start()
    {
        ApplyVisualStyle();
    }
    
    private void Update()
    {
        // ESC键关闭
        if (closeOnEscape && isShowing && Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
        
        // 点击外部关闭
        if (closeOnClickOutside && isShowing && Input.GetMouseButtonDown(0))
        {
            CheckClickOutside();
        }
    }
    
    /// <summary>
    /// 显示面板
    /// </summary>
    public void Show()
    {
        Show(null);
    }
    
    /// <summary>
    /// 显示面板（带回调）
    /// </summary>
    public void Show(System.Action onComplete)
    {
        if (isShowing || isAnimating) return;
        
        gameObject.SetActive(true);
        isShowing = true;
        isAnimating = true;
        
        OnPanelShow?.Invoke();
        
        // 管理Raycast
        SetRaycastState(true);
        
        // 显示背景遮罩
        if (useBackgroundOverlay)
        {
            ShowOverlay();
        }
        
        // 播放动画
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        if (useShowAnimation)
        {
            animationCoroutine = StartCoroutine(ShowAnimation(onComplete));
        }
        else
        {
            canvasGroup.alpha = 1f;
            transform.localScale = showScale;
            isAnimating = false;
            OnPanelShowComplete?.Invoke();
            onComplete?.Invoke();
        }
    }
    
    /// <summary>
    /// 隐藏面板
    /// </summary>
    public void Hide()
    {
        Hide(null);
    }
    
    /// <summary>
    /// 隐藏面板（带回调）
    /// </summary>
    public void Hide(System.Action onComplete)
    {
        if (!isShowing || isAnimating) return;
        
        isAnimating = true;
        
        OnPanelHide?.Invoke();
        
        // 隐藏背景遮罩
        if (useBackgroundOverlay)
        {
            HideOverlay();
        }
        
        // 播放动画
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        if (useHideAnimation)
        {
            animationCoroutine = StartCoroutine(HideAnimation(onComplete));
        }
        else
        {
            gameObject.SetActive(false);
            isShowing = false;
            isAnimating = false;
            SetRaycastState(false);
            OnPanelHideComplete?.Invoke();
            onComplete?.Invoke();
        }
    }
    
    /// <summary>
    /// 切换面板显示状态
    /// </summary>
    public void Toggle()
    {
        if (isShowing)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    
    /// <summary>
    /// 设置面板颜色
    /// </summary>
    public void SetPanelColor(Color color)
    {
        panelColor = color;
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
    }
    
    /// <summary>
    /// 设置面板透明度
    /// </summary>
    public void SetPanelAlpha(float alpha)
    {
        panelColor.a = alpha;
        if (backgroundImage != null)
        {
            backgroundImage.color = panelColor;
        }
    }
    
    /// <summary>
    /// 设置背景遮罩颜色
    /// </summary>
    public void SetOverlayColor(Color color)
    {
        overlayColor = color;
        if (backgroundOverlay != null)
        {
            backgroundOverlay.color = color;
        }
    }
    
    /// <summary>
    /// 设置动画类型
    /// </summary>
    public void SetAnimationType(PanelAnimationType type)
    {
        animationType = type;
    }
    
    /// <summary>
    /// 获取面板内的UI元素
    /// </summary>
    public T GetChildComponent<T>(string childName) where T : Component
    {
        Transform child = transform.Find(childName);
        if (child != null)
        {
            return child.GetComponent<T>();
        }
        return null;
    }
    
    /// <summary>
    /// 添加子元素到面板
    /// </summary>
    public Transform AddChild(GameObject childPrefab)
    {
        GameObject child = Instantiate(childPrefab, transform);
        CacheRaycastTargets();
        return child.transform;
    }
    
    /// <summary>
    /// 清空面板内容
    /// </summary>
    public void ClearChildren()
    {
        foreach (Transform child in transform)
        {
            if (child.name != "Background" && child.name != "Border")
            {
                Destroy(child.gameObject);
            }
        }
        CacheRaycastTargets();
    }
    
    private void CacheRaycastTargets()
    {
        raycastTargets.Clear();
        CanvasGroup[] groups = GetComponentsInChildren<CanvasGroup>(true);
        foreach (var group in groups)
        {
            if (group.blocksRaycasts)
            {
                raycastTargets.Add(group);
            }
        }
    }
    
    private void SetRaycastState(bool state)
    {
        if (!blockRaycasts) return;
        
        canvasGroup.blocksRaycasts = state;
        
        foreach (var group in raycastTargets)
        {
            if (group != canvasGroup)
            {
                group.blocksRaycasts = state;
            }
        }
    }
    
    private void ShowOverlay()
    {
        if (backgroundOverlay == null)
        {
            CreateOverlay();
        }
        
        backgroundOverlay.gameObject.SetActive(true);
        backgroundOverlay.CrossFadeAlpha(0f, 0f, true);
        backgroundOverlay.CrossFadeAlpha(1f, overlayFadeDuration, true);
    }
    
    private void HideOverlay()
    {
        if (backgroundOverlay != null)
        {
            backgroundOverlay.CrossFadeAlpha(0f, overlayFadeDuration, true);
        }
    }
    
    private void CreateOverlay()
    {
        GameObject overlayObj = new GameObject("BackgroundOverlay");
        overlayObj.transform.SetParent(transform);
        overlayObj.transform.SetSiblingIndex(0);
        
        RectTransform overlayRect = overlayObj.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayRect.anchoredPosition = Vector2.zero;
        
        backgroundOverlay = overlayObj.AddComponent<Image>();
        backgroundOverlay.color = overlayColor;
        backgroundOverlay.raycastTarget = closeOnClickOutside;
        
        // 添加点击事件
        backgroundOverlay.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
    }
    
    private void CheckClickOutside()
    {
        if (backgroundOverlay != null && backgroundOverlay.raycastTarget)
        {
            // 检测点击是否在面板外部
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, 
                Input.mousePosition, 
                null, 
                out localPoint
            );
            
            if (!rectTransform.rect.Contains(localPoint))
            {
                Hide();
            }
        }
    }
    
    private IEnumerator ShowAnimation(System.Action onComplete)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = GetTargetScale();
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curveValue = animationCurve.Evaluate(t);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, curveValue);
            transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
            
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        transform.localScale = targetScale;
        isAnimating = false;
        
        OnPanelShowComplete?.Invoke();
        onComplete?.Invoke();
    }
    
    private IEnumerator HideAnimation(System.Action onComplete)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = GetHideScale();
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curveValue = animationCurve.Evaluate(t);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, curveValue);
            transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
            
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        transform.localScale = targetScale;
        gameObject.SetActive(false);
        isShowing = false;
        isAnimating = false;
        SetRaycastState(false);
        
        OnPanelHideComplete?.Invoke();
        onComplete?.Invoke();
    }
    
    private Vector3 GetTargetScale()
    {
        switch (animationType)
        {
            case PanelAnimationType.Fade:
                return showScale;
            case PanelAnimationType.FadeScale:
            case PanelAnimationType.Bounce:
                return showScale;
            case PanelAnimationType.SlideLeft:
            case PanelAnimationType.SlideRight:
            case PanelAnimationType.SlideUp:
            case PanelAnimationType.SlideDown:
                return showScale;
            default:
                return showScale;
        }
    }
    
    private Vector3 GetHideScale()
    {
        switch (animationType)
        {
            case PanelAnimationType.Fade:
                return Vector3.one;
            case PanelAnimationType.FadeScale:
                return Vector3.zero;
            case PanelAnimationType.Bounce:
                return Vector3.zero;
            default:
                return Vector3.zero;
        }
    }
    
    private void ApplyVisualStyle()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = panelColor;
            if (backgroundSprite != null)
            {
                backgroundImage.sprite = backgroundSprite;
                backgroundImage.type = Image.Type.Sliced;
            }
        }
        
        if (borderImage != null)
        {
            borderImage.color = borderColor;
        }
    }
}
