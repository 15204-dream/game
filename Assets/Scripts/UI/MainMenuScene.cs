using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

/// <summary>
/// 主菜单场景管理器
/// </summary>
public class MainMenuScene : MonoBehaviour
{
    [Header("场景配置")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string characterGallerySceneName = "CharacterGallery";
    [SerializeField] private string settingsSceneName = "SettingsScene";
    
    [Header("UI引用")]
    [SerializeField] private MainMenuUI mainMenuUI;
    
    [Header("音频配置")]
    [SerializeField] private AudioClip menuBGM;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("过渡效果")]
    [SerializeField] private bool useScreenTransition = true;
    [SerializeField] private float transitionDuration = 0.5f;
    [SerializeField] private Color transitionColor = new Color(1f, 0.95f, 0.95f);
    
    // 单例
    private static MainMenuScene instance;
    public static MainMenuScene Instance
    {
        get { return instance; }
    }
    
    // 状态
    private bool isTransitioning = false;
    private AsyncOperation loadingOperation;
    
    // 游戏模式
    public enum GameMode
    {
        Guest,      // 嘉宾模式
        Director    // 导演模式
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
        
        Initialize();
    }
    
    private void Start()
    {
        // 播放背景音乐
        PlayBGM();
        
        // 初始化UI
        if (mainMenuUI != null)
        {
            mainMenuUI.Initialize(this);
        }
    }
    
    /// <summary>
    /// 初始化
    /// </summary>
    private void Initialize()
    {
        // 加载保存的数据
        LoadGameData();
        
        // 检查是否有存档
        bool hasSaveData = CheckSaveData();
        if (mainMenuUI != null)
        {
            mainMenuUI.SetContinueButtonActive(hasSaveData);
        }
    }
    
    /// <summary>
    /// 加载游戏数据
    /// </summary>
    private void LoadGameData()
    {
        // TODO: 从PlayerPrefs或存档系统加载数据
        // GameData.Instance.Load();
    }
    
    /// <summary>
    /// 检查存档
    /// </summary>
    private bool CheckSaveData()
    {
        // TODO: 检查存档是否存在
        return PlayerPrefs.HasKey("SaveData_Exists");
    }
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    private void PlayBGM()
    {
        if (menuBGM != null)
        {
            // AudioManager.PlayBGM(menuBGM);
        }
    }
    
    /// <summary>
    /// 播放按钮音效
    /// </summary>
    private void PlayButtonSound()
    {
        if (buttonClickSound != null)
        {
            // AudioManager.PlaySFX(buttonClickSound);
        }
    }
    
    #region 场景切换方法
    
    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartNewGame(GameMode mode)
    {
        if (isTransitioning) return;
        
        PlayButtonSound();
        StartCoroutine(StartNewGameCoroutine(mode));
    }
    
    private IEnumerator StartNewGameCoroutine(GameMode mode)
    {
        isTransitioning = true;
        
        // 保存选择的模式
        PlayerPrefs.SetInt("CurrentGameMode", (int)mode);
        
        // 显示加载过渡
        if (useScreenTransition)
        {
            yield return StartCoroutine(ScreenTransition(true));
        }
        
        // 异步加载游戏场景
        loadingOperation = SceneManager.LoadSceneAsync(gameSceneName);
        yield return loadingOperation;
        
        // 隐藏过渡
        if (useScreenTransition)
        {
            yield return StartCoroutine(ScreenTransition(false));
        }
        
        isTransitioning = false;
    }
    
    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ContinueGame()
    {
        if (isTransitioning) return;
        
        if (!CheckSaveData())
        {
            UIManager.Instance.ShowMessage("提示", "没有找到存档数据");
            return;
        }
        
        PlayButtonSound();
        StartCoroutine(ContinueGameCoroutine());
    }
    
    private IEnumerator ContinueGameCoroutine()
    {
        isTransitioning = true;
        
        if (useScreenTransition)
        {
            yield return StartCoroutine(ScreenTransition(true));
        }
        
        loadingOperation = SceneManager.LoadSceneAsync(gameSceneName);
        yield return loadingOperation;
        
        if (useScreenTransition)
        {
            yield return StartCoroutine(ScreenTransition(false));
        }
        
        isTransitioning = false;
    }
    
