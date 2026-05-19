using UnityEngine;
using System;

namespace 糟糕是心动鸭
{
    /// <summary>
    /// 玩家数据结构 - 用于存储游戏中的玩家/角色状态数据
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [SerializeField]
        private string characterId;

        /// <summary>
        /// 好感度值 (0-100)
        /// </summary>
        [SerializeField]
        private int affection;

        /// <summary>
        /// 当前关系状态
        /// </summary>
        [SerializeField]
        private RelationshipStatus relationshipStatus;

        /// <summary>
        /// 是否被选中
        /// </summary>
        [SerializeField]
        private bool isSelected;

        /// <summary>
        /// 本回合互动次数
        /// </summary>
        [SerializeField]
        private int roundInteractionCount;

        /// <summary>
        /// 累计心动次数
        /// </summary>
        [SerializeField]
        private int totalHeartbeatCount;

        /// <summary>
        /// 最后互动时间
        /// </summary>
        [SerializeField]
        private DateTime lastInteractionTime;

        /// <summary>
        /// 特殊标记列表
        /// </summary>
        [SerializeField]
        private List<string> tags = new List<string>();

        /// <summary>
        /// 收到的礼物列表
        /// </summary>
        [SerializeField]
        private List<string> receivedGifts = new List<string>();

        /// <summary>
        /// 参与的秘密任务列表
        /// </summary>
        [SerializeField]
        private List<string> completedSecretTasks = new List<string>();

        #region 属性访问器

        public string CharacterId
        {
            get => characterId;
            set => characterId = value;
        }

        public int Affection
        {
            get => affection;
            set => affection = Mathf.Clamp(value, 0, Constants.MAX_AFFECTION);
        }

        public RelationshipStatus RelationshipStatus
        {
            get => relationshipStatus;
            set => relationshipStatus = value;
        }

        public bool IsSelected
        {
            get => isSelected;
            set => isSelected = value;
        }

        public int RoundInteractionCount
        {
            get => roundInteractionCount;
            set => roundInteractionCount = Mathf.Max(0, value);
        }

        public int TotalHeartbeatCount
        {
            get => totalHeartbeatCount;
            set => totalHeartbeatCount = Mathf.Max(0, value);
        }

        public DateTime LastInteractionTime
        {
            get => lastInteractionTime;
            set => lastInteractionTime = value;
        }

        public List<string> Tags
        {
            get => tags;
            set => tags = value;
        }

        public List<string> ReceivedGifts
        {
            get => receivedGifts;
            set => receivedGifts = value;
        }

