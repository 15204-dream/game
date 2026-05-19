using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Performance
{
    /// <summary>
    /// 性能优化器 - 统一管理游戏性能优化策略
    /// 根据设备性能自动调整游戏设置
    /// </summary>
    public class PerformanceOptimizer : MonoBehaviour
    {
        private static PerformanceOptimizer _instance;
        public static PerformanceOptimizer Instance
        {
            get { return _instance; }
        }

        [Header("性能配置")]
        [SerializeField] private bool _enableAutoOptimization = true;
        [SerializeField] private float _optimizationCheckInterval = 5f;
        [SerializeField] private int _targetFPS = 60;
        [SerializeField] private int _lowEndTargetFPS = 30;

        [Header("图形设置")]
        [SerializeField] private int _defaultQualityLevel = 2;
        [SerializeField] private int _lowEndQualityLevel = 0;
        [SerializeField] private bool _enableShadows = true;
        [SerializeField] private bool _enablePostProcessing = true;

        [Header("内存设置")]
        [SerializeField] private int _maxMemoryUsageMB = 512;
        [SerializeField] private float _memoryWarningThreshold = 0.8f;
        [SerializeField] private bool _enableAutoGarbageCollection = true;

        private float _lastOptimizationCheck = 0f;
        private int _currentQualityLevel;
        private bool _isLowPerformanceMode = false;
        private PerformanceProfile _currentProfile;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializePerformanceOptimizer();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        void Update()
        {
            if (!_enableAutoOptimization) return;

            if (Time.time - _lastOptimizationCheck > _optimizationCheckInterval)
            {
                _lastOptimizationCheck = Time.time;
                CheckPerformance();
            }
        }

        /// <summary>
        /// 初始化性能优化器
        /// </summary>
        private void InitializePerformanceOptimizer()
        {
            _currentProfile = DetectPerformanceProfile();
            ApplyQualitySettings();
        }

        /// <summary>
        /// 检测设备性能配置
        /// </summary>
        private PerformanceProfile DetectPerformanceProfile()
        {
            var profile = new PerformanceProfile();

            int systemMemoryMB = SystemInfo.systemMemorySize;
            int graphicsMemoryMB = SystemInfo.graphicsMemorySize;
            int processorCount = SystemInfo.processorCount;

            profile.systemMemoryMB = systemMemoryMB;
            profile.graphicsMemoryMB = graphicsMemoryMB;
            profile.processorCount = processorCount;
            profile.graphicsShaderLevel = SystemInfo.graphicsShaderLevel;

            if (systemMemoryMB < 2048 || graphicsMemoryMB < 512)
            {
                profile.tier = PerformanceTier.Low;
            }
            else if (systemMemoryMB < 4096 || graphicsMemoryMB < 1024)
            {
                profile.tier = PerformanceTier.Medium;
            }
            else
            {
                profile.tier = PerformanceTier.High;
            }

            return profile;
        }

        /// <summary>
        /// 检查性能状态
        /// </summary>
        private void CheckPerformance()
        {
            float fps = 1f / Time.deltaTime;
            float memoryUsage = GetMemoryUsageRatio();

            if (fps < _targetFPS * 0.6f || memoryUsage > _memoryWarningThreshold)
            {
                DowngradePerformance();
            }
            else if (fps > _targetFPS * 0.95f && memoryUsage < _memoryWarningThreshold * 0.5f)
            {
                TryUpgradePerformance();
            }
        }

        /// <summary>
        /// 降级性能设置
        /// </summary>
        public void DowngradePerformance()
        {
            if (_currentQualityLevel <= _lowEndQualityLevel) return;

            _currentQualityLevel--;
            _isLowPerformanceMode = true;

            QualitySettings.SetQualityLevel(_currentQualityLevel);
            DisableExpensiveFeatures();

            Debug.Log($"性能已降级: 质量等级 {_currentQualityLevel}");
        }

        /// <summary>
        /// 尝试升级性能设置
        /// </summary>
        public void TryUpgradePerformance()
        {
            if (_currentQualityLevel >= QualitySettings.names.Length - 1) return;

            int newLevel = _currentQualityLevel + 1;
            QualitySettings.SetQualityLevel(newLevel);

            float fps = 1f / Time.deltaTime;
            if (fps > _targetFPS * 0.8f)
            {
                _currentQualityLevel = newLevel;
                EnableExpensiveFeatures();
                Debug.Log($"性能已升级: 质量等级 {_currentQualityLevel}");
            }
            else
            {
                QualitySettings.SetQualityLevel(_currentQualityLevel);
            }
        }

        /// <summary>
        /// 应用质量设置
        /// </summary>
        private void ApplyQualitySettings()
        {
            switch (_currentProfile.tier)
            {
                case PerformanceTier.Low:
                    _currentQualityLevel = _lowEndQualityLevel;
                    _targetFPS = _lowEndTargetFPS;
                    DisableExpensiveFeatures();
                    break;

                case PerformanceTier.Medium:
                    _currentQualityLevel = Mathf.Min(_defaultQualityLevel, 1);
                    break;

                case PerformanceTier.High:
                    _currentQualityLevel = _defaultQualityLevel;
                    break;
            }

            QualitySettings.SetQualityLevel(_currentQualityLevel);
        }

        /// <summary>
        /// 禁用高开销功能
        /// </summary>
        private void DisableExpensiveFeatures()
        {
            _enableShadows = false;
            _enablePostProcessing = false;

            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowResolution = ShadowResolution.Low;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        }

        /// <summary>
        /// 启用高开销功能
        /// </summary>
        private void EnableExpensiveFeatures()
        {
            if (_currentQualityLevel >= 2)
            {
                _enableShadows = true;
                _enablePostProcessing = true;
            }
        }

        /// <summary>
        /// 获取内存使用比例
        /// </summary>
        public float GetMemoryUsageRatio()
        {
            long totalMemory = System.GC.GetTotalMemory(false);
            float usageMB = totalMemory / (1024f * 1024f);
            return usageMB / _maxMemoryUsageMB;
        }

        /// <summary>
        /// 获取当前FPS
        /// </summary>
        public float GetCurrentFPS()
        {
            return 1f / Time.deltaTime;
        }

        /// <summary>
        /// 设置目标FPS
        /// </summary>
        public void SetTargetFPS(int fps)
        {
            _targetFPS = fps;
            Application.targetFrameRate = fps;
        }

        /// <summary>
        /// 获取当前性能配置
        /// </summary>
        public PerformanceProfile GetCurrentProfile()
        {
            return _currentProfile;
        }

        /// <summary>
        /// 是否为低性能模式
        /// </summary>
        public bool IsLowPerformanceMode()
        {
            return _isLowPerformanceMode;
        }

        /// <summary>
        /// 获取质量等级
        /// </summary>
        public int GetQualityLevel()
        {
            return _currentQualityLevel;
        }

        /// <summary>
        /// 强制设置质量等级
        /// </summary>
        public void SetQualityLevel(int level)
        {
            _currentQualityLevel = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(_currentQualityLevel);
        }
    }

    /// <summary>
    /// 性能配置
    /// </summary>
    [Serializable]
    public class PerformanceProfile
    {
        public int systemMemoryMB;
        public int graphicsMemoryMB;
        public int processorCount;
        public int graphicsShaderLevel;
        public PerformanceTier tier;
    }

    /// <summary>
    /// 性能等级
    /// </summary>
    public enum PerformanceTier
    {
        Low,
        Medium,
        High
    }
}
