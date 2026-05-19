using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveBeatGuide
{
    /// <summary>
    /// 嘉宾模式引导控制器
    /// </summary>
    public class GuestModeGuide : MonoBehaviour
    {
        private static GuestModeGuide instance;
        public static GuestModeGuide Instance => instance;

        [Header("引导阶段")]
        [SerializeField] private GuestGuidePhase currentPhase = GuestGuidePhase.None;
        [SerializeField] private GuestGuidePhase completedPhase = GuestGuidePhase.None;

        [Header("引导数据")]
        [SerializeField] private GuestGuideData guideData;

        [Header("引导状态")]
        [SerializeField] private bool isGuideActive;
        [SerializeField] private int currentStepIndex;
        [SerializeField] private List<GuestGuideStepInfo> completedSteps = new List<GuestGuideStepInfo>();

        private Dictionary<GuestGuidePhase, GuestGuidePhaseConfig> phaseConfigs;
        private List<GuestGuideStepInfo> currentPhaseSteps;

        public GuestGuidePhase CurrentPhase => currentPhase;
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
            phaseConfigs = new Dictionary<GuestGuidePhase, GuestGuidePhaseConfig>();

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
            if (completedPhase >= GuestGuidePhase.AllCompleted)
                return;

            GuestGuidePhase nextPhase = GetNextUncompletedPhase();
            if (nextPhase != GuestGuidePhase.None)
            {
                StartPhase(nextPhase);
            }
        }

        /// <summary>
        /// 获取下一个未完成的阶段
        /// </summary>
        private GuestGuidePhase GetNextUncompletedPhase()
        {
            if (completedPhase < GuestGuidePhase.入场准备)
                return GuestGuidePhase.入场准备;
            if (completedPhase < GuestGuidePhase.入住心动小屋)
                return GuestGuidePhase.入住心动小屋;
            if (completedPhase < GuestGuidePhase.心动信箱)
                return GuestGuidePhase.心动信箱;
            if (completedPhase < GuestGuidePhase.任务系统)
                return GuestGuidePhase.任务系统;
            if (completedPhase < GuestGuidePhase.社交互动)
                return GuestGuidePhase.社交互动;

            return GuestGuidePhase.None;
        }

        /// <summary>
        /// 开始指定阶段
        /// </summary>
        public void StartPhase(GuestGuidePhase phase)
        {
            if (!phaseConfigs.ContainsKey(phase))
            {
                Debug.LogWarning($"嘉宾引导阶段配置不存在: {phase}");
                return;
            }

            currentPhase = phase;
            isGuideActive = true;
            currentStepIndex = 0;

            GuestGuidePhaseConfig config = phaseConfigs[phase];
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
        private void ShowPhaseDialogue(GuestGuidePhase phase)
        {
            if (GuideManager.Instance != null && GuideManager.Instance.IsGuiding)
            {
                return;
            }

            GuestPhaseDialogue dialogue = GetPhaseDialogue(phase);
            if (dialogue != null)
            {
                GuideManager.Instance?.ShowStepUI(null);
            }
        }

        /// <summary>
        /// 获取阶段对话
        /// </summary>
        private GuestPhaseDialogue GetPhaseDialogue(GuestGuidePhase phase)
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

            GuestGuideStepInfo stepInfo = currentPhaseSteps[currentStepIndex];
            ExecuteStep(stepInfo);
        }

        /// <summary>
        /// 执行步骤
        /// </summary>
        private void ExecuteStep(GuestGuideStepInfo stepInfo)
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

            GuestGuideStepInfo stepInfo = currentPhaseSteps[currentStepIndex];

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
                GuestGuidePhaseConfig config = phaseConfigs[currentPhase];
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

            GuestGuidePhase nextPhase = GetNextUncompletedPhase();
            if (nextPhase != GuestGuidePhase.None)
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

            GuestGuidePhaseConfig config = phaseConfigs[currentPhase];
            if (config != null && config.canSkip)
            {
                completedPhase = currentPhase;
                isGuideActive = false;
                SaveGuideProgress();

                GuestGuidePhase nextPhase = GetNextUncompletedPhase();
                if (nextPhase != GuestGuidePhase.None)
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
            string phaseKey = "GuestGuide_CompletedPhase";
            if (PlayerPrefs.HasKey(phaseKey))
            {
                completedPhase = (GuestGuidePhase)PlayerPrefs.GetInt(phaseKey);
            }
        }

        /// <summary>
        /// 保存引导进度
        /// </summary>
        private void SaveGuideProgress()
        {
            string phaseKey = "GuestGuide_CompletedPhase";
            PlayerPrefs.SetInt(phaseKey, (int)completedPhase);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 重置引导进度
        /// </summary>
        public void ResetGuideProgress()
        {
            completedPhase = GuestGuidePhase.None;
            currentPhase = GuestGuidePhase.None;
            isGuideActive = false;
            currentStepIndex = 0;
            completedSteps.Clear();

            PlayerPrefs.DeleteKey("GuestGuide_CompletedPhase");
        }

        /// <summary>
        /// 获取阶段进度
        /// </summary>
        public float GetPhaseProgress(GuestGuidePhase phase)
        {
            if (!phaseConfigs.ContainsKey(phase))
                return 0f;

            GuestGuidePhaseConfig config = phaseConfigs[phase];
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
        public bool IsPhaseCompleted(GuestGuidePhase phase)
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
    /// 嘉宾引导阶段
    /// </summary>
    public enum GuestGuidePhase
    {
        None,
        入场准备,
        入住心动小屋,
        心动信箱,
        任务系统,
        社交互动,
        AllCompleted
    }

    /// <summary>
    /// 嘉宾引导步骤信息
    /// </summary>
    [Serializable]
    public class GuestGuideStepInfo
    {
        public string stepId;
        public string stepName;
        public string description;
        public GuestGuideStepType stepType;
        public string targetPath;
        public float delay;
        public bool canSkip;
        public Action onComplete;
    }

    /// <summary>
    /// 嘉宾引导步骤类型
    /// </summary>
    public enum GuestGuideStepType
    {
        Dialogue,
        Tutorial,
        Interaction,
        Task,
        Quiz
    }

    /// <summary>
    /// 嘉宾引导阶段配置
    /// </summary>
    [Serializable]
    public class GuestGuidePhaseConfig
    {
        public GuestGuidePhase phase;
        public string phaseName;
        public string description;
        public List<GuestGuideStepInfo> steps;
        public List<GuideReward> rewards;
        public bool canSkip;
        public UnityEngine.Events.UnityEvent onPhaseStart;
        public UnityEngine.Events.UnityEvent onPhaseComplete;
    }

    /// <summary>
    /// 嘉宾阶段对话
    /// </summary>
    [Serializable]
    public class GuestPhaseDialogue
    {
        public GuestGuidePhase phase;
        public List<GuideDialogueLine> dialogues;
    }

    /// <summary>
    /// 引导对话行
    /// </summary>
    [Serializable]
    public class GuideDialogueLine
    {
        public string speaker;
        public string text;
        public float displayTime;
    }
}
