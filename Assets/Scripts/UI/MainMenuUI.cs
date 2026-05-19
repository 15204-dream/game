using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 主菜单UI控制器
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("主菜单UI组件")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject modeSelectPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("主菜单按钮")]
    [SerializeField] private UIButton newGameButton;
    [SerializeField] private UIButton continueButton;
    [SerializeField] private UIButton galleryButton;
    [SerializeField] private UIButton settingsButton;
    [SerializeField] private UIButton quitButton;
    
    [Header("模式选择按钮")]
    [SerializeField] private UIButton guestModeButton;
    [SerializeField] private UIButton directorModeButton;
    [SerializeField] private UIButton backButton;
    
    [Header("设置面板组件")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private UIButton applySettingsButton;
    [SerializeField] private UIButton closeSettingsButton;
    
    [Header("视觉特效")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite[] backgroundSprites;
    [SerializeField] private float backgroundChangeInterval = 10f;
    [SerializeField] private bool useAnimatedBackground = true;
    
    [Header("动画配置")]
    [SerializeField] private float buttonShowDelay = 0.1f;
    [SerializeField] private AnimationCurve buttonShowCurve;
    [SerializeField] private bool useButtonAnimation = true;
    
    [Header("装饰元素")]
    [SerializeField] private GameObject floatingHeartsPrefab;
    [SerializeField] private int heartCount = 10;
    [SerializeField] private bool showFloatingHearts = true;
    
    // 私有变量
    private MainMenuScene mainMenuScene;
    private List<GameObject> floatingHearts = new List<GameObject>();
    private int currentBackgroundIndex = 0;
    private Coroutine backgroundCoroutine;
    private bool isInitialized = false;
    
    private void Start()
    {
        if (!isInitialized)
        {
            Initialize(FindObjectOfType<MainMenuScene>());
        }
    }
    
    /// <summary>
    /// 初始化UI
    /// </summary>
    public void Initialize(MainMenuScene scene)
    {
        mainMenuScene = scene;
        
        // 设置按钮事件
        SetupButtonEvents();
        
        // 初始化设置面板
        InitializeSettingsPanel();
        
        // 显示主菜单面板
        ShowMainMenuPanel();
        
        // 开始背景动画
        if (useAnimatedBackground)
        {
            StartBackgroundAnimation();
        }
        
        // 创建漂浮爱心
        if (showFloatingHearts)
        {
            CreateFloatingHearts();
        }
        
        // 按钮入场动画
        if (useButtonAnimation)
        {
            PlayButtonEntranceAnimation();
        }
        
        isInitialized = true;
    }
    
    /// <summary>
    /// 设置按钮事件
    /// </summary>
    private void SetupButtonEvents()
    {
        if (newGameButton != null)
        {
            newGameButton.OnClick.AddListener(OnNewGameClicked);
        }
        
        if (continueButton != null)
        {
            continueButton.OnClick.AddListener(OnContinueClicked);
        }
        
        if (galleryButton != null)
        {
            galleryButton.OnClick.AddListener(OnGalleryClicked);
        }
        
        if (settingsButton != null)
        {
            settingsButton.OnClick.AddListener(OnSettingsClicked);
        }
        
        if (quitButton != null)
        {
            quitButton.OnClick.AddListener(OnQuitClicked);
        }
        
        // 模式选择按钮
        if (guestModeButton != null)
        {
            guestModeButton.OnClick.AddListener(OnGuestModeClicked);
        }
        
        if (directorModeButton != null)
        {
            directorModeButton.OnClick.AddListener(OnDirectorModeClicked);
        }
        
        if (backButton != null)
        {
            backButton.OnClick.AddListener(OnBackClicked);
        }
        
        // 设置面板按钮
        if (applySettingsButton != null)
        {
            applySettingsButton.OnClick.AddListener(OnApplySettingsClicked);
        }
        
        if (closeSettingsButton != null)
        {
            closeSettingsButton.OnClick.AddListener(OnCloseSettingsClicked);
        }
    }
    
    #region 按钮点击事件
    
    private void OnNewGameClicked()
    {
        ShowModeSelectPanel();
    }
    
    private void OnContinueClicked()
    {
        if (mainMenuScene != null)
        {
            mainMenuScene.ContinueGame();
        }
    }
    
    private void OnGalleryClicked()
    {
        if (mainMenuScene != null)
        {
            mainMenuScene.OpenCharacterGallery();
        }
    }
    
    private void OnSettingsClicked()
    {
        ShowSettingsPanel();
    }
    
    private void OnQuitClicked()
    {
        if (mainMenuScene != null)
        {
            mainMenuScene.QuitGame();
        }
    }
    
    private void OnGuestModeClicked()
    {
        if (mainMenuScene != null)
        {
            mainMenuScene.StartNewGame(MainMenuScene.GameMode.Guest);
        }
    }
    
    private void OnDirectorModeClicked()
    {
        if (mainMenuScene != null)
        {
            mainMenuScene.StartNewGame(MainMenuScene.GameMode.Director);
        }
    }
    
    private void OnBackClicked()
    {
        ShowMainMenuPanel();
    }
    
    private void OnApplySettingsClicked()
    {
        ApplySettings();
    }
    
    private void OnCloseSettingsClicked()
    {
        HideSettingsPanel();
    }
    
    #endregion
    
    #region 面板切换
    
    /// <summary>
    /// 显示主菜单面板
    /// </summary>
    public void ShowMainMenuPanel()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        
        if (modeSelectPanel != null)
        {
            modeSelectPanel.SetActive(false);
        }
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 显示模式选择面板
    /// </summary>
    public void ShowModeSelectPanel()
    {
        if (modeSelectPanel != null)
        {
            StartCoroutine(ShowPanelWithAnimation(modeSelectPanel));
        }
        
        if (mainMenuPanel != null)
        {
            StartCoroutine(HidePanelWithAnimation(mainMenuPanel));
        }
    }
    
    /// <summary>
    /// 显示设置面板
    /// </summary>
    public void ShowSettingsPanel()
    {
        LoadCurrentSettings();
        
        if (settingsPanel != null)
        {
            StartCoroutine(ShowPanelWithAnimation(settingsPanel));
        }
    }
    
    /// <summary>
    /// 隐藏设置面板
    /// </summary>
    public void HideSettingsPanel()
    {
        if (settingsPanel != null)
        {
            StartCoroutine(HidePanelWithAnimation(settingsPanel));
        }
    }
    
    /// <summary>
    /// 带动画显示面板
    /// </summary>
    private IEnumerator ShowPanelWithAnimation(GameObject panel)
    {
        panel.SetActive(true);
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 0f;
        Vector3 startScale = Vector3.one * 0.8f;
        Vector3 endScale = Vector3.one;
        panel.transform.localScale = startScale;
        
        float elapsed = 0f;
        float duration = 0.3f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = Mathf.SmoothStep(0f, 1f, t);
            
            canvasGroup.alpha = curveValue;
            panel.transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);
            
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        panel.transform.localScale = endScale;
    }
    
    /// <summary>
    /// 带动画隐藏面板
    /// </summary>
    private IEnumerator HidePanelWithAnimation(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }
        
        Vector3 startScale = panel.transform.localScale;
        Vector3 endScale = Vector3.one * 0.8f;
        
        float elapsed = 0f;
        float duration = 0.2f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = Mathf.SmoothStep(0f, 1f, t);
            
            canvasGroup.alpha = 1f - curveValue;
            panel.transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);
            
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        panel.transform.localScale = endScale;
        panel.SetActive(false);
    }
    
    #endregion
    
    #region 按钮动画
    
    /// <summary>
    /// 播放按钮入场动画
    /// </summary>
    private void PlayButtonEntranceAnimation()
    {
        List<UIButton> buttons = new List<UIButton>();
        
        if (newGameButton != null) buttons.Add(newGameButton);
        if (continueButton != null) buttons.Add(continueButton);
        if (galleryButton != null) buttons.Add(galleryButton);
        if (settingsButton != null) buttons.Add(settingsButton);
        if (quitButton != null) buttons.Add(quitButton);
        
        StartCoroutine(AnimateButtonEntrance(buttons));
    }
    
    private IEnumerator AnimateButtonEntrance(List<UIButton> buttons)
    {
        float delay = 0f;
        
        foreach (var button in buttons)
        {
            if (button == null) continue;
            
            StartCoroutine(AnimateSingleButton(button.gameObject, delay));
            delay += buttonShowDelay;
        }
        
        yield return null;
    }
    
    private IEnumerator AnimateSingleButton(GameObject button, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        RectTransform rect = button.GetComponent<RectTransform>();
        if (rect == null) yield break;
        
        Vector3 originalPosition = rect.localPosition;
        Vector3 startPosition = originalPosition + new Vector3(0f, -50f, 0f);
        
        rect.localPosition = startPosition;
        button.transform.localScale = Vector3.zero;
        
        float elapsed = 0f;
        float duration = 0.4f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = buttonShowCurve != null ? 
                buttonShowCurve.Evaluate(t) : Mathf.SmoothStep(0f, 1f, t);
            
            rect.localPosition = Vector3.Lerp(startPosition, originalPosition, curveValue);
            button.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, curveValue);
            
            yield return null;
        }
        
        rect.localPosition = originalPosition;
        button.transform.localScale = Vector3.one;
    }
    
    #endregion
    
    #region 设置面板
    
    /// <summary>
    /// 初始化设置面板
    /// </summary>
    private void InitializeSettingsPanel()
    {
        // 质量设置
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            List<string> qualityOptions = new List<string>();
            string[] qualityNames = QualitySettings.names;
            
            foreach (string name in qualityNames)
            {
                qualityOptions.Add(name);
            }
            
            qualityDropdown.AddOptions(qualityOptions);
            qualityDropdown.value = QualitySettings.GetQualityLevel();
        }
        
        // 分辨率设置
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            List<string> resolutionOptions = new List<string>();
            Resolution[] resolutions = Screen.resolutions;
            
            foreach (Resolution res in resolutions)
            {
                resolutionOptions.Add($"{res.width} x {res.height}");
            }
            
            resolutionDropdown.AddOptions(resolutionOptions);
            resolutionDropdown.value = resolutions.Length - 1;
        }
    }
    
    /// <summary>
    /// 加载当前设置
    /// </summary>
    private void LoadCurrentSettings()
    {
        if (masterVolumeSlider != null)
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            masterVolumeSlider.value = masterVolume;
        }
        
        if (bgmVolumeSlider != null)
        {
            float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
            bgmVolumeSlider.value = bgmVolume;
        }
        
        if (sfxVolumeSlider != null)
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxVolumeSlider.value = sfxVolume;
        }
        
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }
        
        if (qualityDropdown != null)
        {
            qualityDropdown.value = QualitySettings.GetQualityLevel();
        }
    }
    
    /// <summary>
    /// 应用设置
    /// </summary>
    private void ApplySettings()
    {
        if (masterVolumeSlider != null)
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
            if (mainMenuScene != null)
            {
                mainMenuScene.SetMasterVolume(masterVolumeSlider.value);
            }
        }
        
        if (bgmVolumeSlider != null)
        {
            PlayerPrefs.SetFloat("BGMVolume", bgmVolumeSlider.value);
            if (mainMenuScene != null)
            {
                mainMenuScene.SetBGMVolume(bgmVolumeSlider.value);
            }
        }
        
        if (sfxVolumeSlider != null)
        {
            PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
            if (mainMenuScene != null)
            {
                mainMenuScene.SetSFXVolume(sfxVolumeSlider.value);
            }
        }
        
        if (fullscreenToggle != null)
        {
            Screen.fullScreen = fullscreenToggle.isOn;
        }
        
        if (qualityDropdown != null)
        {
            QualitySettings.SetQualityLevel(qualityDropdown.value);
        }
        
        if (resolutionDropdown != null)
        {
            Resolution[] resolutions = Screen.resolutions;
            Resolution selectedResolution = resolutions[resolutionDropdown.value];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        }
        
        PlayerPrefs.Save();
        
        UIManager.Instance.ShowSuccess("设置", "设置已保存！");
    }
    
    #endregion
    
    #region 视觉特效
    
    /// <summary>
    /// 开始背景动画
    /// </summary>
    private void StartBackgroundAnimation()
    {
        if (backgroundSprites != null && backgroundSprites.Length > 0)
        {
            backgroundCoroutine = StartCoroutine(BackgroundAnimationCoroutine());
        }
    }
    
    private IEnumerator BackgroundAnimationCoroutine()
    {
        while (useAnimatedBackground)
        {
            yield return new WaitForSeconds(backgroundChangeInterval);
            
            if (backgroundSprites != null && backgroundSprites.Length > 0)
            {
                currentBackgroundIndex = (currentBackgroundIndex + 1) % backgroundSprites.Length;
                
                if (backgroundImage != null && backgroundSprites[currentBackgroundIndex] != null)
                {
                    StartCoroutine(CrossFadeBackground(backgroundSprites[currentBackgroundIndex]));
                }
            }
        }
    }
    
    private IEnumerator CrossFadeBackground(Sprite newSprite)
    {
        if (backgroundImage == null) yield break;
        
        float duration = 1f;
        float elapsed = 0f;
        Color startColor = backgroundImage.color;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            backgroundImage.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0.5f), t);
            
            yield return null;
        }
        
        backgroundImage.sprite = newSprite;
        
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            backgroundImage.color = Color.Lerp(new Color(1f, 1f, 1f, 0.5f), Color.white, t);
            
            yield return null;
        }
        
        backgroundImage.color = Color.white;
    }
    
    /// <summary>
    /// 创建漂浮爱心
    /// </summary>
    private void CreateFloatingHearts()
    {
        if (floatingHeartsPrefab == null) return;
        
        for (int i = 0; i < heartCount; i++)
        {
            CreateSingleHeart();
        }
    }
    
    private void CreateSingleHeart()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-Screen.width / 2f, Screen.width / 2f),
            Random.Range(-Screen.height / 2f, Screen.height / 2f),
            0f
        );
        
        GameObject heart = Instantiate(floatingHeartsPrefab, Vector3.zero, Quaternion.identity, transform);
        heart.transform.localPosition = randomPosition;
        
        StartCoroutine(FloatHeart(heart));
        
        floatingHearts.Add(heart);
    }
    
    private IEnumerator FloatHeart(GameObject heart)
    {
        Vector3 startPosition = heart.transform.localPosition;
        float floatHeight = Random.Range(20f, 50f);
        float duration = Random.Range(3f, 6f);
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float yOffset = Mathf.Sin(elapsed / duration * Mathf.PI * 2f) * floatHeight;
            heart.transform.localPosition = startPosition + new Vector3(0f, yOffset, 0f);
            
            yield return null;
        }
        
        // 重新放置
        Destroy(heart);
        floatingHearts.Remove(heart);
        CreateSingleHeart();
    }
    
    #endregion
    
    /// <summary>
    /// 设置继续按钮状态
    /// </summary>
    public void SetContinueButtonActive(bool active)
    {
        if (continueButton != null)
        {
            continueButton.interactable = active;
        }
    }
    
    private void OnDestroy()
    {
        if (backgroundCoroutine != null)
        {
            StopCoroutine(backgroundCoroutine);
        }
        
        foreach (var heart in floatingHearts)
        {
            if (heart != null)
            {
                Destroy(heart);
            }
        }
        
        floatingHearts.Clear();
    }
}
