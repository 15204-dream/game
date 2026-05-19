using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI层级管理器 - 管理UI的显示层级和遮罩
/// </summary>
public class UILayers : MonoBehaviour
{
    [Header("层级配置")]
    [SerializeField] private List<UILayerConfig> layerConfigs = new List<UILayerConfig>();
    
    // 层级容器
    private Dictionary<UILayerType, Transform> layerContainers = new Dictionary<UILayerType, Transform>();
    private Dictionary<string, UILayerType> elementLayers = new Dictionary<string, UILayerType>();
    
    // 遮罩层
    private UILayerOverlay currentOverlay;
    
    // 层级类型枚举
    public enum UILayerType
    {
        Background,      // 背景层
        MainUI,          // 主UI层
        HUD,             // HUD层
        Dialogue,        // 对话框层
        Popup,           // 弹窗层
        Modal,           // 模态层
        Loading,         // 加载层
        Debug,           // 调试层
        Camera          // 相机UI层
    }
    
    // 层级配置
    [System.Serializable]
    public class UILayerConfig
    {
        public UILayerType layerType;
        public string layerName;
        public int sortingOrder;
        public bool blockRaycasts;
        public Color overlayColor;
        public bool useOverlay;
    }
    
    private void Awake()
    {
        InitializeLayers();
    }
    
    /// <summary>
    /// 初始化所有层级
    /// </summary>
    private void InitializeLayers()
    {
        // 如果没有配置，使用默认配置
        if (layerConfigs.Count == 0)
        {
            CreateDefaultLayerConfigs();
        }
        
        // 创建层级容器
        foreach (var config in layerConfigs)
        {
            CreateLayerContainer(config);
        }
        
        // 设置默认层级
        SetDefaultSortingOrders();
    }
    
    /// <summary>
    /// 创建默认层级配置
    /// </summary>
    private void CreateDefaultLayerConfigs()
    {
        layerConfigs = new List<UILayerConfig>
        {
            new UILayerConfig
            {
                layerType = UILayerType.Background,
                layerName = "Background",
                sortingOrder = 0,
                blockRaycasts = false,
                overlayColor = Color.clear,
                useOverlay = false
            },
            new UILayerConfig
            {
                layerType = UILayerType.MainUI,
                layerName = "MainUI",
                sortingOrder = 100,
                blockRaycasts = true,
                overlayColor = Color.clear,
                useOverlay = false
            },
            new UILayerConfig
            {
                layerType = UILayerType.HUD,
                layerName = "HUD",
                sortingOrder = 200,
                blockRaycasts = false,
                overlayColor = Color.clear,
                useOverlay = false
            },
            new UILayerConfig
            {
                layerType = UILayerType.Dialogue,
                layerName = "Dialogue",
                sortingOrder = 300,
                blockRaycasts = true,
                overlayColor = new Color(0f, 0f, 0f, 0.3f),
                useOverlay = false
            },
            new UILayerConfig
            {
                layerType = UILayerType.Popup,
                layerName = "Popup",
                sortingOrder = 400,
                blockRaycasts = true,
                overlayColor = new Color(0f, 0f, 0f, 0.5f),
                useOverlay = true
            },
            new UILayerConfig
            {
                layerType = UILayerType.Modal,
                layerName = "Modal",
                sortingOrder = 500,
                blockRaycasts = true,
                overlayColor = new Color(0f, 0f, 0f, 0.7f),
                useOverlay = true
            },
            new UILayerConfig
            {
                layerType = UILayerType.Loading,
                layerName = "Loading",
                sortingOrder = 600,
                blockRaycasts = true,
                overlayColor = new Color(1f, 0.95f, 0.95f, 0.95f),
                useOverlay = true
            },
            new UILayerConfig
            {
                layerType = UILayerType.Debug,
                layerName = "Debug",
                sortingOrder = 700,
                blockRaycasts = false,
                overlayColor = Color.clear,
                useOverlay = false
            },
            new UILayerConfig
            {
                layerType = UILayerType.Camera,
                layerName = "Camera",
                sortingOrder = 800,
                blockRaycasts = false,
                overlayColor = Color.clear,
                useOverlay = false
            }
        };
    }
    
