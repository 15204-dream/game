using System;
using System.Collections.Generic;
using System.Linq;

namespace 糟糕是心动鸭.Dialogue.GuestMode
{
    /// <summary>
    /// 角色对话控制器
    /// 管理单个角色的对话状态和行为
    /// </summary>
    public class CharacterDialogueController
    {
        /// <summary>
        /// 角色数据
        /// </summary>
        private CharacterData characterData;

        /// <summary>
        /// 当前好感度
        /// </summary>
        private int currentAffection;

        /// <summary>
        /// 当前情感值
        /// </summary>
        private float currentEmotion;

        /// <summary>
        /// 对话历史
        /// </summary>
        private List<DialogueExchange> dialogueHistory;

        /// <summary>
        /// 特殊话题列表
        /// </summary>
        private List<string> specialTopics;

        /// <summary>
        /// 已使用的话题
        /// </summary>
        private List<string> usedTopics;

        /// <summary>
        /// 对话轮次计数
        /// </summary>
        private int dialogueRound;

        /// <summary>
        /// 最后交互时间
        /// </summary>
        private DateTime lastInteractionTime;

        /// <summary>
        /// 正面交互计数
        /// </summary>
        private int positiveInteractionCount;

        /// <summary>
        /// 是否已解锁告白
        /// </summary>
        private bool confessionUnlocked;

        /// <summary>
        /// 心动时刻触发阈值
        /// </summary>
        private const int HEART_MOMENT_THRESHOLD = 3;

        #region 属性访问器

        public CharacterData CharacterData => characterData;
        public string CharacterId => characterData.Id;
        public string CharacterName => characterData.Name;
        public int CurrentAffection
        {
            get => currentAffection;
            set => currentAffection = Mathf.Clamp(value, 0, 100);
        }
        public float CurrentEmotion
        {
            get => currentEmotion;
            set => currentEmotion = Mathf.Clamp(value, 0f, 100f);
        }
        public List<DialogueExchange> DialogueHistory => dialogueHistory;
        public int DialogueRound => dialogueRound;
        public bool ConfessionUnlocked => confessionUnlocked;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">角色数据</param>
        public CharacterDialogueController(CharacterData data)
        {
            characterData = data;
            currentAffection = data.InitialAffection;
            currentEmotion = 0f;
            dialogueHistory = new List<DialogueExchange>();
            specialTopics = new List<string>();
            usedTopics = new List<string>();
            dialogueRound = 0;
            lastInteractionTime = DateTime.Now;
            positiveInteractionCount = 0;
            confessionUnlocked = false;

            InitializeSpecialTopics();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化特殊话题
        /// </summary>
        private void InitializeSpecialTopics()
        {
            specialTopics.Clear();

            foreach (var interest in characterData.InterestTags)
            {
                specialTopics.Add(interest);
            }

            foreach (var interest in characterData.PreferredInterests)
            {
                if (!specialTopics.Contains(interest))
                {
                    specialTopics.Add(interest);
                }
            }
        }

        /// <summary>
        /// 异步初始化
        /// </summary>
        public async System.Threading.Tasks.Task Initialize()
        {
            await System.Threading.Tasks.Task.Delay(1);
            lastInteractionTime = DateTime.Now;
        }

        #endregion

        #region 对话交互

        /// <summary>
        /// 记录玩家选择
        /// </summary>
        /// <param name="option">选择的选项</param>
        public void RecordChoice(DialogueOption option)
        {
            if (option == null) return;

            DialogueExchange exchange = new DialogueExchange
            {
                PlayerOption = option.Text,
                Round = dialogueRound,
                Timestamp = DateTime.Now
            };

            dialogueHistory.Add(exchange);
            dialogueRound++;
            lastInteractionTime = DateTime.Now;

            if (option.AffectionChange > 0)
            {
                positiveInteractionCount++;
            }

            if (!string.IsNullOrEmpty(option.RequiredItemId))
            {
                usedTopics.Add(option.RequiredItemId);
            }

            CheckConfessionUnlock();
        }

        /// <summary>
        /// 添加角色响应
        /// </summary>
        /// <param name="response">响应文本</param>
        public void AddResponse(string response)
        {
            if (dialogueHistory.Count > 0)
            {
                dialogueHistory[dialogueHistory.Count - 1].CharacterResponse = response;
            }
        }

        /// <summary>
        /// 更新好感度
        /// </summary>
        /// <param name="change">变化值</param>
        public void UpdateAffection(int change)
        {
            currentAffection = Mathf.Clamp(currentAffection + change, 0, 100);
        }

        /// <summary>
        /// 更新情感值
        /// </summary>
        /// <param name="change">变化值</param>
        public void UpdateEmotion(float change)
        {
            currentEmotion = Mathf.Clamp(currentEmotion + change, 0f, 100f);

            if (change < 0)
            {
                currentEmotion = Mathf.Max(currentEmotion, 0f);
            }
        }

        /// <summary>
        /// 检查告白是否解锁
        /// </summary>
        private void CheckConfessionUnlock()
        {
            if (currentAffection >= Constants.CONFESSION_SUCCESS_THRESHOLD && !confessionUnlocked)
            {
                confessionUnlocked = true;
            }
        }

        #endregion

        #region 话题管理

        /// <summary>
        /// 是否有特殊话题
        /// </summary>
        /// <returns>是否有</returns>
        public bool HasSpecialTopic()
        {
            return GetAvailableSpecialTopics().Count > 0;
        }

        /// <summary>
        /// 获取可用的特殊话题
        /// </summary>
        /// <returns>话题列表</returns>
        public List<string> GetAvailableSpecialTopics()
        {
            return specialTopics.Where(t => !usedTopics.Contains(t)).ToList();
        }

        /// <summary>
        /// 使用话题
        /// </summary>
        /// <param name="topic">话题</param>
        public void UseTopic(string topic)
        {
            if (!usedTopics.Contains(topic))
            {
                usedTopics.Add(topic);
            }
        }

        /// <summary>
        /// 获取下一个可用话题
        /// </summary>
        /// <returns>话题或null</returns>
        public string GetNextSpecialTopic()
        {
            var available = GetAvailableSpecialTopics();
            return available.Count > 0 ? available[0] : null;
        }

        #endregion

        #region 交互检测

        /// <summary>
        /// 是否有最近的正面交互
        /// </summary>
        /// <returns>是否有</returns>
        public bool HasRecentPositiveInteraction()
        {
            return positiveInteractionCount >= HEART_MOMENT_THRESHOLD;
        }

        /// <summary>
        /// 检查是否太久没有交互
        /// </summary>
        /// <param name="hours">小时数</param>
        /// <returns>是否太久</returns>
        public bool IsInteractionStale(int hours = 24)
        {
            TimeSpan timeSinceLastInteraction = DateTime.Now - lastInteractionTime;
            return timeSinceLastInteraction.TotalHours >= hours;
        }

        /// <summary>
        /// 重置交互计数
        /// </summary>
        public void ResetInteractionCount()
        {
            positiveInteractionCount = 0;
        }

        #endregion

        #region 状态查询

        /// <summary>
        /// 是否处于心动状态
        /// </summary>
        /// <returns>是否心动</returns>
        public bool IsInHeartState()
        {
            return currentEmotion >= 70f && currentAffection >= Constants.HEARTBEAT_THRESHOLD;
        }

        /// <summary>
        /// 是否准备好告白
        /// </summary>
        /// <returns>是否准备好</returns>
        public bool IsReadyForConfession()
        {
            return confessionUnlocked && IsInHeartState();
        }

        /// <summary>
        /// 获取关系阶段
        /// </summary>
        /// <returns>阶段描述</returns>
        public string GetRelationshipStage()
        {
            if (confessionUnlocked) return "已解锁告白";
            if (currentAffection >= Constants.HEARTBEAT_THRESHOLD) return "心动状态";
            if (currentAffection >= 40) return "好感状态";
            if (currentAffection >= 20) return "认识阶段";
            return "陌生阶段";
        }

        /// <summary>
        /// 获取好感度描述
        /// </summary>
        /// <returns>描述文本</returns>
        public string GetAffectionDescription()
        {
            if (currentAffection >= 80) return "非常喜欢";
            if (currentAffection >= 60) return "很有好感";
            if (currentAffection >= 40) return "有好感";
            if (currentAffection >= 20) return "普通朋友";
            return "刚认识";
        }

        #endregion

        #region 数据获取

        /// <summary>
        /// 获取对话总结
        /// </summary>
        /// <returns>总结文本</returns>
        public string GetSummary()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"角色: {CharacterName}");
            sb.AppendLine($"好感度: {currentAffection} ({GetAffectionDescription()})");
            sb.AppendLine($"情感值: {currentEmotion:F1}");
            sb.AppendLine($"对话轮次: {dialogueRound}");
            sb.AppendLine($"关系阶段: {GetRelationshipStage()}");
            sb.AppendLine($"告白解锁: {(confessionUnlocked ? "是" : "否")}");
            return sb.ToString();
        }

