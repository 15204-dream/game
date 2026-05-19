using System;
using System.Collections.Generic;
using System.Linq;

namespace 糟糕是心动鸭.Dialogue.Context
{
    /// <summary>
    /// 对话记忆
    /// 存储和管理对话历史记录
    /// </summary>
    public class ConversationMemory
    {
        /// <summary>
        /// 对话记录列表
        /// </summary>
        private List<DialogueRecord> records;

        /// <summary>
        /// 最大记录数
        /// </summary>
        private int maxRecords;

        /// <summary>
        /// 按发言者分组的记录
        /// </summary>
        private Dictionary<string, List<DialogueRecord>> recordsBySpeaker;

        /// <summary>
        /// 关键词索引
        /// </summary>
        private Dictionary<string, List<int>> keywordIndex;

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ConversationMemory()
        {
            maxRecords = 100;
            records = new List<DialogueRecord>();
            recordsBySpeaker = new Dictionary<string, List<DialogueRecord>>();
            keywordIndex = new Dictionary<string, List<int>>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxSize">最大记录数</param>
        public ConversationMemory(int maxSize)
        {
            maxRecords = maxSize;
            records = new List<DialogueRecord>();
            recordsBySpeaker = new Dictionary<string, List<DialogueRecord>>();
            keywordIndex = new Dictionary<string, List<int>>();
        }

        #endregion

        #region 记录管理

        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="speakerId">发言者ID</param>
        /// <param name="content">内容</param>
        public void AddRecord(string speakerId, string content)
        {
            DialogueRecord record = new DialogueRecord
            {
                SpeakerId = speakerId,
                Content = content,
                Timestamp = DateTime.Now
            };

            records.Add(record);

            if (!recordsBySpeaker.ContainsKey(speakerId))
            {
                recordsBySpeaker[speakerId] = new List<DialogueRecord>();
            }
            recordsBySpeaker[speakerId].Add(record);

            IndexKeywords(content, records.Count - 1);

            if (records.Count > maxRecords)
            {
                RemoveOldestRecord();
            }
        }

        /// <summary>
        /// 索引关键词
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="recordIndex">记录索引</param>
        private void IndexKeywords(string content, int recordIndex)
        {
            string[] words = content.Split(' ', '，', '。', '！', '？');

            foreach (string word in words)
            {
                if (word.Length >= 2)
                {
                    if (!keywordIndex.ContainsKey(word))
                    {
                        keywordIndex[word] = new List<int>();
                    }
                    keywordIndex[word].Add(recordIndex);
                }
            }
        }

        /// <summary>
        /// 移除最旧记录
        /// </summary>
        private void RemoveOldestRecord()
        {
            if (records.Count == 0) return;

            DialogueRecord oldest = records[0];
            records.RemoveAt(0);

            if (recordsBySpeaker.ContainsKey(oldest.SpeakerId))
            {
                recordsBySpeaker[oldest.SpeakerId].Remove(oldest);
            }
        }

        /// <summary>
        /// 获取最近的记录
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>记录列表</returns>
        public List<DialogueRecord> GetRecentRecords(int count)
        {
            if (count <= 0) return new List<DialogueRecord>();

            int start = System.Math.Max(0, records.Count - count);
            int length = System.Math.Min(count, records.Count - start);

            return records.GetRange(start, length);
        }

        /// <summary>
        /// 获取指定发言者的记录
        /// </summary>
        /// <param name="speakerId">发言者ID</param>
        /// <returns>记录列表</returns>
        public List<DialogueRecord> GetRecordsBySpeaker(string speakerId)
        {
            if (recordsBySpeaker.ContainsKey(speakerId))
            {
                return new List<DialogueRecord>(recordsBySpeaker[speakerId]);
            }
            return new List<DialogueRecord>();
        }

        /// <summary>
        /// 搜索记录
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>匹配的记录</returns>
        public List<DialogueRecord> Search(string keyword)
        {
            List<DialogueRecord> results = new List<DialogueRecord>();

            if (keywordIndex.ContainsKey(keyword))
            {
                foreach (int index in keywordIndex[keyword])
                {
                    if (index < records.Count)
                    {
                        results.Add(records[index]);
                    }
                }
            }

            foreach (var record in records)
            {
                if (record.Content.Contains(keyword) &&
                    !results.Contains(record))
                {
                    results.Add(record);
                }
            }

            return results;
        }

        #endregion

        #region 统计信息

        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <returns>记录数</returns>
        public int GetTotalRecords()
        {
            return records.Count;
        }

        /// <summary>
        /// 获取发言者数量
        /// </summary>
        /// <returns>发言者数</returns>
        public int GetSpeakerCount()
        {
            return recordsBySpeaker.Count;
        }

        /// <summary>
        /// 获取发言统计
        /// </summary>
        /// <returns>发言统计字典</returns>
        public Dictionary<string, int> GetSpeakerStatistics()
        {
            Dictionary<string, int> stats = new Dictionary<string, int>();

            foreach (var kvp in recordsBySpeaker)
            {
                stats[kvp.Key] = kvp.Value.Count;
            }

            return stats;
        }

        /// <summary>
        /// 获取最活跃的发言者
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>发言者ID列表</returns>
        public List<string> GetMostActiveSpeakers(int count = 3)
        {
            return recordsBySpeaker
                .OrderByDescending(kvp => kvp.Value.Count)
                .Take(count)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        #endregion

        #region 摘要和导出

        /// <summary>
        /// 获取摘要
        /// </summary>
        /// <returns>摘要文本</returns>
        public string GetSummary()
        {
            if (records.Count == 0)
            {
                return "暂无对话记录";
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine($"总对话数: {records.Count}");
            sb.AppendLine($"发言者数: {recordsBySpeaker.Count}");
            sb.AppendLine();
            sb.AppendLine("最近对话:");

            var recentRecords = GetRecentRecords(5);
            foreach (var record in recentRecords)
            {
                sb.AppendLine($"[{record.Timestamp:HH:mm}] {record.SpeakerId}: {record.Content}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 导出所有记录
        /// </summary>
        /// <returns>记录列表</returns>
        public List<DialogueRecord> ExportAll()
        {
            return new List<DialogueRecord>(records);
        }

        /// <summary>
        /// 清除所有记录
        /// </summary>
        public void Clear()
        {
            records.Clear();
            recordsBySpeaker.Clear();
            keywordIndex.Clear();
        }

        #endregion

        #region 高级查询

        /// <summary>
        /// 获取时间范围内的记录
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>记录列表</returns>
        public List<DialogueRecord> GetRecordsInTimeRange(DateTime startTime, DateTime endTime)
        {
            return records
                .Where(r => r.Timestamp >= startTime && r.Timestamp <= endTime)
                .ToList();
        }

        /// <summary>
        /// 获取包含所有关键词的记录
        /// </summary>
        /// <param name="keywords">关键词列表</param>
        /// <returns>记录列表</returns>
        public List<DialogueRecord> GetRecordsWithAllKeywords(List<string> keywords)
        {
            return records
                .Where(r => keywords.All(k => r.Content.Contains(k)))
                .ToList();
        }

        /// <summary>
        /// 获取包含任一关键词的记录
        /// </summary>
        /// <param name="keywords">关键词列表</param>
        /// <returns>记录列表</returns>
        public List<DialogueRecord> GetRecordsWithAnyKeyword(List<string> keywords)
        {
            return records
                .Where(r => keywords.Any(k => r.Content.Contains(k)))
                .ToList();
        }

        #endregion
    }

    /// <summary>
    /// 对话记录
    /// </summary>
    [Serializable]
    public class DialogueRecord
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
        /// 发言者名称（可选）
        /// </summary>
        public string SpeakerName;
    }
}