    /// <summary>
    /// 创建层级容器
    /// </summary>
    private void CreateLayerContainer(UILayerConfig config)
    {
        GameObject layerObj = new GameObject(config.layerName);
        layerObj.transform.SetParent(transform);
        
        RectTransform rect = layerObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        
        // 设置Canvas
        Canvas canvas = layerObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = config.sortingOrder;
        canvas.overrideSorting = true;
        
        CanvasScaler scaler = layerObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        GraphicRaycaster raycaster = layerObj.AddComponent<GraphicRaycaster>();
        raycaster.ignoreReversedGraphics = true;
        raycaster.blockingObjects = config.blockRaycasts ? GraphicRaycaster.BlockingObjects.All : GraphicRaycaster.BlockingObjects.None;
        
        layerContainers[config.layerType] = layerObj.transform;
    }
    
    /// <summary>
    /// 设置默认排序顺序
    /// </summary>
    private void SetDefaultSortingOrders()
    {
        foreach (var config in layerConfigs)
        {
            if (layerContainers.ContainsKey(config.layerType))
            {
                Canvas canvas = layerContainers[config.layerType].GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.sortingOrder = config.sortingOrder;
                }
            }
        }
    }
    
    /// <summary>
    /// 获取层级容器
    /// </summary>
    public Transform GetLayerContainer(UILayerType layerType)
    {
        if (layerContainers.ContainsKey(layerType))
        {
            return layerContainers[layerType];
        }
        return transform;
    }
    
    /// <summary>
    /// 将UI元素添加到指定层级
    /// </summary>
    public void AddToLayer(GameObject uiElement, UILayerType layerType)
    {
        if (uiElement == null) return;
        
        Transform container = GetLayerContainer(layerType);
        if (container != null)
        {
            uiElement.transform.SetParent(container);
            elementLayers[uiElement.name] = layerType;
        }
    }
    
    /// <summary>
    /// 将UI元素移动到指定层级
    /// </summary>
    public void MoveToLayer(GameObject uiElement, UILayerType layerType)
    {
        if (uiElement == null) return;
        
        Transform container = GetLayerContainer(layerType);
        if (container != null)
        {
            uiElement.transform.SetParent(container);
            elementLayers[uiElement.name] = layerType;
        }
    }
    
    /// <summary>
    /// 获取元素所在层级
    /// </summary>
    public UILayerType GetElementLayer(string elementName)
    {
        if (elementLayers.ContainsKey(elementName))
        {
            return elementLayers[elementName];
        }
        return UILayerType.MainUI;
    }
    
    /// <summary>
    /// 设置层级排序顺序
    /// </summary>
    public void SetLayerSortingOrder(UILayerType layerType, int order)
    {
        if (layerContainers.ContainsKey(layerType))
        {
            Canvas canvas = layerContainers[layerType].GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = order;
            }
        }
    }
    
    /// <summary>
    /// 显示层级遮罩
    /// </summary>
    public void ShowOverlay(UILayerType layerType)
    {
        if (layerContainers.ContainsKey(layerType))
        {
            Transform container = layerContainers[layerType];
            
            // 查找或创建遮罩
            Transform overlayTransform = container.Find("Overlay");
            if (overlayTransform == null)
            {
                GameObject overlayObj = new GameObject("Overlay");
                overlayObj.transform.SetParent(container);
                
                RectTransform rect = overlayObj.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
                
                Image overlayImage = overlayObj.AddComponent<Image>();
                
                // 获取层级配置的颜色
                var config = layerConfigs.Find(c => c.layerType == layerType);
                if (config != null)
                {
                    overlayImage.color = config.overlayColor;
                }
                
                overlayTransform = overlayObj.transform;
            }
            
            overlayTransform.gameObject.SetActive(true);
            currentOverlay = overlayTransform.GetComponent<UILayerOverlay>();
        }
    }
    
    /// <summary>
    /// 隐藏层级遮罩
    /// </summary>
    public void HideOverlay(UILayerType layerType)
    {
        if (layerContainers.ContainsKey(layerType))
        {
            Transform container = layerContainers[layerType];
            Transform overlayTransform = container.Find("Overlay");
            if (overlayTransform != null)
            {
                overlayTransform.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 隐藏所有遮罩
    /// </summary>
    public void HideAllOverlays()
    {
        foreach (var container in layerContainers.Values)
        {
            Transform overlayTransform = container.Find("Overlay");
            if (overlayTransform != null)
            {
                overlayTransform.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 获取所有层级配置
    /// </summary>
    public List<UILayerConfig> GetLayerConfigs()
    {
        return layerConfigs;
    }
    
    /// <summary>
    /// 设置层级配置
    /// </summary>
    public void SetLayerConfig(UILayerType layerType, UILayerConfig config)
    {
        int index = layerConfigs.FindIndex(c => c.layerType == layerType);
        if (index >= 0)
        {
            layerConfigs[index] = config;
            
            // 更新层级容器设置
            if (layerContainers.ContainsKey(layerType))
            {
                Transform container = layerContainers[layerType];
                Canvas canvas = container.GetComponent<Canvas>();
                GraphicRaycaster raycaster = container.GetComponent<GraphicRaycaster>();
                
                if (canvas != null)
                {
                    canvas.sortingOrder = config.sortingOrder;
                }
                
                if (raycaster != null)
                {
                    raycaster.blockingObjects = config.blockRaycasts ? GraphicRaycaster.BlockingObjects.All : GraphicRaycaster.BlockingObjects.None;
                }
            }
        }
    }
    
    /// <summary>
    /// 刷新所有层级
    /// </summary>
    public void RefreshLayers()
    {
        SetDefaultSortingOrders();
    }
    
    /// <summary>
    /// 获取当前层级数量
    /// </summary>
    public int GetLayerCount()
    {
        return layerConfigs.Count;
    }
    
    /// <summary>
    /// 获取最高层级
    /// </summary>
    public UILayerType GetTopLayer()
    {
        if (layerConfigs.Count == 0) return UILayerType.MainUI;
        
        UILayerConfig topConfig = layerConfigs[0];
        foreach (var config in layerConfigs)
        {
            if (config.sortingOrder > topConfig.sortingOrder)
            {
                topConfig = config;
            }
        }
        return topConfig.layerType;
    }
    
    /// <summary>
    /// 获取特定层级的排序顺序
    /// </summary>
    public int GetLayerSortingOrder(UILayerType layerType)
    {
        var config = layerConfigs.Find(c => c.layerType == layerType);
        return config != null ? config.sortingOrder : 0;
    }
}

/// <summary>
/// UI层级遮罩组件
/// </summary>
public class UILayerOverlay : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.2f;
    
    private UnityEngine.UI.Image overlayImage;
    private Color originalColor;
    
    private void Awake()
    {
        overlayImage = GetComponent<UnityEngine.UI.Image>();
        if (overlayImage != null)
        {
            originalColor = overlayImage.color;
        }
    }
    
    /// <summary>
    /// 淡入遮罩
    /// </summary>
    public void FadeIn(System.Action onComplete = null)
    {
        if (overlayImage == null) return;
        
        overlayImage.gameObject.SetActive(true);
        StartCoroutine(FadeColor(Color.clear, originalColor, onComplete));
    }
    
    /// <summary>
    /// 淡出遮罩
    /// </summary>
    public void FadeOut(System.Action onComplete = null)
    {
        if (overlayImage == null) return;
        
        StartCoroutine(FadeColor(originalColor, Color.clear, () =>
        {
            overlayImage.gameObject.SetActive(false);
            onComplete?.Invoke();
        }));
    }
    
    private System.Collections.IEnumerator FadeColor(Color from, Color to, System.Action onComplete)
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            
            overlayImage.color = Color.Lerp(from, to, t);
            yield return null;
        }
        
        overlayImage.color = to;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 设置遮罩颜色
    /// </summary>
    public void SetColor(Color color)
    {
        if (overlayImage != null)
        {
            overlayImage.color = color;
            originalColor = color;
        }
    }
}
