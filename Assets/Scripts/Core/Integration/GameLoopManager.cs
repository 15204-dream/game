using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Integration
{
    /// <summary>
    /// 游戏循环管理器 - 管理游戏的主循环逻辑
    /// 包括初始化、更新、暂停等生命周期管理
    /// </summary>
    public class GameLoopManager : MonoBehaviour
    {
        private static GameLoopManager _instance;
        public static GameLoopManager Instance
        {
            get { return _instance; }
        }

        public enum GameState
        {
            Initializing,
            MainMenu,
            Playing,
            Paused,
            Loading,
            Saving,
            Quitting
        }

        private GameState _currentState = GameState.Initializing;
        private GameState _previousState = GameState.Initializing;
        
        private bool _isPaused = false;
        private bool _isQuitting = false;
        
        private float _gameTime = 0f;
        private float _deltaTime = 0f;
        private float _fixedDeltaTime = 0f;
        
        private List<Action> _onUpdateCallbacks = new List<Action>();
        private List<Action> _onFixedUpdateCallbacks = new List<Action>();
        private List<Action> _onLateUpdateCallbacks = new List<Action>();
        private Dictionary<GameState, Action> _onStateEnterCallbacks = new Dictionary<GameState, Action>();
        private Dictionary<GameState, Action> _onStateExitCallbacks = new Dictionary<GameState, Action>();

        public GameState CurrentState
        {
            get { return _currentState; }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
        }

        public float GameTime
        {
            get { return _gameTime; }
        }

        public float DeltaTime
        {
            get { return _deltaTime; }
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
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
            ChangeState(GameState.MainMenu);
        }

        void Update()
        {
            if (_isQuitting) return;

            _deltaTime = Time.deltaTime;
            if (!_isPaused)
            {
                _gameTime += _deltaTime;
            }

            if (!_isPaused)
            {
                ExecuteCallbacks(_onUpdateCallbacks);
            }
        }

        void FixedUpdate()
        {
            if (_isQuitting || _isPaused) return;

            _fixedDeltaTime = Time.fixedDeltaTime;
            ExecuteCallbacks(_onFixedUpdateCallbacks);
        }

        void LateUpdate()
        {
            if (_isQuitting) return;

            if (!_isPaused)
            {
                ExecuteCallbacks(_onLateUpdateCallbacks);
            }
        }

        void OnDestroy()
        {
            _isQuitting = true;
        }

        void OnApplicationQuit()
        {
            _isQuitting = true;
            EventBus.Instance.Publish(GameEvents.OnGameEnd, null);
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        /// <summary>
        /// 初始化游戏循环管理器
        /// </summary>
        private void Initialize()
        {
            RegisterDefaultCallbacks();
            EventBus.Instance.Subscribe(GameEvents.OnGameStart, (Action<object>)(data => ChangeState(GameState.Playing)));
        }

        /// <summary>
        /// 注册默认回调
        /// </summary>
        private void RegisterDefaultCallbacks()
        {
            OnStateEnter(GameState.Playing, () =>
            {
                EventBus.Instance.Publish(GameEvents.OnGameStart, null);
            });

            OnStateExit(GameState.Playing, () =>
            {
                EventBus.Instance.Publish(GameEvents.OnGameEnd, null);
            });
        }

        /// <summary>
        /// 改变游戏状态
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (_currentState == newState) return;

            _previousState = _currentState;
            
            if (_onStateExitCallbacks.ContainsKey(_currentState))
            {
                _onStateExitCallbacks[_currentState]?.Invoke();
            }

            _currentState = newState;

            if (_onStateEnterCallbacks.ContainsKey(_currentState))
            {
                _onStateEnterCallbacks[_currentState]?.Invoke();
            }

            EventBus.Instance.Publish($"OnStateChange_{_currentState}", null);
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void Pause()
        {
            if (_isPaused) return;

            _isPaused = true;
            Time.timeScale = 0f;
            ChangeState(GameState.Paused);
        }

        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void Resume()
        {
            if (!_isPaused) return;

            _isPaused = false;
            Time.timeScale = 1f;
            ChangeState(GameState.Playing);
        }

        /// <summary>
        /// 切换暂停状态
        /// </summary>
        public void TogglePause()
        {
            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        /// <summary>
        /// 注册Update回调
        /// </summary>
        public void RegisterUpdateCallback(Action callback)
        {
            if (!_onUpdateCallbacks.Contains(callback))
            {
                _onUpdateCallbacks.Add(callback);
            }
        }

        /// <summary>
        /// 取消注册Update回调
        /// </summary>
        public void UnregisterUpdateCallback(Action callback)
        {
            _onUpdateCallbacks.Remove(callback);
        }

        /// <summary>
        /// 注册FixedUpdate回调
        /// </summary>
        public void RegisterFixedUpdateCallback(Action callback)
        {
            if (!_onFixedUpdateCallbacks.Contains(callback))
            {
                _onFixedUpdateCallbacks.Add(callback);
            }
        }

        /// <summary>
        /// 取消注册FixedUpdate回调
        /// </summary>
        public void UnregisterFixedUpdateCallback(Action callback)
        {
            _onFixedUpdateCallbacks.Remove(callback);
        }

        /// <summary>
        /// 注册LateUpdate回调
        /// </summary>
        public void RegisterLateUpdateCallback(Action callback)
        {
            if (!_onLateUpdateCallbacks.Contains(callback))
            {
                _onLateUpdateCallbacks.Add(callback);
            }
        }

        /// <summary>
        /// 取消注册LateUpdate回调
        /// </summary>
        public void UnregisterLateUpdateCallback(Action callback)
        {
            _onLateUpdateCallbacks.Remove(callback);
        }

        /// <summary>
        /// 注册状态进入回调
        /// </summary>
        public void OnStateEnter(GameState state, Action callback)
        {
            if (!_onStateEnterCallbacks.ContainsKey(state))
            {
                _onStateEnterCallbacks[state] = null;
            }
            _onStateEnterCallbacks[state] += callback;
        }

        /// <summary>
        /// 注册状态退出回调
        /// </summary>
        public void OnStateExit(GameState state, Action callback)
        {
            if (!_onStateExitCallbacks.ContainsKey(state))
            {
                _onStateExitCallbacks[state] = null;
            }
            _onStateExitCallbacks[state] += callback;
        }

        /// <summary>
        /// 执行回调列表
        /// </summary>
        private void ExecuteCallbacks(List<Action> callbacks)
        {
            foreach (var callback in callbacks)
            {
                try
                {
                    callback?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"游戏循环回调异常: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 重置游戏时间
        /// </summary>
        public void ResetGameTime()
        {
            _gameTime = 0f;
        }

        /// <summary>
        /// 获取上一个状态
        /// </summary>
        public GameState GetPreviousState()
        {
            return _previousState;
        }
    }
}
