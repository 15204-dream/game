using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveBeatGuide
{
    /// <summary>
    /// 嘉宾模式引导步骤实现
    /// </summary>
    public static class GuestGuideSteps
    {
        /// <summary>
        /// 入场引导步骤执行器
        /// </summary>
        public static class EntryGuide
        {
            public static void ExecutePhoneCallStep(Action onComplete)
            {
                Debug.Log("执行入场引导：节目组来电");
                UIEventSystem.Instance?.ShowPanel("PhoneCallPanel");
                EventDispatcher.AddListener<PhoneCallCompleteEvent>(e =>
                {
                    EventDispatcher.RemoveListener<PhoneCallCompleteEvent>(null);
                    onComplete?.Invoke();
                });
            }

            public static void ExecuteRulesStep(Action onComplete)
            {
                Debug.Log("执行入场引导：了解节目规则");
                UIEventSystem.Instance?.ShowPanel("RulesPanel");
                EventDispatcher.AddListener<RulesReadCompleteEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteProfileStep(Action onComplete)
            {
                Debug.Log("执行入场引导：创建角色档案");
                UIEventSystem.Instance?.ShowPanel("ProfileEditor");
                EventDispatcher.AddListener<ProfileSaveCompleteEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteConfirmStep(Action onComplete)
            {
                Debug.Log("执行入场引导：完成入场");
                UIEventSystem.Instance?.EnableButton("ConfirmButton");
                EventDispatcher.AddListener<ConfirmButtonClickedEvent>(e => onComplete?.Invoke());
            }
        }

        /// <summary>
        /// 入住引导步骤执行器
        /// </summary>
        public static class CheckinGuide
        {
            public static void ExecuteIntroductionStep(Action onComplete)
            {
                Debug.Log("执行入住引导：自我介绍");
                UIEventSystem.Instance?.ShowPanel("IntroductionPanel");
                EventDispatcher.AddListener<IntroductionCompleteEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteFirstMeetingStep(Action onComplete)
            {
                Debug.Log("执行入住引导：认识第一位嘉宾");
                UIEventSystem.Instance?.HighlightGuest("FirstGuest");
                EventDispatcher.AddListener<GuestMetEvent>(e =>
                {
                    if (e.guestId == "FirstGuest")
                        onComplete?.Invoke();
                });
            }

            public static void ExecuteExploreStep(Action onComplete)
            {
                Debug.Log("执行入住引导：探索心动小屋");
                UIEventSystem.Instance?.ShowPanel("MapPanel");
                EventDispatcher.AddListener<MapExploredEvent>(e =>
                {
                    if (e.exploredCount >= 3)
                        onComplete?.Invoke();
                });
            }
        }

        /// <summary>
        /// 心动信箱引导步骤执行器
        /// </summary>
        public static class MailboxGuide
        {
            public static void ExecuteTutorialStep(Action onComplete)
            {
                Debug.Log("执行心动信箱引导：学习使用");
                UIEventSystem.Instance?.ShowPanel("MailboxPanel");
                GuideHintSystem.Instance?.ShowHint("MailboxHint", "这是心动信箱，你可以在这里写信");
                EventDispatcher.AddListener<MailboxOpenedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteWriteLetterStep(Action onComplete)
            {
                Debug.Log("执行心动信箱引导：写出第一封信");
                UIEventSystem.Instance?.ShowPanel("LetterEditor");
                GuideHintSystem.Instance?.ShowHint("LetterHint", "选择你心仪的对象，写下你的心声");
                EventDispatcher.AddListener<LetterSentEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteCheckMailStep(Action onComplete)
            {
                Debug.Log("执行心动信箱引导：查看收到的信");
                UIEventSystem.Instance?.ShowPanel("ReceivedMailPanel");
                EventDispatcher.AddListener<MailCheckedEvent>(e => onComplete?.Invoke());
            }
        }

        /// <summary>
        /// 任务系统引导步骤执行器
        /// </summary>
        public static class TaskGuide
        {
            public static void ExecuteViewTasksStep(Action onComplete)
            {
                Debug.Log("执行任务系统引导：查看每日任务");
                UIEventSystem.Instance?.ShowPanel("TaskPanel");
                GuideHintSystem.Instance?.ShowHint("TaskHint", "完成任务可以获得丰厚奖励哦");
                EventDispatcher.AddListener<TaskPanelOpenedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteAcceptTaskStep(Action onComplete)
            {
                Debug.Log("执行任务系统引导：接受任务");
                UIEventSystem.Instance?.HighlightTask("DailyTask_1");
                EventDispatcher.AddListener<TaskAcceptedEvent>(e =>
                {
                    if (e.taskId == "DailyTask_1")
                        onComplete?.Invoke();
                });
            }

            public static void ExecuteCompleteTaskStep(Action onComplete)
            {
                Debug.Log("执行任务系统引导：完成任务");
                EventDispatcher.AddListener<TaskCompletedEvent>(e =>
                {
                    if (e.taskId == "DailyTask_1")
                        onComplete?.Invoke();
                });
            }
        }

        /// <summary>
        /// 社交互动引导步骤执行器
        /// </summary>
        public static class SocialGuide
        {
            public static void ExecuteStartChatStep(Action onComplete)
            {
                Debug.Log("执行社交互动引导：发起对话");
                UIEventSystem.Instance?.ShowPanel("ChatPanel");
                GuideHintSystem.Instance?.ShowHint("ChatHint", "选择一位嘉宾开始聊天");
                EventDispatcher.AddListener<ChatStartedEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteGiftStep(Action onComplete)
            {
                Debug.Log("执行社交互动引导：赠送礼物");
                UIEventSystem.Instance?.ShowPanel("GiftPanel");
                GuideHintSystem.Instance?.ShowHint("GiftHint", "送给心仪的对象一个小礼物吧");
                EventDispatcher.AddListener<GiftSentEvent>(e => onComplete?.Invoke());
            }

            public static void ExecuteReactionStep(Action onComplete)
            {
                Debug.Log("执行社交互动引导：使用表情反应");
                UIEventSystem.Instance?.ShowEmojiPanel();
                EventDispatcher.AddListener<EmojiUsedEvent>(e => onComplete?.Invoke());
            }
        }
    }

    /// <summary>
    /// UI事件系统占位类
    /// </summary>
    public class UIEventSystem
    {
        private static UIEventSystem instance;
        public static UIEventSystem Instance => instance ?? (instance = new UIEventSystem());

        public void ShowPanel(string panelName) { }
        public void HidePanel(string panelName) { }
        public void EnableButton(string buttonName) { }
        public void HighlightGuest(string guestId) { }
        public void HighlightTask(string taskId) { }
        public void ShowEmojiPanel() { }
    }

    /// <summary>
    /// 事件调度器占位类
    /// </summary>
    public class EventDispatcher
    {
        public static void AddListener<T>(Action<T> callback) where T : new() { }
        public static void RemoveListener<T>(Action<T> callback) where T : new() { }
        public static void Dispatch<T>(T eventData) where T : new() { }
    }

    #region 事件类定义

    public class PhoneCallCompleteEvent
    {
        public string callerId;
    }

    public class RulesReadCompleteEvent
    {
        public int rulesRead;
    }

    public class ProfileSaveCompleteEvent
    {
        public string profileId;
    }

    public class ConfirmButtonClickedEvent
    {
    }

    public class IntroductionCompleteEvent
    {
        public string guestId;
    }

    public class GuestMetEvent
    {
        public string guestId;
    }

    public class MapExploredEvent
    {
        public int exploredCount;
    }

    public class MailboxOpenedEvent
    {
    }

    public class LetterSentEvent
    {
        public string recipientId;
    }

    public class MailCheckedEvent
    {
        public int mailCount;
    }

    public class TaskPanelOpenedEvent
    {
    }

    public class TaskAcceptedEvent
    {
        public string taskId;
    }

    public class TaskCompletedEvent
    {
        public string taskId;
    }

    public class ChatStartedEvent
    {
        public string guestId;
    }

    public class GiftSentEvent
    {
        public string recipientId;
        public string giftId;
    }

    public class EmojiUsedEvent
    {
        public string emojiId;
        public string targetGuestId;
    }

    #endregion

    /// <summary>
    /// 引导步骤执行器工厂
    /// </summary>
    public static class GuideStepExecutorFactory
    {
        private static Dictionary<string, Action<Action>> executors = new Dictionary<string, Action<Action>>();

        static GuideStepExecutorFactory()
        {
            RegisterExecutors();
        }

        private static void RegisterExecutors()
        {
            executors["entry_1"] = GuestGuideSteps.EntryGuide.ExecutePhoneCallStep;
            executors["entry_2"] = GuestGuideSteps.EntryGuide.ExecuteRulesStep;
            executors["entry_3"] = GuestGuideSteps.EntryGuide.ExecuteProfileStep;
            executors["entry_4"] = GuestGuideSteps.EntryGuide.ExecuteConfirmStep;

            executors["checkin_1"] = GuestGuideSteps.CheckinGuide.ExecuteIntroductionStep;
            executors["checkin_2"] = GuestGuideSteps.CheckinGuide.ExecuteFirstMeetingStep;
            executors["checkin_3"] = GuestGuideSteps.CheckinGuide.ExecuteExploreStep;

            executors["mail_1"] = GuestGuideSteps.MailboxGuide.ExecuteTutorialStep;
            executors["mail_2"] = GuestGuideSteps.MailboxGuide.ExecuteWriteLetterStep;
            executors["mail_3"] = GuestGuideSteps.MailboxGuide.ExecuteCheckMailStep;

            executors["task_1"] = GuestGuideSteps.TaskGuide.ExecuteViewTasksStep;
            executors["task_2"] = GuestGuideSteps.TaskGuide.ExecuteAcceptTaskStep;
            executors["task_3"] = GuestGuideSteps.TaskGuide.ExecuteCompleteTaskStep;

            executors["social_1"] = GuestGuideSteps.SocialGuide.ExecuteStartChatStep;
            executors["social_2"] = GuestGuideSteps.SocialGuide.ExecuteGiftStep;
            executors["social_3"] = GuestGuideSteps.SocialGuide.ExecuteReactionStep;
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
                Debug.LogWarning($"未找到引导步骤执行器: {stepId}");
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
