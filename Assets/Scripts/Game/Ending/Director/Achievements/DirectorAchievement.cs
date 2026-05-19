using System;
using UnityEngine;

namespace HeartHook.Game.Ending.Director.Achievements
{
    [Serializable]
    public abstract class DirectorAchievement : MonoBehaviour
    {
        public string AchievementId;
        public string AchievementName;
        public string Description;
        public Sprite Icon;
        public bool IsSecret;
        public bool IsUnlocked { get; protected set; }

        [Header("解锁条件")]
        [SerializeField] protected int requiredCPCount;
        [SerializeField] protected int requiredStarCount;
        [SerializeField] protected int requiredTopicCount;
        [SerializeField] protected bool requireNoCrisis;
        [SerializeField] protected int minHeat;
        [SerializeField] protected int minReputation;

        public event Action OnAchievementUnlocked;

        public virtual bool CheckUnlock(DirectorEndingResult result)
        {
            if (result == null)
                return false;

            if (requiredCPCount > 0 && result.CPCount < requiredCPCount)
                return false;

            if (requiredStarCount > 0 && result.StarCount < requiredStarCount)
                return false;

            if (requiredTopicCount > 0 && result.TopicCount < requiredTopicCount)
                return false;

            if (requireNoCrisis && result.HasCrisis)
                return false;

            if (minHeat > 0 && result.FinalHeat < minHeat)
                return false;

            if (minReputation > 0 && result.FinalReputation < minReputation)
                return false;

            return true;
        }

        public virtual void Unlock()
        {
            if (IsUnlocked)
                return;

            IsUnlocked = true;
            OnAchievementUnlocked?.Invoke();

            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.TriggerEnding($"Achievement_{AchievementId}");
            }
        }

        public string GetDisplayName()
        {
            return IsSecret && !IsUnlocked ? "???" : AchievementName;
        }

        public string GetDisplayDescription()
        {
            return IsSecret && !IsUnlocked ? "未解锁" : Description;
        }
    }
}
