using UnityEngine;

/// <summary>
/// UI组件预制体配置 - 提供预制体创建指南
/// </summary>
public class UIPrefabSetup : MonoBehaviour
{
    [Header("组件预制体引用")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject progressBarPrefab;
    [SerializeField] private GameObject dialogueBoxPrefab;
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private GameObject popupPrefab;
    
    [Header("面板预制体引用")]
    [SerializeField] private GameObject mainMenuPrefab;
    [SerializeField] private GameObject settingsPanelPrefab;
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject inventoryPanelPrefab;
    [SerializeField] private GameObject relationshipPanelPrefab;
    
    [Header("HUD预制体引用")]
    [SerializeField] private GameObject gameHUDPrefab;
    [SerializeField] private GameObject modeIndicatorPrefab;
    [SerializeField] private GameObject timeDisplayPrefab;
    [SerializeField] private GameObject currencyDisplayPrefab;
    
    /// <summary>
    /// 创建按钮预制体
    /// </summary>
    public static void CreateButtonPrefab()
    {
        GameObject buttonObj = new GameObject("UIButton");
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 60);
        rect.anchoredPosition = Vector2.zero;
        
        UnityEngine.UI.Image bgImage = buttonObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(1f, 0.8f, 0.85f, 1f);
        
        UIButton button = buttonObj.AddComponent<UIButton>();
        button.backgroundImage = bgImage;
        
        // 添加文字子对象
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = Vector2.zero;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TMPro.TextMeshProUGUI text = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = "按钮";
        text.fontSize = 24;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.color = new Color(0.3f, 0.2f, 0.25f, 1f);
        
        button.buttonText = text;
        
        Debug.Log("按钮预制体创建完成，请在编辑器中保存为Prefab");
    }
    
    /// <summary>
    /// 创建进度条预制体
    /// </summary>
    public static void CreateProgressBarPrefab()
    {
        GameObject progressBarObj = new GameObject("UIProgressBar");
        
        RectTransform rect = progressBarObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 30);
        rect.anchoredPosition = Vector2.zero;
        
        // 背景
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(progressBarObj.transform);
        
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        UnityEngine.UI.Image bgImage = bgObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(1f, 0.85f, 0.9f, 0.3f);
        
        // 填充
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(progressBarObj.transform);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        UnityEngine.UI.Image fillImage = fillObj.AddComponent<UnityEngine.UI.Image>();
        fillImage.color = new Color(1f, 0.6f, 0.7f, 1f);
        fillImage.type = UnityEngine.UI.Image.Type.Filled;
        fillImage.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
        fillImage.fillAmount = 0.5f;
        
        UIProgressBar progressBar = progressBarObj.AddComponent<UIProgressBar>();
        progressBar.backgroundImage = bgImage;
        progressBar.fillImage = fillImage;
        
