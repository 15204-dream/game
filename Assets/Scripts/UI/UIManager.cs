using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 主UI管理器 - 管理所有UI元素和界面切换
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI管理器配置")]
    [SerializeField] private bool enableDebugLog = false;
    [SerializeField] private bool persistAcrossScenes = true;
    
    [Header("UI层级配置")]
    [SerializeField] private UILayers uiLayers;
    
    [Header("预制体引用")]
    [SerializeField] private GameObject dialogueBoxPrefab;
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private GameObject loadingScreenPrefab;
    
    [Header("UI实例引用")]
    [SerializeField] private UIDialogueBox currentDialogueBox;
    [SerializeField] private UIPopup currentPopup;
    [SerializeField] private UIPanel currentLoadingScreen;
    
    // 单例实例
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject uiManagerObj = new GameObject("UIManager");
                    instance = uiManagerObj.AddComponent<UIManager>();
                }
            }
            return instance;
        }
    }
    
    // UI缓存
    private Dictionary<string, UIPanel> panelCache = new Dictionary<string, UIPanel>();
    private Dictionary<string, GameObject> uiElementCache = new Dictionary<string, GameObject>();
    private Stack<UIPanel> panelStack = new Stack<UIPanel>();
    
    // 事件
    public System.Action<string> OnPanelOpened;
    public System.Action<string> OnPanelClosed;
    public System.Action OnAllPanelsClosed;
    
    // 属性
    public UILayers UILayers
    {
        get { return uiLayers; }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void Start()
    {
        // 监听场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        if (instance == this)
        {
            instance = null;
        }
    }
    
    /// <summary>
    /// 初始化UI管理器
    /// </summary>
    private void Initialize()
    {
        if (uiLayers == null)
        {
            CreateDefaultLayers();
        }
        
        Log("UI管理器初始化完成");
    }
    
    /// <summary>
    /// 创建默认层级
    /// </summary>
    private void CreateDefaultLayers()
    {
        GameObject layersObj = new GameObject("UILayers");
        layersObj.transform.SetParent(transform);
        uiLayers = layersObj.AddComponent<UILayers>();
    }
    
    /// <summary>
    /// 场景加载回调
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Log($"场景加载: {scene.name}, 模式: {mode}");
        
        // 清空面板栈
        panelStack.Clear();
        
        // 触发场景加载后的UI刷新
        RefreshUIPanels();
    }
    
    #region 对话框管理
    
    /// <summary>
    /// 显示对话框
    /// </summary>
    public UIDialogueBox ShowDialogue(string speakerName, string text, Sprite portrait = null)
    {
        if (currentDialogueBox == null)
        {
            CreateDialogueBox();
        }
        
        currentDialogueBox.ShowDialogue(speakerName, text, portrait);
        return currentDialogueBox;
    }
    
    /// <summary>
    /// 队列显示对话
    /// </summary>
    public void QueueDialogue(string speakerName, string text, Sprite portrait = null)
    {
        if (currentDialogueBox == null)
        {
            CreateDialogueBox();
        }
        
        currentDialogueBox.QueueDialogue(speakerName, text, portrait);
    }
    
    /// <summary>
    /// 隐藏对话框
    /// </summary>
    public void HideDialogue()
    {
        if (currentDialogueBox != null)
        {
            currentDialogueBox.Hide();
        }
    }
    
    /// <summary>
    /// 完成当前对话
    /// </summary>
    public void CompleteDialogue()
    {
        if (currentDialogueBox != null)
        {
            currentDialogueBox.CompleteCurrentDialogue();
        }
    }
    
    /// <summary>
    /// 创建对话框实例
    /// </summary>
    private void CreateDialogueBox()
    {
        if (dialogueBoxPrefab != null)
        {
            GameObject dialogueObj = Instantiate(dialogueBoxPrefab, transform);
            currentDialogueBox = dialogueObj.GetComponent<UIDialogueBox>();
        }
        else
        {
            GameObject dialogueObj = new GameObject("DialogueBox");
            dialogueObj.transform.SetParent(transform);
            currentDialogueBox = dialogueObj.AddComponent<UIDialogueBox>();
        }
    }
    
    #endregion
    
    #region 弹窗管理
    
    /// <summary>
    /// 显示消息弹窗
    /// </summary>
    public void ShowMessage(string title, string message)
    {
        ShowPopup(UIPopup.PopupType.Message, title, message, null, null, null);
    }
    
    /// <summary>
    /// 显示确认弹窗
    /// </summary>
    public void ShowConfirm(string title, string message, System.Action onConfirm, System.Action onCancel = null)
    {
        if (currentPopup != null && currentPopup.IsShowing)
        {
            currentPopup.Hide();
        }
        
        CreatePopup();
        currentPopup.ShowConfirm(title, message, onConfirm, onCancel);
    }
    
    /// <summary>
    /// 显示警告弹窗
    /// </summary>
    public void ShowWarning(string title, string message, System.Action onConfirm = null)
    {
        CreatePopup();
        currentPopup.ShowWarning(title, message, onConfirm);
    }
    
    /// <summary>
    /// 显示错误弹窗
    /// </summary>
    public void ShowError(string title, string message, System.Action onConfirm = null)
    {
        CreatePopup();
        currentPopup.ShowError(title, message, onConfirm);
    }
    
    /// <summary>
    /// 显示成功弹窗
    /// </summary>
    public void ShowSuccess(string title, string message, System.Action onConfirm = null)
    {
        CreatePopup();
        currentPopup.ShowSuccess(title, message, onConfirm);
    }
    
    /// <summary>
    /// 隐藏当前弹窗
    /// </summary>
    public void HidePopup()
    {
        if (currentPopup != null)
        {
            currentPopup.Hide();
        }
    }
    
    /// <summary>
    /// 创建弹窗实例
    /// </summary>
    private void ShowPopup(UIPopup.PopupType type, string title, string message, 
                          System.Action onConfirm, System.Action onCancel, System.Action onSecondary)
    {
        if (currentPopup != null && currentPopup.IsShowing)
        {
            currentPopup.Hide();
        }
        
        CreatePopup();
        
        switch (type)
        {
            case UIPopup.PopupType.Message:
                currentPopup.ShowMessage(title, message);
                break;
            case UIPopup.PopupType.Confirm:
                currentPopup.ShowConfirm(title, message, onConfirm, onCancel);
                break;
            case UIPopup.PopupType.Warning:
                currentPopup.ShowWarning(title, message, onConfirm);
                break;
            case UIPopup.PopupType.Error:
                currentPopup.ShowError(title, message, onConfirm);
                break;
            case UIPopup.PopupType.Success:
                currentPopup.ShowSuccess(title, message, onConfirm);
                break;
        }
    }
    
    private void CreatePopup()
    {
        if (currentPopup != null && currentPopup.IsShowing)
        {
            return;
        }
        
        if (popupPrefab != null)
        {
            GameObject popupObj = Instantiate(popupPrefab, transform);
            currentPopup = popupObj.GetComponent<UIPopup>();
        }
        else
        {
            GameObject popupObj = new GameObject("Popup");
            popupObj.transform.SetParent(transform);
            currentPopup = popupObj.AddComponent<UIPopup>();
        }
    }
    
    #endregion
    
    #region 面板管理
    
    /// <summary>
    /// 打开面板
    /// </summary>
    public void OpenPanel(string panelId)
    {
        OpenPanel(panelId, null);
    }
    
    /// <summary>
    /// 打开面板（带回调）
    /// </summary>
    public void OpenPanel(string panelId, System.Action onComplete)
    {
        if (panelCache.ContainsKey(panelId))
        {
            UIPanel panel = panelCache[panelId];
            panel.Show();
            panelStack.Push(panel);
            OnPanelOpened?.Invoke(panelId);
            onComplete?.Invoke();
        }
        else
        {
            Log($"面板未找到: {panelId}");
            onComplete?.Invoke();
        }
    }
    
    /// <summary>
    /// 关闭面板
    /// </summary>
    public void ClosePanel(string panelId)
    {
        if (panelCache.ContainsKey(panelId))
        {
            UIPanel panel = panelCache[panelId];
            panel.Hide();
            
            if (panelStack.Count > 0 && panelStack.Peek() == panel)
            {
                panelStack.Pop();
            }
            
            OnPanelClosed?.Invoke(panelId);
        }
    }
    
    /// <summary>
    /// 关闭当前面板
    /// </summary>
    public void CloseCurrentPanel()
    {
        if (panelStack.Count > 0)
        {
            UIPanel panel = panelStack.Pop();
            panel.Hide();
            OnPanelClosed?.Invoke(panel.PanelId);
        }
        else
        {
            OnAllPanelsClosed?.Invoke();
        }
    }
    
    /// <summary>
    /// 关闭所有面板
    /// </summary>
    public void CloseAllPanels()
    {
        foreach (var panel in panelCache.Values)
        {
            if (panel.IsShowing)
            {
                panel.Hide();
            }
        }
        
        panelStack.Clear();
        OnAllPanelsClosed?.Invoke();
    }
    
    /// <summary>
    /// 注册面板
    /// </summary>
    public void RegisterPanel(UIPanel panel)
    {
        if (panel == null) return;
        
        string panelId = panel.PanelId;
        if (string.IsNullOrEmpty(panelId))
        {
            panelId = panel.gameObject.name;
            panel.PanelId = panelId;
        }
        
        if (!panelCache.ContainsKey(panelId))
        {
            panelCache[panelId] = panel;
            Log($"面板注册: {panelId}");
        }
    }
    
    /// <summary>
    /// 注销面板
    /// </summary>
    public void UnregisterPanel(string panelId)
    {
        if (panelCache.ContainsKey(panelId))
        {
            panelCache.Remove(panelId);
            Log($"面板注销: {panelId}");
        }
    }
    
    /// <summary>
    /// 获取面板
    /// </summary>
    public UIPanel GetPanel(string panelId)
    {
        if (panelCache.ContainsKey(panelId))
        {
            return panelCache[panelId];
        }
        return null;
    }
    
    /// <summary>
    /// 刷新UI面板
    /// </summary>
    private void RefreshUIPanels()
    {
        UIPanel[] panels = FindObjectsOfType<UIPanel>();
        foreach (var panel in panels)
        {
            RegisterPanel(panel);
        }
    }
    
    #endregion
    
    #region 加载界面
    
    /// <summary>
    /// 显示加载界面
    /// </summary>
    public void ShowLoadingScreen(System.Action onComplete = null)
    {
        if (currentLoadingScreen != null)
        {
            currentLoadingScreen.Show(onComplete);
        }
        else
        {
            CreateLoadingScreen(onComplete);
        }
    }
    
    /// <summary>
    /// 隐藏加载界面
    /// </summary>
    public void HideLoadingScreen(System.Action onComplete = null)
    {
        if (currentLoadingScreen != null)
        {
            currentLoadingScreen.Hide(onComplete);
        }
        else
        {
            onComplete?.Invoke();
        }
    }
    
    /// <summary>
    /// 创建加载界面
    /// </summary>
    private void CreateLoadingScreen(System.Action onComplete)
    {
        if (loadingScreenPrefab != null)
        {
            GameObject loadingObj = Instantiate(loadingScreenPrefab, transform);
            currentLoadingScreen = loadingObj.GetComponent<UIPanel>();
            currentLoadingScreen.Show(onComplete);
        }
        else
        {
            onComplete?.Invoke();
        }
    }
    
    #endregion
    
    #region UI动画
    
    /// <summary>
    /// 淡入UI元素
    /// </summary>
    public void FadeIn(GameObject target, float duration = 0.3f, System.Action onComplete = null)
    {
        StartCoroutine(UITween.FadeCanvasGroup(target, 0f, 1f, duration, onComplete));
    }
    
    /// <summary>
    /// 淡出UI元素
    /// </summary>
    public void FadeOut(GameObject target, float duration = 0.3f, System.Action onComplete = null)
    {
        StartCoroutine(UITween.FadeCanvasGroup(target, 1f, 0f, duration, onComplete));
    }
    
    /// <summary>
    /// 移动UI元素
    /// </summary>
    public void MoveTo(GameObject target, Vector3 targetPosition, float duration = 0.3f, 
                      System.Action onComplete = null)
    {
        StartCoroutine(UITween.MoveTo(target, targetPosition, duration, onComplete));
    }
    
    /// <summary>
    /// 缩放UI元素
    /// </summary>
    public void ScaleTo(GameObject target, Vector3 targetScale, float duration = 0.3f,
                       System.Action onComplete = null)
    {
        StartCoroutine(UITween.ScaleTo(target, targetScale, duration, onComplete));
    }
    
    #endregion
    
    /// <summary>
    /// 日志输出
    /// </summary>
    private void Log(string message)
    {
        if (enableDebugLog)
        {
            Debug.Log($"[UIManager] {message}");
        }
    }
}
