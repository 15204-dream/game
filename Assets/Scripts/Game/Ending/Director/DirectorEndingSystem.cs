using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeartHook.Game.Ending.Director
{
    public class DirectorEndingSystem : MonoBehaviour
    {
        public static DirectorEndingSystem Instance { get; private set; }

        [SerializeField] private DirectorEndingData endingData;
        [SerializeField] private RankEndingBase[] rankEndings;
        [SerializeField] private DirectorAchievement[] achievements;

        private DirectorGameState currentGameState;
        private int totalHeat;
        private int totalReputation;
        private int totalAudience;
        private List<string> triggeredCrises = new List<string>();
        private List<string> createdCPs = new List<string>();
        private List<string> createdStars = new List<string>();
        private List<string> triggeredTopics = new List<string>();

        public event Action<string> OnRankEndingAchieved;
        public event Action<string> OnAchievementUnlocked;
        public event Action<DirectorEndingResult> OnEndingCalculated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            if (rankEndings == null || rankEndings.Length == 0)
            {
                rankEndings = GetComponentsInChildren<RankEndingBase>();
            }
            if (achievements == null || achievements.Length == 0)
            {
                achievements = GetComponentsInChildren<DirectorAchievement>();
            }
        }

        public void InitializeGameState()
        {
            currentGameState = new DirectorGameState();
            totalHeat = 0;
            totalReputation = 0;
            totalAudience = 0;
            triggeredCrises.Clear();
            createdCPs.Clear();
            createdStars.Clear();
            triggeredTopics.Clear();
        }

        public void AddHeat(int value)
        {
            totalHeat += value;
        }

        public void AddReputation(int value)
        {
            totalReputation += value;
        }

        public void AddAudience(int value)
        {
            totalAudience += value;
        }

        public void RecordCrisis(string crisisId)
        {
            if (!string.IsNullOrEmpty(crisisId) && !triggeredCrises.Contains(crisisId))
            {
                triggeredCrises.Add(crisisId);
            }
        }

        public void RecordCP(string cpId)
        {
            if (!string.IsNullOrEmpty(cpId) && !createdCPs.Contains(cpId))
            {
                createdCPs.Add(cpId);
            }
        }

        public void RecordStar(string starId)
        {
            if (!string.IsNullOrEmpty(starId) && !createdStars.Contains(starId))
            {
                createdStars.Add(starId);
            }
        }

        public void RecordTopic(string topicId)
        {
            if (!string.IsNullOrEmpty(topicId) && !triggeredTopics.Contains(topicId))
            {
                triggeredTopics.Add(topicId);
            }
        }

        public DirectorEndingResult CalculateEnding()
        {
            var result = new DirectorEndingResult();

            result.FinalHeat = totalHeat;
            result.FinalReputation = totalReputation;
            result.FinalAudience = totalAudience;
            result.HasCrisis = triggeredCrises.Count > 0;
            result.CrisisCount = triggeredCrises.Count;
            result.CPCount = createdCPs.Count;
            result.StarCount = createdStars.Count;
            result.TopicCount = triggeredTopics.Count;

            if (totalHeat >= 95 && totalReputation >= 90 && !result.HasCrisis)
            {
                result.RankId = "S";
                result.RankName = "现象级神作";
            }
            else if (totalHeat >= 80 && totalReputation >= 75)
            {
                result.RankId = "A";
                result.RankName = "年度爆款";
            }
            else if (totalHeat >= 60 && totalReputation >= 60)
            {
                result.RankId = "B";
                result.RankName = "热播综艺";
            }
            else if (totalHeat >= 40 && totalReputation >= 40)
            {
                result.RankId = "C";
                result.RankName = "平稳收官";
            }
            else if (totalReputation >= 20)
            {
                result.RankId = "D";
                result.RankName = "口碑滑铁卢";
            }
            else
            {
                result.RankId = "E";
                result.RankName = "紧急停播";
            }

            result.Achievements = CalculateAchievements(result);

            OnEndingCalculated?.Invoke(result);
            return result;
        }

        private List<string> CalculateAchievements(DirectorEndingResult result)
        {
            var unlockedAchievements = new List<string>();

            foreach (var achievement in achievements)
            {
                if (achievement != null && achievement.CheckUnlock(result))
                {
                    unlockedAchievements.Add(achievement.AchievementId);
                    OnAchievementUnlocked?.Invoke(achievement.AchievementId);
                }
            }

            return unlockedAchievements;
        }

        public void TriggerEnding(string rankId)
        {
            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.TriggerEnding($"Director_{rankId}");
            }
            OnRankEndingAchieved?.Invoke(rankId);
        }

        public RankEndingBase GetRankEnding(string rankId)
        {
            foreach (var ending in rankEndings)
            {
                if (ending != null && ending.RankId == rankId)
                {
                    return ending;
                }
            }
            return null;
        }

        public DirectorAchievement GetAchievement(string achievementId)
        {
            foreach (var achievement in achievements)
            {
                if (achievement != null && achievement.AchievementId == achievementId)
                {
                    return achievement;
                }
            }
            return null;
        }

        public int GetCurrentHeat() => totalHeat;
        public int GetCurrentReputation() => totalReputation;
        public int GetCurrentAudience() => totalAudience;
        public int GetCPCount() => createdCPs.Count;
        public int GetStarCount() => createdStars.Count;
        public int GetCrisisCount() => triggeredCrises.Count;
    }

    public class DirectorGameState
    {
        public int CurrentEpisode;
        public int TotalEpisodes;
        public float CurrentHeat;
        public float CurrentReputation;
        public float CurrentAudience;
        public List<string> ActiveGuests;
        public List<string> CompletedEvents;

        public DirectorGameState()
        {
            ActiveGuests = new List<string>();
            CompletedEvents = new List<string>();
        }
    }

    public class DirectorEndingResult
    {
        public string RankId;
        public string RankName;
        public int FinalHeat;
        public int FinalReputation;
        public int FinalAudience;
        public bool HasCrisis;
        public int CrisisCount;
        public int CPCount;
        public int StarCount;
        public int TopicCount;
        public List<string> Achievements;
    }
}
