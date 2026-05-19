using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Integration
{
    /// <summary>
    /// 服务定位器 - 实现依赖注入和服务查找
    /// 提供全局服务访问接口
    /// </summary>
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator _instance;
        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ServiceLocator>();
                    if (_instance == null)
                    {
                        var go = new GameObject("ServiceLocator");
                        _instance = go.AddComponent<ServiceLocator>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private Dictionary<string, object> _namedServices = new Dictionary<string, object>();
        private readonly object _lock = new object();

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDefaultServices();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 初始化默认服务
        /// </summary>
        private void InitializeDefaultServices()
        {
            RegisterService<EventBus>(EventBus.Instance);
            RegisterService<GameLoopManager>(gameObject.AddComponent<GameLoopManager>());
            RegisterService<PerformanceOptimizer>(gameObject.AddComponent<PerformanceOptimizer>());
            RegisterService<MemoryManager>(gameObject.AddComponent<MemoryManager>());
            RegisterService<ObjectPoolManager>(gameObject.AddComponent<ObjectPoolManager>());
            RegisterService<CacheManager>(gameObject.AddComponent<CacheManager>());
            RegisterService<ResourceManager>(gameObject.AddComponent<ResourceManager>());
            RegisterService<DebugConsole>(gameObject.AddComponent<DebugConsole>());
            RegisterService<GameLogger>(gameObject.AddComponent<GameLogger>());
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        public void RegisterService<T>(T service) where T : class
        {
            var type = typeof(T);
            lock (_lock)
            {
                if (_services.ContainsKey(type))
                {
                    Debug.LogWarning($"服务 {type.Name} 已存在，将被替换");
                    _services[type] = service;
                }
                else
                {
                    _services.Add(type, service);
                }
            }
        }

        /// <summary>
        /// 注册命名服务
        /// </summary>
        public void RegisterService<T>(string name, T service) where T : class
        {
            lock (_lock)
            {
                if (_namedServices.ContainsKey(name))
                {
                    Debug.LogWarning($"命名服务 {name} 已存在，将被替换");
                    _namedServices[name] = service;
                }
                else
                {
                    _namedServices.Add(name, service);
                }
            }
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        public T GetService<T>() where T : class
        {
            var type = typeof(T);
            lock (_lock)
            {
                if (_services.ContainsKey(type))
                {
                    return _services[type] as T;
                }
            }
            Debug.LogError($"服务 {type.Name} 未注册");
            return null;
        }

        /// <summary>
        /// 获取命名服务
        /// </summary>
        public T GetService<T>(string name) where T : class
        {
            lock (_lock)
            {
                if (_namedServices.ContainsKey(name))
                {
                    return _namedServices[name] as T;
                }
            }
            Debug.LogError($"命名服务 {name} 未注册");
            return null;
        }

        /// <summary>
        /// 检查服务是否存在
        /// </summary>
        public bool IsServiceRegistered<T>() where T : class
        {
            var type = typeof(T);
            lock (_lock)
            {
                return _services.ContainsKey(type);
            }
        }

        /// <summary>
        /// 取消注册服务
        /// </summary>
        public void UnregisterService<T>() where T : class
        {
            var type = typeof(T);
            lock (_lock)
            {
                _services.Remove(type);
            }
        }

        /// <summary>
        /// 取消注册命名服务
        /// </summary>
        public void UnregisterService(string name)
        {
            lock (_lock)
            {
                _namedServices.Remove(name);
            }
        }

        /// <summary>
        /// 清除所有服务
        /// </summary>
        public void ClearAll()
        {
            lock (_lock)
            {
                _services.Clear();
                _namedServices.Clear();
            }
        }

        /// <summary>
        /// 获取所有已注册服务类型
        /// </summary>
        public List<Type> GetAllRegisteredServices()
        {
            lock (_lock)
            {
                return new List<Type>(_services.Keys);
            }
        }
    }
}
