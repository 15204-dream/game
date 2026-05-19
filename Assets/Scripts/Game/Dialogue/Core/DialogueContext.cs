using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace 糟糕是心动鸭.Dialogue
{
    /// <summary>
    /// 对话上下文数据类
    /// 存储对话过程中的上下文信息
    /// </summary>
    [Serializable]
    public class DialogueContext
    {
        /// <summary>
        /// 上下文ID
        /// </summary>
        private string contextId;

        /// <summary>
        /// 当前对话树ID
        /// </summary>
        private string currentTreeId;

        /// <summary>
        /// 当前节点ID
        /// </summary>
        private string currentNodeId;

        /// <summary>
        /// 当前角色ID
        /// </summary>
        private string currentCharacterId;

        /// <summary>
        /// 玩家ID
        /// </summary>
        private string playerId;

        /// <summary>
        /// 上下文标签
        /// </summary>
        private HashSet<string> contextTags;

        /// <summary>
        /// 对话历史记录
        /// </summary>
        private List<DialogueHistoryEntry> dialogueHistory;

        /// <summary>
        /// 当前好感度
        /// </summary>
        private int currentAffection;

        /// <summary>
        /// 当前情感值
        /// </summary>
        private float currentEmotion;

        /// <summary>
        /// 当前场景ID
        /// </summary>
        private string currentSceneId;

        /// <summary>
        /// 当前回合数
        /// </summary>
        private int currentRound;

        /// <summary>
        /// 对话时间戳
        /// </summary>
        private DateTime timestamp;

        /// <summary>
        /// 是否为关键对话
        /// </summary>
        private bool isKeyDialogue;

        /// <summary>
        /// 自定义数据
        /// </summary>
        private Dictionary<string, string> customData;

        #region 属性访问器

        public string ContextId
        {
            get => contextId;
            set => contextId = value;
        }

        public string CurrentTreeId
        {
            get => currentTreeId;
            set => currentTreeId = value;
        }

        public string CurrentNodeId
        {
            get => currentNodeId;
            set => currentNodeId = value;
        }

        public string CurrentCharacterId
        {
            get => currentCharacterId;
            set => currentCharacterId = value;
        }

        public string PlayerId
        {
            get => playerId;
            set => playerId = value;
        }

        public HashSet<string> ContextTags
        {
            get => contextTags;
            set => contextTags = value;
        }

        public List<DialogueHistoryEntry> DialogueHistory
        {
            get => dialogueHistory;
            set => dialogueHistory = value;
        }

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

        public string CurrentSceneId
        {
            get => currentSceneId;
            set => currentSceneId = value;
        }

        public int CurrentRound
        {
            get => currentRound;
            set => currentRound = Mathf.Max(0, value);
        }

        public DateTime Timestamp
        {
            get => timestamp;
            set => timestamp = value;
        }

        public bool IsKeyDialogue
        {
            get => isKeyDialogue;
            set => isKeyDialogue = value;
        }

        public Dictionary<string, string> CustomData
        {
            get => customData;
            set => customData = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DialogueContext()
        {
            contextId = Guid.NewGuid().ToString();
            currentTreeId = "";
            currentNodeId = "";
            currentCharacterId = "";
            playerId = "";
            contextTags = new HashSet<string>();
            dialogueHistory = new List<DialogueHistoryEntry>();
            currentAffection = Constants.INITIAL_AFFECTION;
            currentEmotion = 0f;
            currentSceneId = "";
            currentRound = 0;
            timestamp = DateTime.Now;
            isKeyDialogue = false;
            customData = new Dictionary<string, string>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="player">玩家ID</param>
        /// <param name="character">角色ID</param>
        public DialogueContext(string player, string character)
        {
            contextId = Guid.NewGuid().ToString();
            currentTreeId = "";
            currentNodeId = "";
            currentCharacterId = character;
            playerId = player;
            contextTags = new HashSet<string>();
            dialogueHistory = new List<DialogueHistoryEntry>();
            currentAffection = Constants.INITIAL_AFFECTION;
            currentEmotion = 0f;
            currentSceneId = "";
            currentRound = 0;
            timestamp = DateTime.Now;
            isKeyDialogue = false;
            customData = new Dictionary<string, string>();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 添加上下文标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void AddTag(string tag)
        {
            contextTags.Add(tag);
        }

        /// <summary>
        /// 移除上下文标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void RemoveTag(string tag)
        {
            contextTags.Remove(tag);
        }

        /// <summary>
        /// 检查是否包含标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>是否包含</returns>
        public bool HasTag(string tag)
        {
            return contextTags.Contains(tag);
        }

        /// <summary>
        /// 添加对话历史
        /// </summary>
        /// <param name="speakerId">发言者ID</param>
        /// <param name="content">内容</param>
        public void AddToHistory(string speakerId, string content)
        {
            DialogueHistoryEntry entry = new DialogueHistoryEntry
            {
                SpeakerId = speakerId,
                Content = content,
                Timestamp = DateTime.Now,
                Round = currentRound
            };
            dialogueHistory.Add(entry);
        }

        /// <summary>
        /// 获取对话历史
        /// </summary>
        /// <param name="maxEntries">最大条目数</param>
        /// <returns>历史记录</returns>
        public List<DialogueHistoryEntry> GetRecentHistory(int maxEntries = 10)
        {
            if (maxEntries <= 0) return new List<DialogueHistoryEntry>();
            int start = Mathf.Max(0, dialogueHistory.Count - maxEntries);
            return dialogueHistory.GetRange(start, dialogueHistory.Count - start);
        }

        /// <summary>
        /// 获取历史记录字符串
        /// </summary>
        /// <returns>格式化的历史记录</returns>
        public string GetHistoryString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var entry in dialogueHistory)
            {
                sb.AppendLine($"[{entry.SpeakerId}]: {entry.Content}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 设置自定义数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetCustomData(string key, string value)
        {
            customData[key] = value;
        }

        /// <summary>
        /// 获取自定义数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        public string GetCustomData(string key, string defaultValue = "")
        {
            return customData.ContainsKey(key) ? customData[key] : defaultValue;
        }

        /// <summary>
        /// 前进回合
        /// </summary>
        public void AdvanceRound()
        {
            currentRound++;
            timestamp = DateTime.Now;
        }

        /// <summary>
        /// 重置回合
        /// </summary>
        public void ResetRound()
        {
            currentRound = 0;
        }

        /// <summary>
        /// 增加好感度
        /// </summary>
        /// <param name="amount">增加量</param>
        public void IncreaseAffection(int amount)
        {
            currentAffection = Mathf.Clamp(currentAffection + amount, 0, 100);
        }

        /// <summary>
        /// 减少好感度
        /// </summary>
        /// <param name="amount">减少量</param>
        public void DecreaseAffection(int amount)
        {
            currentAffection = Mathf.Clamp(currentAffection - amount, 0, 100);
        }

        /// <summary>
        /// 增加情感值
        /// </summary>
        /// <param name="amount">增加量</param>
        public void IncreaseEmotion(float amount)
        {
            currentEmotion = Mathf.Clamp(currentEmotion + amount, 0f, 100f);
        }

        /// <summary>
        /// 减少情感值
        /// </summary>
        /// <param name="amount">减少量</param>
        public void DecreaseEmotion(float amount)
        {
            currentEmotion = Mathf.Clamp(currentEmotion - amount, 0f, 100f);
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        public void ClearHistory()
        {
            dialogueHistory.Clear();
        }

        /// <summary>
        /// 复制上下文
        /// </summary>
        /// <returns>新的上下文副本</returns>
        public DialogueContext Clone()
        {
            DialogueContext clone = new DialogueContext
            {
                contextId = Guid.NewGuid().ToString(),
                currentTreeId = this.currentTreeId,
                currentNodeId = this.currentNodeId,
                currentCharacterId = this.currentCharacterId,
                playerId = this.playerId,
                contextTags = new HashSet<string>(this.contextTags),
                dialogueHistory = new List<DialogueHistoryEntry>(this.dialogueHistory),
                currentAffection = this.currentAffection,
                currentEmotion = this.currentEmotion,
                currentSceneId = this.currentSceneId,
                currentRound = this.currentRound,
                timestamp = DateTime.Now,
                isKeyDialogue = this.isKeyDialogue,
                customData = new Dictionary<string, string>(this.customData)
            };
            return clone;
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

            public static int Max(int a, int b)
            {
                return System.Math.Max(a, b);
            }
        }

        #endregion
    }

    /// <summary>
    /// 对话历史记录条目
    /// </summary>
    [Serializable]
    public class DialogueHistoryEntry
    {
        /// <summary>
        /// 发言者ID
        /// </summary>
        public string SpeakerId;

        /// <summary>
        /// 发言内容
        /// </summary>
        public string Content;

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        /// 回合数
        /// </summary>
        public int Round;

        /// <summary>
        /// 发言者名称
        /// </summary>
        public string SpeakerName;

        /// <summary>
        /// 表情
        /// </summary>
        public string Emotion;
    }
}
