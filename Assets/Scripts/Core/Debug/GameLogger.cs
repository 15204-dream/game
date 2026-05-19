using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core.Debug
{
    /// <summary>
    /// 游戏日志系统 - 提供分级日志、日志保存、过滤等功能
    /// </summary>
    public class GameLogger : MonoBehaviour
    {
        private static GameLogger _instance;
        public static GameLogger Instance
        {
            get { return _instance; }
        }

        [Header("日志配置")]
        [SerializeField] private bool _enableLogFile = true;
        [SerializeField] private bool _enableConsoleLog = true;
        [SerializeField] private int _maxLogCount = 1000;
        [SerializeField] private string _logFileName = "game_log.txt";

        private List<LogEntry> _logs = new List<LogEntry>();
        private Queue<LogEntry> _logQueue = new Queue<LogEntry>();
        private LogFilter _currentFilter = new LogFilter();

        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Fatal
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializeLogger();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// 初始化日志系统
        /// </summary>
        private void InitializeLogger()
        {
            Application.logMessageReceived += HandleLogMessage;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= HandleLogMessage;
            FlushLogs();
        }

        /// <summary>
        /// 处理Unity日志消息
        /// </summary>
        private void HandleLogMessage(string condition, string stackTrace, LogType type)
        {
            var level = ConvertLogType(type);
            AddLog(condition, level, stackTrace);
        }

        /// <summary>
        /// 转换日志类型
        /// </summary>
        private LogLevel ConvertLogType(LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                    return LogLevel.Info;
                case LogType.Warning:
                    return LogLevel.Warning;
                case LogType.Error:
                    return LogLevel.Error;
                case LogType.Exception:
                    return LogLevel.Fatal;
                default:
                    return LogLevel.Debug;
            }
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        public void AddLog(string message, LogLevel level = LogLevel.Info, string stackTrace = "")
        {
            if (!_currentFilter.ShouldLog(level))
            {
                return;
            }

            var entry = new LogEntry
            {
                Message = message,
                Level = level,
                StackTrace = stackTrace,
                Timestamp = DateTime.Now
            };

            _logQueue.Enqueue(entry);

            if (_logs.Count >= _maxLogCount)
            {
                _logs.RemoveAt(0);
            }
            _logs.Add(entry);

            if (_enableConsoleLog)
            {
                OutputToConsole(entry);
            }

            if (_enableLogFile)
            {
                WriteToFile(entry);
            }
        }

        /// <summary>
        /// 输出到控制台
        /// </summary>
        private void OutputToConsole(LogEntry entry)
        {
            string formattedMessage = $"[{entry.Timestamp:HH:mm:ss}] [{entry.Level}] {entry.Message}";
            switch (entry.Level)
            {
                case LogLevel.Debug:
                    Debug.Log(formattedMessage);
                    break;
                case LogLevel.Info:
                    Debug.Log(formattedMessage);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(formattedMessage);
                    break;
                case LogLevel.Error:
                case LogLevel.Fatal:
                    Debug.LogError(formattedMessage);
                    break;
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        private void WriteToFile(LogEntry entry)
        {
            string logLine = $"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{entry.Level}] {entry.Message}\n";
            System.IO.File.AppendAllText(GetLogFilePath(), logLine);
        }

        /// <summary>
        /// 获取日志文件路径
        /// </summary>
        private string GetLogFilePath()
        {
            return Application.persistentDataPath + "/" + _logFileName;
        }

        /// <summary>
        /// 刷新日志
        /// </summary>
        public void FlushLogs()
        {
            _logQueue.Clear();
        }

        /// <summary>
        /// 获取所有日志
        /// </summary>
        public List<LogEntry> GetAllLogs()
        {
            return new List<LogEntry>(_logs);
        }

        /// <summary>
        /// 获取过滤后的日志
        /// </summary>
        public List<LogEntry> GetFilteredLogs(LogLevel minLevel)
        {
            var filtered = new List<LogEntry>();
            foreach (var log in _logs)
            {
                if (log.Level >= minLevel)
                {
                    filtered.Add(log);
                }
            }
            return filtered;
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        public void ClearLogs()
        {
            _logs.Clear();
        }

        /// <summary>
        /// 设置日志过滤器
        /// </summary>
        public void SetFilter(LogFilter filter)
        {
            _currentFilter = filter;
        }

        /// <summary>
        /// 导出日志
        /// </summary>
        public void ExportLogs(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                foreach (var log in _logs)
                {
                    writer.WriteLine($"[{log.Timestamp:yyyy-MM-dd HH:mm:ss}] [{log.Level}] {log.Message}");
                    if (!string.IsNullOrEmpty(log.StackTrace))
                    {
                        writer.WriteLine(log.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Debug日志
        /// </summary>
        public void Debug(string message)
        {
            AddLog(message, LogLevel.Debug);
        }

        /// <summary>
        /// Info日志
        /// </summary>
        public void Info(string message)
        {
            AddLog(message, LogLevel.Info);
        }

        /// <summary>
        /// Warning日志
        /// </summary>
        public void Warning(string message)
        {
            AddLog(message, LogLevel.Warning);
        }

        /// <summary>
        /// Error日志
        /// </summary>
        public void Error(string message)
        {
            AddLog(message, LogLevel.Error);
        }

        /// <summary>
        /// Fatal日志
        /// </summary>
        public void Fatal(string message)
        {
            AddLog(message, LogLevel.Fatal);
        }
    }

    /// <summary>
    /// 日志条目
    /// </summary>
    [Serializable]
    public class LogEntry
    {
        public string Message;
        public GameLogger.LogLevel Level;
        public string StackTrace;
        public DateTime Timestamp;
    }

    /// <summary>
    /// 日志过滤器
    /// </summary>
    [Serializable]
    public class LogFilter
    {
        public bool EnableDebug = true;
        public bool EnableInfo = true;
        public bool EnableWarning = true;
        public bool EnableError = true;
        public bool EnableFatal = true;

        public bool ShouldLog(GameLogger.LogLevel level)
        {
            switch (level)
            {
                case GameLogger.LogLevel.Debug:
                    return EnableDebug;
                case GameLogger.LogLevel.Info:
                    return EnableInfo;
                case GameLogger.LogLevel.Warning:
                    return EnableWarning;
                case GameLogger.LogLevel.Error:
                    return EnableError;
                case GameLogger.LogLevel.Fatal:
                    return EnableFatal;
                default:
                    return true;
            }
        }
    }
}
