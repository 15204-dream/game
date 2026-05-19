using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 主数据管理器 - 管理所有游戏数据
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        private static DataManager instance;
        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DataManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("DataManager");
                        instance = obj.AddComponent<DataManager>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return instance;
            }
        }

        [Header("共享数据")]
        public SharedGameData sharedData = new SharedGameData();
        public SceneData sceneData = new SceneData();
        public TimeSystemData timeData = new TimeSystemData();

        [Header("嘉宾模式数据")]
        public GuestPlayerData guestPlayerData;
        public FriendshipData friendshipData = new FriendshipData();
        public GuestTaskData taskData = new GuestTaskData();
        public GuestInventoryData inventoryData = new GuestInventoryData();

        [Header("导演模式数据")]
        public DirectorPlayerData directorPlayerData;
        public HeatLevelData heatData = new HeatLevelData();
        public ActionPointData actionPointData = new ActionPointData();
        public TopicLibrary topicLibrary = new TopicLibrary();
        public CrisisRecordData crisisData = new CrisisRecordData();

        [Header("系统设置")]
        public bool autoSave = true;
        public int autoSaveInterval = 300;
        public bool debugMode = false;

        [Header("游戏状态")]
        public GameMode currentMode;
        public bool isPaused;
        public DateTime lastUpdateTime;

        private DataSaveLoad saveLoadSystem;
        private GuestDataManager guestDataManager;
        private DirectorDataManager directorDataManager;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeManagers();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            lastUpdateTime = DateTime.Now;
            if (autoSave)
            {
                InvokeRepeating(nameof(AutoSave), autoSaveInterval, autoSaveInterval);
            }
        }

        void Update()
        {
            lastUpdateTime = DateTime.Now;
        }

        /// <summary>
        /// 初始化各个管理器
        /// </summary>
        private void InitializeManagers()
        {
            saveLoadSystem = new DataSaveLoad();
            guestDataManager = new GuestDataManager(this);
            directorDataManager = new DirectorDataManager(this);

            if (guestPlayerData == null)
            {
                guestPlayerData = new GuestPlayerData();
            }

            if (directorPlayerData == null)
            {
                directorPlayerData = new DirectorPlayerData();
            }
        }

        /// <summary>
        /// 切换游戏模式
        /// </summary>
        public void SwitchGameMode(GameMode mode)
        {
            if (currentMode == mode) return;

            GameMode previousMode = currentMode;
            currentMode = mode;
            sharedData.currentMode = mode;

            if (debugMode)
            {
                Debug.Log($"游戏模式从 {previousMode} 切换到 {mode}");
            }

            OnModeSwitched?.Invoke(previousMode, mode);
        }

        /// <summary>
        /// 自动保存
        /// </summary>
        private void AutoSave()
        {
            if (!isPaused && autoSave)
            {
                SaveGame("autosave");
            }
        }

        /// <summary>
        /// 保存游戏
        /// </summary>
        public bool SaveGame(string saveName)
        {
            try
            {
                sharedData.lastSaveTime = DateTime.Now;
                return saveLoadSystem.SaveGame(this, saveName);
            }
            catch (Exception e)
            {
                Debug.LogError($"保存游戏失败: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 加载游戏
        /// </summary>
        public bool LoadGame(string saveName)
        {
            try
            {
                return saveLoadSystem.LoadGame(this, saveName);
            }
            catch (Exception e)
            {
                Debug.LogError($"加载游戏失败: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 创建新游戏
        /// </summary>
        public void NewGame()
        {
            sharedData = new SharedGameData();
            sceneData = new SceneData();
            timeData = new TimeSystemData();

            guestPlayerData = new GuestPlayerData();
            friendshipData = new FriendshipData();
            taskData = new GuestTaskData();
            inventoryData = new GuestInventoryData();

            directorPlayerData = new DirectorPlayerData();
            heatData = new HeatLevelData();
            actionPointData = new ActionPointData();
            crisisData = new CrisisRecordData();

            sharedData.gameStartTime = DateTime.Now;
            timeData.gameTime.dayNumber = 1;
            timeData.gameTime.currentPeriod = TimePeriod.Morning;

            OnNewGameCreated?.Invoke();
        }

        /// <summary>
        /// 获取嘉宾数据管理器
        /// </summary>
        public GuestDataManager GetGuestDataManager()
        {
            return guestDataManager;
        }

        /// <summary>
        /// 获取导演数据管理器
        /// </summary>
        public DirectorDataManager GetDirectorDataManager()
        {
            return directorDataManager;
        }

        /// <summary>
        /// 获取存档列表
        /// </summary>
        public List<SaveFileInfo> GetSaveFileList()
        {
            return saveLoadSystem.GetSaveFileList();
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public bool DeleteSave(string saveName)
        {
            return saveLoadSystem.DeleteSave(saveName);
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        public string ExportData()
        {
            return saveLoadSystem.ExportToJson(this);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public bool ImportData(string jsonData)
        {
            return saveLoadSystem.ImportFromJson(this, jsonData);
        }

        /// <summary>
        /// 重置所有数据
        /// </summary>
        public void ResetAllData()
        {
            NewGame();
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// 获取游戏时长
        /// </summary>
        public TimeSpan GetPlayTime()
        {
            return DateTime.Now - sharedData.gameStartTime;
        }

        /// <summary>
        /// 验证数据完整性
        /// </summary>
        public List<string> ValidateData()
        {
            List<string> errors = new List<string>();

            if (sharedData.allCharacters.Count < 12)
            {
                errors.Add("嘉宾数量不足12人");
            }

            if (sharedData.currentEpisode > sharedData.totalEpisodes)
            {
                errors.Add("当前集数超过总集数");
            }

            if (sharedData.currentEpisode < 1)
            {
                errors.Add("当前集数小于1");
            }

            return errors;
        }

        /// <summary>
        /// 获取角色数据
        /// </summary>
        public CharacterBasicInfo GetCharacterById(string characterId)
        {
            return sharedData.allCharacters.Find(c => c.characterId == characterId);
        }

        /// <summary>
        /// 获取角色状态
        /// </summary>
        public CharacterStatusData GetCharacterStatus(string characterId)
        {
            if (sharedData.characterStatus.TryGetValue(characterId, out var status))
            {
                return status;
            }
            return null;
        }

        /// <summary>
        /// 设置游戏暂停状态
        /// </summary>
        public void SetPause(bool paused)
        {
            isPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
            OnGamePaused?.Invoke(paused);
        }

        /// <summary>
        /// 游戏暂停事件
        /// </summary>
        public event Action<bool> OnGamePaused;

        /// <summary>
        /// 模式切换事件
        /// </summary>
        public event Action<GameMode, GameMode> OnModeSwitched;

        /// <summary>
        /// 新游戏创建事件
        /// </summary>
        public event Action OnNewGameCreated;
    }

    /// <summary>
    /// 存档文件信息
    /// </summary>
    [System.Serializable]
    public class SaveFileInfo
    {
        public string saveName;
        public DateTime saveTime;
        public GameMode mode;
        public int currentEpisode;
        public int dayNumber;
        public TimeSpan playTime;
        public string thumbnailPath;
        public int fileSize;
    }
}
