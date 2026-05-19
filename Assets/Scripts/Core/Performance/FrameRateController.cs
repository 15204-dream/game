using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Performance
{
    /// <summary>
    /// 帧率控制器 - 管理游戏帧率和性能监控
    /// 支持动态帧率调整和性能指标追踪
    /// </summary>
    public class FrameRateController : MonoBehaviour
    {
        private static FrameRateController _instance;
        public static FrameRateController Instance
        {
            get { return _instance; }
        }

        [Header("帧率设置")]
        [SerializeField] private int _targetFrameRate = 60;
        [SerializeField] private int _mobileTargetFrameRate = 30;
        [SerializeField] private int _minFrameRate = 15;
        [SerializeField] private bool _enableVSync = false;

        [Header("自适应帧率")]
        [SerializeField] private bool _enableAdaptiveFPS = true;
        [SerializeField] private float _fpsDropThreshold = 0.6f;
        [SerializeField] private float _fpsRecoveryThreshold = 0.95f;
        [SerializeField] private float _adjustmentInterval = 2f;

        [Header("性能监控")]
        [SerializeField] private bool _enablePerformanceMonitor = true;
        [SerializeField] private int _sampleCount = 60;

        private float _currentFPS = 0f;
        private float _averageFPS = 0f;
        private float _minFPS = float.MaxValue;
        private float _maxFPS = 0f;
        private int _frameCount = 0;
        private float _fpsAccumulator = 0f;
        private float _lastFPSTime = 0f;

        private Queue<float> _fpsHistory = new Queue<float>();
        private float _lastAdjustmentTime = 0f;
        private int _currentTargetFPS;
        private bool _isLowPowerMode = false;

        public float CurrentFPS
        {
            get { return _currentFPS; }
        }

        public float AverageFPS
        {
            get { return _averageFPS; }
        }

        public float MinFPS
        {
            get { return _minFPS; }
        }

        public float MaxFPS
        {
            get { return _maxFPS; }
        }

        public int TargetFrameRate
        {
            get { return _currentTargetFPS; }
        }

        public bool IsLowPowerMode
        {
            get { return _isLowPowerMode; }
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                Initialize();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            InitializeFrameRate();
        }

        void Update()
        {
            UpdateFPS();

            if (_enableAdaptiveFPS)
            {
                TryAdjustFrameRate();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            Application.targetFrameRate = _targetFrameRate;
            _currentTargetFPS = _targetFrameRate;

            QualitySettings.vSyncCount = _enableVSync ? 1 : 0;
        }

        /// <summary>
        /// 初始化帧率
        /// </summary>
        private void InitializeFrameRate()
        {
#if UNITY_ANDROID || UNITY_IOS
            _currentTargetFPS = _mobileTargetFrameRate;
            Application.targetFrameRate = _mobileTargetFrameRate;
#else
            _currentTargetFPS = _targetFrameRate;
            Application.targetFrameRate = _targetFrameRate;
#endif
        }

        /// <summary>
        /// 更新FPS计算
        /// </summary>
        private void UpdateFPS()
        {
            _frameCount++;
            _fpsAccumulator += 1f / Time.deltaTime;

            if (Time.time - _lastFPSTime >= 1f)
            {
                _currentFPS = _fpsAccumulator / _frameCount;
                _fpsAccumulator = 0f;
                _frameCount = 0;
                _lastFPSTime = Time.time;

                UpdateFPSHistory();
                UpdateFPSStats();
            }
        }

        /// <summary>
        /// 更新FPS历史
        /// </summary>
        private void UpdateFPSHistory()
        {
            _fpsHistory.Enqueue(_currentFPS);
            if (_fpsHistory.Count > _sampleCount)
            {
                _fpsHistory.Dequeue();
            }

            float sum = 0f;
            foreach (var fps in _fpsHistory)
            {
                sum += fps;
            }
            _averageFPS = sum / _fpsHistory.Count;
        }

        /// <summary>
        /// 更新FPS统计
        /// </summary>
        private void UpdateFPSStats()
        {
            if (_currentFPS < _minFPS)
            {
                _minFPS = _currentFPS;
            }
            if (_currentFPS > _maxFPS)
            {
                _maxFPS = _currentFPS;
            }
        }

        /// <summary>
        /// 尝试调整帧率
        /// </summary>
        private void TryAdjustFrameRate()
        {
            if (Time.time - _lastAdjustmentTime < _adjustmentInterval)
            {
                return;
            }
            _lastAdjustmentTime = Time.time;

            if (_currentFPS < _currentTargetFPS * _fpsDropThreshold)
            {
                DecreaseFrameRate();
            }
            else if (_currentFPS > _currentTargetFPS * _fpsRecoveryThreshold && _currentTargetFPS < _targetFrameRate)
            {
                IncreaseFrameRate();
            }
        }

        /// <summary>
        /// 降低帧率
        /// </summary>
        public void DecreaseFrameRate()
        {
            int newFPS = _currentTargetFPS;
            if (newFPS > _minFrameRate)
            {
                newFPS = Mathf.Max(newFPS / 2, _minFrameRate);
                SetTargetFrameRate(newFPS);
            }
        }

        /// <summary>
        /// 提高帧率
        /// </summary>
        public void IncreaseFrameRate()
        {
            int newFPS = _currentTargetFPS;
            if (newFPS < _targetFrameRate)
            {
                newFPS = Mathf.Min(newFPS * 2, _targetFrameRate);
                SetTargetFrameRate(newFPS);
            }
        }

        /// <summary>
        /// 设置目标帧率
        /// </summary>
        public void SetTargetFrameRate(int fps)
        {
            _currentTargetFPS = Mathf.Clamp(fps, _minFrameRate, _targetFrameRate);
            Application.targetFrameRate = _currentTargetFPS;

            Debug.Log($"帧率已调整: {_currentTargetFPS} FPS");
        }

        /// <summary>
        /// 设置低功耗模式
        /// </summary>
        public void SetLowPowerMode(bool enable)
        {
            _isLowPowerMode = enable;
            if (enable)
            {
                SetTargetFrameRate(_minFrameRate);
                QualitySettings.SetQualityLevel(0);
            }
            else
            {
                SetTargetFrameRate(_targetFrameRate);
            }
        }

        /// <summary>
        /// 重置帧率
        /// </summary>
        public void ResetFrameRate()
        {
            SetTargetFrameRate(_targetFrameRate);
            _minFPS = float.MaxValue;
            _maxFPS = 0f;
        }

        /// <summary>
        /// 获取FPS报告
        /// </summary>
        public FPSReport GetReport()
        {
            return new FPSReport
            {
                CurrentFPS = _currentFPS,
                AverageFPS = _averageFPS,
                MinFPS = _minFPS == float.MaxValue ? 0 : _minFPS,
                MaxFPS = _maxFPS,
                TargetFPS = _currentTargetFPS,
                FPSHistory = new List<float>(_fpsHistory),
                IsLowPowerMode = _isLowPowerMode
            };
        }

        /// <summary>
        /// 获取性能评级
        /// </summary>
        public PerformanceRating GetPerformanceRating()
        {
            if (_averageFPS >= _targetFrameRate * 0.95f)
            {
                return PerformanceRating.Excellent;
            }
            if (_averageFPS >= _targetFrameRate * 0.7f)
            {
                return PerformanceRating.Good;
            }
            if (_averageFPS >= _targetFrameRate * 0.5f)
            {
                return PerformanceRating.Fair;
            }
            return PerformanceRating.Poor;
        }
    }

    /// <summary>
    /// FPS报告
    /// </summary>
    [Serializable]
    public class FPSReport
    {
        public float CurrentFPS;
        public float AverageFPS;
        public float MinFPS;
        public float MaxFPS;
        public int TargetFPS;
        public List<float> FPSHistory;
        public bool IsLowPowerMode;
    }

    /// <summary>
    /// 性能评级
    /// </summary>
    public enum PerformanceRating
    {
        Excellent,
        Good,
        Fair,
        Poor
    }
}
