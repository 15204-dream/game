using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 危机类型
    /// </summary>
    public enum CrisisType
    {
        PublicRelations,
        GuestConflict,
        Technical,
        Production,
        Legal,
        Financial,
        Safety,
        Reputation,
        Ratings,
        SocialMedia
    }

    /// <summary>
    /// 危机严重程度
    /// </summary>
    public enum CrisisSeverity
    {
        Minor,
        Moderate,
        Serious,
        Severe,
        Critical
    }

    /// <summary>
    /// 危机状态
    /// </summary>
    public enum CrisisStatus
    {
        Potential,
        Emerging,
        Active,
        Contained,
        Resolved,
        Escalated,
        Recovering
    }

    /// <summary>
    /// 危机数据
    /// </summary>
    [System.Serializable]
    public class CrisisData
    {
        [Header("危机基础信息")]
        public string crisisId;
        public string crisisName;
        public string description;
        public CrisisType crisisType;
        public CrisisSeverity severity;
        public CrisisStatus status;

        [Header("影响范围")]
        public int audienceImpact;
        public int reputationImpact;
        public int financialImpact;
        public int heatImpact;
        public List<string> affectedGuestIds = new List<string>();
        public List<string> affectedPlatforms = new List<string>();

        [Header("时间参数")]
        public DateTime? detectionTime;
        public DateTime? startTime;
        public DateTime? endTime;
        public int durationMinutes;
        public int warningMinutes;

        [Header("解决方案")]
        public List<CrisisSolution> possibleSolutions = new List<CrisisSolution>();
        public CrisisSolution currentSolution;
        public bool isSolved;

        [Header("处理进度")]
        public float resolutionProgress;
        public int actionsTaken;
        public int maxActions;
        public bool deadlineMissed;

        [Header("后果记录")]
        public List<CrisisConsequence> consequences = new List<CrisisConsequence>();
        public int totalPenalty;
        public int recoverablePenalty;
    }

    /// <summary>
    /// 危机解决方案
    /// </summary>
    [System.Serializable]
    public class CrisisSolution
    {
        public string solutionId;
        public string solutionName;
        public string description;
        public int cost;
        public int actionPointsCost;
        public int timeRequired;
        public float successRate;
        public int reputationRestored;
        public int heatRestored;
        public int financialCost;
        public List<string> requiredAbilities = new List<string>();
        public bool isAvailable;
        public bool hasBeenTried;
        public string failureConsequence;
    }

    /// <summary>
    /// 危机后果
    /// </summary>
    [System.Serializable]
    public class CrisisConsequence
    {
        public string consequenceId;
        public string description;
        public ConsequenceType type;
        public int penaltyValue;
        public bool isPermanent;
        public int durationDays;
        public DateTime? expiryTime;
    }

    /// <summary>
    /// 后果类型
    /// </summary>
    public enum ConsequenceType
    {
        AudienceLoss,
        ReputationDamage,
        FinancialLoss,
        GuestLeave,
        SponsorPenalty,
        PlatformBan,
        LegalIssue,
        HeatReduction,
        RatingDrop
    }

    /// <summary>
    /// 预警数据
    /// </summary>
    [System.Serializable]
    public class CrisisWarning
    {
        public string warningId;
        public string crisisId;
        public CrisisType type;
        public string message;
        public int severityLevel;
        public DateTime warningTime;
        public bool isAcknowledged;
        public int timeToAct;
    }

    /// <summary>
    /// 危机记录数据 - 导演模式使用
    /// </summary>
    [System.Serializable]
    public class CrisisRecordData
    {
        [Header("当前活跃危机")]
        public List<CrisisData> activeCrises = new List<CrisisData>();
        public CrisisData mostUrgentCrisis;

        [Header("历史危机")]
        public List<CrisisData> crisisHistory = new List<CrisisData>();
        public int totalCrises;
        public int resolvedCrises;
        public int unresolvedCrises;
        public int escalatedCrises;

        [Header("预警系统")]
        public List<CrisisWarning> currentWarnings = new List<CrisisWarning>();
        public Dictionary<CrisisType, int> crisisProbability = new Dictionary<CrisisType, int>();

        [Header("危机统计")]
        public int totalPenaltyAccumulated;
        public int totalRecoverySpent;
        public int averageResolutionTime;
        public Dictionary<CrisisType, int> crisisTypeCount = new Dictionary<CrisisType, int>();
        public Dictionary<CrisisSeverity, int> severityCount = new Dictionary<CrisisSeverity, int>();

        [Header("应急资源")]
        public int emergencyBudget;
        public int usedEmergencyBudget;
        public int crisisControlAbilities;
        public List<string> emergencyPlans = new List<string>();

        [Header("风险管理")]
        public int riskLevel;
        public List<string> activeRiskFactors = new List<string>();
        public Dictionary<string, int> preventionMeasures = new Dictionary<string, int>();

        [Header("冷却和恢复")]
        public Dictionary<string, DateTime> crisisCooldowns = new Dictionary<string, DateTime>();
        public List<RecoveryRecord> recoveryHistory = new List<RecoveryRecord>();
        public int recoveryDaysRequired;

        [Header("危机处理记录")]
        public List<CrisisHandlingRecord> handlingRecords = new List<CrisisHandlingRecord>();
    }

    /// <summary>
    /// 恢复记录
    /// </summary>
    [System.Serializable]
    public class RecoveryRecord
    {
        public string recordId;
        public string crisisId;
        public DateTime startTime;
        public DateTime? completionTime;
        public int recoveryProgress;
        public int totalRecoveryNeeded;
        public bool isCompleted;
        public List<string> recoveryActions = new List<string>();
    }

    /// <summary>
    /// 危机处理记录
    /// </summary>
    [System.Serializable]
    public class CrisisHandlingRecord
    {
        public string recordId;
        public string crisisId;
        public DateTime handlingTime;
        public List<string> actionsTaken = new List<string>();
        public int totalCost;
        public bool wasSuccessful;
        public int finalPenalty;
        public string lessonsLearned;
    }

    /// <summary>
    /// 危机配置
    /// </summary>
    [System.Serializable]
    public class CrisisConfig
    {
        [Header("危机生成概率")]
        public int baseCrisisProbability = 10;
        public Dictionary<CrisisType, int> typeProbabilities = new Dictionary<CrisisType, int>();

        [Header("严重程度分布")]
        public int minorProbability = 50;
        public int moderateProbability = 30;
        public int seriousProbability = 15;
        public int severeProbability = 4;
        public int criticalProbability = 1;

        [Header("处理限制")]
        public int maxActiveCrises = 3;
        public int minResponseTime = 60;
        public int maxResolutionTime = 1800;

        [Header("惩罚配置")]
        public Dictionary<CrisisSeverity, int> severityPenalties = new Dictionary<CrisisSeverity, int>();
        public Dictionary<CrisisType, int> typePenalties = new Dictionary<CrisisType, int>();

        [Header("恢复配置")]
        public int recoveryDaysMinor = 1;
        public int recoveryDaysModerate = 3;
        public int recoveryDaysSerious = 7;
        public int recoveryDaysSevere = 14;
        public int recoveryDaysCritical = 30;
    }

    /// <summary>
    /// 危机工具类
    /// </summary>
    public static class CrisisHelper
    {
        public static CrisisSeverity CalculateSeverity(int audienceImpact, int reputationImpact, int financialImpact)
        {
            int totalImpact = audienceImpact + reputationImpact + financialImpact;

            if (totalImpact >= 50000) return CrisisSeverity.Critical;
            if (totalImpact >= 20000) return CrisisSeverity.Severe;
            if (totalImpact >= 5000) return CrisisSeverity.Serious;
            if (totalImpact >= 1000) return CrisisSeverity.Moderate;
            return CrisisSeverity.Minor;
        }

        public static float CalculateCrisisProbability(CrisisRecordData data, CrisisType type, CrisisConfig config)
        {
            int baseProb = config.baseCrisisProbability;

            if (config.typeProbabilities.TryGetValue(type, out int typeProb))
            {
                baseProb = typeProb;
            }

            if (data.riskLevel > 50)
            {
                baseProb += (data.riskLevel - 50) / 10;
            }

            int activeCount = data.activeCrises.Count;
            if (activeCount > 0)
            {
                baseProb += activeCount * 5;
            }

            return Mathf.Clamp(baseProb / 100f, 0f, 1f);
        }

        public static int CalculatePenalty(CrisisData crisis, CrisisConfig config)
        {
            int severityPenalty = config.severityPenalties.TryGetValue(crisis.severity, out int sevPen) ? sevPen : 0;
            int typePenalty = config.typePenalties.TryGetValue(crisis.crisisType, out int typePen) ? typePen : 0;

            float multiplier = 1f;
            if (crisis.deadlineMissed)
            {
                multiplier = 1.5f;
            }

            if (crisis.resolutionProgress < 0.5f)
            {
                multiplier *= 1.25f;
            }

            return Mathf.RoundToInt((severityPenalty + typePenalty) * multiplier);
        }

        public static bool CanResolveCrisis(CrisisData crisis, CrisisSolution solution, int availablePoints, int availableBudget)
        {
            if (!solution.isAvailable || solution.hasBeenTried)
            {
                return false;
            }

            if (solution.actionPointsCost > availablePoints)
            {
                return false;
            }

            if (solution.cost > availableBudget)
            {
                return false;
            }

            return true;
        }

        public static void UpdateCrisisStatus(CrisisData crisis, float deltaTime)
        {
            if (crisis.status == CrisisStatus.Active && crisis.startTime.HasValue)
            {
                var elapsed = DateTime.Now - crisis.startTime.Value;
                crisis.durationMinutes = (int)elapsed.TotalMinutes;

                if (crisis.durationMinutes >= crisis.warningMinutes && crisis.warningMinutes > 0)
                {
                    crisis.status = CrisisStatus.Escalated;
                }
            }

            if (crisis.actionsTaken >= crisis.maxActions && crisis.maxActions > 0)
            {
                crisis.deadlineMissed = true;
            }
        }
    }
}
