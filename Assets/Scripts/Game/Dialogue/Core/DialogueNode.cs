using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue
{
    /// <summary>
    /// 对话节点数据类
    /// 表示对话树中的一个节点，包含角色发言和选项
    /// </summary>
    [Serializable]
    public class DialogueNode
    {
        /// <summary>
        /// 节点唯一标识符
        /// </summary>
        private string nodeId;

        /// <summary>
        /// 发言角色ID
        /// </summary>
        private string speakerId;

        /// <summary>
        /// 发言角色名称
        /// </summary>
        private string speakerName;

        /// <summary>
        /// 对话内容文本
        /// </summary>
        private string content;

        /// <summary>
        /// 发言时的表情
        /// </summary>
        private string emotion;

        /// <summary>
        /// 发言时的动作/动画
        /// </summary>
        private string action;

        /// <summary>
        /// 对话选项列表
        /// </summary>
        private List<DialogueOption> options;

        /// <summary>
        /// 条件满足时的下一节点ID
        /// </summary>
        private string defaultNextNodeId;

        /// <summary>
        /// 节点类型
        /// </summary>
        private DialogueNodeType nodeType;

        /// <summary>
        /// 进入节点的条件
        /// </summary>
        private string entryCondition;

        /// <summary>
        /// 节点优先级（用于分支选择）
        /// </summary>
        private int priority;

        /// <summary>
        /// 是否为对话结束节点
        /// </summary>
        private bool isEndingNode;

        /// <summary>
        /// 结束类型（成功、失败、普通）
        /// </summary>
        private DialogueEndingType endingType;

        /// <summary>
        /// 触发的事件ID列表
        /// </summary>
        private List<string> triggerEventIds;

        /// <summary>
        /// 需要的上下文标记
        /// </summary>
        private List<string> requiredContextTags;

        /// <summary>
        /// 节点分组（用于批量操作）
        /// </summary>
        private string groupId;

        /// <summary>
        /// 节点标签（用于快速查找）
        /// </summary>
        private List<string> nodeTags;

        /// <summary>
        /// 是否为自动播放节点（无选项）
        /// </summary>
        private bool isAutoPlay;

        /// <summary>
        /// 自动播放延迟时间
        /// </summary>
        private float autoPlayDelay;

        #region 属性访问器

        public string NodeId
        {
            get => nodeId;
            set => nodeId = value;
        }

        public string SpeakerId
        {
            get => speakerId;
            set => speakerId = value;
        }

        public string SpeakerName
        {
            get => speakerName;
            set => speakerName = value;
        }

        public string Content
        {
            get => content;
            set => content = value;
        }

        public string Emotion
        {
            get => emotion;
            set => emotion = value;
        }

        public string Action
        {
            get => action;
            set => action = value;
        }

        public List<DialogueOption> Options
        {
            get => options;
            set => options = value;
        }

        public string DefaultNextNodeId
        {
            get => defaultNextNodeId;
            set => defaultNextNodeId = value;
        }

        public DialogueNodeType NodeType
        {
            get => nodeType;
            set => nodeType = value;
        }

        public string EntryCondition
        {
            get => entryCondition;
            set => entryCondition = value;
        }

        public int Priority
        {
            get => priority;
            set => priority = Mathf.Clamp(value, 0, 100);
        }

        public bool IsEndingNode
        {
            get => isEndingNode;
            set => isEndingNode = value;
        }

        public DialogueEndingType EndingType
        {
            get => endingType;
            set => endingType = value;
        }

        public List<string> TriggerEventIds
        {
            get => triggerEventIds;
            set => triggerEventIds = value;
        }

        public List<string> RequiredContextTags
        {
            get => requiredContextTags;
            set => requiredContextTags = value;
        }

        public string GroupId
        {
            get => groupId;
            set => groupId = value;
        }

        public List<string> NodeTags
        {
            get => nodeTags;
            set => nodeTags = value;
        }

        public bool IsAutoPlay
        {
            get => isAutoPlay;
            set => isAutoPlay = value;
        }

        public float AutoPlayDelay
        {
            get => autoPlayDelay;
            set => autoPlayDelay = Mathf.Max(0f, value);
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DialogueNode()
        {
            nodeId = "";
            speakerId = "";
            speakerName = "";
            content = "";
            emotion = "Normal";
            action = "";
            options = new List<DialogueOption>();
            defaultNextNodeId = "";
            nodeType = DialogueNodeType.Speech;
            entryCondition = "";
            priority = 50;
            isEndingNode = false;
            endingType = DialogueEndingType.Normal;
            triggerEventIds = new List<string>();
            requiredContextTags = new List<string>();
            groupId = "";
            nodeTags = new List<string>();
            isAutoPlay = false;
            autoPlayDelay = 0f;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="speaker">发言角色</param>
        /// <param name="dialogueContent">对话内容</param>
        public DialogueNode(string id, string speaker, string dialogueContent)
        {
            nodeId = id;
            speakerId = speaker;
            speakerName = speaker;
            content = dialogueContent;
            emotion = "Normal";
            action = "";
            options = new List<DialogueOption>();
            defaultNextNodeId = "";
            nodeType = DialogueNodeType.Speech;
            entryCondition = "";
            priority = 50;
            isEndingNode = false;
            endingType = DialogueEndingType.Normal;
            triggerEventIds = new List<string>();
            requiredContextTags = new List<string>();
            groupId = "";
            nodeTags = new List<string>();
            isAutoPlay = false;
            autoPlayDelay = 0f;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 添加对话选项
        /// </summary>
        /// <param name="option">选项</param>
        public void AddOption(DialogueOption option)
        {
            options.Add(option);
        }

        /// <summary>
        /// 移除对话选项
        /// </summary>
        /// <param name="optionId">选项ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveOption(string optionId)
        {
            return options.RemoveAll(o => o.OptionId == optionId) > 0;
        }

        /// <summary>
        /// 获取可用选项
        /// </summary>
        /// <param name="currentAffection">当前好感度</param>
        /// <param name="hasItemFunc">检查是否有道具的函数</param>
        /// <returns>可用选项列表</returns>
        public List<DialogueOption> GetAvailableOptions(int currentAffection, Func<string, bool> hasItemFunc = null)
        {
            List<DialogueOption> available = new List<DialogueOption>();
            foreach (var option in options)
            {
                bool hasItem = hasItemFunc == null ? true : hasItemFunc(option.RequiredItemId);
                if (option.IsAvailable(currentAffection, hasItem))
                {
                    available.Add(option);
                }
            }
            return available;
        }

        /// <summary>
        /// 检查节点条件是否满足
        /// </summary>
        /// <param name="context">上下文数据</param>
        /// <returns>条件是否满足</returns>
        public bool CheckEntryCondition(DialogueContext context)
        {
            if (string.IsNullOrEmpty(entryCondition)) return true;

            foreach (var tag in requiredContextTags)
            {
                if (!context.HasTag(tag))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 触发节点事件
        /// </summary>
        /// <param name="eventManager">事件管理器</param>
        public void TriggerEvents(Action<string> eventManager)
        {
            if (eventManager != null)
            {
                foreach (var eventId in triggerEventIds)
                {
                    eventManager(eventId);
                }
            }
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void AddTag(string tag)
        {
            if (!nodeTags.Contains(tag))
            {
                nodeTags.Add(tag);
            }
        }

        /// <summary>
        /// 检查是否有标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>是否存在</returns>
        public bool HasTag(string tag)
        {
            return nodeTags.Contains(tag);
        }

        /// <summary>
        /// 获取心动选项
        /// </summary>
        /// <returns>心动选项列表</returns>
        public List<DialogueOption> GetHeartOptions()
        {
            return options.FindAll(o => o.IsHeartChoice);
        }

        /// <summary>
        /// 是否需要玩家输入
        /// </summary>
        /// <returns>是否需要输入</returns>
        public bool RequiresPlayerInput()
        {
            return options.Count > 0 && !isAutoPlay;
        }

        /// <summary>
        /// 清除所有选项
        /// </summary>
        public void ClearOptions()
        {
            options.Clear();
        }

        /// <summary>
        /// 获取节点描述
        /// </summary>
        /// <returns>节点描述字符串</returns>
        public string GetNodeDescription()
        {
            return $"[{nodeId}] {speakerName}: {content.Substring(0, System.Math.Min(30, content.Length))}...";
        }

        #endregion

        #region Unity特定

        private static class Mathf
        {
            public static int Clamp(int value, int min, int max)
            {
                return System.Math.Max(min, System.Math.Min(max, value));
            }

            public static float Max(float a, float b)
            {
                return System.Math.Max(a, b);
            }
        }

        #endregion
    }

    /// <summary>
    /// 对话节点类型枚举
    /// </summary>
    public enum DialogueNodeType
    {
        /// <summary>
        /// 角色发言
        /// </summary>
        Speech,

        /// <summary>
        /// 玩家选择
        /// </summary>
        PlayerChoice,

        /// <summary>
        /// 旁白/叙述
        /// </summary>
        Narration,

        /// <summary>
        /// 场景描述
        /// </summary>
        SceneDescription,

        /// <summary>
        /// 事件触发
        /// </summary>
        EventTrigger,

        /// <summary>
        /// 条件分支
        /// </summary>
        ConditionalBranch
    }

    /// <summary>
    /// 对话结束类型枚举
    /// </summary>
    public enum DialogueEndingType
    {
        /// <summary>
        /// 普通结束
        /// </summary>
        Normal,

        /// <summary>
        /// 成功结束
        /// </summary>
        Success,

        /// <summary>
        /// 失败结束
        /// </summary>
        Failure,

        /// <summary>
        /// 特殊结束
        /// </summary>
        Special
    }
}
