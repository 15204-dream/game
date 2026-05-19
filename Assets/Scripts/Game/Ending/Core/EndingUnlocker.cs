using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeartHook.Game.Ending
{
    public class EndingUnlocker
    {
        private Dictionary<string, int> favorabilityMap = new Dictionary<string, int>();
        private HashSet<string> mutualChoices = new HashSet<string>();
        private HashSet<string> completedTasks = new HashSet<string>();
        private HashSet<string> triggeredEvents = new HashSet<string>();
        private HashSet<string> triggeredCrises = new HashSet<string>();
        private int currentDay = 1;
        private int heat;
        private int reputation;
        private int audienceCount;
        private bool playerQuit;
        private bool noHearts;

        public void SetFavorability(string characterId, int value)
        {
            favorabilityMap[characterId] = Mathf.Clamp(value, 0, 100);
        }

        public int GetFavorability(string characterId)
        {
            return favorabilityMap.TryGetValue(characterId, out var value) ? value : 0;
        }

        public Dictionary<string, int> GetAllFavorability()
        {
            return new Dictionary<string, int>(favorabilityMap);
        }

        public void AddMutualChoice(string characterId)
        {
            mutualChoices.Add(characterId);
        }

        public bool HasMutualChoice(string characterId)
        {
            return mutualChoices.Contains(characterId);
        }

        public int GetMutualChoiceCount()
        {
            return mutualChoices.Count;
        }

        public void CompleteTask(string taskId)
        {
            if (!string.IsNullOrEmpty(taskId))
            {
                completedTasks.Add(taskId);
            }
        }

        public bool IsTaskCompleted(string taskId)
        {
            return completedTasks.Contains(taskId);
        }

        public int GetCompletedTaskCount()
        {
            return completedTasks.Count;
        }

        public void TriggerEvent(string eventId)
        {
            if (!string.IsNullOrEmpty(eventId))
            {
                triggeredEvents.Add(eventId);
            }
        }

        public bool IsEventTriggered(string eventId)
        {
            return triggeredEvents.Contains(eventId);
        }

        public void TriggerCrisis(string crisisId)
        {
            if (!string.IsNullOrEmpty(crisisId))
            {
                triggeredCrises.Add(crisisId);
            }
        }

        public bool IsCrisisTriggered(string crisisId)
        {
            return triggeredCrises.Contains(crisisId);
        }

        public bool HasAnyCrisis()
        {
            return triggeredCrises.Count > 0;
        }

        public void SetCurrentDay(int day)
        {
            currentDay = Mathf.Max(1, day);
        }

        public int GetCurrentDay()
        {
            return currentDay;
        }

        public void SetHeat(int value)
        {
            heat = Mathf.Max(0, value);
        }

        public int GetHeat()
        {
            return heat;
        }

        public void SetReputation(int value)
        {
            reputation = Mathf.Max(0, value);
        }

        public int GetReputation()
        {
            return reputation;
        }

        public void SetAudienceCount(int value)
        {
            audienceCount = Mathf.Max(0, value);
        }

        public int GetAudienceCount()
        {
            return audienceCount;
        }

        public void SetPlayerQuit(bool value)
        {
            playerQuit = value;
        }

        public bool HasPlayerQuit()
        {
            return playerQuit;
        }

        public void SetNoHearts(bool value)
        {
            noHearts = value;
        }

        public bool HasNoHearts()
        {
            return noHearts;
        }

        public int GetMaxFavorability()
        {
            if (favorabilityMap.Count == 0)
                return 0;

            int max = 0;
            foreach (var kvp in favorabilityMap)
            {
                if (kvp.Value > max)
                    max = kvp.Value;
            }
            return max;
        }

        public string GetHighestFavorabilityCharacter()
        {
            if (favorabilityMap.Count == 0)
                return null;

            string result = null;
            int max = 0;
            foreach (var kvp in favorabilityMap)
            {
                if (kvp.Value > max)
                {
                    max = kvp.Value;
                    result = kvp.Key;
                }
            }
            return result;
        }

        public bool AllFavorabilityInRange(int min, int max)
        {
            if (favorabilityMap.Count == 0)
                return false;

            foreach (var kvp in favorabilityMap)
            {
                if (kvp.Value < min || kvp.Value > max)
                    return false;
            }
            return true;
        }

        public void Clear()
        {
            favorabilityMap.Clear();
            mutualChoices.Clear();
            completedTasks.Clear();
            triggeredEvents.Clear();
            triggeredCrises.Clear();
            currentDay = 1;
            heat = 0;
            reputation = 0;
            audienceCount = 0;
            playerQuit = false;
            noHearts = false;
        }

        public void CopyFrom(EndingUnlocker other)
        {
            if (other == null)
                return;

            favorabilityMap = new Dictionary<string, int>(other.favorabilityMap);
            mutualChoices = new HashSet<string>(other.mutualChoices);
            completedTasks = new HashSet<string>(other.completedTasks);
            triggeredEvents = new HashSet<string>(other.triggeredEvents);
            triggeredCrises = new HashSet<string>(other.triggeredCrises);
            currentDay = other.currentDay;
            heat = other.heat;
            reputation = other.reputation;
            audienceCount = other.audienceCount;
            playerQuit = other.playerQuit;
            noHearts = other.noHearts;
        }
    }

    [Serializable]
    public class SerializableCallback<T> where T : struct
    {
        [SerializeField] private bool useConstant;
        [SerializeField] private T constantValue;
        [SerializeField] private string methodName;

        public T Invoke()
        {
            return constantValue;
        }
    }
}
