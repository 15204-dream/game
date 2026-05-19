using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UI动画工具类 - 提供常用的UI动画效果
/// </summary>
public class UITween : MonoBehaviour
{
    #region 淡入淡出动画
    
    /// <summary>
    /// CanvasGroup淡入淡出
    /// </summary>
    public static IEnumerator FadeCanvasGroup(GameObject target, float from, float to, 
                                             float duration, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.AddComponent<CanvasGroup>();
        }
        
        float elapsed = 0f;
        canvasGroup.alpha = from;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        
        canvasGroup.alpha = to;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// Image颜色淡入淡出
    /// </summary>
    public static IEnumerator FadeImage(GameObject target, Color from, Color to, 
                                        float duration, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Image image = target.GetComponent<Image>();
        if (image == null) yield break;
        
        float elapsed = 0f;
        image.color = from;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            image.color = Color.Lerp(from, to, t);
            yield return null;
        }
        
        image.color = to;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// Text颜色淡入淡出
    /// </summary>
    public static IEnumerator FadeText(GameObject target, Color from, Color to, 
                                      float duration, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Text text = target.GetComponent<Text>();
        if (text == null) yield break;
        
        float elapsed = 0f;
        text.color = from;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            text.color = Color.Lerp(from, to, t);
            yield return null;
        }
        
