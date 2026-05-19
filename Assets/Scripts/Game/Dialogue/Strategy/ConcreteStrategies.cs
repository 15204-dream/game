using System;
using System.Collections.Generic;
using UnityEngine;

namespace 糟糕是心动鸭.Dialogue.Strategy
{
    /// <summary>
    /// 礼貌疏离策略
    /// 保持礼貌但保持一定距离的对话方式
    /// </summary>
    public class PoliteStrategy : DialogueStrategy
    {
        public override DialogueStrategyType StrategyType => DialogueStrategyType.Polite;

        public override string Name => "礼貌疏离";

        public override string Description => "保持礼貌和尊重，但保持一定距离。适合刚认识或在公共场合的对话。";

        /// <summary>
        /// 礼貌用语库
        /// </summary>
        private string[] politePhrases = new[]
        {
            "很高兴认识你",
            "请问...",
            "打扰一下",
            "非常感谢",
            "不好意思",
            "请多关照",
            "很高兴和你聊天",
            "请问我可以..."
        };

        /// <summary>
        /// 回应模板
        /// </summary>
        private string[] responseTemplates = new[]
        {
            "好的，我明白了",
            "这样啊，我知道了",
            "好的，没问题",
            "明白了，谢谢你",
            "原来如此"
        };

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PoliteStrategy()
        {
            weight = 0.4f;
        }

        #endregion

        #region 核心方法实现

        /// <summary>
        /// 计算响应文本
        /// </summary>
        public override string CalculateResponse(DialogueStrategyContext context)
        {
            float random = UnityEngine.Random.value;

            if (random < 0.3f)
            {
                int index = UnityEngine.Random.Range(0, politePhrases.Length);
                return politePhrases[index];
            }
            else if (random < 0.7f)
            {
                int index = UnityEngine.Random.Range(0, responseTemplates.Length);
                return responseTemplates[index];
            }
            else
            {
                return GeneratePoliteFollowUp(context);
            }
        }

