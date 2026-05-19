using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveBeatGuide
{
    /// <summary>
    /// 导演模式引导步骤实现
    /// </summary>
    public static class DirectorGuideSteps
    {
        /// <summary>
        /// 导演上任引导步骤执行器
        /// </summary>
        public static class DirectorOnboardGuide
        {
            public static void ExecuteInvitationStep(Action onComplete)
            {
                Debug.Log("执行导演上任引导：节目组邀请");
                UIEventSystem.Instance?.ShowPanel("InvitationPanel");
                EventDispatcher.AddListener<InvitationAcceptedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteRoleInfoStep(Action onComplete)
            {
                Debug.Log("执行导演上任引导：了解导演职责");
                UIEventSystem.Instance?.ShowPanel("DirectorRolePanel");
                GuideHintSystem.Instance?.ShowHint("DirectorRoleHint", "作为导演，你需要管理节目流程");
                EventDispatcher.AddListener<RoleInfoReadEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteReviewGuestsStep(Action onComplete)
            {
                Debug.Log("执行导演上任引导：审阅嘉宾资料");
                UIEventSystem.Instance?.ShowPanel("GuestProfilesPanel");
                GuideHintSystem.Instance?.ShowHint("GuestProfilesHint", "这里有12位嘉宾的资料");
                EventDispatcher.AddListener<GuestProfilesReviewedEvent>(e =>
                {
                    if (e.profilesViewed >= 12)
                        onComplete?.Invoke();
                });
            }

            public static void ExecuteConfirmStep(Action onComplete)
            {
                Debug.Log("执行导演上任引导：完成上任");
                UIEventSystem.Instance?.EnableButton("ConfirmButton");
                GuideHintSystem.Instance?.ShowHint("ConfirmHint", "确认成为导演，正式上任");
                EventDispatcher.AddListener<DirectorConfirmedEvent>(e => onComplete?.Invoke());
            }
        }

        /// <summary>
        /// 首期策划引导步骤执行器
        /// </summary>
        public static class FirstEpisodeGuide
        {
            public static void ExecuteDashboardStep(Action onComplete)
            {
                Debug.Log("执行首期策划引导：查看数据面板");
                UIEventSystem.Instance?.ShowPanel("DataDashboardPanel");
                GuideHintSystem.Instance?.ShowHint("DashboardHint", "这是节目的数据面板");
                EventDispatcher.AddListener<DashboardViewedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteViewGuestsStep(Action onComplete)
            {
                Debug.Log("执行首期策划引导：了解嘉宾资料");
                UIEventSystem.Instance?.ShowPanel("GuestListPanel");
                GuideHintSystem.Instance?.ShowHint("GuestListHint", "查看所有嘉宾的详细资料");
                EventDispatcher.AddListener<GuestListViewedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecutePlanActivityStep(Action onComplete)
            {
                Debug.Log("执行首期策划引导：安排首期活动");
                UIEventSystem.Instance?.ShowPanel("ActivityPlannerPanel");
                GuideHintSystem.Instance?.ShowHint("ActivityHint", "为首期节目安排心动活动");
                EventDispatcher.AddListener<FirstActivityPlannedEvent>(e => onComplete?.Invoke());
            }
        }

        /// <summary>
        /// 热度监控引导步骤执行器
        /// </summary>
        public static class HeatMonitorGuide
        {
            public static void ExecuteViewHeatStep(Action onComplete)
            {
                Debug.Log("执行热度监控引导：查看热度数据");
                UIEventSystem.Instance?.ShowPanel("HeatMonitorPanel");
                GuideHintSystem.Instance?.ShowHint("HeatHint", "实时监控节目热度");
                EventDispatcher.AddListener<HeatDataViewedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteAnalyzeFeedbackStep(Action onComplete)
            {
                Debug.Log("执行热度监控引导：分析观众反馈");
                UIEventSystem.Instance?.ShowPanel("FeedbackPanel");
                GuideHintSystem.Instance?.ShowHint("FeedbackHint", "查看观众的评论和反馈");
                EventDispatcher.AddListener<FeedbackAnalyzedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteAdjustStrategyStep(Action onComplete)
            {
                Debug.Log("执行热度监控引导：调整节目策略");
                UIEventSystem.Instance?.ShowPanel("StrategyPanel");
                GuideHintSystem.Instance?.ShowHint("StrategyHint", "根据数据调整节目策略");
                EventDispatcher.AddListener<StrategyAdjustedEvent>(e => onComplete?.Invoke());
            }
        }

        /// <summary>
        /// 活动安排引导步骤执行器
        /// </summary>
        public static class ActivityArrangeGuide
        {
            public static void ExecuteViewTemplatesStep(Action onComplete)
            {
                Debug.Log("执行活动安排引导：查看活动模板");
                UIEventSystem.Instance?.ShowPanel("ActivityTemplatesPanel");
                GuideHintSystem.Instance?.ShowHint("TemplatesHint", "这里有各种活动模板");
                EventDispatcher.AddListener<TemplatesViewedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteCreateActivityStep(Action onComplete)
            {
                Debug.Log("执行活动安排引导：创建新活动");
                UIEventSystem.Instance?.ShowPanel("ActivityCreatorPanel");
                GuideHintSystem.Instance?.ShowHint("CreatorHint", "创建一个心动活动");
                EventDispatcher.AddListener<ActivityCreatedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteScheduleStep(Action onComplete)
            {
                Debug.Log("执行活动安排引导：安排活动时间");
                UIEventSystem.Instance?.ShowPanel("SchedulePanel");
                GuideHintSystem.Instance?.ShowHint("ScheduleHint", "设置活动的时间和规则");
                EventDispatcher.AddListener<ScheduleSetEvent>(e => onComplete?.Invoke());
            }
        }

        /// <summary>
        /// 危机公关引导步骤执行器
        /// </summary>
        public static class CrisisManagementGuide
        {
            public static void ExecuteViewNetworkStep(Action onComplete)
            {
                Debug.Log("执行危机公关引导：查看关系网络");
                UIEventSystem.Instance?.ShowPanel("RelationshipNetworkPanel");
                GuideHintSystem.Instance?.ShowHint("NetworkHint", "了解嘉宾之间的关系网络");
                EventDispatcher.AddListener<NetworkViewedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteIdentifyRiskStep(Action onComplete)
            {
                Debug.Log("执行危机公关引导：识别潜在问题");
                UIEventSystem.Instance?.ShowPanel("CrisisAlertPanel");
                GuideHintSystem.Instance?.ShowHint("RiskHint", "发现可能出现的危机");
                EventDispatcher.AddListener<RiskIdentifiedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteHandleCrisisStep(Action onComplete)
            {
                Debug.Log("执行危机公关引导：处理模拟危机");
                UIEventSystem.Instance?.ShowPanel("CrisisHandlerPanel");
                GuideHintSystem.Instance?.ShowHint("CrisisHint", "学习如何处理危机情况");
                EventDispatcher.AddListener<CrisisHandledEvent>(e => onComplete?.Invoke());
            }
        }
    }

    #region 导演模式事件类定义

    public class InvitationAcceptedEvent
    {
        public string invitationId;
    }

    public class RoleInfoReadEvent
    {
    }

    public class GuestProfilesReviewedEvent
    {
        public int profilesViewed;
    }

    public class DirectorConfirmedEvent
    {
    }

    public class DashboardViewedEvent
    {
    }

    public class GuestListViewedEvent
    {
    }

    public class FirstActivityPlannedEvent
    {
        public string activityId;
    }

    public class HeatDataViewedEvent
    {
    }

    public class FeedbackAnalyzedEvent
    {
    }

    public class StrategyAdjustedEvent
    {
    }

    public class TemplatesViewedEvent
    {
    }

    public class ActivityCreatedEvent
    {
        public string activityId;
    }

    public class ScheduleSetEvent
    {
        public string activityId;
    }

    public class NetworkViewedEvent
    {
    }

    public class RiskIdentifiedEvent
    {
        public string riskId;
    }

    public class CrisisHandledEvent
    {
        public string crisisId;
    }

    #endregion

    /// <summary>
    /// 导演引导步骤执行器工厂
    /// </summary>
    public static class DirectorGuideStepExecutorFactory
    {
        private static Dictionary<string, Action<Action>> executors = new Dictionary<string, Action<Action>>();

        static DirectorGuideStepExecutorFactory()
        {
            RegisterExecutors();
        }

        private static void RegisterExecutors()
        {
            executors["director_1"] = DirectorGuideSteps.DirectorOnboardGuide.ExecuteInvitationStep;
            executors["director_2"] = DirectorGuideSteps.DirectorOnboardGuide.ExecuteRoleInfoStep;
            executors["director_3"] = DirectorGuideSteps.DirectorOnboardGuide.ExecuteReviewGuestsStep;
            executors["director_4"] = DirectorGuideSteps.DirectorOnboardGuide.ExecuteConfirmStep;

            executors["planning_1"] = DirectorGuideSteps.FirstEpisodeGuide.ExecuteDashboardStep;
            executors["planning_2"] = DirectorGuideSteps.FirstEpisodeGuide.ExecuteViewGuestsStep;
            executors["planning_3"] = DirectorGuideSteps.FirstEpisodeGuide.ExecutePlanActivityStep;

            executors["heat_1"] = DirectorGuideSteps.HeatMonitorGuide.ExecuteViewHeatStep;
            executors["heat_2"] = DirectorGuideSteps.HeatMonitorGuide.ExecuteAnalyzeFeedbackStep;
            executors["heat_3"] = DirectorGuideSteps.HeatMonitorGuide.ExecuteAdjustStrategyStep;

            executors["activity_1"] = DirectorGuideSteps.ActivityArrangeGuide.ExecuteViewTemplatesStep;
            executors["activity_2"] = DirectorGuideSteps.ActivityArrangeGuide.ExecuteCreateActivityStep;
            executors["activity_3"] = DirectorGuideSteps.ActivityArrangeGuide.ExecuteScheduleStep;

            executors["crisis_1"] = DirectorGuideSteps.CrisisManagementGuide.ExecuteViewNetworkStep;
            executors["crisis_2"] = DirectorGuideSteps.CrisisManagementGuide.ExecuteIdentifyRiskStep;
            executors["crisis_3"] = DirectorGuideSteps.CrisisManagementGuide.ExecuteHandleCrisisStep;
        }

        /// <summary>
        /// 执行指定步骤
        /// </summary>
        public static void ExecuteStep(string stepId, Action onComplete)
        {
            if (executors.ContainsKey(stepId))
            {
                executors[stepId]?.Invoke(onComplete);
            }
            else
            {
                Debug.LogWarning($"未找到导演引导步骤执行器: {stepId}");
                onComplete?.Invoke();
            }
        }

        /// <summary>
        /// 注册执行器
        /// </summary>
        public static void RegisterExecutor(string stepId, Action<Action> executor)
        {
            executors[stepId] = executor;
        }
    }
}
