using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue.Context
{
    /// <summary>
    /// 短期记忆
    /// 存储临时性的上下文信息，具有过期机制
    /// </summary>
    public class ShortTermMemory
    {
        /// <summary>
        /// 记忆条目字典
        /// </summary>
        private Dictionary<string, MemoryEntry> entries;

        /// <summary>
        /// 最大条目数
        /// </summary>
        private int maxEntries;

        /// <summary>
        /// 过期时间（分钟）
        /// </summary>
        private int expireMinutes;

        /// <summary>
        /// 访问记录（用于LRU）
        /// </summary>
        private Dictionary<string, DateTime> lastAccessTimes;

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ShortTermMemory()
        {
            maxEntries = 20;
            expireMinutes = 30;
            entries = new Dictionary<string, MemoryEntry>();
            lastAccessTimes = new Dictionary<string, DateTime>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxSize">最大条目数</param>
        /// <param name="expire">过期时间（分钟）</param>
        public ShortTermMemory(int maxSize, int expire)
        {
            maxEntries = maxSize;
            expireMinutes = expire;
            entries = new Dictionary<string, MemoryEntry>();
            lastAccessTimes = new Dictionary<string, DateTime>();
        }

        #endregion

        #region 存储操作

        /// <summary>
        /// 存储记忆
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Store(string key, object value)
        {
            if (entries.Count >= maxEntries && !entries.ContainsKey(key))
            {
                RemoveLeastRecentlyUsed();
            }

            MemoryEntry entry = new MemoryEntry
            {
                Key = key,
                Value = value,
                CreatedAt = DateTime.Now,
                LastAccessed = DateTime.Now
            };

            entries[key] = entry;
            lastAccessTimes[key] = DateTime.Now;
        }

        /// <summary>
        /// 移除最少使用的条目
        /// </summary>
        private void RemoveLeastRecentlyUsed()
        {
            if (lastAccessTimes.Count == 0) return;

            string lruKey = null;
            DateTime oldestTime = DateTime.MaxValue;

            foreach (var kvp in lastAccessTimes)
            {
                if (kvp.Value < oldestTime)
                {
                    oldestTime = kvp.Value;
                    lruKey = kvp.Key;
                }
            }

            if (lruKey != null)
            {
                entries.Remove(lruKey);
                lastAccessTimes.Remove(lruKey);
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
            if (!entries.ContainsKey(key))
            {
                return default;
            }

            MemoryEntry entry = entries[key];

            if (IsExpired(entry))
            {
                Remove(key);
                return default;
            }

            entry.LastAccessed = DateTime.Now;
            lastAccessTimes[key] = DateTime.Now;

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
            if (!entries.ContainsKey(key))
            {
                return false;
            }

            MemoryEntry entry = entries[key];
            if (IsExpired(entry))
            {
                Remove(key);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 移除记忆
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功移除</returns>
        public bool Remove(string key)
        {
            lastAccessTimes.Remove(key);
            return entries.Remove(key);
        }

        /// <summary>
        /// 检查是否过期
        /// </summary>
        /// <param name="entry">记忆条目</param>
        /// <returns>是否过期</returns>
        private bool IsExpired(MemoryEntry entry)
        {
            TimeSpan age = DateTime.Now - entry.CreatedAt;
            return age.TotalMinutes >= expireMinutes;
        }

        #endregion

        #region 清理操作

        /// <summary>
        /// 清理过期记忆
        /// </summary>
        public void CleanExpired()
        {
            List<string> keysToRemove = new List<string>();

            foreach (var kvp in entries)
            {
                if (IsExpired(kvp.Value))
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// 清除所有记忆
        /// </summary>
        public void Clear()
        {
            entries.Clear();
            lastAccessTimes.Clear();
        }

        #endregion

        #region 查询操作

        /// <summary>
        /// 获取所有键
        /// </summary>
        /// <returns>键列表</returns>
        public List<string> GetAllKeys()
        {
            CleanExpired();
            return new List<string>(entries.Keys);
        }

        /// <summary>
        /// 获取所有键的字符串
        /// </summary>
        /// <returns>格式化的键字符串</returns>
        public string GetAllKeysString()
        {
            var keys = GetAllKeys();
            return keys.Count > 0 ? string.Join(", ", keys) : "无";
        }

        /// <summary>
        /// 获取记忆数量
        /// </summary>
        /// <returns>数量</returns>
        public int Count()
        {
            CleanExpired();
            return entries.Count;
        }

        /// <summary>
        /// 获取内存占用（估计）
        /// </summary>
        /// <returns>估计的字节数</returns>
        public int GetEstimatedMemoryUsage()
        {
            int total = 0;

            foreach (var entry in entries.Values)
            {
                if (entry.Value is string str)
                {
                    total += str.Length * 2;
                }
                else if (entry.Value != null)
                {
                    total += 100;
                }
            }

            return total;
        }

        #endregion

        #region 特殊操作

        /// <summary>
        /// 更新过期时间
        /// </summary>
        /// <param name="key">键</param>
        public void Refresh(string key)
        {
            if (entries.ContainsKey(key))
            {
                entries[key].CreatedAt = DateTime.Now;
                entries[key].LastAccessed = DateTime.Now;
                lastAccessTimes[key] = DateTime.Now;
            }
        }

        /// <summary>
        /// 获取记忆的剩余寿命（分钟）
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>剩余寿命，-1表示不存在或已过期</returns>
        public double GetRemainingLife(string key)
        {
            if (!entries.ContainsKey(key))
            {
                return -1;
            }

            MemoryEntry entry = entries[key];
            TimeSpan age = DateTime.Now - entry.CreatedAt;
            double remaining = expireMinutes - age.TotalMinutes;

            return remaining > 0 ? remaining : -1;
        }

        /// <summary>
        /// 获取最老的记忆
        /// </summary>
        /// <returns>最老记忆的键，null表示无记忆</returns>
        public string GetOldestKey()
        {
            if (entries.Count == 0) return null;

            string oldestKey = null;
            DateTime oldestTime = DateTime.MaxValue;

            foreach (var kvp in entries)
            {
                if (kvp.Value.CreatedAt < oldestTime)
                {
                    oldestTime = kvp.Value.CreatedAt;
                    oldestKey = kvp.Key;
                }
            }

            return oldestKey;
        }

        #endregion
    }

    /// <summary>
    /// 记忆条目
    /// </summary>
    [Serializable]
    public class MemoryEntry
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
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt;

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime LastAccessed;
    }
}
