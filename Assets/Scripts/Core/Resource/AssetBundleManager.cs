using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Resource
{
    /// <summary>
    /// AssetBundle管理器 - 管理AB包的加载和卸载
    /// 支持依赖解析、版本控制、缓存管理等
    /// </summary>
    public class AssetBundleManager : MonoBehaviour
    {
        private static AssetBundleManager _instance;
        public static AssetBundleManager Instance
        {
            get { return _instance; }
        }

        [Header("AssetBundle配置")]
        [SerializeField] private string _basePath = "";
        [SerializeField] private bool _useCaching = true;
        [SerializeField] private int _maxCachedBundles = 10;
        [SerializeField] private CachingCompressionOption _compression = CachingCompressionOption.LZ4;

        private Dictionary<string, AssetBundle> _loadedBundles = new Dictionary<string, AssetBundle>();
        private Dictionary<string, AssetBundleCreateRequest> _loadingRequests = new Dictionary<string, AssetBundleCreateRequest>();
        private Dictionary<string, string[]> _bundleDependencies = new Dictionary<string, string[]>();
        private Queue<string> _bundleLoadOrder = new Queue<string>();

        public enum CachingCompressionOption
        {
            None,
            LZ4,
            LZMA
        }

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
        /// 同步加载AssetBundle
        /// </summary>
        public AssetBundle LoadAssetBundle(string bundleName)
        {
            if (_loadedBundles.ContainsKey(bundleName))
            {
                return _loadedBundles[bundleName];
            }

            LoadDependencies(bundleName);

            string path = GetBundlePath(bundleName);
            var bundle = AssetBundle.LoadFromFile(path);

            if (bundle != null)
            {
                CacheBundle(bundleName, bundle);
            }

            return bundle;
        }

        /// <summary>
        /// 异步加载AssetBundle
        /// </summary>
        public void LoadAssetBundleAsync(string bundleName, Action<AssetBundle> onComplete)
        {
            if (_loadedBundles.ContainsKey(bundleName))
            {
                onComplete?.Invoke(_loadedBundles[bundleName]);
                return;
            }

            StartCoroutine(LoadAssetBundleCoroutine(bundleName, onComplete));
        }

        /// <summary>
        /// 异步加载协程
        /// </summary>
        private IEnumerator LoadAssetBundleCoroutine(string bundleName, Action<AssetBundle> onComplete)
        {
            LoadDependencies(bundleName);

            string path = GetBundlePath(bundleName);
            var request = AssetBundle.LoadFromFileAsync(path);
            _loadingRequests[bundleName] = request;

            yield return request;

            _loadingRequests.Remove(bundleName);

            if (request.assetBundle != null)
            {
                CacheBundle(bundleName, request.assetBundle);
                onComplete?.Invoke(request.assetBundle);
            }
            else
            {
                Debug.LogError($"AssetBundle加载失败: {bundleName}");
                onComplete?.Invoke(null);
            }
        }

        /// <summary>
        /// 加载依赖
        /// </summary>
        private void LoadDependencies(string bundleName)
        {
            if (_bundleDependencies.ContainsKey(bundleName))
            {
                foreach (var dep in _bundleDependencies[bundleName])
                {
                    if (!_loadedBundles.ContainsKey(dep))
                    {
                        LoadAssetBundle(dep);
                    }
                }
            }
        }

        /// <summary>
        /// 获取Bundle路径
        /// </summary>
        private string GetBundlePath(string bundleName)
        {
            return string.IsNullOrEmpty(_basePath) 
                ? bundleName 
                : $"{_basePath}/{bundleName}";
        }

        /// <summary>
        /// 缓存Bundle
        /// </summary>
        private void CacheBundle(string bundleName, AssetBundle bundle)
        {
            if (_loadedBundles.Count >= _maxCachedBundles)
            {
                UnloadOldestBundle();
            }

            _loadedBundles[bundleName] = bundle;
            _bundleLoadOrder.Enqueue(bundleName);
        }

        /// <summary>
        /// 卸载最旧的Bundle
        /// </summary>
        private void UnloadOldestBundle()
        {
            if (_bundleLoadOrder.Count > 0)
            {
                string oldest = _bundleLoadOrder.Dequeue();
                UnloadAssetBundle(oldest);
            }
        }

        /// <summary>
        /// 卸载AssetBundle
        /// </summary>
        public void UnloadAssetBundle(string bundleName, bool unloadAllLoadedObjects = false)
        {
            if (_loadedBundles.ContainsKey(bundleName))
            {
                _loadedBundles[bundleName].Unload(unloadAllLoadedObjects);
                _loadedBundles.Remove(bundleName);
            }
        }

        /// <summary>
        /// 卸载所有AssetBundle
        /// </summary>
        public void UnloadAll(bool unloadAllLoadedObjects = false)
        {
            foreach (var bundle in _loadedBundles.Values)
            {
                bundle.Unload(unloadAllLoadedObjects);
            }
            _loadedBundles.Clear();
            _bundleLoadOrder.Clear();
        }

        /// <summary>
        /// 从Bundle加载资源
        /// </summary>
        public T LoadAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            var bundle = LoadAssetBundle(bundleName);
            if (bundle != null)
            {
                return bundle.LoadAsset<T>(assetName);
            }
            return null;
        }

        /// <summary>
        /// 异步从Bundle加载资源
        /// </summary>
        public void LoadAssetAsync<T>(string bundleName, string assetName, Action<T> onComplete) where T : UnityEngine.Object
        {
            LoadAssetBundleAsync(bundleName, (bundle) =>
            {
                if (bundle != null)
                {
                    var request = bundle.LoadAssetAsync<T>(assetName);
                    StartCoroutine(HandleAssetRequest(request, onComplete));
                }
                else
                {
                    onComplete?.Invoke(null);
                }
            });
        }

        /// <summary>
        /// 处理资源请求
        /// </summary>
        private IEnumerator HandleAssetRequest<T>(AssetBundleRequest request, Action<T> onComplete) where T : UnityEngine.Object
        {
            yield return request;
            onComplete?.Invoke(request.asset as T);
        }

        /// <summary>
        /// 获取已加载的Bundle数量
        /// </summary>
        public int GetLoadedBundleCount()
        {
            return _loadedBundles.Count;
        }

        /// <summary>
        /// 检查Bundle是否已加载
        /// </summary>
        public bool IsBundleLoaded(string bundleName)
        {
            return _loadedBundles.ContainsKey(bundleName);
        }

        /// <summary>
        /// 注册依赖关系
        /// </summary>
        public void RegisterDependencies(string bundleName, string[] dependencies)
        {
            _bundleDependencies[bundleName] = dependencies;
        }
    }
}
