using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 好感度等级
    /// </summary>
    public enum FriendshipLevel
    {
        Stranger,
        Acquaintance,
        Friend,
        GoodFriend,
        CloseFriend,
        BestFriend,
        RomanticInterest,
        Dating,
        DeepLove,
        Soulmate
    }

    /// <summary>
    /// 关系状态
    /// </summary>
    [System.Serializable]
    public class RelationshipStatus
    {
        public string characterId;
        public int affectionPoints;
        public int trustPoints;
        public int intimacyPoints;
        public FriendshipLevel friendshipLevel;
        public RelationshipType currentRelationshipType;
        public List<string> sharedMemories = new List<string>();
        public List<string> insideJokes = new List<string>();
        public DateTime lastInteractionTime;
        public int interactionCount;
        public bool hasConfessed;
        public bool confessionAccepted;
        public bool isBlocked;
        public string blockReason;
    }

    /// <summary>
    /// 关系类型
    /// </summary>
    public enum RelationshipType
    {
        None,
        Stranger,
        Rival,
        Friend,
        BestFriend,
        LoveInterest,
        Dating,
        Ex,
        Married
    }

    /// <summary>
    /// 好感度变化记录
    /// </summary>
    [System.Serializable]
    public class AffectionChangeRecord
    {
        public string recordId;
        public string fromCharacterId;
        public string toCharacterId;
        public int affectionDelta;
        public int trustDelta;
        public int intimacyDelta;
        public ChangeReason reason;
        public string description;
        public DateTime timestamp;
        public string relatedEventId;
    }

    /// <summary>
    /// 变化原因类型
    /// </summary>
    public enum ChangeReason
    {
        Conversation,
        Gift,
        Date,
        Compliment,
        Help,
        Tease,
        Ignore,
        Betrayal,
        Apology,
        SpecialEvent,
        StoryProgress,
        RandomEncounter
    }

    /// <summary>
    /// 对话好感度影响
    /// </summary>
    [System.Serializable]
    public class ConversationAffectionEffect
    {
        public string conversationId;
        public int baseAffectionChange;
        public int baseTrustChange;
        public int baseIntimacyChange;
        public Dictionary<string, int> characterModifers = new Dictionary<string, int>();
        public Dictionary<string, int> traitModifers = new Dictionary<string, int>();
        public Dictionary<string, int> moodModifers = new Dictionary<string, int>();
    }

    /// <summary>
    /// 礼物好感度影响
    /// </summary>
    [System.Serializable]
    public class GiftAffectionEffect
    {
        public string giftId;
        public string giftName;
        public int rarity;
        public int baseAffectionChange;
        public int baseTrustChange;
        public int baseIntimacyChange;
        public List<string> preferredGiftTags = new List<string>();
        public List<string> dislikedGiftTags = new List<string>();
    }

    /// <summary>
    /// 约会事件影响
    /// </summary>
    [System.Serializable]
    public class DateEventAffectionEffect
    {
        public string eventId;
        public string eventName;
        public int baseAffectionChange;
        public int baseTrustChange;
        public int baseIntimacyChange;
        public int moodBonus;
        public string requiredSceneType;
        public List<string> requiredTags = new List<string>();
    }

    /// <summary>
    /// 好感度数据管理器
    /// </summary>
    [System.Serializable]
    public class FriendshipData
    {
        [Header("所有角色好感度")]
        public Dictionary<string, RelationshipStatus> allRelationships = new Dictionary<string, RelationshipStatus>();

        [Header("好感度变化记录")]
        public List<AffectionChangeRecord> affectionHistory = new List<AffectionChangeRecord>();

        [Header("好感度配置")]
        public FriendshipDataConfig config = new FriendshipDataConfig();

        [Header("特殊关系")]
        public List<string> currentRivals = new List<string>();
        public List<string> potentialRomanticInterests = new List<string>();
        public List<string> closeFriends = new List<string>();

        [Header("关系统计")]
        public int totalRelationshipsFormed;
        public int successfulConfessions;
        public int failedConfessions;
        public int brokenRelationships;
        public Dictionary<string, int> mostInteractedCharacter = new Dictionary<string, int>();
    }

    /// <summary>
    /// 好感度配置
    /// </summary>
    [System.Serializable]
    public class FriendshipDataConfig
    {
        [Header("好感度阈值")]
        public int strangerThreshold = 0;
        public int acquaintanceThreshold = 100;
        public int friendThreshold = 300;
        public int goodFriendThreshold = 600;
        public int closeFriendThreshold = 1000;
        public int bestFriendThreshold = 1500;
        public int romanticInterestThreshold = 2000;
        public int datingThreshold = 3000;
        public int deepLoveThreshold = 4500;
        public int soulmateThreshold = 6000;

        [Header("信任度阈值")]
        public int lowTrustThreshold = 100;
        public int mediumTrustThreshold = 300;
        public int highTrustThreshold = 600;
        public int fullTrustThreshold = 1000;

        [Header("亲密度阈值")]
        public int casualIntimacyThreshold = 100;
        public int comfortableIntimacyThreshold = 300;
        public int closeIntimacyThreshold = 600;
        public int intimateIntimacyThreshold = 1000;

        [Header("每日衰减设置")]
        public int dailyAffectionDecay = 5;
        public int dailyTrustDecay = 3;
        public int noInteractionDaysThreshold = 7;

        [Header("互动加成")]
        public int firstMeetingBonus = 20;
        public int dailyChatBonus = 10;
        public int giftBonusMultiplier = 1;
    }

    /// <summary>
    /// 好感度工具类
    /// </summary>
    public static class FriendshipHelper
    {
        public static FriendshipLevel GetFriendshipLevel(int affectionPoints, FriendshipDataConfig config)
        {
            if (affectionPoints >= config.soulmateThreshold) return FriendshipLevel.Soulmate;
            if (affectionPoints >= config.deepLoveThreshold) return FriendshipLevel.DeepLove;
            if (affectionPoints >= config.datingThreshold) return FriendshipLevel.Dating;
            if (affectionPoints >= config.romanticInterestThreshold) return FriendshipLevel.RomanticInterest;
            if (affectionPoints >= config.bestFriendThreshold) return FriendshipLevel.BestFriend;
            if (affectionPoints >= config.closeFriendThreshold) return FriendshipLevel.CloseFriend;
            if (affectionPoints >= config.goodFriendThreshold) return FriendshipLevel.GoodFriend;
            if (affectionPoints >= config.friendThreshold) return FriendshipLevel.Friend;
            if (affectionPoints >= config.acquaintanceThreshold) return FriendshipLevel.Acquaintance;
            return FriendshipLevel.Stranger;
        }

        public static string GetFriendshipLevelName(FriendshipLevel level)
        {
            switch (level)
            {
                case FriendshipLevel.Stranger: return "陌生人";
                case FriendshipLevel.Acquaintance: return "相识";
                case FriendshipLevel.Friend: return "朋友";
                case FriendshipLevel.GoodFriend: return "好朋友";
                case FriendshipLevel.CloseFriend: return "亲密朋友";
                case FriendshipLevel.BestFriend: return "最好的朋友";
                case FriendshipLevel.RomanticInterest: return "暧昧对象";
                case FriendshipLevel.Dating: return "约会中";
                case FriendshipLevel.DeepLove: return "深爱";
                case FriendshipLevel.Soulmate: return "灵魂伴侣";
                default: return "未知";
            }
        }

        public static int CalculateGiftEffectiveness(GiftAffectionEffect gift, List<string> preferredTags)
        {
            int baseEffectiveness = gift.baseAffectionChange;
            int tagMatchBonus = 0;

            foreach (var tag in gift.preferredGiftTags)
            {
                if (preferredTags.Contains(tag))
                {
                    tagMatchBonus += 20;
                }
            }

            foreach (var tag in gift.dislikedGiftTags)
            {
                if (preferredTags.Contains(tag))
                {
                    tagMatchBonus -= 30;
                }
            }

            return baseEffectiveness + tagMatchBonus;
        }

        public static bool CanConfess(RelationshipStatus status, FriendshipDataConfig config)
        {
            return status.affectionPoints >= config.romanticInterestThreshold &&
                   status.trustPoints >= config.highTrustThreshold &&
                   status.intimacyPoints >= config.closeIntimacyThreshold &&
                   !status.hasConfessed;
        }

        public static float GetConfessionSuccessChance(RelationshipStatus status, int playerRomanticSkill)
        {
            float baseChance = 0.5f;
            float affectionBonus = status.affectionPoints / 10000f;
            float trustBonus = status.trustPoints / 5000f;
            float intimacyBonus = status.intimacyPoints / 3000f;
            float skillBonus = playerRomanticSkill / 200f;

            return Mathf.Clamp(baseChance + affectionBonus + trustBonus + intimacyBonus + skillBonus, 0f, 0.95f);
        }
    }
}
