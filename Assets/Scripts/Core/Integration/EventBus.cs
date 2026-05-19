using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Integration
{
    /// <summary>
    /// 事件总线 - 实现事件驱动架构
    /// 支持全局事件发布订阅模式
    /// </summary>
    public class EventBus : MonoBehaviour
    {
        private static EventBus _instance;
        public static EventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<EventBus>();
                    if (_instance == null)
                    {
                        var go = new GameObject("EventBus");
                        _instance = go.AddComponent<EventBus>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private Dictionary<Type, List<Delegate>> _eventHandlers = new Dictionary<Type, List<Delegate>>();
        private Dictionary<string, List<Delegate>> _stringEventHandlers = new Dictionary<string, List<Delegate>>();
        private readonly object _lock = new object();

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 订阅泛型事件
        /// </summary>
        public void Subscribe<T>(Action<T> handler)
        {
            var eventType = typeof(T);
            lock (_lock)
            {
                if (!_eventHandlers.ContainsKey(eventType))
                {
                    _eventHandlers[eventType] = new List<Delegate>();
                }
                _eventHandlers[eventType].Add(handler);
            }
        }

        /// <summary>
        /// 取消订阅泛型事件
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler)
        {
            var eventType = typeof(T);
            lock (_lock)
            {
                if (_eventHandlers.ContainsKey(eventType))
                {
                    _eventHandlers[eventType].Remove(handler);
                }
            }
        }

        /// <summary>
        /// 发布泛型事件
        /// </summary>
        public void Publish<T>(T eventData)
        {
            var eventType = typeof(T);
            List<Delegate> handlers;

            lock (_lock)
            {
                if (!_eventHandlers.ContainsKey(eventType))
                {
                    return;
                }
                handlers = new List<Delegate>(_eventHandlers[eventType]);
            }

            foreach (var handler in handlers)
            {
                try
                {
                    ((Action<T>)handler)(eventData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"事件处理异常 [{eventType.Name}]: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 订阅字符串事件
        /// </summary>
        public void Subscribe(string eventName, Action<object> handler)
        {
            lock (_lock)
            {
                if (!_stringEventHandlers.ContainsKey(eventName))
                {
                    _stringEventHandlers[eventName] = new List<Delegate>();
                }
                _stringEventHandlers[eventName].Add(handler);
            }
        }

        /// <summary>
        /// 取消订阅字符串事件
        /// </summary>
        public void Unsubscribe(string eventName, Action<object> handler)
        {
            lock (_lock)
            {
                if (_stringEventHandlers.ContainsKey(eventName))
                {
                    _stringEventHandlers[eventName].Remove(handler);
                }
            }
        }

        /// <summary>
        /// 发布字符串事件
        /// </summary>
        public void Publish(string eventName, object eventData = null)
        {
            List<Delegate> handlers;

            lock (_lock)
            {
                if (!_stringEventHandlers.ContainsKey(eventName))
                {
                    return;
                }
                handlers = new List<Delegate>(_stringEventHandlers[eventName]);
            }

            foreach (var handler in handlers)
            {
                try
                {
                    ((Action<object>)handler)(eventData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"事件处理异常 [{eventName}]: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 清除所有事件处理器
        /// </summary>
        public void ClearAll()
        {
            lock (_lock)
            {
                _eventHandlers.Clear();
                _stringEventHandlers.Clear();
            }
        }

        /// <summary>
        /// 清除指定事件类型的处理器
        /// </summary>
        public void ClearEvent<T>()
        {
            var eventType = typeof(T);
            lock (_lock)
            {
                if (_eventHandlers.ContainsKey(eventType))
                {
                    _eventHandlers[eventType].Clear();
                }
            }
        }
    }

    /// <summary>
    /// 游戏事件类型定义
    /// </summary>
    public static class GameEvents
    {
        public const string OnGameStart = "OnGameStart";
        public const string OnGameEnd = "OnGameEnd";
        public const string OnModeSwitch = "OnModeSwitch";
        public const string OnDialogueStart = "OnDialogueStart";
        public const string OnDialogueEnd = "OnDialogueEnd";
        public const string OnEndingReached = "OnEndingReached";
        public const string OnAchievementUnlocked = "OnAchievementUnlocked";
        public const string OnAPIResponse = "OnAPIResponse";
        public const string OnSaveGame = "OnSaveGame";
        public const string OnLoadGame = "OnLoadGame";
        public const string OnSceneLoad = "OnSceneLoad";
        public const string OnCharacterAppear = "OnCharacterAppear";
        public const string OnCharacterDisappear = "OnCharacterDisappear";
        public const string OnDayChange = "OnDayChange";
        public const string OnTimeAdvance = "OnTimeAdvance";
    }
}
