using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core.Integration
{
    /// <summary>
    /// 场景过渡效果管理器
    /// 提供各种场景切换的过渡动画效果
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {
        private static SceneTransition _instance;
        public static SceneTransition Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SceneTransition>();
                }
                return _instance;
            }
        }

        public enum TransitionType
        {
            Fade,
            Slide,
            Zoom,
            Dissolve,
            Custom
        }

        [SerializeField] private Canvas _transitionCanvas;
        [SerializeField] private Image _transitionImage;
        [SerializeField] private float _defaultTransitionDuration = 0.5f;
        [SerializeField] private Color _defaultTransitionColor = Color.black;

        private TransitionType _currentTransitionType = TransitionType.Fade;
        private float _transitionDuration = 0.5f;
        private Color _transitionColor = Color.black;
        private bool _isTransitioning = false;
        private Action _onTransitionComplete;

        private Material _dissolveMaterial;
        private float _dissolveProgress = 0f;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializeTransitionCanvas();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// 初始化过渡画布
        /// </summary>
        private void InitializeTransitionCanvas()
        {
            if (_transitionCanvas == null)
            {
                var canvasObj = new GameObject("TransitionCanvas");
                canvasObj.transform.SetParent(transform);
                _transitionCanvas = canvasObj.AddComponent<Canvas>();
                _transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _transitionCanvas.sortingOrder = 9999;

                var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);

                var graphicRaycaster = canvasObj.AddComponent<GraphicRaycaster>();

                var imageObj = new GameObject("TransitionImage");
                imageObj.transform.SetParent(canvasObj.transform);
                _transitionImage = imageObj.AddComponent<Image>();
                _transitionImage.color = Color.clear;

                var rectTransform = _transitionImage.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
            }
        }

        /// <summary>
        /// 执行场景过渡
        /// </summary>
        public void DoTransition(TransitionType type, float duration, Color color, Action onComplete)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning("过渡正在进行中");
                return;
            }

            _currentTransitionType = type;
            _transitionDuration = duration > 0 ? duration : _defaultTransitionDuration;
            _transitionColor = color;
            _onTransitionComplete = onComplete;
            _isTransitioning = true;

            StartCoroutine(TransitionCoroutine());
        }

        /// <summary>
        /// 使用默认配置的过渡
        /// </summary>
        public void DoTransition(Action onComplete)
        {
            DoTransition(TransitionType.Fade, _defaultTransitionDuration, _defaultTransitionColor, onComplete);
        }

        /// <summary>
        /// 过渡协程
        /// </summary>
        private IEnumerator TransitionCoroutine()
        {
            yield return StartCoroutine(TransitionIn());
            _onTransitionComplete?.Invoke();
            yield return StartCoroutine(TransitionOut());
            _isTransitioning = false;
        }

        /// <summary>
        /// 过渡进入
        /// </summary>
        private IEnumerator TransitionIn()
        {
            switch (_currentTransitionType)
            {
                case TransitionType.Fade:
                    yield return StartCoroutine(FadeTransition(0f, 1f));
                    break;

                case TransitionType.Slide:
                    yield return StartCoroutine(SlideTransition(1f, 0f));
                    break;

                case TransitionType.Zoom:
                    yield return StartCoroutine(ZoomTransition(1f, 0f));
                    break;

                case TransitionType.Dissolve:
                    yield return StartCoroutine(DissolveTransition(0f, 1f));
                    break;

                case TransitionType.Custom:
                    yield return StartCoroutine(CustomTransition(0f, 1f));
                    break;
            }
        }

        /// <summary>
        /// 过渡退出
        /// </summary>
        private IEnumerator TransitionOut()
        {
            switch (_currentTransitionType)
            {
                case TransitionType.Fade:
                    yield return StartCoroutine(FadeTransition(1f, 0f));
                    break;

                case TransitionType.Slide:
                    yield return StartCoroutine(SlideTransition(0f, -1f));
                    break;

                case TransitionType.Zoom:
                    yield return StartCoroutine(ZoomTransition(0f, 1f));
                    break;

                case TransitionType.Dissolve:
                    yield return StartCoroutine(DissolveTransition(1f, 0f));
                    break;

                case TransitionType.Custom:
                    yield return StartCoroutine(CustomTransition(1f, 0f));
                    break;
            }
        }

        /// <summary>
        /// 淡入淡出过渡
        /// </summary>
        private IEnumerator FadeTransition(float from, float to)
        {
            float elapsed = 0f;
            float duration = _transitionDuration / 2f;

            _transitionImage.color = _transitionColor;
            _transitionImage.enabled = true;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float alpha = Mathf.Lerp(from, to, t);
                _transitionImage.color = new Color(_transitionColor.r, _transitionColor.g, _transitionColor.b, alpha);
                yield return null;
            }

            _transitionImage.color = new Color(_transitionColor.r, _transitionColor.g, _transitionColor.b, to);
            if (to == 0f)
            {
                _transitionImage.enabled = false;
            }
        }

        /// <summary>
        /// 滑动过渡
        /// </summary>
        private IEnumerator SlideTransition(float from, float to)
        {
            float elapsed = 0f;
            float duration = _transitionDuration / 2f;

            var rectTransform = _transitionImage.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(from, 0f);
            rectTransform.anchorMax = new Vector2(from, 1f);
            _transitionImage.enabled = true;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float x = Mathf.Lerp(from, to, t);
                rectTransform.anchorMin = new Vector2(x, 0f);
                rectTransform.anchorMax = new Vector2(x, 1f);
                yield return null;
            }

            rectTransform.anchorMin = new Vector2(to, 0f);
            rectTransform.anchorMax = new Vector2(to, 1f);

            if (to < 0f)
            {
                _transitionImage.enabled = false;
            }
        }

        /// <summary>
        /// 缩放过渡
        /// </summary>
        private IEnumerator ZoomTransition(float from, float to)
        {
            float elapsed = 0f;
            float duration = _transitionDuration / 2f;

            _transitionImage.color = _transitionColor;
            _transitionImage.enabled = true;

            var rectTransform = _transitionImage.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one * from;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float scale = Mathf.Lerp(from, to, t);
                rectTransform.localScale = Vector3.one * scale;
                yield return null;
            }

            rectTransform.localScale = Vector3.one * to;

            if (to == 1f)
            {
                _transitionImage.color = new Color(_transitionColor.r, _transitionColor.g, _transitionColor.b, 1f);
            }
            else
            {
                _transitionImage.enabled = false;
            }
        }

        /// <summary>
        /// 溶解过渡
        /// </summary>
        private IEnumerator DissolveTransition(float from, float to)
        {
            float elapsed = 0f;
            float duration = _transitionDuration / 2f;

            _transitionImage.color = _transitionColor;
            _transitionImage.enabled = true;

            var rectTransform = _transitionImage.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _dissolveProgress = Mathf.Lerp(from, to, elapsed / duration);
                _transitionImage.fillAmount = _dissolveProgress;
                yield return null;
            }

            _dissolveProgress = to;
            _transitionImage.fillAmount = to;

            if (to == 0f)
            {
                _transitionImage.enabled = false;
            }
        }

        /// <summary>
        /// 自定义过渡（子类可重写）
        /// </summary>
        protected virtual IEnumerator CustomTransition(float from, float to)
        {
            yield return FadeTransition(from, to);
        }

        /// <summary>
        /// 设置过渡颜色
        /// </summary>
        public void SetTransitionColor(Color color)
        {
            _defaultTransitionColor = color;
        }

        /// <summary>
        /// 设置过渡时长
        /// </summary>
        public void SetTransitionDuration(float duration)
        {
            _defaultTransitionDuration = duration;
        }

        /// <summary>
        /// 检查是否正在过渡
        /// </summary>
        public bool IsTransitioning()
        {
            return _isTransitioning;
        }
    }
}
