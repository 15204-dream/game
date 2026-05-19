using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Core.Integration
{
    /// <summary>
    /// 场景加载器 - 管理场景的异步加载和切换
    /// 支持加载画面、场景预加载、依赖管理等
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader _instance;
        public static SceneLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SceneLoader>();
                    if (_instance == null)
                    {
                        var go = new GameObject("SceneLoader");
                        _instance = go.AddComponent<SceneLoader>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private Dictionary<string, SceneConfig> _sceneConfigs = new Dictionary<string, SceneConfig>();
        private Dictionary<string, AsyncOperation> _loadingOperations = new Dictionary<string, AsyncOperation>();
        private Dictionary<string, SceneLoadProgress> _loadProgress = new Dictionary<string, SceneLoadProgress>();
        
        private SceneConfig _currentSceneConfig;
        private SceneConfig _targetSceneConfig;
        private bool _isLoading = false;
        
        private List<string> _preloadedScenes = new List<string>();
        private float _lastGarbageCollectionTime = 0f;

        public bool IsLoading
        {
            get { return _isLoading; }
        }

        public SceneConfig CurrentSceneConfig
        {
            get { return _currentSceneConfig; }
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            InitializeDefaultScenes();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        /// <summary>
        /// 初始化默认场景配置
        /// </summary>
        private void InitializeDefaultScenes()
        {
            RegisterSceneConfig(new SceneConfig("MainMenu", "Scenes/MainMenu"));
            RegisterSceneConfig(new SceneConfig("Game", "Scenes/Game"));
            RegisterSceneConfig(new SceneConfig("Settings", "Scenes/Settings"));
            RegisterSceneConfig(new SceneConfig("Credits", "Scenes/Credits"));
        }

        /// <summary>
        /// 注册场景配置
        /// </summary>
        public void RegisterSceneConfig(SceneConfig config)
        {
            if (!_sceneConfigs.ContainsKey(config.sceneName))
            {
                _sceneConfigs.Add(config.sceneName, config);
            }
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        public void LoadScene(string sceneName, Action onComplete = null)
        {
            LoadScene(sceneName, null, onComplete);
        }

        /// <summary>
        /// 加载场景（带配置）
        /// </summary>
        public void LoadScene(string sceneName, SceneConfig config, Action onComplete = null)
        {
            if (_isLoading)
            {
                Debug.LogWarning("场景正在加载中，请等待完成");
                return;
            }

            if (!_sceneConfigs.ContainsKey(sceneName) && config == null)
            {
                Debug.LogError($"场景配置未找到: {sceneName}");
                return;
            }

            _targetSceneConfig = config ?? _sceneConfigs[sceneName];
            _isLoading = true;

            var progress = new SceneLoadProgress(sceneName);
            _loadProgress[sceneName] = progress;

            EventBus.Instance.Publish(GameEvents.OnSceneLoad, progress);
            EventBus.Instance.Publish($"OnSceneLoadStart_{sceneName}", null);

            StartCoroutine(LoadSceneCoroutine(_targetSceneConfig, onComplete));
        }

        /// <summary>
        /// 异步加载场景协程
        /// </summary>
        private IEnumerator LoadSceneCoroutine(SceneConfig config, Action onComplete)
        {
            var progress = _loadProgress[config.sceneName];
            progress.status = "加载依赖";

            if (config.preloadDependencies)
            {
                yield return PreloadDependencies(config);
            }

            progress.status = "加载场景";

            AsyncOperation asyncOp;
            if (config.loadMode == LoadSceneMode.Additive)
            {
                asyncOp = SceneManager.LoadSceneAsync(config.scenePath, LoadSceneMode.Additive);
            }
            else
            {
                asyncOp = SceneManager.LoadSceneAsync(config.scenePath, LoadSceneMode.Single);
            }

            _loadingOperations[config.sceneName] = asyncOp;
            asyncOp.priority = (int)config.priority;
            asyncOp.allowSceneActivation = false;

            while (!asyncOp.isDone)
            {
                progress.progress = asyncOp.progress;
                EventBus.Instance.Publish($"OnSceneLoadProgress_{config.sceneName}", progress);
                yield return null;
            }

            progress.progress = 1f;
            progress.isComplete = true;
            progress.endTime = Time.time;
            progress.status = "加载完成";

            _currentSceneConfig = config;
            _isLoading = false;

            if (!config.enableLoadingScreen)
            {
                asyncOp.allowSceneActivation = true;
            }

            yield return new WaitForSeconds(0.1f);
            asyncOp.allowSceneActivation = true;

            EventBus.Instance.Publish($"OnSceneLoadComplete_{config.sceneName}", progress);
            EventBus.Instance.Publish(GameEvents.OnSceneLoad, progress);

            onComplete?.Invoke();

            TriggerGarbageCollection();
        }

        /// <summary>
        /// 预加载依赖场景
        /// </summary>
        private IEnumerator PreloadDependencies(SceneConfig config)
        {
            foreach (var depScene in config.requiredScenes)
            {
                if (!_preloadedScenes.Contains(depScene))
                {
                    var asyncOp = SceneManager.LoadSceneAsync(depScene, LoadSceneMode.Additive);
                    yield return asyncOp;
                    _preloadedScenes.Add(depScene);
                }
            }
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        public void UnloadScene(string sceneName, Action onComplete = null)
        {
            StartCoroutine(UnloadSceneCoroutine(sceneName, onComplete));
        }

        /// <summary>
        /// 卸载场景协程
        /// </summary>
        private IEnumerator UnloadSceneCoroutine(string sceneName, Action onComplete)
        {
            var asyncOp = SceneManager.UnloadSceneAsync(sceneName);
            if (asyncOp != null)
            {
                while (!asyncOp.isDone)
                {
                    yield return null;
                }
            }

            _preloadedScenes.Remove(sceneName);
            onComplete?.Invoke();

            TriggerGarbageCollection();
        }

        /// <summary>
        /// 场景加载完成回调
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"场景已加载: {scene.name}, 模式: {mode}");
        }

        /// <summary>
        /// 场景卸载完成回调
        /// </summary>
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"场景已卸载: {scene.name}");
        }

        /// <summary>
        /// 触发垃圾回收
        /// </summary>
        private void TriggerGarbageCollection()
        {
            if (Time.time - _lastGarbageCollectionTime > 30f)
            {
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                _lastGarbageCollectionTime = Time.time;
            }
        }

        /// <summary>
        /// 获取加载进度
        /// </summary>
        public SceneLoadProgress GetLoadProgress(string sceneName)
        {
            if (_loadProgress.ContainsKey(sceneName))
            {
                return _loadProgress[sceneName];
            }
            return null;
        }

        /// <summary>
        /// 获取当前活动场景名称
        /// </summary>
        public string GetActiveSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        /// <summary>
        /// 预加载场景
        /// </summary>
        public void PreloadScene(string sceneName)
        {
            if (_sceneConfigs.ContainsKey(sceneName))
            {
                var config = _sceneConfigs[sceneName];
                StartCoroutine(PreloadDependencies(config));
            }
        }

        /// <summary>
        /// 获取已预加载的场景列表
        /// </summary>
        public List<string> GetPreloadedScenes()
        {
            return new List<string>(_preloadedScenes);
        }
    }
}