        Debug.Log("进度条预制体创建完成，请在编辑器中保存为Prefab");
    }
    
    /// <summary>
    /// 创建对话框预制体
    /// </summary>
    public static void CreateDialogueBoxPrefab()
    {
        GameObject dialogueObj = new GameObject("UIDialogueBox");
        
        RectTransform rect = dialogueObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(800, 200);
        rect.anchoredPosition = new Vector2(0, -200);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 0);
        
        // 背景
        UnityEngine.UI.Image bgImage = dialogueObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(1f, 0.95f, 0.95f, 0.95f);
        
        UIDialogueBox dialogueBox = dialogueObj.AddComponent<UIDialogueBox>();
        dialogueBox.backgroundImage = bgImage;
        
        // 名称标签
        GameObject nameObj = new GameObject("NameTag");
        nameObj.transform.SetParent(dialogueObj.transform);
        
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.sizeDelta = new Vector2(150, 40);
        nameRect.anchoredPosition = new Vector2(-325, 80);
        
        TMPro.TextMeshProUGUI nameText = nameObj.AddComponent<TMPro.TextMeshProUGUI>();
        nameText.text = "角色名";
        nameText.fontSize = 26;
        nameText.color = new Color(1f, 0.4f, 0.6f, 1f);
        
        dialogueBox.speakerNameText = nameText;
        
        // 对话文本
        GameObject textObj = new GameObject("DialogueText");
        textObj.transform.SetParent(dialogueObj.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(750, 150);
        textRect.anchoredPosition = new Vector2(0, -10);
        
        TMPro.TextMeshProUGUI dialogueText = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        dialogueText.text = "这里是对话内容...";
        dialogueText.fontSize = 24;
        dialogueText.color = new Color(0.3f, 0.2f, 0.25f, 1f);
        dialogueText.alignment = TMPro.TextAlignmentOptions.TopLeft;
        
        dialogueBox.dialogueText = dialogueText;
        
        Debug.Log("对话框预制体创建完成，请在编辑器中保存为Prefab");
    }
    
    /// <summary>
    /// 创建面板预制体
    /// </summary>
    public static void CreatePanelPrefab()
    {
        GameObject panelObj = new GameObject("UIPanel");
        
        RectTransform rect = panelObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500, 400);
        rect.anchoredPosition = Vector2.zero;
        
        UnityEngine.UI.Image bgImage = panelObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(1f, 0.98f, 0.98f, 0.95f);
        
        UIPanel panel = panelObj.AddComponent<UIPanel>();
        panel.backgroundImage = bgImage;
        
        Debug.Log("面板预制体创建完成，请在编辑器中保存为Prefab");
    }
    
    /// <summary>
    /// 创建弹窗预制体
    /// </summary>
    public static void CreatePopupPrefab()
    {
        GameObject popupObj = new GameObject("UIPopup");
        
        RectTransform rect = popupObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(450, 300);
        rect.anchoredPosition = Vector2.zero;
        
        // 背景
        UnityEngine.UI.Image bgImage = popupObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(1f, 0.97f, 0.97f, 0.98f);
        
        UIPopup popup = popupObj.AddComponent<UIPopup>();
        popup.backgroundImage = bgImage;
        
        // 标题
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(popupObj.transform);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(400, 50);
        titleRect.anchoredPosition = new Vector2(0, 100);
        
        TMPro.TextMeshProUGUI titleText = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
        titleText.text = "标题";
        titleText.fontSize = 32;
        titleText.color = new Color(1f, 0.4f, 0.6f, 1f);
        titleText.alignment = TMPro.TextAlignmentOptions.Center;
        
        popup.titleText = titleText;
        
        // 消息
        GameObject messageObj = new GameObject("Message");
        messageObj.transform.SetParent(popupObj.transform);
        
        RectTransform messageRect = messageObj.AddComponent<RectTransform>();
        messageRect.sizeDelta = new Vector2(400, 150);
        messageRect.anchoredPosition = new Vector2(0, 20);
        
        TMPro.TextMeshProUGUI messageText = messageObj.AddComponent<TMPro.TextMeshProUGUI>();
        messageText.text = "消息内容";
        messageText.fontSize = 24;
        messageText.color = new Color(0.3f, 0.2f, 0.25f, 1f);
        messageText.alignment = TMPro.TextAlignmentOptions.Center;
        
        popup.messageText = messageText;
        
        Debug.Log("弹窗预制体创建完成，请在编辑器中保存为Prefab");
    }
    
    /// <summary>
    /// 创建模式指示器预制体
    /// </summary>
    public static void CreateModeIndicatorPrefab()
    {
        GameObject indicatorObj = new GameObject("ModeIndicator");
        
        RectTransform rect = indicatorObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 40);
        rect.anchoredPosition = new Vector2(0, 0);
        
        // 背景
        UnityEngine.UI.Image bgImage = indicatorObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(1f, 0.7f, 0.8f, 1f);
        
        ModeIndicator indicator = indicatorObj.AddComponent<ModeIndicator>();
        indicator.backgroundImage = bgImage;
        
        // 图标
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(indicatorObj.transform);
        
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(30, 30);
        iconRect.anchoredPosition = new Vector2(-40, 0);
        
        UnityEngine.UI.Image iconImage = iconObj.AddComponent<UnityEngine.UI.Image>();
        
        indicator.iconImage = iconImage;
        
        // 文字
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(indicatorObj.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(70, 30);
        textRect.anchoredPosition = new Vector2(10, 0);
        
        TMPro.TextMeshProUGUI modeText = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        modeText.text = "嘉宾";
        modeText.fontSize = 18;
        modeText.color = Color.white;
        modeText.alignment = TMPro.TextAlignmentOptions.Center;
        
        indicator.modeText = modeText;
        
        Debug.Log("模式指示器预制体创建完成，请在编辑器中保存为Prefab");
    }
    
    /// <summary>
    /// 创建时间显示预制体
    /// </summary>
    public static void CreateTimeDisplayPrefab()
    {
        GameObject timeObj = new GameObject("TimeDisplay");
        
        RectTransform rect = timeObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 40);
        rect.anchoredPosition = new Vector2(0, 0);
        
        TimeDisplay timeDisplay = timeObj.AddComponent<TimeDisplay>();
        
        // 时间文本
        TMPro.TextMeshProUGUI timeText = timeObj.AddComponent<TMPro.TextMeshProUGUI>();
        timeText.text = "12:00";
        timeText.fontSize = 28;
        timeText.color = new Color(1f, 0.6f, 0.7f, 1f);
        timeText.alignment = TMPro.TextAlignmentOptions.Center;
        
        timeDisplay.timeText = timeText;
        
        Debug.Log("时间显示预制体创建完成，请在编辑器中保存为Prefab");
    }
    
    /// <summary>
    /// 创建货币显示预制体
    /// </summary>
    public static void CreateCurrencyDisplayPrefab()
    {
        GameObject currencyObj = new GameObject("CurrencyDisplay");
        
        RectTransform rect = currencyObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 40);
        rect.anchoredPosition = new Vector2(0, 0);
        
        CurrencyDisplay currencyDisplay = currencyObj.AddComponent<CurrencyDisplay>();
        
        // 图标
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(currencyObj.transform);
        
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(30, 30);
        iconRect.anchoredPosition = new Vector2(-40, 0);
        
        UnityEngine.UI.Image iconImage = iconObj.AddComponent<UnityEngine.UI.Image>();
        iconImage.color = new Color(1f, 0.6f, 0.7f, 1f);
        
        currencyDisplay.currencyIcon = iconImage;
        
        // 金额文本
        GameObject amountObj = new GameObject("Amount");
        amountObj.transform.SetParent(currencyObj.transform);
        
        RectTransform amountRect = amountObj.AddComponent<RectTransform>();
        amountRect.sizeDelta = new Vector2(70, 30);
        amountRect.anchoredPosition = new Vector2(15, 0);
        
        TMPro.TextMeshProUGUI amountText = amountObj.AddComponent<TMPro.TextMeshProUGUI>();
        amountText.text = "1000";
        amountText.fontSize = 24;
        amountText.color = new Color(1f, 0.6f, 0.7f, 1f);
        amountText.alignment = TMPro.TextAlignmentOptions.Center;
        
        currencyDisplay.amountText = amountText;
        
        Debug.Log("货币显示预制体创建完成，请在编辑器中保存为Prefab");
    }
    
    /// <summary>
    /// 创建漂浮爱心预制体
    /// </summary>
    public static void CreateFloatingHeartPrefab()
    {
        GameObject heartObj = new GameObject("FloatingHeart");
        
        RectTransform rect = heartObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(30, 30);
        
        UnityEngine.UI.Image heartImage = heartObj.AddComponent<UnityEngine.UI.Image>();
        heartImage.color = new Color(1f, 0.6f, 0.7f, 0.6f);
        
        Debug.Log("漂浮爱心预制体创建完成，请在编辑器中保存为Prefab");
    }
    
    /// <summary>
    /// 创建主菜单预制体
    /// </summary>
    public static void CreateMainMenuPrefab()
    {
        GameObject menuObj = new GameObject("MainMenuUI");
        
        RectTransform rect = menuObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1920, 1080);
        rect.anchoredPosition = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        
        // 背景
        UnityEngine.UI.Image bgImage = menuObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(1f, 0.98f, 0.98f, 1f);
        
        MainMenuUI mainMenu = menuObj.AddComponent<MainMenuUI>();
        mainMenu.transform.SetParent(null);
        
        Debug.Log("主菜单预制体创建完成，请在编辑器中保存为Prefab");
    }
    
    /// <summary>
    /// 创建游戏HUD预制体
    /// </summary>
    public static void CreateGameHUDPrefab()
    {
        GameObject hudObj = new GameObject("GameHUD");
        
        RectTransform rect = hudObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1920, 1080);
        rect.anchoredPosition = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        
        GameHUD gameHUD = hudObj.AddComponent<GameHUD>();
        
        Debug.Log("游戏HUD预制体创建完成，请在编辑器中保存为Prefab");
    }
}

