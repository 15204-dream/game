using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue
{
    /// <summary>
    /// 对话选项数据类
    /// 存储玩家可以选择的对话选项
    /// </summary>
    [Serializable]
    public class DialogueOption
    {
        /// <summary>
        /// 选项唯一标识符
        /// </summary>
        private string optionId;

        /// <summary>
        /// 选项文本内容
        /// </summary>
        private string text;

        /// <summary>
        /// 选项描述（悬停时显示）
        /// </summary>
        private string description;

        /// <summary>
        /// 关联的下一个对话节点ID
        /// </summary>
        private string nextNodeId;

        /// <summary>
        /// 选项类型（普通、特殊、隐藏等）
        /// </summary>
        private DialogueOptionType optionType;

        /// <summary>
        /// 选择该选项所需的好感度阈值
        /// </summary>
        private int affectionRequired;

        /// <summary>
        /// 选择该选项后的好感度变化
        /// </summary>
        private int affectionChange;

        /// <summary>
        /// 选择该选项后的情感变化
        /// </summary>
        private float emotionChange;

        /// <summary>
        /// 是否需要特定道具
        /// </summary>
        private bool requiresItem;

        /// <summary>
        /// 所需道具ID
        /// </summary>
        private string requiredItemId;

        /// <summary>
        /// 是否可重复选择
        /// </summary>
        private bool repeatable;

        /// <summary>
        /// 选择次数限制
        /// </summary>
        private int maxSelections;

        /// <summary>
        /// 当前选择次数
        /// </summary>
        private int currentSelections;

        /// <summary>
        /// 是否解锁
        /// </summary>
        private bool isUnlocked;

        /// <summary>
        /// 解锁条件描述
        /// </summary>
        private string unlockCondition;

        /// <summary>
        /// 是否为心动选项
        /// </summary>
        private bool isHeartChoice;

        /// <summary>
        /// 特殊效果标记列表
        /// </summary>
        private List<string> effectTags;

        #region 属性访问器

        public string OptionId
        {
            get => optionId;
            set => optionId = value;
        }

        public string Text
        {
            get => text;
            set => text = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public string NextNodeId
        {
            get => nextNodeId;
            set => nextNodeId = value;
        }

        public DialogueOptionType OptionType
        {
            get => optionType;
            set => optionType = value;
        }

        public int AffectionRequired
        {
            get => affectionRequired;
            set => affectionRequired = Mathf.Clamp(value, 0, 100);
        }

        public int AffectionChange
        {
            get => affectionChange;
            set => affectionChange = value;
        }

        public float EmotionChange
        {
            get => emotionChange;
            set => emotionChange = value;
        }

        public bool RequiresItem
        {
            get => requiresItem;
            set => requiresItem = value;
        }

        public string RequiredItemId
        {
            get => requiredItemId;
            set => requiredItemId = value;
        }

        public bool Repeatable
        {
            get => repeatable;
            set => repeatable = value;
        }

        public int MaxSelections
        {
            get => maxSelections;
            set => maxSelections = Mathf.Max(0, value);
        }

        public int CurrentSelections
        {
            get => currentSelections;
            set => currentSelections = Mathf.Max(0, value);
        }

        public bool IsUnlocked
        {
            get => isUnlocked;
            set => isUnlocked = value;
        }

        public string UnlockCondition
        {
            get => unlockCondition;
            set => unlockCondition = value;
        }

        public bool IsHeartChoice
        {
            get => isHeartChoice;
            set => isHeartChoice = value;
        }

        public List<string> EffectTags
        {
            get => effectTags;
            set => effectTags = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DialogueOption()
        {
            optionId = "";
            text = "";
            description = "";
            nextNodeId = "";
            optionType = DialogueOptionType.Normal;
            affectionRequired = 0;
            affectionChange = 0;
            emotionChange = 0f;
            requiresItem = false;
            requiredItemId = "";
            repeatable = false;
            maxSelections = 1;
            currentSelections = 0;
            isUnlocked = true;
            unlockCondition = "";
            isHeartChoice = false;
            effectTags = new List<string>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">选项ID</param>
        /// <param name="optionText">选项文本</param>
        /// <param name="next">下一节点ID</param>
        public DialogueOption(string id, string optionText, string next)
        {
            optionId = id;
            text = optionText;
            description = "";
            nextNodeId = next;
            optionType = DialogueOptionType.Normal;
            affectionRequired = 0;
            affectionChange = 0;
            emotionChange = 0f;
            requiresItem = false;
            requiredItemId = "";
            repeatable = false;
            maxSelections = 1;
            currentSelections = 0;
            isUnlocked = true;
            unlockCondition = "";
            isHeartChoice = false;
            effectTags = new List<string>();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查选项是否可用
        /// </summary>
        /// <param name="currentAffection">当前好感度</param>
        /// <param name="hasRequiredItem">是否有所需道具</param>
        /// <returns>是否可用</returns>
        public bool IsAvailable(int currentAffection, bool hasRequiredItem = true)
        {
            if (!isUnlocked) return false;
            if (currentAffection < affectionRequired) return false;
            if (requiresItem && !hasRequiredItem) return false;
            if (!repeatable && currentSelections >= maxSelections) return false;
            return true;
        }

        /// <summary>
        /// 使用选项
        /// </summary>
        public void UseOption()
        {
            currentSelections++;
        }

        /// <summary>
        /// 重置选项使用次数
        /// </summary>
        public void ResetSelections()
        {
            currentSelections = 0;
        }

        /// <summary>
        /// 解锁选项
        /// </summary>
        public void Unlock()
        {
            isUnlocked = true;
        }

        /// <summary>
        /// 锁定选项
        /// </summary>
        public void Lock()
        {
            isUnlocked = false;
        }

        /// <summary>
        /// 添加效果标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void AddEffectTag(string tag)
        {
            if (!effectTags.Contains(tag))
            {
                effectTags.Add(tag);
            }
        }

        /// <summary>
        /// 检查是否有指定标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>是否存在</returns>
        public bool HasEffectTag(string tag)
        {
            return effectTags.Contains(tag);
        }

        /// <summary>
        /// 获取选项信息
        /// </summary>
        /// <returns>选项信息字符串</returns>
        public string GetOptionInfo()
        {
            return $"[{optionId}] {text} (好感度要求: {affectionRequired}, 好感度变化: {affectionChange})";
        }

        #endregion

        #region Unity特定

        private static class Mathf
        {
            public static int Clamp(int value, int min, int max)
            {
                return System.Math.Max(min, System.Math.Min(max, value));
            }

            public static float Clamp(float value, float min, float max)
            {
                return System.Math.Max(min, System.Math.Min(max, value));
            }

            public static int Max(int a, int b)
            {
                return System.Math.Max(a, b);
            }
        }

        #endregion
    }

    /// <summary>
    /// 对话选项类型枚举
    /// </summary>
    public enum DialogueOptionType
    {
        /// <summary>
        /// 普通选项
        /// </summary>
        Normal,

        /// <summary>
        /// 特殊选项
        /// </summary>
        Special,

        /// <summary>
        /// 隐藏选项
        /// </summary>
        Hidden,

        /// <summary>
        /// 心动选项
        /// </summary>
        Heart,

        /// <summary>
        /// 危险选项
        /// </summary>
        Dangerous,

        /// <summary>
        /// 秘密任务选项
        /// </summary>
        SecretTask
    }
}
