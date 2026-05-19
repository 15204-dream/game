using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeartHook.Game.Ending
{
    [Serializable]
    public class FavorabilityCondition : EndingConditionBase
    {
        public string TargetCharacterId;
        public int MinFavorability = 80;
        public bool RequireMutualChoice;

        public override string GetDescription()
        {
            var desc = $"好感度达到{MinFavorability}";
            if (!string.IsNullOrEmpty(TargetCharacterId))
            {
                desc = $"与{TargetCharacterId}的" + desc;
            }
            if (RequireMutualChoice)
            {
                desc += "且互选成功";
            }
            return desc;
        }

        public override bool CanUnlock(EndingUnlocker unlocker)
        {
            if (unlocker == null)
                return false;

            if (!string.IsNullOrEmpty(TargetCharacterId))
            {
                var favorability = unlocker.GetFavorability(TargetCharacterId);
                var mutualChoice = unlocker.HasMutualChoice(TargetCharacterId);

                if (RequireMutualChoice)
                {
                    return favorability >= MinFavorability && mutualChoice;
                }
                return favorability >= MinFavorability;
            }

            var allFavorability = unlocker.GetAllFavorability();
            foreach (var fav in allFavorability)
            {
                if (fav.Value >= MinFavorability)
                {
                    return !RequireMutualChoice || unlocker.HasMutualChoice(fav.Key);
                }
            }
            return false;
        }

        public override bool CanBePreviewed()
        {
            return false;
        }
    }

    [Serializable]
    public class TaskCompletionCondition : EndingConditionBase
    {
        public string[] RequiredTaskIds;
        public int MinTaskCount = 1;

        public override string GetDescription()
        {
            if (RequiredTaskIds != null && RequiredTaskIds.Length > 0)
            {
                return $"完成指定任务: {RequiredTaskIds.Length}个";
            }
            return $"完成至少{MinTaskCount}个任务";
        }

        public override bool CanUnlock(EndingUnlocker unlocker)
        {
            if (unlocker == null)
                return false;

            if (RequiredTaskIds != null && RequiredTaskIds.Length > 0)
            {
                int completedCount = 0;
                foreach (var taskId in RequiredTaskIds)
                {
                    if (unlocker.IsTaskCompleted(taskId))
                    {
                        completedCount++;
                    }
                }
                return completedCount >= RequiredTaskIds.Length;
            }

            return unlocker.GetCompletedTaskCount() >= MinTaskCount;
        }

        public override bool CanBePreviewed()
        {
            return false;
        }
    }

    [Serializable]
    public class DayCondition : EndingConditionBase
    {
        public int MinDay = 1;
        public int MaxDay = 999;
        public bool RequireDayReached;

        public override string GetDescription()
        {
            if (RequireDayReached)
            {
                return $"存活至第{MinDay}天";
            }
            return $"游戏天数{MinDay}-{MaxDay}";
        }

        public override bool CanUnlock(EndingUnlocker unlocker)
        {
            if (unlocker == null)
                return false;

            var currentDay = unlocker.GetCurrentDay();
            if (RequireDayReached)
            {
                return currentDay >= MinDay;
            }
            return currentDay >= MinDay && currentDay <= MaxDay;
        }

        public override bool CanBePreviewed()
        {
            return false;
        }
    }

    [Serializable]
    public class SpecialEventCondition : EndingConditionBase
    {
        public string EventId;
        public bool RequireNotTriggered;

        public override string GetDescription()
        {
            if (RequireNotTriggered)
            {
                return $"未触发事件: {EventId}";
            }
            return $"已触发事件: {EventId}";
        }

        public override bool CanUnlock(EndingUnlocker unlocker)
        {
            if (unlocker == null)
                return false;

            var triggered = unlocker.IsEventTriggered(EventId);
            return RequireNotTriggered ? !triggered : triggered;
        }

        public override bool CanBePreviewed()
        {
            return true;
        }
    }

    [Serializable]
    public class CombinationCondition : EndingConditionBase
    {
        public EndingConditionBase[] Conditions;
        public CombinationType LogicType = CombinationType.And;

        public override string GetDescription()
        {
            if (Conditions == null || Conditions.Length == 0)
                return "无条件";

            var descs = new List<string>();
            foreach (var cond in Conditions)
            {
                if (cond != null)
                {
                    descs.Add(cond.GetDescription());
                }
            }

            var separator = LogicType == CombinationType.And ? " 且 " : " 或 ";
            return string.Join(separator, descs);
        }

        public override bool CanUnlock(EndingUnlocker unlocker)
        {
            if (Conditions == null || Conditions.Length == 0)
                return true;

            switch (LogicType)
            {
                case CombinationType.And:
                    foreach (var cond in Conditions)
                    {
                        if (cond != null && !cond.CanUnlock(unlocker))
                            return false;
                    }
                    return true;

                case CombinationType.Or:
                    foreach (var cond in Conditions)
                    {
                        if (cond != null && cond.CanUnlock(unlocker))
                            return true;
                    }
                    return false;

                default:
                    return false;
            }
        }

        public override bool CanBePreviewed()
        {
            if (Conditions == null || Conditions.Length == 0)
                return true;

            foreach (var cond in Conditions)
            {
                if (cond != null && cond.CanBePreviewed())
                    return true;
            }
            return false;
        }
    }

    public enum CombinationType
    {
        And,
        Or
    }

    [Serializable]
    public class RatingCondition : EndingConditionBase
    {
        public int MinHeat = 0;
        public int MinReputation = 0;
        public int MinAudienceCount = 0;

        public override string GetDescription()
        {
            var parts = new List<string>();
            if (MinHeat > 0) parts.Add($"热度≥{MinHeat}");
            if (MinReputation > 0) parts.Add($"口碑≥{MinReputation}");
            if (MinAudienceCount > 0) parts.Add($"观众≥{MinAudienceCount}");
            return parts.Count > 0 ? string.Join(", ", parts) : "无要求";
        }

        public override bool CanUnlock(EndingUnlocker unlocker)
        {
            if (unlocker == null)
                return false;

            return unlocker.GetHeat() >= MinHeat &&
                   unlocker.GetReputation() >= MinReputation &&
                   unlocker.GetAudienceCount() >= MinAudienceCount;
        }

        public override bool CanBePreviewed()
        {
            return false;
        }
    }

    [Serializable]
    public class CrisisCondition : EndingConditionBase
    {
        public bool RequireNoCrisis = true;
        public string[] CrisisIds;

        public override string GetDescription()
        {
            if (RequireNoCrisis)
                return "未触发任何危机";
            return $"触发了危机: {CrisisIds?.Length ?? 0}个";
        }

        public override bool CanUnlock(EndingUnlocker unlocker)
        {
            if (unlocker == null)
                return false;

            if (RequireNoCrisis)
            {
                return !unlocker.HasAnyCrisis();
            }

            if (CrisisIds != null)
            {
                foreach (var crisisId in CrisisIds)
                {
                    if (unlocker.IsCrisisTriggered(crisisId))
                        return true;
                }
            }
            return false;
        }

        public override bool CanBePreviewed()
        {
            return true;
        }
    }
}
