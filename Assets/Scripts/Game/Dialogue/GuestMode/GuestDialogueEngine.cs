using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace 糟糕是心动鸭.Dialogue.GuestMode
{
    /// <summary>
    /// 嘉宾模式对话引擎
    /// 管理玩家与嘉宾之间的对话交互
    /// </summary>
    public class GuestDialogueEngine : MonoBehaviour
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        private static GuestDialogueEngine instance;

        /// <summary>
        /// 当前选中的角色
        /// </summary>
        private CharacterData currentTargetCharacter;

        /// <summary>
        /// 角色对话控制器字典
        /// </summary>
        private Dictionary<string, CharacterDialogueController> characterControllers;

        /// <summary>
        /// 对话响应生成器
        /// </summary>
        private DialogueResponseGenerator responseGenerator;

        /// <summary>
        /// 情感计算器
        /// </summary>
        private EmotionCalculator emotionCalculator;

        /// <summary>
        /// 对话策略管理器
        /// </summary>
        private DialogueStrategyManager strategyManager;

        /// <summary>
        /// 对话选项列表
        /// </summary>
        private List<DialogueOption> currentOptions;

        /// <summary>
        /// 当前使用的策略
        /// </summary>
        private DialogueStrategy currentStrategy;

        /// <summary>
        /// 是否启用AI增强
        /// </summary>
        private bool useAIEnhancement;

        /// <summary>
        /// 对话事件
        /// </summary>
        public event Action<CharacterData, string> OnCharacterResponse;
        public event Action<CharacterData, float> OnEmotionChanged;
        public event Action<CharacterData, int> OnAffectionChanged;
        public event Action<DialogueOption> OnOptionChosen;
        public event Action<string> OnStrategyChanged;
        public event Action<CharacterData> OnDialogueEnded;

        #region 属性访问器

        public static GuestDialogueEngine Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GuestDialogueEngine");
                    instance = go.AddComponent<GuestDialogueEngine>();
                }
                return instance;
            }
        }

        public CharacterData CurrentTargetCharacter => currentTargetCharacter;
        public List<DialogueOption> CurrentOptions => currentOptions;
        public DialogueStrategy CurrentStrategy => currentStrategy;
        public bool UseAIEnhancement
        {
            get => useAIEnhancement;
            set => useAIEnhancement = value;
        }

        #endregion

        #region Unity生命周期

        /// <summary>
        /// Awake方法
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化引擎
        /// </summary>
        private void Initialize()
        {
            characterControllers = new Dictionary<string, CharacterDialogueController>();
            currentOptions = new List<DialogueOption>();
            useAIEnhancement = true;

            responseGenerator = new DialogueResponseGenerator();
            emotionCalculator = new EmotionCalculator();
            strategyManager = new DialogueStrategyManager();
        }

        #endregion

        #region 对话控制

        /// <summary>
        /// 开始与角色的对话
        /// </summary>
        /// <param name="character">角色数据</param>
        /// <returns>是否成功开始</returns>
        public async Task<bool> StartDialogueWith(CharacterData character)
        {
            if (character == null)
            {
                Debug.LogError("Character is null");
                return false;
            }

            currentTargetCharacter = character;

            if (!characterControllers.ContainsKey(character.Id))
            {
                CharacterDialogueController controller = new CharacterDialogueController(character);
                characterControllers[character.Id] = controller;
            }

            CharacterDialogueController dialogueController = characterControllers[character.Id];
            await dialogueController.Initialize();

            currentStrategy = strategyManager.GetDefaultStrategy();
            UpdateDialogueOptions();

            return true;
        }

        /// <summary>
        /// 选择对话选项
        /// </summary>
        /// <param name="option">选项</param>
        /// <returns>角色响应文本</returns>
        public async Task<string> SelectOption(DialogueOption option)
        {
            if (currentTargetCharacter == null || option == null)
            {
                return "";
            }

            OnOptionChosen?.Invoke(option);

            CharacterDialogueController controller = characterControllers[currentTargetCharacter.Id];
            controller.RecordChoice(option);

            int affectionChange = CalculateAffectionChange(option);
            controller.UpdateAffection(affectionChange);
            OnAffectionChanged?.Invoke(currentTargetCharacter, controller.CurrentAffection);

            float emotionChange = emotionCalculator.CalculateEmotionChange(option, controller);
            controller.UpdateEmotion(emotionChange);
            OnEmotionChanged?.Invoke(currentTargetCharacter, controller.CurrentEmotion);

            string response = await GenerateCharacterResponse(option);
            controller.AddResponse(response);

            OnCharacterResponse?.Invoke(currentTargetCharacter, response);

            UpdateDialogueOptions();
            await CheckForSpecialEvents();

            return response;
        }

        /// <summary>
        /// 生成角色响应
        /// </summary>
        /// <param name="option">玩家选择的选项</param>
        /// <returns>响应文本</returns>
        private async Task<string> GenerateCharacterResponse(DialogueOption option)
        {
            if (useAIEnhancement && responseGenerator != null)
            {
                return await responseGenerator.GenerateResponse(
                    currentTargetCharacter,
                    option,
                    characterControllers[currentTargetCharacter.Id]
                );
            }

            return GetFallbackResponse(option);
        }

        /// <summary>
        /// 获取备用响应
        /// </summary>
        /// <param name="option">选项</param>
        /// <returns>响应文本</returns>
        private string GetFallbackResponse(DialogueOption option)
        {
            if (option.AffectionChange > 0)
            {
                return "听起来很有趣呢~";
            }
            else if (option.AffectionChange < 0)
            {
                return "嗯...是吗...";
            }
            return "原来是这样啊";
        }

        /// <summary>
        /// 计算好感度变化
        /// </summary>
        /// <param name="option">选项</param>
        /// <returns>好感度变化值</returns>
        private int CalculateAffectionChange(DialogueOption option)
        {
            int baseChange = option.AffectionChange;

            float strategyMultiplier = currentStrategy?.GetAffectionMultiplier() ?? 1.0f;

            float emotionBonus = emotionCalculator.CalculateEmotionBonus(
                characterControllers[currentTargetCharacter.Id].CurrentEmotion,
                option
            );

            return Mathf.RoundToInt(baseChange * strategyMultiplier * (1 + emotionBonus));
        }

        #endregion

        #region 选项管理

        /// <summary>
        /// 更新对话选项
        /// </summary>
        private void UpdateDialogueOptions()
        {
            currentOptions.Clear();

            if (currentTargetCharacter == null) return;

            CharacterDialogueController controller = characterControllers[currentTargetCharacter.Id];
            currentOptions = GenerateDialogueOptions(controller);
        }

        /// <summary>
        /// 生成对话选项
        /// </summary>
        /// <param name="controller">对话控制器</param>
        /// <returns>选项列表</returns>
        private List<DialogueOption> GenerateDialogueOptions(CharacterDialogueController controller)
        {
            List<DialogueOption> options = new List<DialogueOption>();

            options.Add(CreatePoliteOption());
            options.Add(CreateCasualOption());
            options.Add(CreateFlirtyOption());

            if (controller.CurrentAffection >= Constants.HEARTBEAT_THRESHOLD)
            {
                options.Add(CreateHeartOption());
            }

            if (controller.HasSpecialTopic())
            {
                options.Add(CreateSpecialTopicOption());
            }

            return options;
        }

        /// <summary>
        /// 创建礼貌选项
        /// </summary>
        /// <returns>对话选项</returns>
        private DialogueOption CreatePoliteOption()
        {
            return new DialogueOption("polite_1", "你好，很高兴认识你！", "node_polite_1")
            {
                AffectionChange = 3,
                EmotionChange = 2f,
                OptionType = DialogueOptionType.Normal
            };
        }

        /// <summary>
        /// 创建轻松选项
        /// </summary>
        /// <returns>对话选项</returns>
        private DialogueOption CreateCasualOption()
        {
            return new DialogueOption("casual_1", "嘿，今天怎么样？", "node_casual_1")
            {
                AffectionChange = 2,
                EmotionChange = 3f,
                OptionType = DialogueOptionType.Normal
            };
        }

        /// <summary>
        /// 创建暧昧选项
        /// </summary>
        /// <returns>对话选项</returns>
        private DialogueOption CreateFlirtyOption()
        {
            return new DialogueOption("flirty_1", "你今天看起来很特别", "node_flirty_1")
            {
                AffectionChange = 5,
                EmotionChange = 8f,
                OptionType = DialogueOptionType.Heart,
                IsHeartChoice = true
            };
        }

        /// <summary>
        /// 创建心动选项
        /// </summary>
        /// <returns>对话选项</returns>
        private DialogueOption CreateHeartOption()
        {
            return new DialogueOption("heart_1", "其实...我对你很有好感", "node_heart_1")
            {
                AffectionChange = 15,
                EmotionChange = 20f,
                OptionType = DialogueOptionType.Heart,
                IsHeartChoice = true
            };
        }

        /// <summary>
        /// 创建特殊话题选项
        /// </summary>
        /// <returns>对话选项</returns>
        private DialogueOption CreateSpecialTopicOption()
        {
            return new DialogueOption("special_1", "我听说你喜欢...", "node_special_1")
            {
                AffectionChange = 8,
                EmotionChange = 10f,
                OptionType = DialogueOptionType.Special
            };
        }

        #endregion

        #region 策略管理

        /// <summary>
        /// 切换对话策略
        /// </summary>
        /// <param name="strategyType">策略类型</param>
        public void SwitchStrategy(DialogueStrategyType strategyType)
        {
            currentStrategy = strategyManager.GetStrategy(strategyType);
            OnStrategyChanged?.Invoke(strategyType.ToString());
        }

        /// <summary>
        /// 获取当前策略类型
        /// </summary>
        /// <returns>策略类型</returns>
        public DialogueStrategyType GetCurrentStrategyType()
        {
            return currentStrategy?.StrategyType ?? DialogueStrategyType.Casual;
        }

        #endregion

        #region 特殊事件

        /// <summary>
        /// 检查特殊事件
        /// </summary>
        private async Task CheckForSpecialEvents()
        {
            if (currentTargetCharacter == null) return;

            CharacterDialogueController controller = characterControllers[currentTargetCharacter.Id];

            if (controller.CurrentAffection >= Constants.CONFESSION_SUCCESS_THRESHOLD)
            {
                await TriggerConfessionOpportunity();
            }
            else if (controller.HasRecentPositiveInteraction())
            {
                await TriggerHeartEvent();
            }
        }

        /// <summary>
        /// 触发告白机会
        /// </summary>
        private async Task TriggerConfessionOpportunity()
        {
            if (currentStrategy is RomanticStrategy) return;

            currentOptions.Add(new DialogueOption("confession_1", "我有些话想对你说...", "node_confession")
            {
                AffectionChange = 0,
                EmotionChange = 15f,
                OptionType = DialogueOptionType.Special,
                IsHeartChoice = true
            });

            await Task.Delay(1);
        }

        /// <summary>
        /// 触发心动事件
        /// </summary>
        private async Task TriggerHeartEvent()
        {
            currentOptions.Add(new DialogueOption("heart_moment_1", "（心动时刻 - 观察反应）", "node_heart_moment")
            {
                AffectionChange = 5,
                EmotionChange = 12f,
                OptionType = DialogueOptionType.Special,
                IsHeartChoice = true
            });

            await Task.Delay(1);
        }

        #endregion

        #region 结束对话

        /// <summary>
        /// 结束当前对话
        /// </summary>
        public void EndDialogue()
        {
            if (currentTargetCharacter != null)
            {
                OnDialogueEnded?.Invoke(currentTargetCharacter);
            }

            currentTargetCharacter = null;
            currentOptions.Clear();
        }

        /// <summary>
        /// 获取对话总结
        /// </summary>
        /// <returns>总结文本</returns>
        public string GetDialogueSummary()
        {
            if (currentTargetCharacter == null) return "";

            CharacterDialogueController controller = characterControllers[currentTargetCharacter.Id];
            return controller.GetSummary();
        }

        #endregion

        #region Unity特定

        private static class Mathf
        {
            public static int RoundToInt(float f)
            {
                return (int)Math.Round(f);
            }
        }

        #endregion
    }
}
