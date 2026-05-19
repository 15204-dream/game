using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 嘉宾模式数据管理器 - 管理嘉宾模式的所有数据操作
    /// </summary>
    public class GuestDataManager
    {
        private DataManager dataManager;

        public GuestDataManager(DataManager dm)
        {
            dataManager = dm;
        }

        #region 玩家数据管理

        /// <summary>
        /// 更新玩家状态
        /// </summary>
        public void UpdatePlayerStatus(int health, int energy, int mood)
        {
            var player = dataManager.guestPlayerData;
            player.health = Mathf.Clamp(health, 0, player.maxHealth);
            player.energy = Mathf.Clamp(energy, 0, player.maxEnergy);
            player.mood = Mathf.Clamp(mood, 0, player.maxMood);
        }

        /// <summary>
        /// 消耗精力
        /// </summary>
        public bool ConsumeEnergy(int amount)
        {
            if (dataManager.guestPlayerData.energy >= amount)
            {
                dataManager.guestPlayerData.energy -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 恢复精力
        /// </summary>
        public void RestoreEnergy(int amount)
        {
            dataManager.guestPlayerData.energy = Mathf.Min(
                dataManager.guestPlayerData.energy + amount,
                dataManager.guestPlayerData.maxEnergy);
        }

        /// <summary>
        /// 消耗货币
        /// </summary>
        public bool ConsumeCoins(int amount)
        {
            if (dataManager.guestPlayerData.coins >= amount)
            {
                dataManager.guestPlayerData.coins -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 增加能力经验
        /// </summary>
        public void AddAbilityExperience(string abilityType, int exp)
        {
            var abilities = dataManager.guestPlayerData.abilities;
            if (!abilities.skillExperience.ContainsKey(abilityType))
            {
                abilities.skillExperience[abilityType] = 0;
            }

            abilities.skillExperience[abilityType] += exp;

            int newLevel = PlayerAbilityCalculator.CalculateAbilityLevel(abilities.skillExperience[abilityType]);

            if (!abilities.skillLevel.ContainsKey(abilityType))
            {
                abilities.skillLevel[abilityType] = 1;
            }

            if (newLevel > abilities.skillLevel[abilityType])
            {
                abilities.skillLevel[abilityType] = newLevel;
                OnAbilityLevelUp?.Invoke(abilityType, newLevel);
            }
        }

        /// <summary>
        /// 能力等级提升事件
        /// </summary>
        public event Action<string, int> OnAbilityLevelUp;

        #endregion

        #region 好感度管理

        /// <summary>
        /// 初始化所有角色好感度
        /// </summary>
        public void InitializeAllRelationships()
        {
            dataManager.friendshipData.allRelationships.Clear();

            foreach (var character in dataManager.sharedData.allCharacters)
            {
                if (character.characterId != dataManager.guestPlayerData.playerId)
                {
                    var status = new RelationshipStatus
                    {
                        characterId = character.characterId,
                        affectionPoints = 0,
                        trustPoints = 0,
                        intimacyPoints = 0,
                        friendshipLevel = FriendshipLevel.Stranger,
                        currentRelationshipType = RelationshipType.None,
                        lastInteractionTime = DateTime.Now,
                        interactionCount = 0
                    };
                    dataManager.friendshipData.allRelationships[character.characterId] = status;
                }
            }
        }

        /// <summary>
        /// 增加好感度
        /// </summary>
        public void AddAffection(string characterId, int affectionDelta, int trustDelta, int intimacyDelta, ChangeReason reason)
        {
            if (!dataManager.friendshipData.allRelationships.ContainsKey(characterId))
            {
                InitializeAllRelationships();
            }

            var status = dataManager.friendshipData.allRelationships[characterId];
            int oldAffection = status.affectionPoints;

            status.affectionPoints += affectionDelta;
            status.trustPoints = Mathf.Max(0, status.trustPoints + trustDelta);
            status.intimacyPoints = Mathf.Max(0, status.intimacyPoints + intimacyDelta);
            status.lastInteractionTime = DateTime.Now;
            status.interactionCount++;

            status.friendshipLevel = FriendshipHelper.GetFriendshipLevel(
                status.affectionPoints,
                dataManager.friendshipData.config);

            var record = new AffectionChangeRecord
            {
                recordId = Guid.NewGuid().ToString(),
                fromCharacterId = dataManager.guestPlayerData.playerId,
                toCharacterId = characterId,
                affectionDelta = affectionDelta,
                trustDelta = trustDelta,
                intimacyDelta = intimacyDelta,
                reason = reason,
                timestamp = DateTime.Now
            };
            dataManager.friendshipData.affectionHistory.Add(record);

            if (affectionDelta > 0)
            {
                OnAffectionIncreased?.Invoke(characterId, affectionDelta, status.friendshipLevel);
            }
            else if (affectionDelta < 0)
            {
                OnAffectionDecreased?.Invoke(characterId, affectionDelta);
            }
        }

        /// <summary>
        /// 获取与某角色的好感度信息
        /// </summary>
        public RelationshipStatus GetRelationship(string characterId)
        {
            if (dataManager.friendshipData.allRelationships.ContainsKey(characterId))
            {
                return dataManager.friendshipData.allRelationships[characterId];
            }
            return null;
        }

        /// <summary>
        /// 获取所有好友列表
        /// </summary>
        public List<RelationshipStatus> GetAllFriendships()
        {
            return new List<RelationshipStatus>(dataManager.friendshipData.allRelationships.Values);
        }

        /// <summary>
        /// 获取好感度排名
        /// </summary>
        public List<RelationshipStatus> GetRelationshipRanking()
        {
            var list = new List<RelationshipStatus>(dataManager.friendshipData.allRelationships.Values);
            list.Sort((a, b) => b.affectionPoints.CompareTo(a.affectionPoints));
            return list;
        }

        /// <summary>
        /// 检查是否可以告白
        /// </summary>
        public bool CanConfess(string characterId)
        {
            var status = GetRelationship(characterId);
            if (status == null) return false;
            return FriendshipHelper.CanConfess(status, dataManager.friendshipData.config);
        }

        /// <summary>
        /// 执行告白
        /// </summary>
        public ConfessionResult Confess(string characterId)
        {
            var status = GetRelationship(characterId);
            if (status == null)
            {
                return new ConfessionResult { success = false, message = "关系不存在" };
            }

            if (!CanConfess(characterId))
            {
                return new ConfessionResult { success = false, message = "好感度不足或关系未达到要求" };
            }

            float successChance = FriendshipHelper.GetConfessionSuccessChance(
                status,
                dataManager.guestPlayerData.abilities.romanticSkill);

            bool success = UnityEngine.Random.value <= successChance;

            status.hasConfessed = true;
            dataManager.guestPlayerData.confessionsAttempted++;

            if (success)
            {
                status.confessionAccepted = true;
                status.currentRelationshipType = RelationshipType.Dating;
                status.friendshipLevel = FriendshipLevel.Dating;
                dataManager.guestPlayerData.confessionsSuccessful++;
                dataManager.guestPlayerData.currentCrush = characterId;

                OnConfessionSuccess?.Invoke(characterId);

                return new ConfessionResult
                {
                    success = true,
                    message = "告白成功！",
                    newLevel = status.friendshipLevel
                };
            }
            else
            {
                OnConfessionFailed?.Invoke(characterId);

                return new ConfessionResult
                {
                    success = false,
                    message = "告白失败了...",
                    newLevel = status.friendshipLevel
                };
            }
        }

        /// <summary>
        /// 告白结果
        /// </summary>
        public struct ConfessionResult
        {
            public bool success;
            public string message;
            public FriendshipLevel newLevel;
        }

        /// <summary>
        /// 好感度增加事件
        /// </summary>
        public event Action<string, int, FriendshipLevel> OnAffectionIncreased;

        /// <summary>
        /// 好感度减少事件
        /// </summary>
        public event Action<string, int> OnAffectionDecreased;

        /// <summary>
        /// 告白成功事件
        /// </summary>
        public event Action<string> OnConfessionSuccess;

        /// <summary>
        /// 告白失败事件
        /// </summary>
        public event Action<string> OnConfessionFailed;

        #endregion

        #region 任务管理

        /// <summary>
        /// 开始任务
        /// </summary>
        public bool StartTask(string taskId)
        {
            var task = dataManager.taskData.allTasks.Find(t => t.taskId == taskId);
            if (task == null || task.status != TaskStatus.Available)
            {
                return false;
            }

            if (!GuestTaskHelper.CanStartTask(task, dataManager.guestPlayerData, dataManager.friendshipData))
            {
                return false;
            }

            task.status = TaskStatus.InProgress;
            task.startTime = DateTime.Now;
            dataManager.taskData.availableTasks.Remove(task);

            OnTaskStarted?.Invoke(task);
            return true;
        }

        /// <summary>
        /// 更新任务进度
        /// </summary>
        public void UpdateTaskProgress(string taskId, string objectiveId, int newCount)
        {
            var task = dataManager.taskData.allTasks.Find(t => t.taskId == taskId);
            if (task == null || task.status != TaskStatus.InProgress)
            {
                return;
            }

            var objective = task.objectives.Find(o => o.objectiveId == objectiveId);
            if (objective != null)
            {
                objective.currentCount = Mathf.Min(newCount, objective.targetCount);
                task.completionPercentage = GuestTaskHelper.CalculateTaskProgress(task);

                OnTaskProgressUpdated?.Invoke(taskId, task.completionPercentage);

                if (task.completionPercentage >= 1f)
                {
                    CompleteTask(taskId);
                }
            }
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        public void CompleteTask(string taskId)
        {
            var task = dataManager.taskData.allTasks.Find(t => t.taskId == taskId);
            if (task == null) return;

            task.status = TaskStatus.Completed;
            task.completionPercentage = 1f;

            if (!dataManager.taskData.completedTasks.Contains(task))
            {
                dataManager.taskData.completedTasks.Add(task);
            }

            dataManager.taskData.availableTasks.Remove(task);
            dataManager.taskData.totalTasksCompleted++;

            if (task.reward != null)
            {
                GrantTaskReward(task.reward);
            }

            if (task.bonusReward != null && task.bonusRewardClaimed)
            {
                GrantTaskReward(task.bonusReward);
            }

            OnTaskCompleted?.Invoke(task);
        }

        /// <summary>
        /// 授予任务奖励
        /// </summary>
        private void GrantTaskReward(TaskReward reward)
        {
            dataManager.guestPlayerData.coins += reward.coins;
            dataManager.guestPlayerData.gems += reward.gems;
            dataManager.guestPlayerData.popularity += reward.popularity;

            AddAbilityExperience("romanticSkill", reward.experience);

            if (reward.targetCharacterId != null && reward.affectionPoints > 0)
            {
                AddAffection(reward.targetCharacterId, reward.affectionPoints, 0, 0, ChangeReason.StoryProgress);
            }

            foreach (var item in reward.items)
            {
                GuestInventoryHelper.AddItemToInventory(dataManager.inventoryData, item.itemId, item.quantity);
            }
        }

        /// <summary>
        /// 获取当前任务
        /// </summary>
        public List<TaskProgress> GetCurrentTasks()
        {
            return dataManager.taskData.allTasks.FindAll(t => t.status == TaskStatus.InProgress);
        }

        /// <summary>
        /// 任务开始事件
        /// </summary>
        public event Action<TaskProgress> OnTaskStarted;

        /// <summary>
        /// 任务进度更新事件
        /// </summary>
        public event Action<string, float> OnTaskProgressUpdated;

        /// <summary>
        /// 任务完成事件
        /// </summary>
        public event Action<TaskProgress> OnTaskCompleted;

        #endregion

        #region 背包管理

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        public bool AddItem(string itemId, int quantity = 1)
        {
            return GuestInventoryHelper.AddItemToInventory(dataManager.inventoryData, itemId, quantity);
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        public bool UseItem(string itemId)
        {
            var slot = dataManager.inventoryData.inventorySlots.Find(s => s.itemId == itemId && !s.isEmpty);
            if (slot == null || slot.quantity <= 0)
            {
                return false;
            }

            GuestInventoryHelper.RemoveItemFromInventory(dataManager.inventoryData, itemId, 1);
            OnItemUsed?.Invoke(itemId);
            return true;
        }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        public int GetItemCount(string itemId)
        {
            return GuestInventoryHelper.GetItemCount(dataManager.inventoryData, itemId);
        }

        /// <summary>
        /// 物品使用事件
        /// </summary>
        public event Action<string> OnItemUsed;

        #endregion

        #region 日期管理

        /// <summary>
        /// 记录约会
        /// </summary>
        public void RecordDate(string partnerId, string sceneId, DateQuality quality, int moodChange, int relationshipChange)
        {
            var entry = new DateHistoryEntry
            {
                entryId = Guid.NewGuid().ToString(),
                dateTime = DateTime.Now,
                partnerId = partnerId,
                sceneId = sceneId,
                quality = quality,
                moodChange = moodChange,
                relationshipChange = relationshipChange
            };

            dataManager.guestPlayerData.dateHistory.Add(entry);
            dataManager.guestPlayerData.totalDates++;

            if (quality >= DateQuality.Good)
            {
                dataManager.guestPlayerData.successfulDates++;
            }
            else
            {
                dataManager.guestPlayerData.failedDates++;
            }

            UpdatePlayerStatus(
                dataManager.guestPlayerData.health,
                dataManager.guestPlayerData.energy - 20,
                dataManager.guestPlayerData.mood + moodChange);

            AddAffection(partnerId, relationshipChange, relationshipChange / 2, relationshipChange / 3, ChangeReason.Date);
        }

        #endregion
    }
}
