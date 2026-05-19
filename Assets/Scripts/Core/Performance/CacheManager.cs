using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Performance
{
    /// <summary>
    /// 缓存管理器 - 管理游戏中的各种缓存
    /// 支持TTL过期、LRU淘汰、最大容量限制等
    /// </summary>
    public class CacheManager : MonoBehaviour
    {
        private static CacheManager _instance;
        public static CacheManager Instance
        {
            get { return _instance; }
        }

        [Header("缓存配置")]
        [SerializeField] private int _defaultMaxSize = 100;
        [SerializeField] private float _defaultTTL = 300f;
        [SerializeField] private bool _enableAutoCleanup = true;
        [SerializeField] private float _cleanupInterval = 60f;

        private Dictionary<string, CacheEntry> _caches = new Dictionary<string, CacheEntry>();
        private Dictionary<string, CacheConfig> _cacheConfigs = new Dictionary<string, CacheConfig>();
        private float _lastCleanupTime = 0f;

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

        void Update()
        {
            if (_enableAutoCleanup && Time.time - _lastCleanupTime > _cleanupInterval)
            {
                _lastCleanupTime = Time.time;
                CleanupExpiredEntries();
            }
        }

        /// <summary>
        /// 注册缓存
        /// </summary>
        public void RegisterCache(string cacheName, int maxSize, float ttl)
        {
            if (!_cacheConfigs.ContainsKey(cacheName))
            {
                _cacheConfigs[cacheName] = new CacheConfig
                {
                    Name = cacheName,
                    MaxSize = maxSize,
                    TTL = ttl
                };
                _caches[cacheName] = new CacheEntry
                {
                    Entries = new Dictionary<string, CacheItem>(),
                    AccessOrder = new List<string>()
                };
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        public void Set<T>(string cacheName, string key, T value)
        {
            if (!_caches.ContainsKey(cacheName))
            {
                RegisterCache(cacheName, _defaultMaxSize, _defaultTTL);
            }

            var cache = _caches[cacheName];
            var config = GetOrCreateConfig(cacheName);

            if (cache.Entries.ContainsKey(key))
            {
                cache.Entries[key].Value = value;
                cache.Entries[key].LastAccessTime = Time.time;
                UpdateAccessOrder(cacheName, key);
            }
            else
            {
                if (cache.Entries.Count >= config.MaxSize)
                {
                    EvictLRU(cacheName);
                }

                cache.Entries[key] = new CacheItem
                {
                    Key = key,
                    Value = value,
                    CreatedTime = Time.time,
                    LastAccessTime = Time.time,
                    TTL = config.TTL
                };
                cache.AccessOrder.Add(key);
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        public T Get<T>(string cacheName, string key)
        {
            if (!_caches.ContainsKey(cacheName) || !_caches[cacheName].Entries.ContainsKey(key))
            {
                return default(T);
            }

            var item = _caches[cacheName].Entries[key];

            if (IsExpired(item))
            {
                Remove(cacheName, key);
                return default(T);
            }

            item.LastAccessTime = Time.time;
            UpdateAccessOrder(cacheName, key);

            return (T)item.Value;
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        public bool Contains(string cacheName, string key)
        {
            if (!_caches.ContainsKey(cacheName))
            {
                return false;
            }

            var cache = _caches[cacheName];
            if (!cache.Entries.ContainsKey(key))
            {
                return false;
            }

            if (IsExpired(cache.Entries[key]))
            {
                Remove(cacheName, key);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        public void Remove(string cacheName, string key)
        {
            if (_caches.ContainsKey(cacheName) && _caches[cacheName].Entries.ContainsKey(key))
            {
                _caches[cacheName].Entries.Remove(key);
                _caches[cacheName].AccessOrder.Remove(key);
            }
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear(string cacheName)
        {
            if (_caches.ContainsKey(cacheName))
            {
                _caches[cacheName].Entries.Clear();
                _caches[cacheName].AccessOrder.Clear();
            }
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void ClearAll()
        {
            foreach (var cache in _caches.Values)
            {
                cache.Entries.Clear();
                cache.AccessOrder.Clear();
            }
        }

        /// <summary>
        /// 获取缓存统计
        /// </summary>
        public CacheStats GetStats(string cacheName)
        {
            if (!_caches.ContainsKey(cacheName))
            {
                return null;
            }

            var cache = _caches[cacheName];
            var config = GetOrCreateConfig(cacheName);

            int expiredCount = 0;
            foreach (var item in cache.Entries.Values)
            {
                if (IsExpired(item))
                {
                    expiredCount++;
                }
            }

            return new CacheStats
            {
                CacheName = cacheName,
                EntryCount = cache.Entries.Count,
                MaxSize = config.MaxSize,
                TTL = config.TTL,
                ExpiredCount = expiredCount
            };
        }

        /// <summary>
        /// 获取所有缓存统计
        /// </summary>
        public Dictionary<string, CacheStats> GetAllStats()
        {
            var stats = new Dictionary<string, CacheStats>();
            foreach (var cacheName in _caches.Keys)
            {
                stats[cacheName] = GetStats(cacheName);
            }
            return stats;
        }

        /// <summary>
        /// 清理过期条目
        /// </summary>
        private void CleanupExpiredEntries()
        {
            foreach (var cacheName in _caches.Keys)
            {
                var cache = _caches[cacheName];
                var expiredKeys = new List<string>();

                foreach (var kvp in cache.Entries)
                {
                    if (IsExpired(kvp.Value))
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }

                foreach (var key in expiredKeys)
                {
                    Remove(cacheName, key);
                }
            }
        }

        /// <summary>
        /// LRU淘汰
        /// </summary>
        private void EvictLRU(string cacheName)
        {
            var cache = _caches[cacheName];
            if (cache.AccessOrder.Count > 0)
            {
                string lruKey = cache.AccessOrder[0];
                Remove(cacheName, lruKey);
            }
        }

        /// <summary>
        /// 更新访问顺序
        /// </summary>
        private void UpdateAccessOrder(string cacheName, string key)
        {
            var cache = _caches[cacheName];
            cache.AccessOrder.Remove(key);
            cache.AccessOrder.Add(key);
        }

        /// <summary>
        /// 检查是否过期
        /// </summary>
        private bool IsExpired(CacheItem item)
        {
            if (item.TTL <= 0)
            {
                return false;
            }
            return Time.time - item.CreatedTime > item.TTL;
        }

        /// <summary>
        /// 获取或创建配置
        /// </summary>
        private CacheConfig GetOrCreateConfig(string cacheName)
        {
            if (!_cacheConfigs.ContainsKey(cacheName))
            {
                _cacheConfigs[cacheName] = new CacheConfig
                {
                    Name = cacheName,
                    MaxSize = _defaultMaxSize,
                    TTL = _defaultTTL
                };
            }
            return _cacheConfigs[cacheName];
        }
    }

    /// <summary>
    /// 缓存配置
    /// </summary>
    [Serializable]
    public class CacheConfig
    {
        public string Name;
        public int MaxSize;
        public float TTL;
    }

    /// <summary>
    /// 缓存条目
    /// </summary>
    [Serializable]
    public class CacheEntry
    {
        public Dictionary<string, CacheItem> Entries;
        public List<string> AccessOrder;
    }

    /// <summary>
    /// 缓存项
    /// </summary>
    [Serializable]
    public class CacheItem
    {
        public string Key;
        public object Value;
        public float CreatedTime;
        public float LastAccessTime;
        public float TTL;
    }

    /// <summary>
    /// 缓存统计
    /// </summary>
    [Serializable]
    public class CacheStats
    {
        public string CacheName;
        public int EntryCount;
        public int MaxSize;
        public float TTL;
        public int ExpiredCount;
    }
}
