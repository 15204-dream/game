using System;
using UnityEngine;
using UnityEngine.UI;

namespace HeartHook.Game.Ending
{
    [CreateAssetMenu(fileName = "NewEnding", menuName = "HeartHook/结局数据")]
    public class EndingData : ScriptableObject
    {
        [Header("基础信息")]
        public string Id;
        public string EndingName;
        public string Description;
        public EndingType Type;
        public EndingCategory Category;

        [Header("解锁条件")]
        public EndingConditionBase UnlockCondition;
        public bool IsSecret;

        [Header("显示信息")]
        [TextArea(2, 4)]
        public string Summary;
        public Sprite BackgroundImage;
        public Sprite CharacterImage;
        public Color TitleColor = Color.white;
        public AudioClip BGM;

        [Header("奖励")]
        public int DiamondReward;
        public string UnlockHint;

        [Header("内部状态")]
        [HideInInspector] public bool IsUnlocked;
        [HideInInspector] public bool IsViewed;

        public string GetDisplayName()
        {
            return string.IsNullOrEmpty(EndingName) ? Id : EndingName;
        }

        public string GetCategoryPrefix()
        {
            return Category switch
            {
                EndingCategory.Main => "主线",
                EndingCategory.Character => "角色",
                EndingCategory.Secret => "彩蛋",
                EndingCategory.Director => "导演",
                _ => ""
            };
        }

        public bool CanBePreviewed()
        {
            return UnlockCondition != null && UnlockCondition.CanBePreviewed();
        }

        public void MarkAsViewed()
        {
            IsViewed = true;
        }

        public void ResetViewed()
        {
            IsViewed = false;
        }

        public string GetUnlockConditionText()
        {
            return UnlockCondition?.GetDescription() ?? "无解锁条件";
        }
    }

    public enum EndingType
    {
        GoodEnding,
        NormalEnding,
        BadEnding,
        SecretEnding
    }

    public enum EndingCategory
    {
        Main,
        Character,
        Secret,
        Director
    }

    [Serializable]
    public abstract class EndingConditionBase : SerializableCallback<bool>
    {
        public abstract string GetDescription();
        public abstract bool CanUnlock(EndingUnlocker unlocker);
        public abstract bool CanBePreviewed();
    }
}
