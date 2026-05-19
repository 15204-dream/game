using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue.DirectorMode
{
    /// <summary>
    /// 观众反应模拟器
    /// 模拟观众对节目事件的实时反应
    /// </summary>
    public class AudienceReactionSimulator
    {
        /// <summary>
        /// 观众类型分布
        /// </summary>
        private Dictionary<AudienceType, float> audienceDistribution;

        /// <summary>
        /// 反应类型权重
        /// </summary>
        private Dictionary<ReactionType, float> reactionWeights;

        /// <summary>
        /// 当前回合的反应记录
        /// </summary>
        private List<AudienceReaction> reactionHistory;

        /// <summary>
        /// 反应强度配置
        /// </summary>
        private ReactionIntensityConfig intensityConfig;

        /// <summary>
        /// 最大历史记录数
        /// </summary>
        private const int MAX_HISTORY = 50;

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AudienceReactionSimulator()
        {
            InitializeDistribution();
            InitializeReactionWeights();
            InitializeIntensityConfig();
            reactionHistory = new List<AudienceReaction>();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化观众分布
        /// </summary>
        private void InitializeDistribution()
        {
            audienceDistribution = new Dictionary<AudienceType, float>
            {
                { AudienceType.Romantic, 0.3f },
                { AudienceType.DramaLover, 0.25f },
                { AudienceType.ComedyFan, 0.2f },
                { AudienceType.Analytical, 0.15f },
                { AudienceType.Casual, 0.1f }
            };
        }

        /// <summary>
        /// 初始化反应权重
        /// </summary>
        private void InitializeReactionWeights()
        {
            reactionWeights = new Dictionary<ReactionType, float>
            {
                { ReactionType.SuperPositive, 0.15f },
                { ReactionType.Positive, 0.3f },
                { ReactionType.Neutral, 0.25f },
                { ReactionType.Negative, 0.2f },
                { ReactionType.SuperNegative, 0.1f }
            };
        }

        /// <summary>
        /// 初始化强度配置
        /// </summary>
        private void InitializeIntensityConfig()
        {
            intensityConfig = new ReactionIntensityConfig
            {
                VeryLow = 0.2f,
                Low = 0.4f,
                Medium = 0.6f,
                High = 0.8f,
                VeryHigh = 1.0f
            };
        }

        #endregion

        #region 反应模拟

        /// <summary>
        /// 模拟观众反应
        /// </summary>
        /// <param name="action">导演行动</param>
        /// <param name="currentPhase">当前阶段</param>
        /// <returns>观众反应</returns>
        public AudienceReaction SimulateReaction(DirectorAction action, PlotPhase currentPhase)
        {
            AudienceReaction reaction = new AudienceReaction
            {
                Timestamp = DateTime.Now,
                Phase = currentPhase,
                ActionType = action.ActionType
            };

            float baseIntensity = CalculateBaseIntensity(currentPhase);
            reaction.Intensity = CalculateReactionIntensity(action, baseIntensity);

            reaction.DominantReaction = SelectDominantReaction(action);
            reaction.AudienceBreakdown = GenerateAudienceBreakdown(action, currentPhase);

            reaction.Commentary = GenerateCommentary(reaction, action);
            reaction.SocialMediaBuzz = CalculateSocialMediaBuzz(reaction);

            AddToHistory(reaction);

            return reaction;
        }

        /// <summary>
        /// 计算基础强度
        /// </summary>
        /// <param name="phase">阶段</param>
        /// <returns>基础强度</returns>
        private float CalculateBaseIntensity(PlotPhase phase)
        {
            return phase switch
            {
                PlotPhase.Introduction => 0.5f,
                PlotPhase.Development => 0.65f,
                PlotPhase.Climax => 0.9f,
                PlotPhase.Finale => 1.0f,
                _ => 0.5f
            };
        }

        /// <summary>
        /// 计算反应强度
        /// </summary>
        /// <param name="action">行动</param>
        /// <param name="baseIntensity">基础强度</param>
        /// <returns>反应强度</returns>
        private float CalculateReactionIntensity(DirectorAction action, float baseIntensity)
        {
            float actionMultiplier = GetActionIntensityMultiplier(action.ActionType);
            float randomVariation = UnityEngine.Random.Range(-0.1f, 0.1f);

            return Mathf.Clamp(baseIntensity * actionMultiplier + randomVariation, 0f, 1f);
        }

        /// <summary>
        /// 获取行动强度倍数
        /// </summary>
        /// <param name="actionType">行动类型</param>
        /// <returns>倍数</returns>
        private float GetActionIntensityMultiplier(DirectorActionType actionType)
        {
            return actionType switch
            {
                DirectorActionType.TriggerConfession => 1.5f,
                DirectorActionType.SurpriseEvent => 1.4f,
                DirectorActionType.RevealSecret => 1.3f,
                DirectorActionType.CreateConflict => 1.2f,
                DirectorActionType.ArrangeDate => 1.1f,
                DirectorActionType.ScheduleActivity => 0.9f,
                DirectorActionType.GroupActivity => 0.85f,
                DirectorActionType.PrivateConversation => 0.8f,
                _ => 1.0f
            };
        }

        /// <summary>
        /// 选择主导反应
        /// </summary>
        /// <param name="action">行动</param>
        /// <returns>反应类型</returns>
        private ReactionType SelectDominantReaction(DirectorAction action)
        {
            float roll = UnityEngine.Random.value;
            float cumulative = 0f;

            foreach (var weight in reactionWeights)
            {
                cumulative += weight.Value;
                if (roll <= cumulative)
                {
                    return AdjustReactionByAction(weight.Key, action);
                }
            }

            return ReactionType.Neutral;
        }

        /// <summary>
        /// 根据行动调整反应
        /// </summary>
        /// <param name="baseReaction">基础反应</param>
        /// <param name="action">行动</param>
        /// <returns>调整后的反应</returns>
        private ReactionType AdjustReactionByAction(ReactionType baseReaction, DirectorAction action)
        {
            if (action.ActionType == DirectorActionType.CreateConflict)
            {
                if (baseReaction == ReactionType.Positive)
                    return ReactionType.SuperPositive;
            }

            if (action.ActionType == DirectorActionType.TriggerConfession)
            {
                if (baseReaction != ReactionType.Negative)
                    return ReactionType.SuperPositive;
            }

            return baseReaction;
        }

        /// <summary>
        /// 生成观众分布
        /// </summary>
        /// <param name="action">行动</param>
        /// <param name="phase">阶段</param>
        /// <returns>观众反应分布</returns>
        private Dictionary<AudienceType, ReactionType> GenerateAudienceBreakdown(
            DirectorAction action,
            PlotPhase phase)
        {
            Dictionary<AudienceType, ReactionType> breakdown = new Dictionary<AudienceType, ReactionType>();

            foreach (var audienceType in audienceDistribution.Keys)
            {
                breakdown[audienceType] = GenerateAudienceReaction(audienceType, action, phase);
            }

            return breakdown;
        }

        /// <summary>
        /// 生成单个观众的反应
        /// </summary>
        /// <param name="type">观众类型</param>
        /// <param name="action">行动</param>
        /// <param name="phase">阶段</param>
        /// <returns>反应类型</returns>
        private ReactionType GenerateAudienceReaction(
            AudienceType type,
            DirectorAction action,
            PlotPhase phase)
        {
            float modifier = GetAudienceTypeModifier(type, action);
            float threshold = 0.5f + modifier;

            if (UnityEngine.Random.value < threshold)
            {
                return type == AudienceType.DramaLover
                    ? EscalateReaction(ReactionType.Positive)
                    : ReactionType.Positive;
            }

            return ReactionType.Neutral;
        }

        /// <summary>
        /// 获取观众类型修正
        /// </summary>
        /// <param name="type">观众类型</param>
        /// <param name="action">行动</param>
        /// <returns>修正值</returns>
        private float GetAudienceTypeModifier(AudienceType type, DirectorAction action)
        {
            float modifier = 0f;

            switch (type)
            {
                case AudienceType.Romantic:
                    if (action.ActionType == DirectorActionType.TriggerConfession)
                        modifier = 0.3f;
                    break;

                case AudienceType.DramaLover:
                    if (action.ActionType == DirectorActionType.CreateConflict ||
                        action.ActionType == DirectorActionType.RevealSecret)
                        modifier = 0.25f;
                    break;

                case AudienceType.ComedyFan:
                    if (action.ActionType == DirectorActionType.SurpriseEvent)
                        modifier = 0.2f;
                    break;
            }

            return modifier;
        }

        /// <summary>
        /// 升级反应
        /// </summary>
        /// <param name="reaction">反应</param>
        /// <returns>升级后的反应</returns>
        private ReactionType EscalateReaction(ReactionType reaction)
        {
            return reaction switch
            {
                ReactionType.Neutral => ReactionType.Positive,
                ReactionType.Positive => ReactionType.SuperPositive,
                _ => reaction
            };
        }

        #endregion

        #region 反应文本生成

        /// <summary>
        /// 生成评论
        /// </summary>
        /// <param name="reaction">反应</param>
        /// <param name="action">行动</param>
        /// <returns>评论文本</returns>
        private string GenerateCommentary(AudienceReaction reaction, DirectorAction action)
        {
            string[] positiveComments = new[]
            {
                "观众反响热烈！",
                "太甜了！",
                "这一幕太浪漫了！",
                "观众疯狂尖叫！",
                "全场沸腾！"
            };

            string[] neutralComments = new[]
            {
                "观众表示还不错",
                "反响一般",
                "观众在观望",
                "气氛平稳"
            };

            string[] negativeComments = new[]
            {
                "观众有些失望",
                "反应平平",
                "观众不太买账",
                "嘘声一片"
            };

            var comments = reaction.DominantReaction >= ReactionType.Positive
                ? positiveComments
                : reaction.DominantReaction == ReactionType.Neutral
                    ? neutralComments
                    : negativeComments;

            return comments[UnityEngine.Random.Range(0, comments.Length)];
        }

        /// <summary>
        /// 计算社交媒体热度
        /// </summary>
        /// <param name="reaction">反应</param>
        /// <returns>热度值</returns>
        private float CalculateSocialMediaBuzz(AudienceReaction reaction)
        {
            float baseBuzz = reaction.Intensity * 100f;

            float reactionBonus = reaction.DominantReaction switch
            {
                ReactionType.SuperPositive => 30f,
                ReactionType.Positive => 15f,
                ReactionType.Neutral => 0f,
                ReactionType.Negative => -10f,
                ReactionType.SuperNegative => -20f,
                _ => 0f
            };

            return Mathf.Max(baseBuzz + reactionBonus, 0f);
        }

        #endregion

        #region 历史记录

        /// <summary>
        /// 添加到历史
        /// </summary>
        /// <param name="reaction">反应</param>
        private void AddToHistory(AudienceReaction reaction)
        {
            reactionHistory.Add(reaction);

            if (reactionHistory.Count > MAX_HISTORY)
            {
                reactionHistory.RemoveAt(0);
            }
        }

        /// <summary>
        /// 获取最近反应
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>反应列表</returns>
        public List<AudienceReaction> GetRecentReactions(int count = 10)
        {
            if (count <= 0) return new List<AudienceReaction>();

            int start = Mathf.Max(0, reactionHistory.Count - count);
            int length = Mathf.Min(count, reactionHistory.Count - start);

            return reactionHistory.GetRange(start, length);
        }

        /// <summary>
        /// 获取平均反应强度
        /// </summary>
        /// <returns>平均强度</returns>
        public float GetAverageIntensity()
        {
            if (reactionHistory.Count == 0) return 0f;

            float sum = 0f;
            foreach (var reaction in reactionHistory)
            {
                sum += reaction.Intensity;
            }

            return sum / reactionHistory.Count;
        }

        #endregion

        #region Unity特定

        private static class Mathf
        {
            public static float Clamp(float value, float min, float max)
            {
                return System.Math.Max(min, System.Math.Min(max, value));
            }

            public static int Max(int a, int b)
            {
                return System.Math.Max(a, b);
            }
        }

        #endregion
    }

    /// <summary>
    /// 观众反应
    /// </summary>
    [Serializable]
    public class AudienceReaction
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        /// 当前阶段
        /// </summary>
        public PlotPhase Phase;

        /// <summary>
        /// 行动类型
        /// </summary>
        public DirectorActionType ActionType;

        /// <summary>
        /// 反应强度（0-1）
        /// </summary>
        public float Intensity;

        /// <summary>
        /// 主导反应类型
        /// </summary>
        public ReactionType DominantReaction;

        /// <summary>
        /// 观众分布
        /// </summary>
        public Dictionary<AudienceType, ReactionType> AudienceBreakdown;

        /// <summary>
        /// 评论
        /// </summary>
        public string Commentary;

        /// <summary>
        /// 社交媒体热度
        /// </summary>
        public float SocialMediaBuzz;

        public AudienceReaction()
        {
            AudienceBreakdown = new Dictionary<AudienceType, ReactionType>();
        }
    }

    /// <summary>
    /// 观众类型
    /// </summary>
    public enum AudienceType
    {
        Romantic,
        DramaLover,
        ComedyFan,
        Analytical,
        Casual
    }

    /// <summary>
    /// 反应类型
    /// </summary>
    public enum ReactionType
    {
        SuperPositive,
        Positive,
        Neutral,
        Negative,
        SuperNegative
    }

    /// <summary>
    /// 反应强度配置
    /// </summary>
    [Serializable]
    public class ReactionIntensityConfig
    {
        public float VeryLow;
        public float Low;
        public float Medium;
        public float High;
        public float VeryHigh;
    }
}
