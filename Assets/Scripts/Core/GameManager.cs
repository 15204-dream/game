using UnityEngine;
using System.Collections.Generic;
using System;

namespace 糟糕是心动鸭
{
    /// <summary>
    /// 游戏主管理器 - 单例模式
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region 单例实现

        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        instance = go.AddComponent<GameManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        #endregion

        #region 事件定义

        public event Action<GameMode> OnGameModeChanged;
        public event Action<GameStage> OnGameStageChanged;
        public event Action<int, int> OnRoundChanged;
        public event Action<string, int> OnAffectionChanged;
        public event Action<string> OnCharacterSelected;
        public event Action OnGameSaved;
        public event Action OnGameLoaded;

        #endregion

        #region 核心属性

        [SerializeField]
        private GameConfig gameConfig;

        [SerializeField]
        private GameMode currentGameMode = GameMode.GuestMode;

        [SerializeField]
        private GameStage currentGameStage = GameStage.MainMenu;

        [SerializeField]
        private int currentRound = 0;

        [SerializeField]
        private int totalRounds = Constants.ROUNDS_PER_SHOW;

        private Dictionary<string, PlayerData> playerDataDict = new Dictionary<string, PlayerData>();

        private List<CharacterData> allCharacters = new List<CharacterData>();

        private string currentPlayerId = "";

        private bool isGamePaused = false;

        private bool isInitialized = false;

        #endregion

        #region 属性访问器

        public GameConfig GameConfig
        {
            get => gameConfig;
            set => gameConfig = value;
        }

        public GameMode CurrentGameMode
        {
            get => currentGameMode;
            private set
            {
                if (currentGameMode != value)
                {
                    currentGameMode = value;
                    OnGameModeChanged?.Invoke(value);
                }
            }
        }

        public GameStage CurrentGameStage
        {
            get => currentGameStage;
            private set
            {
                if (currentGameStage != value)
                {
                    currentGameStage = value;
                    OnGameStageChanged?.Invoke(value);
                }
            }
        }

        public int CurrentRound
        {
            get => currentRound;
            private set
            {
                int oldRound = currentRound;
                currentRound = value;
                if (oldRound != value)
                {
                    OnRoundChanged?.Invoke(oldRound, value);
                }
            }
        }

        public int TotalRounds
        {
            get => totalRounds;
            set => totalRounds = Mathf.Max(1, value);
        }

        public string CurrentPlayerId
        {
            get => currentPlayerId;
            private set => currentPlayerId = value;
        }

        public bool IsGamePaused
        {
            get => isGamePaused;
            set
            {
                isGamePaused = value;
                Time.timeScale = value ? 0f : 1f;
            }
        }

        public bool IsInitialized
        {
            get => isInitialized;
        }

        #endregion

        #region Unity生命周期

        private void Awake()
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

