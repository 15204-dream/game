using System;

namespace 糟糕是心动鸭.Dialogue.GuestMode
{
    /// <summary>
    /// 情感计算器
    /// 计算和管理角色的情感状态
    /// </summary>
    public class EmotionCalculator
    {
        /// <summary>
        /// 情感状态阈值配置
        /// </summary>
        private EmotionThresholds thresholds;

        /// <summary>
        /// 情感衰减率（每分钟）
        /// </summary>
        private const float EMOTION_DECAY_RATE = 0.5f;

        /// <summary>
        /// 最大情感值
        /// </summary>
        private const float MAX_EMOTION = 100f;

        /// <summary>
        /// 最小情感值
        /// </summary>
        private const float MIN_EMOTION = 0f;

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EmotionCalculator()
        {
            thresholds = new EmotionThresholds();
        }

        #endregion

        #region 情感计算

        /// <summary>
        /// 计算情感变化
        /// </summary>
        /// <param name="option">对话选项</param>
        /// <param name="controller">对话控制器</param>
        /// <returns>情感变化值</returns>
        public float CalculateEmotionChange(DialogueOption option, CharacterDialogueController controller)
        {
            if (option == null) return 0f;

            float baseChange = option.EmotionChange;

            float affectionBonus = CalculateAffectionBonus(controller.CurrentAffection);

            float personalityBonus = CalculatePersonalityBonus(controller.CharacterData, option);

            float stateBonus = CalculateStateBonus(controller);

            float finalChange = baseChange * (1 + affectionBonus + personalityBonus + stateBonus);

            return Mathf.Clamp(finalChange, -30f, 30f);
        }

        /// <summary>
        /// 计算好感度加成
        /// </summary>
        /// <param name="affection">好感度</param>
        /// <returns>加成值</returns>
        private float CalculateAffectionBonus(int affection)
        {
            if (affection >= 80) return 0.5f;
            if (affection >= 60) return 0.3f;
            if (affection >= 40) return 0.1f;
            return 0f;
        }

        /// <summary>
        /// 计算性格加成
        /// </summary>
        /// <param name="character">角色数据</param>
        /// <param name="option">选项</param>
        /// <returns>加成值</returns>
        private float CalculatePersonalityBonus(CharacterData character, DialogueOption option)
        {
            float bonus = 0f;

            foreach (var tag in character.PersonalityTags)
            {
                if (option.HasEffectTag(tag))
                {
                    bonus += 0.2f;
                }
            }

            if (option.OptionType == DialogueOptionType.Heart)
            {
                if (character.PersonalityTags.Contains("浪漫"))
                {
                    bonus += 0.3f;
                }
                if (character.PersonalityTags.Contains("傲娇"))
                {
                    bonus -= 0.1f;
                }
            }

            return bonus;
        }

        /// <summary>
        /// 计算状态加成
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <returns>加成值</returns>
        private float CalculateStateBonus(CharacterDialogueController controller)
        {
            float bonus = 0f;

            if (controller.IsInHeartState())
            {
                bonus += 0.4f;
            }

            if (controller.HasRecentPositiveInteraction())
            {
                bonus += 0.2f;
            }

            int recentRounds = GetRecentRoundsCount(controller);
            if (recentRounds >= 3)
            {
                bonus += 0.15f;
            }

            return bonus;
        }

        /// <summary>
        /// 获取最近回合数
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <returns>回合数</returns>
        private int GetRecentRoundsCount(CharacterDialogueController controller)
        {
            return controller.DialogueRound;
        }

        #endregion

        #region 情感加成

        /// <summary>
        /// 计算情感加成
        /// </summary>
        /// <param name="currentEmotion">当前情感值</param>
        /// <param name="option">选项</param>
        /// <returns>加成值</returns>
        public float CalculateEmotionBonus(float currentEmotion, DialogueOption option)
        {
            float bonus = 0f;

            if (currentEmotion >= thresholds.High)
            {
                bonus += 0.3f;
            }
            else if (currentEmotion >= thresholds.Medium)
            {
                bonus += 0.15f;
            }

            if (option.IsHeartChoice)
            {
                if (currentEmotion >= thresholds.High)
                {
                    bonus += 0.5f;
                }
                else if (currentEmotion >= thresholds.Medium)
                {
                    bonus += 0.25f;
                }
            }

            return bonus;
        }

        /// <summary>
        /// 计算衰减
        /// </summary>
        /// <param name="elapsedMinutes">经过的分钟数</param>
        /// <returns>衰减量</returns>
        public float CalculateDecay(float elapsedMinutes)
        {
            return EMOTION_DECAY_RATE * elapsedMinutes;
        }

