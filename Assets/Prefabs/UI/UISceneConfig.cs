using UnityEngine;

/// <summary>
/// UI场景配置 - 定义UI场景的初始设置
/// </summary>
[CreateAssetMenu(fileName = "UISceneConfig", menuName = "UI/Create Scene Config")]
public class UISceneConfig : ScriptableObject
{
    [Header("场景信息")]
    [SerializeField] private string sceneName = "UI";
    [SerializeField] private string sceneDescription = "UI主场景";
    
    [Header("预设UI组件")]
    [SerializeField] private GameObject uiManagerPrefab;
    [SerializeField] private GameObject mainMenuPrefab;
    [SerializeField] private GameObject gameHUDPrefab;
    [SerializeField] private GameObject settingsPanelPrefab;
    
    [Header("场景层级配置")]
    [SerializeField] private UILayerConfig[] layerConfigs;
    
    [Header("自动初始化配置")]
    [SerializeField] private bool autoInitializeUIManager = true;
    [SerializeField] private bool autoInitializeHUD = true;
    [SerializeField] private bool loadSavedTheme = true;
    
    [Header("音频配置")]
    [SerializeField] private AudioClip menuBGM;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip confirmSound;
    [SerializeField] private AudioClip cancelSound;
    
    [Header("视觉效果配置")]
    [SerializeField] private bool enableParticleEffects = true;
    [SerializeField] private bool enableFloatingHearts = true;
    [SerializeField] private int floatingHeartCount = 10;
    [SerializeField] private bool enableBackgroundAnimation = true;
    [SerializeField] private float backgroundChangeInterval = 10f;
    
    // 属性访问器
    public string SceneName => sceneName;
    public string SceneDescription => sceneDescription;
    
    public GameObject UIManagerPrefab => uiManagerPrefab;
    public GameObject MainMenuPrefab => mainMenuPrefab;
    public GameObject GameHUDPrefab => gameHUDPrefab;
    public GameObject SettingsPanelPrefab => settingsPanelPrefab;
    
    public bool AutoInitializeUIManager => autoInitializeUIManager;
    public bool AutoInitializeHUD => autoInitializeHUD;
    public bool LoadSavedTheme => loadSavedTheme;
    
    public AudioClip MenuBGM => menuBGM;
    public AudioClip ButtonClickSound => buttonClickSound;
    public AudioClip ConfirmSound => confirmSound;
    public AudioClip CancelSound => cancelSound;
    
    public bool EnableParticleEffects => enableParticleEffects;
    public bool EnableFloatingHearts => enableFloatingHearts;
    public int FloatingHeartCount => floatingHeartCount;
    public bool EnableBackgroundAnimation => enableBackgroundAnimation;
    public float BackgroundChangeInterval => backgroundChangeInterval;
    
    /// <summary>
    /// 获取层级配置
    /// </summary>
    public UILayerConfig[] GetLayerConfigs()
    {
        return layerConfigs;
    }
    
    /// <summary>
    /// 获取特定层级的配置
    /// </summary>
    public UILayerConfig GetLayerConfig(string layerName)
    {
        if (layerConfigs == null) return null;
        
        foreach (var config in layerConfigs)
        {
            if (config != null && config.LayerName == layerName)
            {
                return config;
            }
        }
        return null;
    }
}

/// <summary>
/// UI层级配置
/// </summary>
[System.Serializable]
public class UILayerConfig
{
    [SerializeField] private string layerName = "Default";
    [SerializeField] private int sortingOrder = 0;
    [SerializeField] private bool blockRaycasts = true;
    [SerializeField] private Color overlayColor = Color.clear;
    [SerializeField] private bool useOverlay = false;
    
    public string LayerName => layerName;
    public int SortingOrder => sortingOrder;
    public bool BlockRaycasts => blockRaycasts;
    public Color OverlayColor => overlayColor;
    public bool UseOverlay => useOverlay;
}

/// <summary>
/// UI场景初始化器 - 在场景加载时自动设置UI
/// </summary>
public class UISceneInitializer : MonoBehaviour
{
    [Header("场景配置")]
    [SerializeField] private UISceneConfig sceneConfig;
    
    [Header("自动创建配置")]
    [SerializeField] private bool autoCreateOnStart = true;
    
    private void Start()
    {
        if (autoCreateOnStart)
        {
            InitializeScene();
        }
    }
    
    /// <summary>
    /// 初始化场景
    /// </summary>
    public void InitializeScene()
    {
        if (sceneConfig == null)
        {
            Debug.LogWarning("UISceneConfig未设置，使用默认配置");
            CreateDefaultUIElements();
        }
        else
        {
            CreateUIElementsFromConfig();
        }
    }
    
