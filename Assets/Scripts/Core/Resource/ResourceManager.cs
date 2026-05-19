using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Resource
{
    /// <summary>
    /// 资源管理器 - 统一管理游戏资源的加载和卸载
    /// 支持同步/异步加载、资源缓存、依赖管理等
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        private static ResourceManager _instance;
        public static ResourceManager Instance
        {
            get { return _instance; }
        }

        [Header("资源配置")]
        [SerializeField] private bool _enableCache = true;
        [SerializeField] private int _maxCachedResources = 100;
        [SerializeField] private bool _enableAsyncLoading = true;
        [SerializeField] private int _maxConcurrentLoads = 4;

        private Dictionary<string, ResourceRequest> _pendingLoads = new Dictionary<string, ResourceRequest>();
        private Dictionary<string, UnityEngine.Object> _cachedResources = new Dictionary<string, UnityEngine.Object>();
        private Queue<ResourceRequest> _loadQueue = new Queue<ResourceRequest>();
        private int _currentLoadCount = 0;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            if (_enableCache && _cachedResources.ContainsKey(path))
            {
                return _cachedResources[path] as T;
            }

            var resource = Resources.Load<T>(path);

            if (resource != null && _enableCache)
            {
                CacheResource(path, resource);
            }

            return resource;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAsync<T>(string path, Action<T> onComplete, Action<float> onProgress = null) where T : UnityEngine.Object
        {
            if (_enableCache && _cachedResources.ContainsKey(path))
            {
                onComplete?.Invoke(_cachedResources[path] as T);
                return;
            }

            if (_currentLoadCount >= _maxConcurrentLoads)
            {
                _loadQueue.Enqueue(new ResourceRequest
                {
                    Path = path,
                    Type = typeof(T),
                    OnComplete = (obj) => onComplete?.Invoke(obj as T),
                    OnProgress = onProgress
                });
                return;
            }

            StartCoroutine(LoadAsyncCoroutine(path, typeof(T), 
                (obj) => {
                    _currentLoadCount--;
                    onComplete?.Invoke(obj as T);
                    ProcessLoadQueue();
                }, 
                onProgress));
        }

        /// <summary>
        /// 异步加载协程
        /// </summary>
        private IEnumerator LoadAsyncCoroutine(string path, Type type, Action<UnityEngine.Object> onComplete, Action<float> onProgress)
        {
            _currentLoadCount++;
            var request = Resources.LoadAsync(path, type);

            while (!request.isDone)
            {
                onProgress?.Invoke(request.progress);
                yield return null;
            }

            if (request.asset != null && _enableCache)
            {
                CacheResource(path, request.asset);
            }

            onProgress?.Invoke(1f);
            onComplete?.Invoke(request.asset);
        }

        /// <summary>
        /// 处理加载队列
        /// </summary>
        private void ProcessLoadQueue()
        {
            while (_loadQueue.Count > 0 && _currentLoadCount < _maxConcurrentLoads)
            {
                var request = _loadQueue.Dequeue();
                LoadAsync(request.Path, request.OnComplete, request.OnProgress);
            }
        }

        /// <summary>
        /// 缓存资源
        /// </summary>
        private void CacheResource(string path, UnityEngine.Object resource)
        {
            if (_cachedResources.Count >= _maxCachedResources)
            {
                RemoveOldestCachedResource();
            }
            _cachedResources[path] = resource;
        }

        /// <summary>
        /// 移除最旧的缓存资源
        /// </summary>
        private void RemoveOldestCachedResource()
        {
            var enumerator = _cachedResources.GetEnumerator();
            if (enumerator.MoveNext())
            {
                _cachedResources.Remove(enumerator.Current.Key);
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        public void Unload(string path)
        {
            if (_cachedResources.ContainsKey(path))
            {
                Resources.UnloadAsset(_cachedResources[path]);
                _cachedResources.Remove(path);
            }
        }

        /// <summary>
        /// 卸载所有缓存资源
        /// </summary>
        public void UnloadAllCached()
        {
            foreach (var resource in _cachedResources.Values)
            {
                Resources.UnloadAsset(resource);
            }
            _cachedResources.Clear();
        }

        /// <summary>
        /// 加载多个资源
        /// </summary>
        public void LoadAll<T>(string folderPath, Action<T[]> onComplete) where T : UnityEngine.Object
        {
            StartCoroutine(LoadAllCoroutine(folderPath, onComplete));
        }

        /// <summary>
        /// 加载所有资源协程
        /// </summary>
        private IEnumerator LoadAllCoroutine<T>(string folderPath, Action<T[]> onComplete) where T : UnityEngine.Object
        {
            var resources = Resources.LoadAll<T>(folderPath);
            yield return resources;

            foreach (var resource in resources)
            {
                if (_enableCache && !_cachedResources.ContainsKey(resource.name))
                {
                    _cachedResources[resource.name] = resource;
                }
            }

            onComplete?.Invoke(resources);
        }

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        public bool Exists(string path)
        {
            if (_enableCache && _cachedResources.ContainsKey(path))
            {
                return true;
            }
            return Resources.Load(path) != null;
        }

        /// <summary>
        /// 获取缓存的资源数量
        /// </summary>
        public int GetCachedCount()
        {
            return _cachedResources.Count;
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        public ResourceInfo GetResourceInfo(string path)
        {
            return new ResourceInfo
            {
                Path = path,
                IsCached = _cachedResources.ContainsKey(path),
                Size = 0
            };
        }
    }

    /// <summary>
    /// 资源请求
    /// </summary>
    [Serializable]
    public class ResourceRequest
    {
        public string Path;
        public Type Type;
        public Action<UnityEngine.Object> OnComplete;
        public Action<float> OnProgress;
    }

    /// <summary>
    /// 资源信息
    /// </summary>
    [Serializable]
    public class ResourceInfo
    {
        public string Path;
        public bool IsCached;
        public long Size;
    }
}
