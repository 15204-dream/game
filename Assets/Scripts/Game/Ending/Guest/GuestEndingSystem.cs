using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeartHook.Game.Ending.Guest
{
    public class GuestEndingSystem : MonoBehaviour
    {
        public static GuestEndingSystem Instance { get; private set; }

        [SerializeField] private GuestEndingData endingData;
        [SerializeField] private MainEndingHandler[] mainEndingHandlers;
        [SerializeField] private CharacterEndingHandler[] characterEndingHandlers;
        [SerializeField] private SecretEndingHandler secretEndingHandler;

        private EndingUnlocker currentUnlocker;
        private GuestGameState currentGameState;

        public event Action<string> OnEndingTriggered;
        public event Action<GuestEndingResult> OnEndingCalculated;

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
            if (mainEndingHandlers == null)
            {
                mainEndingHandlers = GetComponentsInChildren<MainEndingHandler>();
            }
            if (characterEndingHandlers == null)
            {
                characterEndingHandlers = GetComponentsInChildren<CharacterEndingHandler>();
            }
            if (secretEndingHandler == null)
            {
                secretEndingHandler = GetComponent<SecretEndingHandler>();
            }
        }

        public void InitializeGameState()
        {
            currentUnlocker = new EndingUnlocker();
            currentGameState = new GuestGameState();
        }

        public void UpdateFavorability(string characterId, int value)
        {
            if (currentUnlocker == null)
                currentUnlocker = new EndingUnlocker();

            currentUnlocker.SetFavorability(characterId, value);
        }

        public void RecordMutualChoice(string characterId)
        {
            currentUnlocker?.AddMutualChoice(characterId);
        }

        public void RecordTaskCompletion(string taskId)
        {
            currentUnlocker?.CompleteTask(taskId);
        }

        public void RecordEvent(string eventId)
        {
            currentUnlocker?.TriggerEvent(eventId);
        }

        public void SetCurrentDay(int day)
        {
            currentUnlocker?.SetCurrentDay(day);
        }

        public void SetPlayerQuit(bool value)
        {
            currentUnlocker?.SetPlayerQuit(value);
        }

        public void SetNoHearts(bool value)
        {
            currentUnlocker?.SetNoHearts(value);
        }

        public GuestEndingResult CalculateEnding()
        {
            if (currentUnlocker == null)
            {
                Debug.LogWarning("[GuestEndingSystem] 解锁器未初始化！");
                return null;
            }

            var result = new GuestEndingResult();

            var maxFavorability = currentUnlocker.GetMaxFavorability();
            var hasMutualChoice = currentUnlocker.GetMutualChoiceCount() > 0;
            var allInRange = currentUnlocker.AllFavorabilityInRange(50, 70);
            var playerQuit = currentUnlocker.HasPlayerQuit();
            var noHearts = currentUnlocker.HasNoHearts();
            var completedTasks = currentUnlocker.GetCompletedTaskCount();

            if (maxFavorability >= 95 && hasMutualChoice)
            {
                result.EndingId = "HE01";
                result.EndingName = "心动终章";
                result.EndingType = EndingType.GoodEnding;
            }
            else if (maxFavorability >= 90 && completedTasks > 0)
            {
                result.EndingId = "HE02";
                result.EndingName = "双向奔赴";
                result.EndingType = EndingType.GoodEnding;
            }
            else if (allInRange && noHearts)
            {
                result.EndingId = "HE03";
                result.EndingName = "友情万岁";
                result.EndingType = EndingType.GoodEnding;
            }
            else if (playerQuit)
            {
                result.EndingId = "HE04";
                result.EndingName = "独自美丽";
                result.EndingType = EndingType.NormalEnding;
            }
            else if (hasMutualChoice && maxFavorability >= 80)
            {
                result.EndingId = "NE01";
                result.EndingName = "意难平";
                result.EndingType = EndingType.NormalEnding;
            }
            else
            {
                result.EndingId = "NE02";
                result.EndingName = "擦肩而过";
                result.EndingType = EndingType.BadEnding;
            }

            result.HighestCharacter = currentUnlocker.GetHighestFavorabilityCharacter();
            result.MaxFavorability = maxFavorability;
            result.HasMutualChoice = hasMutualChoice;
            result.CompletedTaskCount = completedTasks;

            OnEndingCalculated?.Invoke(result);
            return result;
        }

        public string CalculateCharacterEnding()
        {
            if (currentUnlocker == null)
                return null;

            var highestCharacter = currentUnlocker.GetHighestFavorabilityCharacter();
            if (string.IsNullOrEmpty(highestCharacter))
                return null;

            var favorability = currentUnlocker.GetFavorability(highestCharacter);
            if (favorability < 70)
                return null;

            var characterIdToEnding = new Dictionary<string, string>
            {
                { "LengLeng", "CE_LengLeng" },
                { "SenSen", "CE_SenSen" },
                { "ChenGe", "CE_ChenGe" },
                { "XiaoJi", "CE_XiaoJi" },
                { "ZhouZhou", "CE_ZhouZhou" },
                { "YanShen", "CE_YanShen" },
                { "NianNian", "CE_NianNian" },
                { "WanNing", "CE_WanNing" },
                { "ZhiYi", "CE_ZhiYi" },
                { "XingChen", "CE_XingChen" },
                { "YouWei", "CE_YouWei" },
                { "JiuJiu", "CE_JiuJiu" }
            };

            if (characterIdToEnding.TryGetValue(highestCharacter, out var endingId))
            {
                return endingId;
            }

            return null;
        }

        public string CalculateSecretEnding()
        {
            if (currentUnlocker == null)
                return null;

            var allFavorability = currentUnlocker.GetAllFavorability();
            int countAbove80 = 0;
            int countAbove60 = 0;

            foreach (var fav in allFavorability.Values)
            {
                if (fav >= 80) countAbove80++;
                if (fav >= 60) countAbove60++;
            }

            if (countAbove80 >= 4)
            {
                return "SE01";
            }

            if (countAbove60 >= 6)
            {
                return "SE02";
            }

            var maxFav = currentUnlocker.GetMaxFavorability();
            if (maxFav >= 50 && currentUnlocker.GetCurrentDay() >= 14)
            {
                return "SE03";
            }

            if (currentUnlocker.IsEventTriggered("Special_Confession") &&
                currentUnlocker.IsEventTriggered("Special_FourthWall"))
            {
                return "SE04";
            }

            return null;
        }

        public void TriggerEnding(string endingId)
        {
            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.TriggerEnding(endingId);
            }
            OnEndingTriggered?.Invoke(endingId);
        }

        public MainEndingHandler GetMainEndingHandler(string endingId)
        {
            foreach (var handler in mainEndingHandlers)
            {
                if (handler != null && handler.EndingId == endingId)
                {
                    return handler;
                }
            }
            return null;
        }

        public CharacterEndingHandler GetCharacterEndingHandler(string endingId)
        {
            foreach (var handler in characterEndingHandlers)
            {
                if (handler != null && handler.EndingId == endingId)
                {
                    return handler;
                }
            }
            return null;
        }

        public SecretEndingHandler GetSecretEndingHandler()
        {
            return secretEndingHandler;
        }
    }

    public class GuestGameState
    {
        public int CurrentDay;
        public string CurrentScene;
        public Dictionary<string, int> Favorability;
        public List<string> CompletedTasks;
        public List<string> TriggeredEvents;

        public GuestGameState()
        {
            Favorability = new Dictionary<string, int>();
            CompletedTasks = new List<string>();
            TriggeredEvents = new List<string>();
        }
    }

    public class GuestEndingResult
    {
        public string EndingId;
        public string EndingName;
        public EndingType EndingType;
        public string HighestCharacter;
        public int MaxFavorability;
        public bool HasMutualChoice;
        public int CompletedTaskCount;
        public string CharacterEndingId;
        public string SecretEndingId;
    }
}
