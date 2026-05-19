using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 行动点类型
    /// </summary>
    public enum ActionPointType
    {
        General,
        Director,
        Creative,
        Emergency,
        Special
    }

    /// <summary>
    /// 行动类型
    /// </summary>
    public enum ActionType
    {
        Script,
        SceneSetup,
        GuestArrangement,
        EventTrigger,
        Promotion,
        Marketing,
        CrisisManagement,
        Interview,
        Challenge,
        SpecialScene
    }

    /// <summary>
    /// 行动数据
    /// </summary>
    [System.Serializable]
    public class ActionData
    {
        public string actionId;
        public string actionName;
        public string description;
        public ActionType actionType;
        public ActionPointType pointType;
        public int cost;
        public int cooldown;
        public float currentCooldown;
        public bool isAvailable;
        public List<string> requiredAbilities = new List<string>();
        public List<string> requiredTags = new List<string>();
        public ActionResultData result;
        public List<ActionEffect> effects = new List<ActionEffect>();
    }

    /// <summary>
    /// 行动效果
    /// </summary>
    [System.Serializable]
    public class ActionEffect
    {
        public EffectTarget target;
        public string effectType;
        public int value;
        public int duration;
        public string description;
    }

    /// <summary>
    /// 行动结果数据
    /// </summary>
    [System.Serializable]
    public class ActionResultData
    {
        public bool isSuccess;
        public int qualityScore;
        public int audienceImpact;
        public int dramaValue;
        public List<string> outcomes = new List<string>();
        public List<string> unintendedConsequences = new List<string>();
    }

    /// <summary>
    /// 行动计划
    /// </summary>
    [System.Serializable]
    public class ActionPlan
    {
        public string planId;
        public string planName;
        public int dayNumber;
        public List<PlannedAction> plannedActions = new List<PlannedAction>();
        public int totalCost;
        public bool isApproved;
        public bool isExecuted;
        public ActionPlanStatus status;
    }

    /// <summary>
    /// 计划行动
    /// </summary>
    [System.Serializable]
    public class PlannedAction
    {
        public string actionId;
        public ActionType actionType;
        public int cost;
        public int executionOrder;
        public bool isMandatory;
        public bool isExecuted;
        public DateTime? scheduledTime;
        public List<string> targetGuestIds = new List<string>();
    }

    /// <summary>
    /// 行动计划状态
    /// </summary>
    public enum ActionPlanStatus
    {
        Draft,
        Submitted,
        Approved,
        Rejected,
        InExecution,
        Completed,
        Cancelled
    }

    /// <summary>
    /// 行动点数据 - 导演模式使用
    /// </summary>
    [System.Serializable]
    public class ActionPointData
    {
        [Header("行动点基础信息")]
        public int currentPoints;
        public int maxPoints;
        public int dailyRegeneration;
        public int weeklyRegeneration;
        public ActionPointType primaryPointType;

        [Header("特殊行动点")]
        public int directorPoints;
        public int maxDirectorPoints;
        public int creativePoints;
        public int maxCreativePoints;
        public int emergencyPoints;
        public int maxEmergencyPoints;

        [Header("行动记录")]
        public List<ActionRecord> actionHistory = new List<ActionRecord>();
        public Dictionary<ActionType, int> actionUsageCount = new Dictionary<ActionType, int>();
        public int totalActionsUsed;
        public int totalPointsSpent;

        [Header("每日使用统计")]
        public int dailyActionsUsed;
        public int dailyPointsUsed;
        public int dailyActionsLimit;
        public DateTime lastRegenerationTime;

        [Header("冷却管理")]
        public List<CooldownEntry> activeCooldowns = new List<CooldownEntry>();
        public Dictionary<string, float> globalCooldownMultipliers = new Dictionary<string, float>();

        [Header("行动点恢复记录")]
        public List<PointRegenerationRecord> regenerationHistory = new List<PointRegenerationRecord>();

        [Header("紧急行动")]
        public int emergencyActionsAvailable;
        public int maxEmergencyActions;
        public DateTime? emergencyRefreshTime;

        [Header("行动效率")]
        public float actionEfficiencyBonus;
        public Dictionary<string, float> actionTypeDiscounts = new Dictionary<string, float>();
        public List<string> activeBoosts = new List<string>();

        [Header("行动点配置")]
        public ActionPointConfig config = new ActionPointConfig();
    }

    /// <summary>
    /// 行动记录
    /// </summary>
    [System.Serializable]
    public class ActionRecord
    {
        public string recordId;
        public string actionId;
        public string actionName;
        public ActionType actionType;
        public DateTime timestamp;
        public int cost;
        public ActionResultData result;
        public string notes;
    }

    /// <summary>
    /// 冷却条目
    /// </summary>
    [System.Serializable]
    public class CooldownEntry
    {
        public string actionId;
        public float remainingCooldown;
        public float totalCooldown;
        public DateTime? cooldownEndTime;
    }

    /// <summary>
    /// 恢复记录
    /// </summary>
    [System.Serializable]
    public class PointRegenerationRecord
    {
        public string recordId;
        public DateTime timestamp;
        public int pointsRestored;
        public RegenerationType type;
        public string reason;
    }

    /// <summary>
    /// 恢复类型
    /// </summary>
    public enum RegenerationType
    {
        Daily,
        Weekly,
        Achievement,
        Item,
        SpecialEvent,
        EmergencyRefresh
    }

    /// <summary>
    /// 行动点配置
    /// </summary>
    [System.Serializable]
    public class ActionPointConfig
    {
        [Header("基础配置")]
        public int startingPoints = 100;
        public int maxPointsLimit = 500;
        public int dailyRegenAmount = 20;
        public int weeklyRegenAmount = 50;

        [Header("行动消耗")]
        public int scriptCost = 10;
        public int sceneSetupCost = 15;
        public int guestArrangementCost = 5;
        public int eventTriggerCost = 20;
        public int promotionCost = 25;
        public int marketingCost = 30;
        public int crisisManagementCost = 40;
        public int interviewCost = 8;
        public int challengeCost = 15;
        public int specialSceneCost = 50;

        [Header("冷却时间（秒）")]
        public float scriptCooldown = 60;
        public float sceneSetupCooldown = 120;
        public float eventTriggerCooldown = 300;
        public float promotionCooldown = 180;
        public float crisisManagementCooldown = 600;
        public float specialSceneCooldown = 1200;

        [Header("紧急行动点配置")]
        public int maxEmergencyPoints = 3;
        public int emergencyRefreshCost = 50;
        public int emergencyRefreshCooldown = 3600;
    }

    /// <summary>
    /// 行动点工具类
    /// </summary>
    public static class ActionPointHelper
    {
        public static bool CanPerformAction(ActionPointData data, ActionData action)
        {
            if (!action.isAvailable)
            {
                return false;
            }

            int requiredPoints = GetActionCost(data, action);
            if (requiredPoints > data.currentPoints)
            {
                return false;
            }

            var cooldown = data.activeCooldowns.Find(c => c.actionId == action.actionId);
            if (cooldown != null && cooldown.remainingCooldown > 0)
            {
                return false;
            }

            if (data.dailyActionsUsed >= data.dailyActionsLimit)
            {
                return false;
            }

            return true;
        }

        public static int GetActionCost(ActionPointData data, ActionData action)
        {
            int baseCost = action.cost;

            if (data.actionTypeDiscounts.ContainsKey(action.actionId))
            {
                baseCost = Mathf.RoundToInt(baseCost * (1 - data.actionTypeDiscounts[action.actionId]));
            }

            return Mathf.Max(1, baseCost);
        }

        public static void PerformAction(ActionPointData data, ActionData action, ActionResultData result)
        {
            int cost = GetActionCost(data, action);
            data.currentPoints -= cost;
            data.dailyPointsUsed += cost;
            data.dailyActionsUsed++;
            data.totalActionsUsed++;
            data.totalPointsSpent += cost;

            var record = new ActionRecord
            {
                recordId = Guid.NewGuid().ToString(),
                actionId = action.actionId,
                actionName = action.actionName,
                actionType = action.actionType,
                timestamp = DateTime.Now,
                cost = cost,
                result = result
            };
            data.actionHistory.Add(record);

            if (!data.actionUsageCount.ContainsKey(action.actionType))
            {
                data.actionUsageCount[action.actionType] = 0;
            }
            data.actionUsageCount[action.actionType]++;

            if (action.cooldown > 0)
            {
                var cooldown = new CooldownEntry
                {
                    actionId = action.actionId,
                    remainingCooldown = action.cooldown,
                    totalCooldown = action.cooldown,
                    cooldownEndTime = DateTime.Now.AddSeconds(action.cooldown)
                };
                data.activeCooldowns.Add(cooldown);
            }
        }

        public static void RegeneratePoints(ActionPointData data, int amount, RegenerationType type)
        {
            int actualRegen = Mathf.Min(amount, data.maxPoints - data.currentPoints);
            data.currentPoints += actualRegen;

            var record = new PointRegenerationRecord
            {
                recordId = Guid.NewGuid().ToString(),
                timestamp = DateTime.Now,
                pointsRestored = actualRegen,
                type = type,
                reason = $"通过{type}恢复行动点"
            };
            data.regenerationHistory.Add(record);

            data.lastRegenerationTime = DateTime.Now;
        }

        public static void UpdateCooldowns(ActionPointData data, float deltaTime)
        {
            for (int i = data.activeCooldowns.Count - 1; i >= 0; i--)
            {
                var cooldown = data.activeCooldowns[i];
                cooldown.remainingCooldown -= deltaTime;

                if (cooldown.remainingCooldown <= 0)
                {
                    data.activeCooldowns.RemoveAt(i);
                }
            }
        }
    }
}
