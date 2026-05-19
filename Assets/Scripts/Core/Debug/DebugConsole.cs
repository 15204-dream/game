using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core.Debug
{
    /// <summary>
    /// 调试控制台 - 提供运行时调试界面
    /// 支持命令输入、日志查看、性能监控等功能
    /// </summary>
    public class DebugConsole : MonoBehaviour
    {
        private static DebugConsole _instance;
        public static DebugConsole Instance
        {
            get { return _instance; }
        }

        [Header("控制台配置")]
        [SerializeField] private KeyCode _toggleKey = KeyCode.F1;
        [SerializeField] private int _maxVisibleLogs = 50;
        [SerializeField] private bool _startMinimized = true;

        private bool _isOpen = false;
        private bool _isMinimized = true;
        private Vector2 _scrollPosition;
        private string _commandInput = "";
        private List<ConsoleLogEntry> _consoleLogs = new List<ConsoleLogEntry>();
        private List<ConsoleCommand> _commands = new List<ConsoleCommand>();

        private Rect _windowRect = new Rect(20, 20, 600, 400);
        private Rect _minimizedRect = new Rect(20, Screen.height - 40, 200, 30);

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializeConsole();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _isOpen = !_startMinimized;
            _isMinimized = _startMinimized;
        }

        void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                ToggleConsole();
            }
        }

        /// <summary>
        /// 初始化控制台
        /// </summary>
        private void InitializeConsole()
        {
            RegisterDefaultCommands();
            Application.logMessageReceived += HandleLogMessage;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= HandleLogMessage;
        }

        /// <summary>
        /// 处理日志消息
        /// </summary>
        private void HandleLogMessage(string condition, string stackTrace, LogType type)
        {
            var entry = new ConsoleLogEntry
            {
                Message = condition,
                Type = ConvertLogType(type),
                Timestamp = DateTime.Now
            };

            _consoleLogs.Add(entry);

            if (_consoleLogs.Count > _maxVisibleLogs)
            {
                _consoleLogs.RemoveAt(0);
            }
        }

        /// <summary>
        /// 转换日志类型
        /// </summary>
        private ConsoleLogType ConvertLogType(LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                    return ConsoleLogType.Info;
                case LogType.Warning:
                    return ConsoleLogType.Warning;
                case LogType.Error:
                case LogType.Exception:
                    return ConsoleLogType.Error;
                default:
                    return ConsoleLogType.Info;
            }
        }

        /// <summary>
        /// 注册默认命令
        /// </summary>
        private void RegisterDefaultCommands()
        {
            RegisterCommand(new ConsoleCommand("help", "显示帮助", ShowHelp));
            RegisterCommand(new ConsoleCommand("clear", "清空日志", ClearLogs));
            RegisterCommand(new ConsoleCommand("fps", "显示帧率", ShowFPS));
            RegisterCommand(new ConsoleCommand("mem", "显示内存", ShowMemory));
            RegisterCommand(new ConsoleCommand("scene", "切换场景 [name]", ChangeScene));
            RegisterCommand(new ConsoleCommand("quit", "退出游戏", QuitGame));
            RegisterCommand(new ConsoleCommand("gc", "触发垃圾回收", TriggerGC));
        }

        /// <summary>
        /// 注册命令
        /// </summary>
        public void RegisterCommand(ConsoleCommand command)
        {
            _commands.Add(command);
        }

        /// <summary>
        /// 切换控制台
        /// </summary>
        public void ToggleConsole()
        {
            _isOpen = !_isOpen;
            if (_isOpen)
            {
                _isMinimized = false;
            }
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        private void ClearLogs()
        {
            _consoleLogs.Clear();
            AddLog("日志已清空", ConsoleLogType.Info);
        }

        /// <summary>
        /// 显示帮助
        /// </summary>
        private void ShowHelp()
        {
            AddLog("可用命令:", ConsoleLogType.Info);
            foreach (var cmd in _commands)
            {
                AddLog($"  {cmd.Name} - {cmd.Description}", ConsoleLogType.Info);
            }
        }

        /// <summary>
        /// 显示帧率
        /// </summary>
        private void ShowFPS()
        {
            float fps = 1f / Time.deltaTime;
            AddLog($"当前帧率: {fps:F2}", ConsoleLogType.Info);
        }

        /// <summary>
        /// 显示内存
        /// </summary>
        private void ShowMemory()
        {
            float memoryMB = (float)System.GC.GetTotalMemory(false) / (1024f * 1024f);
            AddLog($"当前内存: {memoryMB:F2} MB", ConsoleLogType.Info);
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        private void ChangeScene(string[] args)
        {
            if (args.Length > 0)
            {
                string sceneName = args[0];
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                AddLog($"切换到场景: {sceneName}", ConsoleLogType.Info);
            }
            else
            {
                AddLog("请指定场景名称", ConsoleLogType.Warning);
            }
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        private void QuitGame()
        {
            AddLog("退出游戏...", ConsoleLogType.Info);
            Application.Quit();
        }

        /// <summary>
        /// 触发垃圾回收
        /// </summary>
        private void TriggerGC()
        {
            long before = System.GC.GetTotalMemory(false);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            long after = System.GC.GetTotalMemory(false);
            float freed = (before - after) / (1024f * 1024f);
            AddLog($"释放内存: {freed:F2} MB", ConsoleLogType.Info);
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        public void AddLog(string message, ConsoleLogType type)
        {
            var entry = new ConsoleLogEntry
            {
                Message = message,
                Type = type,
                Timestamp = DateTime.Now
            };

            _consoleLogs.Add(entry);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        private void ExecuteCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            string[] parts = input.Split(' ');
            string commandName = parts[0].ToLower();

            var command = _commands.Find(c => c.Name.ToLower() == commandName);
            if (command != null)
            {
                string[] args = new string[parts.Length - 1];
                Array.Copy(parts, 1, args, 0, args.Length);
                command.Execute(args);
            }
            else
            {
                AddLog($"未知命令: {commandName}", ConsoleLogType.Error);
            }

            _commandInput = "";
        }

        void OnGUI()
        {
            if (!_isOpen) return;

            if (_isMinimized)
            {
                GUI.Box(_minimizedRect, "Debug Console (Click to open)");
                if (GUI.Button(_minimizedRect, "", GUIStyle.none))
                {
                    _isMinimized = false;
                }
            }
            else
            {
                _windowRect = GUI.Window(0, _windowRect, DrawConsoleWindow, "Debug Console");
            }
        }

        /// <summary>
        /// 绘制控制台窗口
        /// </summary>
        private void DrawConsoleWindow(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginArea(new Rect(5, 25, _windowRect.width - 10, _windowRect.height - 80));
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            foreach (var log in _consoleLogs)
            {
                string prefix = $"[{log.Timestamp:HH:mm:ss}]";
                Color originalColor = GUI.contentColor;

                switch (log.Type)
                {
                    case ConsoleLogType.Warning:
                        GUI.contentColor = Color.yellow;
                        break;
                    case ConsoleLogType.Error:
                        GUI.contentColor = Color.red;
                        break;
                    default:
                        GUI.contentColor = Color.white;
                        break;
                }

                GUILayout.Label($"{prefix} {log.Message}");
                GUI.contentColor = originalColor;
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(5, _windowRect.height - 50, _windowRect.width - 10, 45));
            GUILayout.BeginHorizontal();

            GUI.SetNextControlName("CommandInput");
            _commandInput = GUILayout.TextField(_commandInput, GUILayout.Width(_windowRect.width - 80));

            if (GUILayout.Button("Execute", GUILayout.Width(70)))
            {
                ExecuteCommand(_commandInput);
                GUI.FocusControl("CommandInput");
            }

            GUILayout.EndHorizontal();

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                ExecuteCommand(_commandInput);
                GUI.FocusControl("CommandInput");
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(_windowRect.width - 35, 25, 30, 20));
            if (GUILayout.Button("-"))
            {
                _isMinimized = true;
            }
            GUILayout.EndArea();

            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }

    /// <summary>
    /// 控制台日志条目
    /// </summary>
    public class ConsoleLogEntry
    {
        public string Message;
        public ConsoleLogType Type;
        public DateTime Timestamp;
    }

    /// <summary>
    /// 控制台日志类型
    /// </summary>
    public enum ConsoleLogType
    {
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// 控制台命令
    /// </summary>
    public class ConsoleCommand
    {
        public string Name;
        public string Description;
        private Action<string[]> _executeAction;

        public ConsoleCommand(string name, string description, Action executeAction)
        {
            Name = name;
            Description = description;
            _executeAction = (args) => executeAction();
        }

        public ConsoleCommand(string name, string description, Action<string[]> executeAction)
        {
            Name = name;
            Description = description;
            _executeAction = executeAction;
        }

        public void Execute(string[] args)
        {
            _executeAction?.Invoke(args);
        }
    }
}
