using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Appearance
{
    [Serializable]
    public class ExpressionData
    {
        public CharacterState state;
        public float eyeOpenness = 1f;
        public float mouthOpenness = 0f;
        public float eyebrowAngle = 0f;
        public float cheekRedness = 0f;
        public float blushIntensity = 0f;
        public string description;
    }

    public class CharacterExpression : MonoBehaviour
    {
        [Header("表情数据")]
        [SerializeField] private CharacterState currentExpression = CharacterState.Idle;
        [SerializeField] private ExpressionData[] expressionProfiles;

        [Header("表情强度")]
        [Range(0f, 1f)]
        [SerializeField] private float expressionIntensity = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float transitionSpeed = 0.3f;

        [Header("表情动画组件")]
        [SerializeField] private SkinnedMeshRenderer faceMeshRenderer;
        [SerializeField] private int eyeOpenBlendShape = 0;
        [SerializeField] private int mouthOpenBlendShape = 1;
        [SerializeField] private int eyebrowBlendShape = 2;
        [SerializeField] private int blushBlendShape = 3;

        [Header("当前值")]
        [SerializeField] private float currentEyeOpenness = 1f;
        [SerializeField] private float currentMouthOpenness = 0f;
        [SerializeField] private float currentEyebrowAngle = 0f;
        [SerializeField] private float currentCheekRedness = 0f;
        [SerializeField] private float currentBlushIntensity = 0f;

        private Dictionary<CharacterState, ExpressionData> expressionDict = new Dictionary<CharacterState, ExpressionData>();
        private float targetEyeOpenness = 1f;
        private float targetMouthOpenness = 0f;
        private float targetEyebrowAngle = 0f;
        private float targetCheekRedness = 0f;
        private float targetBlushIntensity = 0f;

        public event Action<CharacterState> OnExpressionChanged;

        protected virtual void Awake()
        {
            InitializeExpressionProfiles();
            InitializeFaceMesh();
        }

        protected virtual void Update()
        {
            UpdateExpressionTransition();
        }

        private void InitializeExpressionProfiles()
        {
            expressionDict.Clear();

            if (expressionProfiles == null || expressionProfiles.Length == 0)
            {
                expressionProfiles = CreateDefaultExpressions();
            }

            foreach (var profile in expressionProfiles)
            {
                expressionDict[profile.state] = profile;
            }
        }

        private ExpressionData[] CreateDefaultExpressions()
        {
            return new ExpressionData[]
            {
                new ExpressionData
                {
                    state = CharacterState.Idle,
                    eyeOpenness = 1f,
                    mouthOpenness = 0f,
                    eyebrowAngle = 0f,
                    cheekRedness = 0f,
                    blushIntensity = 0f,
                    description = "平静的表情"
                },
                new ExpressionData
                {
                    state = CharacterState.Happy,
                    eyeOpenness = 0.8f,
                    mouthOpenness = 0.3f,
                    eyebrowAngle = 0f,
                    cheekRedness = 0.3f,
                    blushIntensity = 0.2f,
                    description = "开心的笑容"
                },
                new ExpressionData
                {
                    state = CharacterState.Speaking,
                    eyeOpenness = 1f,
                    mouthOpenness = 0.5f,
                    eyebrowAngle = 0f,
                    cheekRedness = 0f,
                    blushIntensity = 0f,
                    description = "说话中"
                },
                new ExpressionData
                {
                    state = CharacterState.Listening,
                    eyeOpenness = 0.9f,
                    mouthOpenness = 0f,
                    eyebrowAngle = 0.1f,
                    cheekRedness = 0f,
                    blushIntensity = 0f,
                    description = "认真倾听"
                },
                new ExpressionData
                {
                    state = CharacterState.Surprised,
                    eyeOpenness = 1.2f,
                    mouthOpenness = 0.6f,
                    eyebrowAngle = 0.3f,
                    cheekRedness = 0f,
                    blushIntensity = 0f,
                    description = "惊讶的表情"
                },
                new ExpressionData
                {
                    state = CharacterState.Angry,
                    eyeOpenness = 0.9f,
                    mouthOpenness = 0f,
                    eyebrowAngle = -0.3f,
                    cheekRedness = 0f,
                    blushIntensity = 0f,
                    description = "生气的表情"
                },
                new ExpressionData
                {
                    state = CharacterState.Sad,
                    eyeOpenness = 0.7f,
                    mouthOpenness = 0.2f,
                    eyebrowAngle = 0.2f,
                    cheekRedness = 0f,
                    blushIntensity = 0f,
                    description = "难过的表情"
                },
                new ExpressionData
                {
                    state = CharacterState.Flustered,
                    eyeOpenness = 0.85f,
                    mouthOpenness = 0.1f,
                    eyebrowAngle = 0.15f,
                    cheekRedness = 0.8f,
                    blushIntensity = 0.9f,
                    description = "害羞脸红"
                },
                new ExpressionData
                {
                    state = CharacterState.Confident,
                    eyeOpenness = 0.95f,
                    mouthOpenness = 0.15f,
                    eyebrowAngle = -0.1f,
                    cheekRedness = 0.2f,
                    blushIntensity = 0.1f,
                    description = "自信的表情"
                },
                new ExpressionData
                {
                    state = CharacterState.Nervous,
                    eyeOpenness = 1.1f,
                    mouthOpenness = 0.1f,
                    eyebrowAngle = 0.25f,
                    cheekRedness = 0.4f,
                    blushIntensity = 0.3f,
                    description = "紧张的表情"
                },
                new ExpressionData
                {
                    state = CharacterState.Thinking,
                    eyeOpenness = 0.75f,
                    mouthOpenness = 0f,
                    eyebrowAngle = 0.15f,
                    cheekRedness = 0f,
                    blushIntensity = 0f,
                    description = "思考的表情"
                }
            };
        }

        private void InitializeFaceMesh()
        {
            if (faceMeshRenderer == null)
            {
                faceMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }

        public virtual void SetExpression(CharacterState newState)
        {
            if (currentExpression == newState) return;

            CharacterState oldState = currentExpression;
            currentExpression = newState;

            if (expressionDict.ContainsKey(newState))
            {
                ExpressionData data = expressionDict[newState];
                SetExpressionTargets(data);
            }

            OnExpressionChanged?.Invoke(newState);
        }

        private void SetExpressionTargets(ExpressionData data)
        {
            targetEyeOpenness = data.eyeOpenness;
            targetMouthOpenness = data.mouthOpenness;
            targetEyebrowAngle = data.eyebrowAngle;
            targetCheekRedness = data.cheekRedness;
            targetBlushIntensity = data.blushIntensity;
        }

        private void UpdateExpressionTransition()
        {
            float t = Time.deltaTime / transitionSpeed;
            t = Mathf.Clamp01(t);

            currentEyeOpenness = Mathf.Lerp(currentEyeOpenness, targetEyeOpenness * expressionIntensity, t);
            currentMouthOpenness = Mathf.Lerp(currentMouthOpenness, targetMouthOpenness * expressionIntensity, t);
            currentEyebrowAngle = Mathf.Lerp(currentEyebrowAngle, targetEyebrowAngle * expressionIntensity, t);
            currentCheekRedness = Mathf.Lerp(currentCheekRedness, targetCheekRedness * expressionIntensity, t);
            currentBlushIntensity = Mathf.Lerp(currentBlushIntensity, targetBlushIntensity * expressionIntensity, t);

            ApplyBlendShapes();
        }

        protected virtual void ApplyBlendShapes()
        {
            if (faceMeshRenderer == null) return;

            if (faceMeshRenderer.sharedMesh != null)
            {
                int blendShapeCount = faceMeshRenderer.sharedMesh.blendShapeCount;
                if (blendShapeCount > 0)
                {
                    if (eyeOpenBlendShape < blendShapeCount)
                        faceMeshRenderer.SetBlendShapeWeight(eyeOpenBlendShape, currentEyeOpenness * 100f);
                    if (mouthOpenBlendShape < blendShapeCount)
                        faceMeshRenderer.SetBlendShapeWeight(mouthOpenBlendShape, currentMouthOpenness * 100f);
                    if (eyebrowBlendShape < blendShapeCount)
                        faceMeshRenderer.SetBlendShapeWeight(eyebrowBlendShape, currentEyebrowAngle * 100f);
                    if (blushBlendShape < blendShapeCount)
                        faceMeshRenderer.SetBlendShapeWeight(blushBlendShape, currentBlushIntensity * 100f);
                }
            }
        }

        public virtual void SetExpressionIntensity(float intensity)
        {
            expressionIntensity = Mathf.Clamp01(intensity);
            SetExpressionTargets(expressionDict.ContainsKey(currentExpression) 
                ? expressionDict[currentExpression] 
                : expressionDict[CharacterState.Idle]);
        }

        public virtual void Blink(float duration = 0.1f)
        {
            StartCoroutine(BlinkCoroutine(duration));
        }

        private System.Collections.IEnumerator BlinkCoroutine(float duration)
        {
            float originalEyeOpenness = currentEyeOpenness;
            currentEyeOpenness = 0f;
            yield return new WaitForSeconds(duration);
            currentEyeOpenness = originalEyeOpenness;
        }

        public virtual void TriggerEmotion(CharacterState emotionState, float duration = 2f)
        {
            SetExpression(emotionState);
            Invoke(nameof(ResetToIdle), duration);
        }

        private void ResetToIdle()
        {
            SetExpression(CharacterState.Idle);
        }

        public virtual string GetCurrentExpressionDescription()
        {
            if (expressionDict.ContainsKey(currentExpression))
            {
                return expressionDict[currentExpression].description;
            }
            return "无表情";
        }

        #region 属性访问器

        public CharacterState CurrentExpression => currentExpression;
        public float ExpressionIntensity => expressionIntensity;
        public float TransitionSpeed => transitionSpeed;

        #endregion

        public virtual void AddCustomExpression(CharacterState state, ExpressionData data)
        {
            expressionDict[state] = data;
        }

        public virtual ExpressionData GetExpressionData(CharacterState state)
        {
            if (expressionDict.ContainsKey(state))
            {
                return expressionDict[state];
            }
            return null;
        }
    }
}
