using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveBeatGuide
{
    /// <summary>
    /// 奖励类型
    /// </summary>
    public enum GuideRewardType
    {
        Item,
        Currency,
        Experience,
        Achievement,
        Unlock
    }

    /// <summary>
    /// 货币类型
    /// </summary>
    public enum CurrencyType
    {
        Hearts,
        Coins,
        Diamonds,
        DirectorsCoins
    }

    /// <summary>
    /// 引导奖励
    /// </summary>
    [Serializable]
    public class GuideReward
    {
        public GuideRewardType rewardType;
        public string itemId;
        public string currencyType;
        public int amount;
        public string achievementId;
        public string unlockId;
        public string description;
    }

    /// <summary>
    /// 奖励授予信息
    /// </summary>
    [Serializable]
    public class RewardGrantInfo
    {
        public GuideReward reward;
        public bool granted;
        public float grantTime;
        public string sequenceId;
        public string stepId;
    }

    /// <summary>
    /// 引导奖励系统
    /// </summary>
    public class GuideRewardSystem : MonoBehaviour
    {
        private static GuideRewardSystem instance;
        public static GuideRewardSystem Instance => instance;

        [Header("奖励配置")]
        [SerializeField] private bool enableRewardPopup = true;
        [SerializeField] private bool enableRewardSound = true;
        [SerializeField] private float popupDisplayTime = 3f;

        [Header("奖励预设")]
        [SerializeField] private List<GuideReward> presetRewards = new List<GuideReward>();

        [Header("奖励记录")]
        [SerializeField] private List<RewardGrantInfo> grantedRewards = new List<RewardGrantInfo>();

        private Queue<GuideReward> pendingRewards = new Queue<GuideReward>();
        private bool isShowingRewardPopup;
        private Dictionary<string, GuideReward> presetRewardDict = new Dictionary<string, GuideReward>();

        public bool IsShowingRewardPopup => isShowingRewardPopup;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            InitializeRewardSystem();
        }

        private void Start()
        {
            LoadRewardHistory();
        }

        /// <summary>
        /// 初始化奖励系统
        /// </summary>
        private void InitializeRewardSystem()
        {
            presetRewardDict.Clear();

            foreach (var reward in presetRewards)
            {
                if (!string.IsNullOrEmpty(reward.itemId))
                {
                    presetRewardDict[reward.itemId] = reward;
                }
            }
        }

        /// <summary>
        /// 授予奖励
        /// </summary>
        public void GrantRewards(List<GuideReward> rewards)
        {
            if (rewards == null || rewards.Count == 0)
                return;

            foreach (var reward in rewards)
            {
                GrantReward(reward);
            }
        }

        /// <summary>
        /// 授予单个奖励
        /// </summary>
        public void GrantReward(GuideReward reward)
        {
            if (reward == null)
                return;

            RewardGrantInfo info = new RewardGrantInfo
            {
                reward = reward,
                granted = false,
                grantTime = Time.time
            };

            switch (reward.rewardType)
            {
                case GuideRewardType.Item:
                    GrantItemReward(reward);
                    break;

                case GuideRewardType.Currency:
                    GrantCurrencyReward(reward);
                    break;

                case GuideRewardType.Experience:
                    GrantExperienceReward(reward);
                    break;

                case GuideRewardType.Achievement:
                    GrantAchievementReward(reward);
                    break;

                case GuideRewardType.Unlock:
                    GrantUnlockReward(reward);
                    break;
            }

            info.granted = true;
            grantedRewards.Add(info);
            SaveRewardHistory();

            if (enableRewardPopup)
            {
                ShowRewardPopup(reward);
            }
        }

        /// <summary>
        /// 授予物品奖励
        /// </summary>
        private void GrantItemReward(GuideReward reward)
        {
            if (string.IsNullOrEmpty(reward.itemId))
                return;

            Debug.Log($"授予物品奖励: {reward.itemId} x {reward.amount}");
        }

        /// <summary>
        /// 授予货币奖励
        /// </summary>
        private void GrantCurrencyReward(GuideReward reward)
        {
            CurrencyType currency;
            if (Enum.TryParse<CurrencyType>(reward.currencyType, out currency))
            {
                Debug.Log($"授予货币奖励: {currency} + {reward.amount}");
            }
        }

        /// <summary>
        /// 授予经验奖励
        /// </summary>
        private void GrantExperienceReward(GuideReward reward)
        {
            Debug.Log($"授予经验奖励: +{reward.amount}");
        }

        /// <summary>
        /// 授予成就奖励
        /// </summary>
        private void GrantAchievementReward(GuideReward reward)
        {
            if (!string.IsNullOrEmpty(reward.achievementId))
            {
                Debug.Log($"授予成就奖励: {reward.achievementId}");
            }
        }

        /// <summary>
        /// 授予解锁奖励
        /// </summary>
        private void GrantUnlockReward(GuideReward reward)
        {
            if (!string.IsNullOrEmpty(reward.unlockId))
            {
                Debug.Log($"授予解锁奖励: {reward.unlockId}");
            }
        }

        /// <summary>
        /// 显示奖励弹窗
        /// </summary>
        private void ShowRewardPopup(GuideReward reward)
        {
            string rewardText = GetRewardText(reward);
            Debug.Log($"显示奖励弹窗: {rewardText}");
        }

        /// <summary>
        /// 获取奖励文本
        /// </summary>
        private string GetRewardText(GuideReward reward)
        {
            switch (reward.rewardType)
            {
                case GuideRewardType.Item:
                    return $"获得物品: {reward.itemId} x{reward.amount}";

                case GuideRewardType.Currency:
                    return $"获得 {GetCurrencyName(reward.currencyType)} x{reward.amount}";

                case GuideRewardType.Experience:
                    return $"获得经验 +{reward.amount}";

                case GuideRewardType.Achievement:
                    return $"解锁成就: {reward.achievementId}";

                case GuideRewardType.Unlock:
                    return $"解锁: {reward.unlockId}";

                default:
                    return "获得奖励";
            }
        }

        /// <summary>
        /// 获取货币名称
        /// </summary>
        private string GetCurrencyName(string currencyType)
        {
            CurrencyType currency;
            if (Enum.TryParse<CurrencyType>(currencyType, out currency))
            {
                switch (currency)
                {
                    case CurrencyType.Hearts:
                        return "心动值";
                    case CurrencyType.Coins:
                        return "金币";
                    case CurrencyType.Diamonds:
                        return "钻石";
                    case CurrencyType.DirectorsCoins:
                        return "导演币";
                }
            }

            return currencyType;
        }

        /// <summary>
        /// 获取已授予的奖励数量
        /// </summary>
        public int GetGrantedRewardCount()
        {
            return grantedRewards.Count;
        }

        /// <summary>
        /// 获取总奖励数量
        /// </summary>
        public int GetTotalRewardAmount(GuideRewardType rewardType)
        {
            int total = 0;
            foreach (var info in grantedRewards)
            {
                if (info.reward.rewardType == rewardType)
                {
                    total += info.reward.amount;
                }
            }
            return total;
        }

        /// <summary>
        /// 检查奖励是否已授予
        /// </summary>
        public bool IsRewardGranted(string itemId)
        {
            foreach (var info in grantedRewards)
            {
                if (info.reward.itemId == itemId)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 保存奖励历史
        /// </summary>
        private void SaveRewardHistory()
        {
            string key = "GuideRewardHistory";
            string json = JsonUtility.ToJson(grantedRewards);
            PlayerPrefs.SetString(key, json);
        }

        /// <summary>
        /// 加载奖励历史
        /// </summary>
        private void LoadRewardHistory()
        {
            string key = "GuideRewardHistory";
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                grantedRewards = JsonUtility.FromJson<List<RewardGrantInfo>>(json);
            }
        }

        /// <summary>
        /// 重置奖励历史
        /// </summary>
        public void ResetRewardHistory()
        {
            grantedRewards.Clear();
            PlayerPrefs.DeleteKey("GuideRewardHistory");
        }

        /// <summary>
        /// 获取奖励历史
        /// </summary>
        public List<RewardGrantInfo> GetRewardHistory()
        {
            return new List<RewardGrantInfo>(grantedRewards);
        }

        /// <summary>
        /// 注册预设奖励
        /// </summary>
        public void RegisterPresetReward(GuideReward reward)
        {
            if (!string.IsNullOrEmpty(reward.itemId))
            {
                presetRewardDict[reward.itemId] = reward;
            }
        }

        /// <summary>
        /// 获取预设奖励
        /// </summary>
        public GuideReward GetPresetReward(string itemId)
        {
            return presetRewardDict.ContainsKey(itemId) ? presetRewardDict[itemId] : null;
        }
    }
}
