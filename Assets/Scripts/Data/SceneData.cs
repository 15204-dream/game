using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 场景类型
    /// </summary>
    public enum SceneType
    {
        LivingRoom,
        Kitchen,
        Garden,
        Beach,
        Pool,
        Bedroom,
        Restaurant,
        Cafe,
        Cinema,
        BeachBar,
        Rooftop,
        Gym,
        Library,
        MusicRoom,
        ArtStudio
    }

    /// <summary>
    /// 场景时间类型
    /// </summary>
    public enum SceneTimeType
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }

    /// <summary>
    /// 场景配置数据
    /// </summary>
    [System.Serializable]
    public class SceneConfig
    {
        public string sceneId;
        public string sceneName;
        public SceneType sceneType;
        public string description;
        public string prefabPath;
        public Sprite backgroundImage;
        public SceneTimeType availableTime;
        public List<string> availableSeasons = new List<string>();

        [Header("场景容量")]
        public int minCapacity;
        public int maxCapacity;

        [Header("场景功能")]
        public bool canCook;
        public bool canDate;
        public bool canDance;
        public bool canSwim;
        public bool canExercise;
        public bool canSing;
        public bool canPaint;

        [Header("氛围加成")]
        public Dictionary<string, float> atmosphereBonuses = new Dictionary<string, float>();
        public List<string> moodTypes = new List<string>();
    }

    /// <summary>
    /// 场景事件配置
    /// </summary>
    [System.Serializable]
    public class SceneEventConfig
    {
        public string eventId;
        public string eventName;
        public string sceneId;
        public List<string> requiredTags = new List<string>();
        public List<string> excludedTags = new List<string>();
        public int minParticipants;
        public int maxParticipants;
        public float triggerChance;
        public int duration;
        public List<SceneEventEffect> effects = new List<SceneEventEffect>();
        public string eventDescription;
    }

    /// <summary>
    /// 场景事件效果
    /// </summary>
    [System.Serializable]
    public class SceneEventEffect
    {
        public EffectType effectType;
        public string targetId;
        public float value;
        public string description;
    }

    /// <summary>
    /// 效果类型
    /// </summary>
    public enum EffectType
    {
        MoodChange,
        EnergyChange,
        StressChange,
        RelationshipChange,
        UnlockStory,
        TriggerSpecialEvent,
        AudienceHeatChange,
        ConfessionPoint
    }

    /// <summary>
    /// 场景数据管理器 - 管理所有场景状态
    /// </summary>
    [System.Serializable]
    public class SceneData
    {
        [Header("场景配置列表")]
        public List<SceneConfig> availableScenes = new List<SceneConfig>();

        [Header("当前场景状态")]
        public string currentSceneId;
        public SceneStatus currentSceneStatus;

        [Header("场景使用统计")]
        public Dictionary<string, int> sceneVisitCount = new Dictionary<string, int>();
        public Dictionary<string, DateTime> lastVisitTime = new Dictionary<string, DateTime>();

        [Header("特殊场景解锁")]
        public List<string> unlockedSpecialScenes = new List<string>();

        [Header("场景事件记录")]
        public List<SceneEventRecord> eventHistory = new List<SceneEventRecord>();

        [Header("每日场景分配")]
        public DailySceneSchedule dailySchedule = new DailySceneSchedule();
    }

    /// <summary>
    /// 场景状态
    /// </summary>
    [System.Serializable]
    public class SceneStatus
    {
        public string sceneId;
        public List<string> presentCharacterIds = new List<string>();
        public SceneTimeType currentTimeType;
        public float ambientTemperature;
        public float lightingIntensity;
        public bool isDecorated;
        public string decorationTheme;
        public Dictionary<string, int> objectStates = new Dictionary<string, int>();
        public List<string> activeEffects = new List<string>();
    }

    /// <summary>
    /// 场景事件记录
    /// </summary>
    [System.Serializable]
    public class SceneEventRecord
    {
        public string recordId;
        public string sceneId;
        public string eventId;
        public DateTime eventTime;
        public List<string> participantIds = new List<string>();
        public List<string> effects = new List<string>();
        public int audienceReaction;
    }

    /// <summary>
    /// 每日场景安排
    /// </summary>
    [System.Serializable]
    public class DailySceneSchedule
    {
        public int dayNumber;
        public Dictionary<SceneTimeType, string> timeSlotScenes = new Dictionary<SceneTimeType, string>();
        public List<ScheduledActivity> specialActivities = new List<ScheduledActivity>();
    }

    /// <summary>
    /// 定时活动
    /// </summary>
    [System.Serializable]
    public class ScheduledActivity
    {
        public string activityId;
        public string activityName;
        public SceneTimeType scheduledTime;
        public string targetSceneId;
        public List<string> requiredCharacterIds = new List<string>();
        public bool isMandatory;
        public string description;
    }

    /// <summary>
    /// 场景数据验证器
    /// </summary>
    public static class SceneDataValidator
    {
        public static bool ValidateSceneConfig(SceneConfig config, out string errorMessage)
        {
            if (string.IsNullOrEmpty(config.sceneId))
            {
                errorMessage = "场景ID不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(config.sceneName))
            {
                errorMessage = "场景名称不能为空";
                return false;
            }

            if (config.minCapacity > config.maxCapacity)
            {
                errorMessage = "最小容量不能大于最大容量";
                return false;
            }

            if (config.minCapacity < 1)
            {
                errorMessage = "最小容量至少为1";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool CanCharacterUseScene(string characterId, SceneConfig scene, out string reason)
        {
            if (!scene.availableSeasons.Contains("All"))
            {
                reason = "当前季节该场景不可用";
                return false;
            }

            reason = null;
            return true;
        }
    }
}
