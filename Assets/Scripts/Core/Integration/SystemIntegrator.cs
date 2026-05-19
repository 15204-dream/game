using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Integration
{
    /// <summary>
    /// 系统集成管理器 - 统一管理所有游戏系统
    /// 协调各模块间的交互和数据流转
    /// </summary>
    public class SystemIntegrator : MonoBehaviour
    {
        private static SystemIntegrator _instance;
        public static SystemIntegrator Instance
        {
            get { return _instance; }
        }

        private bool _isInitialized = false;
        private bool _isShuttingDown = false;

        private Dictionary<string, MonoBehaviour> _managers = new Dictionary<string, MonoBehaviour>();
        private Dictionary<string, object> _services = new Dictionary<string, object>();

        private List<ISystemModule> _systemModules = new List<ISystemModule>();
        private Queue<System.Action> _pendingInitializations = new Queue<System.Action>();

        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                StartCoroutine(InitializeCoroutine());
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            EventBus.Instance.Publish(GameEvents.OnGameStart, null);
        }

        void Update()
        {
            if (_isShuttingDown) return;

            foreach (var module in _systemModules)
            {
                try
                {
                    module.OnUpdate?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"系统模块更新异常 [{module.GetType().Name}]: {ex.Message}");
                }
            }
        }

        void FixedUpdate()
        {
            if (_isShuttingDown) return;

            foreach (var module in _systemModules)
            {
                try
                {
                    module.OnFixedUpdate?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"系统模块固定更新异常 [{module.GetType().Name}]: {ex.Message}");
                }
            }
        }

        void OnDestroy()
        {
            _isShuttingDown = true;
            Shutdown();
        }

        /// <summary>
        /// 初始化协程
        /// </summary>
        private System.Collections.IEnumerator InitializeCoroutine()
        {
            yield return new WaitForEndOfFrame();

            InitializeCoreSystems();
            yield return new WaitForEndOfFrame();

            InitializeManagers();
            yield return new WaitForEndOfFrame();

            InitializeServices();
            yield return new WaitForEndOfFrame();

            ProcessPendingInitializations();
            yield return new WaitForEndOfFrame();

            _isInitialized = true;
            Debug.Log("系统集成管理器初始化完成");
        }

        /// <summary>
        /// 初始化核心系统
        /// </summary>
        private void InitializeCoreSystems()
        {
            EnsureCoreSystem<EventBus>("EventBus");
            EnsureCoreSystem<ServiceLocator>("ServiceLocator");
            EnsureCoreSystem<GameLoopManager>("GameLoopManager");
        }

        /// <summary>
        /// 确保核心系统存在
        /// </summary>
        private void EnsureCoreSystem<T>(string name) where T : MonoBehaviour
        {
            if (!HasManager(name))
            {
                var component = gameObject.AddComponent<T>();
                RegisterManager(name, component);
            }
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        private void InitializeManagers()
        {
            var managerTypes = new[]
            {
                typeof(PerformanceOptimizer),
                typeof(MemoryManager),
                typeof(ObjectPoolManager),
                typeof(CacheManager),
                typeof(ResourceManager),
                typeof(SceneLoader),
                typeof(AudioManager),
                typeof(LocalizationManager),
                typeof(DebugConsole),
                typeof(GameLogger),
                typeof(PerformanceProfiler)
            };

            foreach (var type in managerTypes)
            {
                EnsureComponentExists(type);
            }
        }

        /// <summary>
        /// 确保组件存在
        /// </summary>
        private void EnsureComponentExists(Type type)
        {
            var component = GetComponent(type);
            if (component == null)
            {
                component = gameObject.AddComponent(type);
            }
            RegisterManager(type.Name, component);
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        private void InitializeServices()
        {
            RegisterService<GameManager>(ServiceLocator.Instance.GetService<GameManager>());
            RegisterService<DataManager>(ServiceLocator.Instance.GetService<DataManager>());
            RegisterService<UIManager>(ServiceLocator.Instance.GetService<UIManager>());
            RegisterService<CharacterManager>(ServiceLocator.Instance.GetService<CharacterManager>());
            RegisterService<DialogueManager>(ServiceLocator.Instance.GetService<DialogueManager>());
        }

        /// <summary>
        /// 处理待处理的初始化
        /// </summary>
        private void ProcessPendingInitializations()
        {
            while (_pendingInitializations.Count > 0)
            {
                var action = _pendingInitializations.Dequeue();
                try
                {
                    action?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"待处理初始化异常: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 注册管理器
        /// </summary>
        public void RegisterManager(string name, MonoBehaviour manager)
        {
            if (!_managers.ContainsKey(name))
            {
                _managers.Add(name, manager);
                Debug.Log($"管理器已注册: {name}");
            }
        }

        /// <summary>
        /// 获取管理器
        /// </summary>
        public T GetManager<T>(string name) where T : MonoBehaviour
        {
            if (_managers.ContainsKey(name))
            {
                return _managers[name] as T;
            }
            Debug.LogWarning($"管理器未找到: {name}");
            return null;
        }

        /// <summary>
        /// 检查管理器是否存在
        /// </summary>
        public bool HasManager(string name)
        {
            return _managers.ContainsKey(name);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        public void RegisterService<T>(T service) where T : class
        {
            var name = typeof(T).Name;
            if (!_services.ContainsKey(name))
            {
                _services.Add(name, service);
                Debug.Log($"服务已注册: {name}");
            }
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        public T GetService<T>() where T : class
        {
            var name = typeof(T).Name;
            if (_services.ContainsKey(name))
            {
                return _services[name] as T;
            }
            Debug.LogWarning($"服务未找到: {name}");
            return null;
        }

        /// <summary>
        /// 注册系统模块
        /// </summary>
        public void RegisterModule(ISystemModule module)
        {
            if (!_systemModules.Contains(module))
            {
                _systemModules.Add(module);
                Debug.Log($"系统模块已注册: {module.GetType().Name}");
            }
        }

        /// <summary>
        /// 取消注册系统模块
        /// </summary>
        public void UnregisterModule(ISystemModule module)
        {
            _systemModules.Remove(module);
        }

        /// <summary>
        /// 延迟初始化
        /// </summary>
        public void DeferInitialization(System.Action action)
        {
            if (_isInitialized)
            {
                action?.Invoke();
            }
            else
            {
                _pendingInitializations.Enqueue(action);
            }
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        private void Shutdown()
        {
            foreach (var module in _systemModules)
            {
                try
                {
                    module.OnShutdown?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"系统模块关闭异常 [{module.GetType().Name}]: {ex.Message}");
                }
            }

            _systemModules.Clear();
            _managers.Clear();
            _services.Clear();

            Debug.Log("系统集成管理器已关闭");
        }

        /// <summary>
        /// 获取所有已注册的管理器
        /// </summary>
        public Dictionary<string, MonoBehaviour> GetAllManagers()
        {
            return new Dictionary<string, MonoBehaviour>(_managers);
        }

        /// <summary>
        /// 获取所有已注册的服务
        /// </summary>
        public Dictionary<string, object> GetAllServices()
        {
            return new Dictionary<string, object>(_services);
        }
    }

    /// <summary>
    /// 系统模块接口
    /// </summary>
    public interface ISystemModule
    {
        Action OnUpdate { get; set; }
        Action OnFixedUpdate { get; set; }
        Action OnShutdown { get; set; }
    }
}
