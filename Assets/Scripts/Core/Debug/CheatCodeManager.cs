using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Debug
{
    /// <summary>
    /// 作弊码管理器 - 管理游戏作弊功能
    /// 支持自定义作弊码序列和对应的作弊效果
    /// </summary>
    public class CheatCodeManager : MonoBehaviour
    {
        private static CheatCodeManager _instance;
        public static CheatCodeManager Instance
        {
            get { return _instance; }
        }

        [Header("作弊码配置")]
        [SerializeField] private bool _enableCheatCodes = true;
        [SerializeField] private bool _showCheatNotifications = true;

        private Dictionary<string, CheatCode> _cheatCodes = new Dictionary<string, CheatCode>();
        private string _currentInput = "";
        private float _lastInputTime = 0f;
        private float _inputTimeout = 1f;
        private int _maxInputLength = 20;

        public event Action<string> OnCheatActivated;
        public event Action<string> OnCheatFailed;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializeCheatCodes();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        void Update()
        {
            if (!_enableCheatCodes) return;

            ProcessInput();
            CheckInputTimeout();
        }

        /// <summary>
        /// 初始化作弊码
        /// </summary>
        private void InitializeCheatCodes()
        {
            RegisterCheatCode(new CheatCode("GODMODE", "无敌模式", ActivateGodMode));
            RegisterCheatCode(new CheatCode("FREEMONEY", "无限金币", ActivateFreeMoney));
            RegisterCheatCode(new CheatCode("MAXHEART", "满心好感", ActivateMaxHearts));
            RegisterCheatCode(new CheatCode("SKIPDAY", "跳过一天", ActivateSkipDay));
            RegisterCheatCode(new CheatCode("ALLENDINGS", "解锁所有结局", UnlockAllEndings));
            RegisterCheatCode(new CheatCode("KILLALL", "强制结束", ActivateKillAll));
            RegisterCheatCode(new CheatCode("FPS", "显示帧率", ToggleFPSCounter));
            RegisterCheatCode(new CheatCode("RELOAD", "重新加载", ReloadScene));
            RegisterCheatCode(new CheatCode("GIVETIME", "增加行动点", GiveTimePoints));
            RegisterCheatCode(new CheatCode("TRUEOUT", "触发真结局", TriggerTrueEnding));
        }

        /// <summary>
        /// 注册作弊码
        /// </summary>
        public void RegisterCheatCode(CheatCode cheatCode)
        {
            if (!_cheatCodes.ContainsKey(cheatCode.Code))
            {
                _cheatCodes.Add(cheatCode.Code.ToUpper(), cheatCode);
            }
        }

        /// <summary>
        /// 处理输入
        /// </summary>
        private void ProcessInput()
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b')
                {
                    if (_currentInput.Length > 0)
                    {
                        _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
                    }
                }
                else if (c == '\n' || c == '\r')
                {
                    TryActivateCheat();
                }
                else if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
                {
                    if (_currentInput.Length < _maxInputLength)
                    {
                        _currentInput += c;
                        _lastInputTime = Time.time;
                    }
                }
            }
        }

        /// <summary>
        /// 检查输入超时
        /// </summary>
        private void CheckInputTimeout()
        {
            if (!string.IsNullOrEmpty(_currentInput) && Time.time - _lastInputTime > _inputTimeout)
            {
                _currentInput = "";
            }
        }

        /// <summary>
        /// 尝试激活作弊码
        /// </summary>
        private void TryActivateCheat()
        {
            string inputUpper = _currentInput.ToUpper().Trim();
            _currentInput = "";

            if (_cheatCodes.ContainsKey(inputUpper))
            {
                var cheatCode = _cheatCodes[inputUpper];
                cheatCode.Activate();

                if (_showCheatNotifications)
                {
                    ShowCheatNotification(cheatCode.Name);
                }

                OnCheatActivated?.Invoke(inputUpper);
                Debug.Log($"作弊码已激活: {cheatCode.Name}");
            }
            else if (inputUpper.Length >= 3)
            {
                OnCheatFailed?.Invoke(inputUpper);
            }
        }

        /// <summary>
        /// 显示作弊通知
        /// </summary>
        private void ShowCheatNotification(string cheatName)
        {
            EventBus.Instance.Publish("OnCheatActivated", cheatName);
        }

        /// <summary>
        /// 手动触发作弊码
        /// </summary>
        public void TriggerCheat(string code)
        {
            if (!_enableCheatCodes)
            {
                Debug.LogWarning("作弊码未启用");
                return;
            }

            string codeUpper = code.ToUpper();
            if (_cheatCodes.ContainsKey(codeUpper))
            {
                var cheatCode = _cheatCodes[codeUpper];
                cheatCode.Activate();
                OnCheatActivated?.Invoke(codeUpper);
            }
        }

        /// <summary>
        /// 启用/禁用作弊码
        /// </summary>
        public void SetCheatCodesEnabled(bool enabled)
        {
            _enableCheatCodes = enabled;
        }

        /// <summary>
        /// 获取所有作弊码
        /// </summary>
        public List<CheatCode> GetAllCheatCodes()
        {
            return new List<CheatCode>(_cheatCodes.Values);
        }

        /// <summary>
        /// 获取当前输入
        /// </summary>
        public string GetCurrentInput()
        {
            return _currentInput;
        }

        // 作弊码实现

        private void ActivateGodMode()
        {
            Debug.Log("无敌模式已激活");
        }

        private void ActivateFreeMoney()
        {
            Debug.Log("无限金币已激活");
        }

        private void ActivateMaxHearts()
        {
            Debug.Log("满心好感已激活");
        }

        private void ActivateSkipDay()
        {
            Debug.Log("跳过一天已激活");
        }

        private void UnlockAllEndings()
        {
            Debug.Log("解锁所有结局已激活");
        }

        private void ActivateKillAll()
        {
            Debug.Log("强制结束已激活");
            Application.Quit();
        }

        private void ToggleFPSCounter()
        {
            EventBus.Instance.Publish("ToggleFPSCounter", null);
        }

        private void ReloadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void GiveTimePoints()
        {
            Debug.Log("增加行动点已激活");
        }

        private void TriggerTrueEnding()
        {
            Debug.Log("触发真结局已激活");
        }
    }

    /// <summary>
    /// 作弊码
    /// </summary>
    [Serializable]
    public class CheatCode
    {
        public string Code;
        public string Name;
        public string Description;
        public bool IsEnabled;
        private Action _action;

        public CheatCode(string code, string name, Action action)
        {
            Code = code.ToUpper();
            Name = name;
            Description = "";
            IsEnabled = true;
            _action = action;
        }

        public void Activate()
        {
            if (IsEnabled && _action != null)
            {
                _action.Invoke();
            }
        }
    }
}
