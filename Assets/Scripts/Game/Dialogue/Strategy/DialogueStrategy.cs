using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue.Strategy
{
    /// <summary>
    /// 对话策略基类
    /// 定义对话策略的接口和通用功能
    /// </summary>
    public abstract class DialogueStrategy
    {
        /// <summary>
        /// 策略类型
        /// </summary>
        public abstract DialogueStrategyType StrategyType { get; }

        /// <summary>
        /// 策略名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 策略描述
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 策略启用状态
        /// </summary>
        protected bool isEnabled;

        /// <summary>
        /// 策略权重
        /// </summary>
        protected float weight;

        /// <summary>
        /// 当前回合数
        /// </summary>
        protected int currentRound;

        /// <summary>
        /// 策略效果事件
        /// </summary>
        public event Action<string> OnStrategyActivated;
        public event Action<string> OnStrategyDeactivated;

        #region 属性访问器

        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public float Weight
        {
            get => weight;
            set => weight = System.Math.Clamp(value, 0f, 1f);
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DialogueStrategy()
        {
            isEnabled = true;
            weight = 0.5f;
            currentRound = 0;
        }

        #endregion

        #region 策略核心方法

        /// <summary>
        /// 计算响应文本
        /// </summary>
        /// <param name="context">对话上下文</param>
        /// <returns>响应文本</returns>
        public abstract string CalculateResponse(DialogueStrategyContext context);

        /// <summary>
        /// 选择最佳选项
        /// </summary>
        /// <param name="options">选项列表</param>
        /// <param name="context">上下文</param>
        /// <returns>最佳选项索引</returns>
        public abstract int SelectBestOption(List<DialogueOption> options, DialogueStrategyContext context);

        /// <summary>
        /// 获取好感度倍数
        /// </summary>
        /// <returns>倍数</returns>
        public virtual float GetAffectionMultiplier()
        {
            return weight;
        }

        /// <summary>
        /// 获取情感倍数
        /// </summary>
        /// <returns>倍数</returns>
        public virtual float GetEmotionMultiplier()
        {
            return weight;
        }

        /// <summary>
        /// 检查是否应该触发特殊反应
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>是否触发</returns>
        public virtual bool ShouldTriggerSpecialReaction(DialogueStrategyContext context)
        {
            return false;
        }

        #endregion

        #region 回合管理

        /// <summary>
        /// 开始新回合
        /// </summary>
        public virtual void StartNewRound()
        {
            currentRound++;
        }

        /// <summary>
        /// 重置回合
        /// </summary>
        public virtual void ResetRounds()
        {
            currentRound = 0;
        }

        /// <summary>
        /// 获取当前回合
        /// </summary>
        /// <returns>回合数</returns>
        public int GetCurrentRound()
        {
            return currentRound;
        }

        #endregion

        #region 策略调整

        /// <summary>
        /// 调整策略强度
        /// </summary>
        /// <param name="delta">变化量</param>
        public virtual void AdjustStrength(float delta)
        {
            weight = System.Math.Clamp(weight + delta, 0f, 1f);
        }

        /// <summary>
        /// 获取策略强度描述
        /// </summary>
        /// <returns>强度描述</returns>
        public virtual string GetStrengthDescription()
        {
            if (weight >= 0.8f) return "极强";
            if (weight >= 0.6f) return "较强";
            if (weight >= 0.4f) return "中等";
            if (weight >= 0.2f) return "较弱";
            return "极弱";
        }

        #endregion

        #region 策略激活

        /// <summary>
        /// 激活策略
        /// </summary>
        public virtual void Activate()
        {
            isEnabled = true;
            OnStrategyActivated?.Invoke(Name);
        }

        /// <summary>
        /// 停用策略
        /// </summary>
        public virtual void Deactivate()
        {
            isEnabled = false;
            OnStrategyDeactivated?.Invoke(Name);
        }

        /// <summary>
        /// 切换策略状态
        /// </summary>
        public virtual void Toggle()
        {
            if (isEnabled)
            {
                Deactivate();
            }
            else
            {
                Activate();
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取策略信息
        /// </summary>
        /// <returns>信息文本</returns>
        public virtual string GetInfo()
        {
            return $"[{StrategyType}] {Name}: {Description} (强度: {GetStrengthDescription()})";
        }

        /// <summary>
        /// 验证策略状态
        /// </summary>
        /// <returns>是否有效</returns>
        public virtual bool Validate()
        {
            return !string.IsNullOrEmpty(Name) && weight >= 0 && weight <= 1;
        }

        #endregion
    }

    /// <summary>
    /// 对话策略上下文
    /// </summary>
    [Serializable]
    public class DialogueStrategyContext
    {
        /// <summary>
        /// 当前好感度
        /// </summary>
        public int CurrentAffection;

        /// <summary>
        /// 当前情感值
        /// </summary>
        public float CurrentEmotion;

        /// <summary>
        /// 回合数
        /// </summary>
        public int Round;

        /// <summary>
        /// 角色性格标签
        /// </summary>
        public List<string> CharacterPersonalityTags;

        /// <summary>
        /// 角色兴趣标签
        /// </summary>
        public List<string> CharacterInterestTags;

        /// <summary>
        /// 玩家选择历史
        /// </summary>
        public List<string> PlayerChoiceHistory;

        /// <summary>
        /// 当前场景
        /// </summary>
        public string CurrentScene;

        /// <summary>
        /// 上下文标签
        /// </summary>
        public List<string> ContextTags;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DialogueStrategyContext()
        {
            CharacterPersonalityTags = new List<string>();
            CharacterInterestTags = new List<string>();
            PlayerChoiceHistory = new List<string>();
            ContextTags = new List<string>();
        }
    }

    /// <summary>
    /// 对话策略类型
    /// </summary>
    public enum DialogueStrategyType
    {
        Polite,
        Casual,
        Intimate,
        Romantic,
        Tsundere,
        Mysterious
    }

    /// <summary>
    /// 对话策略管理器
    /// </summary>
    public class DialogueStrategyManager
    {
        /// <summary>
        /// 可用策略字典
        /// </summary>
        private Dictionary<DialogueStrategyType, DialogueStrategy> strategies;

        /// <summary>
        /// 当前激活的策略
        /// </summary>
        private DialogueStrategy currentStrategy;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DialogueStrategyManager()
        {
            strategies = new Dictionary<DialogueStrategyType, DialogueStrategy>();
            InitializeStrategies();
        }

        /// <summary>
        /// 初始化策略
        /// </summary>
        private void InitializeStrategies()
        {
            strategies[DialogueStrategyType.Polite] = new PoliteStrategy();
            strategies[DialogueStrategyType.Casual] = new CasualStrategy();
            strategies[DialogueStrategyType.Intimate] = new IntimateStrategy();
            strategies[DialogueStrategyType.Romantic] = new RomanticStrategy();
            strategies[DialogueStrategyType.Tsundere] = new TsundereStrategy();
            strategies[DialogueStrategyType.Mysterious] = new MysteriousStrategy();

            currentStrategy = strategies[DialogueStrategyType.Casual];
        }

        /// <summary>
        /// 获取策略
        /// </summary>
        /// <param name="type">策略类型</param>
        /// <returns>策略实例</returns>
        public DialogueStrategy GetStrategy(DialogueStrategyType type)
        {
            return strategies.ContainsKey(type) ? strategies[type] : null;
        }

        /// <summary>
        /// 获取默认策略
        /// </summary>
        /// <returns>默认策略</returns>
        public DialogueStrategy GetDefaultStrategy()
        {
            return strategies[DialogueStrategyType.Casual];
        }

        /// <summary>
        /// 设置当前策略
        /// </summary>
        /// <param name="type">策略类型</param>
        public void SetCurrentStrategy(DialogueStrategyType type)
        {
            if (strategies.ContainsKey(type))
            {
                currentStrategy = strategies[type];
            }
        }

        /// <summary>
        /// 获取当前策略
        /// </summary>
        /// <returns>当前策略</returns>
        public DialogueStrategy GetCurrentStrategy()
        {
            return currentStrategy;
        }

        /// <summary>
        /// 获取所有策略
        /// </summary>
        /// <returns>策略字典</returns>
        public Dictionary<DialogueStrategyType, DialogueStrategy> GetAllStrategies()
        {
            return new Dictionary<DialogueStrategyType, DialogueStrategy>(strategies);
        }
    }
}