        /// <summary>
        /// 获取对话历史文本
        /// </summary>
        /// <returns>历史文本</returns>
        public string GetHistoryText()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var exchange in dialogueHistory)
            {
                sb.AppendLine($"[回合{exchange.Round}]");
                sb.AppendLine($"玩家: {exchange.PlayerOption}");
                if (!string.IsNullOrEmpty(exchange.CharacterResponse))
                {
                    sb.AppendLine($"角色: {exchange.CharacterResponse}");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <returns>统计数据字典</returns>
        public Dictionary<string, object> GetStatistics()
        {
            return new Dictionary<string, object>
            {
                { "totalExchanges", dialogueHistory.Count },
                { "positiveInteractions", positiveInteractionCount },
                { "topicsUsed", usedTopics.Count },
                { "timeSinceLastInteraction", (DateTime.Now - lastInteractionTime).TotalHours }
            };
        }

        #endregion

        #region Unity特定

        private static class Mathf
        {
            public static int Clamp(int value, int min, int max)
            {
                return System.Math.Max(min, System.Math.Min(max, value));
            }

            public static float Clamp(float value, float min, float max)
            {
                return System.Math.Max(min, System.Math.Min(max, value));
            }

            public static float Max(float a, float b)
            {
                return System.Math.Max(a, b);
            }
        }

        #endregion
    }

    /// <summary>
    /// 对话交换记录
    /// </summary>
    [Serializable]
    public class DialogueExchange
    {
        /// <summary>
        /// 玩家选项文本
        /// </summary>
        public string PlayerOption;

        /// <summary>
        /// 角色响应文本
        /// </summary>
        public string CharacterResponse;

        /// <summary>
        /// 对话回合
        /// </summary>
        public int Round;

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp;
    }
}