        private void Update()
        {
            if (!isInitialized) return;
            
            HandleInput();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化游戏管理器
        /// </summary>
        public void Initialize()
        {
            if (isInitialized) return;

            LoadGameConfig();
            LoadCharacterData();
            
            isInitialized = true;
            Debug.Log($"[{Constants.GAME_NAME}] 游戏管理器初始化完成");
        }

        /// <summary>
        /// 加载游戏配置
        /// </summary>
        private void LoadGameConfig()
        {
            if (gameConfig == null)
            {
                gameConfig = new GameConfig();
            }
        }

        /// <summary>
        /// 加载角色数据
        /// </summary>
        private void LoadCharacterData()
        {
            TextAsset characterDataJson = Resources.Load<TextAsset>(Constants.ResourcePaths.CHARACTER_DATA);
            if (characterDataJson != null)
            {
                try
                {
                    CharacterDataList dataList = JsonUtility.FromJson<CharacterDataList>(characterDataJson.text);
                    if (dataList != null && dataList.characters != null)
                    {
                        allCharacters = new List<CharacterData>(dataList.characters);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"加载角色数据失败: {e.Message}");
                }
            }
        }

        #endregion

        #region 游戏流程控制

        /// <summary>
        /// 开始新游戏
        /// </summary>
        /// <param name="mode">游戏模式</param>
        /// <param name="playerId">玩家角色ID</param>
        public void StartNewGame(GameMode mode, string playerId = "")
        {
            CurrentGameMode = mode;
            CurrentPlayerId = playerId;
            CurrentRound = 0;
            CurrentGameStage = GameStage.CharacterSelection;

            InitializePlayerData();
            
            Debug.Log($"[{Constants.GAME_NAME}] 开始新游戏 - 模式: {mode}");
        }

        /// <summary>
        /// 初始化玩家数据
        /// </summary>
        private void InitializePlayerData()
        {
            playerDataDict.Clear();
            
            foreach (var character in allCharacters)
            {
                PlayerData data = new PlayerData
                {
                    CharacterId = character.Id,
                    Affection = Constants.INITIAL_AFFECTION,
                    RelationshipStatus = RelationshipStatus.Stranger,
                    IsSelected = false,
                    RoundInteractionCount = 0
                };
                playerDataDict[character.Id] = data;
            }

            if (!string.IsNullOrEmpty(currentPlayerId) && playerDataDict.ContainsKey(currentPlayerId))
            {
                playerDataDict[currentPlayerId].IsSelected = true;
            }
        }

        /// <summary>
        /// 选择角色
        /// </summary>
        /// <param name="characterId">角色ID</param>
        public void SelectCharacter(string characterId)
        {
            if (!playerDataDict.ContainsKey(characterId)) return;

            foreach (var kvp in playerDataDict)
            {
                kvp.Value.IsSelected = false;
            }

            playerDataDict[characterId].IsSelected = true;
            currentPlayerId = characterId;
            CurrentGameStage = GameStage.ShowInProgress;

            OnCharacterSelected?.Invoke(characterId);
            Debug.Log($"[{Constants.GAME_NAME}] 选择角色: {characterId}");
        }

        /// <summary>
        /// 进入下一回合
        /// </summary>
        public void NextRound()
        {
            if (currentRound < totalRounds)
            {
                CurrentRound++;
                Debug.Log($"[{Constants.GAME_NAME}] 进入第 {currentRound} 回合");
            }
            else
            {
                EndShow();
            }
        }

        /// <summary>
        /// 结束节目
        /// </summary>
        public void EndShow()
        {
            CurrentGameStage = GameStage.ShowEnd;
            Debug.Log($"[{Constants.GAME_NAME}] 节目结束");
        }

        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void ReturnToMainMenu()
        {
            CurrentGameStage = GameStage.MainMenu;
            currentRound = 0;
            IsGamePaused = false;
            Debug.Log($"[{Constants.GAME_NAME}] 返回主菜单");
        }

        #endregion

        #region 数据操作

        /// <summary>
        /// 获取玩家数据
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>玩家数据</returns>
        public PlayerData GetPlayerData(string characterId)
        {
            if (playerDataDict.ContainsKey(characterId))
            {
                return playerDataDict[characterId];
            }
            return null;
        }

        /// <summary>
        /// 更新好感度
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <param name="affectionChange">好感度变化值</param>
        public void UpdateAffection(string characterId, int affectionChange)
        {
            if (!playerDataDict.ContainsKey(characterId)) return;

            PlayerData data = playerDataDict[characterId];
            int oldAffection = data.Affection;
            data.Affection = Mathf.Clamp(data.Affection + affectionChange, 0, Constants.MAX_AFFECTION);

            UpdateRelationshipStatus(data);

            OnAffectionChanged?.Invoke(characterId, data.Affection - oldAffection);
        }

        /// <summary>
        /// 更新关系状态
        /// </summary>
        private void UpdateRelationshipStatus(PlayerData data)
        {
            RelationshipStatus newStatus;

            if (data.Affection >= Constants.CONFESSION_SUCCESS_THRESHOLD)
            {
                newStatus = RelationshipStatus.Lover;
            }
            else if (data.Affection >= Constants.HEARTBEAT_THRESHOLD)
            {
                newStatus = RelationshipStatus.Heartbeat;
            }
            else if (data.Affection >= 40)
            {
                newStatus = RelationshipStatus.Good Impression;
            }
            else if (data.Affection >= 30)
            {
                newStatus = RelationshipStatus.Friend;
            }
            else if (data.Affection >= 25)
            {
                newStatus = RelationshipStatus.Acquaintance;
            }
            else
            {
                newStatus = RelationshipStatus.Stranger;
            }

            if (data.RelationshipStatus != newStatus)
            {
                data.RelationshipStatus = newStatus;
            }
        }

        /// <summary>
        /// 获取所有角色数据
        /// </summary>
        /// <returns>角色数据列表</returns>
        public List<CharacterData> GetAllCharacters()
        {
            return new List<CharacterData>(allCharacters);
        }

        /// <summary>
        /// 根据ID获取角色数据
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色数据</returns>
        public CharacterData GetCharacterById(string characterId)
        {
            return allCharacters.Find(c => c.Id == characterId);
        }

        /// <summary>
        /// 获取玩家与其他角色的好感度列表
        /// </summary>
        /// <returns>好感度字典</returns>
        public Dictionary<string, int> GetAllAffectionData()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (var kvp in playerDataDict)
            {
                result[kvp.Key] = kvp.Value.Affection;
            }
            return result;
        }

