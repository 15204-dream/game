using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveBeatGuide
{
    /// <summary>
    /// 引导序列状态
    /// </summary>
    public enum GuideSequenceState
    {
        Inactive,      // 未激活
        Ready,         // 就绪
        Running,       // 运行中
        Paused,        // 暂停
        Completed,     // 已完成
        Cancelled      // 已取消
    }

    /// <summary>
    /// 引导序列数据
    /// </summary>
    [Serializable]
    public class GuideSequenceData
    {
        public string sequenceId;
        public string sequenceName;
        public string description;
        public List<GuideStepData> steps = new List<GuideStepData>();
        public bool autoStart = false;
        public bool canSkip = true;
        public bool saveProgress = true;
        public int priority = 0;
        public GuideSequenceTrigger trigger;
        public GuideSequenceData nextSequence;
    }

    /// <summary>
    /// 引导序列触发器
    /// </summary>
    [Serializable]
    public class GuideSequenceTrigger
    {
        public GuideTriggerType type;
        public string triggerId;
        public string sceneName;
        public float delay = 0f;
    }

    /// <summary>
    /// 引导触发类型
    /// </summary>
    public enum GuideTriggerType
    {
        None,
        SceneLoad,
        Time,
        Event,
        Manual,
        Achievement,
        Level
    }

    /// <summary>
    /// 引导序列类
    /// </summary>
    public class GuideSequence
    {
        protected GuideSequenceData data;
        protected GuideSequenceState state;
        protected List<GuideStep> steps;
        protected int currentStepIndex;
        protected GuideManager guideManager;
        protected float triggerTime;

        public GuideSequenceData Data => data;
        public GuideSequenceState State => state;
        public string SequenceId => data?.sequenceId;
        public int CurrentStepIndex => currentStepIndex;
        public GuideStep CurrentStep => currentStepIndex >= 0 && currentStepIndex < steps.Count ? steps[currentStepIndex] : null;
        public bool IsRunning => state == GuideSequenceState.Running;
        public int TotalSteps => steps?.Count ?? 0;

        public GuideSequence(GuideSequenceData sequenceData, GuideManager manager)
        {
            this.data = sequenceData;
            this.guideManager = manager;
            this.state = GuideSequenceState.Inactive;
            this.currentStepIndex = -1;
            InitializeSteps();
        }

        /// <summary>
        /// 初始化步骤
        /// </summary>
        private void InitializeSteps()
        {
            steps = new List<GuideStep>();
            
            foreach (var stepData in data.steps)
            {
                GuideStep step = CreateStepInstance(stepData);
                step.SetParentSequence(this);
                steps.Add(step);
            }
        }

        /// <summary>
        /// 创建步骤实例
        /// </summary>
        protected virtual GuideStep CreateStepInstance(GuideStepData stepData)
        {
            return new GenericGuideStep(stepData);
        }

        /// <summary>
        /// 开始序列
        /// </summary>
        public virtual void StartSequence()
        {
            if (state == GuideSequenceState.Running || state == GuideSequenceState.Completed)
                return;

            state = GuideSequenceState.Ready;
            triggerTime = Time.time;

            if (data.steps.Count > 0)
            {
                MoveToNextStep();
            }
            else
            {
                CompleteSequence();
            }
        }

        /// <summary>
        /// 继续序列
        /// </summary>
        public virtual void ResumeSequence()
        {
            if (state != GuideSequenceState.Paused)
                return;

            state = GuideSequenceState.Running;
            
            if (CurrentStep != null)
            {
                CurrentStep.OnStepStart();
            }
        }

        /// <summary>
        /// 暂停序列
        /// </summary>
        public virtual void PauseSequence()
        {
            if (state != GuideSequenceState.Running)
                return;

            state = GuideSequenceState.Paused;
            
            if (CurrentStep != null)
            {
                CurrentStep.OnStepEnd();
            }
        }

        /// <summary>
        /// 移动到下一步
        /// </summary>
        protected virtual void MoveToNextStep()
        {
            if (currentStepIndex >= 0 && currentStepIndex < steps.Count)
            {
                steps[currentStepIndex].OnStepEnd();
            }

            currentStepIndex++;

            if (currentStepIndex >= steps.Count)
            {
                CompleteSequence();
                return;
            }

            state = GuideSequenceState.Running;
            GuideStep step = steps[currentStepIndex];
            step.OnStepStart();
            
            guideManager?.OnSequenceStepChanged(this, step);
        }

        /// <summary>
        /// 步骤完成回调
        /// </summary>
        public virtual void OnStepCompleted(GuideStep step)
        {
            if (data.saveProgress)
            {
                SaveProgress();
            }

            MoveToNextStep();
        }

        /// <summary>
        /// 完成序列
        /// </summary>
        public virtual void CompleteSequence()
        {
            if (state == GuideSequenceState.Completed || state == GuideSequenceState.Cancelled)
                return;

            state = GuideSequenceState.Completed;

            if (data.saveProgress)
            {
                SaveCompleted();
            }

            guideManager?.OnSequenceCompleted(this);

            if (data.nextSequence != null)
            {
                guideManager?.StartSequence(data.nextSequence.sequenceId);
            }
        }

        /// <summary>
        /// 取消序列
        /// </summary>
        public virtual void CancelSequence()
        {
            if (state == GuideSequenceState.Cancelled || state == GuideSequenceState.Completed)
                return;

            if (CurrentStep != null)
            {
                CurrentStep.OnStepEnd();
            }

            state = GuideSequenceState.Cancelled;
            guideManager?.OnSequenceCancelled(this);
        }

        /// <summary>
        /// 跳过当前步骤
        /// </summary>
        public virtual void SkipCurrentStep()
        {
            if (!data.canSkip || CurrentStep == null)
                return;

            CurrentStep.SkipStep();
        }

        /// <summary>
        /// 更新序列
        /// </summary>
        public virtual void UpdateSequence()
        {
            if (state != GuideSequenceState.Running)
                return;

            if (CurrentStep != null)
            {
                CurrentStep.OnStepUpdate();
            }
        }

        /// <summary>
        /// 获取进度
        /// </summary>
        public virtual float GetProgress()
        {
            if (steps.Count == 0)
                return 1f;

            int completedCount = 0;
            foreach (var step in steps)
            {
                if (step.IsCompleted)
                    completedCount++;
            }

            return (float)completedCount / steps.Count;
        }

        /// <summary>
        /// 保存进度
        /// </summary>
        protected virtual void SaveProgress()
        {
            if (!data.saveProgress)
                return;

            string key = $"GuideProgress_{data.sequenceId}";
            PlayerPrefs.SetInt(key, currentStepIndex);
        }

        /// <summary>
        /// 保存完成状态
        /// </summary>
        protected virtual void SaveCompleted()
        {
            string key = $"GuideCompleted_{data.sequenceId}";
            PlayerPrefs.SetInt(key, 1);
        }

        /// <summary>
        /// 检查是否完成
        /// </summary>
        public virtual bool IsCompleted()
        {
            string key = $"GuideCompleted_{data.sequenceId}";
            return PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key) == 1;
        }

        /// <summary>
        /// 加载进度
        /// </summary>
        public virtual void LoadProgress()
        {
            if (state != GuideSequenceState.Inactive && state != GuideSequenceState.Ready)
                return;

            string key = $"GuideProgress_{data.sequenceId}";
            if (PlayerPrefs.HasKey(key))
            {
                currentStepIndex = PlayerPrefs.GetInt(key);
            }
        }

        /// <summary>
        /// 重置进度
        /// </summary>
        public virtual void ResetProgress()
        {
            string progressKey = $"GuideProgress_{data.sequenceId}";
            string completedKey = $"GuideCompleted_{data.sequenceId}";
            
            PlayerPrefs.DeleteKey(progressKey);
            PlayerPrefs.DeleteKey(completedKey);

            currentStepIndex = -1;
            state = GuideSequenceState.Inactive;

            foreach (var step in steps)
            {
                step.OnStepEnd();
            }
        }
    }

    /// <summary>
    /// 通用引导步骤
    /// </summary>
    public class GenericGuideStep : GuideStep
    {
        public GenericGuideStep(GuideStepData stepData) : base(stepData) { }

        protected override void OnStepStartInternal()
        {
            GuideManager.Instance?.ShowStepUI(this);
        }

        protected override void OnStepUpdateInternal()
        {
        }

        protected override void OnStepCompleteInternal()
        {
            GuideManager.Instance?.HideStepUI(this);
        }

        protected override void OnStepEndInternal()
        {
            GuideManager.Instance?.HideStepUI(this);
        }
    }
}
