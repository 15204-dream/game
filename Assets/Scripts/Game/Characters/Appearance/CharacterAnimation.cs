using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Appearance
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimation : MonoBehaviour
    {
        [Header("动画配置")]
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private RuntimeAnimatorController animatorController;

        [Header("动画参数")]
        [SerializeField] private float animationSpeed = 1f;
        [SerializeField] private float transitionDuration = 0.3f;

        [Header("动画状态映射")]
        [SerializeField] private List<AnimationStateMapping> stateMappings = new List<AnimationStateMapping>();

        [Header("特殊动画")]
        [SerializeField] private bool enableIK = false;
        [SerializeField] private Transform lookAtTarget;
        [SerializeField] private float lookAtWeight = 1f;

        [Header("当前状态")]
        [SerializeField] private string currentAnimationState = "Idle";
        [SerializeField] private float currentAnimationTime = 0f;

        [Serializable]
        public class AnimationStateMapping
        {
            public CharacterState characterState;
            public string animationStateName;
            public float transitionTime = 0.3f;
            public bool loop = true;
        }

        private Dictionary<CharacterState, AnimationStateMapping> stateDict = new Dictionary<CharacterState, AnimationStateMapping>();
        private bool isAnimating = true;
        private float animationTimeScale = 1f;

        public event Action<string> OnAnimationStateChanged;

        protected virtual void Awake()
        {
            InitializeAnimator();
            InitializeStateMappings();
        }

        protected virtual void Update()
        {
            if (!isAnimating) return;
            UpdateAnimation();
        }

        private void InitializeAnimator()
        {
            if (characterAnimator == null)
            {
                characterAnimator = GetComponent<Animator>();
            }

            if (animatorController != null)
            {
                characterAnimator.runtimeAnimatorController = animatorController;
            }

            characterAnimator.speed = animationSpeed;
        }

        private void InitializeStateMappings()
        {
            stateDict.Clear();

            if (stateMappings == null || stateMappings.Count == 0)
            {
                stateMappings = CreateDefaultMappings();
            }

            foreach (var mapping in stateMappings)
            {
                stateDict[mapping.characterState] = mapping;
            }
        }

        private List<AnimationStateMapping> CreateDefaultMappings()
        {
            return new List<AnimationStateMapping>
            {
                new AnimationStateMapping { characterState = CharacterState.Idle, animationStateName = "Idle", loop = true },
                new AnimationStateMapping { characterState = CharacterState.Speaking, animationStateName = "Speaking", loop = true },
                new AnimationStateMapping { characterState = CharacterState.Listening, animationStateName = "Listening", loop = true },
                new AnimationStateMapping { characterState = CharacterState.Happy, animationStateName = "Happy", loop = true },
                new AnimationStateMapping { characterState = CharacterState.Angry, animationStateName = "Angry", loop = true },
                new AnimationStateMapping { characterState = CharacterState.Sad, animationStateName = "Sad", loop = true },
                new AnimationStateMapping { characterState = CharacterState.Surprised, animationStateName = "Surprised", loop = false },
                new AnimationStateMapping { characterState = CharacterState.Thinking, animationStateName = "Thinking", loop = true },
                new AnimationStateMapping { characterState = CharacterState.Flustered, animationStateName = "Flustered", loop = true },
                new AnimationStateMapping { characterState = CharacterState.Confident, animationStateName = "Confident", loop = true },
                new AnimationStateMapping { characterState = CharacterState.Nervous, animationStateName = "Nervous", loop = true }
            };
        }

        private void UpdateAnimation()
        {
            currentAnimationTime += Time.deltaTime * animationTimeScale;
        }

        public virtual void PlayAnimation(CharacterState state)
        {
            if (!stateDict.ContainsKey(state))
            {
                Debug.LogWarning($"未找到动画映射: {state}");
                return;
            }

            AnimationStateMapping mapping = stateDict[state];
            PlayAnimationState(mapping.animationStateName, mapping.transitionTime, mapping.loop);
        }

        public virtual void PlayAnimationState(string stateName, float transitionTime = 0.3f, bool loop = true)
        {
            if (characterAnimator == null || string.IsNullOrEmpty(stateName)) return;

            if (currentAnimationState == stateName) return;

            currentAnimationState = stateName;
            currentAnimationTime = 0f;

            characterAnimator.CrossFade(stateName, transitionTime);

            if (!loop)
            {
                StartCoroutine(WaitForAnimationEnd(stateName));
            }

            OnAnimationStateChanged?.Invoke(stateName);
        }

        private System.Collections.IEnumerator WaitForAnimationEnd(string stateName)
        {
            yield return new WaitForSeconds(1f);
            if (currentAnimationState == stateName)
            {
                PlayAnimation(CharacterState.Idle);
            }
        }

        public virtual void PlayOneShotAnimation(string stateName, float transitionTime = 0f)
        {
            if (characterAnimator == null) return;

            AnimatorStateInfo stateInfo = characterAnimator.GetCurrentAnimatorStateInfo(0);
            characterAnimator.Play(stateName, 0, transitionTime);
        }

        public virtual void SetAnimationSpeed(float speed)
        {
            animationSpeed = Mathf.Clamp01(speed);
            if (characterAnimator != null)
            {
                characterAnimator.speed = animationSpeed * animationTimeScale;
            }
        }

        public virtual void SetTimeScale(float timeScale)
        {
            animationTimeScale = Mathf.Clamp01(timeScale);
            if (characterAnimator != null)
            {
                characterAnimator.speed = animationSpeed * animationTimeScale;
            }
        }

        public virtual void PauseAnimation()
        {
            isAnimating = false;
            if (characterAnimator != null)
            {
                characterAnimator.speed = 0f;
            }
        }

        public virtual void ResumeAnimation()
        {
            isAnimating = true;
            if (characterAnimator != null)
            {
                characterAnimator.speed = animationSpeed * animationTimeScale;
            }
        }

        public virtual void TriggerAnimationEvent(string eventName)
        {
            characterAnimator.SetTrigger(eventName);
        }

        public virtual void SetAnimationBool(string paramName, bool value)
        {
            if (characterAnimator != null)
            {
                characterAnimator.SetBool(paramName, value);
            }
        }

        public virtual void SetAnimationFloat(string paramName, float value)
        {
            if (characterAnimator != null)
            {
                characterAnimator.SetFloat(paramName, value);
            }
        }

        public virtual void SetAnimationInt(string paramName, int value)
        {
            if (characterAnimator != null)
            {
                characterAnimator.SetInteger(paramName, value);
            }
        }

        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if (!enableIK || characterAnimator == null) return;

            if (lookAtTarget != null)
            {
                characterAnimator.SetLookAtWeight(lookAtWeight);
                characterAnimator.SetLookAtPosition(lookAtTarget.position);
            }
            else
            {
                characterAnimator.SetLookAtWeight(0f);
            }
        }

        public virtual void SetLookAtTarget(Transform target, float weight = 1f)
        {
            lookAtTarget = target;
            lookAtWeight = Mathf.Clamp01(weight);
            enableIK = true;
        }

        public virtual void ClearLookAtTarget()
        {
            lookAtTarget = null;
            enableIK = false;
        }

        public virtual void PlayRandomIdleAnimation()
        {
            string[] idleAnimations = { "Idle1", "Idle2", "Idle3" };
            string randomIdle = idleAnimations[UnityEngine.Random.Range(0, idleAnimations.Length)];
            PlayAnimationState(randomIdle, 0.3f, true);
        }

        public virtual void PlayReactionAnimation(string reactionType)
        {
            string reactionState = $"Reaction_{reactionType}";
            PlayAnimationState(reactionState, 0.2f, false);
        }

        public virtual void PlayEmotionAnimation(CharacterState emotion)
        {
            switch (emotion)
            {
                case CharacterState.Happy:
                    PlayAnimationState("Emotion_Happy", 0.3f, false);
                    break;
                case CharacterState.Surprised:
                    PlayAnimationState("Emotion_Surprised", 0.2f, false);
                    break;
                case CharacterState.Angry:
                    PlayAnimationState("Emotion_Angry", 0.3f, false);
                    break;
                case CharacterState.Sad:
                    PlayAnimationState("Emotion_Sad", 0.3f, false);
                    break;
                case CharacterState.Flustered:
                    PlayAnimationState("Emotion_Flustered", 0.2f, false);
                    break;
                default:
                    PlayAnimation(emotion);
                    break;
            }
        }

        public virtual void AddAnimationMapping(CharacterState state, string animationName, float transitionTime = 0.3f, bool loop = true)
        {
            AnimationStateMapping mapping = new AnimationStateMapping
            {
                characterState = state,
                animationStateName = animationName,
                transitionTime = transitionTime,
                loop = loop
            };

            stateMappings.Add(mapping);
            stateDict[state] = mapping;
        }

        public virtual string GetCurrentAnimationState()
        {
            return currentAnimationState;
        }

        public virtual float GetAnimationProgress()
        {
            if (characterAnimator != null)
            {
                AnimatorStateInfo stateInfo = characterAnimator.GetCurrentAnimatorStateInfo(0);
                return stateInfo.normalizedTime;
            }
            return 0f;
        }

        #region 属性访问器

        public Animator CharacterAnimator => characterAnimator;
        public float AnimationSpeed => animationSpeed;
        public bool IsAnimating => isAnimating;
        public string CurrentAnimationState => currentAnimationState;

        #endregion
    }
}
