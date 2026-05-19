using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 统计记录器 - 负责记录和保存游戏过程中的各种统计数据
    /// </summary>
    public class StatisticsRecorder : MonoBehaviour
    {
        private static StatisticsRecorder instance;
        public static StatisticsRecorder Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("StatisticsRecorder");
                    instance = go.AddComponent<StatisticsRecorder>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }
        
        private Dictionary<string, List<StatisticsRecord>> records;
        private List<GameEventRecord> gameEventRecords;
        private List<ConversationRecord> conversationRecords;
        private List<DateRecord> dateRecords;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Initialize()
        {
            records = new Dictionary<string, List<StatisticsRecord>>();
            gameEventRecords = new List<GameEventRecord>();
            conversationRecords = new List<ConversationRecord>();
            dateRecords = new List<DateRecord>();
        }
        
        /// <summary>
        /// 记录统计数据
        /// </summary>
        public void RecordStatistics(string category, float value)
        {
            if (!records.ContainsKey(category))
            {
                records[category] = new List<StatisticsRecord>();
            }
            
            StatisticsRecord record = new StatisticsRecord(value);
            records[category].Add(record);
            
            if (records[category].Count > 10000)
            {
                records[category].RemoveAt(0);
            }
        }
        
        /// <summary>
        /// 获取统计数据记录
        /// </summary>
        public List<StatisticsRecord> GetRecords(string category)
        {
            if (records.ContainsKey(category))
            {
                return new List<StatisticsRecord>(records[category]);
            }
            return new List<StatisticsRecord>();
        }
        
        /// <summary>
        /// 记录游戏事件
        /// </summary>
        public void RecordGameEvent(string eventId, string eventName, int day, float impact)
        {
            GameEventRecord record = new GameEventRecord
            {
                eventId = eventId,
                eventName = eventName,
                day = day,
                impact = impact,
                timestamp = DateTime.Now
            };
            gameEventRecords.Add(record);
        }
        
        /// <summary>
        /// 获取游戏事件记录
        /// </summary>
        public List<GameEventRecord> GetGameEventRecords()
        {
            return new List<GameEventRecord>(gameEventRecords);
        }
        
        /// <summary>
        /// 记录对话
        /// </summary>
        public void RecordConversation(string guestId, string guestName, string conversationType, int length)
        {
            ConversationRecord record = new ConversationRecord
            {
                guestId = guestId,
                guestName = guestName,
                conversationType = conversationType,
                length = length,
                timestamp = DateTime.Now
            };
            conversationRecords.Add(record);
        }
        
        /// <summary>
        /// 获取对话记录
        /// </summary>
        public List<ConversationRecord> GetConversationRecords()
        {
            return new List<ConversationRecord>(conversationRecords);
        }
        
        /// <summary>
        /// 获取指定嘉宾的对话记录
        /// </summary>
        public List<ConversationRecord> GetConversationRecordsByGuest(string guestId)
        {
            List<ConversationRecord> filtered = new List<ConversationRecord>();
            foreach (var record in conversationRecords)
            {
                if (record.guestId == guestId)
                {
                    filtered.Add(record);
                }
            }
            return filtered;
        }
        
        /// <summary>
        /// 记录约会
        /// </summary>
        public void RecordDate(string guestId, string guestName, string dateType, float rating)
        {
            DateRecord record = new DateRecord
            {
                guestId = guestId,
                guestName = guestName,
                dateType = dateType,
                rating = rating,
                timestamp = DateTime.Now
            };
            dateRecords.Add(record);
        }
        
        /// <summary>
        /// 获取约会记录
        /// </summary>
        public List<DateRecord> GetDateRecords()
        {
            return new List<DateRecord>(dateRecords);
        }
        
        /// <summary>
        /// 获取指定嘉宾的约会记录
        /// </summary>
        public List<DateRecord> GetDateRecordsByGuest(string guestId)
        {
            List<DateRecord> filtered = new List<DateRecord>();
            foreach (var record in dateRecords)
            {
                if (record.guestId == guestId)
                {
                    filtered.Add(record);
                }
            }
            return filtered;
        }
        
        /// <summary>
        /// 获取对话次数统计
        /// </summary>
        public int GetTotalConversationCount()
        {
            return conversationRecords.Count;
        }
        
        /// <summary>
        /// 获取约会次数统计
        /// </summary>
        public int GetTotalDateCount()
        {
            return dateRecords.Count;
        }
        
        /// <summary>
        /// 获取指定嘉宾的对话次数
        /// </summary>
        public int GetConversationCountByGuest(string guestId)
        {
            int count = 0;
            foreach (var record in conversationRecords)
            {
                if (record.guestId == guestId)
                {
                    count++;
                }
            }
            return count;
        }
        
        /// <summary>
        /// 获取指定嘉宾的约会次数
        /// </summary>
        public int GetDateCountByGuest(string guestId)
        {
            int count = 0;
            foreach (var record in dateRecords)
            {
                if (record.guestId == guestId)
                {
                    count++;
                }
            }
            return count;
        }
        
        /// <summary>
        /// 获取游戏事件统计
        /// </summary>
        public int GetGameEventCount()
        {
            return gameEventRecords.Count;
        }
        
        /// <summary>
        /// 清除所有记录
        /// </summary>
        public void ClearAllRecords()
        {
            records.Clear();
            gameEventRecords.Clear();
            conversationRecords.Clear();
            dateRecords.Clear();
        }
        
        /// <summary>
        /// 导出统计数据为JSON格式
        /// </summary>
        public string ExportToJson()
        {
            ExportData exportData = new ExportData
            {
                exportTime = DateTime.Now,
                records = records,
                gameEvents = gameEventRecords,
                conversations = conversationRecords,
                dates = dateRecords
            };
            
            return JsonUtility.ToJson(exportData, true);
        }
        
        /// <summary>
        /// 保存统计数据到文件
        /// </summary>
        public void SaveToFile(string filePath)
        {
            string json = ExportToJson();
            System.IO.File.WriteAllText(filePath, json);
        }
        
        /// <summary>
        /// 从文件加载统计数据
        /// </summary>
        public void LoadFromFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                ExportData exportData = JsonUtility.FromJson<ExportData>(json);
                
                if (exportData != null)
                {
                    records = exportData.records ?? new Dictionary<string, List<StatisticsRecord>>();
                    gameEventRecords = exportData.gameEvents ?? new List<GameEventRecord>();
                    conversationRecords = exportData.conversations ?? new List<ConversationRecord>();
                    dateRecords = exportData.dates ?? new List<DateRecord>();
                }
            }
        }
    }
    
    /// <summary>
    /// 游戏事件记录
    /// </summary>
    [Serializable]
    public class GameEventRecord
    {
        public string eventId;
        public string eventName;
        public int day;
        public float impact;
        public DateTime timestamp;
    }
    
    /// <summary>
    /// 对话记录
    /// </summary>
    [Serializable]
    public class ConversationRecord
    {
        public string guestId;
        public string guestName;
        public string conversationType;
        public int length;
        public DateTime timestamp;
    }
    
    /// <summary>
    /// 约会记录
    /// </summary>
    [Serializable]
    public class DateRecord
    {
        public string guestId;
        public string guestName;
        public string dateType;
        public float rating;
        public DateTime timestamp;
    }
    
    /// <summary>
    /// 导出数据结构
    /// </summary>
    [Serializable]
    public class ExportData
    {
        public DateTime exportTime;
        public Dictionary<string, List<StatisticsRecord>> records;
        public List<GameEventRecord> gameEvents;
        public List<ConversationRecord> conversations;
        public List<DateRecord> dates;
    }
}
