using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace 糟糕是心动鸭.Dialogue
{
    /// <summary>
    /// 对话管理器
    /// 管理对话流程的核心控制器
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        private static DialogueManager instance;

        /// <summary>
        /// 当前对话上下文
        /// </summary>
        private DialogueContext currentContext;

        /// <summary>
        /// 当前对话树
        /// </summary>
        private DialogueTree currentTree;

        /// <summary>
        /// 对话树缓存
        /// </summary>
        private Dictionary<string, DialogueTree> dialogueTreeCache;

        /// <summary>
        /// 是否正在对话中
        /// </summary>
        private bool isInDialogue;

        /// <summary>
        /// 是否正在等待AI响应
        /// </summary>
        private bool isWaitingForAI;

        /// <summary>
        /// 对话历史记录
        /// </summary>
        private List<DialogueHistoryEntry> dialogueHistory;

        /// <summary>
        /// API客户端引用
        /// </summary>
        private API.APIClient apiClient;

        /// <summary>
        /// 上下文管理器
        /// </summary>
        private ContextManager contextManager;

        /// <summary>
        /// 内容过滤器
        /// </summary>
        private ContentFilter contentFilter;

        /// <summary>
        /// 对话状态机
        /// </summary>
        private DialogueStateMachine stateMachine;

        /// <summary>
        /// 对话事件
        /// </summary>
        public event Action<DialogueNode> OnNodeEnter;
        public event Action<DialogueNode> OnNodeExit;
        public event Action<DialogueOption> OnOptionSelected;
        public event Action<string> OnDialogueComplete;
        public event Action<string> OnDialogueError;
        public event Action OnAIResponseReceived;

        #region 属性访问器

        public static DialogueManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("DialogueManager");
                    instance = go.AddComponent<DialogueManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        public DialogueContext CurrentContext => currentContext;
        public DialogueTree CurrentTree => currentTree;
        public bool IsInDialogue => isInDialogue;
        public bool IsWaitingForAI => isWaitingForAI;
        public List<DialogueHistoryEntry> DialogueHistory => dialogueHistory;
        public DialogueStateMachine StateMachine => stateMachine;

        #endregion

        #region Unity生命周期

        /// <summary>
        /// Awake方法 - 初始化单例
        /// </summary>
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

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化管理器
        /// </summary>
        private void Initialize()
        {
            dialogueTreeCache = new Dictionary<string, DialogueTree>();
            dialogueHistory = new List<DialogueHistoryEntry>();
            isInDialogue = false;
            isWaitingForAI = false;

            apiClient = API.APIClient.Instance;
            contextManager = new ContextManager();
            contentFilter = new ContentFilter();
            stateMachine = new DialogueStateMachine();

            stateMachine.OnStateChanged += HandleStateChanged;
        }

        #endregion

        #region 对话流程控制

        /// <summary>
        /// 开始对话
        /// </summary>
        /// <param name="treeId">对话树ID</param>
        /// <param name="playerId">玩家ID</param>
        /// <param name="characterId">角色ID</param>
        /// <returns>是否成功开始</returns>
        public async Task<bool> StartDialogue(string treeId, string playerId, string characterId)
        {
            try
            {
                if (isInDialogue)
                {
                    Debug.LogWarning("Dialogue already in progress");
                    return false;
                }

                DialogueTree tree = await LoadDialogueTree(treeId);
                if (tree == null)
                {
                    Debug.LogError($"Failed to load dialogue tree: {treeId}");
                    OnDialogueError?.Invoke($"无法加载对话树: {treeId}");
                    return false;
                }

                currentTree = tree;
                currentContext = new DialogueContext(playerId, characterId)
                {
                    CurrentTreeId = treeId,
                    CurrentSceneId = tree.SceneId
                };

                isInDialogue = true;
                stateMachine.TransitionTo(DialogueState.Starting);

                DialogueNode startNode = tree.GetStartNode();
                if (startNode != null)
                {
                    await EnterNode(startNode);
                    return true;
                }

                Debug.LogError($"No start node found in tree: {treeId}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error starting dialogue: {e.Message}");
                OnDialogueError?.Invoke($"启动对话时出错: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 进入对话节点
        /// </summary>
        /// <param name="node">节点</param>
        private async Task EnterNode(DialogueNode node)
        {
            currentContext.CurrentNodeId = node.NodeId;
            currentContext.AddToHistory(node.SpeakerId, node.Content);

            OnNodeEnter?.Invoke(node);

            if (node.IsEndingNode)
            {
                await HandleEndingNode(node);
                return;
            }

            if (node.IsAutoPlay)
            {
                await Task.Delay((int)(node.AutoPlayDelay * 1000));
                await AdvanceToNextNode(node);
            }
            else
            {
                stateMachine.TransitionTo(DialogueState.WaitingForInput);
            }
        }

        /// <summary>
        /// 选择对话选项
        /// </summary>
        /// <param name="option">选项</param>
        /// <returns>是否成功选择</returns>
        public async Task<bool> SelectOption(DialogueOption option)
        {
            try
            {
                if (!isInDialogue || isWaitingForAI)
                {
                    return false;
                }

                OnOptionSelected?.Invoke(option);

                currentContext.IncreaseAffection(option.AffectionChange);
                currentContext.IncreaseEmotion(option.EmotionChange);
                option.UseOption();

                DialogueNode currentNode = currentTree.GetNode(currentContext.CurrentNodeId);
                if (currentNode != null)
                {
                    await AdvanceToNextNode(currentNode);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error selecting option: {e.Message}");
                OnDialogueError?.Invoke($"选择选项时出错: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 前进到下一节点
        /// </summary>
        /// <param name="currentNode">当前节点</param>
        private async Task AdvanceToNextNode(DialogueNode currentNode)
        {
            string nextNodeId = GetNextNodeId(currentNode);
            if (string.IsNullOrEmpty(nextNodeId))
            {
                await EndDialogue();
                return;
            }

            DialogueNode nextNode = currentTree.GetNode(nextNodeId);
            if (nextNode == null)
            {
                Debug.LogError($"Next node not found: {nextNodeId}");
                await EndDialogue();
                return;
            }

            OnNodeExit?.Invoke(currentNode);
            currentContext.AdvanceRound();
            await EnterNode(nextNode);
        }

        /// <summary>
        /// 获取下一节点ID
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <returns>下一节点ID</returns>
        private string GetNextNodeId(DialogueNode node)
        {
            if (node.Options.Count > 0)
            {
                return node.Options[0].NextNodeId;
            }
            return node.DefaultNextNodeId;
        }

        /// <summary>
        /// 结束对话
        /// </summary>
        private async Task EndDialogue()
        {
            isInDialogue = false;
            stateMachine.TransitionTo(DialogueState.Ending);

            OnDialogueComplete?.Invoke(currentTree.TreeId);

            await Task.Delay(100);
            stateMachine.TransitionTo(DialogueState.Idle);
        }

        /// <summary>
        /// 处理结束节点
        /// </summary>
        /// <param name="node">结束节点</param>
        private async Task HandleEndingNode(DialogueNode node)
        {
            currentTree.MarkAsCompleted();
            await EndDialogue();
        }

        #endregion

        #region AI集成

        /// <summary>
        /// 调用AI生成响应
        /// </summary>
        /// <param name="prompt">提示词</param>
        /// <returns>AI响应</returns>
        public async Task<string> GenerateAIResponse(string prompt)
        {
            try
            {
                isWaitingForAI = true;
                stateMachine.TransitionTo(DialogueState.AIGenerating);

                string response = await apiClient.SendRequestAsync(prompt);

                isWaitingForAI = false;
                OnAIResponseReceived?.Invoke();

                if (contentFilter != null)
                {
                    response = contentFilter.FilterContent(response);
                }

                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"AI response error: {e.Message}");
                OnDialogueError?.Invoke($"AI响应错误: {e.Message}");
                isWaitingForAI = false;
                stateMachine.TransitionTo(DialogueState.Error);
                return "抱歉，我现在有些困惑...";
            }
        }

        /// <summary>
        /// 使用上下文生成AI响应
        /// </summary>
        /// <param name="additionalContext">额外上下文</param>
        /// <returns>AI响应</returns>
        public async Task<string> GenerateContextualAIResponse(string additionalContext = "")
        {
            string prompt = BuildPrompt(additionalContext);
            return await GenerateAIResponse(prompt);
        }

        /// <summary>
        /// 构建提示词
        /// </summary>
        /// <param name="additionalContext">额外上下文</param>
        /// <returns>完整的提示词</returns>
        private string BuildPrompt(string additionalContext)
        {
            System.Text.StringBuilder prompt = new System.Text.StringBuilder();

            if (currentContext != null)
            {
                prompt.AppendLine($"当前好感度: {currentContext.CurrentAffection}");
                prompt.AppendLine($"当前回合: {currentContext.CurrentRound}");
                prompt.AppendLine($"对话历史:");
                prompt.AppendLine(currentContext.GetHistoryString());
            }

            if (!string.IsNullOrEmpty(additionalContext))
            {
                prompt.AppendLine($"额外信息: {additionalContext}");
            }

            return prompt.ToString();
        }

        #endregion

        #region 对话树管理

        /// <summary>
        /// 加载对话树
        /// </summary>
        /// <param name="treeId">对话树ID</param>
        /// <returns>对话树</returns>
        private async Task<DialogueTree> LoadDialogueTree(string treeId)
        {
            if (dialogueTreeCache.ContainsKey(treeId))
            {
                return dialogueTreeCache[treeId];
            }

            DialogueTree tree = await LoadTreeFromResources(treeId);
            if (tree != null)
            {
                dialogueTreeCache[treeId] = tree;
            }

            return tree;
        }

        /// <summary>
        /// 从资源加载对话树
        /// </summary>
        /// <param name="treeId">对话树ID</param>
        /// <returns>对话树</returns>
        private async Task<DialogueTree> LoadTreeFromResources(string treeId)
        {
            await Task.Delay(1);
            return null;
        }

        /// <summary>
        /// 注册对话树
        /// </summary>
        /// <param name="tree">对话树</param>
        public void RegisterDialogueTree(DialogueTree tree)
        {
            if (tree != null && !string.IsNullOrEmpty(tree.TreeId))
            {
                dialogueTreeCache[tree.TreeId] = tree;
            }
        }

        /// <summary>
        /// 获取对话树
        /// </summary>
        /// <param name="treeId">对话树ID</param>
        /// <returns>对话树</returns>
        public DialogueTree GetDialogueTree(string treeId)
        {
            return dialogueTreeCache.ContainsKey(treeId) ? dialogueTreeCache[treeId] : null;
        }

        /// <summary>
        /// 清除对话树缓存
        /// </summary>
        public void ClearCache()
        {
            dialogueTreeCache.Clear();
        }

        #endregion

        #region 状态管理

        /// <summary>
        /// 处理状态变化
        /// </summary>
        /// <param name="newState">新状态</param>
        /// <param name="oldState">旧状态</param>
        private void HandleStateChanged(DialogueState newState, DialogueState oldState)
        {
            Debug.Log($"Dialogue state changed: {oldState} -> {newState}");
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns>当前状态</returns>
        public DialogueState GetCurrentState()
        {
            return stateMachine.CurrentState;
        }

        /// <summary>
        /// 中断对话
        /// </summary>
        public void InterruptDialogue()
        {
            if (isInDialogue)
            {
                isInDialogue = false;
                isWaitingForAI = false;
                stateMachine.TransitionTo(DialogueState.Interrupted);
            }
        }

        /// <summary>
        /// 恢复对话
        /// </summary>
        public async void ResumeDialogue()
        {
            if (currentContext != null && currentTree != null)
            {
                isInDialogue = true;
                DialogueNode currentNode = currentTree.GetNode(currentContext.CurrentNodeId);
                if (currentNode != null)
                {
                    stateMachine.TransitionTo(DialogueState.Resuming);
                    await EnterNode(currentNode);
                }
            }
        }

        #endregion

        #region 历史记录

        /// <summary>
        /// 添加到全局历史记录
        /// </summary>
        /// <param name="entry">历史记录条目</param>
        public void AddToGlobalHistory(DialogueHistoryEntry entry)
        {
            dialogueHistory.Add(entry);
            if (dialogueHistory.Count > 100)
            {
                dialogueHistory.RemoveAt(0);
            }
        }

        /// <summary>
        /// 获取全局历史记录
        /// </summary>
        /// <param name="maxEntries">最大条目数</param>
        /// <returns>历史记录</returns>
        public List<DialogueHistoryEntry> GetGlobalHistory(int maxEntries = 50)
        {
            if (maxEntries <= 0) return new List<DialogueHistoryEntry>();
            int start = Mathf.Max(0, dialogueHistory.Count - maxEntries);
            return dialogueHistory.GetRange(start, dialogueHistory.Count - start);
        }

        /// <summary>
        /// 清除全局历史记录
        /// </summary>
        public void ClearGlobalHistory()
        {
            dialogueHistory.Clear();
        }

        #endregion

        #region Unity特定

        private static class Mathf
        {
            public static int Max(int a, int b)
            {
                return System.Math.Max(a, b);
            }
        }

        #endregion
    }
}
