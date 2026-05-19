using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LoveBeatGuide
{
    /// <summary>
    /// 引导管理器
    /// </summary>
    public class GuideManager : MonoBehaviour
    {
        private static GuideManager instance;
        public static GuideManager Instance => instance;

        [Header("引导配置")]
        [SerializeField] private bool enableGuide = true;
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private TextAsset guestGuideConfig;
        [SerializeField] private TextAsset directorGuideConfig;

        [Header("引导UI引用")]
        [SerializeField] private GuideUI guideUI;
        [SerializeField] private GuideAssistant guideAssistant;

        private Dictionary<string, GuideSequence> sequences = new Dictionary<string, GuideSequence>();
        private GuideSequence currentSequence;
        private GuideStep currentStep;
        private bool isGuiding;
        private float lastUpdateTime;

        public bool IsGuiding => isGuiding;
        public GuideSequence CurrentSequence => currentSequence;
        public GuideStep CurrentStep => currentStep;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeManager();
        }

        private void Start()
        {
            LoadGuideConfigurations();
            RegisterDefaultTriggers();
            
            if (enableGuide)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                CheckAutoStartSequences();
            }
        }

        private void Update()
        {
            if (!isGuiding || currentSequence == null)
                return;

            currentSequence.UpdateSequence();
            lastUpdateTime = Time.time;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        private void InitializeManager()
        {
            if (guideUI == null)
            {
                guideUI = GetComponent<GuideUI>();
                if (guideUI == null)
                {
                    guideUI = gameObject.AddComponent<GuideUI>();
                }
            }

            if (guideAssistant == null)
            {
                guideAssistant = GetComponent<GuideAssistant>();
                if (guideAssistant == null)
                {
                    guideAssistant = gameObject.AddComponent<GuideAssistant>();
                }
            }
        }

        /// <summary>
        /// 加载引导配置
        /// </summary>
        private void LoadGuideConfigurations()
        {
            if (guestGuideConfig != null)
            {
                LoadGuestGuideConfig(guestGuideConfig.text);
            }

            if (directorGuideConfig != null)
            {
                LoadDirectorGuideConfig(directorGuideConfig.text);
            }
        }

        /// <summary>
        /// 加载嘉宾引导配置
        /// </summary>
        private void LoadGuestGuideConfig(string json)
        {
            try
            {
                GuestGuideConfigData config = JsonUtility.FromJson<GuestGuideConfigData>(json);
                if (config != null && config.sequences != null)
                {
                    foreach (var sequenceData in config.sequences)
                    {
                        GuideSequence sequence = new GuestGuideSequence(sequenceData, this);
                        sequences[sequence.SequenceId] = sequence;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"加载嘉宾引导配置失败: {e.Message}");
            }
        }

        /// <summary>
        /// 加载导演引导配置
        /// </summary>
        private void LoadDirectorGuideConfig(string json)
        {
            try
            {
                DirectorGuideConfigData config = JsonUtility.FromJson<DirectorGuideConfigData>(json);
                if (config != null && config.sequences != null)
                {
                    foreach (var sequenceData in config.sequences)
                    {
                        GuideSequence sequence = new DirectorGuideSequence(sequenceData, this);
                        sequences[sequence.SequenceId] = sequence;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"加载导演引导配置失败: {e.Message}");
            }
        }

        /// <summary>
        /// 注册默认触发器
        /// </summary>
        private void RegisterDefaultTriggers()
        {
            GuideTriggerManager.Instance.RegisterTrigger(new EventTrigger("game_start", "GameStart"));
            GuideTriggerManager.Instance.RegisterTrigger(new EventTrigger("first_login", "FirstLogin"));
            GuideTriggerManager.Instance.RegisterTrigger(new EventTrigger("guest_mode_start", "GuestModeStart"));
            GuideTriggerManager.Instance.RegisterTrigger(new EventTrigger("director_mode_start", "DirectorModeStart"));
        }

        /// <summary>
        /// 场景加载回调
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!enableGuide)
                return;

            CheckAutoStartSequences();
        }

        /// <summary>
        /// 检查自动开始序列
        /// </summary>
        private void CheckAutoStartSequences()
        {
            foreach (var sequence in sequences.Values)
            {
                if (sequence.Data.autoStart && !sequence.IsCompleted())
                {
                    if (sequence.Data.trigger != null)
                    {
                        bool canStart = false;

                        switch (sequence.Data.trigger.type)
                        {
                            case GuideTriggerType.SceneLoad:
                                canStart = SceneManager.GetActiveScene().name == sequence.Data.trigger.sceneName;
                                break;
                            case GuideTriggerType.Manual:
                                canStart = true;
                                break;
                            case GuideTriggerType.Event:
                                canStart = false;
                                break;
                        }

                        if (canStart)
                        {
                            StartSequence(sequence.SequenceId);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 开始引导序列
        /// </summary>
        public void StartSequence(string sequenceId)
        {
            if (!enableGuide || !sequences.ContainsKey(sequenceId))
            {
                if (showDebugInfo)
                    Debug.Log($"无法开始引导序列: {sequenceId}");
                return;
            }

            if (isGuiding && currentSequence != null)
            {
                currentSequence.PauseSequence();
            }

            currentSequence = sequences[sequenceId];
            currentSequence.StartSequence();
            isGuiding = true;

            if (showDebugInfo)
                Debug.Log($"开始引导序列: {sequenceId}");
        }

        /// <summary>
        /// 继续引导序列
        /// </summary>
        public void ResumeSequence()
        {
            if (currentSequence != null)
            {
                currentSequence.ResumeSequence();
                isGuiding = true;
            }
        }

        /// <summary>
        /// 暂停引导序列
        /// </summary>
        public void PauseSequence()
        {
            if (currentSequence != null)
            {
                currentSequence.PauseSequence();
            }
        }

        /// <summary>
        /// 取消当前引导
        /// </summary>
        public void CancelCurrentGuide()
        {
            if (currentSequence != null)
            {
                currentSequence.CancelSequence();
                isGuiding = false;
                currentSequence = null;
                currentStep = null;
                HideStepUI(null);
            }
        }

        /// <summary>
        /// 跳过当前步骤
        /// </summary>
        public void SkipCurrentStep()
        {
            if (currentSequence != null)
            {
                currentSequence.SkipCurrentStep();
            }
        }

        /// <summary>
        /// 完成当前步骤
        /// </summary>
        public void CompleteCurrentStep()
        {
            if (currentStep != null)
            {
                currentStep.CompleteStep();
            }
        }

        /// <summary>
        /// 显示步骤UI
        /// </summary>
        public void ShowStepUI(GuideStep step)
        {
            currentStep = step;

            if (guideUI != null)
            {
                guideUI.ShowStep(step);
            }

            if (guideAssistant != null && step != null)
            {
                guideAssistant.ShowAssistant(step.GetStepTitle(), step.GetStepDescription());
            }
        }

        /// <summary>
        /// 隐藏步骤UI
        /// </summary>
        public void HideStepUI(GuideStep step)
        {
            if (guideUI != null)
            {
                guideUI.HideStep();
            }

            if (guideAssistant != null)
            {
                guideAssistant.HideAssistant();
            }
        }

        /// <summary>
        /// 序列步骤改变回调
        /// </summary>
        public void OnSequenceStepChanged(GuideSequence sequence, GuideStep step)
        {
            if (showDebugInfo)
                Debug.Log($"引导步骤改变: {step.StepId}");
        }

        /// <summary>
        /// 序列完成回调
        /// </summary>
        public void OnSequenceCompleted(GuideSequence sequence)
        {
            if (showDebugInfo)
                Debug.Log($"引导序列完成: {sequence.SequenceId}");

            isGuiding = false;
            currentSequence = null;
            currentStep = null;

            if (guideAssistant != null)
            {
                guideAssistant.ShowCompletionMessage(sequence.Data.sequenceName);
            }
        }

        /// <summary>
        /// 序列取消回调
        /// </summary>
        public void OnSequenceCancelled(GuideSequence sequence)
        {
            if (showDebugInfo)
                Debug.Log($"引导序列取消: {sequence.SequenceId}");
        }

        /// <summary>
        /// 通知触发器激活
        /// </summary>
        public void NotifyTriggerActivated(GuideTrigger trigger)
        {
            if (showDebugInfo)
                Debug.Log($"触发器激活: {trigger.TriggerId}");
        }

        /// <summary>
        /// 注册自定义序列
        /// </summary>
        public void RegisterCustomSequence(GuideSequence sequence)
        {
            if (!string.IsNullOrEmpty(sequence.SequenceId))
            {
                sequences[sequence.SequenceId] = sequence;
            }
        }

        /// <summary>
        /// 获取序列
        /// </summary>
        public GuideSequence GetSequence(string sequenceId)
        {
            return sequences.ContainsKey(sequenceId) ? sequences[sequenceId] : null;
        }

        /// <summary>
        /// 检查引导是否完成
        /// </summary>
        public bool IsGuideCompleted(string sequenceId)
        {
            return sequences.ContainsKey(sequenceId) && sequences[sequenceId].IsCompleted();
        }

        /// <summary>
        /// 重置引导
        /// </summary>
        public void ResetGuide(string sequenceId)
        {
            if (sequences.ContainsKey(sequenceId))
            {
                sequences[sequenceId].ResetProgress();
            }
        }

        /// <summary>
        /// 重置所有引导
        /// </summary>
        public void ResetAllGuides()
        {
            foreach (var sequence in sequences.Values)
            {
                sequence.ResetProgress();
            }
        }

        /// <summary>
        /// 获取当前引导进度
        /// </summary>
        public float GetCurrentGuideProgress()
        {
            return currentSequence != null ? currentSequence.GetProgress() : 0f;
        }

        /// <summary>
        /// 启用禁用引导
        /// </summary>
        public void SetGuideEnabled(bool enabled)
        {
            enableGuide = enabled;

            if (!enabled && isGuiding)
            {
                CancelCurrentGuide();
            }
        }
    }

    /// <summary>
    /// 嘉宾引导配置数据
    /// </summary>
    [Serializable]
    public class GuestGuideConfigData
    {
        public List<GuideSequenceData> sequences;
    }

    /// <summary>
    /// 导演引导配置数据
    /// </summary>
    [Serializable]
    public class DirectorGuideConfigData
    {
        public List<GuideSequenceData> sequences;
    }

    /// <summary>
    /// 嘉宾引导序列
    /// </summary>
    public class GuestGuideSequence : GuideSequence
    {
        public GuestGuideSequence(GuideSequenceData data, GuideManager manager) : base(data, manager) { }

        protected override GuideStep CreateStepInstance(GuideStepData stepData)
        {
            return new GuestGuideStep(stepData, this);
        }
    }

    /// <summary>
    /// 导演引导序列
    /// </summary>
    public class DirectorGuideSequence : GuideSequence
    {
        public DirectorGuideSequence(GuideSequenceData data, GuideManager manager) : base(data, manager) { }

        protected override GuideStep CreateStepInstance(GuideStepData stepData)
        {
            return new DirectorGuideStep(stepData, this);
        }
    }

    /// <summary>
    /// 嘉宾引导步骤
    /// </summary>
    public class GuestGuideStep : GuideStep
    {
        private GuideSequence parent;

        public GuestGuideStep(GuideStepData stepData, GuideSequence parent) : base(stepData)
        {
            this.parent = parent;
        }

        protected override void OnStepStartInternal()
        {
            GuideManager.Instance?.ShowStepUI(this);
            GuestModeGuide.Instance?.OnGuideStepStarted(this);
        }

        protected override void OnStepUpdateInternal()
        {
            GuestModeGuide.Instance?.OnGuideStepUpdated(this);
        }

        protected override void OnStepCompleteInternal()
        {
            GuideManager.Instance?.HideStepUI(this);
            GuestModeGuide.Instance?.OnGuideStepCompleted(this);
        }

        protected override void OnStepEndInternal()
        {
            GuideManager.Instance?.HideStepUI(this);
            GuestModeGuide.Instance?.OnGuideStepEnded(this);
        }
    }

    /// <summary>
    /// 导演引导步骤
    /// </summary>
    public class DirectorGuideStep : GuideStep
    {
        private GuideSequence parent;

        public DirectorGuideStep(GuideStepData stepData, GuideSequence parent) : base(stepData)
        {
            this.parent = parent;
        }

        protected override void OnStepStartInternal()
        {
            GuideManager.Instance?.ShowStepUI(this);
            DirectorModeGuide.Instance?.OnGuideStepStarted(this);
        }

        protected override void OnStepUpdateInternal()
        {
            DirectorModeGuide.Instance?.OnGuideStepUpdated(this);
        }

        protected override void OnStepCompleteInternal()
        {
            GuideManager.Instance?.HideStepUI(this);
            DirectorModeGuide.Instance?.OnGuideStepCompleted(this);
        }

        protected override void OnStepEndInternal()
        {
            GuideManager.Instance?.HideStepUI(this);
            DirectorModeGuide.Instance?.OnGuideStepEnded(this);
        }
    }
}
