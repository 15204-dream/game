using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Debug
{
    /// <summary>
    /// 性能分析器 - 监控和分析游戏性能指标
    /// 提供实时性能数据和历史记录
    /// </summary>
    public class PerformanceProfiler : MonoBehaviour
    {
        private static PerformanceProfiler _instance;
        public static PerformanceProfiler Instance
        {
            get { return _instance; }
        }

        [Header("性能分析配置")]
        [SerializeField] private int _sampleCount = 60;
        [SerializeField] private float _updateInterval = 0.5f;
        [SerializeField] private bool _enableMemoryTracking = true;
        [SerializeField] private bool _enableFPSAnalysis = true;

        private float _lastUpdateTime = 0f;
        private PerformanceSnapshot _currentSnapshot;
        private Queue<PerformanceSnapshot> _snapshotHistory = new Queue<PerformanceSnapshot>();

        private ProfilerSection _updateSection;
        private ProfilerSection _fixedUpdateSection;
        private ProfilerSection _lateUpdateSection;
        private ProfilerSection _renderSection;

        public PerformanceSnapshot CurrentSnapshot
        {
            get { return _currentSnapshot; }
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializeProfiler();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// 初始化性能分析器
        /// </summary>
        private void InitializeProfiler()
        {
            _updateSection = new ProfilerSection("Update");
            _fixedUpdateSection = new ProfilerSection("FixedUpdate");
            _lateUpdateSection = new ProfilerSection("LateUpdate");
            _renderSection = new ProfilerSection("Render");
        }

        void Update()
        {
            _updateSection.BeginSample();

            if (Time.time - _lastUpdateTime > _updateInterval)
            {
                _lastUpdateTime = Time.time;
                UpdatePerformanceSnapshot();
            }

            _updateSection.EndSample();
        }

        void FixedUpdate()
        {
            _fixedUpdateSection.BeginSample();
            _fixedUpdateSection.EndSample();
        }

        void LateUpdate()
        {
            _lateUpdateSection.BeginSample();
            _lateUpdateSection.EndSample();
        }

        void OnGUI()
        {
            _renderSection.BeginSample();
            _renderSection.EndSample();
        }

        /// <summary>
        /// 更新性能快照
        /// </summary>
        private void UpdatePerformanceSnapshot()
        {
            _currentSnapshot = CollectPerformanceData();

            _snapshotHistory.Enqueue(_currentSnapshot);
            if (_snapshotHistory.Count > _sampleCount)
            {
                _snapshotHistory.Dequeue();
            }
        }

        /// <summary>
        /// 收集性能数据
        /// </summary>
        private PerformanceSnapshot CollectPerformanceData()
        {
            var snapshot = new PerformanceSnapshot
            {
                Timestamp = Time.time
            };

            if (_enableFPSAnalysis)
            {
                snapshot.FPS = 1f / Time.deltaTime;
                snapshot.FrameTime = Time.deltaTime * 1000f;
            }

            if (_enableMemoryTracking)
            {
                snapshot.MemoryUsedMB = (float)System.GC.GetTotalMemory(false) / (1024f * 1024f);
                snapshot.MemoryAllocatedMB = Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
            }

            snapshot.UpdateTime = _updateSection.GetAverageTime();
            snapshot.FixedUpdateTime = _fixedUpdateSection.GetAverageTime();
            snapshot.LateUpdateTime = _lateUpdateSection.GetAverageTime();
            snapshot.RenderTime = _renderSection.GetAverageTime();

            snapshot.ActiveObjects = CountActiveObjects();
            snapshot.DrawCalls = UnityEngine.Rendering.Graphics.activeColorBuffer != null ? 
                UnityEngine.Rendering.Graphics.activeColorBuffer : null;

            return snapshot;
        }

        /// <summary>
        /// 统计活跃对象数量
        /// </summary>
        private int CountActiveObjects()
        {
            return GameObject.FindObjectsOfType<GameObject>().Length;
        }

        /// <summary>
        /// 开始采样指定区域
        /// </summary>
        public void BeginSample(string section)
        {
            GetSection(section)?.BeginSample();
        }

        /// <summary>
        /// 结束采样指定区域
        /// </summary>
        public void EndSample(string section)
        {
            GetSection(section)?.EndSample();
        }

        /// <summary>
        /// 获取指定区域
        /// </summary>
        private ProfilerSection GetSection(string name)
        {
            switch (name)
            {
                case "Update":
                    return _updateSection;
                case "FixedUpdate":
                    return _fixedUpdateSection;
                case "LateUpdate":
                    return _lateUpdateSection;
                case "Render":
                    return _renderSection;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取性能报告
        /// </summary>
        public PerformanceReport GetReport()
        {
            var report = new PerformanceReport
            {
                CurrentSnapshot = _currentSnapshot,
                SnapshotHistory = new List<PerformanceSnapshot>(_snapshotHistory)
            };

            if (_snapshotHistory.Count > 0)
            {
                float totalFPS = 0f;
                float totalMemory = 0f;
                float maxFPS = float.MinValue;
                float minFPS = float.MaxValue;

                foreach (var snapshot in _snapshotHistory)
                {
                    totalFPS += snapshot.FPS;
                    totalMemory += snapshot.MemoryUsedMB;
                    maxFPS = Mathf.Max(maxFPS, snapshot.FPS);
                    minFPS = Mathf.Min(minFPS, snapshot.FPS);
                }

                report.AverageFPS = totalFPS / _snapshotHistory.Count;
                report.AverageMemory = totalMemory / _snapshotHistory.Count;
                report.MaxFPS = maxFPS;
                report.MinFPS = minFPS;
                report.SampleCount = _snapshotHistory.Count;
            }

            return report;
        }

        /// <summary>
        /// 获取FPS历史
        /// </summary>
        public List<float> GetFPSHistory()
        {
            var history = new List<float>();
            foreach (var snapshot in _snapshotHistory)
            {
                history.Add(snapshot.FPS);
            }
            return history;
        }

        /// <summary>
        /// 获取内存历史
        /// </summary>
        public List<float> GetMemoryHistory()
        {
            var history = new List<float>();
            foreach (var snapshot in _snapshotHistory)
            {
                history.Add(snapshot.MemoryUsedMB);
            }
            return history;
        }

        /// <summary>
        /// 重置统计数据
        /// </summary>
        public void Reset()
        {
            _snapshotHistory.Clear();
            _updateSection.Reset();
            _fixedUpdateSection.Reset();
            _lateUpdateSection.Reset();
            _renderSection.Reset();
        }
    }

    /// <summary>
    /// 性能快照
    /// </summary>
    [Serializable]
    public class PerformanceSnapshot
    {
        public float Timestamp;
        public float FPS;
        public float FrameTime;
        public float MemoryUsedMB;
        public float MemoryAllocatedMB;
        public float UpdateTime;
        public float FixedUpdateTime;
        public float LateUpdateTime;
        public float RenderTime;
        public int ActiveObjects;
        public object DrawCalls;
    }

    /// <summary>
    /// 性能报告
    /// </summary>
    [Serializable]
    public class PerformanceReport
    {
        public PerformanceSnapshot CurrentSnapshot;
        public List<PerformanceSnapshot> SnapshotHistory;
        public float AverageFPS;
        public float AverageMemory;
        public float MaxFPS;
        public float MinFPS;
        public int SampleCount;
    }

    /// <summary>
    /// 性能分析区域
    /// </summary>
    public class ProfilerSection
    {
        private string _name;
        private List<float> _samples = new List<float>();
        private int _maxSamples = 60;
        private float _lastBeginTime;

        public ProfilerSection(string name)
        {
            _name = name;
        }

        public void BeginSample()
        {
            _lastBeginTime = Time.time;
        }

        public void EndSample()
        {
            float elapsed = (Time.time - _lastBeginTime) * 1000f;
            _samples.Add(elapsed);

            if (_samples.Count > _maxSamples)
            {
                _samples.RemoveAt(0);
            }
        }

        public float GetAverageTime()
        {
            if (_samples.Count == 0) return 0f;
            float sum = 0f;
            foreach (var sample in _samples)
            {
                sum += sample;
            }
            return sum / _samples.Count;
        }

        public void Reset()
        {
            _samples.Clear();
        }
    }
}
