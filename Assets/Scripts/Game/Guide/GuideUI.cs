using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LoveBeatGuide
{
    /// <summary>
    /// 引导界面
    /// </summary>
    public class GuideUI : MonoBehaviour
    {
        private static GuideUI instance;
        public static GuideUI Instance => instance;

        [Header("UI组件")]
        [SerializeField] private GameObject guidePanel;
        [SerializeField] private GuideDialogue guideDialogue;
        [SerializeField] private GuideArrow guideArrow;
        [SerializeField] private GuideHighlight guideHighlight;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button completeButton;
        [SerializeField] private TextMeshProUGUI stepTitleText;
        [SerializeField] private TextMeshProUGUI stepDescriptionText;
        [SerializeField] private Image progressFill;
        [SerializeField] private TextMeshProUGUI progressText;

        [Header("遮罩")]
        [SerializeField] private GameObject maskPanel;
        [SerializeField] private Material maskMaterial;
        [SerializeField] private Color maskColor = new Color(0, 0, 0, 0.5f);

        [Header("配置")]
        [SerializeField] private bool showSkipButton = true;
        [SerializeField] private bool showCompleteButton = true;
        [SerializeField] private bool showProgress = true;

        private GuideStep currentStep;
        private bool isUIVisible;
        private List<Canvas> temporaryCanvases = new List<Canvas>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            InitializeUI();
        }

        private void Start()
        {
            SetupButtons();
            HideUI();
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            if (guidePanel != null)
            {
                guidePanel.SetActive(false);
            }

            if (maskPanel != null)
            {
                maskPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 设置按钮事件
        /// </summary>
        private void SetupButtons()
        {
            if (skipButton != null)
            {
                skipButton.onClick.RemoveAllListeners();
                skipButton.onClick.AddListener(OnSkipButtonClicked);
            }

            if (completeButton != null)
            {
                completeButton.onClick.RemoveAllListeners();
                completeButton.onClick.AddListener(OnCompleteButtonClicked);
            }
        }

        /// <summary>
        /// 显示步骤
        /// </summary>
        public void ShowStep(GuideStep step)
        {
            if (step == null)
                return;

            currentStep = step;
            isUIVisible = true;

            UpdateStepUI(step);
            ShowUI();

            if (guideHighlight != null)
            {
                if (step.Data.targetPaths.Count > 0)
                {
                    guideHighlight.ShowHighlight(step.Data.targetPaths[0]);
                }
            }

            if (guideArrow != null && step.Data.arrowType != GuideArrowType.None)
            {
                guideArrow.ShowArrow(step.Data.targetPaths.Count > 0 ? step.Data.targetPaths[0] : null);
            }
        }

        /// <summary>
        /// 更新步骤UI
        /// </summary>
        private void UpdateStepUI(GuideStep step)
        {
            if (stepTitleText != null)
            {
                stepTitleText.text = step.GetStepTitle();
            }

            if (stepDescriptionText != null)
            {
                stepDescriptionText.text = step.GetStepDescription();
            }

            if (showSkipButton && skipButton != null)
            {
                skipButton.gameObject.SetActive(step.Data.canSkip);
            }

            if (showCompleteButton && completeButton != null)
            {
                completeButton.gameObject.SetActive(step.Data.requireManualComplete);
            }
        }

        /// <summary>
        /// 显示UI
        /// </summary>
        private void ShowUI()
        {
            if (guidePanel != null)
            {
                guidePanel.SetActive(true);
            }

            if (maskPanel != null)
            {
                maskPanel.SetActive(true);
                UpdateMask();
            }

            if (guideDialogue != null)
            {
                guideDialogue.ShowDialogue(currentStep?.GetStepDescription() ?? "");
            }
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public void HideUI()
        {
            isUIVisible = false;

            if (guidePanel != null)
            {
                guidePanel.SetActive(false);
            }

            if (maskPanel != null)
            {
                maskPanel.SetActive(false);
            }

            if (guideDialogue != null)
            {
                guideDialogue.HideDialogue();
            }

            if (guideHighlight != null)
            {
                guideHighlight.HideHighlight();
            }

            if (guideArrow != null)
            {
                guideArrow.HideArrow();
            }
        }

        /// <summary>
        /// 隐藏步骤
        /// </summary>
        public void HideStep()
        {
            HideUI();
            currentStep = null;
        }

        /// <summary>
        /// 更新遮罩
        /// </summary>
        private void UpdateMask()
        {
            if (maskPanel == null || currentStep == null)
                return;

            if (currentStep.Data.targetPaths.Count > 0)
            {
                RectTransform targetRect = GetTargetRect(currentStep.Data.targetPaths[0]);
                if (targetRect != null)
                {
                    UpdateMaskHole(targetRect);
                }
            }
        }

        /// <summary>
        /// 更新遮罩孔洞
        /// </summary>
        private void UpdateMaskHole(RectTransform target)
        {
            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);

            float minX = corners[0].x;
            float maxX = corners[2].x;
            float minY = corners[0].y;
            float maxY = corners[2].y;

            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;
            float width = maxX - minX + 20f;
            float height = maxY - minY + 20f;
        }

        /// <summary>
        /// 获取目标RectTransform
        /// </summary>
        private RectTransform GetTargetRect(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            GameObject target = GameObject.Find(path);
            if (target != null)
            {
                return target.GetComponent<RectTransform>();
            }

            return null;
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        public void UpdateProgress(int current, int total)
        {
            if (!showProgress)
                return;

            if (progressFill != null)
            {
                progressFill.fillAmount = (float)current / total;
            }

            if (progressText != null)
            {
                progressText.text = $"{current}/{total}";
            }
        }

        /// <summary>
        /// 跳过按钮点击
        /// </summary>
        private void OnSkipButtonClicked()
        {
            if (currentStep != null)
            {
                currentStep.SkipStep();
            }
        }

        /// <summary>
        /// 完成按钮点击
        /// </summary>
        private void OnCompleteButtonClicked()
        {
            if (currentStep != null)
            {
                currentStep.CompleteStep();
            }
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        public void ShowDialogue(string title, string content)
        {
            if (guideDialogue != null)
            {
                guideDialogue.ShowDialogue(content, title);
            }
        }

        /// <summary>
        /// 显示高亮
        /// </summary>
        public void ShowHighlight(string targetPath)
        {
            if (guideHighlight != null)
            {
                guideHighlight.ShowHighlight(targetPath);
            }
        }

        /// <summary>
        /// 隐藏高亮
        /// </summary>
        public void HideHighlight()
        {
            if (guideHighlight != null)
            {
                guideHighlight.HideHighlight();
            }
        }

        /// <summary>
        /// 显示箭头
        /// </summary>
        public void ShowArrow(string targetPath, GuideArrowType arrowType = GuideArrowType.Down)
        {
            if (guideArrow != null)
            {
                guideArrow.SetArrowType(arrowType);
                guideArrow.ShowArrow(targetPath);
            }
        }

        /// <summary>
        /// 隐藏箭头
        /// </summary>
        public void HideArrow()
        {
            if (guideArrow != null)
            {
                guideArrow.HideArrow();
            }
        }

        /// <summary>
        /// 设置跳过按钮可见性
        /// </summary>
        public void SetSkipButtonVisible(bool visible)
        {
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// 设置完成按钮可见性
        /// </summary>
        public void SetCompleteButtonVisible(bool visible)
        {
            if (completeButton != null)
            {
                completeButton.gameObject.SetActive(visible);
            }
        }
    }
}
