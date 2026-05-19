using System;

namespace 糟糕是心动鸭.Dialogue.DirectorMode
{
    /// <summary>
    /// 热度计算器
    /// 计算和管理节目的热度值
    /// </summary>
    public class HeatCalculator
    {
        /// <summary>
        /// 当前热度值
        /// </summary>
        private float currentHeat;

        /// <summary>
        /// 最高热度值
        /// </summary>
        private float peakHeat;

        /// <summary>
        /// 热度历史记录
        /// </summary>
        private float[] heatHistory;

        /// <summary>
        /// 历史记录索引
        /// </summary>
        private int historyIndex;

        /// <summary>
        /// 最大历史记录数
        /// </summary>
        private const int MAX_HISTORY = 20;

        /// <summary>
        /// 最大热度值
        /// </summary>
        private const float MAX_HEAT = 100f;

        /// <summary>
        /// 最小热度值
        /// </summary>
        private const float MIN_HEAT = 0f;

        /// <summary>
        /// 热度衰减率（每回合）
        /// </summary>
        private const float HEAT_DECAY_RATE = 2f;

        /// <summary>
        /// 热度阈值配置
        /// </summary>
        private HeatThresholds thresholds;

        #region 属性访问器

        /// <summary>
        /// 获取当前热度
        /// </summary>
        public float CurrentHeat => currentHeat;

        /// <summary>
        /// 获取峰值热度
        /// </summary>
        public float PeakHeat => peakHeat;

