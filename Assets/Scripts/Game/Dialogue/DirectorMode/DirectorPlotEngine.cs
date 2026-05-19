using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace 糟糕是心动鸭.Dialogue.DirectorMode
{
    /// <summary>
    /// 导演模式剧情引擎
    /// 管理恋综节目的剧情生成和进程控制
    /// </summary>
    public class DirectorPlotEngine : MonoBehaviour
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        private static DirectorPlotEngine instance;

        /// <summary>
        /// 当前节目数据
        /// </summary>
        private DirectorPlayerData currentShowData;

        /// <summary>
        /// 剧情生成器
        /// </summary>
        private PlotGenerator plotGenerator;

        /// <summary>
        /// 观众反应模拟器
        /// </summary>
        private AudienceReactionSimulator reactionSimulator;

        /// <summary>
        /// 热度计算器
        /// </summary>
        private HeatCalculator heatCalculator;

        /// <summary>
        /// 当前剧情阶段
        /// </summary>
        private PlotPhase currentPhase;

        /// <summary>
        /// 剧情事件队列
        /// </summary>
        private Queue<PlotEvent> eventQueue;

        /// <summary>
        /// 已触发的事件
        /// </summary>
        private List<string> triggeredEventIds;

        /// <summary>
        /// 当前回合
        /// </summary>
        private int currentRound;

        /// <summary>
        /// 是否正在生成剧情
        /// </summary>
        private bool isGeneratingPlot;

        /// <summary>
        /// 剧情事件
        /// </summary>
        public event Action<PlotEvent> OnPlotEventGenerated;
        public event Action<PlotPhase> OnPhaseChanged;
        public event Action<int, float> OnHeatChanged;
        public event Action<AudienceReaction> OnAudienceReacted;
        public event Action<string> OnPlotCompleted;

        #region 属性访问器

        public static DirectorPlotEngine Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("DirectorPlotEngine");
                    instance = go.AddComponent<DirectorPlotEngine>();
                }
                return instance;
            }
        }

        public PlotPhase CurrentPhase => currentPhase;
        public int CurrentRound => currentRound;
        public bool IsGeneratingPlot => isGeneratingPlot;
        public DirectorPlayerData CurrentShowData => currentShowData;
        public HeatCalculator HeatCalculator => heatCalculator;

        #endregion

        #region Unity生命周期

        /// <summary>
        /// Awake方法
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化引擎
        /// </summary>
        private void Initialize()
        {
            currentPhase = PlotPhase.Preparation;
            eventQueue = new Queue<PlotEvent>();
            triggeredEventIds = new List<string>();
            currentRound = 0;
            isGeneratingPlot = false;

            plotGenerator = new PlotGenerator();
            reactionSimulator = new AudienceReactionSimulator();
            heatCalculator = new HeatCalculator();
        }

        #endregion

        #region 节目控制

        /// <summary>
        /// 开始新节目
        /// </summary>
        /// <param name="showData">节目数据</param>
        /// <returns>是否成功开始</returns>
        public async Task<bool> StartNewShow(DirectorPlayerData showData)
        {
            try
            {
                currentShowData = showData;
                currentRound = 0;
                triggeredEventIds.Clear();
                eventQueue.Clear();

                await InitializeShowData();

                TransitionToPhase(PlotPhase.Introduction);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error starting show: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 初始化节目数据
        /// </summary>
        private async Task InitializeShowData()
        {
            heatCalculator.Initialize(currentShowData.StartingHeat);
            await Task.Delay(1);
        }

        /// <summary>
        /// 执行导演行动
        /// </summary>
        /// <param name="action">导演行动</param>
        /// <returns>行动结果描述</returns>
        public async Task<string> ExecuteDirectorAction(DirectorAction action)
        {
            if (isGeneratingPlot)
            {
                return "正在生成剧情，请稍候...";
            }

            isGeneratingPlot = true;

            try
            {
                string result = await ProcessAction(action);

                AudienceReaction reaction = reactionSimulator.SimulateReaction(action, currentPhase);
                OnAudienceReacted?.Invoke(reaction);

                float heatChange = heatCalculator.CalculateHeatChange(action, reaction, currentPhase);
                heatCalculator.UpdateHeat(heatChange);
                OnHeatChanged?.Invoke(currentRound, heatCalculator.CurrentHeat);

                await ProcessEventQueue();

                return result;
            }
            finally
            {
                isGeneratingPlot = false;
            }
        }

        /// <summary>
        /// 处理导演行动
        /// </summary>
        /// <param name="action">行动</param>
        /// <returns>结果描述</returns>
        private async Task<string> ProcessAction(DirectorAction action)
        {
            PlotEvent eventData = await plotGenerator.GeneratePlotEvent(action, currentShowData, currentRound);

            if (eventData != null)
            {
                eventQueue.Enqueue(eventData);
                triggeredEventIds.Add(eventData.EventId);
                OnPlotEventGenerated?.Invoke(eventData);
            }

            currentRound++;

            CheckPhaseTransition();

            return eventData?.Description ?? "行动执行完成";
        }

        #endregion

        #region 阶段管理

        /// <summary>
        /// 转换到指定阶段
        /// </summary>
        /// <param name="newPhase">新阶段</param>
        public void TransitionToPhase(PlotPhase newPhase)
        {
            if (currentPhase != newPhase)
            {
                currentPhase = newPhase;
                OnPhaseChanged?.Invoke(newPhase);
            }
        }

        /// <summary>
        /// 检查是否需要阶段转换
        /// </summary>
        private void CheckPhaseTransition()
        {
            PlotPhase newPhase = DetermineNextPhase();
            if (newPhase != currentPhase)
            {
                TransitionToPhase(newPhase);
            }
        }

        /// <summary>
        /// 确定下一个阶段
        /// </summary>
        /// <returns>下一阶段</returns>
        private PlotPhase DetermineNextPhase()
        {
            float heat = heatCalculator.CurrentHeat;

            if (heat < 20f)
            {
                return PlotPhase.Preparation;
            }
            else if (heat < 50f)
            {
                if (currentRound < 3) return PlotPhase.Introduction;
                return PlotPhase.Development;
            }
            else if (heat < 75f)
            {
                return PlotPhase.Climax;
            }
            else
            {
                return PlotPhase.Finale;
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 处理事件队列
        /// </summary>
        private async Task ProcessEventQueue()
        {
            while (eventQueue.Count > 0)
            {
                PlotEvent currentEvent = eventQueue.Peek();

                await ExecuteEvent(currentEvent);

                if (currentEvent.IsComplete)
                {
                    eventQueue.Dequeue();
                }

                await Task.Delay(100);
            }
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        private async Task ExecuteEvent(PlotEvent eventData)
        {
            await Task.Delay(1);
            eventData.IsComplete = true;
        }

        /// <summary>
        /// 触发特殊事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        public async Task TriggerSpecialEvent(string eventId)
        {
            if (triggeredEventIds.Contains(eventId)) return;

            PlotEvent specialEvent = await plotGenerator.GenerateSpecialEvent(eventId, currentShowData);
            if (specialEvent != null)
            {
                eventQueue.Enqueue(specialEvent);
                triggeredEventIds.Add(eventId);
                OnPlotEventGenerated?.Invoke(specialEvent);

                await ProcessEventQueue();
            }
        }

        #endregion

        #region 节目结束

        /// <summary>
        /// 结束当前节目
        /// </summary>
        public void EndShow()
        {
            float finalHeat = heatCalculator.CurrentHeat;
            string rating = heatCalculator.GetHeatRating();

            OnPlotCompleted?.Invoke($"节目完成！最终热度: {finalHeat:F1} ({rating})");

            TransitionToPhase(PlotPhase.Ending);
        }

        /// <summary>
        /// 获取节目总结
        /// </summary>
        /// <returns>总结文本</returns>
        public string GetShowSummary()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("=== 节目总结 ===");
            sb.AppendLine($"总回合数: {currentRound}");
            sb.AppendLine($"最终热度: {heatCalculator.CurrentHeat:F1}");
            sb.AppendLine($"热度评级: {heatCalculator.GetHeatRating()}");
            sb.AppendLine($"触发事件数: {triggeredEventIds.Count}");
            sb.AppendLine($"最终阶段: {currentPhase}");
            return sb.ToString();
        }

        #endregion

        #region 数据查询

        /// <summary>
        /// 获取当前热度
        /// </summary>
        /// <returns>当前热度值</returns>
        public float GetCurrentHeat()
        {
            return heatCalculator.CurrentHeat;
        }

        /// <summary>
        /// 获取热度等级
        /// </summary>
        /// <returns>热度等级描述</returns>
        public string GetHeatLevel()
        {
            return heatCalculator.GetHeatRating();
        }

        /// <summary>
        /// 检查是否已触发事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <returns>是否已触发</returns>
        public bool HasTriggeredEvent(string eventId)
        {
            return triggeredEventIds.Contains(eventId);
        }

        #endregion
    }

    /// <summary>
    /// 导演行动
    /// </summary>
    [Serializable]
    public class DirectorAction
    {
        /// <summary>
        /// 行动类型
        /// </summary>
        public DirectorActionType ActionType;

        /// <summary>
        /// 行动参数
        /// </summary>
        public Dictionary<string, string> Parameters;

        /// <summary>
        /// 行动描述
        /// </summary>
        public string Description;

        /// <summary>
        /// 预期热度影响
        /// </summary>
        public float ExpectedHeatImpact;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DirectorAction()
        {
            Parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">行动类型</param>
        public DirectorAction(DirectorActionType type)
        {
            ActionType = type;
            Parameters = new Dictionary<string, string>();
            Description = GetDefaultDescription(type);
        }

        /// <summary>
        /// 获取默认描述
        /// </summary>
        /// <param name="type">行动类型</param>
        /// <returns>描述</returns>
        private string GetDefaultDescription(DirectorActionType type)
        {
            return type switch
            {
                DirectorActionType.ScheduleActivity => "安排活动",
                DirectorActionType.ArrangeDate => "安排约会",
                DirectorActionType.CreateConflict => "制造冲突",
                DirectorActionType.TriggerConfession => "触发告白",
                DirectorActionType.SurpriseEvent => "惊喜事件",
                DirectorActionType.PrivateConversation => "私下谈话",
                DirectorActionType.GroupActivity => "群体活动",
                DirectorActionType.RevealSecret => "揭露秘密",
                _ => "未知行动"
            };
        }
    }

    /// <summary>
    /// 导演行动类型
    /// </summary>
    public enum DirectorActionType
    {
        ScheduleActivity,
        ArrangeDate,
        CreateConflict,
        TriggerConfession,
        SurpriseEvent,
        PrivateConversation,
        GroupActivity,
        RevealSecret
    }

    /// <summary>
    /// 剧情阶段
    /// </summary>
    public enum PlotPhase
    {
        Preparation,
        Introduction,
        Development,
        Climax,
        Finale,
        Ending
    }

    /// <summary>
    /// 剧情事件
    /// </summary>
    [Serializable]
    public class PlotEvent
    {
        public string EventId;
        public string Title;
        public string Description;
        public PlotPhase Phase;
        public bool IsComplete;
        public List<string> InvolvedCharacterIds;
        public Dictionary<string, float> Effects;

        public PlotEvent()
        {
            InvolvedCharacterIds = new List<string>();
            Effects = new Dictionary<string, float>();
            IsComplete = false;
        }
    }
}
