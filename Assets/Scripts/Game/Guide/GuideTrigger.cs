using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveBeatGuide
{
    /// <summary>
    /// 引导触发器基类
    /// </summary>
    public abstract class GuideTrigger
    {
        protected string triggerId;
        protected bool isActive;
        protected GuideTriggerType triggerType;

        public string TriggerId => triggerId;
        public bool IsActive => isActive;
        public GuideTriggerType TriggerType => triggerType;

        public GuideTrigger(string triggerId, GuideTriggerType type)
        {
            this.triggerId = triggerId;
            this.triggerType = type;
            this.isActive = true;
        }

        /// <summary>
        /// 检查触发条件
        /// </summary>
        public abstract bool CheckTrigger();

        /// <summary>
        /// 激活触发器
        /// </summary>
        public virtual void Activate()
        {
            isActive = true;
        }

        /// <summary>
        /// 停用触发器
        /// </summary>
        public virtual void Deactivate()
        {
            isActive = false;
        }

        /// <summary>
        /// 重置触发器
        /// </summary>
        public virtual void Reset()
        {
            OnReset();
        }

        /// <summary>
        /// 重置内部逻辑
        /// </summary>
        protected abstract void OnReset();
    }

    /// <summary>
    /// 场景加载触发器
    /// </summary>
    public class SceneLoadTrigger : GuideTrigger
    {
        private string targetScene;
        private bool hasTriggered;

        public SceneLoadTrigger(string triggerId, string sceneName) : base(triggerId, GuideTriggerType.SceneLoad)
        {
            this.targetScene = sceneName;
            this.hasTriggered = false;
        }

        public override bool CheckTrigger()
        {
            if (!isActive || hasTriggered)
                return false;

            bool triggered = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == targetScene;
            
            if (triggered)
            {
                hasTriggered = true;
            }

            return triggered;
        }

        protected override void OnReset()
        {
            hasTriggered = false;
        }
    }

    /// <summary>
    /// 时间触发器
    /// </summary>
    public class TimeTrigger : GuideTrigger
    {
        private float triggerTime;
        private float elapsedTime;
        private bool hasTriggered;

        public TimeTrigger(string triggerId, float time) : base(triggerId, GuideTriggerType.Time)
        {
            this.triggerTime = time;
            this.elapsedTime = 0f;
            this.hasTriggered = false;
        }

        public override bool CheckTrigger()
        {
            if (!isActive || hasTriggered)
                return false;

            elapsedTime += Time.deltaTime;
            
            if (elapsedTime >= triggerTime)
            {
                hasTriggered = true;
                return true;
            }

            return false;
        }

        protected override void OnReset()
        {
            elapsedTime = 0f;
            hasTriggered = false;
        }
    }

    /// <summary>
    /// 事件触发器
    /// </summary>
    public class EventTrigger : GuideTrigger
    {
        private string targetEvent;
        private bool requireFirstTime = true;
        private HashSet<string> triggeredEvents = new HashSet<string>();

        public EventTrigger(string triggerId, string eventName, bool requireFirstTime = true) 
            : base(triggerId, GuideTriggerType.Event)
        {
            this.targetEvent = eventName;
            this.requireFirstTime = requireFirstTime;
        }

        public override bool CheckTrigger()
        {
            if (!isActive)
                return false;

            if (requireFirstTime && triggeredEvents.Contains(targetEvent))
                return false;

            return false;
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        public virtual void TriggerEvent(string eventName)
        {
            if (eventName == targetEvent && isActive)
            {
                triggeredEvents.Add(eventName);
                GuideManager.Instance?.NotifyTriggerActivated(this);
            }
        }

        protected override void OnReset()
        {
            if (requireFirstTime)
            {
                triggeredEvents.Remove(targetEvent);
            }
        }
    }

    /// <summary>
    /// 成就触发器
    /// </summary>
    public class AchievementTrigger : GuideTrigger
    {
        private string achievementId;
        private bool hasTriggered;

        public AchievementTrigger(string triggerId, string achievementId) : base(triggerId, GuideTriggerType.Achievement)
        {
            this.achievementId = achievementId;
            this.hasTriggered = false;
        }

        public override bool CheckTrigger()
        {
            if (!isActive || hasTriggered)
                return false;

            bool triggered = AchievementManager.Instance?.IsAchievementUnlocked(achievementId) ?? false;
            
            if (triggered)
            {
                hasTriggered = true;
            }

            return triggered;
        }

        protected override void OnReset()
        {
            hasTriggered = false;
        }
    }

    /// <summary>
    /// 等级触发器
    /// </summary>
    public class LevelTrigger : GuideTrigger
    {
        private int targetLevel;
        private bool hasTriggered;

        public LevelTrigger(string triggerId, int level) : base(triggerId, GuideTriggerType.Level)
        {
            this.targetLevel = level;
            this.hasTriggered = false;
        }

        public override bool CheckTrigger()
        {
            if (!isActive || hasTriggered)
                return false;

            bool triggered = GameManager.Instance?.CurrentLevel >= targetLevel ?? false;
            
            if (triggered)
            {
                hasTriggered = true;
            }

            return triggered;
        }

        protected override void OnReset()
        {
            hasTriggered = false;
        }
    }

    /// <summary>
    /// 引导触发器管理器
    /// </summary>
    public class GuideTriggerManager
    {
        private static GuideTriggerManager instance;
        public static GuideTriggerManager Instance => instance ?? (instance = new GuideTriggerManager());

        private Dictionary<string, GuideTrigger> triggers = new Dictionary<string, GuideTrigger>();
        private List<GuideTrigger> activeTriggers = new List<GuideTrigger>();

        private GuideTriggerManager() { }

        /// <summary>
        /// 注册触发器
        /// </summary>
        public void RegisterTrigger(GuideTrigger trigger)
        {
            if (string.IsNullOrEmpty(trigger.TriggerId))
                return;

            triggers[trigger.TriggerId] = trigger;
        }

        /// <summary>
        /// 移除触发器
        /// </summary>
        public void RemoveTrigger(string triggerId)
        {
            if (triggers.ContainsKey(triggerId))
            {
                triggers.Remove(triggerId);
                activeTriggers.RemoveAll(t => t.TriggerId == triggerId);
            }
        }

        /// <summary>
        /// 获取触发器
        /// </summary>
        public GuideTrigger GetTrigger(string triggerId)
        {
            return triggers.ContainsKey(triggerId) ? triggers[triggerId] : null;
        }

        /// <summary>
        /// 激活触发器
        /// </summary>
        public void ActivateTrigger(string triggerId)
        {
            GuideTrigger trigger = GetTrigger(triggerId);
            if (trigger != null)
            {
                trigger.Activate();
                if (!activeTriggers.Contains(trigger))
                {
                    activeTriggers.Add(trigger);
                }
            }
        }

        /// <summary>
        /// 停用触发器
        /// </summary>
        public void DeactivateTrigger(string triggerId)
        {
            GuideTrigger trigger = GetTrigger(triggerId);
            if (trigger != null)
            {
                trigger.Deactivate();
                activeTriggers.Remove(trigger);
            }
        }

        /// <summary>
        /// 检查所有触发器
        /// </summary>
        public List<GuideTrigger> CheckAllTriggers()
        {
            List<GuideTrigger> triggered = new List<GuideTrigger>();
            
            foreach (var trigger in activeTriggers)
            {
                if (trigger.CheckTrigger())
                {
                    triggered.Add(trigger);
                }
            }

            return triggered;
        }

        /// <summary>
        /// 重置所有触发器
        /// </summary>
        public void ResetAllTriggers()
        {
            foreach (var trigger in triggers.Values)
            {
                trigger.Reset();
            }
        }

        /// <summary>
        /// 清空触发器
        /// </summary>
        public void Clear()
        {
            triggers.Clear();
            activeTriggers.Clear();
        }
    }

    /// <summary>
    /// 占位 AchievementManager
    /// </summary>
    public class AchievementManager
    {
        private static AchievementManager instance;
        public static AchievementManager Instance => instance ?? (instance = new AchievementManager());

        public bool IsAchievementUnlocked(string achievementId)
        {
            return PlayerPrefs.HasKey($"Achievement_{achievementId}");
        }
    }

    /// <summary>
    /// 占位 GameManager
    /// </summary>
    public class GameManager
    {
        private static GameManager instance;
        public static GameManager Instance => instance ?? (instance = new GameManager());

        public int CurrentLevel { get; set; }
    }
}
