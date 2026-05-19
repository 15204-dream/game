using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LoveBeatGuide
{
    /// <summary>
    /// 引导高亮
    /// </summary>
    public class GuideHighlight : MonoBehaviour
    {
        [Header("高亮组件")]
        [SerializeField] private Image highlightImage;
        [SerializeField] private RectTransform highlightRect;

        [Header("动画配置")]
        [SerializeField] private bool enableAnimation = true;
        [SerializeField] private float animationSpeed = 1f;
        [SerializeField] private float borderWidth = 4f;

        [Header("颜色配置")]
        [SerializeField] private Color highlightColor = new Color(1f, 0.8f, 0.5f, 1f);
        [SerializeField] private Color glowColor = new Color(1f, 0.9f, 0.7f, 0.5f);
        [SerializeField] private Color pulseColor = new Color(1f, 0.6f, 0.3f, 0.8f);

        [Header("高亮样式")]
        [SerializeField] private HighlightStyle highlightStyle = HighlightStyle.Rectangle;

        private bool isHighlighting;
        private Transform targetTransform;
        private Vector3 targetOriginalSize;
        private float animationTime;
        private Coroutine updateCoroutine;

        public enum HighlightStyle
        {
            Rectangle,
            RoundedRectangle,
            Circle,
            Glow
        }

        private void Awake()
        {
            if (highlightRect == null)
            {
                highlightRect = GetComponent<RectTransform>();
            }

            if (highlightImage == null)
            {
                highlightImage = GetComponent<Image>();
            }

            if (highlightImage != null)
            {
                highlightImage.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (!enableAnimation || !isHighlighting)
                return;

            animationTime += Time.deltaTime * animationSpeed;
            UpdateAnimation();
        }

        /// <summary>
        /// 显示高亮
        /// </summary>
        public void ShowHighlight(string targetPath)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                Debug.LogWarning("高亮目标路径为空");
                return;
            }

            GameObject target = GameObject.Find(targetPath);
            if (target != null)
            {
                ShowHighlightAtTarget(target.GetComponent<RectTransform>());
            }
            else
            {
                Debug.LogWarning($"未找到高亮目标: {targetPath}");
            }
        }

        /// <summary>
        /// 在目标位置显示高亮
        /// </summary>
        public void ShowHighlightAtTarget(RectTransform target)
        {
            if (target == null)
                return;

            StopHighlight();

            targetTransform = target.GetComponent<RectTransform>();
            isHighlighting = true;
            animationTime = 0f;

            if (highlightImage != null)
            {
                highlightImage.gameObject.SetActive(true);
                UpdateHighlightSize();
            }

            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
            }

            updateCoroutine = StartCoroutine(UpdateHighlightPositionCoroutine());
        }

        /// <summary>
        /// 隐藏高亮
        /// </summary>
        public void HideHighlight()
        {
            StopHighlight();
        }

        /// <summary>
        /// 停止高亮
        /// </summary>
        private void StopHighlight()
        {
            isHighlighting = false;

            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
                updateCoroutine = null;
            }

            if (highlightImage != null)
            {
                highlightImage.gameObject.SetActive(false);
            }

            targetTransform = null;
        }

        /// <summary>
        /// 更新高亮大小
        /// </summary>
        private void UpdateHighlightSize()
        {
            if (targetTransform == null || highlightRect == null)
                return;

            Vector3[] corners = new Vector3[4];
            targetTransform.GetWorldCorners(corners);

            float width = Vector3.Distance(corners[0], corners[3]) + borderWidth * 2;
            float height = Vector3.Distance(corners[0], corners[1]) + borderWidth * 2;

            highlightRect.sizeDelta = new Vector2(width, height);
            targetOriginalSize = new Vector3(width, height, 0);
        }

        /// <summary>
        /// 更新高亮位置协程
        /// </summary>
        private IEnumerator UpdateHighlightPositionCoroutine()
        {
            while (isHighlighting && targetTransform != null)
            {
                UpdateHighlightPosition();
                UpdateHighlightSize();
                yield return null;
            }
        }

        /// <summary>
        /// 更新高亮位置
        /// </summary>
        private void UpdateHighlightPosition()
        {
            if (targetTransform == null || highlightRect == null)
                return;

            Vector3 targetPos = targetTransform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                highlightRect.parent as RectTransform,
                screenPos,
                null,
                out Vector2 localPos
            );

            highlightRect.localPosition = localPos;
        }

        /// <summary>
        /// 更新动画
        /// </summary>
        private void UpdateAnimation()
        {
            switch (highlightStyle)
            {
                case HighlightStyle.Rectangle:
                case HighlightStyle.RoundedRectangle:
                    UpdatePulseAnimation();
                    break;

                case HighlightStyle.Circle:
                    UpdateCircleAnimation();
                    break;

                case HighlightStyle.Glow:
                    UpdateGlowAnimation();
                    break;
            }
        }

        /// <summary>
        /// 更新脉冲动画
        /// </summary>
        private void UpdatePulseAnimation()
        {
            float pulse = Mathf.Sin(animationTime * Mathf.PI * 2f) * 0.1f;
            float scale = 1f + pulse;

            if (highlightRect != null)
            {
                highlightRect.localScale = Vector3.one * scale;
            }

            if (highlightImage != null)
            {
                Color color = Color.Lerp(highlightColor, pulseColor, (pulse + 0.1f) * 5f);
                highlightImage.color = color;
            }
        }

        /// <summary>
        /// 更新圆形动画
        /// </summary>
        private void UpdateCircleAnimation()
        {
            float rotation = animationTime * 360f;
            highlightRect.localRotation = Quaternion.Euler(0, 0, rotation);
        }

        /// <summary>
        /// 更新发光动画
        /// </summary>
        private void UpdateGlowAnimation()
        {
            float intensity = 0.5f + Mathf.Sin(animationTime * Mathf.PI * 3f) * 0.5f;

            if (highlightImage != null)
            {
                Color color = Color.Lerp(highlightColor, glowColor, intensity);
                highlightImage.color = color;
            }
        }

        /// <summary>
        /// 设置高亮样式
        /// </summary>
        public void SetHighlightStyle(HighlightStyle style)
        {
            highlightStyle = style;

            switch (style)
            {
                case HighlightStyle.RoundedRectangle:
                    if (highlightImage != null)
                    {
                        highlightImage.type = Image.Type.Sliced;
                    }
                    break;

                case HighlightStyle.Circle:
                    if (highlightImage != null)
                    {
                        highlightImage.type = Image.Type.Filled;
                    }
                    break;

                default:
                    if (highlightImage != null)
                    {
                        highlightImage.type = Image.Type.Sliced;
                    }
                    break;
            }
        }

        /// <summary>
        /// 设置高亮颜色
        /// </summary>
        public void SetHighlightColor(Color color)
        {
            highlightColor = color;
        }

        /// <summary>
        /// 设置边框宽度
        /// </summary>
        public void SetBorderWidth(float width)
        {
            borderWidth = width;
        }

        /// <summary>
        /// 设置动画速度
        /// </summary>
        public void SetAnimationSpeed(float speed)
        {
            animationSpeed = speed;
        }

        /// <summary>
        /// 启用/禁用动画
        /// </summary>
        public void SetAnimationEnabled(bool enabled)
        {
            enableAnimation = enabled;

            if (!enabled && highlightImage != null)
            {
                highlightImage.color = highlightColor;
                if (highlightRect != null)
                {
                    highlightRect.localScale = Vector3.one;
                }
            }
        }
    }
}
