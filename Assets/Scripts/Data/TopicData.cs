using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 话题类型
    /// </summary>
    public enum TopicType
    {
        Casual,
        Romantic,
        Deep,
        Controversial,
        Fun,
        Personal,
        Relationship,
        Lifestyle,
        Professional,
        Trending
    }

    /// <summary>
    /// 话题难度
    /// </summary>
    public enum TopicDifficulty
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }

    /// <summary>
    /// 话题数据
    /// </summary>
    [System.Serializable]
    public class TopicData
    {
        [Header("话题基础信息")]
        public string topicId;
        public string topicName;
        public string description;
        public TopicType topicType;
        public TopicDifficulty difficulty;
        public List<string> keywords = new List<string>();
        public List<string> relatedTopics = new List<string>();

        [Header("互动效果")]
        public int engagementValue;
        public int dramaPotential;
        public int romancePotential;
        public int humorValue;
        public int controversyLevel;

        [Header("适用场景")]
        public List<string> suitableScenes = new List<string>();
        public List<string> unsuitableScenes = new List<string>();
        public List<string> suitableTimes = new List<string>();

        [Header("参与条件")]
        public List<string> requiredTags = new List<string>();
        public List<string> excludedTags = new List<string>();
        public int minRelationshipLevel;
        public bool requiresChemistry;

        [Header("话题选项")]
        public List<TopicOption> options = new List<TopicOption>();

        [Header("效果加成")]
        public Dictionary<string, int> characterPreferences = new Dictionary<string, int>();
        public Dictionary<string, float> moodMultipliers = new Dictionary<string, float>();
    }

    /// <summary>
    /// 话题选项
    /// </summary>
    [System.Serializable]
    public class TopicOption
    {
        public string optionId;
        public string responseText;
        public ResponseTone tone;
        public int baseEffectValue;
        public Dictionary<string, int> characterEffects = new Dictionary<string, int>();
        public List<string> tags = new List<string>();
        public bool isControversial;
        public bool leadsToDeepConversation;
        public string followUpTopicId;
    }

    /// <summary>
    /// 回应语气
    /// </summary>
    public enum ResponseTone
    {
        Positive,
        Negative,
        Neutral,
        Flirty,
        Sincere,
        Humorous,
        Serious,
        Playful,
        Romantic,
        Defensive
    }

    /// <summary>
    /// 话题使用记录
    /// </summary>
    [System.Serializable]
    public class TopicUsageRecord
    {
        public string recordId;
        public string topicId;
        public DateTime usageTime;
        public List<string> participantIds = new List<string>();
        public string selectedOptionId;
        public TopicOutcome outcome;
        public int engagementScore;
        public string notes;
    }

    /// <summary>
    /// 话题结果
    /// </summary>
    public enum TopicOutcome
    {
        Success,
        PartialSuccess,
        Failure,
        Conflict,
        Romantic,
        Hilarious,
        Awkward,
        Deep
    }

    /// <summary>
    /// 话题库数据
    /// </summary>
    [System.Serializable]
    public class TopicLibrary
    {
        [Header("所有话题")]
        public List<TopicData> allTopics = new List<TopicData>();

        [Header("分类话题")]
        public List<TopicData> casualTopics = new List<TopicData>();
        public List<TopicData> romanticTopics = new List<TopicData>();
        public List<TopicData> deepTopics = new List<TopicData>();
        public List<TopicData> controversialTopics = new List<TopicData>();
        public List<TopicData> funTopics = new List<TopicData>();

        [Header("热门话题")]
        public List<string> trendingTopicIds = new List<string>();
        public List<string> recommendedTopicIds = new List<string>();

        [Header("使用统计")]
        public Dictionary<string, int> topicUsageCount = new Dictionary<string, int>();
        public Dictionary<string, int> topicSuccessCount = new Dictionary<string, int>();
        public Dictionary<TopicType, int> typeUsageCount = new Dictionary<TopicType, int>();

        [Header("话题效果记录")]
        public List<TopicUsageRecord> usageHistory = new List<TopicUsageRecord>();
        public int totalTopicsUsed;
        public int successfulTopics;

        [Header("话题冷却")]
        public Dictionary<string, DateTime> topicCooldowns = new Dictionary<string, DateTime>();
        public int cooldownHours = 24;

        [Header("角色对话偏好")]
        public Dictionary<string, List<string>> characterPreferredTopics = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> characterAvoidedTopics = new Dictionary<string, List<string>>();

        [Header("特殊话题解锁")]
        public List<string> unlockedSpecialTopics = new List<string>();
        public List<string> hiddenTopics = new List<string>();
    }

    /// <summary>
    /// 快速对话配置
    /// </summary>
    [System.Serializable]
    public class QuickTalkConfig
    {
        public string configId;
        public string situation;
        public List<QuickTalkOption> options = new List<QuickTalkOption>();
        public int timeLimit;
        public bool isTimed;
    }

    /// <summary>
    /// 快速对话选项
    /// </summary>
    [System.Serializable]
    public class QuickTalkOption
    {
        public string optionId;
        public string text;
        public ResponseTone tone;
        public int effectiveness;
        public List<string> tags = new List<string>();
    }

    /// <summary>
    /// 对话树数据
    /// </summary>
    [System.Serializable]
    public class DialogueTree
    {
        public string treeId;
        public string treeName;
        public string characterId;
        public List<DialogueNode> nodes = new List<DialogueNode>();
        public string startNodeId;
    }

    /// <summary>
    /// 对话节点
    /// </summary>
    [System.Serializable]
    public class DialogueNode
    {
        public string nodeId;
        public string speakerId;
        public string dialogueText;
        public List<DialogueChoice> choices = new List<DialogueChoice>();
        public string nextNodeId;
        public bool isEndNode;
        public List<DialogueEffect> effects = new List<DialogueEffect>();
    }

    /// <summary>
    /// 对话选择
    /// </summary>
    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceId;
        public string choiceText;
        public string nextNodeId;
        public int requiredAffection;
        public List<DialogueEffect> effects = new List<DialogueEffect>();
        public List<string> requiredFlags = new List<string>();
    }

    /// <summary>
    /// 对话效果
    /// </summary>
    [System.Serializable]
    public class DialogueEffect
    {
        public EffectType effectType;
        public string targetId;
        public int value;
        public string flagToSet;
    }

    /// <summary>
    /// 话题工具类
    /// </summary>
    public static class TopicHelper
    {
        public static TopicData GetRandomTopic(TopicLibrary library, TopicType type, List<string> participantTags)
        {
            var eligibleTopics = library.allTopics.FindAll(t =>
                t.topicType == type &&
                !IsOnCooldown(library, t.topicId) &&
                MeetsRequirements(t, participantTags));

            if (eligibleTopics.Count == 0)
            {
                return null;
            }

            return eligibleTopics[UnityEngine.Random.Range(0, eligibleTopics.Count)];
        }

        public static bool IsOnCooldown(TopicLibrary library, string topicId)
        {
            if (library.topicCooldowns.TryGetValue(topicId, out DateTime cooldownEnd))
            {
                return DateTime.Now < cooldownEnd;
            }
            return false;
        }

        public static bool MeetsRequirements(TopicData topic, List<string> participantTags)
        {
            foreach (var required in topic.requiredTags)
            {
                if (!participantTags.Contains(required))
                {
                    return false;
                }
            }

            foreach (var excluded in topic.excludedTags)
            {
                if (participantTags.Contains(excluded))
                {
                    return false;
                }
            }

            return true;
        }

        public static int CalculateEngagement(TopicData topic, List<string> participantIds, List<ResponseTone> tones)
        {
            int baseEngagement = topic.engagementValue;

            int toneBonus = 0;
            foreach (var tone in tones)
            {
                if (tone == ResponseTone.Flirty || tone == ResponseTone.Romantic)
                {
                    toneBonus += topic.romancePotential;
                }
                else if (tone == ResponseTone.Humorous || tone == ResponseTone.Playful)
                {
                    toneBonus += topic.humorValue;
                }
                else if (tone == ResponseTone.Serious || tone == ResponseTone.Sincere)
                {
                    toneBonus += topic.dramaPotential;
                }
            }

            return baseEngagement + toneBonus;
        }

        public static TopicOutcome GetOutcomeFromTones(List<ResponseTone> tones, int successRate)
        {
            if (successRate >= 80)
            {
                if (tones.Contains(ResponseTone.Flirty) || tones.Contains(ResponseTone.Romantic))
                {
                    return TopicOutcome.Romantic;
                }
                if (tones.Contains(ResponseTone.Humorous))
                {
                    return TopicOutcome.Hilarious;
                }
                if (tones.Contains(ResponseTone.Sincere))
                {
                    return TopicOutcome.Deep;
                }
                return TopicOutcome.Success;
            }
            else if (successRate >= 50)
            {
                return TopicOutcome.PartialSuccess;
            }
            else if (successRate >= 30)
            {
                return TopicOutcome.Awkward;
            }
            else
            {
                return TopicOutcome.Conflict;
            }
        }
    }
}