/// <summary>
/// 预制体创建工具编辑器窗口
/// </summary>
#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(UIPrefabSetup))]
public class UIPrefabSetupEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GUILayout.Space(20);
        GUILayout.Label("预制体创建工具", UnityEditor.EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("创建按钮预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreateButtonPrefab();
        }
        
        if (GUILayout.Button("创建进度条预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreateProgressBarPrefab();
        }
        
        if (GUILayout.Button("创建对话框预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreateDialogueBoxPrefab();
        }
        
        if (GUILayout.Button("创建面板预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreatePanelPrefab();
        }
        
        if (GUILayout.Button("创建弹窗预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreatePopupPrefab();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("创建模式指示器预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreateModeIndicatorPrefab();
        }
        
        if (GUILayout.Button("创建时间显示预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreateTimeDisplayPrefab();
        }
        
        if (GUILayout.Button("创建货币显示预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreateCurrencyDisplayPrefab();
        }
        
        if (GUILayout.Button("创建漂浮爱心预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreateFloatingHeartPrefab();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("创建主菜单预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreateMainMenuPrefab();
        }
        
        if (GUILayout.Button("创建游戏HUD预制体", GUILayout.Height(30)))
        {
            UIPrefabSetup.CreateGameHUDPrefab();
        }
    }
}
#endif
