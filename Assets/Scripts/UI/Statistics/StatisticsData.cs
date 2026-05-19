using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 统计数据基类
    /// </summary>
    [Serializable]
    public class StatisticsData
    {
        /// <summary>
        /// 统计数据ID
        /// </summary>
        public string id;
        
        /// <summary>
        /// 统计数据名称
        /// </summary>
        public string name;
        
        /// <summary>
        /// 当前值
        /// </summary>
        public float currentValue;
        
        /// <summary>
        /// 最大值
        /// </summary>
        public float maxValue;
        
        /// <summary>
        /// 最小值
        /// </summary>
        public float minValue;
        
        /// <summary>
        /// 数据变化历史记录
        /// </summary>
        public List<StatisticsRecord> history;
        
        /// <summary>
        /// 统计数据类型
        /// </summary>
        public StatisticsType type;
        
        /// <summary>
        /// 数据单位
        /// </summary>
        public string unit;
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool isEnabled;
        
        /// <summary>
        /// 是否实时更新
        /// </summary>
        public bool realTimeUpdate;
        
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime lastUpdateTime;
        
        public StatisticsData()
        {
            history = new List<StatisticsRecord>();
            isEnabled = true;
            realTimeUpdate = true;
            lastUpdateTime = DateTime.Now;
        }
        
        public StatisticsData(string id, string name, float maxValue, StatisticsType type = StatisticsType.Numeric)
        {
            this.id = id;
            this.name = name;
            this.maxValue = maxValue;
            this.minValue = 0;
            this.currentValue = 0;
            this.type = type;
            this.unit = "";
            this.isEnabled = true;
            this.realTimeUpdate = true;
            this.lastUpdateTime = DateTime.Now;
            this.history = new List<StatisticsRecord>();
        }
        
        /// <summary>
        /// 设置值
        /// </summary>
        public virtual void SetValue(float value)
        {
            float oldValue = currentValue;
            currentValue = Mathf.Clamp(value, minValue, maxValue);
            
            if (Math.Abs(oldValue - currentValue) > 0.001f)
            {
                AddRecord(currentValue);
            }
        }
        
        /// <summary>
        /// 增加值
        /// </summary>
        public virtual void AddValue(float amount)
        {
            SetValue(currentValue + amount);
        }
        
        /// <summary>
        /// 获取百分比
        /// </summary>
        public float GetPercentage()
        {
            if (Math.Abs(maxValue - minValue) < 0.001f)
                return 0f;
            return (currentValue - minValue) / (maxValue - minValue) * 100f;
        }
        
        /// <summary>
        /// 添加历史记录
        /// </summary>
        public void AddRecord(float value)
        {
            StatisticsRecord record = new StatisticsRecord
            {
                value = value,
                timestamp = DateTime.Now
            };
            history.Add(record);
            lastUpdateTime = DateTime.Now;
            
            if (history.Count > 1000)
            {
                history.RemoveAt(0);
            }
        }
        
        /// <summary>
        /// 获取历史记录
        /// </summary>
        public List<StatisticsRecord> GetHistory(int count = -1)
        {
            if (count < 0 || count >= history.Count)
                return new List<StatisticsRecord>(history);
            
            return new List<StatisticsRecord>(history.GetRange(history.Count - count, count));
        }
        
        /// <summary>
        /// 重置数据
        /// </summary>
        public virtual void Reset()
        {
            currentValue = 0;
            history.Clear();
            lastUpdateTime = DateTime.Now;
        }
        
        /// <summary>
        /// 复制数据
        /// </summary>
        public StatisticsData Clone()
        {
            StatisticsData clone = new StatisticsData
            {
                id = this.id,
                name = this.name,
                currentValue = this.currentValue,
                maxValue = this.maxValue,
                minValue = this.minValue,
                type = this.type,
                unit = this.unit,
                isEnabled = this.isEnabled,
                realTimeUpdate = this.realTimeUpdate,
                lastUpdateTime = this.lastUpdateTime
            };
            
            foreach (var record in history)
            {
                clone.history.Add(record.Clone());
            }
            
            return clone;
        }
    }
    
    /// <summary>
    /// 统计记录
    /// </summary>
    [Serializable]
    public class StatisticsRecord
    {
        public float value;
        public DateTime timestamp;
        
        public StatisticsRecord()
        {
            value = 0;
            timestamp = DateTime.Now;
        }
        
        public StatisticsRecord(float value)
        {
            this.value = value;
            this.timestamp = DateTime.Now;
        }
        
        public StatisticsRecord Clone()
        {
            return new StatisticsRecord
            {
                value = this.value,
                timestamp = this.timestamp
            };
        }
    }
    
    /// <summary>
    /// 统计数据类型
    /// </summary>
    public enum StatisticsType
    {
        Numeric,
        Percentage,
        Rating,
        Currency,
        Time
    }
    
    /// <summary>
    /// 嘉宾统计数据
    /// </summary>
    [Serializable]
    public class GuestStatisticsData : StatisticsData
    {
        public string guestId;
        public string guestName;
        public int heartCount;
        public int conversationCount;
        public int dateCount;
        public float affectionLevel;
        
        public GuestStatisticsData(string guestId, string guestName) : base()
        {
            this.guestId = guestId;
            this.guestName = guestName;
            this.id = $"guest_{guestId}";
            this.name = $"{guestName}的好感度";
            this.maxValue = 100f;
            this.type = StatisticsType.Percentage;
        }
        
        public override void Reset()
        {
            base.Reset();
            heartCount = 0;
            conversationCount = 0;
            dateCount = 0;
            affectionLevel = 0;
        }
    }
    
    /// <summary>
    /// 导演统计数据
    /// </summary>
    [Serializable]
    public class DirectorStatisticsData : StatisticsData
    {
        public float realTimeHeat;
        public float viewership;
        public int danmakuCount;
        public long topicViews;
        public float douyinHeat;
        public float rating;
        public int revenue;
        public int actionPoints;
        
        public DirectorStatisticsData() : base()
        {
            this.id = "director_stats";
            this.name = "导演模式统计";
            realTimeHeat = 50f;
            viewership = 0.5f;
            danmakuCount = 0;
            topicViews = 0;
            douyinHeat = 0f;
            rating = 7.0f;
            revenue = 0;
            actionPoints = 100;
        }
        
        public override void Reset()
        {
            base.Reset();
            realTimeHeat = 50f;
            viewership = 0.5f;
            danmakuCount = 0;
            topicViews = 0;
            douyinHeat = 0f;
            rating = 7.0f;
            revenue = 0;
            actionPoints = 100;
        }
    }
}