        /// <summary>
        /// 获取平均热度
        /// </summary>
        public float AverageHeat
        {
            get
            {
                if (historyIndex == 0) return currentHeat;

                float sum = 0f;
                for (int i = 0; i < historyIndex; i++)
                {
                    sum += heatHistory[i];
                }
                return sum / historyIndex;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public HeatCalculator()
        {
            heatHistory = new float[MAX_HISTORY];
            historyIndex = 0;
            currentHeat = 50f;
            peakHeat = 50f;

            thresholds = new HeatThresholds
            {
                VeryHot = 85f,
                Hot = 70f,
                Warm = 50f,
                Cool = 30f,
                Cold = 15f
            };
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化热度计算器
        /// </summary>
        /// <param name="startingHeat">初始热度</param>
        public void Initialize(float startingHeat)
        {
            currentHeat = Mathf.Clamp(startingHeat, MIN_HEAT, MAX_HEAT);
            peakHeat = currentHeat;
            historyIndex = 0;

            for (int i = 0; i < MAX_HISTORY; i++)
            {
                heatHistory[i] = currentHeat;
            }
        }

        #endregion

        #region 热度计算

        /// <summary>
        /// 计算热度变化
        /// </summary>
        /// <param name="action">导演行动</param>
        /// <param name="reaction">观众反应</param>
        /// <param name="phase">当前阶段</param>
        /// <returns>热度变化值</returns>
        public float CalculateHeatChange(DirectorAction action, AudienceReaction reaction, PlotPhase phase)
        {
            float baseChange = GetActionHeatImpact(action.ActionType);

            float reactionModifier = GetReactionModifier(reaction.DominantReaction);

            float phaseModifier = GetPhaseModifier(phase);

            float randomVariation = GetRandomVariation();

            float finalChange = baseChange * reactionModifier * phaseModifier + randomVariation;

            return Mathf.Clamp(finalChange, -15f, 25f);
        }

        /// <summary>
        /// 获取行动热度影响
        /// </summary>
        /// <param name="actionType">行动类型</param>
        /// <returns>热度影响值</returns>
        private float GetActionHeatImpact(DirectorActionType actionType)
        {
            return actionType switch
            {
                DirectorActionType.TriggerConfession => 20f,
                DirectorActionType.SurpriseEvent => 18f,
                DirectorActionType.RevealSecret => 15f,
                DirectorActionType.CreateConflict => 12f,
                DirectorActionType.ArrangeDate => 10f,
                DirectorActionType.ScheduleActivity => 5f,
                DirectorActionType.GroupActivity => 4f,
                DirectorActionType.PrivateConversation => 3f,
                _ => 5f
            };
        }

        /// <summary>
        /// 获取反应修正
        /// </summary>
        /// <param name="reaction">反应类型</param>
        /// <returns>修正倍数</returns>
        private float GetReactionModifier(ReactionType reaction)
        {
            return reaction switch
            {
                ReactionType.SuperPositive => 1.5f,
                ReactionType.Positive => 1.2f,
                ReactionType.Neutral => 1.0f,
                ReactionType.Negative => 0.7f,
                ReactionType.SuperNegative => 0.4f,
                _ => 1.0f
            };
        }

        /// <summary>
        /// 获取阶段修正
        /// </summary>
        /// <param name="phase">阶段</param>
        /// <returns>修正倍数</returns>
        private float GetPhaseModifier(PlotPhase phase)
        {
            return phase switch
            {
                PlotPhase.Finale => 1.5f,
                PlotPhase.Climax => 1.3f,
                PlotPhase.Development => 1.0f,
                PlotPhase.Introduction => 0.8f,
                _ => 0.7f
            };
        }

        /// <summary>
        /// 获取随机变化
        /// </summary>
        /// <returns>随机变化值</returns>
        private float GetRandomVariation()
        {
            return UnityEngine.Random.Range(-3f, 5f);
        }

        #endregion

        #region 热度更新

        /// <summary>
        /// 更新热度
        /// </summary>
        /// <param name="change">变化值</param>
        public void UpdateHeat(float change)
        {
            float previousHeat = currentHeat;

            currentHeat = Mathf.Clamp(currentHeat + change, MIN_HEAT, MAX_HEAT);

            RecordHeat(currentHeat);

            if (currentHeat > peakHeat)
            {
                peakHeat = currentHeat;
            }

            ApplyDecay();
        }

        /// <summary>
        /// 记录热度到历史
        /// </summary>
        /// <param name="heat">热度值</param>
        private void RecordHeat(float heat)
        {
            heatHistory[historyIndex % MAX_HISTORY] = heat;
            historyIndex++;
        }

        /// <summary>
        /// 应用热度衰减
        /// </summary>
        private void ApplyDecay()
        {
            if (currentHeat > thresholds.Warm)
            {
                currentHeat = Mathf.Max(currentHeat - HEAT_DECAY_RATE * 0.5f, thresholds.Warm);
            }
        }

        /// <summary>
        /// 设置热度（强制设置）
        /// </summary>
        /// <param name="heat">热度值</param>
        public void SetHeat(float heat)
        {
            currentHeat = Mathf.Clamp(heat, MIN_HEAT, MAX_HEAT);
            if (currentHeat > peakHeat)
            {
                peakHeat = currentHeat;
            }
        }

        /// <summary>
        /// 增加热度
        /// </summary>
        /// <param name="amount">增加量</param>
        public void IncreaseHeat(float amount)
        {
            UpdateHeat(amount);
        }

        /// <summary>
        /// 减少热度
        /// </summary>
        /// <param name="amount">减少量</param>
        public void DecreaseHeat(float amount)
        {
            UpdateHeat(-amount);
        }

        #endregion

        #region 热度等级

        /// <summary>
        /// 获取热度等级
        /// </summary>
        /// <returns>等级描述</returns>
        public string GetHeatRating()
        {
            if (currentHeat >= thresholds.VeryHot) return "爆火";
            if (currentHeat >= thresholds.Hot) return "火热";
            if (currentHeat >= thresholds.Warm) return "升温";
            if (currentHeat >= thresholds.Cool) return "平稳";
            if (currentHeat >= thresholds.Cold) return "冷淡";
            return "冰冷";
        }

        /// <summary>
        /// 获取详细等级
        /// </summary>
        /// <returns>详细等级描述</returns>
        public string GetDetailedRating()
        {
            if (currentHeat >= thresholds.VeryHot)
                return "爆火！全网热议！";
            if (currentHeat >= thresholds.Hot)
                return "非常火热，收视飙升！";
            if (currentHeat >= thresholds.Hot - 10)
                return "热度很高，观众期待";
            if (currentHeat >= thresholds.Warm)
                return "热度适中，稳步发展";
            if (currentHeat >= thresholds.Cool)
                return "热度一般，需要刺激";
            if (currentHeat >= thresholds.Cold)
                return "热度低迷，需要爆点";
            return "热度极低，节目危机";
        }

        /// <summary>
        /// 检查是否为高热度
        /// </summary>
        /// <returns>是否高热度</returns>
        public bool IsHighHeat()
        {
            return currentHeat >= thresholds.Hot;
        }

        /// <summary>
        /// 检查是否需要紧急处理
        /// </summary>
        /// <returns>是否紧急</returns>
        public bool NeedsUrgentAction()
        {
            return currentHeat <= thresholds.Cold;
        }

        /// <summary>
        /// 获取推荐行动类型
        /// </summary>
        /// <returns>推荐行动类型</returns>
        public DirectorActionType GetRecommendedActionType()
        {
            if (currentHeat >= thresholds.VeryHot)
                return DirectorActionType.TriggerConfession;

            if (currentHeat >= thresholds.Hot)
                return DirectorActionType.SurpriseEvent;

            if (currentHeat >= thresholds.Warm)
                return DirectorActionType.CreateConflict;

            if (currentHeat >= thresholds.Cool)
                return DirectorActionType.RevealSecret;

            return DirectorActionType.ArrangeDate;
        }

        #endregion

        #region 趋势分析

        /// <summary>
        /// 获取热度趋势
        /// </summary>
        /// <returns>趋势类型</returns>
        public HeatTrend GetTrend()
        {
            if (historyIndex < 3) return HeatTrend.Stable;

            int recentCount = Mathf.Min(5, historyIndex);
            float recentSum = 0f;

            for (int i = 0; i < recentCount; i++)
            {
                int index = (historyIndex - 1 - i + MAX_HISTORY) % MAX_HISTORY;
                recentSum += heatHistory[index];
            }

            float recentAverage = recentSum / recentCount;

            if (recentAverage > currentHeat + 5f)
                return HeatTrend.Rising;
            if (recentAverage < currentHeat - 5f)
                return HeatTrend.Falling;

            return HeatTrend.Stable;
        }

        /// <summary>
        /// 预测下一回合热度
        /// </summary>
        /// <param name="plannedAction">计划行动</param>
        /// <returns>预测热度</returns>
        public float PredictNextHeat(DirectorActionType plannedAction)
        {
            float estimatedChange = GetActionHeatImpact(plannedAction) * 0.8f;
            return Mathf.Min(currentHeat + estimatedChange, MAX_HEAT);
        }

        #endregion

        #region Unity特定

        private static class Mathf
        {
            public static float Clamp(float value, float min, float max)
            {
                return System.Math.Max(min, System.Math.Min(max, value));
            }

            public static int Min(int a, int b)
            {
                return System.Math.Min(a, b);
            }

            public static float Max(float a, float b)
            {
                return System.Math.Max(a, b);
            }
        }

        #endregion
    }

    /// <summary>
    /// 热度阈值配置
    /// </summary>
    [Serializable]
    public class HeatThresholds
    {
        /// <summary>
        /// 非常热
        /// </summary>
        public float VeryHot = 85f;

        /// <summary>
        /// 热
        /// </summary>
        public float Hot = 70f;

        /// <summary>
        /// 温暖
        /// </summary>
        public float Warm = 50f;

        /// <summary>
        /// 凉爽
        /// </summary>
        public float Cool = 30f;

        /// <summary>
        /// 冷
        /// </summary>
        public float Cold = 15f;
    }

    /// <summary>
    /// 热度趋势
    /// </summary>
    public enum HeatTrend
    {
        Rising,
        Stable,
        Falling
    }
}
