using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeartHook.Game.Ending
{
    public class EndingManager : MonoBehaviour
    {
        public static EndingManager Instance { get; private set; }

        [SerializeField] private EndingData[] allEndings;
        [SerializeField] private EndingUnlocker unlocker;

        private Dictionary<string, EndingData> endingDict = new Dictionary<string, EndingData>();
        private List<string> unlockedEndings = new List<string>();
        private string currentEndingId;
        private int playCount;

        public event Action<string> OnEndingUnlocked;
        public event Action<string> OnEndingViewed;
        public event Action OnAllEndingsUnlocked;

        public IReadOnlyList<string> UnlockedEndings => unlockedEndings;
        public string CurrentEndingId => currentEndingId;
        public int TotalPlayCount => playCount;
        public int TotalEndingCount => allEndings?.Length ?? 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }

        private void Initialize()
        {
            if (allEndings == null || allEndings.Length == 0)
            {
                Debug.LogWarning("[EndingManager] 没有配置结局数据！");
                return;
            }

            foreach (var ending in allEndings)
            {
                if (ending != null && !string.IsNullOrEmpty(ending.Id))
                {
                    endingDict[ending.Id] = ending;
                }
            }

            unlocker = new EndingUnlocker();
            LoadProgress();
        }

        public bool IsEndingUnlocked(string endingId)
        {
            return unlockedEndings.Contains(endingId);
        }

        public bool CanPreviewEnding(string endingId)
        {
            if (!endingDict.TryGetValue(endingId, out var ending))
                return false;

            if (ending.UnlockCondition != null)
            {
                return ending.UnlockCondition.CanUnlock(unlocker);
            }

            return false;
        }

        public EndingData GetEndingData(string endingId)
        {
            return endingDict.TryGetValue(endingId, out var data) ? data : null;
        }

        public EndingData[] GetAllEndings()
        {
            return allEndings;
        }

        public EndingData[] GetUnlockedEndings()
        {
            var unlocked = new List<EndingData>();
            foreach (var id in unlockedEndings)
            {
                if (endingDict.TryGetValue(id, out var ending))
                {
                    unlocked.Add(ending);
                }
            }
            return unlocked.ToArray();
        }

        public void UnlockEnding(string endingId)
        {
            if (string.IsNullOrEmpty(endingId))
            {
                Debug.LogWarning("[EndingManager] 尝试解锁空结局ID！");
                return;
            }

            if (!endingDict.ContainsKey(endingId))
            {
                Debug.LogWarning($"[EndingManager] 结局ID不存在: {endingId}");
                return;
            }

            if (unlockedEndings.Contains(endingId))
            {
                return;
            }

            unlockedEndings.Add(endingId);
            currentEndingId = endingId;
            SaveProgress();
            OnEndingUnlocked?.Invoke(endingId);

            if (unlockedEndings.Count >= TotalEndingCount)
            {
                OnAllEndingsUnlocked?.Invoke();
            }
        }

        public void TriggerEnding(string endingId)
        {
            if (!endingDict.ContainsKey(endingId))
            {
                Debug.LogError($"[EndingManager] 结局ID不存在: {endingId}");
                return;
            }

            UnlockEnding(endingId);
            playCount++;
            SaveProgress();
            OnEndingViewed?.Invoke(endingId);
        }

        public void MarkEndingAsViewed(string endingId)
        {
            if (!endingDict.TryGetValue(endingId, out var ending))
                return;

            ending.MarkAsViewed();
            SaveProgress();
        }

        public float GetCompletionRate()
        {
            if (TotalEndingCount == 0)
                return 0f;
            return (float)unlockedEndings.Count / TotalEndingCount;
        }

        public Dictionary<string, float> GetEndingStatistics()
        {
            var stats = new Dictionary<string, float>
            {
                { "completionRate", GetCompletionRate() },
                { "unlockedCount", unlockedEndings.Count },
                { "totalCount", TotalEndingCount },
                { "playCount", playCount }
            };
            return stats;
        }

        public void SaveProgress()
        {
            var saveData = new EndingSaveData
            {
                UnlockedEndings = unlockedEndings,
                PlayCount = playCount,
                ViewedEndings = GetViewedEndingIds()
            };
            ES3.Save("EndingSaveData", saveData);
        }

        public void LoadProgress()
        {
            if (ES3.KeyExists("EndingSaveData"))
            {
                var saveData = ES3.Load<EndingSaveData>("EndingSaveData");
                if (saveData != null)
                {
                    unlockedEndings = saveData.UnlockedEndings ?? new List<string>();
                    playCount = saveData.PlayCount;
                    LoadViewedStatus(saveData.ViewedEndings);
                }
            }
        }

        private string[] GetViewedEndingIds()
        {
            var viewed = new List<string>();
            foreach (var ending in allEndings)
            {
                if (ending != null && ending.IsViewed)
                {
                    viewed.Add(ending.Id);
                }
            }
            return viewed.ToArray();
        }

        private void LoadViewedStatus(string[] viewedIds)
        {
            if (viewedIds == null)
                return;

            foreach (var ending in allEndings)
            {
                if (ending != null && Array.Exists(viewedIds, id => id == ending.Id))
                {
                    ending.MarkAsViewed();
                }
            }
        }

        public void ResetProgress()
        {
            unlockedEndings.Clear();
            playCount = 0;
            foreach (var ending in allEndings)
            {
                if (ending != null)
                {
                    ending.ResetViewed();
                }
            }
            ES3.DeleteKey("EndingSaveData");
        }

        public void NewGamePlus(int bonusFavorability = 10)
        {
            playCount = 0;
        }
    }

    [Serializable]
    public class EndingSaveData
    {
        public List<string> UnlockedEndings;
        public int PlayCount;
        public string[] ViewedEndings;
    }
}
