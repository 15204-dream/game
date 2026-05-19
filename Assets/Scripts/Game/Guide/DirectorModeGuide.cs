using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveBeatGuide
{
    /// <summary>
    /// 导演模式引导控制器
    /// </summary>
    public class DirectorModeGuide : MonoBehaviour
    {
        private static DirectorModeGuide instance;
        public static DirectorModeGuide Instance => instance;

        [Header("引导阶段")]
        [SerializeField] private DirectorGuidePhase currentPhase = DirectorGuidePhase.None;
        [SerializeField] private DirectorGuidePhase completedPhase = DirectorGuidePhase.None;

        [Header("引导数据")]
        [SerializeField] private DirectorGuideData guideData;

        [Header("引导状态")]
        [SerializeField] private bool isGuideActive;
        [SerializeField] private int currentStepIndex;
        [SerializeField] private List<DirectorGuideStepInfo> completedSteps = new List<DirectorGuideStepInfo>();

        private Dictionary<DirectorGuidePhase, DirectorGuidePhaseConfig> phaseConfigs;
        private List<DirectorGuideStepInfo> currentPhaseSteps;

        public DirectorGuidePhase CurrentPhase => currentPhase;
        public bool IsGuideActive => isGuideActive;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            InitializeGuide();
        }

        private void Start()
        {
            LoadGuideProgress();
            CheckAndStartGuide();
        }

        /// <summary>
        /// 初始化引导系统
        /// </summary>
        private void InitializeGuide()
        {
            phaseConfigs = new Dictionary<DirectorGuidePhase, DirectorGuidePhaseConfig>();

            if (guideData != null)
            {
                foreach (var config in guideData.phaseConfigs)
                {
                    phaseConfigs[config.phase] = config;
                }
            }
        }

        /// <summary>
        /// 检查并开始引导
        /// </summary>
        private void CheckAndStartGuide()
        {
            if (completedPhase >= DirectorGuidePhase.AllCompleted)
                return;

            DirectorGuidePhase nextPhase = GetNextUncompletedPhase();
            if (nextPhase != DirectorGuidePhase.None)
            {
                StartPhase(nextPhase);
            }
        }

        /// <summary>
        /// 获取下一个未完成的阶段
        /// </summary>
        private DirectorGuidePhase GetNextUncompletedPhase()
        {
            if (completedPhase < DirectorGuidePhase.导演上任)
                return DirectorGuidePhase.导演上任;
            if (completedPhase < DirectorGuidePhase.首期策划)
                return DirectorGuidePhase.首期策划;
            if (completedPhase < DirectorGuidePhase.热度监控)
                return DirectorGuidePhase.热度监控;
            if (completedPhase < DirectorGuidePhase.活动安排)
                return DirectorGuidePhase.活动安排;
            if (completedPhase < DirectorGuidePhase.危机公关)
                return DirectorGuidePhase.危机公关;

            return DirectorGuidePhase.None;
        }

        /// <summary>
        /// 开始指定阶段
        /// </summary>
        public void StartPhase(DirectorGuidePhase phase)
        {
            if (!phaseConfigs.ContainsKey(phase))
            {
                Debug.LogWarning($"导演引导阶段配置不存在: {phase}");
                return;
            }

            currentPhase = phase;
            isGuideActive = true;
            currentStepIndex = 0;

            DirectorGuidePhaseConfig config = phaseConfigs[phase];
            currentPhaseSteps = config.steps;

            if (config.onPhaseStart != null)
            {
                config.onPhaseStart.Invoke();
            }

            ShowPhaseDialogue(phase);
            StartCurrentStep();
        }

        /// <summary>
        /// 显示阶段对话
        /// </summary>
        private void ShowPhaseDialogue(DirectorGuidePhase phase)
        {
            if (GuideManager.Instance != null && GuideManager.Instance.IsGuiding)
            {
                return;
            }

            DirectorPhaseDialogue dialogue = GetPhaseDialogue(phase);
            if (dialogue != null)
            {
                GuideManager.Instance?.ShowStepUI(null);
            }
        }

        /// <summary>
        /// 获取阶段对话
        /// </summary>
        private DirectorPhaseDialogue GetPhaseDialogue(DirectorGuidePhase phase)
        {
            if (guideData != null)
            {
                return guideData.GetPhaseDialogue(phase);
            }
            return null;
        }

        /// <summary>
        /// 开始当前步骤
        /// </summary>
        private void StartCurrentStep()
        {
            if (currentPhaseSteps == null || currentStepIndex >= currentPhaseSteps.Count)
            {
                CompleteCurrentPhase();
                return;
            }

            DirectorGuideStepInfo stepInfo = currentPhaseSteps[currentStepIndex];
            ExecuteStep(stepInfo);
        }

        /// <summary>
        /// 执行步骤
        /// </summary>
        private void ExecuteStep(DirectorGuideStepInfo stepInfo)
        {
            if (GuideManager.Instance != null)
            {
                GuideManager.Instance.ShowStepUI(null);
            }
        }

        /// <summary>
        /// 完成当前步骤
        /// </summary>
        public void CompleteCurrentStep(string stepId = null)
        {
            if (currentPhaseSteps == null || currentStepIndex >= currentPhaseSteps.Count)
                return;

            DirectorGuideStepInfo stepInfo = currentPhaseSteps[currentStepIndex];

            if (stepId != null && stepInfo.stepId != stepId)
                return;

            completedSteps.Add(stepInfo);
            currentStepIndex++;

            if (currentStepIndex >= currentPhaseSteps.Count)
            {
                CompleteCurrentPhase();
            }
            else
            {
                StartCurrentStep();
            }
        }

        /// <summary>
        /// 完成当前阶段
        /// </summary>
        private void CompleteCurrentPhase()
        {
            completedPhase = currentPhase;

            if (phaseConfigs.ContainsKey(currentPhase))
            {
                DirectorGuidePhaseConfig config = phaseConfigs[currentPhase];
                if (config.onPhaseComplete != null)
                {
                    config.onPhaseComplete.Invoke();
                }

                if (config.rewards != null && config.rewards.Count > 0)
                {
                    GuideRewardSystem.Instance?.GrantRewards(config.rewards);
                }
            }

            SaveGuideProgress();
            isGuideActive = false;

            DirectorGuidePhase nextPhase = GetNextUncompletedPhase();
            if (nextPhase != DirectorGuidePhase.None)
            {
                StartPhase(nextPhase);
            }
        }

        /// <summary>
        /// 跳过当前阶段
        /// </summary>
        public void SkipCurrentPhase()
        {
            if (!isGuideActive)
                return;

            DirectorGuidePhaseConfig config = phaseConfigs[currentPhase];
            if (config != null && config.canSkip)
            {
                completedPhase = currentPhase;
                isGuideActive = false;
                SaveGuideProgress();

                DirectorGuidePhase nextPhase = GetNextUncompletedPhase();
                if (nextPhase != DirectorGuidePhase.None)
                {
                    StartPhase(nextPhase);
                }
            }
        }

        /// <summary>
        /// 加载引导进度
        /// </summary>
        private void LoadGuideProgress()
        {
            string phaseKey = "DirectorGuide_CompletedPhase";
            if (PlayerPrefs.HasKey(phaseKey))
            {
                completedPhase = (DirectorGuidePhase)PlayerPrefs.GetInt(phaseKey);
            }
        }

        /// <summary>
        /// 保存引导进度
        /// </summary>
        private void SaveGuideProgress()
        {
            string phaseKey = "DirectorGuide_CompletedPhase";
            PlayerPrefs.SetInt(phaseKey, (int)completedPhase);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 重置引导进度
        /// </summary>
        public void ResetGuideProgress()
        {
            completedPhase = DirectorGuidePhase.None;
            currentPhase = DirectorGuidePhase.None;
            isGuideActive = false;
            currentStepIndex = 0;
            completedSteps.Clear();

            PlayerPrefs.DeleteKey("DirectorGuide_CompletedPhase");
        }

        /// <summary>
        /// 获取阶段进度
        /// </summary>
        public float GetPhaseProgress(DirectorGuidePhase phase)
        {
            if (!phaseConfigs.ContainsKey(phase))
                return 0f;

            DirectorGuidePhaseConfig config = phaseConfigs[phase];
            if (config.steps == null || config.steps.Count == 0)
                return 1f;

            int completed = 0;
            foreach (var step in config.steps)
            {
                if (completedSteps.Exists(s => s.stepId == step.stepId))
                    completed++;
            }

            return (float)completed / config.steps.Count;
        }

        /// <summary>
        /// 获取总体进度
        /// </summary>
        public float GetTotalProgress()
        {
            int totalSteps = 0;
            int completedStepsCount = 0;

            foreach (var config in phaseConfigs.Values)
            {
                if (config.steps != null)
                {
                    totalSteps += config.steps.Count;
                    completedStepsCount += config.steps.FindAll(s => 
                        completedSteps.Exists(c => c.stepId == s.stepId)).Count;
                }
            }

            return totalSteps > 0 ? (float)completedStepsCount / totalSteps : 1f;
        }

        /// <summary>
        /// 检查阶段是否完成
        /// </summary>
        public bool IsPhaseCompleted(DirectorGuidePhase phase)
        {
            return completedPhase >= phase;
        }

        /// <summary>
        /// 引导步骤开始回调
        /// </summary>
        public void OnGuideStepStarted(GuideStep step)
        {
        }

        /// <summary>
        /// 引导步骤更新回调
        /// </summary>
        public void OnGuideStepUpdated(GuideStep step)
        {
        }

        /// <summary>
        /// 引导步骤完成回调
        /// </summary>
        public void OnGuideStepCompleted(GuideStep step)
        {
        }

        /// <summary>
        /// 引导步骤结束回调
        /// </summary>
        public void OnGuideStepEnded(GuideStep step)
        {
        }
    }

    /// <summary>
    /// 导演引导阶段
    /// </summary>
    public enum DirectorGuidePhase
    {
        None,
        导演上任,
        首期策划,
        热度监控,
        活动安排,
        危机公关,
        AllCompleted
    }

    /// <summary>
    /// 导演引导步骤信息
    /// </summary>
    [Serializable]
    public class DirectorGuideStepInfo
    {
        public string stepId;
        public string stepName;
        public string description;
        public DirectorGuideStepType stepType;
        public string targetPath;
        public float delay;
        public bool canSkip;
        public Action onComplete;
    }

    /// <summary>
    /// 导演引导步骤类型
    /// </summary>
    public enum DirectorGuideStepType
    {
        Dialogue,
        Tutorial,
        Assignment,
        Monitoring,
        Crisis
    }

    /// <summary>
    /// 导演引导阶段配置
    /// </summary>
    [Serializable]
    public class DirectorGuidePhaseConfig
    {
        public DirectorGuidePhase phase;
        public string phaseName;
        public string description;
        public List<DirectorGuideStepInfo> steps;
        public List<GuideReward> rewards;
        public bool canSkip;
        public UnityEngine.Events.UnityEvent onPhaseStart;
        public UnityEngine.Events.UnityEvent onPhaseComplete;
    }

    /// <summary>
    /// 导演阶段对话
    /// </summary>
    [Serializable]
    public class DirectorPhaseDialogue
    {
        public DirectorGuidePhase phase;
        public List<GuideDialogueLine> dialogues;
    }
}
