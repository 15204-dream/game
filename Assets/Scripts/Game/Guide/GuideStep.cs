using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveBeatGuide
{
    /// <summary>
    /// 引导步骤状态
    /// </summary>
    public enum GuideStepState
    {
        Locked,      // 锁定
        Ready,       // 就绪
        Active,      // 进行中
        Completed,   // 已完成
        Skipped      // 已跳过
    }

    /// <summary>
    /// 引导步骤完成条件类型
    /// </summary>
    public enum GuideConditionType
    {
        Click,           // 点击指定对象
        UI,              // UI界面打开/关闭
        Time,            // 等待时间
        Event,           // 事件触发
        Multiple,        // 多个条件（AND/OR）
        Custom           // 自定义条件
    }

    /// <summary>
    /// 引导步骤数据
    /// </summary>
    [Serializable]
    public class GuideStepData
    {
        public string stepId;
        public string title;
        public string description;
        public GuideConditionType conditionType;
        public List<string> targetPaths = new List<string>();
        public float waitTime = 0f;
        public string eventName;
        public bool requireAllConditions = false;
        public int priority = 0;
        public bool canSkip = true;
        public bool requireManualComplete = false;
        public Vector2 dialoguePosition = new Vector2(0.5f, 0.3f);
        public GuideArrowType arrowType = GuideArrowType.None;
        public float arrowOffset = 0f;
    }

    /// <summary>
    /// 引导步骤基类
    /// </summary>
    public abstract class GuideStep
    {
        protected GuideStepData data;
        protected GuideStepState state;
        protected GuideSequence parentSequence;
        protected float startTime;
        protected List<string> completedConditions = new List<string>();

        public GuideStepData Data => data;
        public GuideStepState State => state;
        public string StepId => data?.stepId;
        public bool IsActive => state == GuideStepState.Active;
        public bool IsCompleted => state == GuideStepState.Completed || state == GuideStepState.Skipped;

        public GuideStep(GuideStepData stepData)
        {
            this.data = stepData;
            this.state = GuideStepState.Ready;
        }

        public void SetParentSequence(GuideSequence sequence)
        {
            parentSequence = sequence;
        }

        /// <summary>
        /// 开始步骤
        /// </summary>
        public virtual void OnStepStart()
        {
            if (state != GuideStepState.Ready && state != GuideStepState.Active)
                return;

            state = GuideStepState.Active;
            startTime = Time.time;
            completedConditions.Clear();
            
            OnStepStartInternal();
        }

        /// <summary>
        /// 步骤开始内部逻辑
        /// </summary>
        protected abstract void OnStepStartInternal();

        /// <summary>
        /// 更新步骤
        /// </summary>
        public virtual void OnStepUpdate()
        {
            if (state != GuideStepState.Active)
                return;

            OnStepUpdateInternal();

            if (!data.requireManualComplete)
            {
                CheckConditions();
            }
        }

        /// <summary>
        /// 步骤更新内部逻辑
        /// </summary>
        protected abstract void OnStepUpdateInternal();

        /// <summary>
        /// 检查完成条件
        /// </summary>
        protected virtual void CheckConditions()
        {
            bool conditionMet = false;

            switch (data.conditionType)
            {
                case GuideConditionType.Time:
                    conditionMet = Time.time - startTime >= data.waitTime;
                    break;

                case GuideConditionType.Event:
                    conditionMet = completedConditions.Contains(data.eventName);
                    break;

                case GuideConditionType.Multiple:
                    conditionMet = data.requireAllConditions 
                        ? completedConditions.Count >= data.targetPaths.Count 
                        : completedConditions.Count > 0;
                    break;

                case GuideConditionType.Click:
                case GuideConditionType.UI:
                default:
                    conditionMet = completedConditions.Count > 0;
                    break;
            }

            if (conditionMet)
            {
                CompleteStep();
            }
        }

        /// <summary>
        /// 处理条件完成
        /// </summary>
        public virtual void OnConditionComplete(string conditionId)
        {
            if (state != GuideStepState.Active)
                return;

            if (!string.IsNullOrEmpty(conditionId) && !completedConditions.Contains(conditionId))
            {
                completedConditions.Add(conditionId);
            }

            if (!data.requireManualComplete)
            {
                CheckConditions();
            }
        }

        /// <summary>
        /// 手动完成步骤
        /// </summary>
        public virtual void CompleteStep()
        {
            if (state != GuideStepState.Active)
                return;

            state = GuideStepState.Completed;
            OnStepCompleteInternal();
            parentSequence?.OnStepCompleted(this);
        }

        /// <summary>
        /// 跳过步骤
        /// </summary>
        public virtual void SkipStep()
        {
            if (!data.canSkip || IsCompleted)
                return;

            state = GuideStepState.Skipped;
            OnStepCompleteInternal();
            parentSequence?.OnStepCompleted(this);
        }

        /// <summary>
        /// 步骤完成内部逻辑
        /// </summary>
        protected abstract void OnStepCompleteInternal();

        /// <summary>
        /// 结束步骤
        /// </summary>
        public virtual void OnStepEnd()
        {
            if (state != GuideStepState.Active && state != GuideStepState.Ready)
                return;

            OnStepEndInternal();
        }

        /// <summary>
        /// 步骤结束内部逻辑
        /// </summary>
        protected abstract void OnStepEndInternal();

        /// <summary>
        /// 获取步骤描述
        /// </summary>
        public string GetStepDescription()
        {
            return data.description;
        }

        /// <summary>
        /// 获取步骤标题
        /// </summary>
        public string GetStepTitle()
        {
            return data.title;
        }
    }

    /// <summary>
    /// 引导箭头类型
    /// </summary>
    public enum GuideArrowType
    {
        None,
        Down,
        Up,
        Left,
        Right,
        Pulse,
        Circle
    }
}
