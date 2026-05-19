using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue.Context
{
    /// <summary>
    /// 长期记忆
    /// 存储重要的、持久的上下文信息
    /// </summary>
    public class LongTermMemory
    {
        /// <summary>
        /// 记忆字典
        /// </summary>
        private Dictionary<string, LongTermMemoryEntry> memories;

        /// <summary>
        /// 记忆分类索引
        /// </summary>
        private Dictionary<string, List<string>> categoryIndex;

        /// <summary>
        /// 重要性阈值
        /// </summary>
        private const int IMPORTANCE_THRESHOLD = 5;

        /// <summary>
        /// 最大记忆数
        /// </summary>
        private int maxMemories;

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LongTermMemory()
        {
            maxMemories = 200;
            memories = new Dictionary<string, LongTermMemoryEntry>();
            categoryIndex = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxSize">最大记忆数</param>
        public LongTermMemory(int maxSize)
        {
            maxMemories = maxSize;
            memories = new Dictionary<string, LongTermMemoryEntry>();
            categoryIndex = new Dictionary<string, List<string>>();
        }

        #endregion

        #region 存储操作

        /// <summary>
        /// 存储记忆
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="importance">重要性（1-10）</param>
        /// <param name="category">分类</param>
        public void Store(string key, object value, int importance = 5, string category = "general")
        {
            if (memories.Count >= maxMemories && !memories.ContainsKey(key))
            {
                RemoveLowestImportance();
            }

            LongTermMemoryEntry entry = new LongTermMemoryEntry
            {
                Key = key,
                Value = value,
                Importance = System.Math.Clamp(importance, 1, 10),
                Category = category,
                CreatedAt = DateTime.Now,
                LastAccessed = DateTime.Now,
                AccessCount = 0
            };

            memories[key] = entry;

            if (!categoryIndex.ContainsKey(category))
            {
                categoryIndex[category] = new List<string>();
            }

            if (!categoryIndex[category].Contains(key))
            {
                categoryIndex[category].Add(key);
            }
        }

        /// <summary>
        /// 存储记忆（简化版本）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Store(string key, object value)
        {
            Store(key, value, 5, "general");
        }

        /// <summary>
        /// 移除最低重要性的记忆
        /// </summary>
        private void RemoveLowestImportance()
        {
            string lowestKey = null;
            int lowestImportance = int.MaxValue;

            foreach (var kvp in memories)
            {
                if (kvp.Value.Importance < lowestImportance)
                {
                    lowestImportance = kvp.Value.Importance;
                    lowestKey = kvp.Key;
                }
            }

            if (lowestKey != null)
            {
                Remove(lowestKey);
            }
        }

        /// <summary>
        /// 读取记忆
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>值或默认值</returns>
        public T Retrieve<T>(string key)
        {
            if (!memories.ContainsKey(key))
            {
                return default;
            }

            LongTermMemoryEntry entry = memories[key];
            entry.LastAccessed = DateTime.Now;
            entry.AccessCount++;

            if (entry.Value is T typedValue)
            {
                return typedValue;
            }

            return default;
        }

        /// <summary>
        /// 检查是否包含键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否包含</returns>
        public bool Contains(string key)
        {
            return memories.ContainsKey(key);
        }

        /// <summary>
        /// 移除记忆
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功移除</returns>
        public bool Remove(string key)
        {
            if (memories.TryGetValue(key, out LongTermMemoryEntry entry))
            {
                if (categoryIndex.ContainsKey(entry.Category))
                {
                    categoryIndex[entry.Category].Remove(key);
                }
            }

            return memories.Remove(key);
        }

        #endregion

        #region 查询操作

        /// <summary>
        /// 获取所有键
        /// </summary>
        /// <returns>键列表</returns>
        public List<string> GetAllKeys()
        {
            return new List<string>(memories.Keys);
        }

        /// <summary>
        /// 获取指定分类的记忆
        /// </summary>
        /// <param name="category">分类</param>
        /// <returns>记忆列表</returns>
        public List<LongTermMemoryEntry> GetByCategory(string category)
        {
            if (!categoryIndex.ContainsKey(category))
            {
                return new List<LongTermMemoryEntry>();
            }

            List<LongTermMemoryEntry> results = new List<LongTermMemoryEntry>();
            foreach (string key in categoryIndex[category])
            {
                if (memories.ContainsKey(key))
                {
                    results.Add(memories[key]);
                }
            }

            return results;
        }

        /// <summary>
        /// 获取高重要性记忆
        /// </summary>
        /// <param name="threshold">阈值</param>
        /// <returns>记忆列表</returns>
        public List<LongTermMemoryEntry> GetHighImportance(int threshold = 7)
        {
            List<LongTermMemoryEntry> results = new List<LongTermMemoryEntry>();

            foreach (var entry in memories.Values)
            {
                if (entry.Importance >= threshold)
                {
                    results.Add(entry);
                }
            }

            results.Sort((a, b) => b.Importance.CompareTo(a.Importance));

            return results;
        }

        /// <summary>
        /// 获取最近访问的记忆
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>记忆列表</returns>
        public List<LongTermMemoryEntry> GetRecentlyAccessed(int count = 10)
        {
            List<LongTermMemoryEntry> all = new List<LongTermMemoryEntry>(memories.Values);
            all.Sort((a, b) => b.LastAccessed.CompareTo(a.LastAccessed));

            return all.GetRange(0, System.Math.Min(count, all.Count));
        }

        /// <summary>
        /// 获取记忆数量
        /// </summary>
        /// <returns>数量</returns>
        public int Count()
        {
            return memories.Count;
        }

        #endregion

        #region 统计分析

        /// <summary>
        /// 获取分类统计
        /// </summary>
        /// <returns>分类统计字典</returns>
        public Dictionary<string, int> GetCategoryStatistics()
        {
            Dictionary<string, int> stats = new Dictionary<string, int>();

            foreach (var kvp in categoryIndex)
            {
                stats[kvp.Key] = kvp.Value.Count;
            }

            return stats;
        }

        /// <summary>
        /// 获取平均重要性
        /// </summary>
        /// <returns>平均重要性</returns>
        public float GetAverageImportance()
        {
            if (memories.Count == 0) return 0f;

            int sum = 0;
            foreach (var entry in memories.Values)
            {
                sum += entry.Importance;
            }

            return (float)sum / memories.Count;
        }

        /// <summary>
        /// 获取最常访问的记忆
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>记忆列表</returns>
        public List<LongTermMemoryEntry> GetMostAccessed(int count = 5)
        {
            List<LongTermMemoryEntry> all = new List<LongTermMemoryEntry>(memories.Values);
            all.Sort((a, b) => b.AccessCount.CompareTo(a.AccessCount));

            return all.GetRange(0, System.Math.Min(count, all.Count));
        }

        #endregion

        #region 维护操作

        /// <summary>
        /// 清除所有记忆
        /// </summary>
        public void Clear()
        {
            memories.Clear();
            categoryIndex.Clear();
        }

        /// <summary>
        /// 清除指定分类的记忆
        /// </summary>
        /// <param name="category">分类</param>
        public void ClearCategory(string category)
        {
            if (!categoryIndex.ContainsKey(category)) return;

            List<string> keysToRemove = new List<string>(categoryIndex[category]);
            foreach (string key in keysToRemove)
            {
                memories.Remove(key);
            }

            categoryIndex.Remove(category);
        }

        /// <summary>
        /// 增加记忆重要性
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="amount">增加量</param>
        public void IncreaseImportance(string key, int amount = 1)
        {
            if (memories.ContainsKey(key))
            {
                memories[key].Importance = System.Math.Clamp(
                    memories[key].Importance + amount,
                    1,
                    10
                );
            }
        }

        /// <summary>
        /// 更新记忆分类
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="newCategory">新分类</param>
        public void UpdateCategory(string key, string newCategory)
        {
            if (!memories.ContainsKey(key)) return;

            LongTermMemoryEntry entry = memories[key];
            string oldCategory = entry.Category;

            if (oldCategory != newCategory)
            {
                if (categoryIndex.ContainsKey(oldCategory))
                {
                    categoryIndex[oldCategory].Remove(key);
                }

                entry.Category = newCategory;

                if (!categoryIndex.ContainsKey(newCategory))
                {
                    categoryIndex[newCategory] = new List<string>();
                }

                if (!categoryIndex[newCategory].Contains(key))
                {
                    categoryIndex[newCategory].Add(key);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 长期记忆条目
    /// </summary>
    [Serializable]
    public class LongTermMemoryEntry
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key;

        /// <summary>
        /// 值
        /// </summary>
        public object Value;

        /// <summary>
        /// 重要性（1-10）
        /// </summary>
        public int Importance;

        /// <summary>
        /// 分类
        /// </summary>
        public string Category;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt;

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime LastAccessed;

        /// <summary>
        /// 访问次数
        /// </summary>
        public int AccessCount;

        /// <summary>
        /// 标签列表
        /// </summary>
        public List<string> Tags;
    }
}
