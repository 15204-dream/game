using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum GuestTaskType
    {
        Main,
        Side,
        Daily,
        Weekly,
        Hidden,
        Achievement,
        Special,
        Challenge
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskStatus
    {
        Locked,
        Available,
        InProgress,
        Completed,
        Failed,
        Skipped
    }

    /// <summary>
    /// 任务难度
    /// </summary>
    public enum TaskDifficulty
    {
        Easy,
        Normal,
        Hard,
        Expert,
        Master
    }

    /// <summary>
    /// 任务目标
    /// </summary>
    [System.Serializable]
    public class TaskObjective
    {
        public string objectiveId;
        public string description;
        public ObjectiveType type;
        public string targetId;
        public int targetCount;
        public int currentCount;
        public bool isOptional;
        public List<string> validTargetIds = new List<string>();
    }

    /// <summary>
    /// 目标类型
    /// </summary>
    public enum ObjectiveType
    {
        Talk,
        GiveGift,
        GoOnDate,
        WinCompetition,
        CompleteScene,
        CollectItem,
        ReachRelationshipLevel,
        UseAbility,
        SurviveElimination,
        MakeChoice
    }

    /// <summary>
    /// 任务奖励
    /// </summary>
    [System.Serializable]
    public class TaskReward
    {
        public int coins;
        public int gems;
        public int popularity;
        public int experience;
        public List<ItemReward> items = new List<ItemReward>();
        public List<string> unlockAbilities = new List<string>();
        public List<string> unlockScenes = new List<string>();
        public int affectionPoints;
        public string targetCharacterId;
    }

    /// <summary>
    /// 物品奖励
    /// </summary>
    [System.Serializable]
    public class ItemReward
    {
        public string itemId;
        public string itemName;
        public int quantity;
        public int rarity;
    }

    /// <summary>
    /// 任务进度数据
    /// </summary>
    [System.Serializable]
    public class GuestTaskData
    {
        [Header("任务列表")]
        public List<TaskProgress> allTasks = new List<TaskProgress>();
        public List<TaskProgress> availableTasks = new List<TaskProgress>();
        public List<TaskProgress> completedTasks = new List<TaskProgress>();
        public List<TaskProgress> failedTasks = new List<TaskProgress>();

        [Header("当前活动任务")]
        public TaskProgress currentMainTask;
        public List<TaskProgress> currentSideTasks = new List<TaskProgress>();
        public List<TaskProgress> dailyTasks = new List<TaskProgress>();

        [Header("任务统计")]
        public int totalTasksCompleted;
        public int totalTasksFailed;
        public int currentTaskStreak;
        public int longestTaskStreak;
        public int perfectTaskCount;
        public Dictionary<TaskDifficulty, int> difficultyStats = new Dictionary<TaskDifficulty, int>();

        [Header("周期性任务")]
        public WeeklyTaskProgress weeklyProgress = new WeeklyTaskProgress();
        public DailyTaskProgress dailyProgress = new DailyTaskProgress();

        [Header("隐藏任务")]
        public List<string> discoveredHiddenTasks = new List<string>();
        public List<string> completedHiddenTasks = new List<string>();

        [Header("成就任务")]
        public List<AchievementTaskProgress> achievementTasks = new List<AchievementTaskProgress>();
    }

    /// <summary>
    /// 任务进度
    /// </summary>
    [System.Serializable]
    public class TaskProgress
    {
        public string taskId;
        public string taskName;
        public string description;
        public GuestTaskType taskType;
        public TaskDifficulty difficulty;
        public TaskStatus status;

        [Header("目标")]
        public List<TaskObjective> objectives = new List<TaskObjective>();

        [Header("时间限制")]
        public DateTime? startTime;
        public DateTime? deadline;
        public int? timeLimitMinutes;
        public bool isExpired;

        [Header("奖励")]
        public TaskReward reward;
        public TaskReward bonusReward;
        public bool bonusRewardClaimed;

        [Header("进度")]
        public float completionPercentage;
        public int totalSteps;
        public int completedSteps;

        [Header("依赖")]
        public List<string> prerequisiteTaskIds = new List<string>();
        public List<string> dependentTaskIds = new List<string>();

        [Header("限制")]
        public List<string> allowedScenes = new List<string>();
        public List<string> requiredTags = new List<string>();
        public int? requiredPlayerLevel;

        [Header("特殊条件")]
        public bool canSkip;
        public bool canRetake;
        public bool hasHiddenObjectives;
        public List<string> hintText = new List<string>();
    }

    /// <summary>
    /// 周任务进度
    /// </summary>
    [System.Serializable]
    public class WeeklyTaskProgress
    {
        public int weekNumber;
        public List<TaskProgress> weeklyTasks = new List<TaskProgress>();
        public int tasksCompletedThisWeek;
        public int totalWeeklyTasks;
        public int weeklyTaskPoints;
        public int weeklyBonusPoints;
        public bool weeklyBonusClaimed;
        public List<string> bonusRewardIds = new List<string>();
    }

    /// <summary>
    /// 每日任务进度
    /// </summary>
    [System.Serializable]
    public class DailyTaskProgress
    {
        public int dayNumber;
        public List<TaskProgress> dailyTasks = new List<TaskProgress>();
        public int tasksCompletedToday;
        public int totalDailyTasks;
        public int dailyTaskPoints;
        public int dailyStreak;
        public bool dailyBonusClaimed;
        public DateTime? lastDailyTaskCompleteTime;
    }

    /// <summary>
    /// 成就任务进度
    /// </summary>
    [System.Serializable]
    public class AchievementTaskProgress
    {
        public string achievementId;
        public string achievementName;
        public string description;
        public float progress;
        public float targetProgress;
        public bool isUnlocked;
        public DateTime? unlockTime;
        public TaskReward reward;
    }

    /// <summary>
    /// 任务工具类
    /// </summary>
    public static class GuestTaskHelper
    {
        public static float CalculateTaskProgress(TaskProgress task)
        {
            if (task.objectives.Count == 0) return 0f;

            float totalProgress = 0f;
            int optionalCount = 0;
            int completedOptional = 0;

            foreach (var objective in task.objectives)
            {
                if (objective.isOptional)
                {
                    optionalCount++;
                    if (objective.currentCount >= objective.targetCount)
                    {
                        completedOptional++;
                    }
                }
                else
                {
                    totalProgress += Mathf.Min(1f, (float)objective.currentCount / objective.targetCount);
                }
            }

            float optionalBonus = optionalCount > 0 ? (float)completedOptional / optionalCount * 0.1f : 0f;
            float baseProgress = task.objectives.Exists(o => !o.isOptional)
                ? totalProgress / task.objectives.FindAll(o => !o.isOptional).Count
                : 0f;

            return Mathf.Clamp01(baseProgress + optionalBonus);
        }

        public static bool CanStartTask(TaskProgress task, GuestPlayerData player, FriendshipData friendship)
        {
            if (task.status != TaskStatus.Available && task.status != TaskStatus.Locked)
            {
                return false;
            }

            if (task.requiredPlayerLevel.HasValue && player.abilities.charm < task.requiredPlayerLevel.Value)
            {
                return false;
            }

            foreach (var prereqId in task.prerequisiteTaskIds)
            {
                if (!task.prerequisiteTaskIds.Contains(prereqId))
                {
                    return false;
                }
            }

            return true;
        }

        public static int CalculateTaskScore(TaskProgress task, TimeSpan completionTime)
        {
            int baseScore = (int)(task.difficulty + 1) * 100;
            float difficultyMultiplier = 1f + ((int)task.difficulty * 0.5f);

            TimeSpan timeLimit = task.deadline.HasValue && task.startTime.HasValue
                ? task.deadline.Value - task.startTime.Value
                : TimeSpan.FromHours(24);

            float timeBonus = 1f;
            if (completionTime < timeLimit)
            {
                double timeRatio = completionTime.TotalMinutes / timeLimit.TotalMinutes;
                timeBonus = 1f + (1f - (float)timeRatio);
            }

            int perfectBonus = task.objectives.TrueForAll(o => o.currentCount >= o.targetCount) ? 50 : 0;

            return Mathf.RoundToInt(baseScore * difficultyMultiplier * timeBonus + perfectBonus);
        }

        public static TaskDifficulty GetDifficultyFromScore(int score)
        {
            if (score < 200) return TaskDifficulty.Easy;
            if (score < 500) return TaskDifficulty.Normal;
            if (score < 1000) return TaskDifficulty.Hard;
            if (score < 2000) return TaskDifficulty.Expert;
            return TaskDifficulty.Master;
        }
    }
}
