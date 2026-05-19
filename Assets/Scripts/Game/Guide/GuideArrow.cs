using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LoveBeatGuide
{
    /// <summary>
    /// 引导箭头
    /// </summary>
    public class GuideArrow : MonoBehaviour
    {
        [Header("箭头组件")]
        [SerializeField] private Image arrowImage;
        [SerializeField] private RectTransform arrowRect;

        [Header("动画配置")]
        [SerializeField] private bool enableAnimation = true;
        [SerializeField] private float animationSpeed = 1f;
        [SerializeField] private float moveDistance = 20f;
        [SerializeField] private float rotationSpeed = 180f;

        [Header("箭头样式")]
        [SerializeField] private Sprite downArrowSprite;
        [SerializeField] private Sprite upArrowSprite;
        [SerializeField] private Sprite leftArrowSprite;
        [SerializeField] private Sprite rightArrowSprite;
        [SerializeField] private Sprite pulseArrowSprite;
        [SerializeField] private Sprite circleArrowSprite;

        [Header("颜色配置")]
        [SerializeField] private Color arrowColor = Color.white;
        [SerializeField] private Color glowColor = new Color(1f, 0.8f, 0.5f);

        private GuideArrowType currentArrowType = GuideArrowType.None;
        private bool isAnimating;
        private Vector3 originalPosition;
        private float animationTime;
        private Transform targetTransform;

        private void Awake()
        {
            if (arrowRect == null)
            {
                arrowRect = GetComponent<RectTransform>();
            }

            if (arrowImage == null)
            {
                arrowImage = GetComponent<Image>();
            }

            originalPosition = arrowRect != null ? arrowRect.localPosition : Vector3.zero;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!enableAnimation || !isAnimating)
                return;

            animationTime += Time.deltaTime * animationSpeed;
            UpdateAnimation();
        }

        /// <summary>
        /// 设置箭头类型
        /// </summary>
        public void SetArrowType(GuideArrowType type)
        {
            currentArrowType = type;

            if (arrowImage == null)
                return;

            switch (type)
            {
                case GuideArrowType.Down:
                    arrowImage.sprite = downArrowSprite;
                    arrowRect.localRotation = Quaternion.Euler(0, 0, 180);
                    break;

                case GuideArrowType.Up:
                    arrowImage.sprite = upArrowSprite;
                    arrowRect.localRotation = Quaternion.Euler(0, 0, 0);
                    break;

                case GuideArrowType.Left:
                    arrowImage.sprite = leftArrowSprite;
                    arrowRect.localRotation = Quaternion.Euler(0, 0, 90);
                    break;

                case GuideArrowType.Right:
                    arrowImage.sprite = rightArrowSprite;
                    arrowRect.localRotation = Quaternion.Euler(0, 0, -90);
                    break;

                case GuideArrowType.Pulse:
                    arrowImage.sprite = pulseArrowSprite;
                    break;

                case GuideArrowType.Circle:
                    arrowImage.sprite = circleArrowSprite;
                    break;

                default:
                    arrowImage.sprite = downArrowSprite;
                    break;
            }

            arrowImage.color = arrowColor;
        }

        /// <summary>
        /// 显示箭头
        /// </summary>
        public void ShowArrow(string targetPath = null)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                ShowAtCenter();
                return;
            }

            GameObject target = GameObject.Find(targetPath);
            if (target != null)
            {
                ShowAtTarget(target.transform);
            }
            else
            {
                ShowAtCenter();
            }
        }

        /// <summary>
        /// 在目标位置显示
        /// </summary>
        public void ShowAtTarget(Transform target)
        {
            targetTransform = target;
            gameObject.SetActive(true);
            isAnimating = true;
            animationTime = 0f;

            StartCoroutine(UpdatePositionCoroutine());
        }

        /// <summary>
        /// 在中心显示
        /// </summary>
        public void ShowAtCenter()
        {
            targetTransform = null;
            gameObject.SetActive(true);
            isAnimating = true;
            animationTime = 0f;

            if (arrowRect != null)
            {
                arrowRect.localPosition = originalPosition;
            }
        }

        /// <summary>
        /// 隐藏箭头
        /// </summary>
        public void HideArrow()
        {
            isAnimating = false;
            gameObject.SetActive(false);
            targetTransform = null;
        }

        /// <summary>
        /// 更新位置协程
        /// </summary>
        private IEnumerator UpdatePositionCoroutine()
        {
            while (isAnimating && targetTransform != null)
            {
                UpdatePosition();
                yield return null;
            }
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        private void UpdatePosition()
        {
            if (targetTransform == null || arrowRect == null)
                return;

            Vector3 targetPos = targetTransform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                arrowRect.parent as RectTransform,
                screenPos,
                null,
                out Vector2 localPos
            );

            Vector3 offset = GetArrowOffset();
            arrowRect.localPosition = localPos + offset;
        }

        /// <summary>
        /// 获取箭头偏移
        /// </summary>
        private Vector3 GetArrowOffset()
        {
            switch (currentArrowType)
            {
                case GuideArrowType.Down:
                    return new Vector3(0, -moveDistance, 0);
                case GuideArrowType.Up:
                    return new Vector3(0, moveDistance, 0);
                case GuideArrowType.Left:
                    return new Vector3(-moveDistance, 0, 0);
                case GuideArrowType.Right:
                    return new Vector3(moveDistance, 0, 0);
                default:
                    return new Vector3(0, -moveDistance, 0);
            }
        }

        /// <summary>
        /// 更新动画
        /// </summary>
        private void UpdateAnimation()
        {
            switch (currentArrowType)
            {
                case GuideArrowType.Down:
                case GuideArrowType.Up:
                case GuideArrowType.Left:
                case GuideArrowType.Right:
                    UpdateMoveAnimation();
                    break;

                case GuideArrowType.Pulse:
                    UpdatePulseAnimation();
                    break;

                case GuideArrowType.Circle:
                    UpdateCircleAnimation();
                    break;
            }
        }

        /// <summary>
        /// 更新移动动画
        /// </summary>
        private void UpdateMoveAnimation()
        {
            float offset = Mathf.Sin(animationTime * Mathf.PI * 2f) * moveDistance;

            Vector3 offsetDir = GetArrowOffset().normalized;
            Vector3 animatedOffset = offsetDir * offset;

            if (arrowRect != null)
            {
                Vector3 basePos = originalPosition;
                if (targetTransform != null)
                {
                    Vector3 targetPos = targetTransform.position;
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPos);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        arrowRect.parent as RectTransform,
                        screenPos,
                        null,
                        out Vector2 localPos
                    );
                    basePos = localPos + GetArrowOffset();
                }

                arrowRect.localPosition = basePos + animatedOffset;
            }
        }

        /// <summary>
        /// 更新脉冲动画
        /// </summary>
        private void UpdatePulseAnimation()
        {
            float scale = 1f + Mathf.Sin(animationTime * Mathf.PI * 4f) * 0.2f;

            if (arrowImage != null)
            {
                arrowImage.transform.localScale = Vector3.one * scale;
            }

            float alpha = 0.7f + Mathf.Sin(animationTime * Mathf.PI * 4f) * 0.3f;
            arrowImage.color = Color.Lerp(arrowColor, glowColor, alpha);
        }

        /// <summary>
        /// 更新旋转动画
        /// </summary>
        private void UpdateCircleAnimation()
        {
            if (arrowRect != null)
            {
                arrowRect.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// 设置箭头颜色
        /// </summary>
        public void SetArrowColor(Color color)
        {
            arrowColor = color;
            if (arrowImage != null)
            {
                arrowImage.color = color;
            }
        }

        /// <summary>
        /// 设置动画速度
        /// </summary>
        public void SetAnimationSpeed(float speed)
        {
            animationSpeed = speed;
        }

        /// <summary>
        /// 设置移动距离
        /// </summary>
        public void SetMoveDistance(float distance)
        {
            moveDistance = distance;
        }
    }
}
