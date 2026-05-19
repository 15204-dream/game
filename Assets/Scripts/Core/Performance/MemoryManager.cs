using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace Assets.Scripts.Core.Performance
{
    /// <summary>
    /// 内存管理器 - 监控和管理游戏内存使用
    /// 提供内存分配追踪、自动释放、内存警告等功能
    /// </summary>
    public class MemoryManager : MonoBehaviour
    {
        private static MemoryManager _instance;
        public static MemoryManager Instance
        {
            get { return _instance; }
        }

        [Header("内存监控配置")]
        [SerializeField] private int _maxMemoryMB = 512;
        [SerializeField] private float _warningThreshold = 0.8f;
        [SerializeField] private float _criticalThreshold = 0.95f;
        [SerializeField] private float _checkInterval = 5f;

        [Header("自动内存管理")]
        [SerializeField] private bool _enableAutoGC = true;
        [SerializeField] private float _gcInterval = 30f;
        [SerializeField] private bool _enableAutoUnloadUnused = true;

        private float _lastCheckTime = 0f;
        private float _lastGCTime = 0f;
        private long _peakMemory = 0L;
        private bool _isLowMemory = false;

        private List<MemoryAllocation> _allocations = new List<MemoryAllocation>();
        private Dictionary<string, long> _allocationStats = new Dictionary<string, long>();

        public long CurrentMemoryUsage
        {
            get { return System.GC.GetTotalMemory(false); }
        }

        public float MemoryUsageRatio
        {
            get { return (float)CurrentMemoryUsage / (_maxMemoryMB * 1024 * 1024); }
        }

        public long PeakMemory
        {
            get { return _peakMemory; }
        }

        public bool IsLowMemory
        {
            get { return _isLowMemory; }
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

        void Update()
        {
            if (Time.time - _lastCheckTime > _checkInterval)
            {
                _lastCheckTime = Time.time;
                CheckMemoryStatus();
            }

            if (_enableAutoGC && Time.time - _lastGCTime > _gcInterval)
            {
                _lastGCTime = Time.time;
                TriggerGarbageCollection();
            }
        }

        /// <summary>
        /// 检查内存状态
        /// </summary>
        private void CheckMemoryStatus()
        {
            long currentMemory = CurrentMemoryUsage;
            if (currentMemory > _peakMemory)
            {
                _peakMemory = currentMemory;
            }

            float usageRatio = MemoryUsageRatio;

            if (usageRatio >= _criticalThreshold)
            {
                OnCriticalMemory();
            }
            else if (usageRatio >= _warningThreshold)
            {
                OnMemoryWarning();
            }

            if (_isLowMemory && usageRatio < _warningThreshold)
            {
                _isLowMemory = false;
            }
        }

        /// <summary>
        /// 内存警告处理
        /// </summary>
        private void OnMemoryWarning()
        {
            Debug.LogWarning($"内存使用警告: {MemoryUsageRatio:P0}");

            EventBus.Instance.Publish("OnMemoryWarning", new MemoryWarningEvent
            {
                UsageRatio = MemoryUsageRatio,
                CurrentMB = CurrentMemoryUsage / (1024 * 1024),
                MaxMB = _maxMemoryMB
            });

            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 严重内存警告处理
        /// </summary>
        private void OnCriticalMemory()
        {
            Debug.LogError($"严重内存警告: {MemoryUsageRatio:P0}");

            EventBus.Instance.Publish("OnCriticalMemory", new MemoryWarningEvent
            {
                UsageRatio = MemoryUsageRatio,
                CurrentMB = CurrentMemoryUsage / (1024 * 1024),
                MaxMB = _maxMemoryMB
            });

            TriggerGarbageCollection();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        /// <summary>
        /// 触发垃圾回收
        /// </summary>
        public void TriggerGarbageCollection()
        {
            long beforeGC = CurrentMemoryUsage;

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();

            long afterGC = CurrentMemoryUsage;
            long freedMemory = beforeGC - afterGC;

            Debug.Log($"GC完成: 释放 {freedMemory / (1024 * 1024)}MB");

            EventBus.Instance.Publish("OnGarbageCollected", new GCEvent
            {
                BeforeMB = beforeGC / (1024 * 1024),
                AfterMB = afterGC / (1024 * 1024),
                FreedMB = freedMemory / (1024 * 1024)
            });
        }

        /// <summary>
        /// 记录内存分配
        /// </summary>
        public void RecordAllocation(string category, long size)
        {
            _allocations.Add(new MemoryAllocation
            {
                Category = category,
                Size = size,
                Timestamp = Time.time
            });

            if (_allocationStats.ContainsKey(category))
            {
                _allocationStats[category] += size;
            }
            else
            {
                _allocationStats[category] = size;
            }
        }

        /// <summary>
        /// 获取分配统计
        /// </summary>
        public Dictionary<string, long> GetAllocationStats()
        {
            return new Dictionary<string, long>(_allocationStats);
        }

        /// <summary>
        /// 清除分配统计
        /// </summary>
        public void ClearAllocationStats()
        {
            _allocations.Clear();
            _allocationStats.Clear();
        }

        /// <summary>
        /// 卸载未使用资源
        /// </summary>
        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 清除特定资源
        /// </summary>
        public void ClearAsset(string resourcePath)
        {
            Resources.UnloadAsset(Resources.Load(resourcePath));
        }

        /// <summary>
        /// 获取内存信息字符串
        /// </summary>
        public string GetMemoryInfo()
        {
            return $"当前内存: {CurrentMemoryUsage / (1024 * 1024)}MB / {_maxMemoryMB}MB ({MemoryUsageRatio:P0})\n" +
                   $"峰值内存: {_peakMemory / (1024 * 1024)}MB";
        }

        /// <summary>
        /// 强制清理
        /// </summary>
        public void ForceCleanup()
        {
            TriggerGarbageCollection();
            UnloadUnusedAssets();
            ClearAllocationStats();
        }

        /// <summary>
        /// 获取推荐的最大缓存大小
        /// </summary>
        public int GetRecommendedCacheSize()
        {
            float availableMemory = _maxMemoryMB * (1f - _warningThreshold);
            return Mathf.CeilToInt(availableMemory);
        }
    }

    /// <summary>
    /// 内存分配记录
    /// </summary>
    [Serializable]
    public class MemoryAllocation
    {
        public string Category;
        public long Size;
        public float Timestamp;
    }

    /// <summary>
    /// 内存警告事件
    /// </summary>
    [Serializable]
    public class MemoryWarningEvent
    {
        public float UsageRatio;
        public long CurrentMB;
        public long MaxMB;
    }

    /// <summary>
    /// GC事件
    /// </summary>
    [Serializable]
    public class GCEvent
    {
        public long BeforeMB;
        public long AfterMB;
        public long FreedMB;
    }
}