        /// <summary>
        /// 应用衰减
        /// </summary>
        /// <param name="currentEmotion">当前情感值</param>
        /// <param name="elapsedMinutes">经过的分钟数</param>
        /// <returns>衰减后的情感值</returns>
        public float ApplyDecay(float currentEmotion, float elapsedMinutes)
        {
            float decay = CalculateDecay(elapsedMinutes);
            return Mathf.Max(currentEmotion - decay, MIN_EMOTION);
        }

        #endregion

        #region 情感状态

        /// <summary>
        /// 获取情感状态
        /// </summary>
        /// <param name="emotion">情感值</param>
        /// <returns>状态描述</returns>
        public string GetEmotionState(float emotion)
        {
            if (emotion >= thresholds.Extreme) return "极度心动";
            if (emotion >= thresholds.High) return "非常心动";
            if (emotion >= thresholds.MediumHigh) return "有点心动";
            if (emotion >= thresholds.Medium) return "心跳加速";
            if (emotion >= thresholds.Low) return "有好感";
            return "普通";
        }

        /// <summary>
        /// 检查是否为心动状态
        /// </summary>
        /// <param name="emotion">情感值</param>
        /// <returns>是否心动</returns>
        public bool IsHeartFluttering(float emotion)
        {
            return emotion >= thresholds.MediumHigh;
        }

        /// <summary>
        /// 检查是否极度心动
        /// </summary>
        /// <param name="emotion">情感值</param>
        /// <returns>是否极度心动</returns>
        public bool IsExtremelyHeartFluttering(float emotion)
        {
            return emotion >= thresholds.Extreme;
        }

        /// <summary>
        /// 获取情感变化描述
        /// </summary>
        /// <param name="change">变化值</param>
        /// <returns>描述文本</returns>
        public string GetEmotionChangeDescription(float change)
        {
            if (change >= 15f) return "心跳加速！";
            if (change >= 8f) return "有点心动~";
            if (change >= 3f) return "好感上升";
            if (change >= 0f) return "感觉不错";
            if (change >= -3f) return "没什么感觉";
            if (change >= -8f) return "有点失望...";
            return "好难过...";
        }

        #endregion

        #region 特殊计算

        /// <summary>
        /// 计算心动时刻触发概率
        /// </summary>
        /// <param name="affection">好感度</param>
        /// <param name="emotion">情感值</param>
        /// <param name="isHeartChoice">是否心动选项</param>
        /// <returns>触发概率（0-1）</returns>
        public float CalculateHeartMomentProbability(int affection, float emotion, bool isHeartChoice)
        {
            float baseProbability = 0.1f;

            if (affection >= 70) baseProbability += 0.2f;
            else if (affection >= 50) baseProbability += 0.1f;

            if (emotion >= 60) baseProbability += 0.25f;
            else if (emotion >= 40) baseProbability += 0.1f;

            if (isHeartChoice) baseProbability += 0.3f;

            return Mathf.Min(baseProbability, 0.95f);
        }

        /// <summary>
        /// 计算告白成功率
        /// </summary>
        /// <param name="affection">好感度</param>
        /// <param name="emotion">情感值</param>
        /// <param name="difficulty">角色难度</param>
        /// <returns>成功率（0-1）</returns>
        public float CalculateConfessionSuccessRate(int affection, float emotion, int difficulty)
        {
            float baseRate = 0f;

            baseRate = affection * 0.6f / 100f;
            baseRate += emotion * 0.3f / 100f;

            float difficultyModifier = 1f - (difficulty - 1) * 0.1f;
            baseRate *= difficultyModifier;

            if (emotion >= 80f && affection >= 85)
            {
                baseRate += 0.15f;
            }

            return Mathf.Clamp(baseRate, 0f, 1f);
        }

        #endregion

        #region Unity特定

        private static class Mathf
        {
            public static float Clamp(float value, float min, float max)
            {
                return System.Math.Max(min, System.Math.Min(max, value));
            }

            public static float Min(float a, float b)
            {
                return System.Math.Min(a, b);
            }
        }

        #endregion
    }

    /// <summary>
    /// 情感阈值配置
    /// </summary>
    [Serializable]
    public class EmotionThresholds
    {
        /// <summary>
        /// 极高阈值
        /// </summary>
        public float Extreme = 90f;

        /// <summary>
        /// 高阈值
        /// </summary>
        public float High = 70f;

        /// <summary>
        /// 中高阈值
        /// </summary>
        public float MediumHigh = 50f;

        /// <summary>
        /// 中阈值
        /// </summary>
        public float Medium = 30f;

        /// <summary>
        /// 低阈值
        /// </summary>
        public float Low = 10f;
    }
}