    /// <summary>
    /// 打开角色图鉴
    /// </summary>
    public void OpenCharacterGallery()
    {
        if (isTransitioning) return;
        
        PlayButtonSound();
        StartCoroutine(OpenCharacterGalleryCoroutine());
    }
    
    private IEnumerator OpenCharacterGalleryCoroutine()
    {
        isTransitioning = true;
        
        if (useScreenTransition)
        {
            yield return StartCoroutine(ScreenTransition(true));
        }
        
        SceneManager.LoadScene(characterGallerySceneName);
        
        if (useScreenTransition)
        {
            yield return new WaitForSeconds(transitionDuration);
            yield return StartCoroutine(ScreenTransition(false));
        }
        
        isTransitioning = false;
    }
    
    /// <summary>
    /// 打开设置
    /// </summary>
    public void OpenSettings()
    {
        if (isTransitioning) return;
        
        PlayButtonSound();
        
        if (mainMenuUI != null)
        {
            mainMenuUI.ShowSettingsPanel();
        }
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        if (isTransitioning) return;
        
        PlayButtonSound();
        
        UIManager.Instance.ShowConfirm("退出游戏", "确定要退出游戏吗？", () =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        });
    }
    
    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (isTransitioning) return;
        
        PlayButtonSound();
        StartCoroutine(ReturnToMainMenuCoroutine());
    }
    
    private IEnumerator ReturnToMainMenuCoroutine()
    {
        isTransitioning = true;
        
        if (useScreenTransition)
        {
            yield return StartCoroutine(ScreenTransition(true));
        }
        
        SceneManager.LoadScene("MainMenu");
        
        if (useScreenTransition)
        {
            yield return new WaitForSeconds(transitionDuration);
            yield return StartCoroutine(ScreenTransition(false));
        }
        
        isTransitioning = false;
    }
    
    #endregion
    
    #region 屏幕过渡
    
    /// <summary>
    /// 屏幕过渡效果
    /// </summary>
    private IEnumerator ScreenTransition(bool fadeOut)
    {
        // 创建过渡图像
        GameObject transitionObj = new GameObject("ScreenTransition");
        RectTransform rect = transitionObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        Canvas canvas = transitionObj.AddComponent<Canvas>();
        canvas.sortingOrder = 9999;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        UnityEngine.UI.Image image = transitionObj.AddComponent<UnityEngine.UI.Image>();
        image.color = fadeOut ? Color.clear : transitionColor;
        
        float elapsed = 0f;
        float duration = transitionDuration;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (fadeOut)
            {
                image.color = Color.Lerp(Color.clear, transitionColor, t);
            }
            else
            {
                image.color = Color.Lerp(transitionColor, Color.clear, t);
            }
            
            yield return null;
        }
        
        image.color = fadeOut ? transitionColor : Color.clear;
        
        if (!fadeOut)
        {
            Destroy(transitionObj);
        }
        else
        {
            Destroy(transitionObj);
        }
    }
    
    #endregion
    
    #region 数据管理
    
    /// <summary>
    /// 删除存档
    /// </summary>
    public void DeleteSaveData()
    {
        UIManager.Instance.ShowConfirm("删除存档", "确定要删除所有存档数据吗？此操作不可恢复！", () =>
        {
            // TODO: 删除存档
            PlayerPrefs.DeleteKey("SaveData_Exists");
            
            if (mainMenuUI != null)
            {
                mainMenuUI.SetContinueButtonActive(false);
            }
            
            UIManager.Instance.ShowSuccess("删除成功", "存档已删除");
        });
    }
    
    #endregion
    
    /// <summary>
    /// 设置音量
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80f, 0f, volume));
        }
    }
    
    /// <summary>
    /// 设置BGM音量
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("BGMVolume", Mathf.Lerp(-80f, 0f, volume));
        }
    }
    
    /// <summary>
    /// 设置音效音量
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Lerp(-80f, 0f, volume));
        }
    }
}
