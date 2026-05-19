using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 时间周期类型
    /// </summary>
    public enum TimePeriod
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }

    /// <summary>
    /// 星期类型
    /// </summary>
    public enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    /// <summary>
    /// 季节类型
    /// </summary>
    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    /// <summary>
    /// 游戏时间数据
    /// </summary>
    [System.Serializable]
    public class GameTimeData
    {
        [Header("当前时间")]
        public int year = 2024;
        public int month = 1;
        public int day = 1;
        public int hour = 8;
        public int minute = 0;
        public int second = 0;

        [Header("游戏内时间流速设置")]
        public float timeScale = 1.0f;
        public int minutesPerRealSecond = 1;
        public bool isPaused = false;

        [Header("周期信息")]
        public TimePeriod currentPeriod;
        public DayOfWeek currentDayOfWeek;
        public Season currentSeason;
        public int dayNumber;
        public int weekNumber;

        [Header("特殊时间标记")]
        public bool isHoliday;
        public string holidayName;
        public bool isAnniversary;
        public bool isValentine;
        public bool isChristmas;
    }

    /// <summary>
    /// 日程安排
    /// </summary>
    [System.Serializable]
    public class ScheduleEntry
    {
        public string entryId;
        public int dayNumber;
        public int startHour;
        public int startMinute;
        public int endHour;
        public int endMinute;
        public ScheduleType type;
        public string title;
        public string description;
        public string locationSceneId;
        public List<string> requiredCharacterIds = new List<string>();
        public List<string> optionalCharacterIds = new List<string>();
        public bool isCompleted;
        public bool isMandatory;
        public int priority;
    }

    /// <summary>
    /// 日程类型
    /// </summary>
    public enum ScheduleType
    {
        FreeTime,
        GroupActivity,
        DateTime,
        Challenge,
        Elimination,
        SpecialEvent,
        RestTime,
        Meeting
    }

    /// <summary>
    /// 约会安排
    /// </summary>
    [System.Serializable]
    public class DateSchedule
    {
        public string scheduleId;
        public int dayNumber;
        public TimePeriod timePeriod;
        public string maleGuestId;
        public string femaleGuestId;
        public string sceneId;
        public DateType dateType;
        public bool isConfirmed;
        public bool isCompleted;
        public DateTime? actualStartTime;
        public DateTime? actualEndTime;
        public int duration;
        public List<DateEventRecord> eventRecords = new List<DateEventRecord>();
    }

    /// <summary>
    /// 约会类型
    /// </summary>
    public enum DateType
    {
        OneOnOne,
        GroupDate,
        DoubleDate,
        SurpriseDate,
        CompetitionDate
    }

    /// <summary>
    /// 约会事件记录
    /// </summary>
    [System.Serializable]
    public class DateEventRecord
    {
        public string eventId;
        public string eventName;
        public int timestamp;
        public string description;
        public List<string> participantIds = new List<string>();
        public int moodChange;
        public int relationshipChange;
    }

    /// <summary>
    /// 任务时间限制
    /// </summary>
    [System.Serializable]
    public class TaskTimeLimit
    {
        public string taskId;
        public int dayNumber;
        public int deadlineHour;
        public int deadlineMinute;
        public bool isExpired;
        public int bonusTimeReward;
    }

    /// <summary>
    /// 提醒数据
    /// </summary>
    [System.Serializable]
    public class ReminderData
    {
        public string reminderId;
        public string title;
        public string message;
        public int reminderDay;
        public int reminderHour;
        public int reminderMinute;
        public bool isShown;
        public ReminderPriority priority;
    }

    /// <summary>
    /// 提醒优先级
    /// </summary>
    public enum ReminderPriority
    {
        Low,
        Normal,
        High,
        Urgent
    }

    /// <summary>
    /// 时间系统数据管理器
    /// </summary>
    [System.Serializable]
    public class TimeSystemData
    {
        [Header("基础时间数据")]
        public GameTimeData gameTime = new GameTimeData();

        [Header("日程管理")]
        public List<ScheduleEntry> weeklySchedule = new List<ScheduleEntry>();
        public List<ScheduleEntry> dailySchedule = new List<ScheduleEntry>();
        public ScheduleEntry currentActivity;
        public ScheduleEntry nextActivity;

        [Header("约会安排")]
        public List<DateSchedule> allDateSchedules = new List<DateSchedule>();
        public DateSchedule currentDate;
        public DateSchedule nextDate;

        [Header("任务时间限制")]
        public List<TaskTimeLimit> taskTimeLimits = new List<TaskTimeLimit>();

        [Header("提醒")]
        public List<ReminderData> reminders = new List<ReminderData>();

        [Header("时间事件触发器")]
        public List<TimeEventTrigger> timeEventTriggers = new List<TimeEventTrigger>();

        [Header("时间统计")]
        public int totalGameDays;
        public int totalGameHours;
        public int totalPausedTime;
        public Dictionary<string, int> timeSpentInScenes = new Dictionary<string, int>();
    }

    /// <summary>
    /// 时间事件触发器
    /// </summary>
    [System.Serializable]
    public class TimeEventTrigger
    {
        public string triggerId;
        public TriggerCondition condition;
        public string eventId;
        public bool isTriggered;
        public DateTime? triggeredTime;
    }

    /// <summary>
    /// 触发条件
    /// </summary>
    [System.Serializable]
    public class TriggerCondition
    {
        public ConditionType type;
        public int dayNumber;
        public int hour;
        public int minute;
        public string targetId;
        public string comparisonOperator;
        public int value;
    }

    /// <summary>
    /// 条件类型
    /// </summary>
    public enum ConditionType
    {
        TimeReached,
        RelationshipLevel,
        TaskCompleted,
        FlagSet,
        Random
    }

    /// <summary>
    /// 时间工具类
    /// </summary>
    public static class TimeSystemHelper
    {
        public static TimePeriod GetTimePeriod(int hour)
        {
            if (hour >= 6 && hour < 12) return TimePeriod.Morning;
            if (hour >= 12 && hour < 18) return TimePeriod.Afternoon;
            if (hour >= 18 && hour < 22) return TimePeriod.Evening;
            return TimePeriod.Night;
        }

        public static DayOfWeek GetDayOfWeek(int dayNumber)
        {
            int dayIndex = ((dayNumber - 1) % 7);
            return (DayOfWeek)dayIndex;
        }

        public static Season GetSeason(int month)
        {
            if (month >= 3 && month <= 5) return Season.Spring;
            if (month >= 6 && month <= 8) return Season.Summer;
            if (month >= 9 && month <= 11) return Season.Autumn;
            return Season.Winter;
        }

        public static bool IsSpecialDay(int month, int day)
        {
            if (month == 2 && day == 14) return true;
            if (month == 12 && day == 25) return true;
            if (month == 1 && day == 1) return true;
            return false;
        }

        public static string FormatGameTime(GameTimeData time)
        {
            return $"{time.year}年{time.month}月{time.day}日 {time.hour:D2}:{time.minute:D2}";
        }

        public static int GetTotalMinutes(GameTimeData time)
        {
            return time.hour * 60 + time.minute;
        }

        public static bool IsTimeInRange(int currentMinutes, int startMinutes, int endMinutes)
        {
            if (startMinutes <= endMinutes)
            {
                return currentMinutes >= startMinutes && currentMinutes <= endMinutes;
            }
            else
            {
                return currentMinutes >= startMinutes || currentMinutes <= endMinutes;
            }
        }
    }
}
