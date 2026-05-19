using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue.Context
{
    /// <summary>
    /// 上下文管理器
    /// 管理对话上下文、记忆系统和状态追踪
    /// </summary>
    public class ContextManager
    {
        /// <summary>
        /// 短期记忆
        /// </summary>
        private ShortTermMemory shortTermMemory;

        /// <summary>
        /// 长期记忆
        /// </summary>
        private LongTermMemory longTermMemory;

        /// <summary>
        /// 对话记忆
        /// </summary>
        private ConversationMemory conversationMemory;

        /// <summary>
        /// 当前上下文数据
        /// </summary>
        private Dictionary<string, object> contextData;

        /// <summary>
        /// 上下文标签
        /// </summary>
        private HashSet<string> contextTags;

        /// <summary>
        /// 最大短期记忆条数
        /// </summary>
        private const int MAX_SHORT_TERM_ITEMS = 20;

        /// <summary>
        /// 短期记忆过期时间（分钟）
        /// </summary>
        private const int SHORT_TERM_EXPIRE_MINUTES = 30;

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ContextManager()
        {
            shortTermMemory = new ShortTermMemory(MAX_SHORT_TERM_ITEMS, SHORT_TERM_EXPIRE_MINUTES);
            longTermMemory = new LongTermMemory();
            conversationMemory = new ConversationMemory();
            contextData = new Dictionary<string, object>();
            contextTags = new HashSet<string>();
        }

        #endregion

        #region 上下文数据管理

        /// <summary>
        /// 设置上下文数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetContextData(string key, object value)
        {
            contextData[key] = value;
            shortTermMemory.Store(key, value);
        }

        /// <summary>
        /// 获取上下文数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        public T GetContextData<T>(string key, T defaultValue = default)
        {
            if (contextData.TryGetValue(key, out object value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
            }

            T shortTermValue = shortTermMemory.Retrieve<T>(key);
            if (shortTermValue != null)
            {
                return shortTermValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// 检查是否有上下文数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public bool HasContextData(string key)
        {
            return contextData.ContainsKey(key) || shortTermMemory.Contains(key);
        }

        /// <summary>
        /// 移除上下文数据
        /// </summary>
        /// <param name="key">键</param>
        public void RemoveContextData(string key)
        {
            contextData.Remove(key);
            shortTermMemory.Remove(key);
        }

        /// <summary>
        /// 清除所有上下文数据
        /// </summary>
        public void ClearContextData()
        {
            contextData.Clear();
        }

        #endregion

        #region 标签管理

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
        /// 检查标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>是否存在</returns>
        public bool HasTag(string tag)
        {
            return contextTags.Contains(tag);
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <returns>标签集合</returns>
        public HashSet<string> GetAllTags()
        {
            return new HashSet<string>(contextTags);
        }

        /// <summary>
        /// 清除所有标签
        /// </summary>
        public void ClearTags()
        {
            contextTags.Clear();
        }

        #endregion

        #region 记忆系统

        /// <summary>
        /// 存储到短期记忆
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void StoreToShortTerm(string key, object value)
        {
            shortTermMemory.Store(key, value);
        }

        /// <summary>
        /// 从短期记忆读取
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public T RetrieveFromShortTerm<T>(string key)
        {
            return shortTermMemory.Retrieve<T>(key);
        }

        /// <summary>
        /// 存储到长期记忆
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void StoreToLongTerm(string key, object value)
        {
            longTermMemory.Store(key, value);
        }

        /// <summary>
        /// 从长期记忆读取
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public T RetrieveFromLongTerm<T>(string key)
        {
            return longTermMemory.Retrieve<T>(key);
        }

        /// <summary>
        /// 巩固记忆（从短期转入长期）
        /// </summary>
        /// <param name="key">键</param>
        public void ConsolidateMemory(string key)
        {
            var value = shortTermMemory.Retrieve<object>(key);
            if (value != null)
            {
                longTermMemory.Store(key, value);
                shortTermMemory.Remove(key);
            }
        }

        /// <summary>
        /// 清理过期记忆
        /// </summary>
        public void CleanExpiredMemories()
        {
            shortTermMemory.CleanExpired();
        }

        #endregion

        #region 对话记忆

        /// <summary>
        /// 添加对话记录
        /// </summary>
        /// <param name="speakerId">发言者ID</param>
        /// <param name="content">内容</param>
        public void AddDialogueRecord(string speakerId, string content)
        {
            conversationMemory.AddRecord(speakerId, content);
        }

        /// <summary>
        /// 获取对话历史
        /// </summary>
        /// <param name="maxRecords">最大记录数</param>
        /// <returns>对话记录列表</returns>
        public List<DialogueRecord> GetDialogueHistory(int maxRecords = 20)
        {
            return conversationMemory.GetRecentRecords(maxRecords);
        }

        /// <summary>
        /// 获取对话摘要
        /// </summary>
        /// <returns>摘要文本</returns>
        public string GetDialogueSummary()
        {
            return conversationMemory.GetSummary();
        }

        /// <summary>
        /// 搜索对话
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>匹配的记录</returns>
        public List<DialogueRecord> SearchDialogue(string keyword)
        {
            return conversationMemory.Search(keyword);
        }

        /// <summary>
        /// 清除对话记忆
        /// </summary>
        public void ClearDialogueMemory()
        {
            conversationMemory.Clear();
        }

        #endregion

        #region 上下文构建

        /// <summary>
        /// 构建完整上下文
        /// </summary>
        /// <returns>上下文字符串</returns>
        public string BuildFullContext()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("=== 当前上下文 ===");
            sb.AppendLine($"上下文标签: {string.Join(", ", contextTags)}");
            sb.AppendLine();

            if (contextData.Count > 0)
            {
                sb.AppendLine("上下文数据:");
                foreach (var kvp in contextData)
                {
                    sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
                }
                sb.AppendLine();
            }

            sb.AppendLine("=== 对话历史 ===");
            sb.AppendLine(conversationMemory.GetSummary());
            sb.AppendLine();

            sb.AppendLine("=== 短期记忆 ===");
            sb.AppendLine(shortTermMemory.GetAllKeysString());
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// 构建AI提示上下文
        /// </summary>
        /// <returns>AI提示字符串</returns>
        public string BuildAIContext()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (contextTags.Count > 0)
            {
                sb.AppendLine($"当前情境: {string.Join(", ", contextTags)}");
            }

            sb.AppendLine();
            sb.AppendLine("近期对话:");
            var recentDialogue = conversationMemory.GetRecentRecords(5);
            foreach (var record in recentDialogue)
            {
                sb.AppendLine($"[{record.SpeakerId}]: {record.Content}");
            }

            return sb.ToString();
        }

        #endregion

        #region 状态管理

        /// <summary>
        /// 重置上下文
        /// </summary>
        public void Reset()
        {
            contextData.Clear();
            contextTags.Clear();
            shortTermMemory.Clear();
            conversationMemory.Clear();
        }

        /// <summary>
        /// 保存状态
        /// </summary>
        /// <returns>状态快照</returns>
        public ContextSnapshot SaveState()
        {
            return new ContextSnapshot
            {
                ContextData = new Dictionary<string, object>(contextData),
                Tags = new HashSet<string>(contextTags),
                Timestamp = DateTime.Now
            };
        }

        /// <summary>
        /// 恢复状态
        /// </summary>
        /// <param name="snapshot">状态快照</param>
        public void RestoreState(ContextSnapshot snapshot)
        {
            if (snapshot == null) return;

            contextData = new Dictionary<string, object>(snapshot.ContextData);
            contextTags = new HashSet<string>(snapshot.Tags);
        }

        #endregion
    }

    /// <summary>
    /// 上下文快照
    /// </summary>
    [Serializable]
    public class ContextSnapshot
    {
        /// <summary>
        /// 上下文数据
        /// </summary>
        public Dictionary<string, object> ContextData;

        /// <summary>
        /// 标签集合
        /// </summary>
        public HashSet<string> Tags;

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp;
    }
}
