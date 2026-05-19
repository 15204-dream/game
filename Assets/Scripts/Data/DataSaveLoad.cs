using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 数据保存和加载系统
    /// </summary>
    public class DataSaveLoad
    {
        private const string SAVE_FOLDER = "Saves";
        private const string SAVE_EXTENSION = ".json";
        private const int MAX_SAVE_SLOTS = 10;

        private string saveDirectory;

        public DataSaveLoad()
        {
            saveDirectory = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
        }

        #region 保存功能

        /// <summary>
        /// 保存游戏数据
        /// </summary>
        public bool SaveGame(DataManager dataManager, string saveName)
        {
            try
            {
                SaveData saveData = CreateSaveData(dataManager);
                string json = JsonUtility.ToJson(saveData, true);

                string filePath = GetSaveFilePath(saveName);
                File.WriteAllText(filePath, json, Encoding.UTF8);

                SaveMetadata(saveName, saveData);

                if (dataManager.debugMode)
                {
                    Debug.Log($"游戏已保存: {saveName}");
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"保存失败: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 创建保存数据
        /// </summary>
        private SaveData CreateSaveData(DataManager dataManager)
        {
            SaveData data = new SaveData
            {
                saveVersion = "1.0",
                saveTime = DateTime.Now,
                sharedData = dataManager.sharedData,
                sceneData = dataManager.sceneData,
                timeData = dataManager.timeData,
                guestPlayerData = dataManager.guestPlayerData,
                friendshipData = dataManager.friendshipData,
                taskData = dataManager.taskData,
                inventoryData = dataManager.inventoryData,
                directorPlayerData = dataManager.directorPlayerData,
                heatData = dataManager.heatData,
                actionPointData = dataManager.actionPointData,
                topicLibrary = dataManager.topicLibrary,
                crisisData = dataManager.crisisData,
                currentMode = dataManager.currentMode,
                playTimeSeconds = (long)(DateTime.Now - dataManager.sharedData.gameStartTime).TotalSeconds
            };

            return data;
        }

        /// <summary>
        /// 保存元数据
        /// </summary>
        private void SaveMetadata(string saveName, SaveData saveData)
        {
            string metaPath = GetMetadataFilePath(saveName);
            SaveMetadata metadata = new SaveMetadata
            {
                saveName = saveName,
                saveTime = DateTime.Now,
                gameMode = saveData.currentMode,
                currentEpisode = saveData.sharedData.currentEpisode,
                dayNumber = saveData.timeData.gameTime.dayNumber,
                playTimeSeconds = saveData.playTimeSeconds
            };

            string json = JsonUtility.ToJson(metadata);
            File.WriteAllText(metaPath, json, Encoding.UTF8);
        }

        #endregion

        #region 加载功能

        /// <summary>
        /// 加载游戏数据
        /// </summary>
        public bool LoadGame(DataManager dataManager, string saveName)
        {
            try
            {
                string filePath = GetSaveFilePath(saveName);
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"存档不存在: {saveName}");
                    return false;
                }

                string json = File.ReadAllText(filePath, Encoding.UTF8);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData == null)
                {
                    Debug.LogError("存档数据格式错误");
                    return false;
                }

                ApplySaveData(dataManager, saveData);

                if (dataManager.debugMode)
                {
                    Debug.Log($"游戏已加载: {saveName}");
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"加载失败: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 应用保存数据
        /// </summary>
        private void ApplySaveData(DataManager dataManager, SaveData saveData)
        {
            dataManager.sharedData = saveData.sharedData ?? new SharedGameData();
            dataManager.sceneData = saveData.sceneData ?? new SceneData();
            dataManager.timeData = saveData.timeData ?? new TimeSystemData();

            dataManager.guestPlayerData = saveData.guestPlayerData ?? new GuestPlayerData();
            dataManager.friendshipData = saveData.friendshipData ?? new FriendshipData();
            dataManager.taskData = saveData.taskData ?? new GuestTaskData();
            dataManager.inventoryData = saveData.inventoryData ?? new GuestInventoryData();

            dataManager.directorPlayerData = saveData.directorPlayerData ?? new DirectorPlayerData();
            dataManager.heatData = saveData.heatData ?? new HeatLevelData();
            dataManager.actionPointData = saveData.actionPointData ?? new ActionPointData();
            dataManager.topicLibrary = saveData.topicLibrary ?? new TopicLibrary();
            dataManager.crisisData = saveData.crisisData ?? new CrisisRecordData();

            dataManager.currentMode = saveData.currentMode;
            dataManager.lastUpdateTime = DateTime.Now;
        }

        #endregion

        #region 存档管理

        /// <summary>
        /// 获取存档文件列表
        /// </summary>
        public List<SaveFileInfo> GetSaveFileList()
        {
            List<SaveFileInfo> saveList = new List<SaveFileInfo>();

            if (!Directory.Exists(saveDirectory))
            {
                return saveList;
            }

            string[] metaFiles = Directory.GetFiles(saveDirectory, "*.meta");
            foreach (string metaFile in metaFiles)
            {
                try
                {
                    string json = File.ReadAllText(metaFile, Encoding.UTF8);
                    SaveMetadata metadata = JsonUtility.FromJson<SaveMetadata>(json);

                    if (metadata != null)
                    {
                        SaveFileInfo info = new SaveFileInfo
                        {
                            saveName = metadata.saveName,
                            saveTime = metadata.saveTime,
                            mode = metadata.gameMode,
                            currentEpisode = metadata.currentEpisode,
                            dayNumber = metadata.dayNumber,
                            playTime = TimeSpan.FromSeconds(metadata.playTimeSeconds),
                            fileSize = GetFileSize(GetSaveFilePath(metadata.saveName))
                        };
                        saveList.Add(info);
                    }
                }
                catch
                {
                    continue;
                }
            }

            saveList.Sort((a, b) => b.saveTime.CompareTo(a.saveTime));
            return saveList;
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public bool DeleteSave(string saveName)
        {
            try
            {
                string savePath = GetSaveFilePath(saveName);
                string metaPath = GetMetadataFilePath(saveName);

                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                }

                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"删除存档失败: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 检查存档是否存在
        /// </summary>
        public bool SaveExists(string saveName)
        {
            return File.Exists(GetSaveFilePath(saveName));
        }

        /// <summary>
        /// 获取存档数量
        /// </summary>
        public int GetSaveCount()
        {
            return GetSaveFileList().Count;
        }

        /// <summary>
        /// 复制存档
        /// </summary>
        public bool CopySave(string sourceName, string destName)
        {
            try
            {
                string sourcePath = GetSaveFilePath(sourceName);
                string destPath = GetSaveFilePath(destName);

                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, destPath, true);

                    string sourceMeta = GetMetadataFilePath(sourceName);
                    string destMeta = GetMetadataFilePath(destName);
                    if (File.Exists(sourceMeta))
                    {
                        File.Copy(sourceMeta, destMeta, true);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"复制存档失败: {e.Message}");
                return false;
            }
        }

        #endregion

        #region 导出导入

        /// <summary>
        /// 导出数据为JSON
        /// </summary>
        public string ExportToJson(DataManager dataManager)
        {
            SaveData saveData = CreateSaveData(dataManager);
            return JsonUtility.ToJson(saveData, true);
        }

        /// <summary>
        /// 从JSON导入数据
        /// </summary>
        public bool ImportFromJson(DataManager dataManager, string json)
        {
            try
            {
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                if (saveData == null)
                {
                    return false;
                }

                ApplySaveData(dataManager, saveData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 导出到文件
        /// </summary>
        public bool ExportToFile(DataManager dataManager, string filePath)
        {
            try
            {
                string json = ExportToJson(dataManager);
                File.WriteAllText(filePath, json, Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从文件导入
        /// </summary>
        public bool ImportFromFile(DataManager dataManager, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                string json = File.ReadAllText(filePath, Encoding.UTF8);
                return ImportFromJson(dataManager, json);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 工具方法

        private string GetSaveFilePath(string saveName)
        {
            string safeName = GetSafeFileName(saveName);
            return Path.Combine(saveDirectory, safeName + SAVE_EXTENSION);
        }

        private string GetMetadataFilePath(string saveName)
        {
            string safeName = GetSafeFileName(saveName);
            return Path.Combine(saveDirectory, safeName + ".meta" + SAVE_EXTENSION);
        }

        private string GetSafeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        private int GetFileSize(string filePath)
        {
            if (File.Exists(filePath))
            {
                return (int)new FileInfo(filePath).Length;
            }
            return 0;
        }

        #endregion
    }

    /// <summary>
    /// 保存数据结构
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public string saveVersion;
        public DateTime saveTime;
        public long playTimeSeconds;

        public SharedGameData sharedData;
        public SceneData sceneData;
        public TimeSystemData timeData;

        public GuestPlayerData guestPlayerData;
        public FriendshipData friendshipData;
        public GuestTaskData taskData;
        public GuestInventoryData inventoryData;

        public DirectorPlayerData directorPlayerData;
        public HeatLevelData heatData;
        public ActionPointData actionPointData;
        public TopicLibrary topicLibrary;
        public CrisisRecordData crisisData;

        public GameMode currentMode;
    }

    /// <summary>
    /// 保存元数据
    /// </summary>
    [System.Serializable]
    public class SaveMetadata
    {
        public string saveName;
        public DateTime saveTime;
        public GameMode gameMode;
        public int currentEpisode;
        public int dayNumber;
        public long playTimeSeconds;
    }

    /// <summary>
    /// 云同步状态
    /// </summary>
    public enum CloudSyncStatus
    {
        NotSynced,
        Syncing,
        Synced,
        Error
    }

    /// <summary>
    /// 云同步管理器
    /// </summary>
    public class CloudSyncManager
    {
        private CloudSyncStatus status = CloudSyncStatus.NotSynced;
        private DateTime? lastSyncTime;

        public CloudSyncStatus Status => status;
        public DateTime? LastSyncTime => lastSyncTime;

        public void SyncToCloud(SaveData data)
        {
            status = CloudSyncStatus.Syncing;
        }

        public SaveData LoadFromCloud()
        {
            status = CloudSyncStatus.Syncing;
            return null;
        }

        public void OnSyncComplete(bool success)
        {
            status = success ? CloudSyncStatus.Synced : CloudSyncStatus.Error;
            if (success)
            {
                lastSyncTime = DateTime.Now;
            }
        }
    }
}