        /// <summary>
        /// 选择最佳选项
        /// </summary>
        public override int SelectBestOption(List<DialogueOption> options, DialogueStrategyContext context)
        {
            if (options == null || options.Count == 0) return -1;

            int bestIndex = 0;
            float bestScore = float.MinValue;

            for (int i = 0; i < options.Count; i++)
            {
                float score = EvaluateOption(options[i], context);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// 评估选项得分
        /// </summary>
        private float EvaluateOption(DialogueOption option, DialogueStrategyContext context)
        {
            float score = 0f;

            if (option.AffectionChange > 0)
            {
                score += option.AffectionChange * 0.5f;
            }
            else if (option.AffectionChange < 0)
            {
                score += option.AffectionChange * 1.5f;
            }

            if (option.OptionType == DialogueOptionType.Heart ||
                option.OptionType == DialogueOptionType.Dangerous)
            {
                score -= 30f;
            }

            if (context.Round < 3 && option.AffectionRequired > 40)
            {
                score -= 20f;
            }

            return score;
        }

        /// <summary>
        /// 生成礼貌的后续对话
        /// </summary>
        private string GeneratePoliteFollowUp(DialogueStrategyContext context)
        {
            string[] followUps = new[]
            {
                "你平时有什么爱好吗？",
                "你是做什么工作的呢？",
                "很高兴能和你聊天",
                "这里的氛围真不错",
                "今天的活动感觉怎么样？"
            };

            return followUps[UnityEngine.Random.Range(0, followUps.Length)];
        }

        #endregion

        #region 策略特定方法

        /// <summary>
        /// 获取疏离度
        /// </summary>
        /// <returns>疏离度（0-1）</returns>
        public float GetDistanceLevel()
        {
            return 1f - weight;
        }

        /// <summary>
        /// 检查是否可以更亲密
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>是否可以</returns>
        public bool CanBeCloser(DialogueStrategyContext context)
        {
            return context.CurrentAffection >= 50 && context.Round >= 3;
        }

        #endregion
    }

    /// <summary>
    /// 轻松随意策略
    /// 轻松友好的对话方式
    /// </summary>
    public class CasualStrategy : DialogueStrategy
    {
        public override DialogueStrategyType StrategyType => DialogueStrategyType.Casual;

        public override string Name => "轻松随意";

        public override string Description => "轻松友好的对话方式，营造舒适的氛围。适合日常相处。";

        /// <summary>
        /// 随意用语库
        /// </summary>
        private string[] casualPhrases = new[]
        {
            "嘿！",
            "哈喽~",
            "哎呦！",
            "哇塞！",
            "不错不错！",
            "哈哈哈！",
            "真的吗？",
            "那必须的！"
        };

        /// <summary>
        /// 回应模板
        /// </summary>
        private string[] responseTemplates = new[]
        {
            "哈哈，太有意思了",
            "对对对！",
            "我也这么觉得",
            "太好笑了",
            "嘿嘿~"
        };

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CasualStrategy()
        {
            weight = 0.6f;
        }

        #endregion

        #region 核心方法实现

        /// <summary>
        /// 计算响应文本
        /// </summary>
        public override string CalculateResponse(DialogueStrategyContext context)
        {
            float random = UnityEngine.Random.value;

            if (random < 0.35f)
            {
                int index = UnityEngine.Random.Range(0, casualPhrases.Length);
                return casualPhrases[index];
            }
            else if (random < 0.75f)
            {
                int index = UnityEngine.Random.Range(0, responseTemplates.Length);
                return responseTemplates[index];
            }
            else
            {
                return GenerateCasualFollowUp(context);
            }
        }

        /// <summary>
        /// 选择最佳选项
        /// </summary>
        public override int SelectBestOption(List<DialogueOption> options, DialogueStrategyContext context)
        {
            if (options == null || options.Count == 0) return -1;

            int bestIndex = 0;
            float bestScore = float.MinValue;

            for (int i = 0; i < options.Count; i++)
            {
                float score = EvaluateOption(options[i], context);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// 评估选项得分
        /// </summary>
        private float EvaluateOption(DialogueOption option, DialogueStrategyContext context)
        {
            float score = 0f;

            score += option.AffectionChange * 1.2f;

            score += option.EmotionChange * 0.5f;

            if (option.IsHeartChoice && context.CurrentAffection < 40)
            {
                score -= 15f;
            }

            if (option.OptionType == DialogueOptionType.Dangerous)
            {
                score -= 25f;
            }

            return score;
        }

        /// <summary>
        /// 生成随意的后续对话
        /// </summary>
        private string GenerateCasualFollowUp(DialogueStrategyContext context)
        {
            string[] followUps = new[]
            {
                "话说...你觉得呢？",
                "哎，我们去玩那个吧！",
                "哇，这个好棒！",
                "你饿不饿？",
                "要不我们去那边看看？",
                "哈哈，太好玩了！"
            };

            return followUps[UnityEngine.Random.Range(0, followUps.Length)];
        }

        #endregion

        #region 策略特定方法

        /// <summary>
        /// 获取友好度
        /// </summary>
        /// <returns>友好度（0-1）</returns>
        public float GetFriendlinessLevel()
        {
            return weight;
        }

        #endregion
    }

    /// <summary>
    /// 亲密互动策略
    /// 亲密友好的对话方式
    /// </summary>
    public class IntimateStrategy : DialogueStrategy
    {
        public override DialogueStrategyType StrategyType => DialogueStrategyType.Intimate;

        public override string Name => "亲密互动";

        public override string Description => "亲密友好的对话方式，表达关心和在意。适合感情升温阶段。";

        /// <summary>
        /// 亲密用语库
        /// </summary>
        private string[] intimatePhrases = new[]
        {
            "你今天看起来很开心呢",
            "有你在真好",
            "我一直想着你呢",
            "你对我很重要",
            "和你在一起很舒服",
            "我很在乎你的感受"
        };

        /// <summary>
        /// 回应模板
        /// </summary>
        private string[] responseTemplates = new[]
        {
            "谢谢你的关心~",
            "你真贴心",
            "有你在真好",
            "我也很喜欢你",
            "让我想想..."
        };

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public IntimateStrategy()
        {
            weight = 0.7f;
        }

        #endregion

        #region 核心方法实现

        /// <summary>
        /// 计算响应文本
        /// </summary>
        public override string CalculateResponse(DialogueStrategyContext context)
        {
            float random = UnityEngine.Random.value;

            if (random < 0.4f)
            {
                int index = UnityEngine.Random.Range(0, intimatePhrases.Length);
                return intimatePhrases[index];
            }
            else if (random < 0.8f)
            {
                int index = UnityEngine.Random.Range(0, responseTemplates.Length);
                return responseTemplates[index];
            }
            else
            {
                return GenerateIntimateFollowUp(context);
            }
        }

        /// <summary>
        /// 选择最佳选项
        /// </summary>
        public override int SelectBestOption(List<DialogueOption> options, DialogueStrategyContext context)
        {
            if (options == null || options.Count == 0) return -1;

            int bestIndex = 0;
            float bestScore = float.MinValue;

            for (int i = 0; i < options.Count; i++)
            {
                float score = EvaluateOption(options[i], context);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// 评估选项得分
        /// </summary>
        private float EvaluateOption(DialogueOption option, DialogueStrategyContext context)
        {
            float score = 0f;

            score += option.AffectionChange * 1.5f;

            if (option.IsHeartChoice)
            {
                score += 10f;
            }

            score += option.EmotionChange * 0.8f;

            if (option.AffectionRequired > context.CurrentAffection)
            {
                score -= 20f;
            }

            return score;
        }

        /// <summary>
        /// 生成亲密的后续对话
        /// </summary>
        private string GenerateIntimateFollowUp(DialogueStrategyContext context)
        {
            string[] followUps = new[]
            {
                "等会儿我们可以单独聊聊吗？",
                "今天和你在一起很开心",
                "其实我一直想对你说...",
                "你有什么想对我说的吗？",
                "我们找个安静的地方坐坐吧"
            };

            return followUps[UnityEngine.Random.Range(0, followUps.Length)];
        }

        #endregion

        #region 策略特定方法

        /// <summary>
        /// 检查是否应该升级到浪漫策略
        /// </summary>
        public override bool ShouldTriggerSpecialReaction(DialogueStrategyContext context)
        {
            return context.CurrentAffection >= 75 && context.CurrentEmotion >= 60f;
        }

        #endregion
    }

    /// <summary>
    /// 浪漫表白策略
    /// 直接表达爱意的对话方式
    /// </summary>
    public class RomanticStrategy : DialogueStrategy
    {
        public override DialogueStrategyType StrategyType => DialogueStrategyType.Romantic;

        public override string Name => "浪漫表白";

        public override string Description => "直接表达爱意的对话方式，勇敢追求。适合感情成熟阶段的告白。";

        /// <summary>
        /// 浪漫用语库
        /// </summary>
        private string[] romanticPhrases = new[]
        {
            "我好像喜欢上你了",
            "你是我心动的人",
            "我愿意为你付出一切",
            "你是我想要在一起的人",
            "自从遇见你，我的生活变了",
            "我一直在想你"
        };

        /// <summary>
        /// 回应模板
        /// </summary>
        private string[] responseTemplates = new[]
        {
            "真的吗？！",
            "我也...我也喜欢你",
            "让我好好想想",
            "我们需要谈谈",
            "这太突然了..."
        };

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public RomanticStrategy()
        {
            weight = 0.9f;
        }

        #endregion

        #region 核心方法实现

        /// <summary>
        /// 计算响应文本
        /// </summary>
        public override string CalculateResponse(DialogueStrategyContext context)
        {
            float random = UnityEngine.Random.value;

            if (random < 0.5f)
            {
                int index = UnityEngine.Random.Range(0, romanticPhrases.Length);
                return romanticPhrases[index];
            }
            else
            {
                int index = UnityEngine.Random.Range(0, responseTemplates.Length);
                return responseTemplates[index];
            }
        }

        /// <summary>
        /// 选择最佳选项
        /// </summary>
        public override int SelectBestOption(List<DialogueOption> options, DialogueStrategyContext context)
        {
            if (options == null || options.Count == 0) return -1;

            int heartOptionIndex = -1;
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].IsHeartChoice)
                {
                    heartOptionIndex = i;
                    break;
                }
            }

            if (heartOptionIndex >= 0)
            {
                return heartOptionIndex;
            }

            int bestIndex = 0;
            float bestScore = float.MinValue;

            for (int i = 0; i < options.Count; i++)
            {
                float score = EvaluateOption(options[i], context);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// 评估选项得分
        /// </summary>
        private float EvaluateOption(DialogueOption option, DialogueStrategyContext context)
        {
            float score = 0f;

            score += option.AffectionChange * 2f;

            if (option.IsHeartChoice)
            {
                score += 30f;
            }

            if (option.OptionType == DialogueOptionType.Special)
            {
                score += 20f;
            }

            score += option.EmotionChange;

            return score;
        }

        #endregion

        #region 策略特定方法

        /// <summary>
        /// 获取浪漫度
        /// </summary>
        /// <returns>浪漫度（0-1）</returns>
        public float GetRomanceLevel()
        {
            return weight;
        }

        #endregion
    }

    /// <summary>
    /// 傲娇策略
    /// 嘴硬心软的对话方式
    /// </summary>
    public class TsundereStrategy : DialogueStrategy
    {
        public override DialogueStrategyType StrategyType => DialogueStrategyType.Tsundere;

        public override string Name => "傲娇";

        public override string Description => "嘴硬心软的对话方式，表面冷淡实则在意。适合傲娇角色的对话。";

        /// <summary>
        /// 傲娇用语库
        /// </summary>
        private string[] tsunderePhrases = new[]
        {
            "才、才不是因为你呢！",
            "哼，别误会了",
            "我只是随便看看",
            "你少臭美了",
            "谁、谁在意你了！",
            "我才没有在等你呢"
        };

        /// <summary>
        /// 傲娇回应库
        /// </summary>
        private string[] tsundereResponses = new[]
        {
            "......",
            "算了，随便你",
            "不关我的事",
            "哼",
            "......才不是呢"
        };

        /// <summary>
        /// 傲娇软化回应
        /// </summary>
        private string[] softeningResponses = new[]
        {
            "......但是，还不错",
            "......还好吧",
            "......没你想的那么糟"
        };

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public TsundereStrategy()
        {
            weight = 0.5f;
        }

        #endregion

        #region 核心方法实现

        /// <summary>
        /// 计算响应文本
        /// </summary>
        public override string CalculateResponse(DialogueStrategyContext context)
        {
            float random = UnityEngine.Random.value;

            if (random < 0.4f)
            {
                int index = UnityEngine.Random.Range(0, tsunderePhrases.Length);
                return tsunderePhrases[index];
            }
            else if (random < 0.7f)
            {
                int index = UnityEngine.Random.Range(0, tsundereResponses.Length);
                return tsundereResponses[index];
            }
            else
            {
                if (context.CurrentAffection > 60)
                {
                    int index = UnityEngine.Random.Range(0, softeningResponses.Length);
                    return softeningResponses[index];
                }
                else
                {
                    int index = UnityEngine.Random.Range(0, tsundereResponses.Length);
                    return tsundereResponses[index];
                }
            }
        }

        /// <summary>
        /// 选择最佳选项
        /// </summary>
        public override int SelectBestOption(List<DialogueOption> options, DialogueStrategyContext context)
        {
            if (options == null || options.Count == 0) return -1;

            int bestIndex = 0;
            float bestScore = float.MinValue;

            for (int i = 0; i < options.Count; i++)
            {
                float score = EvaluateOption(options[i], context);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// 评估选项得分
        /// </summary>
        private float EvaluateOption(DialogueOption option, DialogueStrategyContext context)
        {
            float score = 0f;

            if (option.AffectionChange > 0)
            {
                score += option.AffectionChange * 0.8f;
            }
            else
            {
                score += option.AffectionChange * 1.2f;
            }

            if (option.IsHeartChoice)
            {
                score -= 20f;
            }

            if (option.AffectionChange < 0)
            {
                score += 15f;
            }

            return score;
        }

        #endregion

        #region 策略特定方法

        /// <summary>
        /// 获取傲娇程度
        /// </summary>
        /// <returns>傲娇程度（0-1）</returns>
        public float GetTsundereLevel()
        {
            return 1f - weight;
        }

        /// <summary>
        /// 检查是否软化
        /// </summary>
        public bool IsSoftening(DialogueStrategyContext context)
        {
            return context.CurrentAffection > 70 || context.CurrentEmotion > 70f;
        }

        #endregion
    }

    /// <summary>
    /// 神秘试探策略
    /// 神秘感十足的对话方式
    /// </summary>
    public class MysteriousStrategy : DialogueStrategy
    {
        public override DialogueStrategyType StrategyType => DialogueStrategyType.Mysterious;

        public override string Name => "神秘试探";

        public override string Description => "神秘感十足的对话方式，让人好奇。适合制造吸引力的对话。";

        /// <summary>
        /// 神秘用语库
        /// </summary>
        private string[] mysteriousPhrases = new[]
        {
            "也许吧~",
            "谁知道呢",
            "秘密",
            "你猜？",
            "以后你就知道了",
            "这可不能告诉你",
            "呵~"
        };

        /// <summary>
        /// 神秘回应库
        /// </summary>
        private string[] mysteriousResponses = new[]
        {
            "也许吧",
            "不好说",
            "看情况吧",
            "可能吧",
            "谁知道呢"
        };

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MysteriousStrategy()
        {
            weight = 0.6f;
        }

        #endregion

        #region 核心方法实现

        /// <summary>
        /// 计算响应文本
        /// </summary>
        public override string CalculateResponse(DialogueStrategyContext context)
        {
            float random = UnityEngine.Random.value;

            if (random < 0.5f)
            {
                int index = UnityEngine.Random.Range(0, mysteriousPhrases.Length);
                return mysteriousPhrases[index];
            }
            else
            {
                int index = UnityEngine.Random.Range(0, mysteriousResponses.Length);
                return mysteriousResponses[index];
            }
        }

        /// <summary>
        /// 选择最佳选项
        /// </summary>
        public override int SelectBestOption(List<DialogueOption> options, DialogueStrategyContext context)
        {
            if (options == null || options.Count == 0) return -1;

            int bestIndex = 0;
            float bestScore = float.MinValue;

            for (int i = 0; i < options.Count; i++)
            {
                float score = EvaluateOption(options[i], context);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// 评估选项得分
        /// </summary>
        private float EvaluateOption(DialogueOption option, DialogueStrategyContext context)
        {
            float score = 0f;

            if (option.AffectionChange > 0 && option.AffectionChange < 10)
            {
                score += option.AffectionChange * 1.5f;
            }
            else
            {
                score += option.AffectionChange * 0.5f;
            }

            if (option.OptionType == DialogueOptionType.Special)
            {
                score += 15f;
            }

            if (option.OptionType == DialogueOptionType.Heart)
            {
                score -= 25f;
            }

            return score;
        }

        #endregion

        #region 策略特定方法

        /// <summary>
        /// 获取神秘度
        /// </summary>
        /// <returns>神秘度（0-1）</returns>
        public float GetMysteryLevel()
        {
            return weight;
        }

        /// <summary>
        /// 检查是否应该揭示更多
        /// </summary>
        public bool ShouldRevealMore(DialogueStrategyContext context)
        {
            return context.CurrentAffection >= 65 && context.Round >= 4;
        }

        #endregion
    }
}