    /// <summary>
    /// 从配置创建UI元素
    /// </summary>
    private void CreateUIElementsFromConfig()
    {
        // 创建UIManager
        if (sceneConfig.AutoInitializeUIManager)
        {
            if (sceneConfig.UIManagerPrefab != null)
            {
                GameObject uiManager = Instantiate(sceneConfig.UIManagerPrefab, transform);
                uiManager.name = "UIManager";
            }
            else
            {
                GameObject uiManager = new GameObject("UIManager");
                uiManager.transform.SetParent(transform);
                uiManager.AddComponent<UIManager>();
            }
        }
        
        // 创建HUD
        if (sceneConfig.AutoInitializeHUD)
        {
            if (sceneConfig.GameHUDPrefab != null)
            {
                GameObject hud = Instantiate(sceneConfig.GameHUDPrefab, transform);
                hud.name = "GameHUD";
            }
            else
            {
                // 不自动创建，让游戏逻辑控制
            }
        }
        
        // 加载主题
        if (sceneConfig.LoadSavedTheme)
        {
            ThemeManager themeManager = FindObjectOfType<ThemeManager>();
            if (themeManager == null)
            {
                GameObject themeObj = new GameObject("ThemeManager");
                themeManager = themeObj.AddComponent<ThemeManager>();
            }
        }
        
        Debug.Log($"UI场景 [{sceneConfig.SceneName}] 初始化完成");
    }
    
    /// <summary>
    /// 创建默认UI元素
    /// </summary>
    private void CreateDefaultUIElements()
    {
        // 创建UIManager
        GameObject uiManager = new GameObject("UIManager");
        uiManager.transform.SetParent(transform);
        uiManager.AddComponent<UIManager>();
        
        // 创建ThemeManager
        GameObject themeManager = new GameObject("ThemeManager");
        themeManager.transform.SetParent(transform);
        themeManager.AddComponent<ThemeManager>();
        
        Debug.Log("UI场景使用默认配置初始化完成");
    }
    
    /// <summary>
    /// 获取场景配置
    /// </summary>
    public UISceneConfig GetConfig()
    {
        return sceneConfig;
    }
    
    /// <summary>
    /// 设置场景配置
    /// </summary>
    public void SetConfig(UISceneConfig config)
    {
        sceneConfig = config;
    }
}

/// <summary>
/// UI预制体引用配置
/// </summary>
[System.Serializable]
public class UIPrefabReference
{
    public string prefabName;
    public GameObject prefab;
    public bool autoInstantiate = true;
    public Transform parentTransform;
    
    /// <summary>
    /// 实例化预制体
    /// </summary>
    public GameObject Instantiate()
    {
        if (prefab == null)
        {
            Debug.LogWarning($"预制体 [{prefabName}] 未设置");
            return null;
        }
        
        Transform parent = parentTransform != null ? parentTransform : null;
        GameObject instance = Instantiate(prefab, parent);
        instance.name = prefabName;
        
        return instance;
    }
}

/// <summary>
/// UI预制体库
/// </summary>
[CreateAssetMenu(fileName = "UIPrefabLibrary", menuName = "UI/Create Prefab Library")]
public class UIPrefabLibrary : ScriptableObject
{
    [Header("UI组件预制体")]
    [SerializeField] private UIPrefabReference[] uiComponents;
    
    [Header("面板预制体")]
    [SerializeField] private UIPrefabReference[] panels;
    
    [Header("HUD预制体")]
    [SerializeField] private UIPrefabReference[] hudElements;
    
    [Header("特效预制体")]
    [SerializeField] private UIPrefabReference[] effects;
    
    /// <summary>
    /// 获取UI组件预制体
    /// </summary>
    public UIPrefabReference[] GetUIComponents()
    {
        return uiComponents;
    }
    
    /// <summary>
    /// 获取面板预制体
    /// </summary>
    public UIPrefabReference[] GetPanels()
    {
        return panels;
    }
    
    /// <summary>
    /// 获取HUD预制体
    /// </summary>
    public UIPrefabReference[] GetHUDElements()
    {
        return hudElements;
    }
    
    /// <summary>
    /// 获取特效预制体
    /// </summary>
    public UIPrefabReference[] GetEffects()
    {
        return effects;
    }
    
    /// <summary>
    /// 根据名称获取预制体
    /// </summary>
    public GameObject GetPrefabByName(string name)
    {
        // 搜索UI组件
        if (uiComponents != null)
        {
            foreach (var prefab in uiComponents)
            {
                if (prefab != null && prefab.prefabName == name)
                {
                    return prefab.prefab;
                }
            }
        }
        
        // 搜索面板
        if (panels != null)
        {
            foreach (var prefab in panels)
            {
                if (prefab != null && prefab.prefabName == name)
                {
                    return prefab.prefab;
                }
            }
        }
        
        // 搜索HUD
        if (hudElements != null)
        {
            foreach (var prefab in hudElements)
            {
                if (prefab != null && prefab.prefabName == name)
                {
                    return prefab.prefab;
                }
            }
        }
        
        // 搜索特效
        if (effects != null)
        {
            foreach (var prefab in effects)
            {
                if (prefab != null && prefab.prefabName == name)
                {
                    return prefab.prefab;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// 实例化所有设置为自动实例化的预制体
    /// </summary>
    public void InstantiateAllAutoPrefabs(Transform parent)
    {
        // 实例化UI组件
        if (uiComponents != null)
        {
            foreach (var prefab in uiComponents)
            {
                if (prefab != null && prefab.autoInstantiate)
                {
                    prefab.parentTransform = parent;
                    prefab.Instantiate();
                }
            }
        }
    }
}