        public List<string> CompletedSecretTasks
        {
            get => completedSecretTasks;
            set => completedSecretTasks = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PlayerData()
        {
            characterId = "";
            affection = Constants.INITIAL_AFFECTION;
            relationshipStatus = RelationshipStatus.Stranger;
            isSelected = false;
            roundInteractionCount = 0;
            totalHeartbeatCount = 0;
            lastInteractionTime = DateTime.Now;
            tags = new List<string>();
            receivedGifts = new List<string>();
            completedSecretTasks = new List<string>();
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="characterId">角色ID</param>
        public PlayerData(string characterId)
        {
            this.characterId = characterId;
            affection = Constants.INITIAL_AFFECTION;
            relationshipStatus = RelationshipStatus.Stranger;
            isSelected = false;
            roundInteractionCount = 0;
            totalHeartbeatCount = 0;
            lastInteractionTime = DateTime.Now;
            tags = new List<string>();
            receivedGifts = new List<string>();
            completedSecretTasks = new List<string>();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 增加好感度
        /// </summary>
        /// <param name="amount">增加量</param>
        public void AddAffection(int amount)
        {
            int oldAffection = affection;
            affection = Mathf.Clamp(affection + amount, 0, Constants.MAX_AFFECTION);

            if (oldAffection < Constants.HEARTBEAT_THRESHOLD && affection >= Constants.HEARTBEAT_THRESHOLD)
            {
                totalHeartbeatCount++;
            }
        }

        /// <summary>
        /// 减少好感度
        /// </summary>
        /// <param name="amount">减少量</param>
        public void ReduceAffection(int amount)
        {
            affection = Mathf.Clamp(affection - amount, 0, Constants.MAX_AFFECTION);
        }

        /// <summary>
        /// 增加互动次数
        /// </summary>
        public void IncrementInteraction()
        {
            roundInteractionCount++;
            lastInteractionTime = DateTime.Now;
        }

        /// <summary>
        /// 重置回合互动次数
        /// </summary>
        public void ResetRoundInteraction()
        {
            roundInteractionCount = 0;
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void AddTag(string tag)
        {
            if (!tags.Contains(tag))
            {
                tags.Add(tag);
            }
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void RemoveTag(string tag)
        {
            tags.Remove(tag);
        }

        /// <summary>
        /// 添加礼物
        /// </summary>
        /// <param name="giftId">礼物ID</param>
        public void AddGift(string giftId)
        {
            if (!receivedGifts.Contains(giftId))
            {
                receivedGifts.Add(giftId);
            }
        }

        /// <summary>
        /// 完成秘密任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        public void CompleteSecretTask(string taskId)
        {
            if (!completedSecretTasks.Contains(taskId))
            {
                completedSecretTasks.Add(taskId);
            }
        }

        /// <summary>
        /// 是否已完成指定秘密任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否完成</returns>
        public bool HasCompletedSecretTask(string taskId)
        {
            return completedSecretTasks.Contains(taskId);
        }

        /// <summary>
        /// 是否收到过指定礼物
        /// </summary>
        /// <param name="giftId">礼物ID</param>
        /// <returns>是否收到</returns>
        public bool HasReceivedGift(string giftId)
        {
            return receivedGifts.Contains(giftId);
        }

        /// <summary>
        /// 是否心动
        /// </summary>
        /// <returns>是否达到心动阈值</returns>
        public bool IsHeartbeat()
        {
            return affection >= Constants.HEARTBEAT_THRESHOLD;
        }

        /// <summary>
        /// 是否可以告白成功
        /// </summary>
        /// <returns>是否达到告白成功阈值</returns>
        public bool CanConfessSuccessfully()
        {
            return affection >= Constants.CONFESSION_SUCCESS_THRESHOLD;
        }

        /// <summary>
        /// 获取好感度百分比
        /// </summary>
        /// <returns>好感度百分比 (0-100)</returns>
        public float GetAffectionPercentage()
        {
            return (float)affection / Constants.MAX_AFFECTION * 100f;
        }

        /// <summary>
        /// 获取关系状态描述
        /// </summary>
        /// <returns>关系状态中文描述</returns>
        public string GetRelationshipDescription()
        {
            return relationshipStatus switch
            {
                RelationshipStatus.Stranger => "陌生",
                RelationshipStatus.Acquaintance => "认识",
                RelationshipStatus.Friend => "朋友",
                RelationshipStatus.Good Impression => "好感",
                RelationshipStatus.Heartbeat => "心动的信号",
                RelationshipStatus.Lover => "恋人",
                _ => "未知"
            };
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        /// <returns>复制的新对象</returns>
        public PlayerData Clone()
        {
            PlayerData clone = new PlayerData
            {
                characterId = this.characterId,
                affection = this.affection,
                relationshipStatus = this.relationshipStatus,
                isSelected = this.isSelected,
                roundInteractionCount = this.roundInteractionCount,
                totalHeartbeatCount = this.totalHeartbeatCount,
                lastInteractionTime = this.lastInteractionTime,
                tags = new List<string>(this.tags),
                receivedGifts = new List<string>(this.receivedGifts),
                completedSecretTasks = new List<string>(this.completedSecretTasks)
            };
            return clone;
        }

        #endregion
    }
}
