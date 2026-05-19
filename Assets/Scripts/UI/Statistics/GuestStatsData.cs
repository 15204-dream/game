using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 嘉宾统计数据配置 - 用于配置嘉宾的初始统计参数
    /// </summary>
    [CreateAssetMenu(fileName = "GuestStatsConfig", menuName = "DatingShow/GuestStatsConfig")]
    public class GuestStatsDataConfig : ScriptableObject
    {
        [Header("基础统计配置")]
        [SerializeField] private float initialHeartIndex = 30f;
        [SerializeField] private float maxHeartIndex = 100f;
        [SerializeField] private float initialAffection = 20f;
        
        [Header("衰减配置")]
        [SerializeField] private float focusDecayRate = 0.1f;
        [SerializeField] private float affectionDecayRate = 0.05f;
        [SerializeField] private float socialDecayRate = 0.02f;
        
        [Header("增长配置")]
        [SerializeField] private float conversationAffectionGain = 2f;
        [SerializeField] private float dateAffectionGain = 10f;
        [SerializeField] private float heartAffectionGain = 5f;
        
        [Header("社交配置")]
        [SerializeField] private float socialMultiplier = 1f;
        [SerializeField] private float interactionRange = 10f;
        
        [Header("特殊配置")]
        [SerializeField] private bool enableHeartBonus = true;
        [SerializeField] private float heartBonusMultiplier = 1.5f;
        [SerializeField] private int criticalHeartThreshold = 5;
        
        public float InitialHeartIndex => initialHeartIndex;
        public float MaxHeartIndex => maxHeartIndex;
        public float InitialAffection => initialAffection;
        public float FocusDecayRate => focusDecayRate;
        public float AffectionDecayRate => affectionDecayRate;
        public float SocialDecayRate => socialDecayRate;
        public float ConversationAffectionGain => conversationAffectionGain;
        public float DateAffectionGain => dateAffectionGain;
        public float HeartAffectionGain => heartAffectionGain;
        public float SocialMultiplier => socialMultiplier;
        public float InteractionRange => interactionRange;
        public bool EnableHeartBonus => enableHeartBonus;
        public float HeartBonusMultiplier => heartBonusMultiplier;
        public int CriticalHeartThreshold => criticalHeartThreshold;
        
        /// <summary>
        /// 根据配置创建嘉宾统计数据
        /// </summary>
        public GuestStatsData CreateStatsData(string guestId, string guestName)
        {
            GuestStatsData data = new GuestStatsData
            {
                guestId = guestId,
                guestName = guestName,
                heartIndex = initialHeartIndex,
                maxHeartIndex = maxHeartIndex,
                focusRate = 50f,
                focusDecayRate = focusDecayRate,
                socialActivity = 30f,
                socialMultiplier = socialMultiplier,
                affectionLevel = initialAffection,
                conversationCount = 0,
                dateCount = 0,
                heartCount = 0,
                lastInteractionTime = DateTime.Now
            };
            
            return data;
        }
        
        /// <summary>
        /// 计算互动后的好感度变化
        /// </summary>
        public float CalculateAffectionGain(InteractionType type, int comboCount = 0)
        {
            float baseGain = 0f;
            
            switch (type)
            {
                case InteractionType.Conversation:
                    baseGain = conversationAffectionGain;
                    break;
                case InteractionType.Date:
                    baseGain = dateAffectionGain;
                    break;
                case InteractionType.Heart:
                    baseGain = heartAffectionGain;
                    if (enableHeartBonus && comboCount >= criticalHeartThreshold)
                    {
                        baseGain *= heartBonusMultiplier;
                    }
                    break;
            }
            
            return baseGain * socialMultiplier;
        }
        
        /// <summary>
        /// 计算衰减后的值
        /// </summary>
        public float CalculateDecayedValue(float currentValue, DecayType type, float timeSinceLastInteraction)
        {
            float decayRate = 0f;
            
            switch (type)
            {
                case DecayType.Focus:
                    decayRate = focusDecayRate;
                    break;
                case DecayType.Affection:
                    decayRate = affectionDecayRate;
                    break;
                case DecayType.Social:
                    decayRate = socialDecayRate;
                    break;
            }
            
            float decayAmount = decayRate * timeSinceLastInteraction;
            return Mathf.Max(0f, currentValue - decayAmount);
        }
    }
    
    /// <summary>
    /// 互动类型
    /// </summary>
    public enum InteractionType
    {
        Conversation,
        Date,
        Heart
    }
    
    /// <summary>
    /// 衰减类型
    /// </summary>
    public enum DecayType
    {
        Focus,
        Affection,
        Social
    }
    
    /// <summary>
    /// 嘉宾统计数据配置数据类
    /// </summary>
    [Serializable]
    public class GuestStatsConfigData
    {
        public string guestId;
        public string guestName;
        public float initialHeartIndex;
        public float maxHeartIndex;
        public float focusDecayRate;
        public float affectionDecayRate;
        public float socialMultiplier;
        public float conversationAffectionGain;
        public float dateAffectionGain;
        public float heartAffectionGain;
        
        public static GuestStatsConfigData CreateDefault()
        {
            return new GuestStatsConfigData
            {
                guestId = "",
                guestName = "",
                initialHeartIndex = 30f,
                maxHeartIndex = 100f,
                focusDecayRate = 0.1f,
                affectionDecayRate = 0.05f,
                socialMultiplier = 1f,
                conversationAffectionGain = 2f,
                dateAffectionGain = 10f,
                heartAffectionGain = 5f
            };
        }
    }
    
    /// <summary>
    /// 嘉宾统计记录器配置
    /// </summary>
    [Serializable]
    public class GuestStatsRecorderConfig
    {
        public bool recordConversations = true;
        public bool recordDates = true;
        public bool recordHearts = true;
        public bool recordAffectionHistory = true;
        public int maxHistoryRecords = 100;
        public float updateInterval = 0.5f;
        public bool enableAnalytics = true;
    }
    
    /// <summary>
    /// 嘉宾分析数据
    /// </summary>
    [Serializable]
    public class GuestAnalyticsData
    {
        public string guestId;
        public string guestName;
        public int totalInteractions;
        public int conversationCount;
        public int dateCount;
        public int heartCount;
        public float averageAffection;
        public float peakAffection;
        public float affectionGrowthRate;
        public float interactionFrequency;
        public List<InteractionPattern> patterns;
        public DateTime lastInteractionTime;
        
        public GuestAnalyticsData()
        {
            patterns = new List<InteractionPattern>();
            lastInteractionTime = DateTime.Now;
        }
    }
    
    /// <summary>
    /// 互动模式
    /// </summary>
    [Serializable]
    public class InteractionPattern
    {
        public InteractionType type;
        public float frequency;
        public float successRate;
        public float averageAffectionGain;
    }
    
    /// <summary>
    /// 嘉宾对比数据
    /// </summary>
    [Serializable]
    public class GuestComparisonData
    {
        public string guestId1;
        public string guestId2;
        public float affectionDifference;
        public float interactionDifference;
        public float popularityDifference;
        public float recommendationScore;
    }
}
