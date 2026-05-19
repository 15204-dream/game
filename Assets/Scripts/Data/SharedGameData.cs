using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 角色基础信息数据
    /// </summary>
    [System.Serializable]
    public class CharacterBasicInfo
    {
        [Header("基本信息")]
        public string characterId;
        public string characterName;
        public CharacterGender gender;
        public int age;
        public string occupation;
        public string personality;
        public string hobby;
        public string birthday;
        public string zodiac;
        public string mbti;

        [Header("外观描述")]
        public string avatar;
        public string appearanceDescription;
        public string voiceDescription;

        [Header("人设标签")]
        public List<string> tags = new List<string>();

        [Header("恋爱偏好")]
        public List<CharacterTrait> preferredTraits = new List<CharacterTrait>();
        public List<string> turnOffBehaviors = new List<string>();
    }

    /// <summary>
    /// 角色性别枚举
    /// </summary>
    public enum CharacterGender
    {
        Male,
        Female,
        Other
    }

    /// <summary>
    /// 角色特质
    /// </summary>
    [System.Serializable]
    public class CharacterTrait
    {
        public string traitName;
        public int intensity;
        public string description;
    }

    /// <summary>
    /// 角色状态数据
    /// </summary>
    [System.Serializable]
    public class CharacterStatusData
    {
        public string characterId;
        public int currentMood;
        public int maxMood;
        public int energy;
        public int maxEnergy;
        public int stress;
        public int confidence;
        public int socialExposure;
        public bool isEliminated;
        public DateTime eliminationTime;
        public List<string> activeEffects = new List<string>();
        public Dictionary<string, int> currentRelationships = new Dictionary<string, int>();
    }

    /// <summary>
    /// 共享游戏数据 - 包含所有嘉宾和全局设置
    /// </summary>
    [System.Serializable]
    public class SharedGameData
    {
        [Header("游戏基础信息")]
        public string gameVersion = "1.0";
        public string seasonName;
        public int totalEpisodes;
        public int currentEpisode;
        public GameMode currentMode;
        public DateTime gameStartTime;
        public DateTime lastSaveTime;

        [Header("嘉宾数据")]
        public List<CharacterBasicInfo> allCharacters = new List<CharacterBasicInfo>();
        public Dictionary<string, CharacterStatusData> characterStatus = new Dictionary<string, CharacterStatusData>();

        [Header("全局设置")]
        public GameSettings settings = new GameSettings();
        public GlobalStatistics statistics = new GlobalStatistics();

        [Header("剧情进度")]
        public List<string> unlockedStorylines = new List<string>();
        public List<string> completedEvents = new List<string>();
        public Dictionary<string, bool> flags = new Dictionary<string, bool>();

        [Header("约会记录")]
        public List<DateRecord> dateRecords = new List<DateRecord>();

        [Header("CP配对记录")]
        public List<CPRecord> cpRecords = new List<CPRecord>();
    }

    /// <summary>
    /// 游戏模式
    /// </summary>
    public enum GameMode
    {
        GuestMode,
        DirectorMode
    }

    /// <summary>
    /// 游戏设置
    /// </summary>
    [System.Serializable]
    public class GameSettings
    {
        public bool autoSave = true;
        public int autoSaveInterval = 300;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1.0f;
        public bool showSubtitles = true;
        public string language = "zh-CN";
        public bool enableVibration = true;
        public int textSpeed = 2;
    }

    /// <summary>
    /// 全局统计数据
    /// </summary>
    [System.Serializable]
    public class GlobalStatistics
    {
        public int totalPlayTime;
        public int totalInteractions;
        public int successfulDates;
        public int failedDates;
        public int confessionsAttempted;
        public int confessionsSuccessful;
        public int timesEliminated;
        public int timesWon;
    }

    /// <summary>
    /// 约会记录
    /// </summary>
    [System.Serializable]
    public class DateRecord
    {
        public string recordId;
        public DateTime dateTime;
        public string guest1Id;
        public string guest2Id;
        public string sceneId;
        public DateQuality quality;
        public List<string> events = new List<string>();
        public string notes;
    }

    /// <summary>
    /// 约会质量评价
    /// </summary>
    public enum DateQuality
    {
        Terrible,
        Bad,
        Normal,
        Good,
        Excellent,
        Perfect
    }

    /// <summary>
    /// CP配对记录
    /// </summary>
    [System.Serializable]
    public class CPRecord
    {
        public string recordId;
        public string guest1Id;
        public string guest2Id;
        public DateTime formedTime;
        public DateTime? brokenTime;
        public bool isActive;
        public int lovePoints;
        public List<string> memorableMoments = new List<string>();
    }
}