        #endregion

        #region 保存加载

        /// <summary>
        /// 保存游戏
        /// </summary>
        /// <param name="slot">存档槽位</param>
        public void SaveGame(int slot = 0)
        {
            SaveData saveData = new SaveData
            {
                GameMode = currentGameMode,
                GameStage = currentGameStage,
                CurrentRound = currentRound,
                TotalRounds = totalRounds,
                CurrentPlayerId = currentPlayerId,
                PlayerDataList = new List<PlayerData>(playerDataDict.Values),
                GameConfig = gameConfig,
                SaveTime = DateTime.Now
            };

            string json = JsonUtility.ToJson(saveData, true);
            string fileName = string.Format(Constants.SAVE_FILE_FORMAT, slot);
            string fullPath = System.IO.Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_PATH, fileName);

            try
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath));
                System.IO.File.WriteAllText(fullPath, json);
                OnGameSaved?.Invoke();
                Debug.Log($"[{Constants.GAME_NAME}] 游戏已保存到槽位 {slot}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[{Constants.GAME_NAME}] 保存游戏失败: {e.Message}");
            }
        }

        /// <summary>
        /// 加载游戏
        /// </summary>
        /// <param name="slot">存档槽位</param>
        public void LoadGame(int slot = 0)
        {
            string fileName = string.Format(Constants.SAVE_FILE_FORMAT, slot);
            string fullPath = System.IO.Path.Combine(Application.persistentDataPath, Constants.SAVE_DATA_PATH, fileName);

            if (!System.IO.File.Exists(fullPath))
            {
                Debug.LogWarning($"[{Constants.GAME_NAME}] 存档槽位 {slot} 不存在");
                return;
            }

            try
            {
                string json = System.IO.File.ReadAllText(fullPath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData != null)
                {
                    ApplySaveData(saveData);
                    OnGameLoaded?.Invoke();
                    Debug.Log($"[{Constants.GAME_NAME}] 游戏已从槽位 {slot} 加载");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[{Constants.GAME_NAME}] 加载游戏失败: {e.Message}");
            }
        }

        /// <summary>
        /// 应用存档数据
        /// </summary>
        private void ApplySaveData(SaveData saveData)
        {
            currentGameMode = saveData.GameMode;
            currentGameStage = saveData.GameStage;
            currentRound = saveData.CurrentRound;
            totalRounds = saveData.TotalRounds;
            currentPlayerId = saveData.CurrentPlayerId;
            gameConfig = saveData.GameConfig;

            playerDataDict.Clear();
            foreach (var data in saveData.PlayerDataList)
            {
                playerDataDict[data.CharacterId] = data;
            }
        }

        #endregion

        #region 输入处理

        /// <summary>
        /// 处理输入
        /// </summary>
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveGame();
            }
        }

        /// <summary>
        /// 切换暂停状态
        /// </summary>
        public void TogglePause()
        {
            IsGamePaused = !IsGamePaused;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 获取心动角色列表
        /// </summary>
        /// <returns>心动角色ID列表</returns>
        public List<string> GetHeartbeatCharacters()
        {
            List<string> heartbeatList = new List<string>();
            foreach (var kvp in playerDataDict)
            {
                if (kvp.Value.Affection >= Constants.HEARTBEAT_THRESHOLD)
                {
                    heartbeatList.Add(kvp.Key);
                }
            }
            return heartbeatList;
        }

        /// <summary>
        /// 检查游戏是否结束
        /// </summary>
        /// <returns>是否结束</returns>
        public bool IsGameEnded()
        {
            return currentGameStage == GameStage.ShowEnd || 
                   currentGameStage == GameStage.MainMenu;
        }

        #endregion
    }

    #region 保存数据结构

    /// <summary>
    /// 存档数据结构
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public GameMode GameMode;
        public GameStage GameStage;
        public int CurrentRound;
        public int TotalRounds;
        public string CurrentPlayerId;
        public List<PlayerData> PlayerDataList;
        public GameConfig GameConfig;
        public DateTime SaveTime;
    }

    /// <summary>
    /// 角色数据列表（用于JSON序列化）
    /// </summary>
    [System.Serializable]
    public class CharacterDataList
    {
        public List<CharacterData> characters;
    }

    #endregion
}
