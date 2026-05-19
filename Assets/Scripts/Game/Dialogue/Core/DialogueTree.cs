using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue
{
    /// <summary>
    /// 对话树数据类
    /// 包含完整的对话流程和所有节点
    /// </summary>
    [Serializable]
    public class DialogueTree
    {
        /// <summary>
        /// 对话树唯一标识符
        /// </summary>
        private string treeId;

        /// <summary>
        /// 对话树名称
        /// </summary>
        private string treeName;

        /// <summary>
        /// 对话树描述
        /// </summary>
        private string description;

        /// <summary>
        /// 所有对话节点
        /// </summary>
        private Dictionary<string, DialogueNode> nodes;

        /// <summary>
        /// 起始节点ID
        /// </summary>
        private string startNodeId;

        /// <summary>
        /// 当前对话类型
        /// </summary>
        private DialogueTreeType treeType;

        /// <summary>
        /// 对话树版本
        /// </summary>
        private string version;

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime createdAt;

        /// <summary>
        /// 最后修改时间
        /// </summary>
        private DateTime modifiedAt;

        /// <summary>
        /// 关联的角色ID
        /// </summary>
        private string relatedCharacterId;

        /// <summary>
        /// 场景ID
        /// </summary>
        private string sceneId;

        /// <summary>
        /// 所需最低好感度
        /// </summary>
        private int requiredAffection;

        /// <summary>
        /// 是否已解锁
        /// </summary>
        private bool isUnlocked;

        /// <summary>
        /// 是否已完成
        /// </summary>
        private bool isCompleted;

        /// <summary>
        /// 完成次数
        /// </summary>
        private int completionCount;

        /// <summary>
        /// 对话树标签
        /// </summary>
        private List<string> tags;

        /// <summary>
        /// 特殊参数
        /// </summary>
        private Dictionary<string, string> customParameters;

        /// <summary>
        /// 最大对话轮次
        /// </summary>
        private int maxRounds;

        /// <summary>
        /// 是否可重复使用
        /// </summary>
        private bool isRepeatable;

        /// <summary>
        /// 完成后的重置时间（秒）
        /// </summary>
        private float resetTime;

        /// <summary>
        /// 背景音乐ID
        /// </summary>
        private string backgroundMusicId;

        /// <summary>
        /// 背景图片ID
        /// </summary>
        private string backgroundImageId;

        #region 属性访问器

        public string TreeId
        {
            get => treeId;
            set => treeId = value;
        }

        public string TreeName
        {
            get => treeName;
            set => treeName = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public Dictionary<string, DialogueNode> Nodes
        {
            get => nodes;
            set => nodes = value;
        }

        public string StartNodeId
        {
            get => startNodeId;
            set => startNodeId = value;
        }

        public DialogueTreeType TreeType
        {
            get => treeType;
            set => treeType = value;
        }

        public string Version
        {
            get => version;
            set => version = value;
        }

        public DateTime CreatedAt
        {
            get => createdAt;
            set => createdAt = value;
        }

        public DateTime ModifiedAt
        {
            get => modifiedAt;
            set => modifiedAt = value;
        }

        public string RelatedCharacterId
        {
            get => relatedCharacterId;
            set => relatedCharacterId = value;
        }

        public string SceneId
        {
            get => sceneId;
            set => sceneId = value;
        }

        public int RequiredAffection
        {
            get => requiredAffection;
            set => requiredAffection = Mathf.Clamp(value, 0, 100);
        }

        public bool IsUnlocked
        {
            get => isUnlocked;
            set => isUnlocked = value;
        }

        public bool IsCompleted
        {
            get => isCompleted;
            set => isCompleted = value;
        }

        public int CompletionCount
        {
            get => completionCount;
            set => completionCount = Mathf.Max(0, value);
        }

        public List<string> Tags
        {
            get => tags;
            set => tags = value;
        }

        public Dictionary<string, string> CustomParameters
        {
            get => customParameters;
            set => customParameters = value;
        }

        public int MaxRounds
        {
            get => maxRounds;
            set => maxRounds = Mathf.Max(1, value);
        }

        public bool IsRepeatable
        {
            get => isRepeatable;
            set => isRepeatable = value;
        }

        public float ResetTime
        {
            get => resetTime;
            set => resetTime = Mathf.Max(0f, value);
        }

        public string BackgroundMusicId
        {
            get => backgroundMusicId;
            set => backgroundMusicId = value;
        }

        public string BackgroundImageId
        {
            get => backgroundImageId;
            set => backgroundImageId = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DialogueTree()
        {
            treeId = "";
            treeName = "";
            description = "";
            nodes = new Dictionary<string, DialogueNode>();
            startNodeId = "";
            treeType = DialogueTreeType.Normal;
            version = "1.0";
            createdAt = DateTime.Now;
            modifiedAt = DateTime.Now;
            relatedCharacterId = "";
            sceneId = "";
            requiredAffection = 0;
            isUnlocked = false;
            isCompleted = false;
            completionCount = 0;
            tags = new List<string>();
            customParameters = new Dictionary<string, string>();
            maxRounds = 10;
            isRepeatable = false;
            resetTime = 0f;
            backgroundMusicId = "";
            backgroundImageId = "";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">对话树ID</param>
        /// <param name="name">对话树名称</param>
        public DialogueTree(string id, string name)
        {
            treeId = id;
            treeName = name;
            description = "";
            nodes = new Dictionary<string, DialogueNode>();
            startNodeId = "";
            treeType = DialogueTreeType.Normal;
            version = "1.0";
            createdAt = DateTime.Now;
            modifiedAt = DateTime.Now;
            relatedCharacterId = "";
            sceneId = "";
            requiredAffection = 0;
            isUnlocked = false;
            isCompleted = false;
            completionCount = 0;
            tags = new List<string>();
            customParameters = new Dictionary<string, string>();
            maxRounds = 10;
            isRepeatable = false;
            resetTime = 0f;
            backgroundMusicId = "";
            backgroundImageId = "";
        }

        #endregion

        #region 节点管理

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>是否成功添加</returns>
        public bool AddNode(DialogueNode node)
        {
            if (nodes.ContainsKey(node.NodeId))
            {
                return false;
            }
            nodes[node.NodeId] = node;
            modifiedAt = DateTime.Now;
            return true;
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveNode(string nodeId)
        {
            if (nodes.Remove(nodeId))
            {
                modifiedAt = DateTime.Now;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>节点或null</returns>
        public DialogueNode GetNode(string nodeId)
        {
            return nodes.ContainsKey(nodeId) ? nodes[nodeId] : null;
        }

        /// <summary>
        /// 检查节点是否存在
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>是否存在</returns>
        public bool HasNode(string nodeId)
        {
            return nodes.ContainsKey(nodeId);
        }

        /// <summary>
        /// 获取起始节点
        /// </summary>
        /// <returns>起始节点</returns>
        public DialogueNode GetStartNode()
        {
            return GetNode(startNodeId);
        }

        /// <summary>
        /// 设置起始节点
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        public void SetStartNode(string nodeId)
        {
            if (HasNode(nodeId))
            {
                startNodeId = nodeId;
                modifiedAt = DateTime.Now;
            }
        }

        /// <summary>
        /// 获取所有节点列表
        /// </summary>
        /// <returns>节点列表</returns>
        public List<DialogueNode> GetAllNodes()
        {
            return new List<DialogueNode>(nodes.Values);
        }

        /// <summary>
        /// 获取指定类型的节点
        /// </summary>
        /// <param name="type">节点类型</param>
        /// <returns>节点列表</returns>
        public List<DialogueNode> GetNodesByType(DialogueNodeType type)
        {
            List<DialogueNode> result = new List<DialogueNode>();
            foreach (var node in nodes.Values)
            {
                if (node.NodeType == type)
                {
                    result.Add(node);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取带有指定标签的节点
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>节点列表</returns>
        public List<DialogueNode> GetNodesByTag(string tag)
        {
            List<DialogueNode> result = new List<DialogueNode>();
            foreach (var node in nodes.Values)
            {
                if (node.HasTag(tag))
                {
                    result.Add(node);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取结束节点
        /// </summary>
        /// <returns>结束节点列表</returns>
        public List<DialogueNode> GetEndingNodes()
        {
            List<DialogueNode> endings = new List<DialogueNode>();
            foreach (var node in nodes.Values)
            {
                if (node.IsEndingNode)
                {
                    endings.Add(node);
                }
            }
            return endings;
        }

        #endregion

        #region 对话树操作

        /// <summary>
        /// 标记对话树为已完成
        /// </summary>
        public void MarkAsCompleted()
        {
            isCompleted = true;
            completionCount++;
            modifiedAt = DateTime.Now;
        }

        /// <summary>
        /// 重置对话树状态
        /// </summary>
        public void Reset()
        {
            isCompleted = false;
        }

        /// <summary>
        /// 解锁对话树
        /// </summary>
        public void Unlock()
        {
            isUnlocked = true;
        }

        /// <summary>
        /// 锁定对话树
        /// </summary>
        public void Lock()
        {
            isUnlocked = false;
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
        /// 检查标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>是否存在</returns>
        public bool HasTag(string tag)
        {
            return tags.Contains(tag);
        }

        /// <summary>
        /// 设置自定义参数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetCustomParameter(string key, string value)
        {
            customParameters[key] = value;
        }

        /// <summary>
        /// 获取自定义参数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值或默认值</returns>
        public string GetCustomParameter(string key, string defaultValue = "")
        {
            return customParameters.ContainsKey(key) ? customParameters[key] : defaultValue;
        }

        /// <summary>
        /// 检查是否满足进入条件
        /// </summary>
        /// <param name="currentAffection">当前好感度</param>
        /// <returns>是否满足</returns>
        public bool CanEnter(int currentAffection)
        {
            return isUnlocked && currentAffection >= requiredAffection;
        }

        /// <summary>
        /// 验证对话树完整性
        /// </summary>
        /// <returns>是否有效</returns>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(startNodeId)) return false;
            if (!HasNode(startNodeId)) return false;
            if (nodes.Count == 0) return false;

            foreach (var node in nodes.Values)
            {
                foreach (var option in node.Options)
                {
                    if (!string.IsNullOrEmpty(option.NextNodeId) && !HasNode(option.NextNodeId))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取节点总数
        /// </summary>
        /// <returns>节点数量</returns>
        public int GetNodeCount()
        {
            return nodes.Count;
        }

        #endregion

        #region Unity特定

        private static class Mathf
        {
            public static int Clamp(int value, int min, int max)
            {
                return System.Math.Max(min, System.Math.Min(max, value));
            }

            public static int Max(int a, int b)
            {
                return System.Math.Max(a, b);
            }

            public static float Max(float a, float b)
            {
                return System.Math.Max(a, b);
            }
        }

        #endregion
    }

    /// <summary>
    /// 对话树类型枚举
    /// </summary>
    public enum DialogueTreeType
    {
        /// <summary>
        /// 普通对话
        /// </summary>
        Normal,

        /// <summary>
        /// 初次见面
        /// </summary>
        FirstMeeting,

        /// <summary>
        /// 约会
        /// </summary>
        Date,

        /// <summary>
        /// 任务对话
        /// </summary>
        Task,

        /// <summary>
        /// 秘密任务
        /// </summary>
        SecretTask,

        /// <summary>
        /// 告白
        /// </summary>
        Confession,

        /// <summary>
        /// 事件
        /// </summary>
        Event,

        /// <summary>
        /// 特殊
        /// </summary>
        Special
    }
}