        text.color = to;
        onComplete?.Invoke();
    }
    
    #endregion
    
    #region 位移动画
    
    /// <summary>
    /// 移动到目标位置
    /// </summary>
    public static IEnumerator MoveTo(GameObject target, Vector3 targetPosition, 
                                    float duration, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Vector3 startPosition = target.transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            target.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, smoothT);
            yield return null;
        }
        
        target.transform.localPosition = targetPosition;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 移动到目标位置（使用曲线）
    /// </summary>
    public static IEnumerator MoveTo(GameObject target, Vector3 targetPosition, 
                                    float duration, AnimationCurve curve,
                                    System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Vector3 startPosition = target.transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = curve.Evaluate(t);
            
            target.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);
            yield return null;
        }
        
        target.transform.localPosition = targetPosition;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 移动到目标位置（RectTransform）
    /// </summary>
    public static IEnumerator MoveToRect(GameObject target, Vector2 targetAnchoredPosition,
                                         float duration, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null) yield break;
        
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetAnchoredPosition, smoothT);
            yield return null;
        }
        
        rectTransform.anchoredPosition = targetAnchoredPosition;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 水平滑动
    /// </summary>
    public static IEnumerator SlideHorizontal(GameObject target, float targetX, 
                                              float duration, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Vector3 startPosition = target.transform.localPosition;
        Vector3 targetPosition = new Vector3(targetX, startPosition.y, startPosition.z);
        
        yield return MoveTo(target, targetPosition, duration, onComplete);
    }
    
    /// <summary>
    /// 垂直滑动
    /// </summary>
    public static IEnumerator SlideVertical(GameObject target, float targetY, 
                                            float duration, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Vector3 startPosition = target.transform.localPosition;
        Vector3 targetPosition = new Vector3(startPosition.x, targetY, startPosition.z);
        
        yield return MoveTo(target, targetPosition, duration, onComplete);
    }
    
    #endregion
    
    #region 缩放动画
    
    /// <summary>
    /// 缩放到目标大小
    /// </summary>
    public static IEnumerator ScaleTo(GameObject target, Vector3 targetScale, 
                                     float duration, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Vector3 startScale = target.transform.localScale;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            target.transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);
            yield return null;
        }
        
        target.transform.localScale = targetScale;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 缩放到目标大小（使用曲线）
    /// </summary>
    public static IEnumerator ScaleTo(GameObject target, Vector3 targetScale, 
                                     float duration, AnimationCurve curve,
                                     System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Vector3 startScale = target.transform.localScale;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = curve.Evaluate(t);
            
            target.transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
            yield return null;
        }
        
        target.transform.localScale = targetScale;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 弹跳效果
    /// </summary>
    public static IEnumerator Bounce(GameObject target, float duration = 0.5f, 
                                   System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Vector3 originalScale = target.transform.localScale;
        Vector3 overshootScale = originalScale * 1.2f;
        float elapsed = 0f;
        
        // 放大
        while (elapsed < duration * 0.3f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.3f);
            target.transform.localScale = Vector3.Lerp(originalScale, overshootScale, t);
            yield return null;
        }
        
        // 回弹
        elapsed = 0f;
        while (elapsed < duration * 0.7f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.7f);
            float bounceT = 1f - Mathf.Pow(1f - t, 3f); // Cubic ease out
            target.transform.localScale = Vector3.Lerp(overshootScale, originalScale, bounceT);
            yield return null;
        }
        
        target.transform.localScale = originalScale;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 脉冲效果
    /// </summary>
    public static IEnumerator Pulse(GameObject target, Vector3 pulseScale, float duration = 0.3f,
                                   int pulseCount = 1, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Vector3 originalScale = target.transform.localScale;
        
        for (int i = 0; i < pulseCount; i++)
        {
            float elapsed = 0f;
            
            // 放大
            while (elapsed < duration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.5f);
                target.transform.localScale = Vector3.Lerp(originalScale, pulseScale, t);
                yield return null;
            }
            
            // 缩小
            elapsed = 0f;
            while (elapsed < duration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.5f);
                target.transform.localScale = Vector3.Lerp(pulseScale, originalScale, t);
                yield return null;
            }
        }
        
        target.transform.localScale = originalScale;
        onComplete?.Invoke();
    }
    
    #endregion
    
    #region 旋转动画
    
    /// <summary>
    /// 旋转动画
    /// </summary>
    public static IEnumerator Rotate(GameObject target, float targetZ, float duration,
                                    bool clockwise = true, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        float startZ = target.transform.localEulerAngles.z;
        float endZ = targetZ;
        
        if (!clockwise && endZ < startZ)
        {
            endZ += 360f;
        }
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentZ = Mathf.Lerp(startZ, endZ, t);
            target.transform.localEulerAngles = new Vector3(0, 0, currentZ);
            yield return null;
        }
        
        target.transform.localEulerAngles = new Vector3(0, 0, endZ);
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 持续旋转
    /// </summary>
    public static IEnumerator Spin(GameObject target, float speed, float duration,
                                  System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        float elapsed = 0f;
        float startZ = target.transform.localEulerAngles.z;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float rotation = startZ + (elapsed * speed);
            target.transform.localEulerAngles = new Vector3(0, 0, rotation);
            yield return null;
        }
        
        onComplete?.Invoke();
    }
    
    #endregion
    
    #region 组合动画
    
    /// <summary>
    /// 弹窗效果（缩放+淡入）
    /// </summary>
    public static IEnumerator PopupIn(GameObject target, float duration = 0.3f, 
                                     System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.AddComponent<CanvasGroup>();
        }
        
        Vector3 originalScale = target.transform.localScale;
        target.transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;
        
        float elapsed = 0f;
        AnimationCurve curve = AnimationCurve.EaseOutBack(0f, 0f, 1f, 1f);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = curve.Evaluate(t);
            
            target.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, curveValue);
            canvasGroup.alpha = t;
            yield return null;
        }
        
        target.transform.localScale = originalScale;
        canvasGroup.alpha = 1f;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 弹窗关闭效果（缩放+淡出）
    /// </summary>
    public static IEnumerator PopupOut(GameObject target, float duration = 0.2f,
                                      System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        Vector3 originalScale = target.transform.localScale;
        float startAlpha = canvasGroup != null ? canvasGroup.alpha : 1f;
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            target.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            }
            yield return null;
        }
        
        target.transform.localScale = Vector3.zero;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 滑入动画
    /// </summary>
    public static IEnumerator SlideIn(GameObject target, Vector3 startOffset, 
                                    float duration = 0.3f, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null) yield break;
        
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 targetPosition = startPosition - (Vector2)startOffset;
        
        // 设置起始位置
        rectTransform.anchoredPosition = startPosition + (Vector2)startOffset;
        
        yield return MoveToRect(target, targetPosition, duration, onComplete);
    }
    
    /// <summary>
    /// 滑出动画
    /// </summary>
    public static IEnumerator SlideOut(GameObject target, Vector3 endOffset,
                                     float duration = 0.3f, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null) yield break;
        
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = startPosition + (Vector2)endOffset;
        
        yield return MoveToRect(target, endPosition, duration, onComplete);
    }
    
    #endregion
    
    #region 特殊效果
    
    /// <summary>
    /// 震动效果
    /// </summary>
    public static IEnumerator Shake(GameObject target, float intensity = 5f, 
                                   float duration = 0.3f, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Vector3 originalPosition = target.transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * intensity * (1f - elapsed / duration);
            float y = Random.Range(-1f, 1f) * intensity * (1f - elapsed / duration);
            
            target.transform.localPosition = originalPosition + new Vector3(x, y, 0f);
            yield return null;
        }
        
        target.transform.localPosition = originalPosition;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 心跳效果
    /// </summary>
    public static IEnumerator Heartbeat(GameObject target, float duration = 1f,
                                       System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        Vector3 originalScale = target.transform.localScale;
        float elapsed = 0f;
        float beatTime = 0.15f;
        int beatCount = 3;
        
        for (int i = 0; i < beatCount; i++)
        {
            float beatElapsed = 0f;
            
            // 放大
            while (beatElapsed < beatTime)
            {
                beatElapsed += Time.deltaTime;
                float t = beatElapsed / beatTime;
                float scale = Mathf.Lerp(originalScale.x, originalScale.x * 1.3f, t);
                target.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }
            
            // 缩小
            beatElapsed = 0f;
            while (beatElapsed < beatTime)
            {
                beatElapsed += Time.deltaTime;
                float t = beatElapsed / beatTime;
                float scale = Mathf.Lerp(originalScale.x * 1.3f, originalScale.x, t);
                target.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }
            
            // 间隔
            float pauseTime = beatTime;
            float pauseElapsed = 0f;
            while (pauseElapsed < pauseTime)
            {
                pauseElapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        target.transform.localScale = originalScale;
        elapsed += beatCount * (beatTime * 2 + beatTime);
        
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// 闪烁效果
    /// </summary>
    public static IEnumerator Blink(GameObject target, float duration = 1f, 
                                   float blinkSpeed = 0.1f, System.Action onComplete = null)
    {
        if (target == null) yield break;
        
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.AddComponent<CanvasGroup>();
        }
        
        float elapsed = 0f;
        bool visible = true;
        
        while (elapsed < duration)
        {
            elapsed += blinkSpeed;
            visible = !visible;
            canvasGroup.alpha = visible ? 1f : 0f;
            yield return new WaitForSeconds(blinkSpeed);
        }
        
        canvasGroup.alpha = 1f;
        onComplete?.Invoke();
    }
    
    #endregion
    
    #region 缓动曲线
    
    /// <summary>
    /// 获取常用缓动曲线
    /// </summary>
    public static class Curves
    {
        public static AnimationCurve EaseOutBack => AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        public static AnimationCurve EaseInOut
        {
            get
            {
                return new AnimationCurve(
                    new Keyframe(0f, 0f, 0f, 0f),
                    new Keyframe(0.5f, 0.5f, 1f, 1f),
                    new Keyframe(1f, 1f, 0f, 0f)
                );
            }
        }
        
        public static AnimationCurve Bounce
        {
            get
            {
                return new AnimationCurve(
                    new Keyframe(0f, 0f, 0f, 0f),
                    new Keyframe(0.2f, 0.8f, 1f, 1f),
                    new Keyframe(0.4f, 1.1f, -1f, -1f),
                    new Keyframe(0.6f, 0.95f, 1f, 1f),
                    new Keyframe(0.8f, 1.02f, -1f, -1f),
                    new Keyframe(1f, 1f, 0f, 0f)
                );
            }
        }
        
        public static AnimationCurve Punch
        {
            get
            {
                return new AnimationCurve(
                    new Keyframe(0f, 0f, 6.5f, 6.5f),
                    new Keyframe(0.2f, 1.1f, 0f, 0f),
                    new Keyframe(0.3f, 1f, 0f, 0f),
                    new Keyframe(0.4f, 1.03f, 0f, 0f),
                    new Keyframe(0.5f, 1f, 0f, 0f),
                    new Keyframe(0.55f, 1.01f, 0f, 0f),
                    new Keyframe(0.6f, 1f, -1f, -1f),
                    new Keyframe(0.75f, 0.9f, 0f, 0f),
                    new Keyframe(0.85f, 0.95f, 0f, 0f),
                    new Keyframe(1f, 1f, 0f, 0f)
                );
            }
        }
    }
    
    #endregion
}
